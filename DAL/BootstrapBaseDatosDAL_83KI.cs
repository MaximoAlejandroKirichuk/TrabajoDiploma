using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DAL.interfaces;
using Microsoft.Win32;
using Service.DTOs;

namespace DAL
{
    /// <summary>
    /// implementacion dal para descubrimiento de instancias SQL Server y validacion de conectividad.
    /// usa SqlDataSourceEnumerator para detectar instancias locales y abre una conexion con Integrated Security
    /// para validar que la base de datos existe en la instancia especificada.
    /// </summary>
    public class BootstrapBaseDatosDAL_83KI : IBootstrapBaseDatosDAL_83KI
    {
        public IReadOnlyList<string> DescubrirInstancias()
        {
            var instances = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            // Estrategia 1: SqlDataSourceEnumerator (deteccion por broadcast)
            try
            {
                DataTable dt = SqlDataSourceEnumerator.Instance.GetDataSources();
                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        string serverName = row["ServerName"]?.ToString() ?? string.Empty;
                        string instanceName = row["InstanceName"]?.ToString();

                        if (!string.IsNullOrWhiteSpace(instanceName))
                            instances.Add($"{serverName}\\{instanceName}");
                        else if (!string.IsNullOrWhiteSpace(serverName))
                            instances.Add(serverName);
                    }
                }
            }
            catch
            {
                // La estrategia 1 puede fallar silenciosamente — continuar con otras estrategias
            }

            // Estrategia 2: busqueda en registro para instancias nombradas
            try
            {
                using (var key = Registry.LocalMachine.OpenSubKey(
                    @"SOFTWARE\Microsoft\Microsoft SQL Server\Instance Names\SQL"))
                {
                    if (key != null)
                    {
                        foreach (string instanceName in key.GetValueNames())
                        {
                            if (!string.IsNullOrWhiteSpace(instanceName))
                                instances.Add($"{Environment.MachineName}\\{instanceName}");
                        }
                    }
                }
            }
            catch
            {
                // La estrategia 2 puede fallar silenciosamente — continuar con otras estrategias
            }

            // Estrategia 3: comando sqllocaldb (instancias LocalDB)
            try
            {
                var psi = new ProcessStartInfo("sqllocaldb", "info")
                {
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (var process = Process.Start(psi))
                {
                    string output = process.StandardOutput.ReadToEnd();
                    process.WaitForExit(3000);

                    if (!string.IsNullOrWhiteSpace(output))
                    {
                        foreach (string line in output.Split('\n'))
                        {
                            string name = line.Trim();
                            if (!string.IsNullOrEmpty(name))
                                instances.Add($"(localdb)\\{name}");
                        }
                    }
                }
            }
            catch
            {
                // La estrategia 3 puede fallar silenciosamente — continuar con otras estrategias
            }

            return instances.ToList().AsReadOnly();
        }

        public (bool success, string message) ValidarConexion(ConfiguracionConexionBD_83KI settings)
        {
            if (settings == null)
                return (false, "No se proporciono configuracion de conexion.");

            if (string.IsNullOrWhiteSpace(settings.InstanciaServidor))
                return (false, "El nombre de la instancia del servidor es obligatorio.");

            if (string.IsNullOrWhiteSpace(settings.NombreBaseDatos))
                return (false, "El nombre de la base de datos es obligatorio.");

            string connectionString =
                $"Data Source={settings.InstanciaServidor};" +
                $"Initial Catalog={settings.NombreBaseDatos};" +
                "Integrated Security=True;";

            // 1. validar conectividad a la instancia (via master)
            string masterConnectionString =
                $"Data Source={settings.InstanciaServidor};" +
                "Initial Catalog=master;" +
                "Integrated Security=True;";

            try
            {
                using (var conn = new SqlConnection(masterConnectionString))
                {
                    conn.Open();
                }
            }
            catch (SqlException ex)
            {
                return (false, $"No se pudo conectar a la instancia '{settings.InstanciaServidor}': {ex.Message}");
            }

            // 2. verificar que la base de datos existe
            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    using (var cmd = new SqlCommand("SELECT DB_ID(@dbName)", conn))
                    {
                        cmd.Parameters.AddWithValue("@dbName", settings.NombreBaseDatos);
                        object result = cmd.ExecuteScalar();

                        if (result == null || result == DBNull.Value)
                            return (false, $"La base de datos '{settings.NombreBaseDatos}' no existe en la instancia '{settings.InstanciaServidor}'. Debe crearla manualmente antes de continuar.");
                    }
                }
            }
            catch (SqlException ex)
            {
                return (false, $"Error al verificar la base de datos '{settings.NombreBaseDatos}': {ex.Message}");
            }

            return (true, "Conexion validada correctamente.");
        }

        public (bool success, string message) InstalarBaseDatos(string serverInstance, string databaseName)
        {
            if (string.IsNullOrWhiteSpace(serverInstance))
                return (false, "El nombre de la instancia del servidor es obligatorio.");

            if (string.IsNullOrWhiteSpace(databaseName))
                return (false, "El nombre de la base de datos es obligatorio.");

            // Resolver ruta del script SQL relativa al directorio del ejecutable
            string scriptPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Scripts", "Sql", "EsquemaCompleto.sql");

            if (!File.Exists(scriptPath))
                return (false, $"No se encontro el script de instalacion en '{scriptPath}'. Verifique la instalacion de la aplicacion.");

            string scriptContent;
            try
            {
                scriptContent = File.ReadAllText(scriptPath);
            }
            catch (Exception ex)
            {
                return (false, $"Error al leer el script de instalacion: {ex.Message}");
            }

            // Reemplazar el nombre de base de datos fijo por el provisto por el usuario
            scriptContent = scriptContent.Replace("GestionUsuarios", databaseName);

            // Dividir por separadores de lote GO (lineas que contienen solo "GO")
            string[] batches = Regex.Split(
                scriptContent,
                @"^\s*GO\s*$",
                RegexOptions.Multiline | RegexOptions.IgnoreCase);

            string masterConnectionString =
                $"Data Source={serverInstance};" +
                "Initial Catalog=master;" +
                "Integrated Security=True;";

            try
            {
                using (var conn = new SqlConnection(masterConnectionString))
                {
                    conn.Open();

                    foreach (string batch in batches)
                    {
                        string trimmedBatch = batch.Trim();
                        if (string.IsNullOrEmpty(trimmedBatch))
                            continue;

                        using (var cmd = new SqlCommand(trimmedBatch, conn))
                        {
                            cmd.CommandTimeout = 60; // dar tiempo suficiente a las operaciones DDL
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                return (false, $"Error al ejecutar el script de instalacion: {ex.Message}");
            }

            // Verificar que la base de datos se creo correctamente
            string targetConnectionString =
                $"Data Source={serverInstance};" +
                $"Initial Catalog={databaseName};" +
                "Integrated Security=True;";

            try
            {
                using (var conn = new SqlConnection(targetConnectionString))
                {
                    conn.Open();
                }
            }
            catch (SqlException ex)
            {
                return (false, $"La base de datos se creo pero no se pudo conectar: {ex.Message}");
            }

            return (true, $"Base de datos '{databaseName}' instalada correctamente en '{serverInstance}'.");
        }
    }
}

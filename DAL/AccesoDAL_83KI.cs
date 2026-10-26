using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Service.Interfaces;
using Service.DTOs;

namespace DAL
{
    namespace DAL
    {
        internal class AccesoDAL_83KI
        {
            private readonly IProveedorConfiguracionConexion_83KI _settingsProvider;

            /// <summary>
            /// proveedor global de configuracion de conexion establecido desde la capa de aplicacion (BLL/UI).
            /// se usa como respaldo cuando la instancia no fue construida con un proveedor explicito.
            /// </summary>
            private static IProveedorConfiguracionConexion_83KI s_defaultProvider;

            internal static void EstablecerProveedorPredeterminado(IProveedorConfiguracionConexion_83KI provider)
            {
                s_defaultProvider = provider;
            }

            internal static IProveedorConfiguracionConexion_83KI ProveedorPredeterminado => s_defaultProvider;

            internal AccesoDAL_83KI()
            {
                // constructor sin parametros mantiene compatibilidad con clases DAL no refactorizadas.
            }

            internal AccesoDAL_83KI(IProveedorConfiguracionConexion_83KI settingsProvider)
            {
                _settingsProvider = settingsProvider;
            }

            /// <summary>
            /// devuelve la cadena de conexion configurada, usando el proveedor de instancia o el predeterminado.
            /// falla explícitamente si no hay una configuración completa para evitar conectar a otra base.
            /// </summary>
            private string ResolvedConnectionString
            {
                get
                {
                    var provider = _settingsProvider ?? s_defaultProvider;
                    if (provider == null)
                        throw new InvalidOperationException("No se configuró el proveedor de conexión a la base de datos.");

                    var settings = provider.Cargar();
                    if (settings == null ||
                        string.IsNullOrWhiteSpace(settings.InstanciaServidor) ||
                        string.IsNullOrWhiteSpace(settings.NombreBaseDatos))
                        throw new InvalidOperationException("La configuración de conexión está incompleta: se requieren la instancia del servidor y el nombre de la base de datos.");

                    var builder = new SqlConnectionStringBuilder
                    {
                        DataSource = settings.InstanciaServidor.Trim(),
                        InitialCatalog = settings.NombreBaseDatos.Trim(),
                        IntegratedSecurity = true
                    };
                    return builder.ConnectionString;
                }
            }

            // cadena de conexion a master para RESTORE DATABASE (la base destino se vuelve inaccesible).
            // se deriva de la cadena principal reemplazando el catalogo.
            private string _masterConnectionString
            {
                get
                {
                    var builder = new System.Data.SqlClient.SqlConnectionStringBuilder(ResolvedConnectionString);
                    builder.InitialCatalog = "master";
                    return builder.ConnectionString;
                }
            }

            internal void EjecutarTransaccion(Action<SqlConnection, SqlTransaction> operacion)
            {
                using (SqlConnection conn = new SqlConnection(ResolvedConnectionString))
                {
                    conn.Open();
                    using (SqlTransaction tran = conn.BeginTransaction())
                    {
                        try
                        {
                            operacion(conn, tran);
                            tran.Commit();
                        }
                        catch (Exception original)
                        {
                            try { tran.Rollback(); }
                            catch (Exception rollback)
                            {
                                // Conservar el error de negocio o de confirmación y su traza de pila; la limpieza es evidencia secundaria.
                                original.Data["RollbackException"] = rollback;
                            }
                            throw;
                        }
                    }
                }
            }

            internal static object LeerEscalarTransaccional(SqlConnection conn, SqlTransaction tran, string consulta, List<SqlParameter> parametros = null)
            {
                using (SqlCommand cmd = new SqlCommand(consulta, conn, tran))
                {
                    if (parametros != null)
                    {
                        cmd.Parameters.AddRange(parametros.ToArray());
                    }

                    return cmd.ExecuteScalar();
                }
            }

            internal static int EscribirTransaccional(SqlConnection conn, SqlTransaction tran, string consulta, List<SqlParameter> parametros)
            {
                using (SqlCommand cmd = new SqlCommand(consulta, conn, tran))
                {
                    if (parametros != null)
                    {
                        cmd.Parameters.AddRange(parametros.ToArray());
                    }

                    return cmd.ExecuteNonQuery();
                }
            }

            public DataSet Leer(string consulta, List<SqlParameter> parametros = null)
            {
                DataSet ds = new DataSet();
                using (SqlConnection conn = new SqlConnection(ResolvedConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(consulta, conn))
                    {
                        if (parametros != null)
                        {
                            cmd.Parameters.AddRange(parametros.ToArray());
                        }

                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        // el fill se encarga de abrir y cerrar la conexion solo si es necesario
                        da.Fill(ds);
                    }
                }
                return ds;
            }

            public object LeerEscalar(string consulta, List<SqlParameter> parametros = null)
            {
                using (SqlConnection conn = new SqlConnection(ResolvedConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(consulta, conn))
                    {
                        if (parametros != null)
                        {
                            cmd.Parameters.AddRange(parametros.ToArray());
                        }

                        try
                        {
                            conn.Open();
                            return cmd.ExecuteScalar(); // devuelve solo la primera columna de la primera fila
                        }
                        catch (SqlException ex)
                        {
                            throw new Exception("Error al obtener valor escalar", ex);
                        }
                    }
                }
            }

            public int Escribir(string consulta, List<SqlParameter> parametros)
            {
                using (SqlConnection conn = new SqlConnection(ResolvedConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(consulta, conn))
                    {
                        if (parametros != null)
                        {
                            cmd.Parameters.AddRange(parametros.ToArray());
                        }

                        try
                        {
                            conn.Open();
                            return cmd.ExecuteNonQuery();
                        }
                        catch (SqlException ex)
                        {
                            throw new Exception("Error al escribir en la base de datos", ex);
                        }
                        finally
                        {
                            // por las dudas, aunque el using se encarga, el finally asegura el cierre
                            if (conn.State == ConnectionState.Open) conn.Close();
                        }
                    }
                }
            }

            /// <summary>
            /// ejecuta un comando contra la base master.
            /// se usa para RESTORE DATABASE ya que la base destino se vuelve inaccesible.
            /// </summary>
            internal void EjecutarEnMaster(string consulta, List<SqlParameter> parametros = null)
            {
                using (SqlConnection conn = new SqlConnection(_masterConnectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(consulta, conn))
                    {
                        if (parametros != null)
                            cmd.Parameters.AddRange(parametros.ToArray());
                        cmd.CommandTimeout = 300; // la restauracion puede tardar
                        cmd.ExecuteNonQuery();
                    }
                }
            }

            /// <summary>
            /// ejecuta una consulta de lectura contra la base master y devuelve un DataSet.
            /// se usa para RESTORE FILELISTONLY y consultas de metadatos antes del restore.
            /// </summary>
            internal DataSet LeerDesdeMaster(string consulta, List<SqlParameter> parametros = null)
            {
                DataSet ds = new DataSet();
                using (SqlConnection conn = new SqlConnection(_masterConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(consulta, conn))
                    {
                        if (parametros != null)
                            cmd.Parameters.AddRange(parametros.ToArray());
                        cmd.CommandTimeout = 60;
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(ds);
                        }
                    }
                }
                return ds;
            }

            /// <summary>
            /// lee datos dentro de una transaccion existente y devuelve un DataSet.
            /// </summary>
            internal static DataSet LeerTransaccional(
                SqlConnection conn,
                SqlTransaction tran,
                string consulta,
                List<SqlParameter> parametros = null)
            {
                DataSet ds = new DataSet();
                using (SqlCommand cmd = new SqlCommand(consulta, conn, tran))
                {
                    if (parametros != null)
                        cmd.Parameters.AddRange(parametros.ToArray());

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(ds);
                    }
                }
                return ds;
            }

            public DataSet LeerStoredProcedure(string procedimiento, List<SqlParameter> parametros = null)
            {
                DataSet ds = new DataSet();
                using (SqlConnection conn = new SqlConnection(ResolvedConnectionString))
                using (SqlCommand cmd = new SqlCommand(procedimiento, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (parametros != null) cmd.Parameters.AddRange(parametros.ToArray());
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(ds);
                    }
                }
                return ds;
            }

            public object LeerEscalarStoredProcedure(string procedimiento, List<SqlParameter> parametros = null)
            {
                using (SqlConnection conn = new SqlConnection(ResolvedConnectionString))
                using (SqlCommand cmd = new SqlCommand(procedimiento, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (parametros != null) cmd.Parameters.AddRange(parametros.ToArray());
                    conn.Open();
                    return cmd.ExecuteScalar();
                }
            }

            internal static DataSet LeerStoredProcedureTransaccional(SqlConnection conn, SqlTransaction tran, string procedimiento, List<SqlParameter> parametros = null)
            {
                DataSet ds = new DataSet();
                using (SqlCommand cmd = new SqlCommand(procedimiento, conn, tran))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (parametros != null) cmd.Parameters.AddRange(parametros.ToArray());
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(ds);
                    }
                }
                return ds;
            }

            internal static int EscribirStoredProcedureTransaccional(SqlConnection conn, SqlTransaction tran, string procedimiento, List<SqlParameter> parametros = null)
            {
                using (SqlCommand cmd = new SqlCommand(procedimiento, conn, tran))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (parametros != null) cmd.Parameters.AddRange(parametros.ToArray());
                    return cmd.ExecuteNonQuery();
                }
            }
        }
    }
}

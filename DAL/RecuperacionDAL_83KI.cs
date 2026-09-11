using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Service.Interfaces;

namespace DAL
{
    /// <summary>
    /// helper dal para comandos de backup y restore de base de datos.
    /// usa composicion sobre herencia para acceder a los helpers internos de AccesoDAL_83KI.
    /// </summary>
    public class RecuperacionDAL_83KI : interfaces.IRecuperacionDAL_83KI
    {
        private readonly DAL.AccesoDAL_83KI _acceso;
        private readonly IProveedorConfiguracionConexion_83KI _settingsProvider;

        public RecuperacionDAL_83KI()
        {
            _acceso = new DAL.AccesoDAL_83KI();
        }

        public RecuperacionDAL_83KI(IProveedorConfiguracionConexion_83KI settingsProvider)
        {
            _settingsProvider = settingsProvider;
            _acceso = new DAL.AccesoDAL_83KI(settingsProvider);
        }

        private string DatabaseName
        {
            get
            {
                var provider = _settingsProvider ?? DAL.AccesoDAL_83KI.ProveedorPredeterminado;
                if (provider != null)
                {
                    var settings = provider.Cargar();
                    if (settings != null && !string.IsNullOrWhiteSpace(settings.NombreBaseDatos))
                        return settings.NombreBaseDatos;
                }
                return "GestionUsuarios";
            }
        }

        public void EjecutarBackup(string rutaArchivo)
        {
            string consulta = $"BACKUP DATABASE [{DatabaseName}] TO DISK = @ruta WITH INIT";
            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@ruta", rutaArchivo)
            };
            _acceso.Escribir(consulta, parametros);
        }

        public void EjecutarRestore(string rutaArchivo)
        {
            string rutaSegura = rutaArchivo.Replace("'", "''");
            string dbName = DatabaseName;

            try
            {
                // paso 1: leer los nombres logicos de los archivos dentro del backup
                string fileListQuery = $"RESTORE FILELISTONLY FROM DISK = N'{rutaSegura}'";
                var fileList = _acceso.LeerDesdeMaster(fileListQuery);

                // paso 2: encontrar los nombres logicos de datos (Type=D) y log (Type=L)
                string dataLogical = null, logLogical = null;
                foreach (DataRow row in fileList.Tables[0].Rows)
                {
                    string type = row["Type"]?.ToString().Trim().ToUpperInvariant();
                    if (type == "D" && dataLogical == null)
                        dataLogical = row["LogicalName"]?.ToString();
                    else if (type == "L" && logLogical == null)
                        logLogical = row["LogicalName"]?.ToString();
                }

                // paso 3: obtener el directorio de datos y log del servidor destino
                string dataDir = ObtenerDirectorioServidor("D");
                string logDir = ObtenerDirectorioServidor("L");

                // paso 4: armar las clausulas MOVE para reubicar los archivos
                string moveClauses = "";
                if (!string.IsNullOrEmpty(dataLogical) && !string.IsNullOrEmpty(dataDir))
                {
                    string targetMdf = System.IO.Path.Combine(dataDir, dbName + ".mdf");
                    moveClauses += $", MOVE N'{dataLogical.Replace("'", "''")}' TO N'{targetMdf.Replace("'", "''")}'";
                }
                if (!string.IsNullOrEmpty(logLogical) && !string.IsNullOrEmpty(logDir))
                {
                    string targetLdf = System.IO.Path.Combine(logDir, dbName + "_log.ldf");
                    moveClauses += $", MOVE N'{logLogical.Replace("'", "''")}' TO N'{targetLdf.Replace("'", "''")}'";
                }

                // paso 5: ejecutar restore con move
                string consulta = string.Format(
                    "ALTER DATABASE [{0}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; " +
                    "RESTORE DATABASE [{0}] FROM DISK = N'{1}' WITH REPLACE{2}; " +
                    "ALTER DATABASE [{0}] SET MULTI_USER;",
                    dbName, rutaSegura, moveClauses);
                _acceso.EjecutarEnMaster(consulta);
            }
            catch (SqlException ex) when (ex.Number == 3169)
            {
                // backup creado en una version de SQL Server mas reciente que el servidor actual
                throw new System.Exception(
                    "El archivo de backup fue creado con una version de SQL Server mas reciente " +
                    "que la del servidor actual. Restaurelo en una instancia de SQL Server compatible.", ex);
            }
        }

        /// <summary>
        /// obtiene el directorio de datos o log del servidor SQL actual
        /// consultando master.sys.database_files. type: D para datos, L para log.
        /// </summary>
        private string ObtenerDirectorioServidor(string fileType)
        {
            // intento 1: derivar del archivo fisico de la base master
            try
            {
                string typeCode = fileType == "D" ? "0" : "1";
                string query = $"SELECT physical_name FROM master.sys.database_files WHERE type = {typeCode}";
                var ds = _acceso.LeerDesdeMaster(query);
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    string fullPath = ds.Tables[0].Rows[0][0]?.ToString();
                    if (!string.IsNullOrEmpty(fullPath))
                    {
                        string dir = System.IO.Path.GetDirectoryName(fullPath);
                        if (!string.IsNullOrEmpty(dir))
                            return dir;
                    }
                }
            }
            catch { /* fallback */ }

            // intento 2: usar SERVERPROPERTY (disponible desde SQL Server 2012)
            try
            {
                string prop = fileType == "D" ? "InstanceDefaultDataPath" : "InstanceDefaultLogPath";
                string query = $"SELECT CONVERT(NVARCHAR(512), SERVERPROPERTY('{prop}'))";
                var ds = _acceso.LeerDesdeMaster(query);
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    string dir = ds.Tables[0].Rows[0][0]?.ToString()?.TrimEnd('\\');
                    if (!string.IsNullOrEmpty(dir))
                        return dir;
                }
            }
            catch { /* ultimo fallback: sin MOVE */ }

            return null;
        }
    }
}

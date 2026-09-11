using Service.DTOs;
using Service.Interfaces;

namespace UI
{
    /// <summary>
    /// adaptador sobre Properties.Settings.Default para persistir y cargar
    /// la configuracion de conexion a la base de datos fuera del codigo fuente.
    /// </summary>
    public class ProveedorConfiguracionBD_83KI : IProveedorConfiguracionConexion_83KI
    {
        public ConfiguracionConexionBD_83KI Cargar()
        {
            var server = Properties.Settings.Default.DatabaseServerInstance;
            var database = Properties.Settings.Default.DatabaseName;

            if (string.IsNullOrWhiteSpace(server) && string.IsNullOrWhiteSpace(database))
                return null;

            return new ConfiguracionConexionBD_83KI
            {
                InstanciaServidor = server,
                NombreBaseDatos = database
            };
        }

        public void Guardar(ConfiguracionConexionBD_83KI settings)
        {
            Properties.Settings.Default.DatabaseServerInstance = settings?.InstanciaServidor ?? string.Empty;
            Properties.Settings.Default.DatabaseName = settings?.NombreBaseDatos ?? string.Empty;
            Properties.Settings.Default.Save();
        }
    }
}

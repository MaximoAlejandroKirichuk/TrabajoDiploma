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
        private const string DefaultServerInstance = @"(localdb)\ProjectModels";
        private const string DefaultDatabaseName = "TrabajoDiploma9/13";

        public ConfiguracionConexionBD_83KI Cargar()
        {
            return new ConfiguracionConexionBD_83KI
            {
                InstanciaServidor = DefaultServerInstance,
                NombreBaseDatos = DefaultDatabaseName
            };
        }

        public void Guardar(ConfiguracionConexionBD_83KI settings)
        {
            Properties.Settings.Default.DatabaseServerInstance = DefaultServerInstance;
            Properties.Settings.Default.DatabaseName = DefaultDatabaseName;
            Properties.Settings.Default.Save();
        }
    }
}

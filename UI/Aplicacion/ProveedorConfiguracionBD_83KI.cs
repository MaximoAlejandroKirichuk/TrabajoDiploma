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
            return new ConfiguracionConexionBD_83KI
            {
                InstanciaServidor = Properties.Settings.Default.DatabaseServerInstance,
                NombreBaseDatos = Properties.Settings.Default.DatabaseName
            };
        }

        public void Guardar(ConfiguracionConexionBD_83KI settings)
        {
            if (settings == null)
                throw new System.ArgumentNullException(nameof(settings));

            Properties.Settings.Default.DatabaseServerInstance = settings.InstanciaServidor;
            Properties.Settings.Default.DatabaseName = settings.NombreBaseDatos;
            Properties.Settings.Default.Save();
        }
    }
}

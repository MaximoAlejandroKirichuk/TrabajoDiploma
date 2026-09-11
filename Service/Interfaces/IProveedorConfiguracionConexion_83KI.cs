using Service.DTOs;

namespace Service.Interfaces
{
    /// <summary>
    /// contrato para cargar y persistir la configuracion de conexion.
    /// la implementacion usa el almacenamiento de configuracion de la aplicacion.
    /// </summary>
    public interface IProveedorConfiguracionConexion_83KI
    {
        ConfiguracionConexionBD_83KI Cargar();
        void Guardar(ConfiguracionConexionBD_83KI settings);
    }
}

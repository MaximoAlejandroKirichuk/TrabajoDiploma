using System.Collections.Generic;
using Service.DTOs;

namespace DAL.interfaces
{
    /// <summary>
    /// contrato dal minimo para descubrimiento de instancias SQL Server y validacion de conexion.
    /// </summary>
    public interface IBootstrapBaseDatosDAL_83KI
    {
        IReadOnlyList<string> DescubrirInstancias();
        (bool success, string message) ValidarConexion(ConfiguracionConexionBD_83KI settings);
        (bool success, string message) InstalarBaseDatos(string serverInstance, string databaseName);
    }
}

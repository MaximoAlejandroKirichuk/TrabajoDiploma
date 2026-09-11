using System.Collections.Generic;
using Service.DTOs;

namespace Service.Interfaces
{
    /// <summary>
    /// contrato para el coordinador de bootstrap de base de datos.
    /// la UI consume este servicio para evaluar, descubrir, validar y guardar configuracion.
    /// </summary>
    public interface IBootstrapBaseDatosService_83KI
    {
        ResultadoBootstrap_83KI EvaluarInicio();
        IReadOnlyList<string> DescubrirInstancias();
        ResultadoBootstrap_83KI Validar(ConfiguracionConexionBD_83KI settings);
        ResultadoBootstrap_83KI InstalarBaseDatos(string serverInstance, string databaseName);
        void Guardar(ConfiguracionConexionBD_83KI settings);
    }
}

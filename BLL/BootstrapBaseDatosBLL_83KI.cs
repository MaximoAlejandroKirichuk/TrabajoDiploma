using System.Collections.Generic;
using DAL.interfaces;
using Service.DTOs;
using Service.Interfaces;

namespace BLL
{
    /// <summary>
    /// coordinador de bootstrap de base de datos.
    /// evalua el estado de configuracion al inicio, valida conectividad y persiste la configuracion validada.
    /// </summary>
    public class BootstrapBaseDatosBLL_83KI : IBootstrapBaseDatosService_83KI
    {
        private readonly IBootstrapBaseDatosDAL_83KI _dal;
        private readonly IProveedorConfiguracionConexion_83KI _provider;

        public BootstrapBaseDatosBLL_83KI(IBootstrapBaseDatosDAL_83KI dal, IProveedorConfiguracionConexion_83KI provider)
        {
            _dal = dal;
            _provider = provider;
        }

        public ResultadoBootstrap_83KI EvaluarInicio()
        {
            var settings = _provider.Cargar();

            // sin configuracion persistida — primera ejecucion
            if (settings == null ||
                (string.IsNullOrWhiteSpace(settings.InstanciaServidor) &&
                 string.IsNullOrWhiteSpace(settings.NombreBaseDatos)))
            {
                return new ResultadoBootstrap_83KI
                {
                    Estado = EstadoBootstrap_83KI.RequiereConfiguracion,
                    Mensaje = "No se encontro configuracion de base de datos. Complete el formulario de configuracion inicial."
                };
            }

            // configuracion incompleta
            if (string.IsNullOrWhiteSpace(settings.InstanciaServidor) ||
                string.IsNullOrWhiteSpace(settings.NombreBaseDatos))
            {
                return new ResultadoBootstrap_83KI
                {
                    Estado = EstadoBootstrap_83KI.RequiereConfiguracion,
                    Mensaje = "La configuracion de base de datos esta incompleta. Complete ambos campos."
                };
            }

            // configuracion existe — validarla
            var (success, message) = _dal.ValidarConexion(settings);

            if (!success)
            {
                // determinar si es un problema de instancia o de base de datos
                if (message.Contains("no existe") || message.Contains("database"))
                {
                    return new ResultadoBootstrap_83KI
                    {
                        Estado = EstadoBootstrap_83KI.BaseDeDatosFaltante,
                        Mensaje = message
                    };
                }

                return new ResultadoBootstrap_83KI
                {
                    Estado = EstadoBootstrap_83KI.ConexionInvalida,
                    Mensaje = message
                };
            }

            return new ResultadoBootstrap_83KI
            {
                Estado = EstadoBootstrap_83KI.Listo,
                Mensaje = "Configuracion de base de datos valida."
            };
        }

        public IReadOnlyList<string> DescubrirInstancias()
        {
            return _dal.DescubrirInstancias();
        }

        public ResultadoBootstrap_83KI Validar(ConfiguracionConexionBD_83KI settings)
        {
            var (success, message) = _dal.ValidarConexion(settings);

            if (!success)
            {
                if (message.Contains("no existe") || message.Contains("database"))
                {
                    return new ResultadoBootstrap_83KI
                    {
                        Estado = EstadoBootstrap_83KI.BaseDeDatosFaltante,
                        Mensaje = message
                    };
                }

                return new ResultadoBootstrap_83KI
                {
                    Estado = EstadoBootstrap_83KI.ConexionInvalida,
                    Mensaje = message
                };
            }

            return new ResultadoBootstrap_83KI
            {
                Estado = EstadoBootstrap_83KI.Listo,
                Mensaje = "Conexion validada correctamente."
            };
        }

        public void Guardar(ConfiguracionConexionBD_83KI settings)
        {
            _provider.Guardar(settings);
        }

        public ResultadoBootstrap_83KI InstalarBaseDatos(string serverInstance, string databaseName)
        {
            var (success, message) = _dal.InstalarBaseDatos(serverInstance, databaseName);

            if (!success)
            {
                return new ResultadoBootstrap_83KI
                {
                    Estado = EstadoBootstrap_83KI.ConexionInvalida,
                    Mensaje = message
                };
            }

            return new ResultadoBootstrap_83KI
            {
                Estado = EstadoBootstrap_83KI.Listo,
                Mensaje = message
            };
        }
    }
}

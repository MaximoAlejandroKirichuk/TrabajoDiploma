using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
using BLL;
using Service.Interfaces;
using DAL.interfaces;


namespace Service
{
    public static class ServiceFactory_83KI
    {
        // Usamos para crear los objetos solo cuando se necesitan
        private static IGestorUsuario_83KI _gestorUsuario;
        private static IGestorRol_83KI _gestorRol;
        private static IBitacoraManager_83KI _bitacoraManager;
        private static IConsultaBitacoraEventos_83KI _consultaBitacoraEventos;
        private static IGestorIdioma_83KI _gestorIdioma;
        private static IIntegridadDatosService_83KI _integridadDatosService;
        private static IRecuperacionBaseDatosService_83KI _recuperacionBaseDatosService;
        private static IBootstrapBaseDatosService_83KI _servicioBootstrapBaseDatos;
        private static IProveedorConfiguracionConexion_83KI _proveedorConfiguracionConexion;

        public static IGestorUsuario_83KI GetGestorUsuario()
        {
            if (_gestorUsuario == null)
            {
                //necesito para consulta y escriba con la base de datos
                IUsuarioDAL_83KI datos = new UsuarioDAL_83KI();

                IEncriptador_83KI encriptador = new Encriptador_83KI();
                ISessionManager_83KI sessionManager = SessionManager_83KI.Instancia;
                IBitacoraManager_83KI bitacoraManager = GetBitacoraManager();
                IGestorIdioma_83KI gestorIdioma = GetGestorIdioma();
                IIntegridadDatosService_83KI integridadService = GetIntegridadDatosService();

                _gestorUsuario = new GestorUsuarioBLL_83KI(datos, encriptador, sessionManager, bitacoraManager, new RolDAL_83KI(), gestorIdioma, integridadService);
            }
            return _gestorUsuario;
        }

        public static IGestorIdioma_83KI GetGestorIdioma()
        {
            if (_gestorIdioma == null)
            {
                _gestorIdioma = new GestorIdioma_83KI();
            }

            return _gestorIdioma;
        }

        public static IGestorRol_83KI GetGestorRol()
        {
            if (_gestorRol == null)
            {
                IRolDAL_83KI rolDal = new RolDAL_83KI();
                ISessionManager_83KI sessionManager = SessionManager_83KI.Instancia;
                IBitacoraManager_83KI bitacoraManager = GetBitacoraManager();
                _gestorRol = new GestorRolBLL_83KI(rolDal, sessionManager, bitacoraManager);
            }

            return _gestorRol;
        }

        public static IBitacoraManager_83KI GetBitacoraManager()
        {
            if (_bitacoraManager == null)
            {
                IBitacoraDAL_83KI bitacoraDAL = new BitacoraEventoDAL_83KI();
                _bitacoraManager = new BitacoraBLL_83KI(bitacoraDAL);
            }

            return _bitacoraManager;
        }

        public static IConsultaBitacoraEventos_83KI GetConsultaBitacoraEventos()
        {
            if (_consultaBitacoraEventos == null)
            {
                _consultaBitacoraEventos = new ConsultaBitacoraEventos_83KI(GetGestorUsuario(), GetBitacoraManager());
            }

            return _consultaBitacoraEventos;
        }

        public static IIntegridadDatosService_83KI GetIntegridadDatosService()
        {
            if (_integridadDatosService == null)
            {
                IIntegridadDAL_83KI integridadDal = new IntegridadDAL_83KI(new Encriptador_83KI());
                IBitacoraManager_83KI bitacoraManager = GetBitacoraManager();
                _integridadDatosService = new IntegridadBLL_83KI(integridadDal, bitacoraManager);
            }
            return _integridadDatosService;
        }

        public static IRecuperacionBaseDatosService_83KI GetRecuperacionBaseDatosService()
        {
            if (_recuperacionBaseDatosService == null)
            {
                IRecuperacionDAL_83KI recuperacionDal = new RecuperacionDAL_83KI(_proveedorConfiguracionConexion);
                IBitacoraManager_83KI bitacoraManager = GetBitacoraManager();
                _recuperacionBaseDatosService = new RecuperacionBaseDatosBLL_83KI(recuperacionDal, bitacoraManager);
            }
            return _recuperacionBaseDatosService;
        }

        /// <summary>
        /// registra el proveedor de configuracion de conexion.
        /// debe llamarse desde UI antes de usar cualquier servicio que consuma DAL.
        /// </summary>
        public static void EstablecerProveedorConfiguracionConexion(IProveedorConfiguracionConexion_83KI provider)
        {
            _proveedorConfiguracionConexion = provider;
            // propaga el proveedor a la capa DAL para que todas las clases que usan
            // el constructor sin parametros de AccesoDAL_83KI tambien usen la configuracion persistida.
            DAL.DAL.AccesoDAL_83KI.EstablecerProveedorPredeterminado(provider);
        }

        /// <summary>
        /// obtiene el servicio de bootstrap de base de datos.
        /// requiere que EstablecerProveedorConfiguracionConexion haya sido llamado previamente.
        /// </summary>
        public static IBootstrapBaseDatosService_83KI ObtenerServicioBootstrapBaseDatos()
        {
            if (_servicioBootstrapBaseDatos == null)
            {
                var dal = new BootstrapBaseDatosDAL_83KI();
                _servicioBootstrapBaseDatos = new BootstrapBaseDatosBLL_83KI(dal, _proveedorConfiguracionConexion);
            }
            return _servicioBootstrapBaseDatos;
        }
    }
}

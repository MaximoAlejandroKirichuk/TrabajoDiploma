using Service.Interfaces;
using Service.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class SessionManager_83KI : ISessionManager_83KI
    {
        private static SessionManager_83KI _instancia;
        public Usuario_83KI UsuarioActivo { get; private set; }

        // estado de recuperacion de integridad de datos
        public bool RequiereRecuperacionIntegridad { get; private set; }
        public System.Collections.Generic.IReadOnlyList<string> TablasAfectadasIntegridad { get; private set; }
            = new System.Collections.Generic.List<string>();

        // bandera que se activa tras un restore exitoso para solicitar reinicio de la aplicacion
        public bool ReinicioRequeridoDespuesDeRestore { get; set; }

        private SessionManager_83KI() { }

        public static SessionManager_83KI Instancia
        {
            get
            {
                if (_instancia == null)
                {
                    _instancia = new SessionManager_83KI();
                }
                return _instancia;
            }
        }
        public void IniciarSesion(Usuario_83KI usuario)
        {
            UsuarioActivo = usuario;
        }
        public void CerrarSesion()
        {
            UsuarioActivo = null;
            ReinicioRequeridoDespuesDeRestore = false;
            LimpiarRecuperacionIntegridad();
        }

        public void EstablecerRecuperacionIntegridad(System.Collections.Generic.IEnumerable<string> tablasAfectadas)
        {
            RequiereRecuperacionIntegridad = true;
            TablasAfectadasIntegridad = tablasAfectadas != null
                ? new System.Collections.Generic.List<string>(tablasAfectadas).AsReadOnly()
                : new System.Collections.Generic.List<string>().AsReadOnly();
        }

        public void LimpiarRecuperacionIntegridad()
        {
            RequiereRecuperacionIntegridad = false;
            TablasAfectadasIntegridad = new System.Collections.Generic.List<string>().AsReadOnly();
        }

        public IEnumerable<Patente_83KI> ObtenerPermisos()
        {
            if (UsuarioActivo == null || UsuarioActivo.Rol == null)
            {
                return Enumerable.Empty<Patente_83KI>();
            }

            return UsuarioActivo.Rol.ObtenerPatentes().ToList();
        }

        public bool TienePermiso(PermisoSistema_83KI permiso)
        {
            int codigoPermiso = (int)permiso;
            return ObtenerPermisos().Any(p => p.CodigoPatente == codigoPermiso);
        }
    }
}

using Service.Entidades;
using System.Collections.Generic;

namespace Service.Interfaces
{
    public interface ISessionManager_83KI
    {
        void IniciarSesion(Usuario_83KI usuario);
        void CerrarSesion();
        IEnumerable<Patente_83KI> ObtenerPermisos();
        bool TienePermiso(PermisoSistema_83KI permiso);
        Usuario_83KI UsuarioActivo { get; }

        // estado de recuperacion de integridad de datos
        bool RequiereRecuperacionIntegridad { get; }
        System.Collections.Generic.IReadOnlyList<string> TablasAfectadasIntegridad { get; }
        void EstablecerRecuperacionIntegridad(System.Collections.Generic.IEnumerable<string> tablasAfectadas);
        void LimpiarRecuperacionIntegridad();
    }
}

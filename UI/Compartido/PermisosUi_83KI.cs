using Service;
using BE.Entidades;
using Service.Entidades;
using System.Linq;
using System.Windows.Forms;

namespace UI
{
    internal static class PermisosUi_83KI
    {
        public static bool Tiene(PermisoSistema_83KI permiso)
        {
            return SessionManager_83KI.Instancia.TienePermiso(permiso);
        }

        public static bool TieneAlguno(params PermisoSistema_83KI[] permisos)
        {
            return permisos.Any(Tiene);
        }

        public static bool TienePermisoMaestros()
        {
            int permisoDesde = (int)PermisoSistema_83KI.GestionProfesores;
            int permisoHasta = (int)PermisoSistema_83KI.DeshabilitarCursoProfesor;

            return SessionManager_83KI.Instancia.ObtenerPermisos()
                .Any(p => p.CodigoPatente >= permisoDesde && p.CodigoPatente <= permisoHasta);
        }

        public static void AplicarVisible(Control control, PermisoSistema_83KI permiso)
        {
            control.Visible = Tiene(permiso);
        }

        public static void AplicarVisible(ToolStripItem item, PermisoSistema_83KI permiso)
        {
            item.Visible = Tiene(permiso);
        }
    }
}

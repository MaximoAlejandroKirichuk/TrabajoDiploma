using Service;
using Service.Entidades;
using System.Collections.Generic;
using System.Linq;

namespace UI
{
    internal sealed class TemaAyuda_83KI
    {
        public string Pantalla { get; private set; }
        public string Grupo { get; private set; }
        public string Titulo { get; private set; }
        public PermisoSistema_83KI[] Permisos { get; private set; }

        public TemaAyuda_83KI(string pantalla, string grupo, string titulo, params PermisoSistema_83KI[] permisos)
        {
            Pantalla = pantalla;
            Grupo = grupo;
            Titulo = titulo;
            Permisos = permisos;
        }

        public bool Disponible()
        {
            var sesion = SessionManager_83KI.Instancia;
            if (sesion.RequiereRecuperacionIntegridad || sesion.ReinicioRequeridoDespuesDeRestore) return false;
            if (Permisos.Length == 0)
                return sesion.UsuarioActivo == null || (Pantalla == "Login" && PermisosUi_83KI.Tiene(PermisoSistema_83KI.ReLogin));
            if (Grupo == "Ayuda.Administracion" && !PermisosUi_83KI.Tiene(PermisoSistema_83KI.GestionAdmin)) return false;
            return PermisosUi_83KI.Tiene(PermisoSistema_83KI.Ayuda)
                && Permisos.Any(PermisosUi_83KI.Tiene) && CatalogoAyuda_83KI.ContenedorDisponible(Pantalla);
        }
    }

    internal static class CatalogoAyuda_83KI
    {
        private static readonly Dictionary<string, string> Contenedores = new Dictionary<string, string>
        {
            { "FrmCrearUsuario", "FrmGestionUsuarios" },
            { "FrmModificarUsuario", "FrmGestionUsuarios" },
            { "FrmConfirmarEstadoUsuario", "FrmGestionUsuarios" },
            { "FrmCrearProfesor_83KI", "FrmGestionProfesores_83KI" },
            { "FrmModificarProfesor_83KI", "FrmGestionProfesores_83KI" },
            { "FrmCrearCurso_83KI", "FrmGestionCursos_83KI" },
            { "FrmModificarCurso_83KI", "FrmGestionCursos_83KI" },
            { "FrmEditarComision_83KI", "FrmGestionComisiones_83KI" },
            { "FrmDetalleMorosidad_83KI", "FrmReporteMorosidad_83KI" }
        };

        public static bool ContenedorDisponible(string pantalla)
        {
            string contenedor = ContenedorDe(pantalla);
            return contenedor == null || Buscar(contenedor).Disponible();
        }

        public static string ContenedorDe(string pantalla)
        {
            string contenedor;
            return Contenedores.TryGetValue(pantalla, out contenedor) ? contenedor : null;
        }

        private static readonly TemaAyuda_83KI[] Temas =
        {
            new TemaAyuda_83KI("Login", "Ayuda.General", "Login.Titulo"),
            new TemaAyuda_83KI("FrmConfiguracionBD_83KI", "Ayuda.General", "Ayuda.Configuracion"),
            new TemaAyuda_83KI("FrmPrincipal", "Ayuda.General", "FrmPrincipal.Titulo", PermisoSistema_83KI.Ayuda),
            new TemaAyuda_83KI("FrmCambiarContrasena", "Ayuda.General", "FrmCambiarContrasena.Titulo", PermisoSistema_83KI.CambiarContrasena),
            new TemaAyuda_83KI("FrmRecuperacionIntegridadLogin_83KI", "Ayuda.General", "RecuperacionIntegridad.Titulo", PermisoSistema_83KI.RecalcularHashes, PermisoSistema_83KI.EjecutarBackup, PermisoSistema_83KI.EjecutarRestore),
            new TemaAyuda_83KI("FrmRecuperacionIntegridad_83KI", "Ayuda.Administracion", "RecuperacionIntegridad.Titulo", PermisoSistema_83KI.RecalcularHashes, PermisoSistema_83KI.EjecutarBackup, PermisoSistema_83KI.EjecutarRestore),
            new TemaAyuda_83KI("FrmGestionUsuarios", "Ayuda.Administracion", "FrmGestionUsuarios.Titulo", PermisoSistema_83KI.GestionUsuarios),
            new TemaAyuda_83KI("FrmCrearUsuario", "Ayuda.Administracion", "FrmCrearUsuario.Titulo", PermisoSistema_83KI.CrearUsuario),
            new TemaAyuda_83KI("FrmModificarUsuario", "Ayuda.Administracion", "FrmModificarUsuario.Titulo", PermisoSistema_83KI.ModificarUsuario),
            new TemaAyuda_83KI("FrmConfirmarEstadoUsuario", "Ayuda.Administracion", "FrmConfirmarEstadoUsuario.Titulo", PermisoSistema_83KI.HabilitarUsuario, PermisoSistema_83KI.DeshabilitarUsuario),
            new TemaAyuda_83KI("FrmGestionFamilias_83KI", "Ayuda.Administracion", "FrmGestionFamilias.Titulo", PermisoSistema_83KI.GestionFamilias),
            new TemaAyuda_83KI("FrmGestionRoles_83KI", "Ayuda.Administracion", "FrmGestionRoles.Titulo", PermisoSistema_83KI.GestionRoles),
            new TemaAyuda_83KI("FrmBitacoraEventos", "Ayuda.Administracion", "FrmBitacoraEventos.Titulo", PermisoSistema_83KI.VerBitacoraEventos, PermisoSistema_83KI.ConsultarBitacoraEventos),
            new TemaAyuda_83KI("FrmGestionProfesores_83KI", "FrmPrincipal.Maestros", "FrmGestionProfesores.Titulo", PermisoSistema_83KI.GestionProfesores, PermisoSistema_83KI.VerProfesores),
            new TemaAyuda_83KI("FrmCrearProfesor_83KI", "FrmPrincipal.Maestros", "FrmCrearProfesor.Titulo", PermisoSistema_83KI.CrearProfesor),
            new TemaAyuda_83KI("FrmModificarProfesor_83KI", "FrmPrincipal.Maestros", "FrmModificarProfesor.Titulo", PermisoSistema_83KI.ModificarProfesor),
            new TemaAyuda_83KI("FrmGestionCursos_83KI", "FrmPrincipal.Maestros", "FrmGestionCursos.Titulo", PermisoSistema_83KI.GestionCursos, PermisoSistema_83KI.VerCursos),
            new TemaAyuda_83KI("FrmCrearCurso_83KI", "FrmPrincipal.Maestros", "FrmCrearCurso.Titulo", PermisoSistema_83KI.CrearCurso),
            new TemaAyuda_83KI("FrmModificarCurso_83KI", "FrmPrincipal.Maestros", "FrmModificarCurso.Titulo", PermisoSistema_83KI.ModificarCurso),
            new TemaAyuda_83KI("FrmGestionCursoProfesor_83KI", "FrmPrincipal.Maestros", "FrmGestionCursoProfesor.Titulo", PermisoSistema_83KI.GestionCursoProfesor, PermisoSistema_83KI.VerCursoProfesor),
            new TemaAyuda_83KI("FrmRegistrarPreaperturaComision_83KI", "FrmPrincipal.PlanificacionAcademica", "FrmRegistrarPreaperturaComision.Titulo", PermisoSistema_83KI.RegistrarPreaperturaComision),
            new TemaAyuda_83KI("FrmGestionComisiones_83KI", "FrmPrincipal.PlanificacionAcademica", "FrmGestionComisiones.Titulo", PermisoSistema_83KI.VerComisiones),
            new TemaAyuda_83KI("FrmEditarComision_83KI", "FrmPrincipal.PlanificacionAcademica", "FrmEditarComision.Titulo", PermisoSistema_83KI.ModificarComision),
            new TemaAyuda_83KI("FrmRegistrarConsultaLead_83KI", "FrmPrincipal.PlanificacionAcademica", "FrmRegistrarConsultaLead.Titulo", PermisoSistema_83KI.RegistrarConsultaLead),
            new TemaAyuda_83KI("FrmRegistrarSolicitudInscripcion_83KI", "FrmPrincipal.PlanificacionAcademica", "FrmRegistrarSolicitudInscripcion.Titulo", PermisoSistema_83KI.RegistrarSolicitudInscripcion),
            new TemaAyuda_83KI("FrmRegistrarDecisionBeca_83KI", "FrmPrincipal.PlanificacionAcademica", "FrmRegistrarDecisionBeca.Titulo", PermisoSistema_83KI.RegistrarDecisionBeca),
            new TemaAyuda_83KI("FrmRegistrarEstadoDefinitivoComision_83KI", "FrmPrincipal.PlanificacionAcademica", "FrmRegistrarEstadoDefinitivoComision.Titulo", PermisoSistema_83KI.RegistrarEstadoDefinitivoComision),
            new TemaAyuda_83KI("FrmRegistrarPagoInscripcion_83KI", "FrmPrincipal.CobrosMorosidadActas", "FrmRegistrarPagoInscripcion.Titulo", PermisoSistema_83KI.RegistrarPagoInscripcion),
            new TemaAyuda_83KI("FrmRegistrarPagoCuota_83KI", "FrmPrincipal.CobrosMorosidadActas", "FrmRegistrarPagoCuota.Titulo", PermisoSistema_83KI.RegistrarPagoCuotas),
            new TemaAyuda_83KI("FrmRegistrarResultadosEvaluacion_83KI", "FrmPrincipal.CobrosMorosidadActas", "Cun08.Titulo", PermisoSistema_83KI.RegistrarResultadosEvaluacion),
            new TemaAyuda_83KI("FrmEvaluaciones_83KI", "FrmPrincipal.CobrosMorosidadActas", "Evaluaciones.Titulo", PermisoSistema_83KI.VerEvaluaciones),
            new TemaAyuda_83KI("FrmGenerarCertificado_83KI", "FrmPrincipal.CobrosMorosidadActas", "Cun09.Titulo", PermisoSistema_83KI.GenerarCertificadoFinalizacion),
            new TemaAyuda_83KI("FrmReporteMorosidad_83KI", "FrmPrincipal.Reportes", "ReporteMorosidad.Titulo", PermisoSistema_83KI.VerReporteMorosidad),
            new TemaAyuda_83KI("FrmDetalleMorosidad_83KI", "FrmPrincipal.Reportes", "ReporteMorosidad.DetalleTitulo", PermisoSistema_83KI.VerReporteMorosidad),
            new TemaAyuda_83KI("FrmReporteOcupacionComisiones_83KI", "FrmPrincipal.Reportes", "FrmReporteOcupacionComisiones.Titulo", PermisoSistema_83KI.VerReporteOcupacionComisiones)
        };

        public static IEnumerable<TemaAyuda_83KI> Disponibles()
        {
            return Temas.Where(t => t.Disponible());
        }

        public static TemaAyuda_83KI Buscar(string pantalla)
        {
            return Temas.FirstOrDefault(t => t.Pantalla == pantalla);
        }
    }
}

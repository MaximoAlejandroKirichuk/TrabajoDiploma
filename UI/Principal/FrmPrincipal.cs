using Service;
using BE.Entidades;
using Service.Entidades;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UI.Modulos.Maestros;

namespace UI
{
    public partial class FrmPrincipal : Form, IObservadorIdioma
    {
        private readonly IGestorUsuario_83KI _gestorUsuario;
        private readonly IGestorRol_83KI _gestorRol;
        private readonly IGestorIdioma_83KI _gestorIdioma;
        private readonly IIntegridadDatosService_83KI _integridadService;
        private readonly IRecuperacionBaseDatosService_83KI _recuperacionService;
        private bool _logoutConfirmado;

        public FrmPrincipal(IGestorUsuario_83KI gestorUsuario, IGestorRol_83KI gestorRol)
        {
            InitializeComponent();
            _gestorUsuario = gestorUsuario;
            _gestorRol = gestorRol;
            _gestorIdioma = ServiceFactory_83KI.GetGestorIdioma();
            _integridadService = ServiceFactory_83KI.GetIntegridadDatosService();
            _recuperacionService = ServiceFactory_83KI.GetRecuperacionBaseDatosService();
            AplicarPermisos();
            _gestorIdioma.Suscribir(this);
        }

        private void AplicarPermisos()
        {
            gestionDeUsuariosToolStripMenuItem.Visible = PermisosUi_83KI.Tiene(PermisoSistema_83KI.GestionUsuarios);
            gestionDeFamiliasToolStripMenuItem.Visible = PermisosUi_83KI.Tiene(PermisoSistema_83KI.GestionFamilias);
            gestionDeRolesToolStripMenuItem.Visible = PermisosUi_83KI.Tiene(PermisoSistema_83KI.GestionRoles);
            bitacoraEventosToolStripMenuItem.Visible = PermisosUi_83KI.Tiene(PermisoSistema_83KI.VerBitacoraEventos)
                || PermisosUi_83KI.Tiene(PermisoSistema_83KI.ConsultarBitacoraEventos);
            adminToolStripMenuItem.Visible = PermisosUi_83KI.Tiene(PermisoSistema_83KI.GestionAdmin);

            // menu de recuperacion: visible cuando el admin tiene al menos un permiso de recuperacion
            recuperacionIntegridadToolStripMenuItem.Visible =
                PermisosUi_83KI.Tiene(PermisoSistema_83KI.RecalcularHashes)
                || PermisosUi_83KI.Tiene(PermisoSistema_83KI.EjecutarBackup)
                || PermisosUi_83KI.Tiene(PermisoSistema_83KI.EjecutarRestore);

            // oculta menus de negocio regulares cuando el estado de recuperacion de integridad esta activo
            if (SessionManager_83KI.Instancia.RequiereRecuperacionIntegridad)
            {
                gestionDeUsuariosToolStripMenuItem.Visible = false;
                gestionDeFamiliasToolStripMenuItem.Visible = false;
                gestionDeRolesToolStripMenuItem.Visible = false;
            }

            menuCerrarSesion.Visible = PermisosUi_83KI.Tiene(PermisoSistema_83KI.CerrarSesion);
            iniciarSesionToolStripMenuItem.Visible = PermisosUi_83KI.Tiene(PermisoSistema_83KI.ReLogin);
            cambiarContraseñaToolStripMenuItem.Visible = PermisosUi_83KI.Tiene(PermisoSistema_83KI.CambiarContrasena);
            reToolStripMenuItem.Visible = PermisosUi_83KI.Tiene(PermisoSistema_83KI.Ayuda);
            menuIdioma.Visible = PermisosUi_83KI.Tiene(PermisoSistema_83KI.CambiarIdioma);
            gestionProfesoresToolStripMenuItem.Visible = PermisosUi_83KI.TieneAlguno(
                PermisoSistema_83KI.GestionProfesores,
                PermisoSistema_83KI.VerProfesores);
            gestionCursosToolStripMenuItem.Visible = PermisosUi_83KI.TieneAlguno(
                PermisoSistema_83KI.GestionCursos,
                PermisoSistema_83KI.VerCursos);
            gestionCursoProfesorToolStripMenuItem.Visible = PermisosUi_83KI.TieneAlguno(
                PermisoSistema_83KI.GestionCursoProfesor,
                PermisoSistema_83KI.VerCursoProfesor);
            maestrosToolStripMenuItem.Visible = PermisosUi_83KI.TienePermisoMaestros();
        }

        public void ActualizarIdioma(IIdioma idioma)
        {
            Text = Texto("FrmPrincipal.Titulo");
            menuSesion.Text = Texto("FrmPrincipal.Usuario");
            menuCerrarSesion.Text = Texto("FrmPrincipal.CerrarSesion");
            iniciarSesionToolStripMenuItem.Text = Texto("FrmPrincipal.IniciarSesion");
            cambiarContraseñaToolStripMenuItem.Text = Texto("FrmPrincipal.CambiarContrasena");
            adminToolStripMenuItem.Text = Texto("FrmPrincipal.Admin");
            gestionDeUsuariosToolStripMenuItem.Text = Texto("FrmPrincipal.GestionUsuarios");
            gestionDeFamiliasToolStripMenuItem.Text = Texto("FrmPrincipal.GestionFamilias");
            gestionDeRolesToolStripMenuItem.Text = Texto("FrmPrincipal.GestionRoles");
            bitacoraEventosToolStripMenuItem.Text = Texto("FrmPrincipal.BitacoraEventos");
            maestrosToolStripMenuItem.Text = Texto("FrmPrincipal.Maestros");
            gestionCursosToolStripMenuItem.Text = Texto("FrmPrincipal.GestionCursos");
            gestionProfesoresToolStripMenuItem.Text = Texto("FrmPrincipal.GestionProfesores");
            gestionCursoProfesorToolStripMenuItem.Text = Texto("FrmPrincipal.GestionCursoProfesor");
            planificacionAcademicaToolStripMenuItem.Text = Texto("FrmPrincipal.PlanificacionAcademica");
            cobrosMorosidadActasToolStripMenuItem.Text = Texto("FrmPrincipal.CobrosMorosidadActas");
            reportesToolStripMenuItem.Text = Texto("FrmPrincipal.Reportes");
            reToolStripMenuItem.Text = Texto("FrmPrincipal.Ayuda");
            menuIdioma.Text = Texto("FrmPrincipal.Idioma");
            espanolToolStripMenuItem.Text = Texto("FrmPrincipal.Espanol");
            inglesToolStripMenuItem.Text = Texto("FrmPrincipal.Ingles");
            recuperacionIntegridadToolStripMenuItem.Text = Texto("RecuperacionIntegridad.Titulo");

            string idiomaActualId = idioma?.Id ?? GestorIdioma_83KI.IdiomaPorDefecto;
            espanolToolStripMenuItem.Checked = idiomaActualId == "es-AR";
            inglesToolStripMenuItem.Checked = idiomaActualId == "en-US";
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _gestorIdioma.Desuscribir(this);
            base.OnFormClosed(e);
        }

        private string Texto(string clave)
        {
            //Este metodo busca una traduccion en el idioma actual.
            return _gestorIdioma.ObtenerTexto(clave);
        }

        private void menuCerrarSesion_Click(object sender, EventArgs e)
        {
            ConfirmarLogoutYCerrar();
        }

        private void FrmPrincipal_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_logoutConfirmado)
            {
                return;
            }

            if (e.CloseReason == CloseReason.UserClosing)
            {
                bool logoutAprobado = ConfirmarLogoutYCerrar();

                if (logoutAprobado == false)
                {
                    e.Cancel = true;
                }
                else
                {
                    e.Cancel = false;
                }
            }
        }

        private bool ConfirmarLogoutYCerrar()
        {
            var confirmacion = MessageBox.Show(
                Texto("FrmPrincipal.ConfirmarCerrarSesionMensaje"),
                Texto("FrmPrincipal.ConfirmarCerrarSesionTitulo"),
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Question);

            if (confirmacion != DialogResult.OK)
            {
                return false;
            }

            _gestorUsuario.Logout();
            _logoutConfirmado = true;
            DialogResult = DialogResult.Retry;
            Close();
            return true;
        }

        private void gestionDeUsuariosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var gestion = new FrmGestionUsuarios(_gestorUsuario, _gestorRol))
            {
                gestion.ShowDialog(this);
            }
        }

        private void gestionDeFamiliasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var gestion = new FrmGestionFamilias_83KI(_gestorRol))
            {
                gestion.ShowDialog(this);
            }
        }

        private void gestionDeRolesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var gestion = new FrmGestionRoles_83KI(_gestorRol))
            {
                gestion.ShowDialog(this);
            }
        }

        private void cambiarContraseñaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var cambiarContrasena = new FrmCambiarContrasena(_gestorUsuario);
            DialogResult resultado = cambiarContrasena.ShowDialog(this);

            if(resultado == DialogResult.OK)
            {
                ForzarLogout(); 
            }
        }

        private void ForzarLogout()
        {
            MessageBox.Show(
                Texto("FrmPrincipal.CambioContrasenaMensaje"),
                Texto("FrmPrincipal.CambioContrasenaTitulo"),
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            _gestorUsuario.Logout();
            _logoutConfirmado = true;
            DialogResult = DialogResult.Retry;
            this.Close();
        }

        private void ForzarLogoutRestore()
        {
            MessageBox.Show(
                Texto("RecuperacionIntegridad.RestoreExitoso"),
                Texto("RecuperacionIntegridad.Titulo"),
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            _gestorUsuario.Logout();
            _logoutConfirmado = true;
            DialogResult = DialogResult.Retry;
            this.Close();
        }

        private void iniciarSesionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Login login = new Login();
            login.Show();
        }

        private void menuSesion_Click(object sender, EventArgs e)
        {

        }

        private void bitacoraEventosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var bitacoraEventos = new FrmBitacoraEventos(
                ServiceFactory_83KI.GetConsultaBitacoraEventos(),
                new Exportacion.BitacoraEventosPdfExporter_83KI(),
                ServiceFactory_83KI.GetBitacoraManager()))
            {
                bitacoraEventos.ShowDialog(this);
            }
        }

        private void espanolToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _gestorUsuario.CambiarIdiomaUsuarioActual("es-AR");
        }

        private void inglesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _gestorUsuario.CambiarIdiomaUsuarioActual("en-US");
        }

        private void recuperacionIntegridadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!PermisosUi_83KI.TieneAlguno(
                PermisoSistema_83KI.RecalcularHashes,
                PermisoSistema_83KI.EjecutarBackup,
                PermisoSistema_83KI.EjecutarRestore))
            {
                IdiomaUiHelper_83KI.MostrarAdvertencia(
                    this,
                    "Errores.SinPermisos",
                    "Comun.Seguridad");
                return;
            }

            using (var frmRecuperacion = new FrmRecuperacionIntegridad_83KI())
            {
                var resultado = frmRecuperacion.ShowDialog(this);

                // post-restore la aplicacion debe cerrarse
                if (SessionManager_83KI.Instancia.ReinicioRequeridoDespuesDeRestore)
                {
                    ForzarLogoutRestore();
                    return;
                }

                // si el recalculo fue exitoso, refrescar visibilidad de permisos
                if (frmRecuperacion.RecalculoExitoso)
                {
                    AplicarPermisos();
                }
            }
        }

        private void gestionProfesoresToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var gestion = new FrmGestionProfesores_83KI(ComposicionMaestros_83KI.CrearGestorProfesor()))
            {
                gestion.ShowDialog(this);
            }
        }

        private void gestionCursosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var gestion = new FrmGestionCursos_83KI(ComposicionMaestros_83KI.CrearGestorCurso()))
            {
                gestion.ShowDialog(this);
            }
        }

        private void gestionCursoProfesorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var gestion = new FrmGestionCursoProfesor_83KI(ComposicionMaestros_83KI.CrearGestorCursoProfesor()))
            {
                gestion.ShowDialog(this);
            }
        }

        private void planificacionAcademicaToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}

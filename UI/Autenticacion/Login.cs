using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using Service.Entidades;
using Service.Excepciones.Login;
using Service.Interfaces;
using Service.Excepciones;
using Service;

namespace UI
{
    public partial class Login : Form
    {
        private readonly IGestorUsuario_83KI _gestor;
        private readonly IGestorRol_83KI _gestorRol;

        public Login()
        {
            _gestor = Service.ServiceFactory_83KI.GetGestorUsuario();
            _gestorRol = Service.ServiceFactory_83KI.GetGestorRol();
            InitializeComponent();
        }


        private void Login_Load(object sender, EventArgs e)
        {
            AplicarTextosPorDefectoEspanol();
            LoginDesignConfig();
            RedondearPanel(panelLogin);
            ButtonDesing(btnLogin);

        }
        private void btnLogin_Click(object sender, EventArgs e)
        {
            var userName = txt_userName.Text.Trim();
            var contrasena = txt_Contrasena.Text.Trim();

            try
            {
                _gestor.Login(userName, contrasena);

                var usuarioActivo = SessionManager_83KI.Instancia.UsuarioActivo;

                if (usuarioActivo != null)
                {
                    //aca se hace la contrasena base para compararla
                    string contrasenaPorDefecto = Usuario_83KI.EstablecerContrasenaPorDefecto(usuarioActivo.Apellido, usuarioActivo.DNI);

                    bool usaContrasenaPorDefecto = (contrasena == contrasenaPorDefecto);

                    if (usaContrasenaPorDefecto == true)
                    {
                        bool cambioExitoso = MostrarAdvertenciaYForzarCambio();

                        if (!cambioExitoso)
                        {
                            _gestor.Logout();
                            txt_Contrasena.Clear();
                            txt_Contrasena.Focus();
                            return;
                        }
                    }
                }

                if (SessionManager_83KI.Instancia.RequiereRecuperacionIntegridad)
                {
                    bool puedeRecuperar = PermisosUi_83KI.TieneAlguno(
                        PermisoSistema_83KI.RecalcularHashes,
                        PermisoSistema_83KI.EjecutarRestore);

                    if (puedeRecuperar)
                    {
                        Hide();

                        bool recalculoExitoso = false;

                        using (var frmRecuperacion = new FrmRecuperacionIntegridadLogin_83KI())
                        {
                            frmRecuperacion.ShowDialog(this);
                            recalculoExitoso = frmRecuperacion.RecalculoExitoso;
                        }

                        if (SessionManager_83KI.Instancia.ReinicioRequeridoDespuesDeRestore)
                        {
                            _gestor.Logout();
                            Close();
                            return;
                        }

                // no permitir cerrar el formulario de recuperacion sin una recuperacion real.
                // cancelar/cerrar sin recuperacion NO debe avanzar a FrmPrincipal.
                        if (!recalculoExitoso)
                        {
                            _gestor.Logout();
                            txt_Contrasena.Clear();
                            txt_Contrasena.Focus();
                            Show();
                            return;
                        }

                        using (var formPrincipal = new FrmPrincipal(_gestor, _gestorRol))
                        {
                            var resultado = formPrincipal.ShowDialog(this);

                            if (resultado == DialogResult.Retry)
                            {
                                txt_Contrasena.Clear();
                                txt_Contrasena.Focus();
                                Show();
                                return;
                            }
                        }
                        Close();
                        return;
                    }

                   
                    IdiomaUiHelper_83KI.MostrarAdvertencia(
                        this,
                        "Errores.SinPermisos",
                        "Comun.Seguridad");
                    _gestor.Logout();
                    txt_Contrasena.Clear();
                    txt_Contrasena.Focus();
                    Show();
                    return;
                }

                Hide();

                using (var formPrincipal = new FrmPrincipal(_gestor, _gestorRol))
                {
                    var resultado = formPrincipal.ShowDialog(this);

                    if (resultado == DialogResult.Retry)
                    {
                        txt_Contrasena.Clear();
                        txt_Contrasena.Focus();
                        Show();
                        return;
                    }
                }
                Close();
            }
            catch (IntegridadComprometidaException_83KI ex)
            {
                Show();
                // usuario no-admin bloqueado por falla de integridad
                IdiomaUiHelper_83KI.MostrarError(this, ex, "Comun.Seguridad", MessageBoxIcon.Stop);
            }
            catch (UsuarioActivoActualmenteException_83KI ex)
            {
                Show();
                IdiomaUiHelper_83KI.MostrarError(this, ex, "Comun.Error", MessageBoxIcon.Warning);
            }
            catch (UsuarioNoExisteException_83KI ex)
            {
                Show();
                IdiomaUiHelper_83KI.MostrarError(this, ex, "Comun.Error", MessageBoxIcon.Warning);
            }
            catch (UsuarioBloqueadoException_83KI ex)
            {
                Show();
                IdiomaUiHelper_83KI.MostrarError(this, ex, "Comun.Seguridad", MessageBoxIcon.Stop);
            }
            catch (UsuarioDeshabilitadoException_83KI ex)
            {
                Show();
                IdiomaUiHelper_83KI.MostrarError(this, ex, "Comun.Usuarios", MessageBoxIcon.Stop);
            }
            catch (ContrasenaInvalidaException_83KI ex)
            {
                Show();
                IdiomaUiHelper_83KI.MostrarError(this, ex, "Comun.Error", MessageBoxIcon.Warning);
            }
        }

        private bool MostrarAdvertenciaYForzarCambio()
        {
            IdiomaUiHelper_83KI.MostrarAdvertencia(
                this,
                "Login.ContrasenaDefaultMensaje",
                "Login.ContrasenaDefaultTitulo");

            using (var frmCambio = new FrmCambiarContrasena(_gestor, forzarCambio: true))
            {
                var resultadoCambio = frmCambio.ShowDialog(this);

                if (resultadoCambio == DialogResult.OK)
                {
                    IdiomaUiHelper_83KI.MostrarInformacion(this, "Login.ContrasenaActualizada", "Comun.Informacion");
                    return true;
                }

                // El usuario cancela o cierra el dialogo - el login abortara
                return false;
            }
        }


        public void LoginDesignConfig()  //diseno de la interfaz
        {
            BackColor = Color.FromArgb(70, 130, 180);
            txt_userName.BackColor = Color.FromArgb(240, 240, 240);
            txt_Contrasena.BackColor = Color.FromArgb(240, 240, 240);
            //diseno imagen
            GraphicsPath path = new GraphicsPath();
        }
        private void RedondearPanel(Panel panel) //diseno del panel
        {
            GraphicsPath path = new GraphicsPath();
            int radio = 30;

            path.StartFigure();
            path.AddArc(0, 0, radio, radio, 180, 90);
            path.AddArc(panel.Width - radio, 0, radio, radio, 270, 90);
            path.AddArc(panel.Width - radio, panel.Height - radio, radio, radio, 0, 90);
            path.AddArc(0, panel.Height - radio, radio, radio, 90, 90);
            path.CloseFigure();

            panel.Region = new Region(path);
        }
        public void ButtonDesing(Button btn) //diseno del boton
        {
            btn.BackColor = Color.FromArgb(70, 130, 180);
            btn.ForeColor = Color.White;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            GraphicsPath path = new GraphicsPath();
            int radio = 20;

            path.StartFigure();
            path.AddArc(0, 0, radio, radio, 180, 90);
            path.AddArc(btn.Width - radio, 0, radio, radio, 270, 90);
            path.AddArc(btn.Width - radio, btn.Height - radio, radio, radio, 0, 90);
            path.AddArc(0, btn.Height - radio, radio, radio, 90, 90);
            path.CloseFigure();

            btn.Region = new Region(path);
        }

        private void AplicarTextosPorDefectoEspanol()
        {
            Text = "Login";
            lbl_Email.Text = "Usuario";
            lbl_Contrasena.Text = "Contraseña";
            btnLogin.Text = "Ingresar";
        }
    }
}

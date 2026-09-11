using Service.Excepciones.Login;
using Service.Interfaces;
using System;
using System.Windows.Forms;

namespace UI
{
    public partial class FrmCambiarContrasena : Form, IObservadorIdioma
    {
        private readonly IGestorUsuario_83KI _gestorUsuario;
        private readonly IGestorIdioma_83KI _gestorIdioma;
        private readonly bool _forzarCambio;

        public FrmCambiarContrasena(IGestorUsuario_83KI gestorUsuario, bool forzarCambio = false)
        {
            InitializeComponent();
            _gestorUsuario = gestorUsuario;
            _forzarCambio = forzarCambio;
            _gestorIdioma = Service.ServiceFactory_83KI.GetGestorIdioma();
            _gestorIdioma.Suscribir(this);
        }

        private void btnCambiarContrasena_Click(object sender, EventArgs e)
        {
            bool formularioValido = ValidarFormulario();
            if (formularioValido == false)
            {
                return;
            }
            try
            {
                _gestorUsuario.CambiarContrasenaUsuarioActual(txtContrasenaActual.Text, txtNuevaContrasena.Text);

                DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (ContrasenaInvalidaException_83KI ex)
            {
                IdiomaUiHelper_83KI.MostrarError(this, ex, "Comun.Validacion", MessageBoxIcon.Warning);
            }
            catch (UsuarioNoAutenticadoException_83KI ex)
            {
                IdiomaUiHelper_83KI.MostrarError(this, ex, "Comun.Sesion", MessageBoxIcon.Warning);
                this.Close();
            }
            catch (Exception ex)
            {
                IdiomaUiHelper_83KI.MostrarError(this, ex, "Comun.Error", MessageBoxIcon.Error);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private bool ValidarFormulario()
        {
            if (string.IsNullOrWhiteSpace(txtContrasenaActual.Text) ||
                string.IsNullOrWhiteSpace(txtNuevaContrasena.Text) ||
                string.IsNullOrWhiteSpace(txtConfirmarContrasena.Text))
            {
                IdiomaUiHelper_83KI.MostrarAdvertencia(this, "Validaciones.CamposObligatorios", "Comun.Validacion");
                return false;
            }

            if (txtNuevaContrasena.Text != txtConfirmarContrasena.Text)
            {
                IdiomaUiHelper_83KI.MostrarAdvertencia(this, "Validaciones.ContrasenasNoCoinciden", "Comun.Validacion");
                return false;
            }
            if (txtNuevaContrasena.Text == txtContrasenaActual.Text)
            {
                IdiomaUiHelper_83KI.MostrarAdvertencia(this, "Validaciones.ContrasenaNuevaDiferente", "Comun.Validacion");
                return false;
            }

            return true;
        }

        public void ActualizarIdioma(IIdioma idioma)
        {
            Text = IdiomaUiHelper_83KI.Texto("FrmCambiarContrasena.Titulo");
            lblContrasenaActual.Text = IdiomaUiHelper_83KI.Texto("FrmCambiarContrasena.ContrasenaActual");
            lblNuevaContrasena.Text = IdiomaUiHelper_83KI.Texto("FrmCambiarContrasena.NuevaContrasena");
            lblConfirmarContrasena.Text = IdiomaUiHelper_83KI.Texto("FrmCambiarContrasena.ConfirmarContrasena");
            btnCambiarContrasena.Text = IdiomaUiHelper_83KI.Texto("FrmCambiarContrasena.CambiarContrasena");
            btnCancelar.Text = IdiomaUiHelper_83KI.Texto("Comun.Cancelar");
            btnCancelar.Visible = !_forzarCambio;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _gestorIdioma.Desuscribir(this);
            base.OnFormClosed(e);
        }
    }
}

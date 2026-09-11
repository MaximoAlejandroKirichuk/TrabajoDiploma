using Service.Entidades;
using Service.Interfaces;
using System;
using System.Windows.Forms;

namespace UI
{
    public partial class FrmConfirmarEstadoUsuario : Form, IObservadorIdioma
    {
        private readonly Usuario_83KI _usuario;
        private readonly IGestorIdioma_83KI _gestorIdioma;

        public FrmConfirmarEstadoUsuario(Usuario_83KI usuario)
        {
            _usuario = usuario ?? throw new ArgumentNullException(nameof(usuario));
            InitializeComponent();
            _gestorIdioma = Service.ServiceFactory_83KI.GetGestorIdioma();
            CargarDatos();
            _gestorIdioma.Suscribir(this);
        }

        private void CargarDatos()
        {
            txtNombre.Text = _usuario.Nombre;
            txtApellido.Text = _usuario.Apellido;
            txtDni.Text = _usuario.DNI.ToString();
            txtEmail.Text = _usuario.Email;
            txtRol.Text = _usuario.Rol.ToString();
            ActualizarIdioma(_gestorIdioma.IdiomaActual);
        }

        private void btnConfirmar_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        public void ActualizarIdioma(IIdioma idioma)
        {
            Text = IdiomaUiHelper_83KI.Texto("FrmConfirmarEstadoUsuario.Titulo");
            lblNombre.Text = IdiomaUiHelper_83KI.Texto("Comun.Nombre");
            lblApellido.Text = IdiomaUiHelper_83KI.Texto("Comun.Apellido");
            lblDni.Text = IdiomaUiHelper_83KI.Texto("Comun.Dni");
            lblEmail.Text = IdiomaUiHelper_83KI.Texto("Comun.Email");
            lblRol.Text = IdiomaUiHelper_83KI.Texto("Comun.Rol");
            lblTitulo.Text = _usuario.Activo
                ? IdiomaUiHelper_83KI.Texto("FrmConfirmarEstadoUsuario.ConfirmarDeshabilitacionTitulo")
                : IdiomaUiHelper_83KI.Texto("FrmConfirmarEstadoUsuario.ConfirmarHabilitacionTitulo");
            btnConfirmar.Text = _usuario.Activo
                ? IdiomaUiHelper_83KI.Texto("FrmConfirmarEstadoUsuario.ConfirmarDeshabilitacion")
                : IdiomaUiHelper_83KI.Texto("FrmConfirmarEstadoUsuario.ConfirmarHabilitacion");
            btnCancelar.Text = IdiomaUiHelper_83KI.Texto("Comun.Cancelar");
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _gestorIdioma.Desuscribir(this);
            base.OnFormClosed(e);
        }
    }
}

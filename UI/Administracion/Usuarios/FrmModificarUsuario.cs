using Service.Entidades;
using Service.Interfaces;
using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace UI
{
    public partial class FrmModificarUsuario : Form, IObservadorIdioma
    {
        private readonly IGestorUsuario_83KI _gestorUsuario;
        private readonly IGestorRol_83KI _gestorRol;
        private readonly IGestorIdioma_83KI _gestorIdioma;
        private readonly Usuario_83KI _usuarioOriginal;

        public FrmModificarUsuario(IGestorUsuario_83KI gestorUsuario, IGestorRol_83KI gestorRol, Usuario_83KI usuario)
        {
            _gestorUsuario = gestorUsuario ?? throw new ArgumentNullException(nameof(gestorUsuario));
            _gestorRol = gestorRol ?? throw new ArgumentNullException(nameof(gestorRol));
            _usuarioOriginal = usuario ?? throw new ArgumentNullException(nameof(usuario));

            InitializeComponent();
            _gestorIdioma = Service.ServiceFactory_83KI.GetGestorIdioma();
            CargarRoles();
            CargarDatos();
            _gestorIdioma.Suscribir(this);
        }

        private void CargarRoles()
        {
            cmbRol.DataSource = null;
            cmbRol.DisplayMember = nameof(Rol_83KI.Nombre);
            cmbRol.ValueMember = nameof(Rol_83KI.CodigoRol);
            cmbRol.DataSource = _gestorRol.ObtenerRoles();
        }

        private void CargarDatos()
        {
            txtNombre.Text = _usuarioOriginal.Nombre;
            txtApellido.Text = _usuarioOriginal.Apellido;
            txtDni.Text = _usuarioOriginal.DNI.ToString();
            txtEmail.Text = _usuarioOriginal.Email;
            cmbRol.SelectedValue = _usuarioOriginal.Rol.CodigoRol;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            string emailNormalizado = txtEmail.Text.Trim();
            try
            {
                ValidarDatos(emailNormalizado);
                _gestorUsuario.ModificarUsuario(_usuarioOriginal.DNI, emailNormalizado, ObtenerRolSeleccionado());
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                IdiomaUiHelper_83KI.MostrarError(this, ex, "FrmModificarUsuario.Titulo", MessageBoxIcon.Warning);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private Rol_83KI ObtenerRolSeleccionado()
        {
            if (cmbRol.SelectedItem is Rol_83KI rol)
            {
                return rol;
            }
            return _usuarioOriginal.Rol;
        }
        private void ValidarDatos(string emailNormalizado)
        {
            string patronEmail = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (string.IsNullOrWhiteSpace(emailNormalizado))
            {
                throw new InvalidOperationException("El email es obligatorio.");
            }
            if (!Regex.IsMatch(emailNormalizado, patronEmail))
            {
                throw new InvalidOperationException("El formato del email no es valido.");
            }
        }

        public void ActualizarIdioma(IIdioma idioma)
        {
            Text = IdiomaUiHelper_83KI.Texto("FrmModificarUsuario.Titulo");
            lblNombre.Text = IdiomaUiHelper_83KI.Texto("Comun.Nombre");
            lblApellido.Text = IdiomaUiHelper_83KI.Texto("Comun.Apellido");
            lblDni.Text = IdiomaUiHelper_83KI.Texto("Comun.Dni");
            lblEmail.Text = IdiomaUiHelper_83KI.Texto("Comun.Email");
            lblRol.Text = IdiomaUiHelper_83KI.Texto("Comun.Rol");
            btnGuardar.Text = IdiomaUiHelper_83KI.Texto("FrmModificarUsuario.Guardar");
            btnCancelar.Text = IdiomaUiHelper_83KI.Texto("Comun.Cancelar");
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _gestorIdioma.Desuscribir(this);
            base.OnFormClosed(e);
        }
    }
}

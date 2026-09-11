using Service.Entidades;
using Service.Excepciones.CrearUsuario;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UI
{
    public partial class FrmCrearUsuario : Form, IObservadorIdioma
    {
        private readonly IGestorUsuario_83KI _usuarioService;
        private readonly IGestorRol_83KI _gestorRol;
        private readonly IGestorIdioma_83KI _gestorIdioma;

        public FrmCrearUsuario(IGestorUsuario_83KI gestorUsuario, IGestorRol_83KI gestorRol)
        {
            InitializeComponent();
            _usuarioService = gestorUsuario;
            _gestorRol = gestorRol;
            _gestorIdioma = Service.ServiceFactory_83KI.GetGestorIdioma();
            CargarRoles();
            _gestorIdioma.Suscribir(this);
        }

        private void CargarRoles()
        {
            comboBox1.DataSource = null;
            comboBox1.DisplayMember = nameof(Rol_83KI.Nombre);
            comboBox1.ValueMember = nameof(Rol_83KI.CodigoRol);
            comboBox1.DataSource = _gestorRol.ObtenerRoles();
        }

        private void btnCrearUsuario_Click(object sender, EventArgs e)
        {
            if (!ValidarDatosFormato()) return;
            try
            {
                CrearUsuarioDesdeFormulario();
                IdiomaUiHelper_83KI.MostrarInformacion(this, "FrmCrearUsuario.UsuarioRegistrado", "Comun.Informacion");
                this.DialogResult = DialogResult.OK;
            }

            catch (DniRegistradoException_83KI ex)
            {
                IdiomaUiHelper_83KI.MostrarError(this, ex, "Comun.Usuarios", MessageBoxIcon.Warning);
            }
            catch (EmailRegistradoException_83KI ex)
            {
                IdiomaUiHelper_83KI.MostrarError(this, ex, "Comun.Usuarios", MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                IdiomaUiHelper_83KI.MostrarError(this, ex, "Comun.Error", MessageBoxIcon.Warning);
            }
        }
        private void CrearUsuarioDesdeFormulario()
        {
            int.TryParse(txt_Dni.Text, out int dni);
            if (!(comboBox1.SelectedItem is Rol_83KI rol))
            {
                throw new InvalidOperationException("El rol seleccionado no existe.");
            }
            _usuarioService.CrearUsuario(
                txtNombre.Text,
                txtApellido.Text,
                dni,
                txtEmail.Text,
                rol
            );
        }


        private bool ValidarDatosFormato()
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text) ||
                string.IsNullOrWhiteSpace(txtApellido.Text) ||
                string.IsNullOrWhiteSpace(txt_Dni.Text))
            {
                IdiomaUiHelper_83KI.MostrarAdvertencia(this, "Validaciones.CamposObligatorios", "Comun.Validacion");
                return false;
            }

            string patronEmail = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(txtEmail.Text, patronEmail))
            {
                IdiomaUiHelper_83KI.MostrarAdvertencia(this, "Validaciones.EmailFormatoInvalido", "Comun.Validacion");
                return false;
            }

            if (!long.TryParse(txt_Dni.Text, out _) || txt_Dni.Text.Length < 7)
            {
                IdiomaUiHelper_83KI.MostrarAdvertencia(this, "Validaciones.DniValido", "Comun.Validacion");
                return false;
            }

            if (comboBox1.SelectedItem == null)
            {
                IdiomaUiHelper_83KI.MostrarAdvertencia(this, "Validaciones.SeleccionarRol", "Comun.Validacion");
                return false;
            }

            return true;
        }
        public void FormDesign(Panel panel, Button btn)
        {
            //colores de los botones, txt, etc
            BackColor = Color.FromArgb(70, 130, 180);
            txtNombre.BackColor = Color.FromArgb(240, 240, 240);
            txtApellido.BackColor = Color.FromArgb(240, 240, 240);
            txtEmail.BackColor = Color.FromArgb(240, 240, 240);
            txt_Dni.BackColor = Color.FromArgb(240, 240, 240);
            comboBox1.BackColor = Color.FromArgb(240, 240, 240);
            btnCrearUsuario.BackColor = Color.FromArgb(240, 240, 240);

            //bordes del panel
            GraphicsPath path = new GraphicsPath();
            int radio = 30;

            path.StartFigure();
            path.AddArc(0, 0, radio, radio, 180, 90);
            path.AddArc(panel.Width - radio, 0, radio, radio, 270, 90);
            path.AddArc(panel.Width - radio, panel.Height - radio, radio, radio, 0, 90);
            path.AddArc(0, panel.Height - radio, radio, radio, 90, 90);
            path.CloseFigure();

            panel.Region = new Region(path);

            //color de boton
            btn.BackColor = Color.FromArgb(70, 130, 180);
            btn.ForeColor = Color.White;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            path.Reset();
            int radioBtn = btn.Height;
            path.StartFigure();
            path.AddArc(0, 0, radio, radio, 180, 90);
            path.AddArc(btn.Width - radio, 0, radio, radio, 270, 90);
            path.AddArc(btn.Width - radio, btn.Height - radio, radio, radio, 0, 90);
            path.AddArc(0, btn.Height - radio, radio, radio, 90, 90);
            path.CloseFigure();

            btn.Region = new Region(path);
        }
        private void FrmCrearUsuario_Load(object sender, EventArgs e)
        {
            FormDesign(panelCrearUsu, btnCrearUsuario); 
        }

        public void ActualizarIdioma(IIdioma idioma)
        {
            Text = IdiomaUiHelper_83KI.Texto("FrmCrearUsuario.Titulo");
            frm_lbl_nombre.Text = IdiomaUiHelper_83KI.Texto("Comun.Nombre");
            frm_lbl_apellido.Text = IdiomaUiHelper_83KI.Texto("Comun.Apellido");
            frm_lbl_email.Text = IdiomaUiHelper_83KI.Texto("Comun.Email");
            frm_lbl_dni.Text = IdiomaUiHelper_83KI.Texto("Comun.Dni");
            frm_lbl_rol.Text = IdiomaUiHelper_83KI.Texto("Comun.Rol");
            btnCrearUsuario.Text = IdiomaUiHelper_83KI.Texto("FrmCrearUsuario.CrearUsuario");
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _gestorIdioma.Desuscribir(this);
            base.OnFormClosed(e);
        }
    }
}

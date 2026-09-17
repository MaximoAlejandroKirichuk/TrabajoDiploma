using Service;
using BE.Entidades;
using Service.Entidades;
using Service.Interfaces;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace UI.Modulos.Maestros
{
    public class FrmCrearProfesor_83KI : Form, IObservadorIdioma
    {
        private readonly IGestorProfesor_83KI _gestorProfesor;
        private readonly IGestorIdioma_83KI _gestorIdioma;
        private Label lblDni;
        private Label lblNombre;
        private Label lblApellido;
        private Label lblEmail;
        private TextBox txtDni;
        private TextBox txtNombre;
        private TextBox txtApellido;
        private TextBox txtEmail;
        private Button btnGuardar;
        private Button btnCancelar;

        public FrmCrearProfesor_83KI(IGestorProfesor_83KI gestorProfesor)
        {
            _gestorProfesor = gestorProfesor;
            _gestorIdioma = ServiceFactory_83KI.GetGestorIdioma();
            InicializarControles();
            _gestorIdioma.Suscribir(this);
        }

        private void InicializarControles()
        {
            Width = 380;
            Height = 330;
            StartPosition = FormStartPosition.CenterParent;
            Font = new Font("Segoe UI", 10F);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            lblDni = new Label { Left = 20, Top = 20, Width = 320 };
            Controls.Add(lblDni);
            txtDni = new TextBox { Left = 20, Top = 45, Width = 320 };
            Controls.Add(txtDni);
            lblNombre = new Label { Left = 20, Top = 80, Width = 320 };
            Controls.Add(lblNombre);
            txtNombre = new TextBox { Left = 20, Top = 105, Width = 320 };
            Controls.Add(txtNombre);
            lblApellido = new Label { Left = 20, Top = 140, Width = 320 };
            Controls.Add(lblApellido);
            txtApellido = new TextBox { Left = 20, Top = 165, Width = 320 };
            Controls.Add(txtApellido);
            lblEmail = new Label { Left = 20, Top = 200, Width = 320 };
            Controls.Add(lblEmail);
            txtEmail = new TextBox { Left = 20, Top = 225, Width = 320 };
            Controls.Add(txtEmail);

            btnGuardar = new Button { Left = 100, Top = 260, Width = 110, Height = 30 };
            btnGuardar.Click += btnGuardar_Click;
            Controls.Add(btnGuardar);
            btnCancelar = new Button { Left = 230, Top = 260, Width = 110, Height = 30, DialogResult = DialogResult.Cancel };
            Controls.Add(btnCancelar);

            AcceptButton = btnGuardar;
            CancelButton = btnCancelar;
            ActualizarIdioma(null);
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                _gestorProfesor.CrearProfesor(txtDni.Text, txtNombre.Text, txtApellido.Text, txtEmail.Text);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                IdiomaUiHelper_83KI.MostrarError(this, ex, "Comun.Error", MessageBoxIcon.Warning);
            }
        }

        public void ActualizarIdioma(IIdioma idioma)
        {
            Text = IdiomaUiHelper_83KI.Texto("FrmCrearProfesor.Titulo");
            lblDni.Text = IdiomaUiHelper_83KI.Texto("Comun.Dni");
            lblNombre.Text = IdiomaUiHelper_83KI.Texto("Comun.Nombre");
            lblApellido.Text = IdiomaUiHelper_83KI.Texto("Comun.Apellido");
            lblEmail.Text = IdiomaUiHelper_83KI.Texto("Comun.Email");
            btnGuardar.Text = IdiomaUiHelper_83KI.Texto("FrmGestionProfesores.Crear");
            btnCancelar.Text = IdiomaUiHelper_83KI.Texto("Comun.Cancelar");
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _gestorIdioma.Desuscribir(this);
            base.OnFormClosed(e);
        }
    }
}

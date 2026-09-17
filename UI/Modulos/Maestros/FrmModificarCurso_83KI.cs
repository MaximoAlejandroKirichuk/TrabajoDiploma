using Service;
using BE.Entidades;
using Service.Entidades;
using Service.Interfaces;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace UI.Modulos.Maestros
{
    public class FrmModificarCurso_83KI : Form, IObservadorIdioma
    {
        private readonly IGestorCurso_83KI _gestorCurso;
        private readonly IGestorIdioma_83KI _gestorIdioma;
        private readonly Curso_83KI _curso;
        private Label lblNombre;
        private Label lblDescripcion;
        private Label lblCargaHoraria;
        private TextBox txtNombre;
        private TextBox txtDescripcion;
        private NumericUpDown nudCargaHoraria;
        private Button btnGuardar;
        private Button btnCancelar;

        public FrmModificarCurso_83KI(IGestorCurso_83KI gestorCurso, Curso_83KI curso)
        {
            _gestorCurso = gestorCurso;
            _curso = curso;
            _gestorIdioma = ServiceFactory_83KI.GetGestorIdioma();
            InicializarControles();
            _gestorIdioma.Suscribir(this);
        }

        private void InicializarControles()
        {
            Width = 420;
            Height = 390;
            StartPosition = FormStartPosition.CenterParent;
            Font = new Font("Segoe UI", 10F);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            lblNombre = new Label { Left = 20, Top = 20, Width = 360 };
            Controls.Add(lblNombre);
            txtNombre = new TextBox { Left = 20, Top = 45, Width = 360, Text = _curso.Nombre };
            Controls.Add(txtNombre);
            lblDescripcion = new Label { Left = 20, Top = 80, Width = 360 };
            Controls.Add(lblDescripcion);
            txtDescripcion = new TextBox { Left = 20, Top = 105, Width = 360, Height = 70, Multiline = true, ScrollBars = ScrollBars.Vertical, Text = _curso.Descripcion };
            Controls.Add(txtDescripcion);
            lblCargaHoraria = new Label { Left = 20, Top = 185, Width = 360 };
            Controls.Add(lblCargaHoraria);
            nudCargaHoraria = new NumericUpDown { Left = 20, Top = 210, Width = 120, Minimum = 1, Maximum = 10000, Value = Math.Max(1, Math.Min(10000, _curso.CargaHoraria)) };
            Controls.Add(nudCargaHoraria);

            btnGuardar = new Button { Left = 140, Top = 275, Width = 110, Height = 30 };
            btnGuardar.Click += btnGuardar_Click;
            Controls.Add(btnGuardar);
            btnCancelar = new Button { Left = 270, Top = 275, Width = 110, Height = 30, DialogResult = DialogResult.Cancel };
            Controls.Add(btnCancelar);

            AcceptButton = btnGuardar;
            CancelButton = btnCancelar;
            ActualizarIdioma(null);
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                _gestorCurso.ModificarCurso(_curso.IdCurso, txtNombre.Text, txtDescripcion.Text, Convert.ToInt32(nudCargaHoraria.Value));
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
            Text = IdiomaUiHelper_83KI.Texto("FrmModificarCurso.Titulo");
            lblNombre.Text = IdiomaUiHelper_83KI.Texto("Comun.Nombre");
            lblDescripcion.Text = IdiomaUiHelper_83KI.Texto("FrmGestionCursos.Descripcion");
            lblCargaHoraria.Text = IdiomaUiHelper_83KI.Texto("FrmGestionCursos.CargaHoraria");
            btnGuardar.Text = IdiomaUiHelper_83KI.Texto("Comun.Guardar");
            btnCancelar.Text = IdiomaUiHelper_83KI.Texto("Comun.Cancelar");
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _gestorIdioma.Desuscribir(this);
            base.OnFormClosed(e);
        }
    }
}

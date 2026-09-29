using BE.Entidades;
using Service;
using Service.Interfaces;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace UI.Modulos.PlanificacionAcademica
{
    public partial class FrmEditarComision_83KI : Form, IObservadorIdioma
    {
        private readonly IGestorComision_83KI _gestor;
        private readonly IGestorIdioma_83KI _gestorIdioma;
        private readonly Comision_83KI _comision;
        private Label[] _etiquetas;
        private NumericUpDown nudCupoMinimo;
        private NumericUpDown nudCupoMaximo;
        private NumericUpDown nudArancelBase;
        private DateTimePicker dtpHoraInicio;
        private DateTimePicker dtpHoraFin;
        private DateTimePicker dtpFechaLimitePago;
        private DateTimePicker dtpFechaInicio;
        private DateTimePicker dtpFechaFin;
        private Button btnGuardar;
        private Button btnCancelar;
        private Label lblMensaje;

        public FrmEditarComision_83KI(IGestorComision_83KI gestor, Comision_83KI comision)
        {
            _gestor = gestor;
            _comision = comision;
            _gestorIdioma = ServiceFactory_83KI.GetGestorIdioma();
            InitializeComponent();
            InicializarControles();
            CargarDatos();
            _gestorIdioma.Suscribir(this);
        }

        private void InicializarControles()
        {
            Width = 520;
            Height = 430;
            StartPosition = FormStartPosition.CenterParent;
            Font = new Font("Segoe UI", 10F);
            var tabla = new TableLayoutPanel { Left = 16, Top = 16, Width = 460, Height = 300, ColumnCount = 2, RowCount = 8 };
            tabla.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 180F));
            tabla.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            for (int i = 0; i < 8; i++) tabla.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));
            nudCupoMinimo = CrearNumeric(1, 999, 0);
            nudCupoMaximo = CrearNumeric(1, 999, 0);
            nudArancelBase = CrearNumeric(1, 999999999, 2);
            dtpHoraInicio = CrearHora();
            dtpHoraFin = CrearHora();
            dtpFechaLimitePago = CrearFecha();
            dtpFechaInicio = CrearFecha();
            dtpFechaFin = CrearFecha();
            var controles = new Control[] { dtpHoraInicio, dtpHoraFin, nudCupoMinimo, nudCupoMaximo, dtpFechaLimitePago, dtpFechaInicio, dtpFechaFin, nudArancelBase };
            _etiquetas = new Label[8];
            for (int i = 0; i < 8; i++)
            {
                var lbl = new Label { AutoSize = true, Anchor = AnchorStyles.Left };
                tabla.Controls.Add(lbl, 0, i);
                tabla.Controls.Add(controles[i], 1, i);
                _etiquetas[i] = lbl;
            }
            Controls.Add(tabla);
            lblMensaje = new Label { Left = 16, Top = 320, Width = 460, Height = 38, ForeColor = Color.DarkBlue };
            Controls.Add(lblMensaje);
            btnGuardar = new Button { Left = 220, Top = 365, Width = 120 };
            btnGuardar.Click += btnGuardar_Click;
            Controls.Add(btnGuardar);
            btnCancelar = new Button { Left = 356, Top = 365, Width = 120, DialogResult = DialogResult.Cancel };
            Controls.Add(btnCancelar);
            ActualizarIdioma(null);
        }

        private NumericUpDown CrearNumeric(int min, int max, int decimales)
        {
            return new NumericUpDown { Minimum = min, Maximum = max, DecimalPlaces = decimales, Dock = DockStyle.Left, Width = 160 };
        }

        private DateTimePicker CrearFecha()
        {
            return new DateTimePicker { Format = DateTimePickerFormat.Short, Dock = DockStyle.Left, Width = 160 };
        }

        private DateTimePicker CrearHora()
        {
            return new DateTimePicker { Format = DateTimePickerFormat.Time, ShowUpDown = true, Dock = DockStyle.Left, Width = 160 };
        }

        private void CargarDatos()
        {
            dtpHoraInicio.Value = DateTime.Today.Add(_comision.HoraInicio);
            dtpHoraFin.Value = DateTime.Today.Add(_comision.HoraFin);
            nudCupoMinimo.Value = _comision.CupoMinimo;
            nudCupoMaximo.Value = _comision.CupoMaximo;
            dtpFechaLimitePago.Value = _comision.FechaLimitePago;
            dtpFechaInicio.Value = _comision.FechaInicio;
            dtpFechaFin.Value = _comision.FechaFin;
            nudArancelBase.Value = _comision.ArancelBase;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                _comision.ActualizarDatos(_comision.IdCurso, _comision.IdProfesor, _comision.DiaSemana, dtpHoraInicio.Value.TimeOfDay, dtpHoraFin.Value.TimeOfDay, (int)nudCupoMinimo.Value, (int)nudCupoMaximo.Value, dtpFechaLimitePago.Value.Date, dtpFechaInicio.Value.Date, dtpFechaFin.Value.Date, nudArancelBase.Value);
                _gestor.ModificarComision(_comision);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                lblMensaje.Text = IdiomaUiHelper_83KI.TraducirExcepcion(ex);
            }
        }

        public void ActualizarIdioma(Service.Entidades.IIdioma idioma)
        {
            Text = IdiomaUiHelper_83KI.Texto("FrmEditarComision.Titulo");
            var claves = new[] { "FrmRegistrarPreaperturaComision.HoraInicio", "FrmRegistrarPreaperturaComision.HoraFin",
                "FrmRegistrarPreaperturaComision.CupoMinimo", "FrmRegistrarPreaperturaComision.CupoMaximo",
                "FrmRegistrarPreaperturaComision.FechaLimitePago", "FrmRegistrarPreaperturaComision.FechaInicio",
                "FrmRegistrarPreaperturaComision.FechaFin", "FrmRegistrarPreaperturaComision.ArancelBase" };
            for (int i = 0; i < 8 && i < _etiquetas.Length; i++)
            {
                if (_etiquetas[i] != null) _etiquetas[i].Text = IdiomaUiHelper_83KI.Texto(claves[i]);
            }
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

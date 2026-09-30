using BE.Entidades;
using Service;
using Service.DTOs;
using Service.Entidades;
using Service.Interfaces;
using System;
using System.Linq;
using System.Windows.Forms;

namespace UI.Modulos.PreInscripcion
{
    public partial class FrmRegistrarPagoInscripcion_83KI : Form, IObservadorIdioma
    {
        private readonly IPagoInscripcionBLL_83KI _pagoBLL;
        private readonly IGestorIdioma_83KI _gestorIdioma;
        private BusquedaPagoInscripcionResultado_83KI _resultadoBusqueda;

        public FrmRegistrarPagoInscripcion_83KI(IPagoInscripcionBLL_83KI pagoBLL)
        {
            _pagoBLL = pagoBLL;
            _gestorIdioma = ServiceFactory_83KI.GetGestorIdioma();
            InitializeComponent();
            ConfigurarGrid();
            CargarMetodosPago();
            HabilitarFormularioPago(false);
            _gestorIdioma.Suscribir(this);
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            BuscarCuotasPendientes();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            GuardarPago();
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
        }

        private void dgvCuotas_SelectionChanged(object sender, EventArgs e)
        {
            CargarCuotaSeleccionada();
        }

        private void ConfigurarGrid()
        {
            dgvCuotas.Columns.Clear();
            dgvCuotas.Columns.Add(new DataGridViewTextBoxColumn { Name = "IdCuota", DataPropertyName = "IdCuota", Visible = false });
            dgvCuotas.Columns.Add(new DataGridViewTextBoxColumn { Name = "NumeroCuota", HeaderText = "Cuota", DataPropertyName = "NumeroCuota", ReadOnly = true });
            dgvCuotas.Columns.Add(new DataGridViewTextBoxColumn { Name = "MontoOriginal", HeaderText = "Monto original", DataPropertyName = "MontoOriginal", ReadOnly = true });
            dgvCuotas.Columns.Add(new DataGridViewTextBoxColumn { Name = "BalanceAdeudado", HeaderText = "Balance adeudado", DataPropertyName = "BalanceAdeudado", ReadOnly = true });
            dgvCuotas.Columns.Add(new DataGridViewTextBoxColumn { Name = "FechaVencimiento", HeaderText = "Vencimiento", DataPropertyName = "FechaVencimiento", ReadOnly = true });
        }

        private void CargarMetodosPago()
        {
            cmbMetodoPago.Items.Clear();
            cmbMetodoPago.Items.Add(PagoInscripcion_83KI.MetodoTransferencia);
            cmbMetodoPago.Items.Add(PagoInscripcion_83KI.MetodoTarjeta);
            cmbMetodoPago.Items.Add(PagoInscripcion_83KI.MetodoBilleteraVirtual);
            cmbMetodoPago.SelectedIndex = 0;
            dtpFechaPago.Value = DateTime.Today;
        }

        private void BuscarCuotasPendientes()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtDni.Text))
                {
                    lblMensaje.Text = IdiomaUiHelper_83KI.Texto("Errores.PagoDniObligatorio");
                    HabilitarFormularioPago(false);
                    return;
                }

                _resultadoBusqueda = _pagoBLL.BuscarCuotasPendientes(txtDni.Text);
                if (_resultadoBusqueda == null || _resultadoBusqueda.CuotasPendientes == null || _resultadoBusqueda.CuotasPendientes.Count == 0)
                {
                    lblAlumno.Text = string.Empty;
                    lblSolicitud.Text = string.Empty;
                    dgvCuotas.Rows.Clear();
                    HabilitarFormularioPago(false);
                    lblMensaje.Text = IdiomaUiHelper_83KI.Texto("FrmRegistrarPagoInscripcion.SinCuotasPendientes");
                    return;
                }

                lblAlumno.Text = IdiomaUiHelper_83KI.Texto("FrmRegistrarPagoInscripcion.AlumnoEncontrado", _resultadoBusqueda.Alumno.Nombre, _resultadoBusqueda.Alumno.Apellido, _resultadoBusqueda.Alumno.DNI);
                lblSolicitud.Text = IdiomaUiHelper_83KI.Texto("FrmRegistrarPagoInscripcion.SolicitudEncontrada", _resultadoBusqueda.Solicitud.Codigo, _resultadoBusqueda.CodigoComision, _resultadoBusqueda.Curso);

                dgvCuotas.Rows.Clear();
                foreach (var cuota in _resultadoBusqueda.CuotasPendientes)
                {
                    dgvCuotas.Rows.Add(cuota.IdCuota, cuota.NumeroCuota, cuota.MontoOriginal, cuota.BalanceAdeudado, cuota.FechaVencimiento.ToShortDateString());
                }

                if (dgvCuotas.Rows.Count > 0) dgvCuotas.Rows[0].Selected = true;
                HabilitarFormularioPago(true);
                lblMensaje.Text = string.Empty;
            }
            catch (Exception ex)
            {
                HabilitarFormularioPago(false);
                MostrarError(ex);
            }
        }

        private void CargarCuotaSeleccionada()
        {
            if (dgvCuotas.SelectedRows.Count == 0) return;
            var row = dgvCuotas.SelectedRows[0];
            if (row.Cells["BalanceAdeudado"].Value != null)
                txtMonto.Text = row.Cells["BalanceAdeudado"].Value.ToString();
        }

        private void GuardarPago()
        {
            try
            {
                if (dgvCuotas.SelectedRows.Count == 0)
                {
                    lblMensaje.Text = IdiomaUiHelper_83KI.Texto("Errores.PagoCuotaObligatoria");
                    return;
                }

                int idCuota = Convert.ToInt32(dgvCuotas.SelectedRows[0].Cells["IdCuota"].Value);
                var resultado = _pagoBLL.RegistrarPagoInscripcion(new PagoInscripcionRequest_83KI
                {
                    DNI = txtDni.Text,
                    IdCuota = idCuota,
                    MetodoPago = cmbMetodoPago.SelectedItem != null ? cmbMetodoPago.SelectedItem.ToString() : string.Empty,
                    MontoRecibido = decimal.TryParse(txtMonto.Text, out decimal monto) ? monto : 0,
                    NumeroReferencia = txtNumeroReferencia.Text,
                    FechaPago = dtpFechaPago.Value
                });

                MostrarConfirmacion(resultado);
            }
            catch (Exception ex) { MostrarError(ex); }
        }

        public void HabilitarFormularioPago(bool habilitado)
        {
            grpPago.Enabled = habilitado;
            btnGuardar.Enabled = habilitado;
            dgvCuotas.Enabled = habilitado;
        }

        private void MostrarConfirmacion(PagoInscripcionResultado_83KI resultado)
        {
            string mensaje = IdiomaUiHelper_83KI.Texto(
                "FrmRegistrarPagoInscripcion.Registrado",
                resultado.Alumno.Nombre,
                resultado.Alumno.DNI,
                resultado.CodigoSolicitud,
                resultado.Cuota.NumeroCuota,
                resultado.Pago.MontoPagado,
                resultado.Pago.MetodoPago,
                resultado.Pago.NumeroReferencia);

            if (resultado.AuditoriaFallida)
                mensaje += Environment.NewLine + IdiomaUiHelper_83KI.Texto("FrmRegistrarPagoInscripcion.AuditoriaFallida");

            MessageBox.Show(this, mensaje, IdiomaUiHelper_83KI.Texto("Comun.Informacion"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            DialogResult = DialogResult.OK;
            Close();
        }

        private void MostrarError(Exception ex)
        {
            lblMensaje.Text = IdiomaUiHelper_83KI.TraducirExcepcion(ex);
            IdiomaUiHelper_83KI.MostrarError(this, ex, "Comun.Validacion", MessageBoxIcon.Warning);
        }

        private void LimpiarFormulario()
        {
            txtDni.Clear();
            txtMonto.Clear();
            txtNumeroReferencia.Clear();
            cmbMetodoPago.SelectedIndex = 0;
            dtpFechaPago.Value = DateTime.Today;
            dgvCuotas.Rows.Clear();
            lblAlumno.Text = string.Empty;
            lblSolicitud.Text = string.Empty;
            lblMensaje.Text = string.Empty;
            _resultadoBusqueda = null;
            HabilitarFormularioPago(false);
        }

        public void ActualizarIdioma(IIdioma idioma)
        {
            Text = IdiomaUiHelper_83KI.Texto("FrmRegistrarPagoInscripcion.Titulo");
            grpBusqueda.Text = IdiomaUiHelper_83KI.Texto("FrmRegistrarPagoInscripcion.Busqueda");
            lblDni.Text = IdiomaUiHelper_83KI.Texto("Comun.Dni");
            btnBuscar.Text = IdiomaUiHelper_83KI.Texto("FrmRegistrarPagoInscripcion.Buscar");
            grpPago.Text = IdiomaUiHelper_83KI.Texto("FrmRegistrarPagoInscripcion.DatosPago");
            lblMonto.Text = IdiomaUiHelper_83KI.Texto("FrmRegistrarPagoInscripcion.MontoRecibido");
            lblMetodoPago.Text = IdiomaUiHelper_83KI.Texto("FrmRegistrarPagoInscripcion.MetodoPago");
            lblNumeroReferencia.Text = IdiomaUiHelper_83KI.Texto("FrmRegistrarPagoInscripcion.NumeroReferencia");
            lblFechaPago.Text = IdiomaUiHelper_83KI.Texto("FrmRegistrarPagoInscripcion.FechaPago");
            btnGuardar.Text = IdiomaUiHelper_83KI.Texto("Comun.Guardar");
            btnLimpiar.Text = IdiomaUiHelper_83KI.Texto("Comun.Limpiar");
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _gestorIdioma.Desuscribir(this);
            base.OnFormClosed(e);
        }
    }
}

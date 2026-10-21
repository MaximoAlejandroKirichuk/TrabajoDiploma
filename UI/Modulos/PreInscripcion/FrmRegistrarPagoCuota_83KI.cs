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
    public partial class FrmRegistrarPagoCuota_83KI : Form, IObservadorIdioma
    {
        private readonly IPagoCuotaBLL_83KI _pagoBLL;
        private readonly IGestorIdioma_83KI _gestorIdioma;
        private BusquedaPagoInscripcionResultado_83KI _resultadoBusqueda;
        private PagoInscripcionResultado_83KI _pagoPendienteDeGuardarFactura;
        private readonly Label _lblPlanDePago = new Label { AutoSize = true, Location = new System.Drawing.Point(18, 106) };

        public FrmRegistrarPagoCuota_83KI(IPagoCuotaBLL_83KI pagoBLL)
        {
            _pagoBLL = pagoBLL;
            _gestorIdioma = ServiceFactory_83KI.GetGestorIdioma();
            InitializeComponent();
            grpBusqueda.Controls.Add(_lblPlanDePago);
            _lblPlanDePago.MaximumSize = new System.Drawing.Size(720, 0);
            grpBusqueda.Height = 154;
            dgvCuotas.Top = 176;
            dgvCuotas.Height = 190;
            grpPago.Top = 386;
            btnGuardar.Top = btnLimpiar.Top = grpPago.Bottom + 8;
            lblMensaje.Top = btnGuardar.Bottom + 12;
            ClientSize = new System.Drawing.Size(795, lblMensaje.Top + 40);
            cmbComision.SelectedValueChanged += cmbComision_SelectedValueChanged;
            ConfigurarGrid();
            CargarMetodosPago();
            ConfigurarFechas();
            HabilitarFormularioPago(false);
            _gestorIdioma.Suscribir(this);
        }

        private void btnBuscar_Click(object sender, EventArgs e) => BuscarCuotasPendientes();

        private void txtDni_TextChanged(object sender, EventArgs e)
        {
            cmbComision.DataSource = null;
            _resultadoBusqueda = null;
            dgvCuotas.Rows.Clear();
            lblAlumno.Text = string.Empty;
            lblSolicitud.Text = string.Empty;
            _lblPlanDePago.Text = string.Empty;
            lblMensaje.Text = string.Empty;
            HabilitarFormularioPago(false);
        }

        private void txtDni_Leave(object sender, EventArgs e)
        {
            cmbComision.DataSource = null;
            if (string.IsNullOrWhiteSpace(txtDni.Text)) return;
            try
            {
                var opciones = _pagoBLL.ListarComisionesConCuotasPendientes(txtDni.Text).ToList();
                cmbComision.DataSource = opciones;
                cmbComision.DisplayMember = nameof(ComisionPagoOpcion_83KI.Descripcion);
                cmbComision.ValueMember = nameof(ComisionPagoOpcion_83KI.Codigo);
                lblMensaje.Text = opciones.Count == 0 ? IdiomaUiHelper_83KI.Texto("FrmRegistrarPagoCuota.SinCuotas") : string.Empty;
            }
            catch (Exception ex) { MostrarError(ex); }
        }

        private void btnGuardar_Click(object sender, EventArgs e) => GuardarPago();
        private void btnLimpiar_Click(object sender, EventArgs e) => LimpiarFormulario();
        private void dgvCuotas_SelectionChanged(object sender, EventArgs e) => CargarCuotaSeleccionada();

        private void cmbComision_SelectedValueChanged(object sender, EventArgs e)
        {
            _resultadoBusqueda = null;
            dgvCuotas.Rows.Clear();
            lblAlumno.Text = lblSolicitud.Text = _lblPlanDePago.Text = lblMensaje.Text = string.Empty;
            txtMonto.Clear();
            HabilitarFormularioPago(false);
        }

        private void ConfigurarGrid()
        {
            dgvCuotas.Columns.Clear();
            dgvCuotas.Columns.Add(new DataGridViewTextBoxColumn { Name = "IdCuota", DataPropertyName = "IdCuota", Visible = false });
            dgvCuotas.Columns.Add(new DataGridViewTextBoxColumn { Name = "NumeroCuota", HeaderText = "Cuota", DataPropertyName = "NumeroCuota", ReadOnly = true });
            dgvCuotas.Columns.Add(new DataGridViewTextBoxColumn { Name = "MontoOriginal", HeaderText = "Monto original", DataPropertyName = "MontoOriginal", ReadOnly = true });
            dgvCuotas.Columns.Add(new DataGridViewTextBoxColumn { Name = "BalanceAdeudado", HeaderText = "Balance adeudado", DataPropertyName = "BalanceAdeudado", ReadOnly = true });
            dgvCuotas.Columns.Add(new DataGridViewTextBoxColumn { Name = "FechaVencimiento", HeaderText = "Vencimiento", DataPropertyName = "FechaVencimiento", ReadOnly = true });
            dgvCuotas.Columns.Add(new DataGridViewTextBoxColumn { Name = "Estado", HeaderText = "Estado", ReadOnly = true });
            dgvCuotas.Columns.Add(new DataGridViewTextBoxColumn { Name = "Periodo", HeaderText = "Período", ReadOnly = true });
            foreach (DataGridViewColumn columna in dgvCuotas.Columns)
                columna.SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvCuotas.Columns["MontoOriginal"].DefaultCellStyle.Format = "N2";
            dgvCuotas.Columns["BalanceAdeudado"].DefaultCellStyle.Format = "N2";
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

        private void ConfigurarFechas()
        {
            DateTime hoy = DateTime.Today;
            dtpFechaPago.MaxDate = hoy;
            dtpFechaAcreditacion.MaxDate = hoy;
            dtpFechaPago.Value = hoy;
            dtpFechaAcreditacion.MinDate = dtpFechaPago.Value.Date;
            dtpFechaAcreditacion.Value = hoy;
            dtpFechaPago.ValueChanged += dtpFechaPago_ValueChanged;
        }

        private void dtpFechaPago_ValueChanged(object sender, EventArgs e)
        {
            DateTime fechaPago = dtpFechaPago.Value.Date;
            dtpFechaAcreditacion.MinDate = fechaPago;
            if (dtpFechaAcreditacion.Value.Date < fechaPago) dtpFechaAcreditacion.Value = fechaPago;
        }

        private void BuscarCuotasPendientes()
        {
            _resultadoBusqueda = null;
            dgvCuotas.Rows.Clear();
            lblAlumno.Text = lblSolicitud.Text = _lblPlanDePago.Text = string.Empty;
            txtMonto.Clear();
            HabilitarFormularioPago(false);
            try
            {
                if (string.IsNullOrWhiteSpace(txtDni.Text))
                {
                    lblMensaje.Text = IdiomaUiHelper_83KI.Texto("Errores.PagoDniObligatorio");
                    HabilitarFormularioPago(false);
                    return;
                }
                if (cmbComision.SelectedValue == null) throw new InvalidOperationException("Errores.PagoComisionObligatoria");
                _resultadoBusqueda = _pagoBLL.BuscarCuotasPendientes(txtDni.Text, cmbComision.SelectedValue.ToString());
                if (_resultadoBusqueda == null || _resultadoBusqueda.Cuotas == null || _resultadoBusqueda.Cuotas.Count == 0)
                {
                    lblAlumno.Text = string.Empty; lblSolicitud.Text = string.Empty; _lblPlanDePago.Text = string.Empty; dgvCuotas.Rows.Clear();
                    HabilitarFormularioPago(false);
                    lblMensaje.Text = IdiomaUiHelper_83KI.Texto("FrmRegistrarPagoCuota.SinCuotas");
                    return;
                }
                lblAlumno.Text = IdiomaUiHelper_83KI.Texto("FrmRegistrarPagoInscripcion.AlumnoEncontrado", _resultadoBusqueda.Alumno.Nombre, _resultadoBusqueda.Alumno.Apellido, _resultadoBusqueda.Alumno.DNI);
                lblSolicitud.Text = IdiomaUiHelper_83KI.Texto("FrmRegistrarPagoInscripcion.SolicitudEncontrada", _resultadoBusqueda.Solicitud.Codigo, _resultadoBusqueda.CodigoComision, _resultadoBusqueda.Curso);
                ActualizarResumenFinanciero();
                dgvCuotas.Rows.Clear();
                foreach (var cuota in _resultadoBusqueda.Cuotas.OrderBy(c => c.NumeroCuota))
                {
                    int indice = dgvCuotas.Rows.Add(cuota.IdCuota, cuota.NumeroMensualidad, cuota.MontoOriginal, cuota.BalanceAdeudado, cuota.FechaVencimiento.ToShortDateString(), DescribirEstado(cuota), cuota.Periodo.HasValue ? cuota.Periodo.Value.ToString("MM/yyyy") : string.Empty);
                    dgvCuotas.Rows[indice].Tag = cuota;
                    if (cuota.BalanceAdeudado == 0)
                        dgvCuotas.Rows[indice].DefaultCellStyle.ForeColor = System.Drawing.Color.DimGray;
                }
                dgvCuotas.ClearSelection();
                var primera = _resultadoBusqueda.CuotasPendientes.OrderBy(c => c.NumeroCuota).FirstOrDefault();
                var fila = dgvCuotas.Rows.Cast<DataGridViewRow>().FirstOrDefault(r => primera != null && ((Cuota_83KI)r.Tag).IdCuota == primera.IdCuota);
                if (fila != null) { dgvCuotas.CurrentCell = fila.Cells["NumeroCuota"]; fila.Selected = true; }
                CargarCuotaSeleccionada();
            }
            catch (Exception ex) { HabilitarFormularioPago(false); MostrarError(ex); }
        }

        private void CargarCuotaSeleccionada()
        {
            HabilitarFormularioPago(false);
            txtMonto.Clear();
            lblMensaje.Text = string.Empty;
            if (_pagoPendienteDeGuardarFactura != null) { btnGuardar.Enabled = true; return; }
            var cuota = dgvCuotas.SelectedRows.Count == 0 ? null : dgvCuotas.SelectedRows[0].Tag as Cuota_83KI;
            if (cuota == null || _resultadoBusqueda == null) return;
            if (!_resultadoBusqueda.MatriculaPagada)
            {
                lblMensaje.Text = IdiomaUiHelper_83KI.Texto("Errores.PagoMatriculaPendiente");
                return;
            }
            if (cuota.Estado != Cuota_83KI.EstadoPendiente || cuota.BalanceAdeudado <= 0) return;
            if (_resultadoBusqueda.Cuotas.Any(c => c.NumeroCuota < cuota.NumeroCuota && c.BalanceAdeudado > 0))
            {
                lblMensaje.Text = IdiomaUiHelper_83KI.Texto("Errores.PagoCuotaAnteriorPendiente");
                return;
            }
            txtMonto.Text = cuota.BalanceAdeudado.ToString();
            HabilitarFormularioPago(true);
        }

        private static string DescribirEstado(Cuota_83KI cuota)
        {
            string estado = cuota.Estado == Cuota_83KI.EstadoPagada ? "Pagada"
                : cuota.Estado != Cuota_83KI.EstadoPendiente ? "NoDisponible"
                : cuota.FechaVencimiento.Date < DateTime.Today ? "Vencida" : "Pendiente";
            return IdiomaUiHelper_83KI.Texto("FrmRegistrarPagoCuota." + estado);
        }

        private void GuardarPago()
        {
            if (_pagoPendienteDeGuardarFactura != null) { MostrarConfirmacion(_pagoPendienteDeGuardarFactura); return; }
            try
            {
                if (dgvCuotas.SelectedRows.Count == 0) { lblMensaje.Text = IdiomaUiHelper_83KI.Texto("Errores.PagoCuotaObligatoria"); return; }
                int idCuota = Convert.ToInt32(dgvCuotas.SelectedRows[0].Cells["IdCuota"].Value);
                var resultado = _pagoBLL.RegistrarPagoCuota(new PagoInscripcionRequest_83KI
                {
                    DNI = txtDni.Text, CodigoComision = cmbComision.SelectedValue != null ? cmbComision.SelectedValue.ToString() : string.Empty,
                    IdCuota = idCuota, MetodoPago = cmbMetodoPago.SelectedItem != null ? cmbMetodoPago.SelectedItem.ToString() : string.Empty,
                    MontoRecibido = decimal.TryParse(txtMonto.Text, out decimal monto) ? monto : 0, NumeroReferencia = txtNumeroReferencia.Text,
                    FechaPago = dtpFechaPago.Value, FechaAcreditacion = dtpFechaAcreditacion.Value
                });
                MostrarConfirmacion(resultado);
            }
            catch (Exception ex) { MostrarError(ex); }
        }

        public void HabilitarFormularioPago(bool habilitado) { grpPago.Enabled = habilitado; btnGuardar.Enabled = habilitado; }

        private void MostrarConfirmacion(PagoInscripcionResultado_83KI resultado)
        {
            string mensaje = IdiomaUiHelper_83KI.Texto("FrmRegistrarPagoInscripcion.Registrado", resultado.Alumno.Nombre, resultado.Alumno.DNI, resultado.CodigoSolicitud, resultado.Cuota.NumeroMensualidad, resultado.Pago.MontoPagado, resultado.Pago.MetodoPago, resultado.Pago.NumeroReferencia);
            _pagoPendienteDeGuardarFactura = resultado;
            grpBusqueda.Enabled = dgvCuotas.Enabled = btnLimpiar.Enabled = grpPago.Enabled = false;
            if (resultado.AuditoriaFallida) mensaje += Environment.NewLine + IdiomaUiHelper_83KI.Texto("FrmRegistrarPagoInscripcion.AuditoriaFallida");
            string errorFactura = null; bool facturaGuardada = false;
            using (var dialogo = new SaveFileDialog { Filter = "Archivo PDF (*.pdf)|*.pdf", DefaultExt = "pdf", AddExtension = true, FileName = resultado.NumeroReciboInstitucional + ".pdf", Title = "Guardar factura" })
            {
                if (dialogo.ShowDialog(this) == DialogResult.OK)
                    try { new FacturaPagoPdfExporter_83KI().Exportar(resultado, dialogo.FileName); facturaGuardada = true; }
                    catch (Exception ex) { errorFactura = ex.Message; }
            }
            if (errorFactura != null) mensaje += Environment.NewLine + "El pago quedó registrado, pero no se pudo guardar la factura: " + errorFactura + " Podés volver a intentar con Guardar.";
            else if (!facturaGuardada) mensaje += Environment.NewLine + "El pago quedó registrado, pero la factura no se guardó. Podés volver a intentar con Guardar.";
            MessageBox.Show(this, mensaje, IdiomaUiHelper_83KI.Texto("Comun.Informacion"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            if (facturaGuardada)
            {
                _pagoPendienteDeGuardarFactura = null;
                grpBusqueda.Enabled = dgvCuotas.Enabled = btnLimpiar.Enabled = true;
                txtNumeroReferencia.Clear();
                BuscarCuotasPendientes();
            }
        }

        private void MostrarError(Exception ex) { lblMensaje.Text = IdiomaUiHelper_83KI.TraducirExcepcion(ex); IdiomaUiHelper_83KI.MostrarError(this, ex, "Comun.Validacion", MessageBoxIcon.Warning); }

        private void LimpiarFormulario()
        {
            txtDni.Clear(); cmbComision.DataSource = null; txtMonto.Clear(); txtNumeroReferencia.Clear(); cmbMetodoPago.SelectedIndex = 0;
            dtpFechaPago.Value = DateTime.Today; dtpFechaAcreditacion.MinDate = dtpFechaPago.Value.Date; dtpFechaAcreditacion.Value = DateTime.Today;
            dgvCuotas.Rows.Clear(); lblAlumno.Text = string.Empty; lblSolicitud.Text = string.Empty; lblMensaje.Text = string.Empty;
            _resultadoBusqueda = null; HabilitarFormularioPago(false);
        }

        public void ActualizarIdioma(IIdioma idioma)
        {
            Text = "CUN07 - " + IdiomaUiHelper_83KI.Texto("FrmRegistrarPagoCuota.Titulo");
            foreach (string columna in new[] { "NumeroCuota", "MontoOriginal", "BalanceAdeudado", "FechaVencimiento", "Estado", "Periodo" })
            {
                string clave = columna == "NumeroCuota" ? "Numero" : columna == "MontoOriginal" ? "Monto" : columna == "BalanceAdeudado" ? "Saldo" : columna == "FechaVencimiento" ? "Vencimiento" : columna;
                dgvCuotas.Columns[columna].HeaderText = IdiomaUiHelper_83KI.Texto("FrmRegistrarPagoCuota." + clave);
            }
            foreach (DataGridViewRow fila in dgvCuotas.Rows)
                if (fila.Tag is Cuota_83KI cuota) fila.Cells["Estado"].Value = DescribirEstado(cuota);
            if (_resultadoBusqueda != null)
                ActualizarResumenFinanciero();
            grpBusqueda.Text = IdiomaUiHelper_83KI.Texto("FrmRegistrarPagoInscripcion.Busqueda"); lblDni.Text = IdiomaUiHelper_83KI.Texto("Comun.Dni");
            lblCodigoComision.Text = IdiomaUiHelper_83KI.Texto("FrmRegistrarPagoInscripcion.Comision"); btnBuscar.Text = IdiomaUiHelper_83KI.Texto("FrmRegistrarPagoInscripcion.Buscar");
            grpPago.Text = IdiomaUiHelper_83KI.Texto("FrmRegistrarPagoInscripcion.DatosPago"); lblMonto.Text = IdiomaUiHelper_83KI.Texto("FrmRegistrarPagoInscripcion.MontoRecibido");
            lblMetodoPago.Text = IdiomaUiHelper_83KI.Texto("FrmRegistrarPagoInscripcion.MetodoPago"); lblNumeroReferencia.Text = IdiomaUiHelper_83KI.Texto("FrmRegistrarPagoInscripcion.NumeroReferencia");
            lblFechaPago.Text = IdiomaUiHelper_83KI.Texto("FrmRegistrarPagoInscripcion.FechaPago"); lblFechaAcreditacion.Text = IdiomaUiHelper_83KI.Texto("FrmRegistrarPagoInscripcion.FechaAcreditacion");
            btnGuardar.Text = IdiomaUiHelper_83KI.Texto("Comun.Guardar"); btnLimpiar.Text = IdiomaUiHelper_83KI.Texto("Comun.Limpiar");
        }

        protected override void OnFormClosed(FormClosedEventArgs e) { _gestorIdioma.Desuscribir(this); base.OnFormClosed(e); }

        private void ActualizarResumenFinanciero()
        {
            var solicitud = _resultadoBusqueda.Solicitud;
            string estado = _resultadoBusqueda.Cuotas.Any(c => c.BalanceAdeudado > 0 && c.FechaVencimiento.Date < DateTime.Today) ? "ConDeuda" : "AlDia";
            _lblPlanDePago.Text = IdiomaUiHelper_83KI.Texto("FrmRegistrarPagoCuota.Plan", _resultadoBusqueda.NombrePlanDePago, solicitud.RecargoPlanSnapshot)
                + Environment.NewLine + IdiomaUiHelper_83KI.Texto("FrmRegistrarPagoCuota.Resumen", solicitud.CantidadCuotasSnapshot, solicitud.TotalCursoSnapshot,
                    IdiomaUiHelper_83KI.Texto("FrmRegistrarPagoCuota." + estado));
        }
    }
}

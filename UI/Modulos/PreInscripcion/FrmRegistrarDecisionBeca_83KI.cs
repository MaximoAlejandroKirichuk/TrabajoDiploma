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
    public partial class FrmRegistrarDecisionBeca_83KI : Form, IObservadorIdioma
    {
        private readonly IGestorBeca_83KI _gestorBecaBLL;
        private readonly IGestorIdioma_83KI _gestorIdioma;
        private BusquedaDecisionBecaResultado_83KI _alumnoInscripcion;

        public FrmRegistrarDecisionBeca_83KI(IGestorBeca_83KI gestorBecaBLL)
        {
            _gestorBecaBLL = gestorBecaBLL;
            _gestorIdioma = ServiceFactory_83KI.GetGestorIdioma();
            InitializeComponent();
            CargarComisiones();
            IngresarResolucionBeca();
            HabilitarFormularioResolucion(false);
            _gestorIdioma.Suscribir(this);
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            BuscarAlumnoInscripcion();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            GuardarDecision();
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
        }

        private void BuscarAlumnoInscripcion()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(CodigoComisionSeleccionada))
                {
                    lblMensaje.Text = IdiomaUiHelper_83KI.Texto("Errores.BecaCodigoComisionObligatorio");
                    HabilitarFormularioResolucion(false);
                    return;
                }

                _alumnoInscripcion = _gestorBecaBLL.BuscarAlumnoConInscripcion(txtDni.Text, CodigoComisionSeleccionada);
                if (_alumnoInscripcion == null)
                {
                    lblAlumno.Text = string.Empty;
                    lblSolicitud.Text = string.Empty;
                    HabilitarFormularioResolucion(false);
                    lblMensaje.Text = IdiomaUiHelper_83KI.Texto("FrmRegistrarDecisionBeca.SinInscripcionValida");
                    return;
                }

                lblAlumno.Text = IdiomaUiHelper_83KI.Texto("FrmRegistrarDecisionBeca.AlumnoEncontrado", _alumnoInscripcion.Alumno.Nombre, _alumnoInscripcion.Alumno.Apellido, _alumnoInscripcion.Alumno.DNI);
                lblSolicitud.Text = IdiomaUiHelper_83KI.Texto("FrmRegistrarDecisionBeca.SolicitudEncontrada", _alumnoInscripcion.Solicitud.Codigo, _alumnoInscripcion.CodigoComision, _alumnoInscripcion.Curso);
                HabilitarFormularioResolucion(true);
                lblMensaje.Text = string.Empty;
            }
            catch (Exception ex)
            {
                HabilitarFormularioResolucion(false);
                MostrarError(ex);
            }
        }

        private void IngresarResolucionBeca()
        {
            cmbEstadoBeca.Items.Clear();
            cmbEstadoBeca.Items.Add(Beca_83KI.EstadoAprobada);
            cmbEstadoBeca.Items.Add(Beca_83KI.EstadoDenegada);
            cmbEstadoBeca.SelectedIndex = 0;
            dtpFechaSolicitud.Value = DateTime.Today;
        }

        private void CargarComisiones()
        {
            try
            {
                var comisiones = _gestorBecaBLL.ListarComisionesElegibles().ToList();
                cmbComisiones.DataSource = comisiones;
                cmbComisiones.DisplayMember = "Descripcion";
                cmbComisiones.ValueMember = "Codigo";
                cmbComisiones.Enabled = comisiones.Count > 0;
                cmbComisiones.SelectedIndex = -1;
                if (comisiones.Count == 0) lblMensaje.Text = IdiomaUiHelper_83KI.Texto("Errores.ComisionElegibleNoDisponible");
            }
            catch (Exception ex)
            {
                cmbComisiones.DataSource = null;
                cmbComisiones.Enabled = false;
                MostrarError(ex);
            }
        }

        private void GuardarDecision()
        {
            try
            {
                var resultado = _gestorBecaBLL.RegistrarDecisionBeca(new DecisionBecaRequest_83KI
                {
                    DNI = txtDni.Text,
                    CodigoComision = CodigoComisionSeleccionada,
                    TipoBeneficio = txtTipoBeneficio.Text,
                    FechaSolicitud = dtpFechaSolicitud.Value,
                    EstadoBeca = cmbEstadoBeca.SelectedItem != null ? cmbEstadoBeca.SelectedItem.ToString() : string.Empty,
                    MotivoDecision = txtMotivoDecision.Text
                });

                MostrarConfirmacion(resultado);
            }
            catch (Exception ex) { MostrarError(ex); }
        }

        public void HabilitarFormularioResolucion(bool habilitado)
        {
            grpResolucion.Enabled = habilitado;
            btnGuardar.Enabled = habilitado;
        }

        private void MostrarConfirmacion(DecisionBecaResultado_83KI resultado)
        {
            string mensaje = IdiomaUiHelper_83KI.Texto(
                "FrmRegistrarDecisionBeca.Registrada",
                resultado.Alumno.Nombre,
                resultado.Alumno.DNI,
                resultado.CodigoSolicitud,
                resultado.CodigoComision,
                resultado.Beca.EstadoBeca,
                resultado.Beca.TipoBeneficio);

            if (resultado.AuditoriaFallida)
                mensaje += Environment.NewLine + IdiomaUiHelper_83KI.Texto("FrmRegistrarDecisionBeca.AuditoriaFallida");

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
            cmbComisiones.SelectedIndex = -1;
            txtTipoBeneficio.Clear();
            txtMotivoDecision.Clear();
            lblAlumno.Text = string.Empty;
            lblSolicitud.Text = string.Empty;
            lblMensaje.Text = string.Empty;
            _alumnoInscripcion = null;
            IngresarResolucionBeca();
            HabilitarFormularioResolucion(false);
        }

        public void ActualizarIdioma(IIdioma idioma)
        {
            Text = IdiomaUiHelper_83KI.Texto("FrmRegistrarDecisionBeca.Titulo");
            grpBusqueda.Text = IdiomaUiHelper_83KI.Texto("FrmRegistrarDecisionBeca.Busqueda");
            lblDni.Text = IdiomaUiHelper_83KI.Texto("Comun.Dni");
            lblCodigoComision.Text = IdiomaUiHelper_83KI.Texto("FrmRegistrarDecisionBeca.CodigoComision");
            btnBuscar.Text = IdiomaUiHelper_83KI.Texto("FrmRegistrarDecisionBeca.Buscar");
            grpResolucion.Text = IdiomaUiHelper_83KI.Texto("FrmRegistrarDecisionBeca.Resolucion");
            lblTipoBeneficio.Text = IdiomaUiHelper_83KI.Texto("FrmRegistrarDecisionBeca.TipoBeneficio");
            lblFechaSolicitud.Text = IdiomaUiHelper_83KI.Texto("FrmRegistrarDecisionBeca.FechaSolicitud");
            lblEstadoBeca.Text = IdiomaUiHelper_83KI.Texto("FrmRegistrarDecisionBeca.EstadoBeca");
            lblMotivoDecision.Text = IdiomaUiHelper_83KI.Texto("FrmRegistrarDecisionBeca.MotivoDecision");
            btnGuardar.Text = IdiomaUiHelper_83KI.Texto("FrmRegistrarDecisionBeca.Guardar");
            btnLimpiar.Text = IdiomaUiHelper_83KI.Texto("FrmRegistrarConsultaLead.LimpiarNuevaBusqueda");
        }

        private string CodigoComisionSeleccionada { get { return cmbComisiones.SelectedValue as string ?? string.Empty; } }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _gestorIdioma.Desuscribir(this);
            base.OnFormClosed(e);
        }
    }
}

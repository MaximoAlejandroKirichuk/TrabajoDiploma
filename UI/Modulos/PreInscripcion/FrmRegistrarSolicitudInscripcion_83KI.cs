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
    public partial class FrmRegistrarSolicitudInscripcion_83KI : Form, IObservadorIdioma
    {
        private readonly IInscripcionBLL_83KI _inscripcionBLL;
        private readonly IGestorIdioma_83KI _gestorIdioma;
        private BusquedaInscripcionPersonaResultado_83KI _personaSeleccionada;

        public FrmRegistrarSolicitudInscripcion_83KI(IInscripcionBLL_83KI inscripcionBLL)
        {
            _inscripcionBLL = inscripcionBLL;
            _gestorIdioma = ServiceFactory_83KI.GetGestorIdioma();
            InitializeComponent();
            CargarComisiones();
            _gestorIdioma.Suscribir(this);
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            try
            {
                var resultados = _inscripcionBLL.BuscarPersonas(txtBusqueda.Text).ToList();
                lstResultados.DataSource = resultados;
                lstResultados.DisplayMember = "Display";
                LockIdentityFields(false);
                if (resultados.Count == 0)
                {
                    ReiniciarSeleccion();
                    if (EsDni(txtBusqueda.Text)) txtDni.Text = txtBusqueda.Text.Trim();
                    lblMensaje.Text = IdiomaUiHelper_83KI.Texto("FrmRegistrarSolicitudInscripcion.SinCoincidencias");
                }
                else
                {
                    lblMensaje.Text = string.Empty;
                }
            }
            catch (Exception ex) { MostrarError(ex); }
        }

        private void lstResultados_SelectedIndexChanged(object sender, EventArgs e)
        {
            _personaSeleccionada = lstResultados.SelectedItem as BusquedaInscripcionPersonaResultado_83KI;
            if (_personaSeleccionada == null)
            {
                ReiniciarSeleccion();
                return;
            }
            txtDni.Text = _personaSeleccionada.DNI;
            txtNombre.Text = _personaSeleccionada.Nombre;
            txtApellido.Text = _personaSeleccionada.Apellido;
            txtEmail.Text = _personaSeleccionada.Email;
            txtTelefono.Text = _personaSeleccionada.Telefono;
            LockIdentityFields(true);
            ReiniciarComisionYPlan();
            if (_personaSeleccionada.IdComisionSugerida.HasValue) cmbComisiones.SelectedValue = _personaSeleccionada.IdComisionSugerida.Value;
        }

        private void cmbComisiones_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarPlanes();
            PrevisualizarCuota();
        }

        private void cmbPlanes_SelectedIndexChanged(object sender, EventArgs e)
        {
            PrevisualizarCuota();
        }

        private void btnRegistrar_Click(object sender, EventArgs e)
        {
            try
            {
                var resultado = _inscripcionBLL.RegistrarSolicitud(new SolicitudInscripcionRequest_83KI
                {
                    DNI = txtDni.Text,
                    Nombre = txtNombre.Text,
                    Apellido = txtApellido.Text,
                    Email = txtEmail.Text,
                    Telefono = txtTelefono.Text,
                    IdAlumno = _personaSeleccionada != null ? _personaSeleccionada.IdAlumno : null,
                    IdLead = _personaSeleccionada != null ? _personaSeleccionada.IdLead : null,
                    IdComision = IdComisionSeleccionada,
                    IdPlanDePago = IdPlanSeleccionado,
                    Observaciones = txtObservaciones.Text
                });
                MessageBox.Show(this, IdiomaUiHelper_83KI.Texto("FrmRegistrarSolicitudInscripcion.Registrada", resultado.CodigoSolicitud), IdiomaUiHelper_83KI.Texto("Comun.Informacion"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex) { MostrarError(ex); }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            txtBusqueda.Clear();
            txtObservaciones.Clear();
            lstResultados.DataSource = null;
            LockIdentityFields(false);
            ReiniciarSeleccion();
            lblMensaje.Text = string.Empty;
        }

        private void ReiniciarSeleccion()
        {
            _personaSeleccionada = null;
            txtDni.Clear();
            txtNombre.Clear();
            txtApellido.Clear();
            txtEmail.Clear();
            txtTelefono.Clear();
            ReiniciarComisionYPlan();
        }

        private void ReiniciarComisionYPlan()
        {
            cmbComisiones.SelectedIndex = -1;
            cmbPlanes.DataSource = null;
            cmbPlanes.Enabled = false;
            lblPreview.Text = string.Empty;
        }

        private static bool EsDni(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto)) return false;
            return texto.Trim().All(char.IsDigit);
        }

        private void CargarComisiones()
        {
            var comisiones = _inscripcionBLL.ListarComisionesElegibles().ToList();
            cmbComisiones.DataSource = comisiones;
            cmbComisiones.DisplayMember = "Descripcion";
            cmbComisiones.ValueMember = "IdComision";
            cmbComisiones.Enabled = comisiones.Count > 0;
            btnRegistrar.Enabled = comisiones.Count > 0;
            ReiniciarComisionYPlan();
            if (comisiones.Count == 0) lblMensaje.Text = IdiomaUiHelper_83KI.Texto("Errores.ComisionElegibleNoDisponible");
        }

        private void CargarPlanes()
        {
            if (IdComisionSeleccionada <= 0)
            {
                cmbPlanes.DataSource = null;
                cmbPlanes.Enabled = false;
                return;
            }
            var planes = _inscripcionBLL.ObtenerPlanesComision(IdComisionSeleccionada).ToList();
            cmbPlanes.DataSource = planes;
            cmbPlanes.DisplayMember = "NombreConRecargo";
            cmbPlanes.ValueMember = "IdPlanDePago";
            cmbPlanes.Enabled = planes.Count > 0;
        }

        private void PrevisualizarCuota()
        {
            try
            {
                if (IdComisionSeleccionada <= 0)
                {
                    lblPreview.Text = string.Empty;
                    return;
                }
                var preview = _inscripcionBLL.PrevisualizarCuotaInicial(IdComisionSeleccionada);
                lblPreview.Text = IdiomaUiHelper_83KI.Texto("FrmRegistrarSolicitudInscripcion.PreviewCuota", preview.NumeroCuota, preview.MontoOriginal, preview.FechaVencimiento.ToShortDateString(), preview.Estado);
            }
            catch { lblPreview.Text = string.Empty; }
        }

        private void LockIdentityFields(bool locked)
        {
            txtDni.ReadOnly = locked;
            txtNombre.ReadOnly = locked;
            txtApellido.ReadOnly = locked;
            txtEmail.ReadOnly = locked;
            txtTelefono.ReadOnly = locked;
        }

        private int IdComisionSeleccionada { get { return cmbComisiones.SelectedValue is int ? (int)cmbComisiones.SelectedValue : 0; } }
        private int IdPlanSeleccionado { get { return cmbPlanes.SelectedValue is int ? (int)cmbPlanes.SelectedValue : 0; } }

        private void MostrarError(Exception ex)
        {
            lblMensaje.Text = IdiomaUiHelper_83KI.TraducirExcepcion(ex);
            IdiomaUiHelper_83KI.MostrarError(this, ex, "Comun.Validacion", MessageBoxIcon.Warning);
        }

        public void ActualizarIdioma(IIdioma idioma)
        {
            Text = IdiomaUiHelper_83KI.Texto("FrmRegistrarSolicitudInscripcion.Titulo");
            lblBusqueda.Text = IdiomaUiHelper_83KI.Texto("FrmRegistrarSolicitudInscripcion.Busqueda");
            lblDni.Text = IdiomaUiHelper_83KI.Texto("Comun.Dni");
            lblNombre.Text = IdiomaUiHelper_83KI.Texto("Comun.Nombre");
            lblApellido.Text = IdiomaUiHelper_83KI.Texto("Comun.Apellido");
            lblEmail.Text = IdiomaUiHelper_83KI.Texto("Comun.Email");
            lblTelefono.Text = IdiomaUiHelper_83KI.Texto("FrmRegistrarConsultaLead.Telefono");
            lblComision.Text = IdiomaUiHelper_83KI.Texto("FrmRegistrarSolicitudInscripcion.Comision");
            lblPlan.Text = IdiomaUiHelper_83KI.Texto("FrmRegistrarSolicitudInscripcion.Plan");
            lblObservaciones.Text = IdiomaUiHelper_83KI.Texto("FrmRegistrarConsultaLead.Observaciones");
            btnBuscar.Text = IdiomaUiHelper_83KI.Texto("FrmRegistrarConsultaLead.Buscar");
            btnLimpiar.Text = IdiomaUiHelper_83KI.Texto("FrmRegistrarConsultaLead.LimpiarNuevaBusqueda");
            btnRegistrar.Text = IdiomaUiHelper_83KI.Texto("FrmRegistrarSolicitudInscripcion.Registrar");
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _gestorIdioma.Desuscribir(this);
            base.OnFormClosed(e);
        }
    }
}

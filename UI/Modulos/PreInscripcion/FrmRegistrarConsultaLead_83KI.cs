using BE.Entidades;
using Service;
using Service.DTOs;
using Service.Entidades;
using Service.Interfaces;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace UI.Modulos.PreInscripcion
{
    public partial class FrmRegistrarConsultaLead_83KI : Form, IObservadorIdioma
    {
        private readonly ILeadBLL_83KI _leadBLL;
        private readonly IComisionConsultaBLL_83KI _comisionBLL;
        private readonly IGestorIdioma_83KI _gestorIdioma;

        private sealed class ComisionComboItem_83KI
        {
            public int IdComision { get; set; }
            public string Display { get; set; }
            public ComisionListado_83KI Comision { get; set; }
        }

        public FrmRegistrarConsultaLead_83KI(ILeadBLL_83KI leadBLL, IComisionConsultaBLL_83KI comisionBLL)
        {
            _leadBLL = leadBLL;
            _comisionBLL = comisionBLL;
            _gestorIdioma = ServiceFactory_83KI.GetGestorIdioma();
            InitializeComponent();
            CargarComisiones();
            _gestorIdioma.Suscribir(this);
        }

        private void CargarComisiones()
        {
            try
            {
                var comisiones = _comisionBLL.ListarComisionesPreapertura().ToList();
                var items = comisiones.Select(c => new ComisionComboItem_83KI
                {
                    IdComision = c.IdComision,
                    Comision = c,
                    Display = $"{c.Curso} - {IdiomaUiHelper_83KI.Texto("Dominio.DiaSemana." + c.DiaSemana)} {c.HoraInicio:hh\\:mm}-{c.HoraFin:hh\\:mm} - {c.Profesor}"
                }).ToList();
                cmbComisiones.DataSource = items;
                cmbComisiones.DisplayMember = "Display";
                cmbComisiones.ValueMember = "IdComision";
                bool hayComisionesDisponibles = items.Count > 0;
                cmbComisiones.Enabled = hayComisionesDisponibles;
                btnRegistrar.Enabled = hayComisionesDisponibles;
                if (!hayComisionesDisponibles) lblMensaje.Text = IdiomaUiHelper_83KI.Texto("Errores.ComisionPreaperturaNoDisponible");
            }
            catch (Exception ex)
            {
                cmbComisiones.DataSource = null;
                cmbComisiones.Enabled = false;
                btnRegistrar.Enabled = false;
                MostrarError(ex);
            }
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            try
            {
                var resultado = _leadBLL.BuscarLead(txtDni.Text, txtEmail.Text, txtTelefono.Text);
                if (resultado.EsAmbigua)
                {
                    LockIdentityFields(false);
                    lblMensaje.Text = IdiomaUiHelper_83KI.Texto("Errores.LeadCoincidenciaAmbigua");
                    return;
                }
                if (resultado.TieneUnico) Prefill(resultado.Lead);
                else
                {
                    LockIdentityFields(false);
                    lblMensaje.Text = IdiomaUiHelper_83KI.Texto("FrmRegistrarConsultaLead.SinCoincidencias");
                }
            }
            catch (Exception ex) { MostrarError(ex); }
        }

        private void Prefill(Lead_83KI lead)
        {
            txtDni.Text = lead.DNI;
            txtNombre.Text = lead.Nombre;
            txtApellido.Text = lead.Apellido;
            txtEmail.Text = lead.Email;
            txtTelefono.Text = lead.Telefono;
            LockIdentityFields(true);
            lblMensaje.Text = IdiomaUiHelper_83KI.Texto("FrmRegistrarConsultaLead.LeadEncontrado");
        }

        private void LockIdentityFields(bool locked)
        {
            txtDni.ReadOnly = locked;
            txtNombre.ReadOnly = locked;
            txtApellido.ReadOnly = locked;
            txtEmail.ReadOnly = locked;
            txtTelefono.ReadOnly = locked;

            Color backColor = locked ? SystemColors.Control : SystemColors.Window;
            txtDni.BackColor = backColor;
            txtNombre.BackColor = backColor;
            txtApellido.BackColor = backColor;
            txtEmail.BackColor = backColor;
            txtTelefono.BackColor = backColor;
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            txtDni.Clear();
            txtNombre.Clear();
            txtApellido.Clear();
            txtEmail.Clear();
            txtTelefono.Clear();
            LockIdentityFields(false);
            lblMensaje.Text = string.Empty;
        }

        private void btnRegistrar_Click(object sender, EventArgs e)
        {
            try
            {
                if (IdComisionSeleccionada <= 0)
                {
                    lblMensaje.Text = IdiomaUiHelper_83KI.Texto("Errores.ComisionPreaperturaNoDisponible");
                    return;
                }

                var resultado = _leadBLL.RegistrarConsulta(new ConsultaLeadRequest_83KI
                {
                    DNI = txtDni.Text,
                    Nombre = txtNombre.Text,
                    Apellido = txtApellido.Text,
                    Email = txtEmail.Text,
                    Telefono = txtTelefono.Text,
                    IdComision = IdComisionSeleccionada,
                    MedioContacto = txtMedioContacto.Text,
                    Motivo = txtMotivo.Text,
                    Observaciones = txtObservaciones.Text
                });
                MessageBox.Show(this, IdiomaUiHelper_83KI.Texto("FrmRegistrarConsultaLead.Registrada", resultado.Consulta.Codigo), IdiomaUiHelper_83KI.Texto("Comun.Informacion"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex) { MostrarError(ex); }
        }

        private int IdComisionSeleccionada { get { return cmbComisiones.SelectedValue is int ? (int)cmbComisiones.SelectedValue : 0; } }

        private void MostrarError(Exception ex)
        {
            lblMensaje.Text = IdiomaUiHelper_83KI.TraducirExcepcion(ex);
            IdiomaUiHelper_83KI.MostrarError(this, ex, "Comun.Validacion", MessageBoxIcon.Warning);
        }

        public void ActualizarIdioma(IIdioma idioma)
        {
            Text = IdiomaUiHelper_83KI.Texto("FrmRegistrarConsultaLead.Titulo");
            lblDni.Text = IdiomaUiHelper_83KI.Texto("Comun.Dni");
            lblNombre.Text = IdiomaUiHelper_83KI.Texto("Comun.Nombre");
            lblApellido.Text = IdiomaUiHelper_83KI.Texto("Comun.Apellido");
            lblEmail.Text = IdiomaUiHelper_83KI.Texto("Comun.Email");
            lblTelefono.Text = IdiomaUiHelper_83KI.Texto("FrmRegistrarConsultaLead.Telefono");
            lblComision.Text = IdiomaUiHelper_83KI.Texto("FrmRegistrarConsultaLead.Comision");
            lblMedioContacto.Text = IdiomaUiHelper_83KI.Texto("FrmRegistrarConsultaLead.MedioContacto");
            lblMotivo.Text = IdiomaUiHelper_83KI.Texto("FrmRegistrarConsultaLead.Motivo");
            lblObservaciones.Text = IdiomaUiHelper_83KI.Texto("FrmRegistrarConsultaLead.Observaciones");
            btnBuscar.Text = IdiomaUiHelper_83KI.Texto("FrmRegistrarConsultaLead.Buscar");
            btnLimpiar.Text = IdiomaUiHelper_83KI.Texto("FrmRegistrarConsultaLead.LimpiarNuevaBusqueda");
            btnRegistrar.Text = IdiomaUiHelper_83KI.Texto("FrmRegistrarConsultaLead.Registrar");
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _gestorIdioma.Desuscribir(this);
            base.OnFormClosed(e);
        }
    }
}

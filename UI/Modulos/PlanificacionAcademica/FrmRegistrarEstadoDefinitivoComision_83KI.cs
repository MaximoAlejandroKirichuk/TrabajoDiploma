using Service;
using Service.DTOs;
using Service.Entidades;
using Service.Interfaces;
using System;
using System.Linq;
using System.Windows.Forms;

namespace UI.Modulos.PlanificacionAcademica
{
    public partial class FrmRegistrarEstadoDefinitivoComision_83KI : Form, IObservadorIdioma
    {
        private readonly IEstadoDefinitivoComisionBLL_83KI _bll;
        private readonly IGestorIdioma_83KI _gestorIdioma;

        public FrmRegistrarEstadoDefinitivoComision_83KI(IEstadoDefinitivoComisionBLL_83KI bll)
        {
            _bll = bll;
            _gestorIdioma = ServiceFactory_83KI.GetGestorIdioma();
            InitializeComponent();
            _gestorIdioma.Suscribir(this);
            CargarComisiones();
        }

        private void CargarComisiones()
        {
            try
            {
                dgvComisiones.Rows.Clear();
                foreach (var item in _bll.ListarComisionesVencidas())
                {
                    dgvComisiones.Rows.Add(item.IdComision, item.Codigo, item.Curso, item.Profesor, item.CupoMinimo, item.VacantesRegularizadas, item.CumpleQuorum ? IdiomaUiHelper_83KI.Texto("FrmRegistrarEstadoDefinitivoComision.CumpleQuorum") : IdiomaUiHelper_83KI.Texto("FrmRegistrarEstadoDefinitivoComision.NoCumpleQuorum"));
                }
                lblMensaje.Text = dgvComisiones.Rows.Count == 0 ? IdiomaUiHelper_83KI.Texto("FrmRegistrarEstadoDefinitivoComision.SinComisiones") : string.Empty;
                EvaluarSeleccion();
            }
            catch (Exception ex) { ActualizarAcciones(null); MostrarError(ex); }
        }

        private void dgvComisiones_SelectionChanged(object sender, EventArgs e)
        {
            EvaluarSeleccion();
        }

        private void EvaluarSeleccion()
        {
            try
            {
                if (IdComisionSeleccionada <= 0) { lblEvaluacion.Text = string.Empty; ActualizarAcciones(null); return; }
                var evaluacion = _bll.Evaluar(IdComisionSeleccionada);
                if (evaluacion == null) { ActualizarAcciones(null); lblMensaje.Text = IdiomaUiHelper_83KI.Texto("Errores.ComisionNoEncontrada"); return; }
                lblEvaluacion.Text = IdiomaUiHelper_83KI.Texto("FrmRegistrarEstadoDefinitivoComision.Evaluacion", evaluacion.VacantesRegularizadas, evaluacion.CupoMinimo, evaluacion.CumpleQuorum ? IdiomaUiHelper_83KI.Texto("FrmRegistrarEstadoDefinitivoComision.CumpleQuorum") : IdiomaUiHelper_83KI.Texto("FrmRegistrarEstadoDefinitivoComision.NoCumpleQuorum"));
                ActualizarAcciones(evaluacion);
            }
            catch (Exception ex) { ActualizarAcciones(null); MostrarError(ex); }
        }

        private void btnActualizar_Click(object sender, EventArgs e) { CargarComisiones(); }

        private void btnRegistrarAlta_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Confirmar("FrmRegistrarEstadoDefinitivoComision.ConfirmarAlta")) return;
                ActualizarAcciones(null);
                var resultado = _bll.RegistrarAltaOficial(new AltaOficialComisionRequest_83KI { IdComision = IdComisionSeleccionada, FechaAlta = dtpFechaActa.Value.Date, NumeroActa = txtNumeroActa.Text, Observaciones = txtObservaciones.Text });
                MostrarResultado(resultado, "FrmRegistrarEstadoDefinitivoComision.AltaRegistrada");
                CargarComisiones();
            }
            catch (Exception ex) { MostrarError(ex); EvaluarSeleccion(); }
        }

        private void btnRegistrarCierre_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Confirmar("FrmRegistrarEstadoDefinitivoComision.ConfirmarCierre")) return;
                ActualizarAcciones(null);
                var resultado = _bll.RegistrarActaCierre(new ActaCierreComisionRequest_83KI { IdComision = IdComisionSeleccionada, FechaCierre = dtpFechaActa.Value.Date, NumeroActa = txtNumeroActa.Text, Motivo = txtMotivo.Text, Observaciones = txtObservaciones.Text });
                MostrarResultado(resultado, "FrmRegistrarEstadoDefinitivoComision.CierreRegistrado");
                CargarComisiones();
            }
            catch (Exception ex) { MostrarError(ex); EvaluarSeleccion(); }
        }

        private bool Confirmar(string clave)
        {
            return MessageBox.Show(this, IdiomaUiHelper_83KI.Texto(clave), IdiomaUiHelper_83KI.Texto("Comun.Confirmacion"), MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK;
        }

        private void MostrarResultado(ResultadoEstadoDefinitivoComision_83KI resultado, string clave)
        {
            string mensaje = IdiomaUiHelper_83KI.Texto(clave, resultado.CodigoComision, resultado.EstadoDefinitivo, resultado.NumeroActa);
            if (resultado.AuditoriaFallida) mensaje += Environment.NewLine + IdiomaUiHelper_83KI.Texto("FrmRegistrarEstadoDefinitivoComision.AuditoriaFallida");
            MessageBox.Show(this, mensaje, IdiomaUiHelper_83KI.Texto("Comun.Informacion"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            lblMensaje.Text = mensaje;
        }

        private void ActualizarAcciones(EvaluacionQuorumComision_83KI evaluacion)
        {
            btnRegistrarAlta.Enabled = evaluacion != null && evaluacion.CumpleQuorum;
            btnRegistrarCierre.Enabled = evaluacion != null && !evaluacion.CumpleQuorum;
            txtMotivo.Enabled = evaluacion != null && !evaluacion.CumpleQuorum;
        }

        private int IdComisionSeleccionada
        {
            get
            {
                if (dgvComisiones.SelectedRows.Count == 0) return 0;
                return Convert.ToInt32(dgvComisiones.SelectedRows[0].Cells["IdComision"].Value);
            }
        }

        private void MostrarError(Exception ex)
        {
            lblMensaje.Text = IdiomaUiHelper_83KI.TraducirExcepcion(ex);
            IdiomaUiHelper_83KI.MostrarError(this, ex, "Comun.Validacion", MessageBoxIcon.Warning);
        }

        public void ActualizarIdioma(IIdioma idioma)
        {
            Text = IdiomaUiHelper_83KI.Texto("FrmRegistrarEstadoDefinitivoComision.Titulo");
            grpActa.Text = IdiomaUiHelper_83KI.Texto("FrmRegistrarEstadoDefinitivoComision.DatosActa");
            lblNumeroActa.Text = IdiomaUiHelper_83KI.Texto("FrmRegistrarEstadoDefinitivoComision.NumeroActa");
            lblFechaActa.Text = IdiomaUiHelper_83KI.Texto("FrmRegistrarEstadoDefinitivoComision.FechaActa");
            lblMotivo.Text = IdiomaUiHelper_83KI.Texto("FrmRegistrarEstadoDefinitivoComision.Motivo");
            lblObservaciones.Text = IdiomaUiHelper_83KI.Texto("FrmRegistrarEstadoDefinitivoComision.Observaciones");
            btnActualizar.Text = IdiomaUiHelper_83KI.Texto("Comun.Actualizar");
            btnRegistrarAlta.Text = IdiomaUiHelper_83KI.Texto("FrmRegistrarEstadoDefinitivoComision.RegistrarAlta");
            btnRegistrarCierre.Text = IdiomaUiHelper_83KI.Texto("FrmRegistrarEstadoDefinitivoComision.RegistrarCierre");
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _gestorIdioma.Desuscribir(this);
            base.OnFormClosed(e);
        }
    }
}

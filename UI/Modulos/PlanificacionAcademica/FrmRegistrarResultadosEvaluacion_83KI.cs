using BLL;
using Service;
using Service.DTOs;
using BE.DTOs.Comisiones;
using BE.DTOs.Evaluaciones;
using Service.Entidades;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace UI.Modulos.PlanificacionAcademica
{
    public partial class FrmRegistrarResultadosEvaluacion_83KI : Form, IObservadorIdioma
    {
        private readonly KI68ResultadoEvaluacionBLL _bll;
        private readonly Service.Interfaces.IGestorIdioma_83KI _gestorIdioma;
        private string CodigoCurso;
        private string NumeroComision;
        private DateTime FechaEvaluacion;
        private List<AlumnoEvaluacionListado_83KI> AlumnosHabilitados = new List<AlumnoEvaluacionListado_83KI>();
        private readonly List<ResultadoAlumnoRequest_83KI> ResultadosProvisorios = new List<ResultadoAlumnoRequest_83KI>();
        private int IdAlumnoSeleccionado;
        private bool PuedeRegistrarDefinitivamente;
        private bool _auditoriaFallida;
        private bool _cargandoSelectores;
        private List<ComisionListado_83KI> _comisionesAsignadas = new List<ComisionListado_83KI>();
        private ComisionListado_83KI CursoSeleccionado { get { return cmbCurso.SelectedItem as ComisionListado_83KI; } }
        private ComisionListado_83KI ComisionSeleccionada { get { return cmbComision.SelectedItem as ComisionListado_83KI; } }
        private bool SeleccionValida { get { return CursoSeleccionado != null && ComisionSeleccionada != null && ComisionSeleccionada.IdCurso == CursoSeleccionado.IdCurso; } }

        public FrmRegistrarResultadosEvaluacion_83KI(KI68ResultadoEvaluacionBLL bll)
            : this(bll, ServiceFactory_83KI.GetGestorIdioma()) { }

        public FrmRegistrarResultadosEvaluacion_83KI(KI68ResultadoEvaluacionBLL bll, IGestorIdioma_83KI gestorIdioma)
        {
            _bll = bll ?? throw new ArgumentNullException(nameof(bll));
            _gestorIdioma = gestorIdioma ?? throw new ArgumentNullException(nameof(gestorIdioma));
            InitializeComponent();
            _gestorIdioma.Suscribir(this);
            ActualizarEstadoBotonRegistrar();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            try { CargarComisionesAsignadas(); }
            catch (Exception ex) { MostrarExcepcion(ex); }
        }

        public void CargarComisionesAsignadas()
        {
            dtpFecha.Enabled = false;
            var hoy = _bll.ObtenerFechaActual().Date;
            if (dtpFecha.Value > hoy) dtpFecha.Value = hoy;
            dtpFecha.MaxDate = hoy;
            dtpFecha.Enabled = true;
            var comisiones = _bll.ListarComisionesAsignadasParaEvaluacion();
            int? curso = CursoSeleccionado?.IdCurso, comision = ComisionSeleccionada?.IdComision;
            _cargandoSelectores = true;
            try
            {
                _comisionesAsignadas = comisiones;
                cmbCurso.DataSource = comisiones.GroupBy(c => c.IdCurso).Select(g => g.First()).ToList();
                cmbCurso.SelectedItem = ((List<ComisionListado_83KI>)cmbCurso.DataSource).FirstOrDefault(c => c.IdCurso == curso);
                FiltrarComisiones(comision);
                cmbCurso.Enabled = comisiones.Count > 0;
            }
            finally { _cargandoSelectores = false; }
            // Las recargas y los cambios de selección nunca descartan resultados provisorios.
            lblMensaje.Text = Texto(comisiones.Count == 0 ? "Cun08.SinAsignaciones" : "Cun08.SeleccionarComision");
            ActualizarEstadoBotonRegistrar();
        }

        private void FiltrarComisiones(int? idComision)
        {
            var comisiones = _comisionesAsignadas.Where(c => c.IdCurso == CursoSeleccionado?.IdCurso).ToList();
            cmbComision.DataSource = comisiones;
            cmbComision.SelectedItem = comisiones.FirstOrDefault(c => c.IdComision == idComision);
            cmbComision.Enabled = comisiones.Count > 0;
        }

        private void CursoChanged(object sender, EventArgs e)
        {
            if (_cargandoSelectores) return;
            _cargandoSelectores = true;
            try { FiltrarComisiones(null); }
            finally { _cargandoSelectores = false; }
            ActualizarEstadoBotonRegistrar();
        }

        private void ComisionFormat(object sender, ListControlConvertEventArgs e)
        {
            var comision = e.ListItem as ComisionListado_83KI;
            if (comision != null) e.Value = string.Format(CultureInfo.CurrentCulture, "{0} ({1:d} – {2:d})", comision.Codigo, comision.FechaInicio, comision.FechaFin);
        }

        public void BuscarAlumnosHabilitados()
        {
            try
            {
                if (!SeleccionValida) throw new InvalidOperationException("Cun08.SeleccionarComision");
                if (ResultadosProvisorios.Count > 0 && !Confirmar("Cun08.Descartar")) return;
                var request = new BusquedaResultadoEvaluacionRequest_83KI { CodigoCurso = CursoSeleccionado.IdCurso.ToString(CultureInfo.InvariantCulture), NumeroComision = ComisionSeleccionada.Codigo, FechaEvaluacion = dtpFecha.Value.Date };
                var alumnos = _bll.BuscarAlumnosHabilitados(request);
                CodigoCurso = request.CodigoCurso; NumeroComision = request.NumeroComision; FechaEvaluacion = request.FechaEvaluacion;
                AlumnosHabilitados = alumnos;
                ResultadosProvisorios.Clear();
                RefrescarGrilla();
                if (alumnos.Count == 0) MostrarError("Cun08.SinAlumnos");
                else lblMensaje.Text = Texto("Cun08.Provisorio");
            }
            catch (Exception ex) { MostrarExcepcion(ex); }
            ActualizarEstadoBotonRegistrar();
        }

        public void SeleccionarAlumno(int idInscripcion)
        {
            IdAlumnoSeleccionado = AlumnosHabilitados.Any(a => a.IdInscripcion == idInscripcion) ? idInscripcion : 0;
            var r = ResultadosProvisorios.FirstOrDefault(x => x.IdInscripcion == IdAlumnoSeleccionado);
            txtNota.Text = r == null ? string.Empty : r.Calificacion.ToString(CultureInfo.CurrentCulture);
            txtObservaciones.Text = r == null ? string.Empty : r.Observaciones;
            txtNota.Enabled = txtObservaciones.Enabled = btnAgregar.Enabled = IdAlumnoSeleccionado > 0;
            btnQuitar.Enabled = r != null;
        }

        public void AgregarResultadoProvisorio(ResultadoAlumnoRequest_83KI resultado)
        {
            if (resultado == null || !AlumnosHabilitados.Any(a => a.IdInscripcion == resultado.IdInscripcion)) throw new InvalidOperationException("Cun08.AlumnosInvalidos");
            if (resultado.Calificacion < 1m || resultado.Calificacion > 10m || decimal.Round(resultado.Calificacion, 2) != resultado.Calificacion)
                throw new InvalidOperationException("Cun08.CalificacionInvalida");
            if ((resultado.Observaciones ?? "").Length > 500) throw new InvalidOperationException("Cun08.ObservacionesLargas");
            var anterior = ResultadosProvisorios.FirstOrDefault(r => r.IdInscripcion == resultado.IdInscripcion);
            if (anterior != null) ResultadosProvisorios.Remove(anterior);
            ResultadosProvisorios.Add(new ResultadoAlumnoRequest_83KI { IdInscripcion = resultado.IdInscripcion, Calificacion = resultado.Calificacion, Observaciones = resultado.Observaciones ?? "" });
            RefrescarGrilla(); ActualizarEstadoBotonRegistrar();
        }

        private bool CriteriosVigentes { get { return SeleccionValida && CodigoCurso == CursoSeleccionado.IdCurso.ToString(CultureInfo.InvariantCulture) && NumeroComision == ComisionSeleccionada.Codigo && AlumnosHabilitados.Any(a => a.IdComision == ComisionSeleccionada.IdComision) && FechaEvaluacion == dtpFecha.Value.Date; } }
        public void ActualizarEstadoBotonRegistrar()
        {
            btnBuscar.Enabled = !_cargandoSelectores && SeleccionValida;
            PuedeRegistrarDefinitivamente = ResultadosProvisorios.Count > 0 && CriteriosVigentes;
            btnRegistrar.Enabled = PuedeRegistrarDefinitivamente;
        }

        public void RegistrarResultadosDefinitivamente()
        {
            try
            {
                ActualizarEstadoBotonRegistrar();
                if (!PuedeRegistrarDefinitivamente) throw new InvalidOperationException("Cun08.BuscarNuevamente");
                if (!Confirmar("Cun08.Confirmar")) return;
                var request = new ResultadoEvaluacionRequest_83KI { IdComision = AlumnosHabilitados[0].IdComision, FechaEvaluacion = FechaEvaluacion,
                    Resultados = ResultadosProvisorios.Select(r => new ResultadoAlumnoRequest_83KI { IdInscripcion = r.IdInscripcion, Calificacion = r.Calificacion, Observaciones = r.Observaciones }).ToList() };
                btnRegistrar.Enabled = false;
                _bll.RegistrarResultados(request);
                var guardados = new HashSet<int>(ResultadosProvisorios.Select(r => r.IdInscripcion));
                AlumnosHabilitados.RemoveAll(a => guardados.Contains(a.IdInscripcion));
                ResultadosProvisorios.Clear(); // Limpiar también cuando la confirmación incluya una advertencia de auditoría.
                _auditoriaFallida = request.AuditoriaFallida;
                RefrescarGrilla(); MostrarConfirmacion();
            }
            catch (Exception ex) { MostrarExcepcion(ex); }
            finally { ActualizarEstadoBotonRegistrar(); }
        }

        private void RefrescarGrilla()
        {
            dgvAlumnos.Rows.Clear();
            foreach (var a in AlumnosHabilitados)
            {
                var r = ResultadosProvisorios.FirstOrDefault(x => x.IdInscripcion == a.IdInscripcion);
                int row = dgvAlumnos.Rows.Add(a.DNI, a.Nombre, a.Apellido, r == null ? "" : r.Calificacion.ToString(CultureInfo.CurrentCulture), r == null ? "" : r.Observaciones);
                dgvAlumnos.Rows[row].Tag = a;
            }
            dgvAlumnos.ClearSelection(); SeleccionarAlumno(0);
        }

        public void MostrarError(string mensaje)
        {
            lblMensaje.Text = Texto(mensaje);
            MessageBox.Show(this, lblMensaje.Text, Texto("Comun.Validacion"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        private void MostrarExcepcion(Exception ex)
        {
            lblMensaje.Text = TraducirError(ex);
            MessageBox.Show(this, lblMensaje.Text, Texto("Comun.Validacion"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        private string TraducirError(Exception ex)
        {
            var sql = ex as System.Data.SqlClient.SqlException;
            if (sql != null && (sql.Number == 2601 || sql.Number == 2627)) return Texto("Cun08.AlumnosInvalidos");
            string clave = (ex.Message ?? "").Split('\r', '\n')[0].Trim();
            if (clave.StartsWith("Cun08.", StringComparison.Ordinal) || clave.StartsWith("Errores.", StringComparison.Ordinal)) return Texto(clave);
            return Texto("Cun08.NoRegistrado");
        }
        public void MostrarConfirmacion()
        {
            lblMensaje.Text = Texto("Cun08.Registrado") + (_auditoriaFallida ? Environment.NewLine + Texto("Cun08.AuditoriaFallida") : "");
            MessageBox.Show(this, lblMensaje.Text, Texto("Comun.Informacion"), MessageBoxButtons.OK, _auditoriaFallida ? MessageBoxIcon.Warning : MessageBoxIcon.Information);
        }
        private bool Confirmar(string clave) { return MessageBox.Show(this, Texto(clave), Texto("Comun.Confirmacion"), MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK; }
        private string Texto(string clave) { return _gestorIdioma.ObtenerTexto(clave); }
        private void Buscar_Click(object sender, EventArgs e) { BuscarAlumnosHabilitados(); }
        private void Registrar_Click(object sender, EventArgs e) { RegistrarResultadosDefinitivamente(); }
        private void Nota_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar) || char.IsDigit(e.KeyChar)) return;
            string separador = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            if (separador.Length == 1 && e.KeyChar == separador[0] && !txtNota.Text.Contains(separador)) return;
            e.Handled = true;
        }
        private void CriteriosChanged(object sender, EventArgs e) { if (!_cargandoSelectores) ActualizarEstadoBotonRegistrar(); }
        private void SeleccionChanged(object sender, EventArgs e)
        {
            var a = dgvAlumnos.SelectedRows.Count == 0 ? null : dgvAlumnos.SelectedRows[0].Tag as AlumnoEvaluacionListado_83KI;
            SeleccionarAlumno(a == null ? 0 : a.IdInscripcion);
        }
        private void Agregar_Click(object sender, EventArgs e)
        {
            try
            {
                decimal nota;
                if (!decimal.TryParse(txtNota.Text, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.CurrentCulture, out nota)) throw new InvalidOperationException("Cun08.CalificacionInvalida");
                AgregarResultadoProvisorio(new ResultadoAlumnoRequest_83KI { IdInscripcion = IdAlumnoSeleccionado, Calificacion = nota, Observaciones = txtObservaciones.Text });
                lblMensaje.Text = Texto("Cun08.Provisorio");
            }
            catch (Exception ex) { MostrarExcepcion(ex); }
        }
        private void Quitar_Click(object sender, EventArgs e)
        {
            ResultadosProvisorios.RemoveAll(r => r.IdInscripcion == IdAlumnoSeleccionado);
            RefrescarGrilla(); ActualizarEstadoBotonRegistrar();
        }
        public void ActualizarIdioma(IIdioma idioma)
        {
            Text = Texto("Cun08.Titulo"); lblCurso.Text = Texto("Cun08.Curso"); lblComision.Text = Texto("Cun08.Comision");
            lblFecha.Text = Texto("Cun08.Fecha"); lblNota.Text = Texto("Cun08.Nota"); lblObservaciones.Text = Texto("Cun08.Observaciones");
            btnBuscar.Text = Texto("Cun08.Buscar"); btnAgregar.Text = Texto("Cun08.Agregar"); btnQuitar.Text = Texto("Comun.Quitar"); btnRegistrar.Text = Texto("Cun08.Registrar");
            string[] claves = { "Comun.Dni", "Comun.Nombre", "Comun.Apellido", "Cun08.NotaProvisoria", "Cun08.Observaciones" };
            for (int i = 0; i < claves.Length; i++) dgvAlumnos.Columns[i].HeaderText = Texto(claves[i]);
            lblMensaje.Text = Texto(_comisionesAsignadas.Count == 0 ? "Cun08.SinAsignaciones" : "Cun08.Provisorio");
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (ResultadosProvisorios.Count > 0 && !Confirmar("Cun08.Descartar")) e.Cancel = true;
            base.OnFormClosing(e);
        }
        protected override void OnFormClosed(FormClosedEventArgs e) { _gestorIdioma.Desuscribir(this); base.OnFormClosed(e); }
    }
}

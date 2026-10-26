using BLL;
using Service;
using BE.DTOs.Pagos;
using Service.DTOs;
using BE.DTOs.Comisiones;
using Service.Entidades;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace UI.Modulos.CobrosMorosidadActas
{
    // RF2 - Informe de morosidad por alumno y comisión.
    // El formulario solo muestra: los filtros se validan y el reporte se arma en KI68ReporteMorosidadBLL.
    public partial class FrmReporteMorosidad_83KI : Form, IObservadorIdioma
    {
        private readonly IReporteMorosidadBLL_83KI _bll;
        private readonly IGestorIdioma_83KI _gestorIdioma;

        private List<ComisionListado_83KI> _comisiones = new List<ComisionListado_83KI>();
        private List<ReporteMorosidadFila_83KI> _filas = new List<ReporteMorosidadFila_83KI>();
        private bool _hayBusqueda;
        private DateTime _fechaReporte;

        // colores de la torta, de menor a mayor gravedad
        private static readonly Color Color1a30 = Color.FromArgb(242, 193, 78);
        private static readonly Color Color31a60 = Color.FromArgb(230, 126, 34);
        private static readonly Color ColorMasDe60 = Color.FromArgb(192, 57, 43);

        public FrmReporteMorosidad_83KI(IReporteMorosidadBLL_83KI bll)
        {
            _bll = bll;
            _gestorIdioma = ServiceFactory_83KI.GetGestorIdioma();
            InitializeComponent();
            dgvMorosidad.CellClick += dgvMorosidad_CellClick;
            dgvMorosidad.KeyDown += dgvMorosidad_KeyDown;
            // los títulos de columna largos se parten en dos renglones en lugar de cortarse
            dgvMorosidad.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True;
            CargarFiltros();
            _gestorIdioma.Suscribir(this);
        }

        // ------------------------------------------------------------------
        // Carga inicial: solo los combos de filtros (la búsqueda es un paso aparte)
        // ------------------------------------------------------------------

        private void CargarFiltros()
        {
            try
            {
                _comisiones = _bll.ListarComisionesParaFiltro();
            }
            catch (Exception ex)
            {
                _comisiones = new List<ComisionListado_83KI>();
                IdiomaUiHelper_83KI.MostrarError(this, ex, "Comun.Validacion", MessageBoxIcon.Warning);
            }
        }

        private void RellenarCursos()
        {
            int? seleccionado = IdSeleccionado(cmbCurso);
            cmbCurso.Items.Clear();
            cmbCurso.Items.Add(new OpcionFiltro(null, IdiomaUiHelper_83KI.Texto("Comun.Todos")));
            foreach (var curso in _comisiones.GroupBy(c => c.IdCurso).Select(g => g.First()).OrderBy(c => c.Curso))
            {
                cmbCurso.Items.Add(new OpcionFiltro(curso.IdCurso, curso.Curso));
            }
            Seleccionar(cmbCurso, seleccionado);
        }

        private void RellenarComisiones()
        {
            int? seleccionado = IdSeleccionado(cmbComision);
            int? idCurso = IdSeleccionado(cmbCurso);
            cmbComision.Items.Clear();
            cmbComision.Items.Add(new OpcionFiltro(null, IdiomaUiHelper_83KI.Texto("Comun.Todos")));
            foreach (var comision in _comisiones.Where(c => !idCurso.HasValue || c.IdCurso == idCurso.Value).OrderBy(c => c.Codigo))
            {
                cmbComision.Items.Add(new OpcionFiltro(comision.IdComision, comision.Codigo + " (" + comision.Curso + ")"));
            }
            Seleccionar(cmbComision, seleccionado);
        }

        private void cmbCurso_SelectionChangeCommitted(object sender, EventArgs e)
        {
            RellenarComisiones();
        }

        // ------------------------------------------------------------------
        // Búsqueda
        // ------------------------------------------------------------------

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            Buscar();
        }

        private void Buscar()
        {
            try
            {
                DateTime fecha = DateTime.Now;
                _filas = _bll.Generar(LeerFiltro());
                _fechaReporte = fecha;
                _hayBusqueda = true;
                MostrarResultado();
            }
            catch (Exception ex)
            {
                IdiomaUiHelper_83KI.MostrarError(this, ex, "Comun.Validacion", MessageBoxIcon.Warning);
            }
        }

        private FiltroReporteMorosidad_83KI LeerFiltro()
        {
            return new FiltroReporteMorosidad_83KI
            {
                IdCurso = IdSeleccionado(cmbCurso),
                IdComision = IdSeleccionado(cmbComision),
                DNI = txtDni.Text.Trim(),
                DiasAtrasoMinimo = nudDiasMinimos.Value > 0 ? (int?)nudDiasMinimos.Value : null,
                DeudaVencidaMinima = nudDeudaMinima.Value > 0 ? (decimal?)nudDeudaMinima.Value : null
            };
        }

        private void MostrarResultado()
        {
            dgvMorosidad.Rows.Clear();
            foreach (var f in _filas)
            {
                int indice = dgvMorosidad.Rows.Add(
                    f.CodigoInscripcion,
                    f.DNI,
                    f.ApellidoYNombre,
                    f.Email,
                    f.Telefono,
                    f.CodigoComision,
                    f.Curso,
                    f.PlanDePago,
                    f.CantidadCuotasVencidas,
                    f.VencimientoImpagoMasAntiguo,
                    f.DiasAtraso,
                    f.DeudaVencida,
                    f.DeudaTotal);
                dgvMorosidad.Rows[indice].Tag = f;
            }

            lblTotales.Text = _filas.Count == 0
                ? IdiomaUiHelper_83KI.Texto("ReporteMorosidad.SinResultados")
                : IdiomaUiHelper_83KI.Texto("ReporteMorosidad.Totales",
                    _filas.Count,
                    _filas.Sum(f => f.DeudaVencida).ToString("N2"),
                    _filas.Sum(f => f.DeudaTotal).ToString("N2"));

            lblFecha.Text = IdiomaUiHelper_83KI.Texto("ReporteMorosidad.Generado", _fechaReporte.ToString("dd/MM/yyyy HH:mm"));
            MostrarTorta();
        }

        private void dgvMorosidad_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) MostrarDetalle(dgvMorosidad.Rows[e.RowIndex]);
        }

        private void dgvMorosidad_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter || dgvMorosidad.CurrentRow == null) return;
            e.Handled = true;
            e.SuppressKeyPress = true;
            MostrarDetalle(dgvMorosidad.CurrentRow);
        }

        private void MostrarDetalle(DataGridViewRow row)
        {
            var fila = row.Tag as ReporteMorosidadFila_83KI;
            if (fila == null) return;
            using (var detalle = new FrmDetalleMorosidad_83KI(fila, _fechaReporte))
                detalle.ShowDialog(this);
        }

        private void MostrarTorta()
        {
            Series serie = chartAtraso.Series["SerieAtraso"];
            serie.Points.Clear();

            Dictionary<string, int> conteo = KI68ReporteMorosidadBLL.ContarPorRango(_filas);
            AgregarPorcion(serie, IdiomaUiHelper_83KI.Texto("ReporteMorosidad.Rango1a30"), conteo[KI68ReporteMorosidadBLL.Rango1a30], Color1a30);
            AgregarPorcion(serie, IdiomaUiHelper_83KI.Texto("ReporteMorosidad.Rango31a60"), conteo[KI68ReporteMorosidadBLL.Rango31a60], Color31a60);
            AgregarPorcion(serie, IdiomaUiHelper_83KI.Texto("ReporteMorosidad.RangoMasDe60"), conteo[KI68ReporteMorosidadBLL.RangoMasDe60], ColorMasDe60);

            chartAtraso.Titles["TituloAtraso"].Text = serie.Points.Count == 0
                ? IdiomaUiHelper_83KI.Texto("ReporteMorosidad.GraficoSinDatos")
                : IdiomaUiHelper_83KI.Texto("ReporteMorosidad.GraficoTitulo");
        }

        // Los rangos sin inscripciones no se dibujan para no mostrar porciones de 0 %.
        private static void AgregarPorcion(Series serie, string texto, int cantidad, Color color)
        {
            if (cantidad <= 0) return;
            int indice = serie.Points.AddY(cantidad);
            DataPoint punto = serie.Points[indice];
            punto.Color = color;
            punto.LegendText = texto;
            punto.ToolTip = texto + ": " + cantidad;
        }

        // ------------------------------------------------------------------
        // Limpiar
        // ------------------------------------------------------------------

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            Seleccionar(cmbCurso, null);
            RellenarComisiones();
            Seleccionar(cmbComision, null);
            txtDni.Clear();
            nudDiasMinimos.Value = 0;
            nudDeudaMinima.Value = 0;

            _filas = new List<ReporteMorosidadFila_83KI>();
            _hayBusqueda = false;
            dgvMorosidad.Rows.Clear();
            chartAtraso.Series["SerieAtraso"].Points.Clear();
            chartAtraso.Titles["TituloAtraso"].Text = IdiomaUiHelper_83KI.Texto("ReporteMorosidad.GraficoTitulo");
            lblTotales.Text = IdiomaUiHelper_83KI.Texto("ReporteMorosidad.Instrucciones");
            lblFecha.Text = string.Empty;
        }

        // ------------------------------------------------------------------
        // Idioma
        // ------------------------------------------------------------------

        public void ActualizarIdioma(IIdioma idioma)
        {
            Text = IdiomaUiHelper_83KI.Texto("ReporteMorosidad.Titulo");
            lblCurso.Text = IdiomaUiHelper_83KI.Texto("ReporteMorosidad.Curso");
            lblComision.Text = IdiomaUiHelper_83KI.Texto("ReporteMorosidad.Comision");
            lblDni.Text = IdiomaUiHelper_83KI.Texto("Comun.Dni");
            lblDiasMinimos.Text = IdiomaUiHelper_83KI.Texto("ReporteMorosidad.DiasMinimos");
            lblDeudaMinima.Text = IdiomaUiHelper_83KI.Texto("ReporteMorosidad.DeudaMinima");
            btnBuscar.Text = IdiomaUiHelper_83KI.Texto("ReporteMorosidad.Buscar");
            btnLimpiar.Text = IdiomaUiHelper_83KI.Texto("Comun.Limpiar");

            colCodigoInscripcion.HeaderText = IdiomaUiHelper_83KI.Texto("ReporteMorosidad.ColInscripcion");
            colDni.HeaderText = IdiomaUiHelper_83KI.Texto("Comun.Dni");
            colAlumno.HeaderText = IdiomaUiHelper_83KI.Texto("ReporteMorosidad.ColAlumno");
            colEmail.HeaderText = IdiomaUiHelper_83KI.Texto("Comun.Email");
            colTelefono.HeaderText = IdiomaUiHelper_83KI.Texto("ReporteMorosidad.ColTelefono");
            colComision.HeaderText = IdiomaUiHelper_83KI.Texto("ReporteMorosidad.Comision");
            colCurso.HeaderText = IdiomaUiHelper_83KI.Texto("ReporteMorosidad.Curso");
            colPlanDePago.HeaderText = IdiomaUiHelper_83KI.Texto("ReporteMorosidad.ColPlanDePago");
            colCuotasVencidas.HeaderText = IdiomaUiHelper_83KI.Texto("ReporteMorosidad.ColCuotasVencidas");
            colVencimientoMasAntiguo.HeaderText = IdiomaUiHelper_83KI.Texto("ReporteMorosidad.ColVencimientoMasAntiguo");
            colDiasAtraso.HeaderText = IdiomaUiHelper_83KI.Texto("ReporteMorosidad.ColDiasAtraso");
            colDeudaVencida.HeaderText = IdiomaUiHelper_83KI.Texto("ReporteMorosidad.ColDeudaVencida");
            colDeudaTotal.HeaderText = IdiomaUiHelper_83KI.Texto("ReporteMorosidad.ColDeudaTotal");

            // los combos tienen el texto "Todos" traducido
            RellenarCursos();
            RellenarComisiones();

            if (_hayBusqueda)
            {
                MostrarResultado();
            }
            else
            {
                chartAtraso.Titles["TituloAtraso"].Text = IdiomaUiHelper_83KI.Texto("ReporteMorosidad.GraficoTitulo");
                lblTotales.Text = IdiomaUiHelper_83KI.Texto("ReporteMorosidad.Instrucciones");
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _gestorIdioma.Desuscribir(this);
            base.OnFormClosed(e);
        }

        // ------------------------------------------------------------------
        // Utilidades de combos
        // ------------------------------------------------------------------

        private static int? IdSeleccionado(ComboBox combo)
        {
            var opcion = combo.SelectedItem as OpcionFiltro;
            return opcion != null ? opcion.Id : null;
        }

        private static void Seleccionar(ComboBox combo, int? id)
        {
            for (int i = 0; i < combo.Items.Count; i++)
            {
                var opcion = (OpcionFiltro)combo.Items[i];
                if (opcion.Id == id)
                {
                    combo.SelectedIndex = i;
                    return;
                }
            }
            if (combo.Items.Count > 0) combo.SelectedIndex = 0;
        }

        // ítem de los combos de filtros: Id null = "Todos"
        private class OpcionFiltro
        {
            public OpcionFiltro(int? id, string texto)
            {
                Id = id;
                Texto = texto;
            }

            public int? Id { get; private set; }
            public string Texto { get; private set; }

            public override string ToString()
            {
                return Texto;
            }
        }
    }
}

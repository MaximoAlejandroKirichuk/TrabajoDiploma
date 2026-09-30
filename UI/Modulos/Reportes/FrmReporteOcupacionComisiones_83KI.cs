using Service;
using Service.DTOs;
using Service.Entidades;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace UI.Modulos.Reportes
{
    public partial class FrmReporteOcupacionComisiones_83KI : Form, IObservadorIdioma
    {
        private readonly IReporteOcupacionComisionBLL_83KI _bll;
        private readonly IGestorIdioma_83KI _gestorIdioma;

        // mismo orden que los items del combo; null = todos
        private readonly string[] _estados = { null, "preapertura", "confirmada", "cancelada_falta_quorum" };

        // datos para el grafico de torta
        private int _pagaron;
        private int _sinPagar;

        public FrmReporteOcupacionComisiones_83KI(IReporteOcupacionComisionBLL_83KI bll)
        {
            _bll = bll;
            _gestorIdioma = ServiceFactory_83KI.GetGestorIdioma();
            InitializeComponent();
            _gestorIdioma.Suscribir(this);
            Cargar();
        }

        private void Cargar()
        {
            try
            {
                string estado = cmbEstado.SelectedIndex > 0 ? _estados[cmbEstado.SelectedIndex] : null;
                List<ReporteOcupacionComision_83KI> filas = _bll.Generar(estado).ToList();

                dgvReporte.Rows.Clear();
                foreach (var f in filas)
                {
                    dgvReporte.Rows.Add(
                        f.Codigo,
                        f.Curso,
                        f.Profesor,
                        f.FechaLimitePago.ToString("dd/MM/yyyy"),
                        f.CupoMinimo,
                        f.CupoMaximo,
                        f.Inscriptos,
                        f.Pagaron,
                        f.PendientesPago,
                        f.VacantesLibres,
                        f.FaltanParaQuorum,
                        TextoEstado(f.Estado),
                        f.NumeroActa,
                        f.CuotasReintegro,
                        f.MontoReintegro.ToString("N2"));
                }

                lblTotales.Text = IdiomaUiHelper_83KI.Texto("FrmReporteOcupacionComisiones.Totales",
                    filas.Count,
                    filas.Sum(x => x.Inscriptos),
                    filas.Sum(x => x.Pagaron),
                    filas.Count(x => x.FaltanParaQuorum > 0),
                    filas.Sum(x => x.MontoReintegro).ToString("N2"));

                lblFecha.Text = IdiomaUiHelper_83KI.Texto("FrmReporteOcupacionComisiones.Generado", DateTime.Now.ToString("dd/MM/yyyy HH:mm"));

                _pagaron = filas.Sum(x => x.Pagaron);
                _sinPagar = filas.Sum(x => x.PendientesPago);
                pnlGrafico.Invalidate();
            }
            catch (Exception ex)
            {
                IdiomaUiHelper_83KI.MostrarError(this, ex, "Comun.Validacion", MessageBoxIcon.Warning);
            }
        }

        private string TextoEstado(string estado)
        {
            switch (estado)
            {
                case "preapertura": return IdiomaUiHelper_83KI.Texto("FrmReporteOcupacionComisiones.EstadoPreapertura");
                case "confirmada": return IdiomaUiHelper_83KI.Texto("FrmReporteOcupacionComisiones.EstadoConfirmada");
                case "cancelada_falta_quorum": return IdiomaUiHelper_83KI.Texto("FrmReporteOcupacionComisiones.EstadoCancelada");
                default: return estado;
            }
        }

        private void pnlGrafico_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            g.DrawString(IdiomaUiHelper_83KI.Texto("FrmReporteOcupacionComisiones.GraficoTitulo"), Font, Brushes.Black, 10, 10);

            int lado = Math.Min(pnlGrafico.Width - 40, pnlGrafico.Height - 120);
            if (lado < 20) return;
            Rectangle area = new Rectangle((pnlGrafico.Width - lado) / 2, 40, lado, lado);

            int total = _pagaron + _sinPagar;
            Color colorPagaron = Color.FromArgb(76, 154, 106);
            Color colorSinPagar = Color.FromArgb(222, 139, 62);

            if (total == 0)
            {
                g.DrawEllipse(Pens.LightGray, area);
                g.DrawString(IdiomaUiHelper_83KI.Texto("FrmReporteOcupacionComisiones.GraficoSinDatos"), Font, Brushes.Gray, area.Left, area.Bottom + 15);
                return;
            }

            float angulo = 360f * _pagaron / total;
            using (var pincelPagaron = new SolidBrush(colorPagaron))
            using (var pincelSinPagar = new SolidBrush(colorSinPagar))
            {
                g.FillPie(pincelPagaron, area, -90, angulo);
                g.FillPie(pincelSinPagar, area, -90 + angulo, 360 - angulo);
            }

            int y = area.Bottom + 15;
            DibujarReferencia(g, colorPagaron, IdiomaUiHelper_83KI.Texto("FrmReporteOcupacionComisiones.ColPagaron"), _pagaron, total, area.Left, y);
            DibujarReferencia(g, colorSinPagar, IdiomaUiHelper_83KI.Texto("FrmReporteOcupacionComisiones.ColPendientes"), _sinPagar, total, area.Left, y + 22);
        }

        private void DibujarReferencia(Graphics g, Color color, string texto, int valor, int total, int x, int y)
        {
            using (var pincel = new SolidBrush(color))
            {
                g.FillRectangle(pincel, x, y, 12, 12);
            }
            string linea = string.Format("{0}: {1} ({2:0}%)", texto, valor, 100.0 * valor / total);
            g.DrawString(linea, Font, Brushes.Black, x + 18, y - 1);
        }

        private void pnlGrafico_Resize(object sender, EventArgs e)
        {
            pnlGrafico.Invalidate();
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            Cargar();
        }

        private void cmbEstado_SelectionChangeCommitted(object sender, EventArgs e)
        {
            Cargar();
        }

        public void ActualizarIdioma(IIdioma idioma)
        {
            Text = IdiomaUiHelper_83KI.Texto("FrmReporteOcupacionComisiones.Titulo");
            lblEstado.Text = IdiomaUiHelper_83KI.Texto("FrmReporteOcupacionComisiones.Estado");
            btnActualizar.Text = IdiomaUiHelper_83KI.Texto("Comun.Actualizar");

            int seleccionado = cmbEstado.SelectedIndex < 0 ? 0 : cmbEstado.SelectedIndex;
            cmbEstado.Items.Clear();
            cmbEstado.Items.Add(IdiomaUiHelper_83KI.Texto("Comun.Todos"));
            cmbEstado.Items.Add(IdiomaUiHelper_83KI.Texto("FrmReporteOcupacionComisiones.EstadoPreapertura"));
            cmbEstado.Items.Add(IdiomaUiHelper_83KI.Texto("FrmReporteOcupacionComisiones.EstadoConfirmada"));
            cmbEstado.Items.Add(IdiomaUiHelper_83KI.Texto("FrmReporteOcupacionComisiones.EstadoCancelada"));
            cmbEstado.SelectedIndex = seleccionado;

            colCodigo.HeaderText = IdiomaUiHelper_83KI.Texto("FrmReporteOcupacionComisiones.ColComision");
            colCurso.HeaderText = IdiomaUiHelper_83KI.Texto("FrmReporteOcupacionComisiones.ColCurso");
            colProfesor.HeaderText = IdiomaUiHelper_83KI.Texto("FrmReporteOcupacionComisiones.ColProfesor");
            colFechaLimite.HeaderText = IdiomaUiHelper_83KI.Texto("FrmReporteOcupacionComisiones.ColFechaLimite");
            colCupoMinimo.HeaderText = IdiomaUiHelper_83KI.Texto("FrmReporteOcupacionComisiones.ColCupoMinimo");
            colCupoMaximo.HeaderText = IdiomaUiHelper_83KI.Texto("FrmReporteOcupacionComisiones.ColCupoMaximo");
            colInscriptos.HeaderText = IdiomaUiHelper_83KI.Texto("FrmReporteOcupacionComisiones.ColInscriptos");
            colPagaron.HeaderText = IdiomaUiHelper_83KI.Texto("FrmReporteOcupacionComisiones.ColPagaron");
            colPendientes.HeaderText = IdiomaUiHelper_83KI.Texto("FrmReporteOcupacionComisiones.ColPendientes");
            colVacantes.HeaderText = IdiomaUiHelper_83KI.Texto("FrmReporteOcupacionComisiones.ColVacantes");
            colFaltan.HeaderText = IdiomaUiHelper_83KI.Texto("FrmReporteOcupacionComisiones.ColFaltan");
            colEstado.HeaderText = IdiomaUiHelper_83KI.Texto("FrmReporteOcupacionComisiones.ColEstado");
            colActa.HeaderText = IdiomaUiHelper_83KI.Texto("FrmReporteOcupacionComisiones.ColActa");
            colCuotasReintegro.HeaderText = IdiomaUiHelper_83KI.Texto("FrmReporteOcupacionComisiones.ColCuotasReintegro");
            colMontoReintegro.HeaderText = IdiomaUiHelper_83KI.Texto("FrmReporteOcupacionComisiones.ColMontoReintegro");

            // si ya habia datos, recargo para traducir la columna Estado
            if (dgvReporte.Rows.Count > 0) Cargar();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _gestorIdioma.Desuscribir(this);
            base.OnFormClosed(e);
        }
    }
}

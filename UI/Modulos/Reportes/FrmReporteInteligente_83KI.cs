using BE.DTOs.Reportes;
using BE.Entidades;
using Service.Entidades;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace UI.Modulos.Reportes
{
    public sealed class FrmReporteInteligente_83KI : FormConAyuda_83KI, IObservadorIdioma
    {
        private readonly IReporteInteligenteBLL_83KI _bll;
        private readonly IGestorIdioma_83KI _idioma = ServiceFactory_83KI.GetGestorIdioma();
        private readonly DateTimePicker _desde = new DateTimePicker { ShowCheckBox = true, Checked = false, Format = DateTimePickerFormat.Short, Width = 125 };
        private readonly DateTimePicker _hasta = new DateTimePicker { ShowCheckBox = true, Checked = false, Format = DateTimePickerFormat.Short, Width = 125 };
        private readonly ComboBox _curso = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 250 };
        private readonly ComboBox _condicion = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 180 };
        private readonly TabControl _tabs = new TabControl { Dock = DockStyle.Fill };
        private readonly Label _nota = new Label { AutoSize = false, Dock = DockStyle.Top, Height = 58, Padding = new Padding(8) };
        private readonly Dictionary<string, Control> _textos = new Dictionary<string, Control>();
        private readonly DataGridView _demanda = Grilla(), _ocupacion = Grilla(), _oferta = Grilla(), _candidatos = Grilla();
        private readonly DataGridView _cumplimiento = Grilla();
        private ReporteInteligente_83KI _reporte;
        private Exception _errorConsulta;
        private List<Curso_83KI> _cursos = new List<Curso_83KI>();

        public FrmReporteInteligente_83KI(IReporteInteligenteBLL_83KI bll)
        {
            _bll = bll;
            Name = nameof(FrmReporteInteligente_83KI);
            ClientSize = new Size(1200, 700); MinimumSize = new Size(900, 500);
            StartPosition = FormStartPosition.CenterParent; Font = new Font("Segoe UI", 9F);
            var filtros = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 76, Padding = new Padding(8), AutoScroll = true };
            AgregarEtiqueta(filtros, "Inteligente.Desde"); filtros.Controls.Add(_desde);
            AgregarEtiqueta(filtros, "Inteligente.Hasta"); filtros.Controls.Add(_hasta);
            AgregarEtiqueta(filtros, "Inteligente.Curso"); filtros.Controls.Add(_curso);
            var consultar = new Button { AutoSize = true }; _textos.Add("Comun.Actualizar", consultar); filtros.Controls.Add(consultar);
            consultar.Click += (s,e) => Cargar();
            AgregarTab("Inteligente.Demanda", _demanda); AgregarTab("Inteligente.Ocupacion", _ocupacion);
            AgregarTab("Inteligente.Oferta", _oferta);
            AgregarTab("Inteligente.Cumplimiento", _cumplimiento);
            var censo = new TabPage(); _textos.Add("Inteligente.Censo", censo);
            var selector = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 40, Padding = new Padding(5) };
            selector.Controls.Add(_condicion); censo.Controls.Add(_candidatos); censo.Controls.Add(selector); _tabs.TabPages.Add(censo);
            _condicion.SelectedIndexChanged += (s,e) => MostrarCandidatos();
            _ocupacion.CellFormatting += TraducirEstado;
            _oferta.CellFormatting += TraducirEstado;
            Controls.Add(_tabs); Controls.Add(_nota); Controls.Add(filtros);
            try
            {
                _cursos = _bll.Cursos();
                CargarOpcionesCurso();
            }
            catch (Exception ex) { MostrarError(ex); }
            _idioma.Suscribir(this); Shown += (s,e) => Cargar();
        }
        private static DataGridView Grilla()
        {
            return new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false,
                AllowUserToDeleteRows = false, AutoGenerateColumns = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect, MultiSelect = false, RowHeadersVisible = false, BackgroundColor = SystemColors.Window };
        }
        private void AgregarEtiqueta(FlowLayoutPanel panel, string clave)
        { var label = new Label { AutoSize = true, Padding = new Padding(0,6,0,0) }; _textos.Add(clave,label); panel.Controls.Add(label); }
        private void AgregarTab(string clave, Control control)
        { var tab = new TabPage(); tab.Controls.Add(control); _tabs.TabPages.Add(tab); _textos.Add(clave,tab); }
        private static string T(string key) { return IdiomaUiHelper_83KI.Texto(key); }
        private void CargarOpcionesCurso()
        {
            int seleccionado = _curso.SelectedValue is int ? (int)_curso.SelectedValue : 0;
            _curso.DisplayMember = "Nombre"; _curso.ValueMember = "IdCurso";
            _curso.DataSource = new[] { new { IdCurso = 0, Nombre = T("Comun.Todos") } }
                .Concat(_cursos.Select(c => new { c.IdCurso, c.Nombre })).ToList();
            _curso.SelectedValue = seleccionado;
        }
        private void MostrarError(Exception ex) { IdiomaUiHelper_83KI.MostrarError(this, ex, "Comun.Validacion", MessageBoxIcon.Warning); }
        private void Cargar()
        {
            try
            {
                int id = _curso.SelectedValue is int ? (int)_curso.SelectedValue : 0;
                _reporte = _bll.Generar(new FiltroReporteInteligente_83KI { Desde = _desde.Checked ? _desde.Value.Date : (DateTime?)null,
                    Hasta = _hasta.Checked ? _hasta.Value.Date : (DateTime?)null, IdCurso = id > 0 ? id : (int?)null });
                _demanda.DataSource = _reporte.Demanda; _ocupacion.DataSource = _reporte.OcupacionHistorica;
                _cumplimiento.DataSource = _reporte.Cumplimiento;
                _oferta.DataSource = _reporte.OfertaActual; MostrarCandidatos(); TraducirGrillas();
                _errorConsulta = null;
                ActualizarNota();
            }
            catch (Exception ex)
            {
                _errorConsulta = ex;
                _reporte = null;
                foreach (var grid in new[] { _demanda, _ocupacion, _oferta, _candidatos, _cumplimiento })
                {
                    grid.DataSource = null;
                    grid.Rows.Clear();
                }
                ActualizarNota();
            }
        }
        private void ActualizarNota()
        {
            _nota.ForeColor = _errorConsulta == null ? SystemColors.ControlText : Color.Firebrick;
            _nota.Text = _errorConsulta == null
                ? T("Inteligente.Nota") + (_reporte == null ? "" : " " + _reporte.FechaCenso.ToShortDateString())
                : T("Inteligente.ErrorConsulta") + " " + IdiomaUiHelper_83KI.TraducirExcepcion(_errorConsulta);
        }
        private void TraducirEstado(object sender, DataGridViewCellFormattingEventArgs e)
        {
            var grid = (DataGridView)sender;
            if (grid.Columns[e.ColumnIndex].DataPropertyName != "Estado") return;
            string estado = Convert.ToString(e.Value);
            switch (estado)
            {
                case "preapertura": case "confirmada": case "cancelada_falta_quorum": case "eliminada":
                    e.Value = T("Inteligente.Estado." + estado); break;
                case "": e.Value = T("Inteligente.Estado.SinComision"); break;
                default: e.Value = T("Inteligente.Estado.Desconocido"); break;
            }
            e.FormattingApplied = true;
        }
        private void MostrarCandidatos()
        {
            if (_reporte == null) return;
            int filtro = _condicion.SelectedIndex;
            _candidatos.DataSource = _reporte.Candidatos.Where(c => filtro <= 0 || (filtro == 1 ? c.Elegible : !c.Elegible))
                .Select(c => new { c.Alumno, c.Email, c.Telefono, c.CursoAprobado, c.CursoRecomendado,
                    Motivo = T(c.Motivo), c.DeudaVencida, c.DiasAtraso, c.SaldoFuturo }).ToList();
            TraducirGrillas();
        }
        private void TraducirGrillas()
        {
            foreach (var grid in new[] { _demanda, _ocupacion, _oferta, _candidatos, _cumplimiento })
            foreach (DataGridViewColumn col in grid.Columns)
            {
                col.Visible = !col.DataPropertyName.StartsWith("Id", StringComparison.Ordinal);
                if (grid == _ocupacion && (col.DataPropertyName == "Elegibles" || col.DataPropertyName == "Condicionados" || col.DataPropertyName == "BrechaConElegibles")) col.Visible = false;
                if (grid == _oferta && col.DataPropertyName == "Brecha" && _reporte != null && _reporte.OfertaActual.All(o => !o.Brecha.HasValue)) col.Visible = false;
                if (col.DataPropertyName == "Anuladas" && _reporte != null) col.Visible = _reporte.Demanda.Any(d => d.Anuladas > 0);
                col.HeaderText = T("Inteligente.Col." + col.DataPropertyName);
                if (col.ValueType == typeof(decimal) || col.ValueType == typeof(decimal?)) col.DefaultCellStyle.Format = "N2";
            }
            _ocupacion.Invalidate(); _oferta.Invalidate();
        }
        public void ActualizarIdioma(IIdioma idioma)
        {
            Text = T("Inteligente.Titulo");
            CargarOpcionesCurso();
            foreach (var item in _textos) item.Value.Text = T(item.Key);
            int seleccionado = Math.Max(0,_condicion.SelectedIndex);
            _condicion.Items.Clear(); _condicion.Items.AddRange(new object[] { T("Comun.Todos"),T("Inteligente.Elegibles"),T("Inteligente.Condicionados") });
            _condicion.SelectedIndex = seleccionado;
            ActualizarNota();
            MostrarCandidatos(); TraducirGrillas();
        }
        protected override void OnFormClosed(FormClosedEventArgs e) { _idioma.Desuscribir(this); base.OnFormClosed(e); }
    }
}

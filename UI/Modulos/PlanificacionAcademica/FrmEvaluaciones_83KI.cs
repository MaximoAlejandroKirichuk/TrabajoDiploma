using BE.Entidades;
using BLL;
using Service;
using Service.DTOs;
using Service.Entidades;
using Service.Interfaces;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace UI.Modulos.PlanificacionAcademica
{
    public sealed class FrmEvaluaciones_83KI : Form, IObservadorIdioma
    {
        private readonly KI68EvaluacionesBLL _bll;
        private readonly IGestorIdioma_83KI _idioma;
        private readonly ComboBox _comisiones = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 250 };
        private readonly DataGridView _resultados = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true,
            AllowUserToAddRows = false, AllowUserToDeleteRows = false, MultiSelect = false, RowHeadersVisible = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
        private readonly NumericUpDown _nota = new NumericUpDown { Minimum = 1, Maximum = 10, DecimalPlaces = 2, Increment = 0.01m, Width = 100 };
        private readonly DateTimePicker _fecha = new DateTimePicker { Format = DateTimePickerFormat.Short, Width = 140 };
        private readonly TextBox _obs = new TextBox { MaxLength = 500, Multiline = true, ScrollBars = ScrollBars.Vertical, Dock = DockStyle.Fill };
        private readonly Label _estado = new Label { AutoSize = true, Dock = DockStyle.Fill };
        private readonly Button _buscar = new Button { AutoSize = true, Tag = "Cun09.Buscar" };
        private readonly Button _guardar = new Button { AutoSize = true, Tag = "Evaluaciones.Guardar" };
        private readonly Button _cancelar = new Button { AutoSize = true, Tag = "Evaluaciones.Cancelar" };
        private bool _cargando;
        private string _estadoKey = "Evaluaciones.Seleccionar";

        public FrmEvaluaciones_83KI(KI68EvaluacionesBLL bll) : this(bll, ServiceFactory_83KI.GetGestorIdioma()) { }
        public FrmEvaluaciones_83KI(KI68EvaluacionesBLL bll, IGestorIdioma_83KI idioma)
        {
            _bll = bll ?? throw new ArgumentNullException(nameof(bll)); _idioma = idioma ?? throw new ArgumentNullException(nameof(idioma));
            Width = 1150; Height = 700; MinimumSize = new Size(850, 600); StartPosition = FormStartPosition.CenterParent;
            var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 7, Padding = new Padding(12) };
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42)); layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42)); layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 24)); layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 80));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
            var filtros = new FlowLayoutPanel { Dock = DockStyle.Fill };
            filtros.Controls.AddRange(new Control[] { Etiqueta("Evaluaciones.Comision"), _comisiones, _buscar });
            layout.Controls.Add(filtros, 0, 0); layout.Controls.Add(_resultados, 0, 1); layout.Controls.Add(_estado, 0, 2);
            var campos = new FlowLayoutPanel { Dock = DockStyle.Fill };
            campos.Controls.AddRange(new Control[] { Etiqueta("Evaluaciones.Nota"), _nota, Etiqueta("Evaluaciones.Fecha"), _fecha });
            layout.Controls.Add(campos, 0, 3); layout.Controls.Add(Etiqueta("Cun09.Observaciones"), 0, 4); layout.Controls.Add(_obs, 0, 5);
            var acciones = new FlowLayoutPanel { Dock = DockStyle.Fill }; acciones.Controls.AddRange(new Control[] { _guardar, _cancelar });
            layout.Controls.Add(acciones, 0, 6); Controls.Add(layout);
            foreach (string key in new[] { "Evaluaciones.Id", "Evaluaciones.Inscripcion", "Evaluaciones.Alumno", "Evaluaciones.Fecha", "Evaluaciones.Nota", "Cun09.Condicion", "Cun09.Observaciones" })
                _resultados.Columns.Add(key, key);
            _resultados.Columns[0].FillWeight = 40; _resultados.Columns[1].FillWeight = 45; _resultados.Columns[2].FillWeight = 180;
            _resultados.Columns[6].FillWeight = 200;
            _buscar.Click += (s, e) => Ejecutar(Buscar);
            _comisiones.SelectedIndexChanged += (s, e) => Limpiar();
            _resultados.SelectionChanged += (s, e) => { if (!_cargando) Ejecutar(Seleccionar); };
            _guardar.Click += (s, e) => Ejecutar(Guardar);
            _cancelar.Click += (s, e) => { _bll.Cancelar(); _resultados.ClearSelection(); Mostrar(null); };
            _idioma.Suscribir(this); Mostrar(null); ActualizarIdioma(null);
        }
        private static Label Etiqueta(string key) { return new Label { Tag = key, AutoSize = true, Padding = new Padding(0, 5, 0, 0) }; }
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            try { _comisiones.DisplayMember = "Codigo"; _comisiones.DataSource = _bll.ListarComisionesConResultados(); }
            catch (Exception ex) { Error(ex); Close(); }
        }
        private void Limpiar()
        {
            _bll.Cancelar(); _cargando = true;
            try { _resultados.Rows.Clear(); Mostrar(null); } finally { _cargando = false; }
        }
        private void Buscar()
        {
            Limpiar();
            var c = _comisiones.SelectedItem as ComisionListado_83KI;
            var rows = _bll.Buscar(c == null ? 0 : c.IdComision);
            _cargando = true;
            try
            {
                foreach (var r in rows)
                {
                    int n = _resultados.Rows.Add(r.IdResultado, r.IdInscripcion, r.Alumno, r.FechaEvaluacion.ToString("d"),
                        r.Calificacion.ToString("0.00"), T(r.Calificacion >= 7 ? "Cun09.Aprobado" : "Cun09.Desaprobado"), r.Observaciones);
                    _resultados.Rows[n].Tag = r.IdResultado;
                }
                _resultados.ClearSelection();
                if (rows.Count == 0) Estado("Evaluaciones.SinResultados");
            }
            finally { _cargando = false; }
        }
        private void Seleccionar()
        {
            _bll.Cancelar(); Mostrar(null);
            if (_resultados.SelectedRows.Count > 0) Mostrar(_bll.Seleccionar((int)_resultados.SelectedRows[0].Tag));
        }
        private void Mostrar(EvaluacionConsulta_83KI r)
        {
            _nota.Enabled = _fecha.Enabled = _guardar.Enabled = _cancelar.Enabled = false; _obs.ReadOnly = true; _obs.Clear();
            Estado("Evaluaciones.Seleccionar");
            if (r == null) return;
            _nota.Value = Math.Max(1, Math.Min(10, r.Calificacion)); _obs.Text = r.Observaciones; _cancelar.Enabled = true;
            // El valor histórico exacto queda siempre visible en la grilla. No asignar límites inválidos al selector.
            DateTime inicio = r.Inicio < DateTimePicker.MinimumDateTime ? DateTimePicker.MinimumDateTime : r.Inicio;
            DateTime fin = r.Fin < r.Hoy ? r.Fin : r.Hoy;
            if (fin > DateTimePicker.MaximumDateTime) fin = DateTimePicker.MaximumDateTime;
            _fecha.MinDate = DateTimePicker.MinimumDateTime; _fecha.MaxDate = DateTimePicker.MaximumDateTime;
            bool rango = inicio <= fin && inicio <= DateTimePicker.MaximumDateTime && fin >= DateTimePicker.MinimumDateTime;
            if (rango)
            {
                _fecha.MaxDate = fin; _fecha.MinDate = inicio;
                _fecha.Value = r.FechaEvaluacion < inicio ? inicio : r.FechaEvaluacion > fin ? fin : r.FechaEvaluacion;
            }
            bool permitido = _bll.PuedeModificar();
            bool editable = permitido && !r.TieneCertificado && r.CantidadNotas <= 2 && rango;
            _nota.Enabled = _fecha.Enabled = _guardar.Enabled = editable; _obs.ReadOnly = !editable;
            Estado(r.TieneCertificado ? "Evaluaciones.Certificado" : !permitido ? "Evaluaciones.SoloLectura" :
                r.CantidadNotas > 2 ? "Evaluaciones.Legado" : !rango ? "Cun08.FechaInvalida" :
                r.FechaEvaluacion < inicio || r.FechaEvaluacion > fin ? "Evaluaciones.FechaHistorica" : "Evaluaciones.Editable");
        }
        private void Guardar()
        {
            if (MessageBox.Show(this, T("Evaluaciones.Confirmar"), Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            var result = _bll.Modificar(new ModificarEvaluacionRequest_83KI { Calificacion = _nota.Value, FechaEvaluacion = _fecha.Value.Date, Observaciones = _obs.Text });
            Mostrar(null);
            Mensaje(result.Modificado ? "Evaluaciones.Guardado" : "Evaluaciones.SinCambios");
            if (result.AuditoriaFallida) Mensaje("Cun08.AuditoriaFallida");
            try { Buscar(); } catch { Limpiar(); Mensaje("Evaluaciones.Recargar"); } // No informar fallo de una escritura ya confirmada.
        }
        private void Estado(string key) { _estadoKey = key; _estado.Text = T(key); }
        private string T(string key) { return _idioma.ObtenerTexto(key); }
        private void Mensaje(string key) { MessageBox.Show(this, T(key), T("Evaluaciones.Titulo"), MessageBoxButtons.OK, MessageBoxIcon.Information); }
        private void Ejecutar(Action action) { try { action(); } catch (Exception ex) { Error(ex); } }
        private void Error(Exception ex)
        {
            string key = (ex.Message ?? "").Split('\r', '\n')[0];
            Mensaje(key.StartsWith("Evaluaciones.", StringComparison.Ordinal) || key.StartsWith("Cun08.", StringComparison.Ordinal)
                || key == "Errores.SinPermisos" ? key : "Evaluaciones.Error");
        }
        public void ActualizarIdioma(IIdioma idioma)
        {
            Text = T("Evaluaciones.Titulo"); Traducir(Controls); _estado.Text = T(_estadoKey);
            foreach (DataGridViewColumn c in _resultados.Columns) c.HeaderText = T(c.Name);
            foreach (DataGridViewRow row in _resultados.Rows) row.Cells[5].Value = T(Convert.ToDecimal(row.Cells[4].Value) >= 7 ? "Cun09.Aprobado" : "Cun09.Desaprobado");
        }
        private void Traducir(Control.ControlCollection controls) { foreach (Control c in controls) { if (c.Tag is string) c.Text = T((string)c.Tag); Traducir(c.Controls); } }
        protected override void OnFormClosed(FormClosedEventArgs e) { _bll.Cancelar(); _idioma.Desuscribir(this); base.OnFormClosed(e); }
    }
}

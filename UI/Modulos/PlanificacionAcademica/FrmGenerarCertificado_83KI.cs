using BE.Entidades;
using BE.DTOs.Certificaciones;
using BLL;
using Service;
using Service.Entidades;
using Service.Interfaces;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace UI.Modulos.PlanificacionAcademica
{
    public sealed class FrmGenerarCertificado_83KI : FormConAyuda_83KI, IObservadorIdioma
    {
        private readonly KI68CertificadoBLL _bll;
        private readonly IGestorIdioma_83KI _idioma;
        private readonly ComboBox _comisiones = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 240 };
        private readonly DataGridView _alumnos = Grid();
        private readonly DataGridView _cuotas = Grid();
        private readonly TextBox _detalle = new TextBox { Multiline = true, ReadOnly = true, ScrollBars = ScrollBars.Vertical, Dock = DockStyle.Fill };
        private readonly TableLayoutPanel _campos = new TableLayoutPanel { ColumnCount = 4, RowCount = 4, Dock = DockStyle.Fill, Padding = new Padding(0, 4, 0, 4) };
        private readonly TextBox _firmante = new TextBox { Width = 170, MaxLength = 200, ReadOnly = true };
        private readonly TextBox _cargo = new TextBox { MaxLength = 200, ReadOnly = true, Text = KI68CertificadoBLL.CargoResponsable };
        private readonly TextBox _folio = new TextBox { Width = 150, MaxLength = 200 };
        private readonly TextBox _prefijo = new TextBox { ReadOnly = true };
        private readonly TextBox _email = new TextBox { Width = 230, MaxLength = 254 };
        private string _emailPredeterminado = "";
        private readonly TextBox _mencion = new TextBox { Width = 230, MaxLength = 200, Multiline = true, ScrollBars = ScrollBars.Vertical };
        private readonly TextBox _obs = new TextBox { Width = 400, MaxLength = 500, Multiline = true, Height = 48 };
        private readonly ComboBox _modalidad = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 170 };
        private readonly Button _buscar = new Button { AutoSize = true, Tag = "Cun09.Buscar" };
        private readonly Button _generar = new Button { AutoSize = true, Tag = "Cun09.Generar" };
        private readonly Button _retener = new Button { AutoSize = true, Tag = "Cun09.Retener" };
        private readonly Button _guardar = new Button { AutoSize = true, Tag = "Cun09.Guardar" };
        private AlumnoCertificacion_83KI _actual;
        private bool _cargando;

        public FrmGenerarCertificado_83KI(KI68CertificadoBLL bll) : this(bll, ServiceFactory_83KI.GetGestorIdioma()) { }
        public FrmGenerarCertificado_83KI(KI68CertificadoBLL bll, IGestorIdioma_83KI idioma)
        {
            _bll = bll ?? throw new ArgumentNullException(nameof(bll)); _idioma = idioma ?? throw new ArgumentNullException(nameof(idioma));
            var usuarioActivo = SessionManager_83KI.Instancia.UsuarioActivo;
            _firmante.Text = usuarioActivo == null ? "" : (usuarioActivo.Nombre + " " + usuarioActivo.Apellido).Trim();
            _emailPredeterminado = usuarioActivo == null ? "" : usuarioActivo.Email;
            _email.Text = _emailPredeterminado;
            Width = 1100; Height = 850; MinimumSize = new Size(850, 700); StartPosition = FormStartPosition.CenterParent;
            var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 8, Padding = new Padding(12) };
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42)); layout.RowStyles.Add(new RowStyle(SizeType.Percent, 38));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 35)); layout.RowStyles.Add(new RowStyle(SizeType.Percent, 27));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 172)); layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 24));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 58)); layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
            var top = new FlowLayoutPanel { Dock = DockStyle.Fill }; top.Controls.Add(Label("Cun09.Comision")); top.Controls.Add(_comisiones); top.Controls.Add(_buscar);
            layout.Controls.Add(top, 0, 0); layout.Controls.Add(_alumnos, 0, 1); layout.Controls.Add(_detalle, 0, 2); layout.Controls.Add(_cuotas, 0, 3);
            _campos.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 17)); _campos.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
            _campos.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 17)); _campos.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
            for (int row = 0; row < 4; row++) _campos.RowStyles.Add(new RowStyle(SizeType.Percent, 25));
            AddField("Cun09.Firmante", _firmante, 0, 0); AddField("Cun09.Cargo", _cargo, 2, 0);
            AddField("Cun09.NumeroFolio", _folio, 0, 1); AddField("Cun09.Email", _email, 2, 1);
            AddField("Cun09.Modalidad", _modalidad, 0, 2); AddField("Cun09.Mencion", _mencion, 2, 2);
            AddField("Cun09.PrefijoFolio", _prefijo, 0, 3); _campos.SetColumnSpan(_prefijo, 3);
            layout.Controls.Add(_campos, 0, 4);
            layout.Controls.Add(Label("Cun09.Observaciones"), 0, 5); _obs.Dock = DockStyle.Fill; layout.Controls.Add(_obs, 0, 6);
            var actions = new FlowLayoutPanel { Dock = DockStyle.Fill }; actions.Controls.AddRange(new Control[] { _generar, _retener, _guardar }); layout.Controls.Add(actions, 0, 7);
            Controls.Add(layout);
            foreach (string key in new[] { "Comun.Dni", "Comun.Nombre", "Comun.Apellido", "Cun09.Evaluacion1", "Cun09.Evaluacion2", "Cun09.Promedio", "Cun09.Condicion" }) _alumnos.Columns.Add(key, key);
            foreach (string key in new[] { "Cun09.Numero", "Cun09.Balance", "Cun09.Vencimiento" }) _cuotas.Columns.Add(key, key);
            _modalidad.Items.AddRange(new object[] { "Solo digital", "Digital e impresa" }); _modalidad.SelectedIndex = 0;
            _buscar.Click += (s, e) => Ejecutar(Buscar);
            _alumnos.SelectionChanged += (s, e) => { if (!_cargando) Ejecutar(Seleccionar); };
            _comisiones.SelectedIndexChanged += (s, e) => LimpiarBusqueda();
            _generar.Click += (s, e) => Ejecutar(Generar);
            _retener.Click += (s, e) => Ejecutar(Retener);
            _guardar.Click += (s, e) => Guardar();
            _idioma.Suscribir(this); ActualizarIdioma(null); Mostrar(null);
        }
        private static DataGridView Grid() { return new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false, AllowUserToDeleteRows = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, SelectionMode = DataGridViewSelectionMode.FullRowSelect, MultiSelect = false, RowHeadersVisible = false }; }
        private static Label Label(string key) { return new Label { Tag = key, AutoSize = true, Padding = new Padding(0, 6, 0, 0) }; }
        private void AddField(string key, Control field, int column, int row)
        {
            field.Dock = DockStyle.Fill;
            _campos.Controls.Add(Label(key), column, row);
            _campos.Controls.Add(field, column + 1, row);
        }
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            try
            {
                var commissions = _bll.ListarFinalizadas();
                if (commissions.Count == 0) { Mensaje("Cun09.SinComisiones"); Close(); return; }
                _comisiones.DisplayMember = "Codigo"; _comisiones.ValueMember = "IdComision"; _comisiones.DataSource = commissions;
                // Cargar comisiones no realiza una búsqueda de alumnos.
            }
            catch (Exception ex) { Error(ex); Close(); }
        }
        private void LimpiarBusqueda()
        {
            _cargando = true; try { _alumnos.Rows.Clear(); Mostrar(null); } finally { _cargando = false; }
        }
        private void Buscar()
        {
            LimpiarBusqueda();
            var c = _comisiones.SelectedItem as Comision_83KI;
            var rows = _bll.Buscar(c == null ? 0 : c.IdComision);
            _cargando = true;
            try
            {
                foreach (var a in rows) { int n = _alumnos.Rows.Add(a.DNI, a.Nombre, a.Apellido, a.Evaluacion1.ToString("0.00"), a.Evaluacion2.ToString("0.00"), a.Promedio.ToString("0.0000"), T(a.Aprobado ? "Cun09.Aprobado" : "Cun09.Desaprobado")); _alumnos.Rows[n].Tag = a.IdInscripcion; }
                _alumnos.ClearSelection();
            }
            finally { _cargando = false; }
            if (rows.Count == 0) Mensaje("Cun09.SinAlumnos");
        }
        private void Seleccionar()
        {
            Mostrar(null);
            if (_alumnos.SelectedRows.Count != 0) Mostrar(_bll.Seleccionar((int)_alumnos.SelectedRows[0].Tag));
        }
        private void Mostrar(AlumnoCertificacion_83KI a)
        {
            _actual = a; _cuotas.Rows.Clear(); _obs.Clear();
            _campos.Visible = a != null && a.Certificado == null && a.Aprobado && a.Balance == 0;
            _generar.Enabled = _campos.Visible; _retener.Enabled = a != null && a.Motivos != ""; _guardar.Enabled = a != null && a.Certificado != null;
            _obs.Enabled = a != null && a.Certificado == null;
            foreach (var box in new[] { _folio, _mencion, _prefijo }) box.Clear();
            _detalle.Clear();
            if (a == null) return;
            if (a.Certificado == null && a.Aprobado && a.Balance == 0) _prefijo.Text = KI68CertificadoBLL.PrefijoFolio(a);
            if (a.Certificado == null) _email.Text = string.IsNullOrWhiteSpace(a.Email) ? _emailPredeterminado : a.Email;
            if (a.Certificado != null) _detalle.Text = CertificadoPdfExporter_83KI.Contenido(a.Certificado);
            else _detalle.Text = a.Nombre + " " + a.Apellido + " | " + a.Curso + "\r\n" +
                T("Cun09.Evaluacion1") + ": " + a.Evaluacion1.ToString("0.00") + " | " + T("Cun09.Evaluacion2") + ": " + a.Evaluacion2.ToString("0.00") + "\r\n" +
                T("Cun09.Promedio") + ": " + a.Promedio.ToString("0.0000") +
                " | " + a.Hoy.ToString("dd/MM/yyyy") + "\r\n" + T("Cun09.Balance") + ": " + a.Balance.ToString("0.00") + "\r\n" +
                T("Cun09.Motivos") + ": " + (a.Motivos == "" ? "" : T("Cun09." + a.Motivos)) + (!a.Aprobado ? "\r\n" + T("Cun09.Profesor") + ": " + a.ObservacionesProfesor : "");
            foreach (var q in a.Cuotas) _cuotas.Rows.Add(q.Numero, q.Balance.ToString("0.00"), q.Vencimiento.ToString("dd/MM/yyyy"));
        }
        private void Generar()
        {
            if (_actual == null) return;
            var result = _bll.Generar(new GenerarCertificadoRequest_83KI { IdInscripcion = _actual.IdInscripcion, Firmante = _firmante.Text, Cargo = KI68CertificadoBLL.CargoResponsable,
                NumeroFolio = _folio.Text, Email = _email.Text, Modalidad = _modalidad.SelectedIndex == 0 ? "Solo digital" : "Digital e impresa", Mencion = _mencion.Text, Observaciones = _obs.Text });
            Mostrar(result.Alumno);
            Mensaje(result.Existente ? "Cun09.Existente" : "Cun09.Generado", result.Alumno.Certificado.Email);
            if (result.AuditoriaFallida) Mensaje("Cun09.AuditoriaFallida");
        }
        private void Retener()
        {
            if (_actual == null) return;
            var result = _bll.Retener(_actual.IdInscripcion, _obs.Text);
            Mostrar(result.Alumno);
            if (result.Retenido) { Mensaje("Cun09.Retenido"); Close(); }
            else Mensaje("Cun09.Reconfirmar");
        }
        private void Guardar()
        {
            if (_actual == null || _actual.Certificado == null) return;
            using (var dialog = new SaveFileDialog { Filter = "PDF (*.pdf)|*.pdf", DefaultExt = "pdf", AddExtension = true, FileName = CertificadoPdfExporter_83KI.NombreArchivo(_actual.Certificado) })
            {
                if (dialog.ShowDialog(this) != DialogResult.OK) { Mensaje("Cun09.Cancelado"); return; }
                try { new CertificadoPdfExporter_83KI().Exportar(_actual.Certificado, dialog.FileName); Mensaje("Cun09.Guardado"); }
                catch { Mensaje("Cun09.ErrorGuardar"); } // La instantánea persistida y la opción Guardar permanecen sin cambios.
            }
        }
        private void Ejecutar(Action action) { try { action(); } catch (Exception ex) { Error(ex); } }
        private void Error(Exception ex)
        {
            string key = (ex.Message ?? "").Split('\r', '\n')[0];
            Mensaje(key.StartsWith("Cun09.", StringComparison.Ordinal) || key == "Errores.SinPermisos" ? key : "Cun09.Error");
        }
        private string T(string key) { return _idioma.ObtenerTexto(key); }
        private void Mensaje(string key, string extra = "") { MessageBox.Show(this, T(key) + (extra == "" ? "" : "\r\n" + extra), T("Cun09.Titulo"), MessageBoxButtons.OK, MessageBoxIcon.Information); }
        public void ActualizarIdioma(IIdioma idioma)
        {
            Text = T("Cun09.Titulo"); Traducir(Controls);
            int mode = _modalidad.SelectedIndex;
            _modalidad.Items.Clear(); _modalidad.Items.AddRange(new object[] { T("Cun09.Digital"), T("Cun09.Impresa") });
            _modalidad.SelectedIndex = mode < 0 ? 0 : mode;
            foreach (DataGridView grid in new[] { _alumnos, _cuotas }) foreach (DataGridViewColumn col in grid.Columns) col.HeaderText = T(col.Name);
        }
        private void Traducir(Control.ControlCollection controls) { foreach (Control c in controls) { if (c.Tag is string) c.Text = T((string)c.Tag); Traducir(c.Controls); } }
        protected override void OnFormClosed(FormClosedEventArgs e) { _idioma.Desuscribir(this); base.OnFormClosed(e); }
    }
}

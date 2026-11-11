using BE.DTOs.Disponibilidades;
using Service.Entidades;
using Service.Interfaces;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace UI.Modulos.Maestros
{
    public sealed class FrmHistorialDisponibilidadProfesor_83KI : FormConAyuda_83KI, IObservadorIdioma
    {
        private readonly IDisponibilidadProfesorHistorialBLL_83KI _historial;
        private readonly IGestorIdioma_83KI _idioma;
        private readonly int _idProfesor;
        private readonly int? _idDisponibilidadInicial;
        private readonly Label _lblProfesor = new Label();
        private readonly Label _lblDisponibilidad = new Label();
        private readonly Label _lblDesde = new Label();
        private readonly Label _lblHasta = new Label();
        private readonly TextBox _txtProfesor = new TextBox();
        private readonly TextBox _txtDisponibilidad = new TextBox();
        private readonly CheckBox _chkDesde = new CheckBox();
        private readonly CheckBox _chkHasta = new CheckBox();
        private readonly DateTimePicker _dtpDesde = new DateTimePicker();
        private readonly DateTimePicker _dtpHasta = new DateTimePicker();
        private readonly DataGridView _grid = new DataGridView();
        private readonly Button _btnAplicar = new Button();
        private readonly Button _btnLimpiar = new Button();
        private readonly Button _btnActivar = new Button();
        private readonly Button _btnSalir = new Button();

        public FrmHistorialDisponibilidadProfesor_83KI(IDisponibilidadProfesorHistorialBLL_83KI historial, int idProfesor, int? idDisponibilidadProfesor)
        {
            _historial = historial ?? throw new ArgumentNullException(nameof(historial));
            if (idProfesor <= 0) throw new ArgumentOutOfRangeException(nameof(idProfesor));
            _idProfesor = idProfesor;
            _idDisponibilidadInicial = idDisponibilidadProfesor;
            _idioma = ServiceFactory_83KI.GetGestorIdioma();
            InicializarControles();
            _idioma.Suscribir(this);
        }

        private void InicializarControles()
        {
            Text = "";
            Width = 1050;
            Height = 620;
            MinimumSize = new Size(850, 500);
            StartPosition = FormStartPosition.CenterParent;
            Font = new Font("Segoe UI", 9F);

            _lblProfesor.SetBounds(16, 18, 125, 25);
            _txtProfesor.SetBounds(145, 15, 100, 25);
            _txtProfesor.ReadOnly = true;
            _txtProfesor.Text = _idProfesor.ToString();
            _lblDisponibilidad.SetBounds(270, 18, 150, 25);
            _txtDisponibilidad.SetBounds(425, 15, 120, 25);
            _txtDisponibilidad.Text = _idDisponibilidadInicial.HasValue ? _idDisponibilidadInicial.Value.ToString() : string.Empty;
            _lblDesde.SetBounds(16, 57, 125, 25);
            _chkDesde.SetBounds(145, 57, 20, 25);
            _dtpDesde.SetBounds(170, 54, 150, 25);
            _dtpDesde.Format = DateTimePickerFormat.Short;
            _lblHasta.SetBounds(345, 57, 75, 25);
            _chkHasta.SetBounds(425, 57, 20, 25);
            _dtpHasta.SetBounds(450, 54, 150, 25);
            _dtpHasta.Format = DateTimePickerFormat.Short;
            _dtpHasta.Value = DateTime.Today;
            _chkDesde.CheckedChanged += (s, e) => _dtpDesde.Enabled = _chkDesde.Checked;
            _chkHasta.CheckedChanged += (s, e) => _dtpHasta.Enabled = _chkHasta.Checked;
            _dtpDesde.Enabled = _dtpHasta.Enabled = false;

            _grid.SetBounds(16, 95, 1000, 410);
            _grid.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            _grid.ReadOnly = true;
            _grid.AllowUserToAddRows = false;
            _grid.AllowUserToDeleteRows = false;
            _grid.AutoGenerateColumns = false;
            _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _grid.MultiSelect = false;
            AddColumn("IdVersion", "IdVersion", 75);
            AddColumn("IdDisponibilidadProfesor", "IdDisponibilidadProfesor", 130);
            AddColumn("IdProfesor", "IdProfesor", 90);
            AddColumn("TipoOperacion", "TipoOperacion", 130);
            AddColumn("Usuario", "Usuario", 140);
            AddColumn("DiaSemana", "DiaSemana", 110);
            AddColumn("HoraInicio", "HoraInicio", 90);
            AddColumn("HoraFin", "HoraFin", 90);
            AddColumn("EstadoActivo", "EstadoActivo", 90);
            AddColumn("DVH", "DVH", 170);
            AddColumn("Act", "Act", 65);
            AddColumn("FechaRegistro", "FechaRegistro", 150);

            _btnAplicar.SetBounds(16, 520, 125, 34);
            _btnLimpiar.SetBounds(151, 520, 125, 34);
            _btnActivar.SetBounds(286, 520, 125, 34);
            _btnSalir.SetBounds(891, 520, 125, 34);
            _btnAplicar.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
            _btnLimpiar.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
            _btnActivar.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
            _btnSalir.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            _btnAplicar.Click += (s, e) => CargarHistorial();
            _btnLimpiar.Click += (s, e) => LimpiarFiltros();
            _btnActivar.Click += (s, e) => ActivarSeleccionada();
            _btnSalir.Click += (s, e) => Close();
            Controls.AddRange(new Control[] { _lblProfesor, _txtProfesor, _lblDisponibilidad, _txtDisponibilidad, _lblDesde, _chkDesde, _dtpDesde, _lblHasta, _chkHasta, _dtpHasta, _grid, _btnAplicar, _btnLimpiar, _btnActivar, _btnSalir });
        }

        private void AddColumn(string property, string header, int width)
        {
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = property, DataPropertyName = property, HeaderText = header, Width = width });
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            CargarHistorial();
        }

        private void CargarHistorial()
        {
            try
            {
                int? idDisponibilidad = null;
                if (!string.IsNullOrWhiteSpace(_txtDisponibilidad.Text))
                {
                    int parsed;
                    if (!int.TryParse(_txtDisponibilidad.Text.Trim(), out parsed) || parsed <= 0)
                    {
                        MessageBox.Show(this, T("InvalidAvailability"), T("Title"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    idDisponibilidad = parsed;
                }
                DateTime? desde = _chkDesde.Checked ? _dtpDesde.Value.Date : (DateTime?)null;
                DateTime? hasta = _chkHasta.Checked ? _dtpHasta.Value.Date : (DateTime?)null;
                if (desde.HasValue && hasta.HasValue && desde.Value > hasta.Value)
                {
                    MessageBox.Show(this, T("InvalidRange"), T("Title"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                _grid.DataSource = _historial.ObtenerHistorial(_idProfesor, idDisponibilidad, desde, hasta)
                    .Select(x => new HistorialVista(x)).ToList();
                ConfigureColumns();
            }
            catch (Exception ex)
            {
                IdiomaUiHelper_83KI.MostrarError(this, ex, "Comun.Error", MessageBoxIcon.Warning);
            }
        }

        private void ConfigureColumns()
        {
            SetHeader("IdVersion", T("Version"));
            SetHeader("IdDisponibilidadProfesor", T("AvailabilityId"));
            SetHeader("IdProfesor", T("Professor"));
            SetHeader("TipoOperacion", T("Operation"));
            SetHeader("Usuario", T("User"));
            SetHeader("DiaSemana", T("Day"));
            SetHeader("HoraInicio", T("Start"));
            SetHeader("HoraFin", T("End"));
            SetHeader("EstadoActivo", T("Active"));
            SetHeader("DVH", T("Integrity"));
            SetHeader("Act", T("Current"));
            SetHeader("FechaRegistro", T("Registered"));
        }

        private void SetHeader(string column, string text) { _grid.Columns[column].HeaderText = text; }

        private void LimpiarFiltros()
        {
            _txtDisponibilidad.Clear();
            _chkDesde.Checked = false;
            _chkHasta.Checked = false;
            CargarHistorial();
        }

        private void ActivarSeleccionada()
        {
            var row = _grid.CurrentRow == null ? null : _grid.CurrentRow.DataBoundItem as HistorialVista;
            if (row == null) return;
            if (!PermisosUi_83KI.Tiene(PermisoSistema_83KI.ModificarProfesor))
            {
                MessageBox.Show(this, T("NoPermission"), T("Title"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                _historial.ActivarVersion(row.IdVersion, _idProfesor);
                CargarHistorial();
            }
            catch (Exception ex)
            {
                IdiomaUiHelper_83KI.MostrarError(this, ex, "Comun.Error", MessageBoxIcon.Warning);
            }
        }

        public void ActualizarIdioma(IIdioma idioma)
        {
            Text = T("Title");
            _lblProfesor.Text = T("Professor");
            _lblDisponibilidad.Text = T("AvailabilityId");
            _lblDesde.Text = T("From");
            _lblHasta.Text = T("Through");
            _btnAplicar.Text = T("Apply");
            _btnLimpiar.Text = T("Clear");
            _btnActivar.Text = T("Activate");
            _btnSalir.Text = T("Exit");
            ConfigureColumns();
        }

        private string T(string key)
        {
            string translationKey;
            switch (key)
            {
                case "Title": translationKey = "FrmHistorialDisponibilidad.Titulo"; break;
                case "Professor": translationKey = "FrmHistorialDisponibilidad.Profesor"; break;
                case "Operation": translationKey = "FrmHistorialDisponibilidad.Operacion"; break;
                case "User": translationKey = "FrmHistorialDisponibilidad.Usuario"; break;
                case "AvailabilityId": translationKey = "FrmHistorialDisponibilidad.Disponibilidad"; break;
                case "From": translationKey = "FrmHistorialDisponibilidad.Desde"; break;
                case "Through": translationKey = "FrmHistorialDisponibilidad.Hasta"; break;
                case "Apply": translationKey = "Comun.Aplicar"; break;
                case "Clear": translationKey = "Comun.Limpiar"; break;
                case "Activate": translationKey = "FrmHistorialDisponibilidad.Activar"; break;
                case "Exit": translationKey = "FrmHistorialDisponibilidad.Salir"; break;
                case "Version": translationKey = "FrmHistorialDisponibilidad.Version"; break;
                case "Day": translationKey = "FrmGestionProfesores.DisponibilidadDia"; break;
                case "Start": translationKey = "FrmGestionProfesores.DisponibilidadHoraInicio"; break;
                case "End": translationKey = "FrmGestionProfesores.DisponibilidadHoraFin"; break;
                case "Active": translationKey = "FrmGestionProfesores.DisponibilidadActiva"; break;
                case "Integrity": translationKey = "FrmHistorialDisponibilidad.Integridad"; break;
                case "Current": translationKey = "FrmHistorialDisponibilidad.VersionActual"; break;
                case "Registered": translationKey = "FrmHistorialDisponibilidad.Registrada"; break;
                case "InvalidAvailability": translationKey = "FrmHistorialDisponibilidad.IdInvalido"; break;
                case "InvalidRange": translationKey = "FrmHistorialDisponibilidad.RangoInvalido"; break;
                case "NoPermission": translationKey = "Errores.SinPermisos"; break;
                default: return key;
            }
            return IdiomaUiHelper_83KI.Texto(translationKey);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _idioma.Desuscribir(this);
            base.OnFormClosed(e);
        }

        private sealed class HistorialVista
        {
            private readonly BE.DTOs.Disponibilidades.DisponibilidadProfesorHistorial_83KI _item;
            public HistorialVista(BE.DTOs.Disponibilidades.DisponibilidadProfesorHistorial_83KI item) { _item = item; }
            public long IdVersion { get { return _item.IdVersion; } }
            public int IdDisponibilidadProfesor { get { return _item.IdDisponibilidadProfesor; } }
            public int IdProfesor { get { return _item.IdProfesor; } }
            public string TipoOperacion { get { return IdiomaUiHelper_83KI.Texto("FrmHistorialDisponibilidad.Operacion." + _item.TipoOperacion); } }
            public string Usuario { get { return string.IsNullOrWhiteSpace(_item.Usuario) ? IdiomaUiHelper_83KI.Texto("FrmHistorialDisponibilidad.UsuarioDesconocido") : _item.Usuario; } }
            public string DiaSemana { get { return _item.DiaSemana; } }
            public string HoraInicio { get { return _item.HoraInicio.ToString(@"hh\:mm"); } }
            public string HoraFin { get { return _item.HoraFin.ToString(@"hh\:mm"); } }
            public bool EstadoActivo { get { return _item.EstadoActivo; } }
            public string DVH { get { return _item.DVH; } }
            public bool Act { get { return _item.Act; } }
            public DateTime FechaRegistro { get { return _item.FechaRegistro; } }
        }
    }
}

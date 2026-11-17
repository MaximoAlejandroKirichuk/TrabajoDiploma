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
            _lblDesde.SetBounds(16, 57, 125, 25);
            _chkDesde.SetBounds(145, 57, 20, 25);
            _dtpDesde.SetBounds(170, 54, 150, 25);
            _dtpDesde.Format = DateTimePickerFormat.Short;
            _lblHasta.SetBounds(345, 57, 75, 25);
            _chkHasta.SetBounds(425, 57, 20, 25);
            _dtpHasta.SetBounds(450, 54, 150, 25);
            _dtpHasta.Format = DateTimePickerFormat.Short;
            _chkDesde.CheckedChanged += (s, e) => _dtpDesde.Enabled = _chkDesde.Checked;
            _chkHasta.CheckedChanged += (s, e) => _dtpHasta.Enabled = _chkHasta.Checked;
            RestablecerFiltrosIniciales();

            _grid.SetBounds(16, 95, 1000, 410);
            _grid.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            _grid.ReadOnly = true;
            _grid.AllowUserToAddRows = false;
            _grid.AllowUserToDeleteRows = false;
            _grid.AutoGenerateColumns = false;
            _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _grid.MultiSelect = false;
            AgregarColumna("IdVersion", "IdVersion", 75);
            AgregarColumna("IdDisponibilidadProfesor", "IdDisponibilidadProfesor", 130);
            AgregarColumna("IdProfesor", "IdProfesor", 90);
            AgregarColumna("TipoOperacion", "TipoOperacion", 130);
            AgregarColumna("Usuario", "Usuario", 140);
            AgregarColumna("DiaSemana", "DiaSemana", 110);
            AgregarColumna("HoraInicio", "HoraInicio", 90);
            AgregarColumna("HoraFin", "HoraFin", 90);
            AgregarColumna("EstadoActivo", "EstadoActivo", 90);
            AgregarColumna("DVH", "DVH", 170);
            AgregarColumna("Act", "Act", 65);
            AgregarColumna("FechaRegistro", "FechaRegistro", 150);

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

        private void AgregarColumna(string propiedad, string cabecera, int ancho)
        {
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = propiedad, DataPropertyName = propiedad, HeaderText = cabecera, Width = ancho });
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
                    int idIngresado;
                    if (!int.TryParse(_txtDisponibilidad.Text.Trim(), out idIngresado) || idIngresado <= 0)
                    {
                        MessageBox.Show(this, T("FrmHistorialDisponibilidad.IdInvalido"), T("FrmHistorialDisponibilidad.Titulo"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    idDisponibilidad = idIngresado;
                }
                DateTime? desde = _chkDesde.Checked ? _dtpDesde.Value.Date : (DateTime?)null;
                DateTime? hasta = _chkHasta.Checked ? _dtpHasta.Value.Date : (DateTime?)null;
                if (desde.HasValue && hasta.HasValue && desde.Value > hasta.Value)
                {
                    MessageBox.Show(this, T("FrmHistorialDisponibilidad.RangoInvalido"), T("FrmHistorialDisponibilidad.Titulo"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                _grid.DataSource = _historial.ObtenerHistorial(_idProfesor, idDisponibilidad, desde, hasta)
                    .Select(x => new HistorialVista(x)).ToList();
                ConfigurarColumnas();
            }
            catch (Exception ex)
            {
                IdiomaUiHelper_83KI.MostrarError(this, ex, "Comun.Error", MessageBoxIcon.Warning);
            }
        }

        private void ConfigurarColumnas()
        {
            EstablecerCabecera("IdVersion", T("FrmHistorialDisponibilidad.Version"));
            EstablecerCabecera("IdDisponibilidadProfesor", T("FrmHistorialDisponibilidad.Disponibilidad"));
            EstablecerCabecera("IdProfesor", T("FrmHistorialDisponibilidad.Profesor"));
            EstablecerCabecera("TipoOperacion", T("FrmHistorialDisponibilidad.Operacion"));
            EstablecerCabecera("Usuario", T("FrmHistorialDisponibilidad.Usuario"));
            EstablecerCabecera("DiaSemana", T("FrmGestionProfesores.DisponibilidadDia"));
            EstablecerCabecera("HoraInicio", T("FrmGestionProfesores.DisponibilidadHoraInicio"));
            EstablecerCabecera("HoraFin", T("FrmGestionProfesores.DisponibilidadHoraFin"));
            EstablecerCabecera("EstadoActivo", T("FrmGestionProfesores.DisponibilidadActiva"));
            EstablecerCabecera("DVH", T("FrmHistorialDisponibilidad.Integridad"));
            EstablecerCabecera("Act", T("FrmHistorialDisponibilidad.VersionActual"));
            EstablecerCabecera("FechaRegistro", T("FrmHistorialDisponibilidad.Registrada"));
        }

        private void EstablecerCabecera(string columna, string texto) { _grid.Columns[columna].HeaderText = texto; }

        // Estado de arranque: la disponibilidad con la que se abrió la pantalla y sin filtro de fechas,
        // para que se vean todas sus versiones (también las de más de un mes).
        private void RestablecerFiltrosIniciales()
        {
            _txtDisponibilidad.Text = _idDisponibilidadInicial.HasValue ? _idDisponibilidadInicial.Value.ToString() : string.Empty;
            QuitarFiltroFechas();
        }

        // Las fechas quedan sugeridas en el último mes, pero no filtran hasta que el usuario las tilda.
        private void QuitarFiltroFechas()
        {
            _dtpDesde.Value = DateTime.Today.AddMonths(-1);
            _dtpHasta.Value = DateTime.Today;
            _chkDesde.Checked = false;
            _chkHasta.Checked = false;
            _dtpDesde.Enabled = false;
            _dtpHasta.Enabled = false;
        }

        // Limpiar quita todos los filtros: muestra el historial completo del profesor.
        private void LimpiarFiltros()
        {
            _txtDisponibilidad.Clear();
            QuitarFiltroFechas();
            CargarHistorial();
        }

        private void ActivarSeleccionada()
        {
            var fila = _grid.CurrentRow == null ? null : _grid.CurrentRow.DataBoundItem as HistorialVista;
            if (fila == null) return;
            if (!PermisosUi_83KI.Tiene(PermisoSistema_83KI.ModificarProfesor))
            {
                MessageBox.Show(this, T("Errores.SinPermisos"), T("FrmHistorialDisponibilidad.Titulo"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                _historial.ActivarVersion(fila.IdVersion, _idProfesor);
                CargarHistorial();
            }
            catch (Exception ex)
            {
                IdiomaUiHelper_83KI.MostrarError(this, ex, "Comun.Error", MessageBoxIcon.Warning);
            }
        }

        public void ActualizarIdioma(IIdioma idioma)
        {
            Text = T("FrmHistorialDisponibilidad.Titulo");
            _lblProfesor.Text = T("FrmHistorialDisponibilidad.Profesor");
            _lblDisponibilidad.Text = T("FrmHistorialDisponibilidad.Disponibilidad");
            _lblDesde.Text = T("FrmHistorialDisponibilidad.Desde");
            _lblHasta.Text = T("FrmHistorialDisponibilidad.Hasta");
            _btnAplicar.Text = T("Comun.Aplicar");
            _btnLimpiar.Text = T("Comun.Limpiar");
            _btnActivar.Text = T("FrmHistorialDisponibilidad.Activar");
            _btnSalir.Text = T("FrmHistorialDisponibilidad.Salir");
            ConfigurarColumnas();
        }

        private static string T(string clave) { return IdiomaUiHelper_83KI.Texto(clave); }

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

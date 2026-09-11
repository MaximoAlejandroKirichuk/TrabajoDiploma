using Service.DTOs;
using Service.Entidades;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using UI.Exportacion;

namespace UI
{
    public partial class FrmBitacoraEventos : Form, IObservadorIdioma
    {
        private const string OpcionTodos = "Todos";
        private const string OpcionCualquiera = "Cualquiera";
        private readonly IConsultaBitacoraEventos_83KI _consultaBitacoraEventos;
        private readonly IBitacoraEventosExporter_83KI _exporter;
        private readonly IGestorIdioma_83KI _gestorIdioma;
        private readonly IBitacoraManager_83KI _bitacoraManager;
        private readonly List<BitacoraEventoVista_83KI> _eventosVisibles = new List<BitacoraEventoVista_83KI>();
        private bool _actualizandoGrilla;
        private bool _cargandoCombos;

        public FrmBitacoraEventos(IConsultaBitacoraEventos_83KI consultaBitacoraEventos)
            : this(consultaBitacoraEventos, new BitacoraEventosPdfExporter_83KI(), null)
        {
        }

        public FrmBitacoraEventos(IConsultaBitacoraEventos_83KI consultaBitacoraEventos, IBitacoraEventosExporter_83KI exporter)
            : this(consultaBitacoraEventos, exporter, null)
        {
        }

        public FrmBitacoraEventos(IConsultaBitacoraEventos_83KI consultaBitacoraEventos, IBitacoraEventosExporter_83KI exporter, IBitacoraManager_83KI bitacoraManager)
        {
            InitializeComponent();
            _consultaBitacoraEventos = consultaBitacoraEventos;
            _exporter = exporter;
            _bitacoraManager = bitacoraManager;
            _gestorIdioma = Service.ServiceFactory_83KI.GetGestorIdioma();
            dgvEventos.CellFormatting += dgvEventos_CellFormatting;
            _gestorIdioma.Suscribir(this);
        }

        private void FrmBitacoraEventos_Load(object sender, EventArgs e)
        {
            AplicarPermisos();
            ConfigurarFechas();
            CargarCombos();
            RestaurarFiltrosIniciales();
            if (PuedeVerBitacora())
            {
                CargarEventos();
            }
        }

        private bool PuedeVerBitacora()
        {
            return PermisosUi_83KI.Tiene(PermisoSistema_83KI.VerBitacoraEventos)
                || PermisosUi_83KI.Tiene(PermisoSistema_83KI.ConsultarBitacoraEventos);
        }

        private void AplicarPermisos()
        {
            bool puedeVer = PuedeVerBitacora();
            bool puedeFiltrar = PermisosUi_83KI.Tiene(PermisoSistema_83KI.FiltrarBitacoraEventos);

            dgvEventos.Visible = puedeVer;
            btnAplicar.Visible = puedeFiltrar;
            btnLimpiar.Visible = PermisosUi_83KI.Tiene(PermisoSistema_83KI.LimpiarFiltrosBitacora);
            btnImprimir.Visible = PermisosUi_83KI.Tiene(PermisoSistema_83KI.ExportarBitacoraPdf);

            lblNombre.Visible = puedeFiltrar;
            txtNombre.Visible = puedeFiltrar;
            lblApellido.Visible = puedeFiltrar;
            txtApellido.Visible = puedeFiltrar;
            lblLogin.Visible = puedeFiltrar;
            txtLogin.Visible = puedeFiltrar;
            lblFechaInicio.Visible = puedeFiltrar;
            dtpFechaInicio.Visible = puedeFiltrar;
            lblFechaFin.Visible = puedeFiltrar;
            dtpFechaFin.Visible = puedeFiltrar;
            lblEvento.Visible = puedeFiltrar;
            cmbEvento.Visible = puedeFiltrar;
            lblModulo.Visible = puedeFiltrar;
            cmbModulo.Visible = puedeFiltrar;
            lblCriticidad.Visible = puedeFiltrar;
            cmbCriticidad.Visible = puedeFiltrar;
        }

        private void ConfigurarFechas()
        {
            dtpFechaInicio.MaxDate = DateTime.Today;
            dtpFechaFin.MaxDate = DateTime.Today;
        }

        private void CargarCombos()
        {
            _cargandoCombos = true;
            cmbModulo.Items.Clear();
            cmbModulo.Items.Add(new ComboItemIdioma_83KI(OpcionTodos, IdiomaUiHelper_83KI.Texto("Comun.Todos")));
            cmbModulo.Items.Add(new ComboItemIdioma_83KI(Modulo.Usuarios, IdiomaUiHelper_83KI.TraducirModulo(Modulo.Usuarios)));
            cmbModulo.Items.Add(new ComboItemIdioma_83KI(Modulo.Admin, IdiomaUiHelper_83KI.TraducirModulo(Modulo.Admin)));
            cmbModulo.SelectedIndex = 0;
            _cargandoCombos = false;
            CargarEventosPorModulo();
            cmbCriticidad.Items.Clear();
            cmbCriticidad.Items.Add(new ComboItemIdioma_83KI(OpcionCualquiera, IdiomaUiHelper_83KI.Texto("Comun.Cualquiera")));
            foreach (Criticidad criticidad in Enum.GetValues(typeof(Criticidad)))
            {
                cmbCriticidad.Items.Add(new ComboItemIdioma_83KI(criticidad, IdiomaUiHelper_83KI.TraducirCriticidad(criticidad)));
            }
            cmbCriticidad.SelectedIndex = 0;
        }

        private void RestaurarFiltrosIniciales()
        {
            txtNombre.Clear();
            txtApellido.Clear();
            txtLogin.Clear();
            cmbModulo.SelectedIndex = 0;
            cmbEvento.SelectedIndex = 0;
            cmbCriticidad.SelectedIndex = 0;
            dtpFechaInicio.Value = DateTime.Today.AddDays(-3);
            dtpFechaFin.Value = DateTime.Today;
        }

        private void CargarEventosPorModulo()
        {
            cmbEvento.Items.Clear();
            cmbEvento.Items.Add(new ComboItemIdioma_83KI(OpcionTodos, IdiomaUiHelper_83KI.Texto("Comun.Todos")));

            object moduloSeleccionado = ObtenerValorCombo(cmbModulo);
            if (moduloSeleccionado is Modulo)
            {
                Modulo modulo = (Modulo)moduloSeleccionado;
                foreach (EventoBitacoraOpcion_83KI evento in EventoBitacoraCatalogo_83KI.ObtenerPorModulo(modulo))
                {
                    cmbEvento.Items.Add(new ComboItemIdioma_83KI(evento.Nombre, IdiomaUiHelper_83KI.TraducirEventoBitacora(evento.Nombre)));
                }
            }

            cmbEvento.SelectedIndex = 0;
        }

        private void CargarEventos()
        {
            FiltroBitacoraEventos_83KI filtro = ObtenerFiltroDesdeUI();
            if (filtro.FechaDesde.Date > DateTime.Today || filtro.FechaHasta.Date > DateTime.Today.AddDays(1).AddTicks(-1))
            {
                IdiomaUiHelper_83KI.MostrarAdvertencia(this, "FrmBitacoraEventos.FechasFuturas", "Comun.Validacion");
                RestaurarFechasValidas();
                return;
            }

            if (filtro.FechaDesde.Date > filtro.FechaHasta.Date)
            {
                IdiomaUiHelper_83KI.MostrarAdvertencia(this, "FrmBitacoraEventos.FechaInicioMayor", "Comun.Validacion");
                return;
            }

            try
            {
                _eventosVisibles.Clear();
                _eventosVisibles.AddRange(_consultaBitacoraEventos.Consultar(filtro));
                ActualizarDataGridView();
            }
            catch (Exception ex)
            {
                IdiomaUiHelper_83KI.MostrarError(this, ex, "FrmBitacoraEventos.Titulo", MessageBoxIcon.Warning);
            }
        }

        private FiltroBitacoraEventos_83KI ObtenerFiltroDesdeUI()
        {
            DateTime desde = dtpFechaInicio.Value.Date;
            DateTime hasta = dtpFechaFin.Value.Date;

            return new FiltroBitacoraEventos_83KI
            {
                FechaDesde = desde,
                FechaHasta = hasta.AddDays(1).AddTicks(-1),
                Nombre = txtNombre.Text,
                Apellido = txtApellido.Text,
                Username = txtLogin.Text,
                Evento = ObtenerTextoCombo(cmbEvento),
                Modulo = ObtenerValorCombo(cmbModulo) is Modulo ? (Modulo?)ObtenerValorCombo(cmbModulo) : null,
                Criticidad = ObtenerValorCombo(cmbCriticidad) is Criticidad ? (Criticidad?)ObtenerValorCombo(cmbCriticidad) : null
            };
        }

        private string ObtenerTextoCombo(ComboBox combo)
        {
            object valor = ObtenerValorCombo(combo);
            if (valor == null ||
                string.Equals(valor.ToString(), OpcionTodos, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(valor.ToString(), OpcionCualquiera, StringComparison.OrdinalIgnoreCase))
            {
                return string.Empty;
            }
            return valor.ToString();
        }

        private object ObtenerValorCombo(ComboBox combo)
        {
            ComboItemIdioma_83KI item = combo.SelectedItem as ComboItemIdioma_83KI;
            return item == null ? combo.SelectedItem : item.Valor;
        }

        private void RestaurarFechasValidas()
        {
            if (dtpFechaInicio.Value.Date > DateTime.Today)
            {
                dtpFechaInicio.Value = DateTime.Today;
            }

            if (dtpFechaFin.Value.Date > DateTime.Today)
            {
                dtpFechaFin.Value = DateTime.Today;
            }
        }

        private void ActualizarDataGridView()
        {
            _actualizandoGrilla = true;
            dgvEventos.DataSource = null;
            dgvEventos.DataSource = _eventosVisibles.ToList();
            ConfigurarGrilla();
            dgvEventos.ClearSelection();
            dgvEventos.CurrentCell = null;
            _actualizandoGrilla = false;
        }
        #region Configrar grilla
        private void ConfigurarGrilla()
        {
            if (dgvEventos.Columns["Id"] != null)
            {
                dgvEventos.Columns["Id"].DisplayIndex = 0;
                dgvEventos.Columns["Id"].Width = 55;
            }

            ConfigurarColumna("Fecha", IdiomaUiHelper_83KI.Texto("FrmBitacoraEventos.ColumnaFecha"), 125);
            ConfigurarColumna("Username", IdiomaUiHelper_83KI.Texto("Comun.Username"), 110);
            ConfigurarColumna("Nombre", IdiomaUiHelper_83KI.Texto("Comun.Nombre"), 110);
            ConfigurarColumna("Apellido", IdiomaUiHelper_83KI.Texto("Comun.Apellido"), 110);
            ConfigurarColumna("Modulo", IdiomaUiHelper_83KI.Texto("FrmBitacoraEventos.Modulo"), 95);
            ConfigurarColumna("Criticidad", IdiomaUiHelper_83KI.Texto("FrmBitacoraEventos.Criticidad"), 95);
            ConfigurarColumna("Evento", IdiomaUiHelper_83KI.Texto("FrmBitacoraEventos.Evento"), 290);
        }

        private void ConfigurarColumna(string nombre, string titulo, int ancho)
        {
            if (dgvEventos.Columns[nombre] == null)
            {
                return;
            }

            dgvEventos.Columns[nombre].HeaderText = titulo;
            dgvEventos.Columns[nombre].Width = ancho;
        }

        #endregion

        private void dgvEventos_SelectionChanged(object sender, EventArgs e)
        {
            if (_actualizandoGrilla)
            {
                return;
            }

            BitacoraEventoVista_83KI evento = ObtenerEventoSeleccionado();
            if (evento == null)
            {
                return;
            }

            txtNombre.Text = evento.Nombre;
            txtApellido.Text = evento.Apellido;
            txtLogin.Text = evento.Username;
        }

        private BitacoraEventoVista_83KI ObtenerEventoSeleccionado()
        {
            return dgvEventos.CurrentRow == null ? null : dgvEventos.CurrentRow.DataBoundItem as BitacoraEventoVista_83KI;
        }

        private void btnAplicar_Click(object sender, EventArgs e)
        {
            CargarEventos();
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            RestaurarFiltrosIniciales();
            CargarEventos();
        }

        private void btnImprimir_Click(object sender, EventArgs e)
        {
            if (_eventosVisibles.Count == 0)
            {
                IdiomaUiHelper_83KI.MostrarAdvertencia(this, "FrmBitacoraEventos.SinEventosExportar", "Comun.Validacion");
                return;
            }

            string mensaje;
            if (!_exporter.PuedeExportar(out mensaje))
            {
                MessageBox.Show(this, mensaje, IdiomaUiHelper_83KI.Texto("FrmBitacoraEventos.Titulo"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SaveFileDialog dialogo = new SaveFileDialog())
            {
                dialogo.Title = "Exportar bitacora de eventos";
                dialogo.Filter = "Archivo PDF (*.pdf)|*.pdf";
                dialogo.FileName = "BitacoraEventos_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".pdf";
                dialogo.DefaultExt = "pdf";
                dialogo.AddExtension = true;
                dialogo.OverwritePrompt = true;

                if (dialogo.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                try
                {
                    _exporter.Exportar(_eventosVisibles, dialogo.FileName);
                    RegistrarAuditoriaExportacionPdf();
                    IdiomaUiHelper_83KI.MostrarInformacion(this, "FrmBitacoraEventos.PdfExportado", "Comun.Informacion");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, IdiomaUiHelper_83KI.Texto("FrmBitacoraEventos.ErrorExportarPdf", IdiomaUiHelper_83KI.TraducirExcepcion(ex)), IdiomaUiHelper_83KI.Texto("Comun.Error"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void cmbModulo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_cargandoCombos)
            {
                return;
            }

            CargarEventosPorModulo();
        }

        private void dgvEventos_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.Value == null || e.ColumnIndex < 0)
            {
                return;
            }

            string nombreColumna = dgvEventos.Columns[e.ColumnIndex].Name;
            if (nombreColumna == "Modulo" && e.Value is Modulo)
            {
                e.Value = IdiomaUiHelper_83KI.TraducirModulo((Modulo)e.Value);
                e.FormattingApplied = true;
            }
            else if (nombreColumna == "Criticidad" && e.Value is Criticidad)
            {
                e.Value = IdiomaUiHelper_83KI.TraducirCriticidad((Criticidad)e.Value);
                e.FormattingApplied = true;
            }
        }

        public void ActualizarIdioma(IIdioma idioma)
        {
            Text = IdiomaUiHelper_83KI.Texto("FrmBitacoraEventos.Titulo");
            lblNombre.Text = IdiomaUiHelper_83KI.Texto("Comun.Nombre");
            lblApellido.Text = IdiomaUiHelper_83KI.Texto("Comun.Apellido");
            lblLogin.Text = IdiomaUiHelper_83KI.Texto("Comun.Username");
            lblFechaInicio.Text = IdiomaUiHelper_83KI.Texto("FrmBitacoraEventos.FechaInicio");
            lblFechaFin.Text = IdiomaUiHelper_83KI.Texto("FrmBitacoraEventos.FechaFin");
            lblEvento.Text = IdiomaUiHelper_83KI.Texto("FrmBitacoraEventos.Evento");
            lblModulo.Text = IdiomaUiHelper_83KI.Texto("FrmBitacoraEventos.Modulo");
            lblCriticidad.Text = IdiomaUiHelper_83KI.Texto("FrmBitacoraEventos.Criticidad");
            btnLimpiar.Text = IdiomaUiHelper_83KI.Texto("Comun.Limpiar");
            btnAplicar.Text = IdiomaUiHelper_83KI.Texto("Comun.Aplicar");
            btnImprimir.Text = IdiomaUiHelper_83KI.Texto("FrmBitacoraEventos.ExportarPdf");
            btnCancelar.Text = IdiomaUiHelper_83KI.Texto("Comun.Cancelar");
            RefrescarCombosTraducidos();
            ConfigurarGrilla();
            dgvEventos.Refresh();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _gestorIdioma.Desuscribir(this);
            base.OnFormClosed(e);
        }

        private void RegistrarAuditoriaExportacionPdf()
        {
            if (_bitacoraManager == null)
            {
                return;
            }

            try
            {
                var usuarioActivo = Service.SessionManager_83KI.Instancia.UsuarioActivo;
                string username = usuarioActivo != null ? usuarioActivo.UserName : "Sistema";
                _bitacoraManager.RegistrarEvento(
                    BitacoraEvento_83KI.CrearNuevo(
                        $"Bitácora exportada a PDF: {_eventosVisibles.Count} eventos",
                        Criticidad.Bajo,
                        Modulo.Admin,
                        username
                    )
                );
            }
            catch
            {
                // La auditoría no debe interrumpir la exportación
            }
        }

        private void RefrescarCombosTraducidos()
        {
            if (cmbModulo.Items.Count == 0)
            {
                return;
            }

            object modulo = ObtenerValorCombo(cmbModulo);
            object evento = ObtenerValorCombo(cmbEvento);
            object criticidad = ObtenerValorCombo(cmbCriticidad);

            CargarCombos();

            SeleccionarValor(cmbModulo, modulo);
            CargarEventosPorModulo();
            SeleccionarValor(cmbEvento, evento);
            SeleccionarValor(cmbCriticidad, criticidad);
        }

        private void SeleccionarValor(ComboBox combo, object valor)
        {
            for (int i = 0; i < combo.Items.Count; i++)
            {
                object valorItem = combo.Items[i] is ComboItemIdioma_83KI
                    ? ((ComboItemIdioma_83KI)combo.Items[i]).Valor
                    : combo.Items[i];

                if (object.Equals(valorItem, valor))
                {
                    combo.SelectedIndex = i;
                    return;
                }
            }
        }
    }
}

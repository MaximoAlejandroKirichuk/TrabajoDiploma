using BE.Entidades;
using BLL;
using DAL;
using Service;
using Service.DTOs;
using Service.Entidades;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace UI.Modulos.PlanificacionAcademica
{
    public partial class FrmGestionComisiones_83KI : Form, IObservadorIdioma
    {
        private readonly IGestorComision_83KI _gestor;
        private readonly IGestorIdioma_83KI _gestorIdioma;
        private readonly IComisionSerializacionBLL_83KI _serializacion;
        private List<ComisionXml_83KI> _comisionesDeserializadas;
        private bool _mostrandoDeserializadas;
        private DataGridView dgvComisiones;
        private ComboBox cmbCursos;
        private ComboBox cmbProfesores;
        private ComboBox cmbEstado;
        private TextBox txtCodigo;
        private DateTimePicker dtpFechaInicio;
        private DateTimePicker dtpFechaFin;
        private CheckBox chkFechaInicio;
        private CheckBox chkFechaFin;
        private Button btnBuscar;
        private Button btnLimpiar;
        private Button btnModificar;
        private Button btnEliminar;
        private Button btnRegistrarPreapertura;
        private Button btnSerializar;
        private Button btnDeserializar;
        private Button btnGuardarDeserializadas;
        private Label lblMensaje;
        private Label lblCurso;
        private Label lblProfesor;
        private Label lblEstado;
        private Label lblCodigo;

        public FrmGestionComisiones_83KI(IGestorComision_83KI gestor)
            : this(gestor, new KI68ComisionBLL(new ComisionDAL_83KI()))
        {
        }

        public FrmGestionComisiones_83KI(IGestorComision_83KI gestor, IComisionSerializacionBLL_83KI serializacion)
        {
            _gestor = gestor;
            _serializacion = serializacion;
            _gestorIdioma = ServiceFactory_83KI.GetGestorIdioma();
            InitializeComponent();
            InicializarControles();
            _gestorIdioma.Suscribir(this);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            CargarCombos();
            Buscar();
            AplicarPermisos();
        }

        private void InicializarControles()
        {
            Width = 1180;
            Height = 650;
            StartPosition = FormStartPosition.CenterParent;
            Font = new Font("Segoe UI", 10F);

            var filtros = new TableLayoutPanel { Left = 12, Top = 12, Width = 880, Height = 110, ColumnCount = 6, RowCount = 3 };
            for (int i = 0; i < 6; i++) filtros.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.6F));
            for (int i = 0; i < 3; i++) filtros.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));
            cmbCursos = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Dock = DockStyle.Fill };
            cmbProfesores = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Dock = DockStyle.Fill };
            cmbEstado = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Dock = DockStyle.Fill };
            txtCodigo = new TextBox { Dock = DockStyle.Fill };
            chkFechaInicio = new CheckBox { Dock = DockStyle.Fill };
            chkFechaFin = new CheckBox { Dock = DockStyle.Fill };
            dtpFechaInicio = new DateTimePicker { Format = DateTimePickerFormat.Short, Dock = DockStyle.Fill };
            dtpFechaFin = new DateTimePicker { Format = DateTimePickerFormat.Short, Dock = DockStyle.Fill };
            lblCurso = new Label { AutoSize = true };
            lblProfesor = new Label { AutoSize = true };
            lblEstado = new Label { AutoSize = true };
            lblCodigo = new Label { AutoSize = true };
            filtros.Controls.Add(lblCurso, 0, 0);
            filtros.Controls.Add(lblProfesor, 1, 0);
            filtros.Controls.Add(lblEstado, 2, 0);
            filtros.Controls.Add(lblCodigo, 3, 0);
            filtros.Controls.Add(chkFechaInicio, 4, 0);
            filtros.Controls.Add(chkFechaFin, 5, 0);
            filtros.Controls.Add(cmbCursos, 0, 1);
            filtros.Controls.Add(cmbProfesores, 1, 1);
            filtros.Controls.Add(cmbEstado, 2, 1);
            filtros.Controls.Add(txtCodigo, 3, 1);
            filtros.Controls.Add(dtpFechaInicio, 4, 1);
            filtros.Controls.Add(dtpFechaFin, 5, 1);
            btnBuscar = new Button { Width = 120 };
            btnBuscar.Click += (s, e) => Buscar();
            btnLimpiar = new Button { Width = 120 };
            btnLimpiar.Click += (s, e) => LimpiarFiltros();
            filtros.Controls.Add(btnBuscar, 4, 2);
            filtros.Controls.Add(btnLimpiar, 5, 2);
            Controls.Add(filtros);

            dgvComisiones = new DataGridView { Left = 12, Top = 135, Width = 880, Height = 455, ReadOnly = true, AutoGenerateColumns = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect, MultiSelect = true };
            dgvComisiones.SelectionChanged += (s, e) => ActualizarAcciones();
            dgvComisiones.CellFormatting += dgvComisiones_CellFormatting;
            Controls.Add(dgvComisiones);

            int x = 920;
            btnModificar = new Button { Left = x, Top = 140, Width = 210, Height = 36 };
            btnModificar.Click += btnModificar_Click;
            Controls.Add(btnModificar);
            btnEliminar = new Button { Left = x, Top = 190, Width = 210, Height = 36 };
            btnEliminar.Click += btnEliminar_Click;
            Controls.Add(btnEliminar);
            btnRegistrarPreapertura = new Button { Left = x, Top = 240, Width = 210, Height = 48 };
            btnRegistrarPreapertura.Click += btnRegistrarPreapertura_Click;
            Controls.Add(btnRegistrarPreapertura);
            btnSerializar = new Button { Left = x, Top = 300, Width = 210, Height = 36 };
            btnSerializar.Click += btnSerializar_Click;
            Controls.Add(btnSerializar);
            btnDeserializar = new Button { Left = x, Top = 346, Width = 210, Height = 36 };
            btnDeserializar.Click += btnDeserializar_Click;
            Controls.Add(btnDeserializar);
            btnGuardarDeserializadas = new Button { Left = x, Top = 392, Width = 210, Height = 48 };
            btnGuardarDeserializadas.Click += btnGuardarDeserializadas_Click;
            Controls.Add(btnGuardarDeserializadas);
            lblMensaje = new Label { Left = x, Top = 450, Width = 210, Height = 140, ForeColor = Color.DarkBlue };
            Controls.Add(lblMensaje);
            ActualizarIdioma(null);
        }

        private void CargarCombos()
        {
            cmbCursos.DataSource = new object[] { new Curso_83KIItem(0, IdiomaUiHelper_83KI.Texto("Comun.Todos")) }.Concat(_gestor.ObtenerCursosActivosParaPreapertura().Select(c => new Curso_83KIItem(c.IdCurso, c.Nombre))).ToList();
            cmbCursos.DisplayMember = "Nombre";
            cmbCursos.ValueMember = "Id";
            cmbProfesores.DataSource = new[] { new Curso_83KIItem(0, IdiomaUiHelper_83KI.Texto("Comun.Todos")) }.Concat(_gestor.ObtenerProfesoresActivosParaFiltro().Select(p => new Curso_83KIItem(p.IdProfesor, p.NombreCompleto))).ToList();
            cmbProfesores.DisplayMember = "Nombre";
            cmbProfesores.ValueMember = "Id";
            CargarEstados();
        }

        private void CargarEstados()
        {
            cmbEstado.DataSource = new[]
            {
                new EstadoComisionItem(string.Empty, IdiomaUiHelper_83KI.Texto("Comun.Todos")),
                new EstadoComisionItem(Comision_83KI.EstadoPreapertura, IdiomaUiHelper_83KI.Texto("FrmGestionComisiones.EstadoPreapertura")),
                new EstadoComisionItem(Comision_83KI.EstadoEliminada, IdiomaUiHelper_83KI.Texto("FrmGestionComisiones.EstadoEliminada"))
            };
        }

        private void Buscar()
        {
            try
            {
                var filtro = new FiltroComision_83KI
                {
                    IdCurso = ValorCombo(cmbCursos),
                    IdProfesor = ValorCombo(cmbProfesores),
                    Estado = EstadoSeleccionado(),
                    Codigo = txtCodigo.Text,
                    FechaInicioDesde = chkFechaInicio.Checked ? (DateTime?)dtpFechaInicio.Value.Date : null,
                    FechaFinHasta = chkFechaFin.Checked ? (DateTime?)dtpFechaFin.Value.Date : null
                };
                var comisiones = _gestor.ListarComisiones(filtro).ToList();
                _mostrandoDeserializadas = false;
                _comisionesDeserializadas = null;
                dgvComisiones.DataSource = comisiones;
                ConfigurarGrilla();
                lblMensaje.Text = string.Empty;
            }
            catch (Exception ex)
            {
                lblMensaje.Text = IdiomaUiHelper_83KI.TraducirExcepcion(ex);
            }
            ActualizarAcciones();
        }

        private int? ValorCombo(ComboBox combo)
        {
            var item = combo.SelectedItem as Curso_83KIItem;
            return item != null && item.Id > 0 ? (int?)item.Id : null;
        }

        private string EstadoSeleccionado()
        {
            var item = cmbEstado.SelectedItem as EstadoComisionItem;
            return item != null && !string.IsNullOrWhiteSpace(item.Valor) ? item.Valor : null;
        }

        private void LimpiarFiltros()
        {
            cmbCursos.SelectedIndex = 0;
            cmbProfesores.SelectedIndex = 0;
            cmbEstado.SelectedIndex = 0;
            txtCodigo.Clear();
            chkFechaInicio.Checked = false;
            chkFechaFin.Checked = false;
            Buscar();
        }

        private void ConfigurarGrilla()
        {
            if (dgvComisiones.Columns["IdComision"] != null) dgvComisiones.Columns["IdComision"].Visible = false;
            if (dgvComisiones.Columns["IdCurso"] != null) dgvComisiones.Columns["IdCurso"].Visible = false;
            if (dgvComisiones.Columns["IdProfesor"] != null) dgvComisiones.Columns["IdProfesor"].Visible = false;
            if (dgvComisiones.Columns["Eliminada"] != null) dgvComisiones.Columns["Eliminada"].Visible = false;
            if (dgvComisiones.Columns["Descripcion"] != null) dgvComisiones.Columns["Descripcion"].Visible = false;
            if (dgvComisiones.Columns["HoraInicioXml"] != null) dgvComisiones.Columns["HoraInicioXml"].Visible = false;
            if (dgvComisiones.Columns["HoraFinXml"] != null) dgvComisiones.Columns["HoraFinXml"].Visible = false;
            if (dgvComisiones.Columns["IdPlanDePago"] != null) dgvComisiones.Columns["IdPlanDePago"].Visible = false;
            if (dgvComisiones.Columns["RecargoPlanSnapshot"] != null) dgvComisiones.Columns["RecargoPlanSnapshot"].Visible = false;

            ConfigurarEncabezado("Codigo", "FrmGestionComisiones.ColCodigo");
            ConfigurarEncabezado("Curso", "FrmGestionComisiones.ColCurso");
            ConfigurarEncabezado("Profesor", "FrmGestionComisiones.ColProfesor");
            ConfigurarEncabezado("DiaSemana", "FrmGestionComisiones.ColDia");
            ConfigurarEncabezado("HoraInicio", "FrmGestionComisiones.ColHoraInicio");
            ConfigurarEncabezado("HoraFin", "FrmGestionComisiones.ColHoraFin");
            ConfigurarEncabezado("CupoMinimo", "FrmGestionComisiones.ColCupoMinimo");
            ConfigurarEncabezado("CupoMaximo", "FrmGestionComisiones.ColCupoMaximo");
            ConfigurarEncabezado("FechaLimitePago", "FrmGestionComisiones.ColFechaLimitePago");
            ConfigurarEncabezado("FechaInicio", "FrmGestionComisiones.ColFechaInicio");
            ConfigurarEncabezado("FechaFin", "FrmGestionComisiones.ColFechaFin");
            ConfigurarEncabezado("ArancelBase", "FrmGestionComisiones.ColArancelBase");
            ConfigurarEncabezado("Estado", "FrmGestionComisiones.ColEstado");
        }

        private void ConfigurarEncabezado(string columna, string clave)
        {
            if (dgvComisiones.Columns[columna] != null)
                dgvComisiones.Columns[columna].HeaderText = IdiomaUiHelper_83KI.Texto(clave);
        }

        private void dgvComisiones_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.Value == null || e.RowIndex < 0) return;
            string nombre = dgvComisiones.Columns[e.ColumnIndex].Name;
            if (nombre == "DiaSemana" && e.Value is DayOfWeek)
            {
                e.Value = IdiomaUiHelper_83KI.Texto("Dominio.DiaSemana." + (DayOfWeek)e.Value);
                e.FormattingApplied = true;
            }
            else if (nombre == "Estado")
            {
                string estado = e.Value.ToString();
                if (string.Equals(estado, Comision_83KI.EstadoPreapertura, StringComparison.OrdinalIgnoreCase))
                    e.Value = IdiomaUiHelper_83KI.Texto("FrmGestionComisiones.EstadoPreapertura");
                else if (string.Equals(estado, Comision_83KI.EstadoEliminada, StringComparison.OrdinalIgnoreCase))
                    e.Value = IdiomaUiHelper_83KI.Texto("FrmGestionComisiones.EstadoEliminada");
                e.FormattingApplied = true;
            }
        }

        private ComisionListado_83KI Seleccionada()
        {
            var fila = dgvComisiones.SelectedRows.Cast<DataGridViewRow>().OrderBy(f => f.Index).FirstOrDefault();
            return fila == null ? null : fila.DataBoundItem as ComisionListado_83KI;
        }

        private List<ComisionListado_83KI> ComisionesSeleccionadas()
        {
            return dgvComisiones.SelectedRows.Cast<DataGridViewRow>()
                .OrderBy(f => f.Index)
                .Select(f => f.DataBoundItem as ComisionListado_83KI)
                .Where(c => c != null)
                .ToList();
        }

        private List<ComisionXml_83KI> DeserializadasSeleccionadas()
        {
            return dgvComisiones.SelectedRows.Cast<DataGridViewRow>()
                .OrderBy(f => f.Index)
                .Select(f => f.DataBoundItem as ComisionXml_83KI)
                .Where(c => c != null)
                .ToList();
        }

        private void ActualizarAcciones()
        {
            var seleccion = Seleccionada();
            bool activa = !_mostrandoDeserializadas && seleccion != null && !seleccion.Eliminada;
            btnModificar.Enabled = activa;
            btnEliminar.Enabled = activa;
            btnSerializar.Enabled = !_mostrandoDeserializadas && dgvComisiones.SelectedRows.Count > 0;
            btnGuardarDeserializadas.Enabled = _mostrandoDeserializadas && dgvComisiones.SelectedRows.Count > 0;
        }

        // A03 - Serializar: el usuario selecciona comisiones, elige ubicacion y nombre, y se genera el XML.
        private void btnSerializar_Click(object sender, EventArgs e)
        {
            var seleccionadas = ComisionesSeleccionadas();
            if (seleccionadas.Count == 0)
            {
                IdiomaUiHelper_83KI.MostrarAdvertencia(this, "FrmGestionComisiones.SeleccioneParaSerializar", "Comun.Validacion");
                return;
            }

            using (var dialogo = new SaveFileDialog())
            {
                dialogo.Filter = IdiomaUiHelper_83KI.Texto("FrmGestionComisiones.FiltroXml");
                dialogo.DefaultExt = "xml";
                dialogo.AddExtension = true;
                dialogo.FileName = "Comisiones.xml";
                if (dialogo.ShowDialog(this) != DialogResult.OK) return;

                try
                {
                    _serializacion.Serializar(dialogo.FileName, seleccionadas);
                    IdiomaUiHelper_83KI.MostrarInformacion(this, "FrmGestionComisiones.SerializacionExitosa", "Comun.Informacion");
                }
                catch (Exception ex)
                {
                    IdiomaUiHelper_83KI.MostrarError(this, ex, "Comun.Error", MessageBoxIcon.Warning);
                }
            }
        }

        // A03 - Deserializar: el usuario elige un XML y las comisiones se muestran en la grilla.
        private void btnDeserializar_Click(object sender, EventArgs e)
        {
            using (var dialogo = new OpenFileDialog())
            {
                dialogo.Filter = IdiomaUiHelper_83KI.Texto("FrmGestionComisiones.FiltroXml");
                dialogo.CheckFileExists = true;
                dialogo.Multiselect = false;
                if (dialogo.ShowDialog(this) != DialogResult.OK) return;

                try
                {
                    _comisionesDeserializadas = _serializacion.Deserializar(dialogo.FileName);
                    MostrarDeserializadas();
                    lblMensaje.Text = IdiomaUiHelper_83KI.Texto("FrmGestionComisiones.DeserializacionExitosa", _comisionesDeserializadas.Count);
                }
                catch (Exception ex)
                {
                    IdiomaUiHelper_83KI.MostrarError(this, ex, "Comun.Error", MessageBoxIcon.Warning);
                }
            }
        }

        private void MostrarDeserializadas()
        {
            _mostrandoDeserializadas = true;
            dgvComisiones.DataSource = _comisionesDeserializadas.ToList();
            ConfigurarGrilla();
            dgvComisiones.ClearSelection();
            ActualizarAcciones();
        }

        // Las comisiones deserializadas que el usuario selecciona se registran como preapertura nueva.
        private void btnGuardarDeserializadas_Click(object sender, EventArgs e)
        {
            var seleccionadas = DeserializadasSeleccionadas();
            if (seleccionadas.Count == 0)
            {
                IdiomaUiHelper_83KI.MostrarAdvertencia(this, "FrmGestionComisiones.SeleccioneParaGuardar", "Comun.Validacion");
                return;
            }

            string confirmacion = IdiomaUiHelper_83KI.Texto("FrmGestionComisiones.ConfirmarGuardar", seleccionadas.Count);
            if (MessageBox.Show(this, confirmacion, IdiomaUiHelper_83KI.Texto("Comun.Confirmacion"), MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK) return;

            int registradas = 0;
            int conError = 0;
            var detalle = new List<string>();
            foreach (var comision in seleccionadas)
            {
                try
                {
                    string codigoNuevo = _gestor.RegistrarPreapertura(comision.IdCurso, comision.IdProfesor, comision.DiaSemana, comision.HoraInicio, comision.HoraFin,
                        comision.CupoMinimo, comision.CupoMaximo, comision.FechaLimitePago, comision.FechaInicio, comision.FechaFin,
                        comision.ArancelBase, comision.IdPlanDePago, comision.RecargoPlanSnapshot);
                    registradas++;
                    detalle.Add("• " + comision.Codigo + "  →  " + IdiomaUiHelper_83KI.Texto("FrmGestionComisiones.DetalleRegistrada", codigoNuevo));
                    _comisionesDeserializadas.Remove(comision);
                }
                catch (Exception ex)
                {
                    conError++;
                    detalle.Add("• " + comision.Codigo + "  →  " + IdiomaUiHelper_83KI.TraducirExcepcion(ex));
                }
            }

            string resumen = IdiomaUiHelper_83KI.Texto("FrmGestionComisiones.ResultadoGuardar", registradas, conError)
                + Environment.NewLine + Environment.NewLine + string.Join(Environment.NewLine + Environment.NewLine, detalle);
            MessageBox.Show(this, resumen, IdiomaUiHelper_83KI.Texto(conError == 0 ? "Comun.Informacion" : "Comun.Validacion"), MessageBoxButtons.OK, conError == 0 ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

            // Si ya no quedan comisiones del XML pendientes, se vuelve a la grilla de la base de datos.
            if (_comisionesDeserializadas.Count == 0)
                Buscar();
            else
                MostrarDeserializadas();
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            var seleccion = Seleccionada();
            if (seleccion == null) return;
            using (var dialogo = new FrmEditarComision_83KI(_gestor, _gestor.ObtenerComision(seleccion.IdComision)))
            {
                if (dialogo.ShowDialog(this) == DialogResult.OK) Buscar();
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            var seleccion = Seleccionada();
            if (seleccion == null) return;
            if (MessageBox.Show(this, IdiomaUiHelper_83KI.Texto("FrmGestionComisiones.ConfirmarEliminar"), IdiomaUiHelper_83KI.Texto("Comun.Validacion"), MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK) return;
            try
            {
                _gestor.EliminarComision(seleccion.IdComision);
                Buscar();
            }
            catch (Exception ex)
            {
                IdiomaUiHelper_83KI.MostrarError(this, ex, "Comun.Error", MessageBoxIcon.Warning);
            }
        }

        private void btnRegistrarPreapertura_Click(object sender, EventArgs e)
        {
            using (var formulario = new FrmRegistrarPreaperturaComision_83KI(_gestor))
            {
                if (formulario.ShowDialog(this) == DialogResult.OK) Buscar();
            }
        }

        private void AplicarPermisos()
        {
            btnModificar.Visible = PermisosUi_83KI.Tiene(PermisoSistema_83KI.ModificarComision);
            btnEliminar.Visible = PermisosUi_83KI.Tiene(PermisoSistema_83KI.EliminarComision);
            btnRegistrarPreapertura.Visible = PermisosUi_83KI.Tiene(PermisoSistema_83KI.RegistrarPreaperturaComision);
            btnSerializar.Visible = PermisosUi_83KI.Tiene(PermisoSistema_83KI.SerializarComisiones);
            btnDeserializar.Visible = PermisosUi_83KI.Tiene(PermisoSistema_83KI.DeserializarComisiones);
            btnGuardarDeserializadas.Visible = PermisosUi_83KI.Tiene(PermisoSistema_83KI.DeserializarComisiones)
                && PermisosUi_83KI.Tiene(PermisoSistema_83KI.RegistrarPreaperturaComision);
        }

        public void ActualizarIdioma(IIdioma idioma)
        {
            Text = IdiomaUiHelper_83KI.Texto("FrmGestionComisiones.Titulo");
            lblCurso.Text = IdiomaUiHelper_83KI.Texto("FrmGestionComisiones.Curso");
            lblProfesor.Text = IdiomaUiHelper_83KI.Texto("FrmGestionComisiones.Profesor");
            lblEstado.Text = IdiomaUiHelper_83KI.Texto("FrmGestionComisiones.Estado");
            lblCodigo.Text = IdiomaUiHelper_83KI.Texto("FrmGestionComisiones.Codigo");
            chkFechaInicio.Text = IdiomaUiHelper_83KI.Texto("FrmGestionComisiones.FechaInicioDesde");
            chkFechaFin.Text = IdiomaUiHelper_83KI.Texto("FrmGestionComisiones.FechaFinHasta");
            btnBuscar.Text = IdiomaUiHelper_83KI.Texto("Comun.Aplicar");
            btnLimpiar.Text = IdiomaUiHelper_83KI.Texto("Comun.Limpiar");
            btnModificar.Text = IdiomaUiHelper_83KI.Texto("FrmGestionComisiones.Modificar");
            btnEliminar.Text = IdiomaUiHelper_83KI.Texto("FrmGestionComisiones.Eliminar");
            btnRegistrarPreapertura.Text = IdiomaUiHelper_83KI.Texto("FrmGestionComisiones.RegistrarPreapertura");
            btnSerializar.Text = IdiomaUiHelper_83KI.Texto("FrmGestionComisiones.Serializar");
            btnDeserializar.Text = IdiomaUiHelper_83KI.Texto("FrmGestionComisiones.Deserializar");
            btnGuardarDeserializadas.Text = IdiomaUiHelper_83KI.Texto("FrmGestionComisiones.GuardarDeserializadas");
            CargarEstados();
            ConfigurarGrilla();
            dgvComisiones.Refresh();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _gestorIdioma.Desuscribir(this);
            base.OnFormClosed(e);
        }

        private sealed class Curso_83KIItem
        {
            public Curso_83KIItem(int id, string nombre) { Id = id; Nombre = nombre; }
            public int Id { get; private set; }
            public string Nombre { get; private set; }
        }

        private sealed class EstadoComisionItem
        {
            public EstadoComisionItem(string valor, string texto) { Valor = valor; Texto = texto; }
            public string Valor { get; private set; }
            public string Texto { get; private set; }
            public override string ToString() { return Texto; }
        }
    }
}

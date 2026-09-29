using BE.Entidades;
using BLL;
using DAL;
using Service;
using Service.DTOs;
using Service.Entidades;
using Service.Interfaces;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace UI.Modulos.PlanificacionAcademica
{
    public partial class FrmGestionComisiones_83KI : Form, IObservadorIdioma
    {
        private readonly IGestorComision_83KI _gestor;
        private readonly IGestorIdioma_83KI _gestorIdioma;
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
        private Label lblMensaje;
        private Label lblCurso;
        private Label lblProfesor;
        private Label lblEstado;
        private Label lblCodigo;

        public FrmGestionComisiones_83KI(IGestorComision_83KI gestor)
        {
            _gestor = gestor;
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

            dgvComisiones = new DataGridView { Left = 12, Top = 135, Width = 880, Height = 455, ReadOnly = true, AutoGenerateColumns = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect, MultiSelect = false };
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
            lblMensaje = new Label { Left = x, Top = 305, Width = 210, Height = 140, ForeColor = Color.DarkBlue };
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
                dgvComisiones.DataSource = _gestor.ListarComisiones(filtro).ToList();
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
            return dgvComisiones.SelectedRows.Count == 0 ? null : dgvComisiones.SelectedRows[0].DataBoundItem as ComisionListado_83KI;
        }

        private void ActualizarAcciones()
        {
            var seleccion = Seleccionada();
            bool activa = seleccion != null && !seleccion.Eliminada;
            btnModificar.Enabled = activa;
            btnEliminar.Enabled = activa;
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

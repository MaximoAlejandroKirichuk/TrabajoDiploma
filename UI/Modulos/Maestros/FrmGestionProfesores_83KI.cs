using Service;
using BE.Entidades;
using Service.Entidades;
using Service.Interfaces;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace UI.Modulos.Maestros
{
    public class FrmGestionProfesores_83KI : Form, IObservadorIdioma
    {
        private readonly IGestorProfesor_83KI _gestorProfesor;
        private readonly IGestorIdioma_83KI _gestorIdioma;
        private DataGridView dgvProfesores;
        private Button btnCrear;
        private Button btnModificar;
        private Button btnCambiarEstado;
        private GroupBox grpDisponibilidad;
        private DataGridView dgvDisponibilidades;
        private Label lblDisponibilidadEstado;
        private Label lblDia;
        private Label lblHoraInicio;
        private Label lblHoraFin;
        private ComboBox cmbDia;
        private DateTimePicker dtpHoraInicio;
        private DateTimePicker dtpHoraFin;
        private Button btnAgregarDisponibilidad;
        private Button btnModificarDisponibilidad;
        private Button btnCambiarEstadoDisponibilidad;

        public FrmGestionProfesores_83KI(IGestorProfesor_83KI gestorProfesor)
        {
            _gestorProfesor = gestorProfesor;
            _gestorIdioma = ServiceFactory_83KI.GetGestorIdioma();
            InicializarControles();
            _gestorIdioma.Suscribir(this);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            CargarProfesores();
            AplicarPermisos();
        }

        private void InicializarControles()
        {
            Width = 1120;
            Height = 640;
            StartPosition = FormStartPosition.CenterParent;
            Font = new Font("Segoe UI", 10F);

            dgvProfesores = new DataGridView { Left = 12, Top = 12, Width = 620, Height = 540, ReadOnly = true, AutoGenerateColumns = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect, MultiSelect = false };
            dgvProfesores.SelectionChanged += (s, e) => { ActualizarAccionEstado(); CargarDisponibilidadesProfesor(); };
            Controls.Add(dgvProfesores);

            int x = 660;
            btnCrear = new Button { Left = x, Top = 20, Width = 180, Height = 36 };
            btnCrear.Click += btnCrear_Click;
            Controls.Add(btnCrear);
            btnModificar = new Button { Left = x, Top = 70, Width = 180, Height = 36 };
            btnModificar.Click += btnModificar_Click;
            Controls.Add(btnModificar);
            btnCambiarEstado = new Button { Left = x, Top = 120, Width = 180, Height = 36 };
            btnCambiarEstado.Click += btnCambiarEstado_Click;
            Controls.Add(btnCambiarEstado);

            grpDisponibilidad = new GroupBox { Left = 660, Top = 175, Width = 420, Height = 377 };
            Controls.Add(grpDisponibilidad);

            lblDisponibilidadEstado = new Label { Left = 14, Top = 28, Width = 390, Height = 24, ForeColor = Color.DarkBlue };
            grpDisponibilidad.Controls.Add(lblDisponibilidadEstado);

            dgvDisponibilidades = new DataGridView { Left = 14, Top = 58, Width = 390, Height = 150, ReadOnly = true, AutoGenerateColumns = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect, MultiSelect = false };
            dgvDisponibilidades.SelectionChanged += (s, e) => { CargarDisponibilidadSeleccionada(); ActualizarEstadoDisponibilidad(); };
            grpDisponibilidad.Controls.Add(dgvDisponibilidades);

            lblDia = new Label { Left = 14, Top = 224, Width = 110, Height = 24 };
            grpDisponibilidad.Controls.Add(lblDia);
            cmbDia = new ComboBox { Left = 135, Top = 220, Width = 170, DropDownStyle = ComboBoxStyle.DropDownList };
            grpDisponibilidad.Controls.Add(cmbDia);

            lblHoraInicio = new Label { Left = 14, Top = 257, Width = 110, Height = 24 };
            grpDisponibilidad.Controls.Add(lblHoraInicio);
            dtpHoraInicio = new DateTimePicker { Left = 135, Top = 253, Width = 110, Format = DateTimePickerFormat.Time, ShowUpDown = true };
            grpDisponibilidad.Controls.Add(dtpHoraInicio);

            lblHoraFin = new Label { Left = 14, Top = 290, Width = 110, Height = 24 };
            grpDisponibilidad.Controls.Add(lblHoraFin);
            dtpHoraFin = new DateTimePicker { Left = 135, Top = 286, Width = 110, Format = DateTimePickerFormat.Time, ShowUpDown = true };
            grpDisponibilidad.Controls.Add(dtpHoraFin);

            btnAgregarDisponibilidad = new Button { Left = 14, Top = 327, Width = 115, Height = 32 };
            btnAgregarDisponibilidad.Click += btnAgregarDisponibilidad_Click;
            grpDisponibilidad.Controls.Add(btnAgregarDisponibilidad);
            btnModificarDisponibilidad = new Button { Left = 141, Top = 327, Width = 115, Height = 32 };
            btnModificarDisponibilidad.Click += btnModificarDisponibilidad_Click;
            grpDisponibilidad.Controls.Add(btnModificarDisponibilidad);
            btnCambiarEstadoDisponibilidad = new Button { Left = 268, Top = 327, Width = 136, Height = 32 };
            btnCambiarEstadoDisponibilidad.Click += btnCambiarEstadoDisponibilidad_Click;
            grpDisponibilidad.Controls.Add(btnCambiarEstadoDisponibilidad);

            CargarDiasDisponibilidad();
            dtpHoraInicio.Value = DateTime.Today.AddHours(8);
            dtpHoraFin.Value = DateTime.Today.AddHours(22);
            ActualizarIdioma(null);
        }

        private void CargarProfesores()
        {
            dgvProfesores.DataSource = null;
            dgvProfesores.DataSource = _gestorProfesor.ObtenerProfesores().ToList();
            ConfigurarGrilla();
            dgvProfesores.ClearSelection();
            ActualizarAccionEstado();
            CargarDisponibilidadesProfesor();
        }

        private void ConfigurarGrilla()
        {
            if (dgvProfesores.Columns["DVH"] != null) dgvProfesores.Columns["DVH"].Visible = false;
            if (dgvProfesores.Columns["IdProfesor"] != null) dgvProfesores.Columns["IdProfesor"].HeaderText = "Id";
            if (dgvProfesores.Columns["DNI"] != null) dgvProfesores.Columns["DNI"].HeaderText = IdiomaUiHelper_83KI.Texto("Comun.Dni");
            if (dgvProfesores.Columns["Nombre"] != null) dgvProfesores.Columns["Nombre"].HeaderText = IdiomaUiHelper_83KI.Texto("Comun.Nombre");
            if (dgvProfesores.Columns["Apellido"] != null) dgvProfesores.Columns["Apellido"].HeaderText = IdiomaUiHelper_83KI.Texto("Comun.Apellido");
            if (dgvProfesores.Columns["Email"] != null) dgvProfesores.Columns["Email"].HeaderText = IdiomaUiHelper_83KI.Texto("Comun.Email");
            if (dgvProfesores.Columns["EstadoActivo"] != null) dgvProfesores.Columns["EstadoActivo"].HeaderText = IdiomaUiHelper_83KI.Texto("FrmGestionProfesores.Activo");
        }

        private void ConfigurarGrillaDisponibilidades()
        {
            if (dgvDisponibilidades.Columns["Original"] != null) dgvDisponibilidades.Columns["Original"].Visible = false;
            if (dgvDisponibilidades.Columns["Id"] != null) dgvDisponibilidades.Columns["Id"].HeaderText = "Id";
            if (dgvDisponibilidades.Columns["Dia"] != null) dgvDisponibilidades.Columns["Dia"].HeaderText = IdiomaUiHelper_83KI.Texto("FrmGestionProfesores.DisponibilidadDia");
            if (dgvDisponibilidades.Columns["HoraInicio"] != null) dgvDisponibilidades.Columns["HoraInicio"].HeaderText = IdiomaUiHelper_83KI.Texto("FrmGestionProfesores.DisponibilidadHoraInicio");
            if (dgvDisponibilidades.Columns["HoraFin"] != null) dgvDisponibilidades.Columns["HoraFin"].HeaderText = IdiomaUiHelper_83KI.Texto("FrmGestionProfesores.DisponibilidadHoraFin");
            if (dgvDisponibilidades.Columns["Activa"] != null) dgvDisponibilidades.Columns["Activa"].HeaderText = IdiomaUiHelper_83KI.Texto("FrmGestionProfesores.DisponibilidadActiva");
        }

        private void ActualizarAccionEstado()
        {
            var profesor = ProfesorSeleccionado();
            if (profesor == null)
            {
                btnCambiarEstado.Text = IdiomaUiHelper_83KI.Texto("FrmGestionProfesores.GestionarEstado");
                return;
            }

            btnCambiarEstado.Text = profesor.EstadoActivo ? IdiomaUiHelper_83KI.Texto("FrmGestionProfesores.Desactivar") : IdiomaUiHelper_83KI.Texto("FrmGestionProfesores.Activar");
        }

        private void CargarDiasDisponibilidad()
        {
            DayOfWeek seleccionado = DiaSeleccionadoDisponibilidad;
            cmbDia.Items.Clear();
            foreach (DayOfWeek dia in Enum.GetValues(typeof(DayOfWeek)))
            {
                cmbDia.Items.Add(new ComboItemIdioma_83KI(dia, IdiomaUiHelper_83KI.Texto("Dominio.DiaSemana." + dia)));
            }

            cmbDia.SelectedIndex = (int)seleccionado;
        }

        private void CargarDisponibilidadesProfesor()
        {
            var profesor = ProfesorSeleccionado();
            dgvDisponibilidades.DataSource = null;

            if (profesor == null)
            {
                lblDisponibilidadEstado.Text = IdiomaUiHelper_83KI.Texto("FrmGestionProfesores.DisponibilidadSinProfesor");
                ActualizarEstadoDisponibilidad();
                return;
            }

            try
            {
                var disponibilidades = _gestorProfesor.ObtenerDisponibilidades(profesor.IdProfesor)
                    .Select(d => new DisponibilidadProfesorVista_83KI(d, TraducirDia(d.DiaSemana)))
                    .ToList();
                dgvDisponibilidades.DataSource = disponibilidades;
                ConfigurarGrillaDisponibilidades();
                lblDisponibilidadEstado.Text = disponibilidades.Count == 0 ? IdiomaUiHelper_83KI.Texto("FrmGestionProfesores.DisponibilidadVacia") : string.Empty;
            }
            catch (Exception ex)
            {
                lblDisponibilidadEstado.Text = IdiomaUiHelper_83KI.TraducirExcepcion(ex);
            }
            finally
            {
                ActualizarEstadoDisponibilidad();
            }
        }

        private void CargarDisponibilidadSeleccionada()
        {
            var disponibilidad = DisponibilidadSeleccionada();
            if (disponibilidad == null) return;
            cmbDia.SelectedIndex = (int)disponibilidad.DiaSemana;
            dtpHoraInicio.Value = DateTime.Today.Add(disponibilidad.HoraInicio);
            dtpHoraFin.Value = DateTime.Today.Add(disponibilidad.HoraFin);
        }

        private void ActualizarEstadoDisponibilidad()
        {
            bool profesorSeleccionado = ProfesorSeleccionado() != null;
            bool puedeModificar = PermisosUi_83KI.Tiene(PermisoSistema_83KI.ModificarProfesor);
            bool disponibilidadSeleccionada = DisponibilidadSeleccionada() != null;

            grpDisponibilidad.Enabled = profesorSeleccionado;
            btnAgregarDisponibilidad.Enabled = profesorSeleccionado && puedeModificar;
            btnModificarDisponibilidad.Enabled = profesorSeleccionado && puedeModificar && disponibilidadSeleccionada;
            btnCambiarEstadoDisponibilidad.Enabled = profesorSeleccionado && puedeModificar && disponibilidadSeleccionada;

            var disponibilidad = DisponibilidadSeleccionada();
            btnCambiarEstadoDisponibilidad.Text = disponibilidad == null || disponibilidad.EstadoActivo
                ? IdiomaUiHelper_83KI.Texto("FrmGestionProfesores.DisponibilidadDesactivar")
                : IdiomaUiHelper_83KI.Texto("FrmGestionProfesores.DisponibilidadActivar");
        }

        private void btnCrear_Click(object sender, EventArgs e)
        {
            using (var dialogo = new FrmCrearProfesor_83KI(_gestorProfesor))
            {
                if (dialogo.ShowDialog(this) == DialogResult.OK)
                {
                    CargarProfesores();
                    IdiomaUiHelper_83KI.MostrarInformacion(this, "FrmGestionProfesores.Guardado", "Comun.Informacion");
                }
            }
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            var profesor = ProfesorSeleccionado();
            if (profesor == null) { IdiomaUiHelper_83KI.MostrarAdvertencia(this, "FrmGestionProfesores.Seleccionar", "Comun.Validacion"); return; }

            using (var dialogo = new FrmModificarProfesor_83KI(_gestorProfesor, profesor))
            {
                if (dialogo.ShowDialog(this) == DialogResult.OK)
                {
                    CargarProfesores();
                    IdiomaUiHelper_83KI.MostrarInformacion(this, "FrmGestionProfesores.Guardado", "Comun.Informacion");
                }
            }
        }

        private void btnCambiarEstado_Click(object sender, EventArgs e)
        {
            try
            {
                var profesor = ProfesorSeleccionado();
                if (profesor == null) { IdiomaUiHelper_83KI.MostrarAdvertencia(this, "FrmGestionProfesores.Seleccionar", "Comun.Validacion"); return; }
                if (profesor.EstadoActivo) _gestorProfesor.DesactivarProfesor(profesor.IdProfesor); else _gestorProfesor.ActivarProfesor(profesor.IdProfesor);
                CargarProfesores();
            }
            catch (Exception ex)
            {
                IdiomaUiHelper_83KI.MostrarError(this, ex, "Comun.Error", MessageBoxIcon.Warning);
            }
        }

        private void btnAgregarDisponibilidad_Click(object sender, EventArgs e)
        {
            try
            {
                var profesor = ProfesorSeleccionado();
                if (profesor == null) { IdiomaUiHelper_83KI.MostrarAdvertencia(this, "FrmGestionProfesores.Seleccionar", "Comun.Validacion"); return; }
                if (!ValidarHorarioDisponibilidad()) return;
                _gestorProfesor.AgregarDisponibilidad(profesor.IdProfesor, DiaSeleccionadoDisponibilidad, HoraInicioDisponibilidad, HoraFinDisponibilidad);
                CargarDisponibilidadesProfesor();
                IdiomaUiHelper_83KI.MostrarInformacion(this, "FrmGestionProfesores.DisponibilidadGuardada", "Comun.Informacion");
            }
            catch (Exception ex)
            {
                IdiomaUiHelper_83KI.MostrarError(this, ex, "Comun.Validacion", MessageBoxIcon.Warning);
            }
        }

        private void btnModificarDisponibilidad_Click(object sender, EventArgs e)
        {
            try
            {
                var profesor = ProfesorSeleccionado();
                var disponibilidad = DisponibilidadSeleccionada();
                if (profesor == null) { IdiomaUiHelper_83KI.MostrarAdvertencia(this, "FrmGestionProfesores.Seleccionar", "Comun.Validacion"); return; }
                if (disponibilidad == null) { IdiomaUiHelper_83KI.MostrarAdvertencia(this, "FrmGestionProfesores.DisponibilidadSeleccionar", "Comun.Validacion"); return; }
                if (!ValidarHorarioDisponibilidad()) return;
                _gestorProfesor.ModificarDisponibilidad(disponibilidad.IdDisponibilidadProfesor, profesor.IdProfesor, DiaSeleccionadoDisponibilidad, HoraInicioDisponibilidad, HoraFinDisponibilidad);
                CargarDisponibilidadesProfesor();
                IdiomaUiHelper_83KI.MostrarInformacion(this, "FrmGestionProfesores.DisponibilidadGuardada", "Comun.Informacion");
            }
            catch (Exception ex)
            {
                IdiomaUiHelper_83KI.MostrarError(this, ex, "Comun.Validacion", MessageBoxIcon.Warning);
            }
        }

        private void btnCambiarEstadoDisponibilidad_Click(object sender, EventArgs e)
        {
            try
            {
                var profesor = ProfesorSeleccionado();
                var disponibilidad = DisponibilidadSeleccionada();
                if (profesor == null) { IdiomaUiHelper_83KI.MostrarAdvertencia(this, "FrmGestionProfesores.Seleccionar", "Comun.Validacion"); return; }
                if (disponibilidad == null) { IdiomaUiHelper_83KI.MostrarAdvertencia(this, "FrmGestionProfesores.DisponibilidadSeleccionar", "Comun.Validacion"); return; }
                if (disponibilidad.EstadoActivo) _gestorProfesor.DesactivarDisponibilidad(disponibilidad.IdDisponibilidadProfesor, profesor.IdProfesor); else _gestorProfesor.ActivarDisponibilidad(disponibilidad.IdDisponibilidadProfesor, profesor.IdProfesor);
                CargarDisponibilidadesProfesor();
            }
            catch (Exception ex)
            {
                IdiomaUiHelper_83KI.MostrarError(this, ex, "Comun.Validacion", MessageBoxIcon.Warning);
            }
        }

        private bool ValidarHorarioDisponibilidad()
        {
            if (HoraInicioDisponibilidad < HoraFinDisponibilidad) return true;
            IdiomaUiHelper_83KI.MostrarAdvertencia(this, "Errores.HorarioInvalido", "Comun.Validacion");
            return false;
        }

        private Profesor_83KI ProfesorSeleccionado()
        {
            return dgvProfesores.SelectedRows.Count == 0 ? null : dgvProfesores.SelectedRows[0].DataBoundItem as Profesor_83KI;
        }

        private DisponibilidadProfesor_83KI DisponibilidadSeleccionada()
        {
            if (dgvDisponibilidades.SelectedRows.Count == 0) return null;
            var vista = dgvDisponibilidades.SelectedRows[0].DataBoundItem as DisponibilidadProfesorVista_83KI;
            return vista == null ? null : vista.Original;
        }

        private DayOfWeek DiaSeleccionadoDisponibilidad
        {
            get
            {
                ComboItemIdioma_83KI item = cmbDia.SelectedItem as ComboItemIdioma_83KI;
                return item != null && item.Valor is DayOfWeek ? (DayOfWeek)item.Valor : DayOfWeek.Monday;
            }
        }

        private TimeSpan HoraInicioDisponibilidad { get { return dtpHoraInicio.Value.TimeOfDay; } }
        private TimeSpan HoraFinDisponibilidad { get { return dtpHoraFin.Value.TimeOfDay; } }

        private string TraducirDia(DayOfWeek dia)
        {
            return IdiomaUiHelper_83KI.Texto("Dominio.DiaSemana." + dia);
        }

        private void AplicarPermisos()
        {
            dgvProfesores.Visible = PermisosUi_83KI.TieneAlguno(PermisoSistema_83KI.VerProfesores, PermisoSistema_83KI.GestionProfesores);
            btnCrear.Visible = PermisosUi_83KI.Tiene(PermisoSistema_83KI.CrearProfesor);
            btnModificar.Visible = PermisosUi_83KI.Tiene(PermisoSistema_83KI.ModificarProfesor);
            btnCambiarEstado.Visible = PermisosUi_83KI.TieneAlguno(PermisoSistema_83KI.ActivarProfesor, PermisoSistema_83KI.DesactivarProfesor);
            grpDisponibilidad.Visible = dgvProfesores.Visible;
            ActualizarEstadoDisponibilidad();
        }

        public void ActualizarIdioma(IIdioma idioma)
        {
            Text = IdiomaUiHelper_83KI.Texto("FrmGestionProfesores.Titulo");
            btnCrear.Text = IdiomaUiHelper_83KI.Texto("FrmGestionProfesores.Crear");
            btnModificar.Text = IdiomaUiHelper_83KI.Texto("FrmGestionProfesores.Modificar");
            btnCambiarEstado.Text = IdiomaUiHelper_83KI.Texto("FrmGestionProfesores.GestionarEstado");
            grpDisponibilidad.Text = IdiomaUiHelper_83KI.Texto("FrmGestionProfesores.DisponibilidadTitulo");
            lblDia.Text = IdiomaUiHelper_83KI.Texto("FrmGestionProfesores.DisponibilidadDia");
            lblHoraInicio.Text = IdiomaUiHelper_83KI.Texto("FrmGestionProfesores.DisponibilidadHoraInicio");
            lblHoraFin.Text = IdiomaUiHelper_83KI.Texto("FrmGestionProfesores.DisponibilidadHoraFin");
            btnAgregarDisponibilidad.Text = IdiomaUiHelper_83KI.Texto("Comun.Agregar");
            btnModificarDisponibilidad.Text = IdiomaUiHelper_83KI.Texto("Comun.Guardar");
            CargarDiasDisponibilidad();
            ConfigurarGrilla();
            ConfigurarGrillaDisponibilidades();
            ActualizarAccionEstado();
            ActualizarEstadoDisponibilidad();
            if (ProfesorSeleccionado() == null) lblDisponibilidadEstado.Text = IdiomaUiHelper_83KI.Texto("FrmGestionProfesores.DisponibilidadSinProfesor");
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _gestorIdioma.Desuscribir(this);
            base.OnFormClosed(e);
        }

        private sealed class DisponibilidadProfesorVista_83KI
        {
            public DisponibilidadProfesorVista_83KI(DisponibilidadProfesor_83KI original, string dia)
            {
                Original = original;
                Dia = dia;
            }

            public DisponibilidadProfesor_83KI Original { get; private set; }
            public int Id { get { return Original.IdDisponibilidadProfesor; } }
            public string Dia { get; private set; }
            public string HoraInicio { get { return Original.HoraInicio.ToString(@"hh\:mm"); } }
            public string HoraFin { get { return Original.HoraFin.ToString(@"hh\:mm"); } }
            public bool Activa { get { return Original.EstadoActivo; } }
        }
    }
}

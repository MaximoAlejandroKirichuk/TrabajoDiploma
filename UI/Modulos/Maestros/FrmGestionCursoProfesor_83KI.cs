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
    public class FrmGestionCursoProfesor_83KI : Form, IObservadorIdioma
    {
        private readonly IGestorCursoProfesor_83KI _gestorCursoProfesor;
        private readonly IGestorIdioma_83KI _gestorIdioma;
        private DataGridView dgvRelaciones;
        private ComboBox cboCursos;
        private ComboBox cboProfesores;
        private Label lblCurso;
        private Label lblProfesor;
        private Button btnHabilitar;
        private Button btnDeshabilitar;
        private Button btnActualizar;

        public FrmGestionCursoProfesor_83KI(IGestorCursoProfesor_83KI gestorCursoProfesor)
        {
            _gestorCursoProfesor = gestorCursoProfesor;
            _gestorIdioma = ServiceFactory_83KI.GetGestorIdioma();
            InicializarControles();
            _gestorIdioma.Suscribir(this);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            CargarCombos();
            CargarRelaciones();
            AplicarPermisos();
        }

        private void InicializarControles()
        {
            Width = 940;
            Height = 540;
            StartPosition = FormStartPosition.CenterParent;
            Font = new Font("Segoe UI", 10F);

            dgvRelaciones = new DataGridView { Left = 12, Top = 12, Width = 600, Height = 445, ReadOnly = true, AutoGenerateColumns = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect, MultiSelect = false };
            dgvRelaciones.SelectionChanged += (s, e) => CargarSeleccion();
            Controls.Add(dgvRelaciones);

            int x = 635;
            lblCurso = new Label { Left = x, Top = 20, Width = 240 };
            Controls.Add(lblCurso);
            cboCursos = new ComboBox { Left = x, Top = 45, Width = 260, DropDownStyle = ComboBoxStyle.DropDownList };
            Controls.Add(cboCursos);

            lblProfesor = new Label { Left = x, Top = 90, Width = 240 };
            Controls.Add(lblProfesor);
            cboProfesores = new ComboBox { Left = x, Top = 115, Width = 260, DropDownStyle = ComboBoxStyle.DropDownList };
            Controls.Add(cboProfesores);

            btnHabilitar = new Button { Left = x, Top = 175, Width = 125, Height = 36 };
            btnHabilitar.Click += btnHabilitar_Click;
            Controls.Add(btnHabilitar);

            btnDeshabilitar = new Button { Left = x + 135, Top = 175, Width = 125, Height = 36 };
            btnDeshabilitar.Click += btnDeshabilitar_Click;
            Controls.Add(btnDeshabilitar);

            btnActualizar = new Button { Left = x, Top = 225, Width = 260, Height = 36 };
            btnActualizar.Click += (s, e) => { CargarCombos(); CargarRelaciones(); };
            Controls.Add(btnActualizar);

            ActualizarIdioma(null);
        }

        private void CargarCombos()
        {
            cboCursos.DataSource = _gestorCursoProfesor.ObtenerCursosActivosParaRelacion().ToList();
            cboCursos.DisplayMember = "Nombre";
            cboCursos.ValueMember = "IdCurso";

            cboProfesores.DataSource = _gestorCursoProfesor.ObtenerProfesoresActivosParaRelacion().ToList();
            cboProfesores.DisplayMember = "NombreCompleto";
            cboProfesores.ValueMember = "IdProfesor";
        }

        private void CargarRelaciones()
        {
            dgvRelaciones.DataSource = null;
            dgvRelaciones.DataSource = _gestorCursoProfesor.ObtenerRelaciones().ToList();
            ConfigurarGrilla();
        }

        private void ConfigurarGrilla()
        {
            if (dgvRelaciones.Columns["DVH"] != null) dgvRelaciones.Columns["DVH"].Visible = false;
            if (dgvRelaciones.Columns["IdCurso"] != null) dgvRelaciones.Columns["IdCurso"].HeaderText = IdiomaUiHelper_83KI.Texto("FrmGestionCursoProfesor.IdCurso");
            if (dgvRelaciones.Columns["IdProfesor"] != null) dgvRelaciones.Columns["IdProfesor"].HeaderText = IdiomaUiHelper_83KI.Texto("FrmGestionCursoProfesor.IdProfesor");
            if (dgvRelaciones.Columns["CursoNombre"] != null) dgvRelaciones.Columns["CursoNombre"].HeaderText = IdiomaUiHelper_83KI.Texto("FrmGestionCursoProfesor.Curso");
            if (dgvRelaciones.Columns["ProfesorNombre"] != null) dgvRelaciones.Columns["ProfesorNombre"].HeaderText = IdiomaUiHelper_83KI.Texto("FrmGestionCursoProfesor.Profesor");
            if (dgvRelaciones.Columns["EstadoActivo"] != null) dgvRelaciones.Columns["EstadoActivo"].HeaderText = IdiomaUiHelper_83KI.Texto("FrmGestionCursoProfesor.RelacionActiva");
            if (dgvRelaciones.Columns["CursoActivo"] != null) dgvRelaciones.Columns["CursoActivo"].HeaderText = IdiomaUiHelper_83KI.Texto("FrmGestionCursoProfesor.CursoActivo");
            if (dgvRelaciones.Columns["ProfesorActivo"] != null) dgvRelaciones.Columns["ProfesorActivo"].HeaderText = IdiomaUiHelper_83KI.Texto("FrmGestionCursoProfesor.ProfesorActivo");
            if (dgvRelaciones.Columns["HabilitadoEfectivo"] != null) dgvRelaciones.Columns["HabilitadoEfectivo"].HeaderText = IdiomaUiHelper_83KI.Texto("FrmGestionCursoProfesor.HabilitadoEfectivo");
        }

        private void CargarSeleccion()
        {
            var relacion = RelacionSeleccionada();
            if (relacion == null) return;
            cboCursos.SelectedValue = relacion.IdCurso;
            cboProfesores.SelectedValue = relacion.IdProfesor;
        }

        private void btnHabilitar_Click(object sender, EventArgs e)
        {
            try
            {
                if (!TieneSeleccionCombos()) return;
                _gestorCursoProfesor.HabilitarProfesorParaCurso((int)cboCursos.SelectedValue, (int)cboProfesores.SelectedValue);
                CargarRelaciones();
                IdiomaUiHelper_83KI.MostrarInformacion(this, "FrmGestionCursoProfesor.Habilitado", "Comun.Informacion");
            }
            catch (Exception ex)
            {
                IdiomaUiHelper_83KI.MostrarError(this, ex, "Comun.Error", MessageBoxIcon.Warning);
            }
        }

        private void btnDeshabilitar_Click(object sender, EventArgs e)
        {
            try
            {
                var relacion = RelacionSeleccionada();
                if (relacion == null) { IdiomaUiHelper_83KI.MostrarAdvertencia(this, "FrmGestionCursoProfesor.Seleccionar", "Comun.Validacion"); return; }
                _gestorCursoProfesor.DeshabilitarProfesorParaCurso(relacion.IdCurso, relacion.IdProfesor);
                CargarRelaciones();
                IdiomaUiHelper_83KI.MostrarInformacion(this, "FrmGestionCursoProfesor.Deshabilitado", "Comun.Informacion");
            }
            catch (Exception ex)
            {
                IdiomaUiHelper_83KI.MostrarError(this, ex, "Comun.Error", MessageBoxIcon.Warning);
            }
        }

        private bool TieneSeleccionCombos()
        {
            if (cboCursos.SelectedValue == null || cboProfesores.SelectedValue == null)
            {
                IdiomaUiHelper_83KI.MostrarAdvertencia(this, "FrmGestionCursoProfesor.SeleccionarCursoProfesor", "Comun.Validacion");
                return false;
            }
            return true;
        }

        private CursoProfesor_83KI RelacionSeleccionada()
        {
            return dgvRelaciones.CurrentRow == null ? null : dgvRelaciones.CurrentRow.DataBoundItem as CursoProfesor_83KI;
        }

        private void AplicarPermisos()
        {
            dgvRelaciones.Visible = PermisosUi_83KI.TieneAlguno(PermisoSistema_83KI.VerCursoProfesor, PermisoSistema_83KI.GestionCursoProfesor);
            btnHabilitar.Visible = PermisosUi_83KI.Tiene(PermisoSistema_83KI.HabilitarCursoProfesor);
            btnDeshabilitar.Visible = PermisosUi_83KI.Tiene(PermisoSistema_83KI.DeshabilitarCursoProfesor);
        }

        public void ActualizarIdioma(IIdioma idioma)
        {
            Text = IdiomaUiHelper_83KI.Texto("FrmGestionCursoProfesor.Titulo");
            lblCurso.Text = IdiomaUiHelper_83KI.Texto("FrmGestionCursoProfesor.Curso");
            lblProfesor.Text = IdiomaUiHelper_83KI.Texto("FrmGestionCursoProfesor.Profesor");
            btnHabilitar.Text = IdiomaUiHelper_83KI.Texto("FrmGestionCursoProfesor.Habilitar");
            btnDeshabilitar.Text = IdiomaUiHelper_83KI.Texto("FrmGestionCursoProfesor.Deshabilitar");
            btnActualizar.Text = IdiomaUiHelper_83KI.Texto("FrmGestionCursoProfesor.Actualizar");
            ConfigurarGrilla();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _gestorIdioma.Desuscribir(this);
            base.OnFormClosed(e);
        }
    }
}

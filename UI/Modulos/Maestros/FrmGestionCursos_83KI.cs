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
    public class FrmGestionCursos_83KI : Form, IObservadorIdioma
    {
        private readonly IGestorCurso_83KI _gestorCurso;
        private readonly IGestorIdioma_83KI _gestorIdioma;
        private DataGridView dgvCursos;
        private Button btnCrear;
        private Button btnModificar;
        private Button btnCambiarEstado;

        public FrmGestionCursos_83KI(IGestorCurso_83KI gestorCurso)
        {
            _gestorCurso = gestorCurso;
            _gestorIdioma = ServiceFactory_83KI.GetGestorIdioma();
            InicializarControles();
            _gestorIdioma.Suscribir(this);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            CargarCursos();
            AplicarPermisos();
        }

        private void InicializarControles()
        {
            Width = 900;
            Height = 560;
            StartPosition = FormStartPosition.CenterParent;
            Font = new Font("Segoe UI", 10F);

            dgvCursos = new DataGridView { Left = 12, Top = 12, Width = 620, Height = 490, ReadOnly = true, AutoGenerateColumns = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect, MultiSelect = false };
            dgvCursos.SelectionChanged += (s, e) => ActualizarAccionEstado();
            Controls.Add(dgvCursos);

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
            ActualizarIdioma(null);
        }

        private void CargarCursos()
        {
            dgvCursos.DataSource = null;
            dgvCursos.DataSource = _gestorCurso.ObtenerCursos().ToList();
            ConfigurarGrilla();
            dgvCursos.ClearSelection();
            ActualizarAccionEstado();
        }

        private void ConfigurarGrilla()
        {
            if (dgvCursos.Columns["DVH"] != null) dgvCursos.Columns["DVH"].Visible = false;
            if (dgvCursos.Columns["IdCurso"] != null) dgvCursos.Columns["IdCurso"].HeaderText = IdiomaUiHelper_83KI.Texto("FrmGestionCursos.IdCurso");
            if (dgvCursos.Columns["Nombre"] != null) dgvCursos.Columns["Nombre"].HeaderText = IdiomaUiHelper_83KI.Texto("Comun.Nombre");
            if (dgvCursos.Columns["Descripcion"] != null) dgvCursos.Columns["Descripcion"].HeaderText = IdiomaUiHelper_83KI.Texto("FrmGestionCursos.Descripcion");
            if (dgvCursos.Columns["CargaHoraria"] != null) dgvCursos.Columns["CargaHoraria"].HeaderText = IdiomaUiHelper_83KI.Texto("FrmGestionCursos.CargaHoraria");
            if (dgvCursos.Columns["EstadoActivo"] != null) dgvCursos.Columns["EstadoActivo"].HeaderText = IdiomaUiHelper_83KI.Texto("FrmGestionCursos.Activo");
        }

        private void ActualizarAccionEstado()
        {
            var curso = CursoSeleccionado();
            if (curso == null)
            {
                btnCambiarEstado.Text = IdiomaUiHelper_83KI.Texto("FrmGestionCursos.GestionarEstado");
                return;
            }

            btnCambiarEstado.Text = curso.EstadoActivo ? IdiomaUiHelper_83KI.Texto("FrmGestionCursos.Desactivar") : IdiomaUiHelper_83KI.Texto("FrmGestionCursos.Activar");
        }

        private void btnCrear_Click(object sender, EventArgs e)
        {
            using (var dialogo = new FrmCrearCurso_83KI(_gestorCurso))
            {
                if (dialogo.ShowDialog(this) == DialogResult.OK)
                {
                    CargarCursos();
                    IdiomaUiHelper_83KI.MostrarInformacion(this, "FrmGestionCursos.Guardado", "Comun.Informacion");
                }
            }
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            var curso = CursoSeleccionado();
            if (curso == null) { IdiomaUiHelper_83KI.MostrarAdvertencia(this, "FrmGestionCursos.Seleccionar", "Comun.Validacion"); return; }

            using (var dialogo = new FrmModificarCurso_83KI(_gestorCurso, curso))
            {
                if (dialogo.ShowDialog(this) == DialogResult.OK)
                {
                    CargarCursos();
                    IdiomaUiHelper_83KI.MostrarInformacion(this, "FrmGestionCursos.Guardado", "Comun.Informacion");
                }
            }
        }

        private void btnCambiarEstado_Click(object sender, EventArgs e)
        {
            try
            {
                var curso = CursoSeleccionado();
                if (curso == null) { IdiomaUiHelper_83KI.MostrarAdvertencia(this, "FrmGestionCursos.Seleccionar", "Comun.Validacion"); return; }
                if (curso.EstadoActivo) _gestorCurso.DesactivarCurso(curso.IdCurso); else _gestorCurso.ActivarCurso(curso.IdCurso);
                CargarCursos();
            }
            catch (Exception ex)
            {
                IdiomaUiHelper_83KI.MostrarError(this, ex, "Comun.Error", MessageBoxIcon.Warning);
            }
        }

        private Curso_83KI CursoSeleccionado()
        {
            return dgvCursos.SelectedRows.Count == 0 ? null : dgvCursos.SelectedRows[0].DataBoundItem as Curso_83KI;
        }

        private void AplicarPermisos()
        {
            dgvCursos.Visible = PermisosUi_83KI.TieneAlguno(PermisoSistema_83KI.VerCursos, PermisoSistema_83KI.GestionCursos);
            btnCrear.Visible = PermisosUi_83KI.Tiene(PermisoSistema_83KI.CrearCurso);
            btnModificar.Visible = PermisosUi_83KI.Tiene(PermisoSistema_83KI.ModificarCurso);
            btnCambiarEstado.Visible = PermisosUi_83KI.TieneAlguno(PermisoSistema_83KI.ActivarCurso, PermisoSistema_83KI.DesactivarCurso);
        }

        public void ActualizarIdioma(IIdioma idioma)
        {
            Text = IdiomaUiHelper_83KI.Texto("FrmGestionCursos.Titulo");
            btnCrear.Text = IdiomaUiHelper_83KI.Texto("FrmGestionCursos.Crear");
            btnModificar.Text = IdiomaUiHelper_83KI.Texto("FrmGestionCursos.Modificar");
            btnCambiarEstado.Text = IdiomaUiHelper_83KI.Texto("FrmGestionCursos.GestionarEstado");
            ConfigurarGrilla();
            ActualizarAccionEstado();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _gestorIdioma.Desuscribir(this);
            base.OnFormClosed(e);
        }
    }
}

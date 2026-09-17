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
            Width = 900;
            Height = 560;
            StartPosition = FormStartPosition.CenterParent;
            Font = new Font("Segoe UI", 10F);

            dgvProfesores = new DataGridView { Left = 12, Top = 12, Width = 620, Height = 490, ReadOnly = true, AutoGenerateColumns = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect, MultiSelect = false };
            dgvProfesores.SelectionChanged += (s, e) => ActualizarAccionEstado();
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
            ActualizarIdioma(null);
        }

        private void CargarProfesores()
        {
            dgvProfesores.DataSource = null;
            dgvProfesores.DataSource = _gestorProfesor.ObtenerProfesores().ToList();
            ConfigurarGrilla();
            dgvProfesores.ClearSelection();
            ActualizarAccionEstado();
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

        private Profesor_83KI ProfesorSeleccionado()
        {
            return dgvProfesores.SelectedRows.Count == 0 ? null : dgvProfesores.SelectedRows[0].DataBoundItem as Profesor_83KI;
        }

        private void AplicarPermisos()
        {
            dgvProfesores.Visible = PermisosUi_83KI.TieneAlguno(PermisoSistema_83KI.VerProfesores, PermisoSistema_83KI.GestionProfesores);
            btnCrear.Visible = PermisosUi_83KI.Tiene(PermisoSistema_83KI.CrearProfesor);
            btnModificar.Visible = PermisosUi_83KI.Tiene(PermisoSistema_83KI.ModificarProfesor);
            btnCambiarEstado.Visible = PermisosUi_83KI.TieneAlguno(PermisoSistema_83KI.ActivarProfesor, PermisoSistema_83KI.DesactivarProfesor);
        }

        public void ActualizarIdioma(IIdioma idioma)
        {
            Text = IdiomaUiHelper_83KI.Texto("FrmGestionProfesores.Titulo");
            btnCrear.Text = IdiomaUiHelper_83KI.Texto("FrmGestionProfesores.Crear");
            btnModificar.Text = IdiomaUiHelper_83KI.Texto("FrmGestionProfesores.Modificar");
            btnCambiarEstado.Text = IdiomaUiHelper_83KI.Texto("FrmGestionProfesores.GestionarEstado");
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

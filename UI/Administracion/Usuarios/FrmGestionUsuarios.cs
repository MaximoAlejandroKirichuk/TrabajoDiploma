using Service.Entidades;
using Service.Interfaces;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace UI
{
    public partial class FrmGestionUsuarios : Form, IObservadorIdioma
    {
        private readonly IGestorUsuario_83KI _gestorUsuario;
        private readonly IGestorRol_83KI _gestorRol;
        private readonly IGestorIdioma_83KI _gestorIdioma;

        public FrmGestionUsuarios(IGestorUsuario_83KI gestorUsuario, IGestorRol_83KI gestorRol)
        {
            InitializeComponent();
            _gestorUsuario = gestorUsuario;
            _gestorRol = gestorRol;
            _gestorIdioma = Service.ServiceFactory_83KI.GetGestorIdioma();
            _gestorIdioma.Suscribir(this);
        }

        private void FrmGestionUsuarios_Load(object sender, EventArgs e)
        {
            AplicarPermisos();
            if (PermisosUi_83KI.Tiene(PermisoSistema_83KI.VerUsuarios))
            {
                ActualizarDatos();
            }
            ActualizarBotonesAccion();
        }

        private void AplicarPermisos()
        {
            PermisosUi_83KI.AplicarVisible(dgvUsuarios, PermisoSistema_83KI.VerUsuarios);
            PermisosUi_83KI.AplicarVisible(btnCrearUsuario, PermisoSistema_83KI.CrearUsuario);
            PermisosUi_83KI.AplicarVisible(btnModificarUsuario, PermisoSistema_83KI.ModificarUsuario);
            PermisosUi_83KI.AplicarVisible(btnDesbloquearUsuario, PermisoSistema_83KI.DesbloquearUsuario);
            btnCambiarEstadoUsuario.Visible = PermisosUi_83KI.TieneAlguno(
                PermisoSistema_83KI.HabilitarUsuario,
                PermisoSistema_83KI.DeshabilitarUsuario);
        }

        private void ActualizarDatos()
        {
            dgvUsuarios.DataSource = null;
            dgvUsuarios.DataSource = _gestorUsuario.ObtenerUsuarios();
            ConfigurarGrilla();
        }

        private void ActualizarBotonesAccion()
        {
            Usuario_83KI usuarioSeleccionado = ObtenerUsuarioSeleccionado();
            bool haySeleccion = usuarioSeleccionado != null && dgvUsuarios.Visible;
            bool puedeCambiarEstado = !haySeleccion
                ? PermisosUi_83KI.TieneAlguno(PermisoSistema_83KI.HabilitarUsuario, PermisoSistema_83KI.DeshabilitarUsuario)
                : PermisosUi_83KI.Tiene(usuarioSeleccionado.Activo ? PermisoSistema_83KI.DeshabilitarUsuario : PermisoSistema_83KI.HabilitarUsuario);

            btnModificarUsuario.Enabled = haySeleccion && btnModificarUsuario.Visible;
            btnCambiarEstadoUsuario.Visible = puedeCambiarEstado;
            btnCambiarEstadoUsuario.Enabled = haySeleccion && puedeCambiarEstado;
            btnDesbloquearUsuario.Enabled = haySeleccion && btnDesbloquearUsuario.Visible && usuarioSeleccionado.Bloqueado;

            if (!haySeleccion)
            {
                btnCambiarEstadoUsuario.Text = IdiomaUiHelper_83KI.Texto("FrmGestionUsuarios.GestionarEstado");
                btnCambiarEstadoUsuario.BackColor = SystemColors.Control;
                btnCambiarEstadoUsuario.ForeColor = SystemColors.ControlText;
                btnDesbloquearUsuario.BackColor = SystemColors.Control;
                btnDesbloquearUsuario.ForeColor = SystemColors.ControlText;
                return;
            }

            btnCambiarEstadoUsuario.Text = usuarioSeleccionado.Activo
                ? IdiomaUiHelper_83KI.Texto("FrmGestionUsuarios.DeshabilitarUsuario")
                : IdiomaUiHelper_83KI.Texto("FrmGestionUsuarios.HabilitarUsuario");
            btnCambiarEstadoUsuario.BackColor = usuarioSeleccionado.Activo ? Color.IndianRed : Color.DarkSeaGreen;
            btnCambiarEstadoUsuario.ForeColor = Color.Black;
            btnDesbloquearUsuario.BackColor = usuarioSeleccionado.Bloqueado ? Color.DarkSeaGreen : SystemColors.Control;
            btnDesbloquearUsuario.ForeColor = Color.Black;
        }

        private void btnCrearUsuario_Click(object sender, EventArgs e)
        {
            try
            {
                using (FrmCrearUsuario frmCrearUsuario = new FrmCrearUsuario(_gestorUsuario, _gestorRol))
                {
                    if (frmCrearUsuario.ShowDialog(this) == DialogResult.OK)
                    {
                        ActualizarDatos();
                        ActualizarBotonesAccion();
                    }
                }
            }
            catch (Exception ex)
            {
                IdiomaUiHelper_83KI.MostrarError(this, ex, "Comun.Error", MessageBoxIcon.Warning);
            }
        }

        private void ConfigurarGrilla()
        {
            if (dgvUsuarios.Columns["Contrasena"] != null)
            {
                dgvUsuarios.Columns["Contrasena"].Visible = false;
            }

            if (dgvUsuarios.Columns["UserName"] != null)
            {
                dgvUsuarios.Columns["UserName"].Visible = false;
            }

            if (dgvUsuarios.Columns["Nombre"] != null)
            {
                dgvUsuarios.Columns["Nombre"].DisplayIndex = 0;
            }

            if (dgvUsuarios.Columns["Apellido"] != null)
            {
                dgvUsuarios.Columns["Apellido"].DisplayIndex = 1;
            }

            if (dgvUsuarios.Columns["DNI"] != null)
            {
                dgvUsuarios.Columns["DNI"].DisplayIndex = 2;
            }

            if (dgvUsuarios.Columns["Email"] != null)
            {
                dgvUsuarios.Columns["Email"].DisplayIndex = 3;

            }

            if (dgvUsuarios.Columns["Rol"] != null)
            {
                dgvUsuarios.Columns["Rol"].HeaderText = IdiomaUiHelper_83KI.Texto("Comun.Rol");
                dgvUsuarios.Columns["Rol"].DisplayIndex = 4;
            }

            if (dgvUsuarios.Columns["Activo"] != null)
            {
                dgvUsuarios.Columns["Activo"].HeaderText = IdiomaUiHelper_83KI.Texto("FrmGestionUsuarios.Habilitado");
                dgvUsuarios.Columns["Activo"].DisplayIndex = 5;
            }

            if (dgvUsuarios.Columns["Bloqueado"] != null)
            {
                dgvUsuarios.Columns["Bloqueado"].HeaderText = IdiomaUiHelper_83KI.Texto("FrmGestionUsuarios.Bloqueado");
                dgvUsuarios.Columns["Bloqueado"].DisplayIndex = 6;
            }

           

        }

        private void dgvUsuarios_SelectionChanged(object sender, EventArgs e)
        {
            ActualizarBotonesAccion();
        }

        private void btnCambiarEstadoUsuario_Click(object sender, EventArgs e)
        {
            try
            {
                Usuario_83KI usuarioElegido = ObtenerUsuarioSeleccionado();
                if (usuarioElegido == null)
                {
                    IdiomaUiHelper_83KI.MostrarAdvertencia(this, "Validaciones.SeleccionarUsuario", "Comun.Validacion");
                    return;
                }

                using (FrmConfirmarEstadoUsuario frmConfirmacion = new FrmConfirmarEstadoUsuario(usuarioElegido))
                {
                    if (frmConfirmacion.ShowDialog(this) != DialogResult.OK)
                    {
                        return;
                    }
                }

                if (usuarioElegido.Activo)
                {
                    _gestorUsuario.DeshabilitarUsuario(usuarioElegido.DNI);
                    IdiomaUiHelper_83KI.MostrarInformacion(this, "FrmGestionUsuarios.UsuarioDeshabilitado", "Comun.Informacion");
                }
                else
                {
                    _gestorUsuario.HabilitarUsuario(usuarioElegido.DNI);
                    IdiomaUiHelper_83KI.MostrarInformacion(this, "FrmGestionUsuarios.UsuarioHabilitado", "Comun.Informacion");
                }

                ActualizarDatos();
                ActualizarBotonesAccion();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, IdiomaUiHelper_83KI.Texto("FrmGestionUsuarios.ErrorGestionarEstado", IdiomaUiHelper_83KI.TraducirExcepcion(ex)), IdiomaUiHelper_83KI.Texto("Comun.Error"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private Usuario_83KI ObtenerUsuarioSeleccionado()
        {
            return dgvUsuarios.CurrentRow?.DataBoundItem as Usuario_83KI;
        }

        private void btnModificarUsuario_Click(object sender, EventArgs e)
        {
            try
            {
                Usuario_83KI usuarioElegido = ObtenerUsuarioSeleccionado();
                if (usuarioElegido == null)
                {
                    IdiomaUiHelper_83KI.MostrarAdvertencia(this, "Validaciones.SeleccionarUsuario", "Comun.Validacion");
                    return;
                }

                using (FrmModificarUsuario frmModificarUsuario = new FrmModificarUsuario(_gestorUsuario, _gestorRol, usuarioElegido))
                {
                    if (frmModificarUsuario.ShowDialog(this) == DialogResult.OK)
                    {
                        ActualizarDatos();
                        ActualizarBotonesAccion();
                        IdiomaUiHelper_83KI.MostrarInformacion(this, "FrmGestionUsuarios.UsuarioModificado", "Comun.Informacion");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, IdiomaUiHelper_83KI.Texto("FrmGestionUsuarios.ErrorModificar", IdiomaUiHelper_83KI.TraducirExcepcion(ex)), IdiomaUiHelper_83KI.Texto("Comun.Error"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnDesbloquearUsuario_Click(object sender, EventArgs e)
        {
            try
            {
                Usuario_83KI usuarioElegido = ObtenerUsuarioSeleccionado();
                if (usuarioElegido == null)
                {
                    IdiomaUiHelper_83KI.MostrarAdvertencia(this, "Validaciones.SeleccionarUsuario", "Comun.Validacion");
                    return;
                }

                if (!usuarioElegido.Bloqueado)
                {
                    IdiomaUiHelper_83KI.MostrarAdvertencia(this, "FrmGestionUsuarios.UsuarioNoBloqueado", "Comun.Validacion");
                    return;
                }

                _gestorUsuario.DesbloquearCuenta(usuarioElegido.DNI);
                ActualizarDatos();
                ActualizarBotonesAccion();
                IdiomaUiHelper_83KI.MostrarInformacion(this, "FrmGestionUsuarios.UsuarioDesbloqueado", "Comun.Informacion");
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, IdiomaUiHelper_83KI.Texto("FrmGestionUsuarios.ErrorDesbloquear", IdiomaUiHelper_83KI.TraducirExcepcion(ex)), IdiomaUiHelper_83KI.Texto("Comun.Error"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        public void ActualizarIdioma(IIdioma idioma)
        {
            Text = IdiomaUiHelper_83KI.Texto("FrmGestionUsuarios.Titulo");
            btnCrearUsuario.Text = IdiomaUiHelper_83KI.Texto("FrmGestionUsuarios.CrearUsuario");
            btnModificarUsuario.Text = IdiomaUiHelper_83KI.Texto("FrmGestionUsuarios.ModificarUsuario");
            btnDesbloquearUsuario.Text = IdiomaUiHelper_83KI.Texto("FrmGestionUsuarios.DesbloquearUsuario");
            ConfigurarGrilla();
            ActualizarBotonesAccion();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _gestorIdioma.Desuscribir(this);
            base.OnFormClosed(e);
        }
    }
}

using Service.Entidades;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace UI
{
    public partial class FrmGestionRoles_83KI : Form, IObservadorIdioma
    {
        private readonly IGestorRol_83KI _gestorRol;
        private readonly IGestorIdioma_83KI _gestorIdioma;
        private bool _cargandoDatos;

        public FrmGestionRoles_83KI(IGestorRol_83KI gestorRol)
        {
            InitializeComponent();
            _gestorRol = gestorRol;
            _gestorIdioma = Service.ServiceFactory_83KI.GetGestorIdioma();
            _gestorIdioma.Suscribir(this);
        }

        private void FrmGestionRoles_83KI_Load(object sender, EventArgs e)
        {
            ActualizarVisibilidad();
            CargarDatos();
        }

        private void CargarDatos()
        {
            _cargandoDatos = true;
            CargarRolesGestion();
            CargarFamiliasCrear();
            CargarPatentesCrear();
            _cargandoDatos = false;
            CargarFamiliasDelRol();
            CargarPatentesDelRol();
            CargarFamiliasGestion();
            CargarPatentesGestion();
        }

        // ── Tab: Crear rol ─────────────────────────────────────────────

        private void CargarFamiliasCrear()
        {
            clbFamiliasCrear.DataSource = null;
            clbFamiliasCrear.DisplayMember = nameof(Familia_83KI.Nombre);
            clbFamiliasCrear.ValueMember = nameof(Familia_83KI.CodigoFamilia);
            clbFamiliasCrear.DataSource = _gestorRol.ObtenerFamilias().ToList();
        }

        private void CargarPatentesCrear()
        {
            clbPatentesCrear.DataSource = null;
            clbPatentesCrear.DisplayMember = nameof(Patente_83KI.Nombre);
            clbPatentesCrear.ValueMember = nameof(Patente_83KI.CodigoPatente);
            clbPatentesCrear.DataSource = _gestorRol.ObtenerPatentes().ToList();
        }

        private void btnCrearRol_Click(object sender, EventArgs e)
        {
            string nombre = txtNombreRol.Text.Trim();

            if (string.IsNullOrWhiteSpace(nombre))
            {
                IdiomaUiHelper_83KI.MostrarAdvertencia(this, "Errores.NombreObligatorio", "FrmGestionRoles.Titulo");
                return;
            }

            List<int> codigosPatentes = clbPatentesCrear.CheckedItems
                .OfType<Patente_83KI>()
                .Select(p => p.CodigoPatente)
                .ToList();

            List<int> codigosFamilias = clbFamiliasCrear.CheckedItems
                .OfType<Familia_83KI>()
                .Select(f => f.CodigoFamilia)
                .ToList();

            if (codigosPatentes.Count == 0 && codigosFamilias.Count == 0)
            {
                IdiomaUiHelper_83KI.MostrarAdvertencia(this, "Errores.RolSinComponente", "FrmGestionRoles.Titulo");
                return;
            }

            try
            {
                Rol_83KI rolCreado = _gestorRol.CrearRol(nombre, codigosPatentes, codigosFamilias);
                LimpiarCreacion();
                CargarDatos();
                cmbRolExistente.SelectedValue = rolCreado.CodigoRol;
                tabPrincipal.SelectedTab = tabGestionar;
            }
            catch (Exception ex)
            {
                IdiomaUiHelper_83KI.MostrarError(this, ex, "FrmGestionRoles.Titulo", MessageBoxIcon.Warning);
            }
        }

        private void btnLimpiarCreacion_Click(object sender, EventArgs e)
        {
            LimpiarCreacion();
        }

        private void LimpiarCreacion()
        {
            txtNombreRol.Clear();

            for (int i = 0; i < clbFamiliasCrear.Items.Count; i++)
            {
                clbFamiliasCrear.SetItemChecked(i, false);
            }

            for (int i = 0; i < clbPatentesCrear.Items.Count; i++)
            {
                clbPatentesCrear.SetItemChecked(i, false);
            }
        }

        // ── Tab: Gestionar roles ───────────────────────────────────────

        private void CargarRolesGestion()
        {
            Rol_83KI seleccionado = cmbRolExistente.SelectedItem as Rol_83KI;
            int? codigoAnterior = seleccionado?.CodigoRol;

            cmbRolExistente.DataSource = null;
            cmbRolExistente.DisplayMember = nameof(Rol_83KI.Nombre);
            cmbRolExistente.ValueMember = nameof(Rol_83KI.CodigoRol);
            cmbRolExistente.DataSource = _gestorRol.ObtenerRolesConPermisos().ToList();

            if (codigoAnterior.HasValue)
            {
                cmbRolExistente.SelectedValue = codigoAnterior.Value;
            }
        }

        private void cmbRolExistente_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_cargandoDatos)
            {
                return;
            }

            CargarFamiliasDelRol();
            CargarPatentesDelRol();
            CargarFamiliasGestion();
            CargarPatentesGestion();
        }

        private Rol_83KI ObtenerRolSeleccionado()
        {
            return cmbRolExistente.SelectedItem as Rol_83KI;
        }

        private void CargarFamiliasDelRol()
        {
            Rol_83KI rol = ObtenerRolSeleccionado();
            lstFamiliasRol.DataSource = null;
            lstFamiliasRol.DisplayMember = nameof(Familia_83KI.Nombre);
            lstFamiliasRol.ValueMember = nameof(Familia_83KI.CodigoFamilia);
            lstFamiliasRol.DataSource = rol == null
                ? null
                : rol.Familias.OrderBy(f => f.Nombre).ToList();

            CargarContenidoFamilia();
        }

        private void lstFamiliasRol_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarContenidoFamilia();
        }

        private void CargarContenidoFamilia()
        {
            treeContenidoFamilia.Nodes.Clear();

            Familia_83KI familia = lstFamiliasRol.SelectedItem as Familia_83KI;

            if (familia == null)
            {
                return;
            }

            TreeNode raiz = CrearNodo(familia);
            treeContenidoFamilia.Nodes.Add(raiz);
            raiz.ExpandAll();
        }

        private TreeNode CrearNodo(ComponentePermiso_83KI componente)
        {
            TreeNode nodo = new TreeNode(componente.Nombre) { Tag = componente };

            Familia_83KI familia = componente as Familia_83KI;

            if (familia == null)
            {
                return nodo;
            }

            foreach (ComponentePermiso_83KI hijo in familia.Hijos.OrderBy(h => h.Nombre))
            {
                nodo.Nodes.Add(CrearNodo(hijo));
            }

            return nodo;
        }

        private void CargarPatentesDelRol()
        {
            lstPatentesRol.DataSource = null;

            Rol_83KI rol = ObtenerRolSeleccionado();
            lstPatentesRol.DisplayMember = nameof(Patente_83KI.Nombre);
            lstPatentesRol.ValueMember = nameof(Patente_83KI.CodigoPatente);
            lstPatentesRol.DataSource = rol == null
                ? null
                : rol.ObtenerPatentes().OrderBy(p => p.Nombre).ToList();
        }

        private void CargarFamiliasGestion()
        {
            Rol_83KI rol = ObtenerRolSeleccionado();
            var familias = _gestorRol.ObtenerFamilias().ToList();

            if (rol != null)
            {
                var codigosActuales = new HashSet<int>(rol.Familias.Select(f => f.CodigoFamilia));
                familias = familias.Where(f => !codigosActuales.Contains(f.CodigoFamilia)).ToList();
            }

            Familia_83KI seleccionada = cmbFamiliaAgregar.SelectedItem as Familia_83KI;
            int? codigoAnterior = seleccionada?.CodigoFamilia;

            cmbFamiliaAgregar.DataSource = null;
            cmbFamiliaAgregar.DisplayMember = nameof(Familia_83KI.Nombre);
            cmbFamiliaAgregar.ValueMember = nameof(Familia_83KI.CodigoFamilia);
            cmbFamiliaAgregar.DataSource = familias;

            if (codigoAnterior.HasValue && familias.Any(f => f.CodigoFamilia == codigoAnterior.Value))
            {
                cmbFamiliaAgregar.SelectedValue = codigoAnterior.Value;
            }
        }

        private void CargarPatentesGestion()
        {
            cmbPatenteAgregar.DataSource = null;

            Rol_83KI rol = ObtenerRolSeleccionado();
            if (rol == null)
            {
                return;
            }

            var patentes = _gestorRol.ObtenerPatentes().ToList();
            var codigosActuales = new HashSet<int>(
                rol.ObtenerPatentes().Select(p => p.CodigoPatente));
            patentes = patentes.Where(p => !codigosActuales.Contains(p.CodigoPatente)).ToList();

            Patente_83KI seleccionada = cmbPatenteAgregar.SelectedItem as Patente_83KI;
            int? codigoAnterior = seleccionada?.CodigoPatente;

            cmbPatenteAgregar.DisplayMember = nameof(Patente_83KI.Nombre);
            cmbPatenteAgregar.ValueMember = nameof(Patente_83KI.CodigoPatente);
            cmbPatenteAgregar.DataSource = patentes;

            if (codigoAnterior.HasValue && patentes.Any(p => p.CodigoPatente == codigoAnterior.Value))
            {
                cmbPatenteAgregar.SelectedValue = codigoAnterior.Value;
            }
        }

        private void btnAgregarFamilia_Click(object sender, EventArgs e)
        {
            Rol_83KI rol = ObtenerRolSeleccionado();
            Familia_83KI familia = cmbFamiliaAgregar.SelectedItem as Familia_83KI;

            if (rol == null || familia == null)
            {
                return;
            }

            EjecutarOperacion(() => _gestorRol.AsignarFamiliaARol(rol.CodigoRol, familia.CodigoFamilia));
        }

        private void btnQuitarFamilia_Click(object sender, EventArgs e)
        {
            Rol_83KI rol = ObtenerRolSeleccionado();
            Familia_83KI familia = lstFamiliasRol.SelectedItem as Familia_83KI;

            if (rol == null || familia == null)
            {
                return;
            }

            var resultado = IdiomaUiHelper_83KI.Mostrar(this,
                "FrmGestionRoles.ConfirmarQuitarFamilia",
                "FrmGestionRoles.Titulo",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (resultado != DialogResult.Yes)
            {
                return;
            }

            EjecutarOperacion(() => _gestorRol.QuitarFamiliaDeRol(rol.CodigoRol, familia.CodigoFamilia));
        }

        private void btnAsignarPatente_Click(object sender, EventArgs e)
        {
            Rol_83KI rol = ObtenerRolSeleccionado();
            Patente_83KI patente = cmbPatenteAgregar.SelectedItem as Patente_83KI;

            if (rol == null || patente == null)
            {
                return;
            }

            EjecutarOperacion(() => _gestorRol.AsignarPatenteARol(rol.CodigoRol, patente.CodigoPatente));
        }

        private void btnQuitarPatente_Click(object sender, EventArgs e)
        {
            Rol_83KI rol = ObtenerRolSeleccionado();
            Patente_83KI patente = lstPatentesRol.SelectedItem as Patente_83KI;

            if (rol == null || patente == null)
            {
                return;
            }

            bool patenteDirecta = rol.PatentesDirectas
                .Any(p => p.CodigoPatente == patente.CodigoPatente);

            if (!patenteDirecta)
            {
                IdiomaUiHelper_83KI.MostrarInformacion(this, "FrmGestionRoles.PatenteIndirecta", "FrmGestionRoles.Titulo");
                return;
            }

            var resultado = IdiomaUiHelper_83KI.Mostrar(this,
                "FrmGestionRoles.ConfirmarQuitarPatente",
                "FrmGestionRoles.Titulo",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (resultado != DialogResult.Yes)
            {
                return;
            }

            EjecutarOperacion(() => _gestorRol.QuitarPatenteDeRol(rol.CodigoRol, patente.CodigoPatente));
        }

        private void btnEliminarRol_Click(object sender, EventArgs e)
        {
            Rol_83KI rol = ObtenerRolSeleccionado();

            if (rol == null)
            {
                return;
            }

            var resultado = IdiomaUiHelper_83KI.Mostrar(this,
                "FrmGestionRoles.ConfirmarEliminarRol",
                "FrmGestionRoles.Titulo",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (resultado != DialogResult.Yes)
            {
                return;
            }

            EjecutarOperacion(() => _gestorRol.EliminarRol(rol.CodigoRol));
        }

        // ── Shared ──────────────────────────────────────────────────────

        private void EjecutarOperacion(Action operacion)
        {
            try
            {
                operacion();
                CargarDatos();
                ActualizarVisibilidad();
            }
            catch (Exception ex)
            {
                IdiomaUiHelper_83KI.MostrarError(this, ex, "FrmGestionRoles.Titulo", MessageBoxIcon.Warning);
            }
        }

        private void ActualizarVisibilidad()
        {
            bool puedeVerRoles = PermisosUi_83KI.Tiene(PermisoSistema_83KI.VerRoles);
            bool puedeVerPermisosEfectivos = PermisosUi_83KI.Tiene(PermisoSistema_83KI.VerPermisosEfectivosRol);
            bool puedeGestionarRoles = PermisosUi_83KI.Tiene(PermisoSistema_83KI.GestionRoles);
            bool puedeAgregarFamilia = PermisosUi_83KI.Tiene(PermisoSistema_83KI.AgregarFamiliaRol);
            bool puedeQuitarFamilia = PermisosUi_83KI.Tiene(PermisoSistema_83KI.QuitarFamiliaRol);
            bool puedeAsignarPatente = PermisosUi_83KI.Tiene(PermisoSistema_83KI.AsignarPatenteRol);
            bool puedeQuitarPatente = PermisosUi_83KI.Tiene(PermisoSistema_83KI.QuitarPatenteRol);
            bool puedeEliminarRol = PermisosUi_83KI.Tiene(PermisoSistema_83KI.EliminarRol);

            // Tab Crear
            bool puedeCrear = puedeGestionarRoles;
            tabCrear.Visible = puedeCrear;
            lblNombreRol.Visible = puedeCrear;
            txtNombreRol.Visible = puedeCrear;
            lblFamiliasCrear.Visible = puedeCrear;
            clbFamiliasCrear.Visible = puedeCrear;
            lblPatentesCrear.Visible = puedeCrear;
            clbPatentesCrear.Visible = puedeCrear;
            btnCrearRol.Visible = puedeCrear;
            btnLimpiarCreacion.Visible = puedeCrear;

            // Tab Gestionar
            bool puedeGestionar = puedeVerRoles;
            tabGestionar.Visible = puedeGestionar;
            lblRoles.Visible = puedeGestionar;
            cmbRolExistente.Visible = puedeGestionar;
            lblFamiliasRol.Visible = puedeGestionar;
            lstFamiliasRol.Visible = puedeGestionar;
            lblPatentesFamilia.Visible = puedeVerPermisosEfectivos;
            treeContenidoFamilia.Visible = puedeVerPermisosEfectivos;
            lblPatentesRol.Visible = puedeVerPermisosEfectivos;
            lstPatentesRol.Visible = puedeVerPermisosEfectivos;

            // Acciones
            lblFamiliasGestion.Visible = puedeAgregarFamilia;
            cmbFamiliaAgregar.Visible = puedeAgregarFamilia;
            btnAgregarFamilia.Visible = puedeAgregarFamilia;
            btnQuitarFamilia.Visible = puedeQuitarFamilia;

            btnAsignarPatente.Visible = puedeAsignarPatente;
            btnQuitarPatente.Visible = puedeQuitarPatente;
            cmbPatenteAgregar.Visible = puedeAsignarPatente;
            lblPatentesGestion.Visible = puedeAsignarPatente;

            btnEliminarRol.Visible = puedeEliminarRol;
        }

        // ── Idioma ──────────────────────────────────────────────────────

        public void ActualizarIdioma(IIdioma idioma)
        {
            Text = IdiomaUiHelper_83KI.Texto("FrmGestionRoles.Titulo");

            tabCrear.Text = IdiomaUiHelper_83KI.Texto("FrmGestionRoles.TabCrearRol");
            tabGestionar.Text = IdiomaUiHelper_83KI.Texto("FrmGestionRoles.TabGestionarRoles");

            // Tab Crear
            lblNombreRol.Text = IdiomaUiHelper_83KI.Texto("FrmGestionRoles.Nombre");
            lblFamiliasCrear.Text = IdiomaUiHelper_83KI.Texto("FrmGestionRoles.Familias");
            lblPatentesCrear.Text = IdiomaUiHelper_83KI.Texto("FrmGestionRoles.Patentes");
            btnCrearRol.Text = IdiomaUiHelper_83KI.Texto("Comun.Guardar");
            btnLimpiarCreacion.Text = IdiomaUiHelper_83KI.Texto("Comun.Limpiar");

            // Tab Gestionar
            lblRoles.Text = IdiomaUiHelper_83KI.Texto("FrmGestionRoles.Roles");
            lblFamiliasRol.Text = IdiomaUiHelper_83KI.Texto("FrmGestionRoles.FamiliasRol");
            lblPatentesFamilia.Text = IdiomaUiHelper_83KI.Texto("FrmGestionRoles.ContenidoFamilia");
            lblPatentesRol.Text = IdiomaUiHelper_83KI.Texto("FrmGestionRoles.PatentesRol");

            lblFamiliasGestion.Text = IdiomaUiHelper_83KI.Texto("FrmGestionRoles.FamiliasDisponibles");
            btnAgregarFamilia.Text = IdiomaUiHelper_83KI.Texto("Comun.Agregar");
            btnQuitarFamilia.Text = IdiomaUiHelper_83KI.Texto("Comun.Quitar");

            lblPatentesGestion.Text = IdiomaUiHelper_83KI.Texto("FrmGestionRoles.PatentesDisponibles");
            btnAsignarPatente.Text = IdiomaUiHelper_83KI.Texto("Comun.Asignar");
            btnQuitarPatente.Text = IdiomaUiHelper_83KI.Texto("Comun.Quitar");

            btnEliminarRol.Text = IdiomaUiHelper_83KI.Texto("FrmGestionRoles.EliminarRol");

            ActualizarVisibilidad();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _gestorIdioma.Desuscribir(this);
            base.OnFormClosed(e);
        }
    }
}

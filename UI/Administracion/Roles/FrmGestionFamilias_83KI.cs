using Service.Entidades;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace UI
{
    public partial class FrmGestionFamilias_83KI : Form, IObservadorIdioma
    {
        private readonly IGestorRol_83KI _gestorRol;
        private readonly IGestorIdioma_83KI _gestorIdioma;
        private bool _cargandoDatos;

        public FrmGestionFamilias_83KI(IGestorRol_83KI gestorRol)
        {
            InitializeComponent();
            _gestorRol = gestorRol;
            _gestorIdioma = Service.ServiceFactory_83KI.GetGestorIdioma();
            _gestorIdioma.Suscribir(this);
        }

        private void FrmGestionFamilias_83KI_Load(object sender, EventArgs e)
        {
            CargarDatos();
            ActualizarVisibilidad();
        }

        private void CargarDatos()
        {
            _cargandoDatos = true;
            CargarFamiliasGestion();
            CargarPatentesCreacion();
            CargarFamiliasCreacion();
            _cargandoDatos = false;
            CargarDetalleFamilia();
            CargarCombosGestion();
            ActualizarBotonesRemover();
        }

        // ── Pestaña: Crear familia ──────────────────────────────────────

        private void CargarPatentesCreacion()
        {
            lstPatentesDisponibles.DataSource = null;
            lstPatentesDisponibles.DisplayMember = nameof(Patente_83KI.Nombre);
            lstPatentesDisponibles.ValueMember = nameof(Patente_83KI.CodigoPatente);
            lstPatentesDisponibles.DataSource = _gestorRol.ObtenerPatentes().ToList();
        }

        private void CargarFamiliasCreacion()
        {
            var familias = _gestorRol.ObtenerFamilias().ToList();
            clbFamiliasCreacion.DataSource = null;
            clbFamiliasCreacion.DisplayMember = nameof(Familia_83KI.Nombre);
            clbFamiliasCreacion.ValueMember = nameof(Familia_83KI.CodigoFamilia);
            clbFamiliasCreacion.DataSource = familias;
        }

        private void btnCrearFamilia_Click(object sender, EventArgs e)
        {
            string nombre = txtNombreFamilia.Text.Trim();

            if (string.IsNullOrWhiteSpace(nombre))
            {
                IdiomaUiHelper_83KI.MostrarAdvertencia(this, "Errores.NombreObligatorio", "FrmGestionFamilias.Titulo");
                return;
            }

            List<int> codigosPatentes = lstPatentesDisponibles.CheckedItems
                .OfType<Patente_83KI>()
                .Select(p => p.CodigoPatente)
                .ToList();

            List<int> codigosFamilias = clbFamiliasCreacion.CheckedItems
                .OfType<Familia_83KI>()
                .Select(f => f.CodigoFamilia)
                .ToList();

            if (codigosPatentes.Count == 0 && codigosFamilias.Count == 0)
            {
                IdiomaUiHelper_83KI.MostrarAdvertencia(this, "Errores.FamiliaSinPatente", "FrmGestionFamilias.Titulo");
                return;
            }

            try
            {
                Familia_83KI familiaCreada = _gestorRol.CrearFamilia(nombre, codigosPatentes, codigosFamilias);
                LimpiarCreacion();
                CargarDatos();
                cmbFamiliaExistente.SelectedValue = familiaCreada.CodigoFamilia;
                tabPrincipal.SelectedTab = tabGestionar;
            }
            catch (Exception ex)
            {
                IdiomaUiHelper_83KI.MostrarError(this, ex, "FrmGestionFamilias.Titulo", MessageBoxIcon.Warning);
            }
        }

        private void btnLimpiarCreacion_Click(object sender, EventArgs e)
        {
            LimpiarCreacion();
        }

        private void LimpiarCreacion()
        {
            txtNombreFamilia.Clear();

            for (int i = 0; i < lstPatentesDisponibles.Items.Count; i++)
            {
                lstPatentesDisponibles.SetItemChecked(i, false);
            }

            for (int i = 0; i < clbFamiliasCreacion.Items.Count; i++)
            {
                clbFamiliasCreacion.SetItemChecked(i, false);
            }
        }

        // ── Pestaña: Gestionar familias ─────────────────────────────────

        private void CargarFamiliasGestion()
        {
            Familia_83KI seleccionada = cmbFamiliaExistente.SelectedItem as Familia_83KI;
            int? codigoAnterior = seleccionada?.CodigoFamilia;

            cmbFamiliaExistente.DataSource = null;
            cmbFamiliaExistente.DisplayMember = nameof(Familia_83KI.Nombre);
            cmbFamiliaExistente.ValueMember = nameof(Familia_83KI.CodigoFamilia);
            cmbFamiliaExistente.DataSource = _gestorRol.ObtenerFamilias().ToList();

            if (codigoAnterior.HasValue)
            {
                cmbFamiliaExistente.SelectedValue = codigoAnterior.Value;
            }
        }

        private void cmbFamiliaExistente_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_cargandoDatos)
            {
                return;
            }

            CargarDetalleFamilia();
            CargarCombosGestion();
            ActualizarBotonesRemover();
        }

        private void CargarDetalleFamilia()
        {
            treeFamilia.Nodes.Clear();

            if (!PermisosUi_83KI.Tiene(PermisoSistema_83KI.VerFamilias))
            {
                return;
            }

            Familia_83KI familia = cmbFamiliaExistente.SelectedItem as Familia_83KI;

            if (familia == null)
            {
                return;
            }

            TreeNode raiz = CrearNodo(familia);
            treeFamilia.Nodes.Add(raiz);
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

        private void treeFamilia_AfterSelect(object sender, TreeViewEventArgs e)
        {
            ActualizarBotonesRemover();
        }

        private void ActualizarBotonesRemover()
        {
            TreeNode nodo = treeFamilia.SelectedNode;
            bool esHijoDirecto = nodo != null
                && nodo.Parent != null
                && treeFamilia.Nodes.Count > 0
                && nodo.Parent == treeFamilia.Nodes[0];

            btnQuitarPatente.Enabled = esHijoDirecto && nodo.Tag is Patente_83KI;
            btnQuitarSubfamilia.Enabled = esHijoDirecto && nodo.Tag is Familia_83KI;
        }

        private void CargarCombosGestion()
        {
            CargarComboPatentesGestion();
            CargarComboSubfamiliasGestion();
        }

        private void CargarComboPatentesGestion()
        {
            Familia_83KI familia = cmbFamiliaExistente.SelectedItem as Familia_83KI;
            var patentes = _gestorRol.ObtenerPatentes().ToList();

            if (familia != null)
            {
                var codigosDirectos = new HashSet<int>(
                    familia.Hijos.OfType<Patente_83KI>().Select(p => p.CodigoPatente));
                patentes = patentes.Where(p => !codigosDirectos.Contains(p.CodigoPatente)).ToList();
            }

            Patente_83KI seleccionada = cmbPatenteAgregar.SelectedItem as Patente_83KI;
            int? codigoAnterior = seleccionada?.CodigoPatente;

            cmbPatenteAgregar.DataSource = null;
            cmbPatenteAgregar.DisplayMember = nameof(Patente_83KI.Nombre);
            cmbPatenteAgregar.ValueMember = nameof(Patente_83KI.CodigoPatente);
            cmbPatenteAgregar.DataSource = patentes;

            if (codigoAnterior.HasValue && patentes.Any(p => p.CodigoPatente == codigoAnterior.Value))
            {
                cmbPatenteAgregar.SelectedValue = codigoAnterior.Value;
            }
        }

        private void CargarComboSubfamiliasGestion()
        {
            Familia_83KI familia = cmbFamiliaExistente.SelectedItem as Familia_83KI;
            var familias = _gestorRol.ObtenerFamilias().ToList();

            if (familia != null)
            {
                familias = familias
                    .Where(f => f.CodigoFamilia != familia.CodigoFamilia
                                && !familia.Contiene(f)
                                && !f.Contiene(familia))
                    .ToList();
            }

            Familia_83KI seleccionada = cmbSubfamiliaAgregar.SelectedItem as Familia_83KI;
            int? codigoAnterior = seleccionada?.CodigoFamilia;

            cmbSubfamiliaAgregar.DataSource = null;
            cmbSubfamiliaAgregar.DisplayMember = nameof(Familia_83KI.Nombre);
            cmbSubfamiliaAgregar.ValueMember = nameof(Familia_83KI.CodigoFamilia);
            cmbSubfamiliaAgregar.DataSource = familias;

            if (codigoAnterior.HasValue && familias.Any(f => f.CodigoFamilia == codigoAnterior.Value))
            {
                cmbSubfamiliaAgregar.SelectedValue = codigoAnterior.Value;
            }
        }

        private void btnAgregarPatente_Click(object sender, EventArgs e)
        {
            Familia_83KI familia = cmbFamiliaExistente.SelectedItem as Familia_83KI;
            Patente_83KI patente = cmbPatenteAgregar.SelectedItem as Patente_83KI;

            if (familia == null)
            {
                IdiomaUiHelper_83KI.MostrarAdvertencia(this, "FrmGestionFamilias.SeleccionarFamilia", "FrmGestionFamilias.Titulo");
                return;
            }

            if (patente == null)
            {
                IdiomaUiHelper_83KI.MostrarAdvertencia(this, "FrmGestionFamilias.SeleccionarPatenteAgregar", "FrmGestionFamilias.Titulo");
                return;
            }

            var resultado = IdiomaUiHelper_83KI.Mostrar(this,
                "FrmGestionFamilias.ConfirmarAgregarPatente",
                "FrmGestionFamilias.Titulo",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (resultado != DialogResult.Yes)
            {
                return;
            }

            EjecutarOperacion(() => _gestorRol.AsignarPatenteAFamilia(familia.CodigoFamilia, patente.CodigoPatente));
        }

        private void btnQuitarPatente_Click(object sender, EventArgs e)
        {
            EjecutarRemocion<Patente_83KI>(
                "FrmGestionFamilias.ConfirmarQuitarPatente",
                "FrmGestionFamilias.NodoNoEsPatente",
                (codigoFamilia, codigoComponente) => _gestorRol.QuitarPatenteDeFamilia(codigoFamilia, codigoComponente));
        }

        private void btnAgregarSubfamilia_Click(object sender, EventArgs e)
        {
            Familia_83KI familia = cmbFamiliaExistente.SelectedItem as Familia_83KI;
            Familia_83KI subfamilia = cmbSubfamiliaAgregar.SelectedItem as Familia_83KI;

            if (familia == null)
            {
                IdiomaUiHelper_83KI.MostrarAdvertencia(this, "FrmGestionFamilias.SeleccionarFamilia", "FrmGestionFamilias.Titulo");
                return;
            }

            if (subfamilia == null)
            {
                IdiomaUiHelper_83KI.MostrarAdvertencia(this, "FrmGestionFamilias.SeleccionarSubfamiliaAgregar", "FrmGestionFamilias.Titulo");
                return;
            }

            var resultado = IdiomaUiHelper_83KI.Mostrar(this,
                "FrmGestionFamilias.ConfirmarAgregarSubfamilia",
                "FrmGestionFamilias.Titulo",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (resultado != DialogResult.Yes)
            {
                return;
            }

            EjecutarOperacion(() => _gestorRol.AsignarFamiliaAFamilia(familia.CodigoFamilia, subfamilia.CodigoFamilia));
        }

        private void btnQuitarSubfamilia_Click(object sender, EventArgs e)
        {
            EjecutarRemocion<Familia_83KI>(
                "FrmGestionFamilias.ConfirmarQuitarSubfamilia",
                "FrmGestionFamilias.NodoNoEsSubfamilia",
                (codigoFamilia, codigoComponente) => _gestorRol.QuitarFamiliaDeFamilia(codigoFamilia, codigoComponente));
        }

        private void EjecutarRemocion<T>(string claveConfirmacion, string claveTipoInvalido,
            Action<int, int> accionRemover) where T : ComponentePermiso_83KI
        {
            Familia_83KI familia = cmbFamiliaExistente.SelectedItem as Familia_83KI;

            if (familia == null)
            {
                return;
            }

            TreeNode nodo = treeFamilia.SelectedNode;

            if (nodo == null || nodo.Tag == null)
            {
                IdiomaUiHelper_83KI.MostrarAdvertencia(this, "FrmGestionFamilias.SeleccionarNodoArbol", "FrmGestionFamilias.Titulo");
                return;
            }

            if (!(nodo.Tag is T componente))
            {
                IdiomaUiHelper_83KI.MostrarAdvertencia(this, claveTipoInvalido, "FrmGestionFamilias.Titulo");
                return;
            }

            var resultado = IdiomaUiHelper_83KI.Mostrar(this,
                claveConfirmacion,
                "FrmGestionFamilias.Titulo",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (resultado != DialogResult.Yes)
            {
                return;
            }

            EjecutarOperacion(() => accionRemover(familia.CodigoFamilia, componente.Codigo));
        }

        private void btnEliminarFamilia_Click(object sender, EventArgs e)
        {
            Familia_83KI familia = cmbFamiliaExistente.SelectedItem as Familia_83KI;

            if (familia == null)
            {
                return;
            }

            var resultado = IdiomaUiHelper_83KI.Mostrar(this,
                "FrmGestionFamilias.ConfirmarEliminarFamilia",
                "FrmGestionFamilias.Titulo",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (resultado != DialogResult.Yes)
            {
                return;
            }

            EjecutarOperacion(() => _gestorRol.EliminarFamilia(familia.CodigoFamilia));
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
                IdiomaUiHelper_83KI.MostrarError(this, ex, "FrmGestionFamilias.Titulo", MessageBoxIcon.Warning);
            }
        }

        private void ActualizarVisibilidad()
        {
            bool puedeVerFamilias = PermisosUi_83KI.Tiene(PermisoSistema_83KI.VerFamilias);
            bool puedeCrearFamilia = PermisosUi_83KI.Tiene(PermisoSistema_83KI.CrearFamilia);
            bool puedeEliminarFamilia = PermisosUi_83KI.Tiene(PermisoSistema_83KI.EliminarFamilia);
            bool puedeAgregarPatente = PermisosUi_83KI.Tiene(PermisoSistema_83KI.AgregarPatenteFamilia);
            bool puedeQuitarPatente = PermisosUi_83KI.Tiene(PermisoSistema_83KI.QuitarPatenteFamilia);
            bool puedeAgregarSubfamilia = PermisosUi_83KI.Tiene(PermisoSistema_83KI.AgregarSubfamilia);
            bool puedeQuitarSubfamilia = PermisosUi_83KI.Tiene(PermisoSistema_83KI.QuitarSubfamilia);

            // Tab Crear
            lblNombre.Visible = puedeCrearFamilia;
            txtNombreFamilia.Visible = puedeCrearFamilia;
            lblPatentes.Visible = puedeCrearFamilia;
            lstPatentesDisponibles.Visible = puedeCrearFamilia;
            lblSubfamilias.Visible = puedeCrearFamilia;
            clbFamiliasCreacion.Visible = puedeCrearFamilia;
            btnCrearFamilia.Visible = puedeCrearFamilia;
            btnLimpiarCreacion.Visible = puedeCrearFamilia;

            // Tab Gestionar
            lblFamiliasExistentes.Visible = puedeVerFamilias;
            cmbFamiliaExistente.Visible = puedeVerFamilias;
            lblDetalleFamilia.Visible = puedeVerFamilias;
            treeFamilia.Visible = puedeVerFamilias;

            lblPatentesGestion.Visible = puedeVerFamilias && puedeAgregarPatente;
            cmbPatenteAgregar.Visible = puedeVerFamilias && puedeAgregarPatente;
            btnAgregarPatente.Visible = puedeVerFamilias && puedeAgregarPatente;

            btnQuitarPatente.Visible = puedeVerFamilias && puedeQuitarPatente;

            lblSubfamiliasGestion.Visible = puedeVerFamilias && puedeAgregarSubfamilia;
            cmbSubfamiliaAgregar.Visible = puedeVerFamilias && puedeAgregarSubfamilia;
            btnAgregarSubfamilia.Visible = puedeVerFamilias && puedeAgregarSubfamilia;

            btnQuitarSubfamilia.Visible = puedeVerFamilias && puedeQuitarSubfamilia;

            btnEliminarFamilia.Visible = puedeVerFamilias && puedeEliminarFamilia;
        }

        // ── Idioma ──────────────────────────────────────────────────────

        public void ActualizarIdioma(IIdioma idioma)
        {
            Text = IdiomaUiHelper_83KI.Texto("FrmGestionFamilias.Titulo");

            tabCrear.Text = IdiomaUiHelper_83KI.Texto("FrmGestionFamilias.CrearFamiliaPanel");
            tabGestionar.Text = IdiomaUiHelper_83KI.Texto("FrmGestionFamilias.TabGestionar");

            lblNombre.Text = IdiomaUiHelper_83KI.Texto("FrmGestionFamilias.Nombre");
            lblPatentes.Text = IdiomaUiHelper_83KI.Texto("FrmGestionFamilias.PatentesDisponibles");
            lblSubfamilias.Text = IdiomaUiHelper_83KI.Texto("FrmGestionFamilias.Subfamilias");
            btnCrearFamilia.Text = IdiomaUiHelper_83KI.Texto("Comun.Guardar");
            btnLimpiarCreacion.Text = IdiomaUiHelper_83KI.Texto("Comun.Limpiar");

            lblFamiliasExistentes.Text = IdiomaUiHelper_83KI.Texto("FrmGestionFamilias.SeleccionarFamiliaExistente");
            lblDetalleFamilia.Text = IdiomaUiHelper_83KI.Texto("FrmGestionFamilias.DetalleFamilia");

            lblPatentesGestion.Text = IdiomaUiHelper_83KI.Texto("FrmGestionFamilias.PatentesDisponibles");
            btnAgregarPatente.Text = IdiomaUiHelper_83KI.Texto("FrmGestionFamilias.AgregarPatente");
            btnQuitarPatente.Text = IdiomaUiHelper_83KI.Texto("FrmGestionFamilias.QuitarPatente");

            lblSubfamiliasGestion.Text = IdiomaUiHelper_83KI.Texto("FrmGestionFamilias.FamiliasDisponibles");
            btnAgregarSubfamilia.Text = IdiomaUiHelper_83KI.Texto("FrmGestionFamilias.AgregarSubfamilia");
            btnQuitarSubfamilia.Text = IdiomaUiHelper_83KI.Texto("FrmGestionFamilias.QuitarSubfamilia");

            btnEliminarFamilia.Text = IdiomaUiHelper_83KI.Texto("FrmGestionFamilias.EliminarFamilia");

            ActualizarVisibilidad();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _gestorIdioma.Desuscribir(this);
            base.OnFormClosed(e);
        }


    }
}

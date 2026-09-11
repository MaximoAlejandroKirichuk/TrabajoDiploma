using Service;
using Service.DTOs;
using Service.Entidades;
using Service.Interfaces;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace UI
{
    /// <summary>
    /// formulario de recuperacion del admin para inconsistencias de integridad de datos.
    /// provee acciones de recalculo de hashes, backup y restore de base de datos
    /// controladas por los permisos 38 (EjecutarBackup), 39 (EjecutarRestore) y 40 (RecalcularHashes).
    /// todos los textos usan claves de localizacion bajo "RecuperacionIntegridad.*".
    /// </summary>
    public partial class FrmRecuperacionIntegridad_83KI : Form
    {
        private readonly IIntegridadDatosService_83KI _integridadService;
        private readonly IRecuperacionBaseDatosService_83KI _recuperacionService;
        private bool _recalculoExitoso;

        public FrmRecuperacionIntegridad_83KI()
        {
            InitializeComponent();
            _integridadService = ServiceFactory_83KI.GetIntegridadDatosService();
            _recuperacionService = ServiceFactory_83KI.GetRecuperacionBaseDatosService();
        }

        /// <summary>
        /// true cuando el admin recalculo hashes exitosamente durante esta sesion.
        /// los llamadores pueden usar esto para decidir si limpiar el estado de recuperacion.
        /// </summary>
        public bool RecalculoExitoso
        {
            get { return _recalculoExitoso; }
        }

        private void FrmRecuperacionIntegridad_Load(object sender, EventArgs e)
        {
            AplicarTextos();
            AplicarPermisos();
            CargarEstadoIntegridad();
        }

        private void AplicarTextos()
        {
            Text = Texto("RecuperacionIntegridad.Titulo");
            lblTitulo.Text = Texto("RecuperacionIntegridad.Titulo");
            grpEstado.Text = Texto("RecuperacionIntegridad.EstadoSistema");
            lblTablas.Text = Texto("RecuperacionIntegridad.TablasAfectadas");
            grpAcciones.Text = Texto("RecuperacionIntegridad.RecalcularHashes");
            btnBackup.Text = Texto("RecuperacionIntegridad.Backup");
            lblBackupDesc.Text = Texto("RecuperacionIntegridad.BackupDescripcion");
            btnRestore.Text = Texto("RecuperacionIntegridad.Restore");
            lblRestoreDesc.Text = Texto("RecuperacionIntegridad.RestoreDescripcion");
            btnCerrar.Text = Texto("Comun.Cancelar");
        }

        private void AplicarPermisos()
        {
            btnBackup.Visible = PermisosUi_83KI.Tiene(PermisoSistema_83KI.EjecutarBackup);
            btnRestore.Visible = PermisosUi_83KI.Tiene(PermisoSistema_83KI.EjecutarRestore);

            lblBackupDesc.Visible = btnBackup.Visible;
            lblRestoreDesc.Visible = btnRestore.Visible;
        }

        private void CargarEstadoIntegridad()
        {
            try
            {
                var estado = _integridadService.Verificar();

                if (estado.EstaSano)
                {
                    lblEstadoSistema.ForeColor = Color.DarkGreen;
                    lblEstadoSistema.Text = Texto("RecuperacionIntegridad.EstadoSaludable");
                }
                else
                {
                    lblEstadoSistema.ForeColor = Color.DarkRed;
                    lblEstadoSistema.Text = Texto("RecuperacionIntegridad.EstadoComprometido");
                }

                treeInconsistencias.Nodes.Clear();

                if (!estado.EstaSano && estado.Tablas != null)
                {
                    int contador = 0;

                    foreach (var tabla in estado.Tablas.Where(t => !t.EsValido))
                    {
                        if (tabla.FilasInconsistentes != null && tabla.FilasInconsistentes.Count > 0)
                        {
                            // agrupa por tabla
                            TreeNode nodoTabla = new TreeNode(tabla.NombreTabla)
                            {
                                Tag = tabla,
                                ImageIndex = 0,
                                SelectedImageIndex = 0
                            };

                            foreach (var fila in tabla.FilasInconsistentes)
                            {
                                // nodo fila / pk
                                TreeNode nodoFila = new TreeNode(fila.ClavePrimaria)
                                {
                                    Tag = fila
                                };

                                // nodo tipo
                                string tipoTexto = ObtenerTextoInconsistencia(fila.Tipo);
                                TreeNode nodoTipo = new TreeNode(tipoTexto);

                                // opcional: nodo de columnas afectadas (informativo)
                                if (fila.ColumnasAfectadas != null && fila.ColumnasAfectadas.Count > 0)
                                {
                                    string columnas = string.Join(", ", fila.ColumnasAfectadas);
                                    TreeNode nodoColumnas = new TreeNode($"{Texto("RecuperacionIntegridad.Columnas")}: {columnas}");
                                    nodoTipo.Nodes.Add(nodoColumnas);
                                }

                                nodoFila.Nodes.Add(nodoTipo);
                                nodoTabla.Nodes.Add(nodoFila);
                                contador++;
                            }

                            treeInconsistencias.Nodes.Add(nodoTabla);
                        }
                        else
                        {
                            // falla a nivel tabla sin detalle por fila (ej. inaccesible)
                            TreeNode nodoTabla = new TreeNode($"{tabla.NombreTabla} — {Texto("RecuperacionIntegridad.SinDetalleFilas")}");
                            treeInconsistencias.Nodes.Add(nodoTabla);
                            contador++;
                        }
                    }

                    if (contador > 0)
                    {
                        lblTablas.Text = string.Format(Texto("RecuperacionIntegridad.FilasInconsistentes"), contador);
                    }

                    // expande los primeros dos niveles para visibilidad inmediata
                    treeInconsistencias.ExpandAll();
                }

                if (treeInconsistencias.Nodes.Count == 0 && estado.EstaSano)
                {
                    treeInconsistencias.Nodes.Add(new TreeNode(Texto("RecuperacionIntegridad.Ninguna")));
                }
            }
            catch (Exception ex)
            {
                lblEstadoSistema.Text = Texto("RecuperacionIntegridad.ErrorVerificar");
                treeInconsistencias.Nodes.Clear();
                treeInconsistencias.Nodes.Add(new TreeNode(ex.Message));
            }
        }

        private void btnBackup_Click(object sender, EventArgs e)
        {
            if (!PermisosUi_83KI.Tiene(PermisoSistema_83KI.EjecutarBackup))
            {
                IdiomaUiHelper_83KI.MostrarAdvertencia(
                    this,
                    "Errores.SinPermisos",
                    "Comun.Seguridad");
                return;
            }

            using (var saveDialog = new SaveFileDialog())
            {
                saveDialog.Filter = "Backup files (*.bak)|*.bak|All files (*.*)|*.*";
                saveDialog.DefaultExt = "bak";
                saveDialog.FileName = "GestionUsuarios_backup_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".bak";
                saveDialog.Title = Texto("RecuperacionIntegridad.Backup");

                if (saveDialog.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                string rutaOriginal = saveDialog.FileName;
                var usuarioActivo = SessionManager_83KI.Instancia.UsuarioActivo;
                string actor = usuarioActivo?.UserName ?? "sistema";

                try
                {
                    _recuperacionService.GenerarBackup(rutaOriginal, actor);

                    IdiomaUiHelper_83KI.MostrarInformacion(
                        this,
                        "RecuperacionIntegridad.BackupExitoso",
                        "Comun.Informacion");
                }
                catch (Exception ex)
                {
                    if (EsErrorAccesoBackup(ex))
                    {
                        string nombreArchivo = System.IO.Path.GetFileName(rutaOriginal);
                        string rutaFallback = System.IO.Path.Combine(@"C:\BackupsSQL", nombreArchivo);

                        var resultado = MessageBox.Show(
                            this,
                            "No se pudo escribir el backup en la ruta seleccionada. " +
                            "Esto suele ocurrir por permisos de acceso o porque la ruta no existe.\n\n" +
                            "Detalle: " + ObtenerDetalleSqlReal(ex) + "\n\n" +
                            "¿Desea reintentar en la carpeta alternativa C:\\BackupsSQL\\?",
                            Texto("Comun.Error"),
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Warning);

                        if (resultado == DialogResult.Yes)
                        {
                            try
                            {
                                System.IO.Directory.CreateDirectory(@"C:\BackupsSQL");
                                _recuperacionService.GenerarBackup(rutaFallback, actor);

                                IdiomaUiHelper_83KI.MostrarInformacion(
                                    this,
                                    "RecuperacionIntegridad.BackupExitoso",
                                    "Comun.Informacion");
                                return;
                            }
                            catch (Exception exFallback)
                            {
                                MessageBox.Show(
                                    this,
                                    "Tampoco se pudo escribir en C:\\BackupsSQL\\. " +
                                    "Verifique que la cuenta que ejecuta el servicio de base de datos " +
                                    "(SQL Server o LocalDB) tenga permisos de escritura en esa carpeta.\n\n" +
                                    "Detalle: " + ObtenerDetalleSqlReal(exFallback),
                                    Texto("Comun.Error"),
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                                return;
                            }
                        }
                    }

                    string detalleBackup = ObtenerDetalleSqlReal(ex);
                    MessageBox.Show(
                        this,
                        "Error al realizar el backup.\n\nDetalle: " + detalleBackup,
                        Texto("Comun.Error"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        private static bool EsErrorAccesoBackup(Exception ex)
        {
            var sqlEx = ObtenerSqlException(ex);
            if (sqlEx == null)
            {
                return false;
            }
            foreach (System.Data.SqlClient.SqlError error in sqlEx.Errors)
            {
                if (error.Number == 3201 || error.Number == 3013)
                    return true;
            }
            return false;
        }

        private static System.Data.SqlClient.SqlException ObtenerSqlException(Exception ex)
        {
            var current = ex;
            while (current != null)
            {
                var sqlEx = current as System.Data.SqlClient.SqlException;
                if (sqlEx != null)
                    return sqlEx;
                current = current.InnerException;
            }
            return null;
        }

        private static string ObtenerDetalleSqlReal(Exception ex)
        {
            var sqlEx = ObtenerSqlException(ex);
            return sqlEx != null ? sqlEx.Message : ex.Message;
        }

        private void btnRestore_Click(object sender, EventArgs e)
        {
            if (!PermisosUi_83KI.Tiene(PermisoSistema_83KI.EjecutarRestore))
            {
                IdiomaUiHelper_83KI.MostrarAdvertencia(
                    this,
                    "Errores.SinPermisos",
                    "Comun.Seguridad");
                return;
            }

            var confirmacion = MessageBox.Show(
                Texto("RecuperacionIntegridad.RestoreAdvertencia"),
                Texto("RecuperacionIntegridad.Restore"),
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirmacion != DialogResult.Yes)
            {
                return;
            }

            using (var openDialog = new OpenFileDialog())
            {
                openDialog.Filter = "Backup files (*.bak)|*.bak|All files (*.*)|*.*";
                openDialog.Title = Texto("RecuperacionIntegridad.Restore");

                if (openDialog.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                try
                {
                    var usuarioActivo = SessionManager_83KI.Instancia.UsuarioActivo;
                    string actor = usuarioActivo?.UserName ?? "sistema";

                    _recuperacionService.RestaurarBackup(openDialog.FileName, actor);

                    IdiomaUiHelper_83KI.MostrarInformacion(
                        this,
                        "RecuperacionIntegridad.RestoreExitoso",
                        "Comun.Informacion");

                    // senala al llamador que la aplicacion debe cerrarse
                    DialogResult = DialogResult.Abort;
                    Close();
                }
                catch (Exception ex)
                {
                    IdiomaUiHelper_83KI.MostrarError(this, ex, "Comun.Error", MessageBoxIcon.Error);
                }
            }
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            Close();
        }

        private string Texto(string clave)
        {
            return IdiomaUiHelper_83KI.Texto(clave);
        }

        /// <summary>
        /// mapea el tipo de inconsistencia heuristica a una etiqueta en castellano
        /// para el arbol de inconsistencias.
        /// </summary>
        private static string ObtenerTextoInconsistencia(TipoInconsistenciaFilas_83KI tipo)
        {
            switch (tipo)
            {
                case TipoInconsistenciaFilas_83KI.Insercion:
                    return IdiomaUiHelper_83KI.Texto("RecuperacionIntegridad.InconsistenciaInsercion");
                case TipoInconsistenciaFilas_83KI.Modificacion:
                    return IdiomaUiHelper_83KI.Texto("RecuperacionIntegridad.InconsistenciaModificacion");
                case TipoInconsistenciaFilas_83KI.Desconocido:
                    return IdiomaUiHelper_83KI.Texto("RecuperacionIntegridad.InconsistenciaDesconocido");
                default:
                    return tipo.ToString();
            }
        }
    }
}

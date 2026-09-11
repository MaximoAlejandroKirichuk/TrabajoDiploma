using System;
using System.Diagnostics;
using System.Windows.Forms;
using Service.DTOs;
using Service.Interfaces;

namespace UI
{
    /// <summary>
    /// formulario de configuracion inicial de base de datos.
    /// permite al usuario seleccionar una instancia SQL Server, ingresar el nombre de la base,
    /// validar la conexion y confirmar la configuracion.
    /// </summary>
    public partial class FrmConfiguracionBD_83KI : Form
    {
        private readonly IBootstrapBaseDatosService_83KI _bootstrapService;
        private bool _validacionAprobada;

        public bool ConfiguracionCompletada { get; private set; }

        public FrmConfiguracionBD_83KI(IBootstrapBaseDatosService_83KI bootstrapService)
        {
            _bootstrapService = bootstrapService;
            InitializeComponent();
        }

        private void FrmConfiguracionBD_83KI_Load(object sender, EventArgs e)
        {
            Text = "Configuracion de Base de Datos";
            lblTitle.Text = "Configuracion Inicial de Base de Datos";
            lblInstance.Text = "Instancia SQL Server:";
            lblDatabase.Text = "Nombre de la Base de Datos:";
            btnValidate.Text = "Validar Conexion";
            btnConfirm.Text = "Confirmar";
            btnCancel.Text = "Cancelar";

            btnConfirm.Enabled = false;
            lblStatus.Text = "Seleccione una instancia e ingrese el nombre de la base de datos.";

            CargarInstancias();
        }

        private void CargarInstancias()
        {
            cmbInstance.Items.Clear();
            cmbInstance.Text = string.Empty;

            var instances = _bootstrapService.DescubrirInstancias();

            if (instances.Count == 0)
            {
                lblStatus.Text = "No se detectaron instancias SQL Server locales. Puede ingresar el nombre del servidor manualmente.";
                cmbInstance.DropDownStyle = ComboBoxStyle.DropDown;
            }
            else
            {
                foreach (var instance in instances)
                {
                    cmbInstance.Items.Add(instance);
                }

                if (cmbInstance.Items.Count == 1)
                {
                    cmbInstance.SelectedIndex = 0;
                }

                cmbInstance.DropDownStyle = ComboBoxStyle.DropDown;
                lblStatus.Text = $"Se detectaron {instances.Count} instancia(s). Seleccione una o ingrese manualmente.";
            }
        }

        private void btnValidate_Click(object sender, EventArgs e)
        {
            string instance = cmbInstance.Text.Trim();
            string database = txtDatabaseName.Text.Trim();

            if (string.IsNullOrWhiteSpace(instance))
            {
                lblStatus.Text = "Debe especificar una instancia de SQL Server.";
                return;
            }

            if (string.IsNullOrWhiteSpace(database))
            {
                lblStatus.Text = "Debe especificar el nombre de la base de datos.";
                return;
            }

            lblStatus.Text = "Validando conexion...";
            Cursor = Cursors.WaitCursor;
            btnValidate.Enabled = false;

            try
            {
                var settings = new ConfiguracionConexionBD_83KI
                {
                    InstanciaServidor = instance,
                    NombreBaseDatos = database
                };

                var result = _bootstrapService.Validar(settings);

                if (result.Estado == EstadoBootstrap_83KI.Listo)
                {
                    _validacionAprobada = true;
                    lblStatus.Text = result.Mensaje;
                    btnConfirm.Enabled = true;
                }
                else if (result.Estado == EstadoBootstrap_83KI.BaseDeDatosFaltante)
                {
                    _validacionAprobada = false;
                    lblStatus.Text = result.Mensaje;
                    btnConfirm.Enabled = false;

                    Cursor = Cursors.Default;
                    var dialogResult = MessageBox.Show(
                        "La base de datos no existe. ¿Desea crearla ahora?",
                        "Base de datos no encontrada",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (dialogResult == DialogResult.Yes)
                    {
                        Cursor = Cursors.WaitCursor;
                        lblStatus.Text = "Instalando base de datos. Esto puede tardar unos momentos...";

                        var installResult = _bootstrapService.InstalarBaseDatos(instance, database);

                        if (installResult.Estado == EstadoBootstrap_83KI.Listo)
                        {
                            MessageBox.Show(
                                installResult.Mensaje,
                                "Instalacion exitosa",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);

                            // Volver a validar para confirmar que la base de datos es accesible
                            var revalidateResult = _bootstrapService.Validar(settings);
                            if (revalidateResult.Estado == EstadoBootstrap_83KI.Listo)
                            {
                                _validacionAprobada = true;
                                lblStatus.Text = revalidateResult.Mensaje;
                                btnConfirm.Enabled = true;
                            }
                            else
                            {
                                lblStatus.Text = revalidateResult.Mensaje;
                            }
                        }
                        else
                        {
                            MessageBox.Show(
                                installResult.Mensaje,
                                "Error de instalacion",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                            lblStatus.Text = installResult.Mensaje;
                        }
                    }
                }
                else
                {
                    _validacionAprobada = false;
                    lblStatus.Text = result.Mensaje;
                    btnConfirm.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                _validacionAprobada = false;
                lblStatus.Text = $"Error inesperado durante la validacion: {ex.Message}";
                btnConfirm.Enabled = false;
            }
            finally
            {
                Cursor = Cursors.Default;
                btnValidate.Enabled = true;
            }
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            if (!_validacionAprobada)
                return;

            string instance = cmbInstance.Text.Trim();
            string database = txtDatabaseName.Text.Trim();

            var settings = new ConfiguracionConexionBD_83KI
            {
                InstanciaServidor = instance,
                NombreBaseDatos = database
            };

            _bootstrapService.Guardar(settings);
            ConfiguracionCompletada = true;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ConfiguracionCompletada = false;
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnRetryDiscovery_Click(object sender, EventArgs e)
        {
            lblStatus.Text = "Reintentando deteccion de instancias...";
            Cursor = Cursors.WaitCursor;
            btnRetryDiscovery.Enabled = false;

            try
            {
                CargarInstancias();
            }
            finally
            {
                Cursor = Cursors.Default;
                btnRetryDiscovery.Enabled = true;
            }
        }

        private void lnkInstallGuide_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "https://www.microsoft.com/es-es/sql-server/sql-server-downloads",
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No se pudo abrir la guia de instalacion: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }
    }
}

using System;
using System.Windows.Forms;
using Service;
using Service.DTOs;

namespace UI
{
    internal static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// Evalua la configuracion de base de datos antes de mostrar Login.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // registrar el proveedor de configuracion antes del bootstrap
            ServiceFactory_83KI.EstablecerProveedorConfiguracionConexion(new ProveedorConfiguracionBD_83KI());

            // puerta de bootstrap: evaluar configuracion antes de llegar a Login
            var bootstrapService = ServiceFactory_83KI.ObtenerServicioBootstrapBaseDatos();
            var result = bootstrapService.EvaluarInicio();

            if (result.Estado == EstadoBootstrap_83KI.Listo)
            {
                // configuracion valida — continuar normalmente
                Application.Run(new Login());
                return;
            }

            // configuracion falta o es invalida — mostrar formulario de configuracion
            bool setupCompletado = false;
            while (!setupCompletado)
            {
                using (var setupForm = new FrmConfiguracionBD_83KI(bootstrapService))
                {
                    var dialogResult = setupForm.ShowDialog();

                    if (dialogResult == DialogResult.OK && setupForm.ConfiguracionCompletada)
                    {
                        // re-evaluar despues de guardar configuracion
                        result = bootstrapService.EvaluarInicio();
                        if (result.Estado == EstadoBootstrap_83KI.Listo)
                        {
                            setupCompletado = true;
                            Application.Run(new Login());
                            return;
                        }

                        // la configuracion se guardo pero sigue sin ser valida — reabrir formulario
                        MessageBox.Show(
                            result.Mensaje,
                            "Error de Configuracion",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                    }
                    else
                    {
                        // usuario cancelo — salir limpiamente
                        return;
                    }
                }
            }
        }
    }
}

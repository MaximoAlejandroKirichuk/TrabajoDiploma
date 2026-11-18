using System;
using System.Linq;
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
        static int Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // registrar el proveedor de configuracion antes del bootstrap
            ServiceFactory_83KI.EstablecerProveedorConfiguracionConexion(new ProveedorConfiguracionBD_83KI());

            // modo sin pantalla que usa el instalador: deja consistentes los hashes de integridad de la base recien instalada
            if (args.Any(a => string.Equals(a, "/recalcular-integridad", StringComparison.OrdinalIgnoreCase)))
                return RecalcularIntegridadSinPantalla();

            // puerta de bootstrap: evaluar configuracion antes de llegar a Login
            var bootstrapService = ServiceFactory_83KI.ObtenerServicioBootstrapBaseDatos();
            var result = bootstrapService.EvaluarInicio();

            if (result.Estado == EstadoBootstrap_83KI.Listo)
            {
                // configuracion valida — continuar normalmente
                Application.Run(new Login());
                return 0;
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
                            return 0;
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
                        return 0;
                    }
                }
            }

            return 0;
        }

        private static int RecalcularIntegridadSinPantalla()
        {
            try
            {
                var estado = ServiceFactory_83KI.GetIntegridadDatosService().RecalcularTodo("Instalador");
                return estado != null && estado.EstaSano ? 0 : 2;
            }
            catch
            {
                return 1;
            }
        }
    }
}

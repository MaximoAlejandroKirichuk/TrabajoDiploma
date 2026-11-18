using Microsoft.Win32;
using Service.DTOs;
using Service.Interfaces;

namespace UI
{
    /// <summary>
    /// adaptador sobre Properties.Settings.Default para persistir y cargar
    /// la configuracion de conexion a la base de datos fuera del codigo fuente.
    /// Si el instalador dejo una conexion nueva (InstallId distinto al ya aplicado por este usuario),
    /// la toma una sola vez para que la aplicacion apunte a la base recien instalada.
    /// </summary>
    public class ProveedorConfiguracionBD_83KI : IProveedorConfiguracionConexion_83KI
    {
        private const string ClaveInstalador = @"Software\InstitutoVisionEnAccion";

        public ConfiguracionConexionBD_83KI Cargar()
        {
            AplicarConexionDelInstalador();

            return new ConfiguracionConexionBD_83KI
            {
                InstanciaServidor = Properties.Settings.Default.DatabaseServerInstance,
                NombreBaseDatos = Properties.Settings.Default.DatabaseName
            };
        }

        public void Guardar(ConfiguracionConexionBD_83KI settings)
        {
            if (settings == null)
                throw new System.ArgumentNullException(nameof(settings));

            Properties.Settings.Default.DatabaseServerInstance = settings.InstanciaServidor;
            Properties.Settings.Default.DatabaseName = settings.NombreBaseDatos;
            Properties.Settings.Default.Save();
        }

        private static void AplicarConexionDelInstalador()
        {
            try
            {
                string idInstalacion, instancia, baseDatos;

                // vista de 64 bits: el instalador escribe ahi aunque la aplicacion corra como 32 bits
                using (var raiz = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
                using (var clave = raiz.OpenSubKey(ClaveInstalador))
                {
                    if (clave == null) return;
                    idInstalacion = clave.GetValue("InstallId") as string;
                    instancia = clave.GetValue("SqlInstance") as string;
                    baseDatos = clave.GetValue("SqlDatabase") as string;
                }

                if (string.IsNullOrWhiteSpace(idInstalacion) ||
                    string.IsNullOrWhiteSpace(instancia) || string.IsNullOrWhiteSpace(baseDatos))
                    return;

                using (var claveUsuario = Registry.CurrentUser.CreateSubKey(ClaveInstalador))
                {
                    if (claveUsuario == null) return;
                    if (idInstalacion == claveUsuario.GetValue("InstallIdAplicado") as string) return;

                    Properties.Settings.Default.DatabaseServerInstance = instancia;
                    Properties.Settings.Default.DatabaseName = baseDatos;
                    Properties.Settings.Default.Save();
                    claveUsuario.SetValue("InstallIdAplicado", idInstalacion);
                }
            }
            catch
            {
                // sin permiso de registro se conserva la configuracion guardada del usuario
            }
        }
    }
}

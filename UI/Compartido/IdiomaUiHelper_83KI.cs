using Service;
using Service.Entidades;
using Service.Excepciones;
using Service.Excepciones.CambiarContrasenaUsuario;
using Service.Excepciones.CrearUsuario;
using Service.Excepciones.Login;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace UI
{
    internal static class IdiomaUiHelper_83KI
    {
        private static readonly Dictionary<string, string> ClavesErrores = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Email ya registrado a un usuario.", "Errores.EmailRegistrado" },
            { "El DNI ya esta registrado a un usuario", "Errores.DniRegistrado" },
            { "El usuario no existe", "Errores.UsuarioNoExiste" },
            { "El usuario ya inicio sesion en este dispositivo", "Errores.UsuarioActivoActualmente" },
            { "La cuenta está bloqueada. Contacte a un administrador.", "Errores.UsuarioBloqueado" },
            { "La cuenta esta bloqueada. Contacte a un administrador.", "Errores.UsuarioBloqueado" },
            { "El usuario se encuentra deshabilitado. Contacte a un administrador.", "Errores.UsuarioDeshabilitado" },
            { "No hay un usuario autenticado para realizar esta operación.", "Errores.UsuarioNoAutenticado" },
            { "No hay un usuario autenticado para realizar esta operacion.", "Errores.UsuarioNoAutenticado" },
            { "La contraseña actual ingresada no coincide con la registrada.", "Errores.ContrasenaActualInvalida" },
            { "La nueva contraseña y su confirmación deben coincidir.", "Errores.ContrasenasNoCoinciden" },
            { "La nueva contraseña y su confirmacion deben coincidir.", "Errores.ContrasenasNoCoinciden" },
            { "No tiene permisos para realizar esta accion.", "Errores.SinPermisos" },
            { "No puede modificar su propio rol.", "Errores.ModificarPropioRol" },
            { "No puede deshabilitar su propio usuario.", "Errores.DeshabilitarPropioUsuario" },
            { "El rol del usuario no existe o no pudo cargarse con permisos.", "Errores.RolUsuarioNoExiste" },
            { "El rol seleccionado no existe.", "Errores.RolNoExiste" },
            { "La familia seleccionada no existe.", "Errores.FamiliaNoExiste" },
            { "La patente seleccionada no existe.", "Errores.PatenteNoExiste" },
            { "Ya existe un rol con ese nombre.", "Errores.RolDuplicado" },
            { "Ya existe una familia con ese nombre.", "Errores.FamiliaDuplicada" },
            { "No se puede eliminar la familia porque está asignada a uno o más roles.", "Errores.FamiliaAsignadaARol" },
            { "No se puede eliminar la familia porque es subfamilia de otra familia.", "Errores.FamiliaEsSubfamilia" },
            { "El nombre es obligatorio.", "Errores.NombreObligatorio" },
            { "El email es obligatorio.", "Errores.EmailObligatorio" },
            { "El formato del email no es valido.", "Errores.EmailFormatoInvalido" },
            { "El formato del email no es válido.", "Errores.EmailFormatoInvalido" },
            { "El componente ya se encuentra asignado.", "Errores.ComponenteAsignado" },
            { "La patente ya se encuentra asignada.", "Errores.PatenteAsignada" },
            { "La asignacion duplicaria permisos indirectos.", "Errores.PermisosIndirectosDuplicados" },
            { "No se puede asignar una familia a si misma ni generar ciclos.", "Errores.CicloFamilia" },
            { "Una patente no puede contener otros permisos.", "Errores.PatenteNoContienePermisos" },
            { "La familia debe contener al menos una patente.", "Errores.FamiliaSinPatente" },
            { "El rol debe contener al menos una patente o familia.", "Errores.RolSinComponente" },
            { "El rol tiene usuarios asignados.", "Errores.RolConUsuarios" }
        };

        public static IGestorIdioma_83KI GestorIdioma
        {
            get { return ServiceFactory_83KI.GetGestorIdioma(); }
        }

        public static string Texto(string clave)
        {
            return GestorIdioma.ObtenerTexto(clave);
        }

        public static string Texto(string clave, params object[] args)
        {
            return string.Format(Texto(clave), args);
        }

        public static DialogResult Mostrar(IWin32Window owner, string claveMensaje, string claveTitulo, MessageBoxButtons botones, MessageBoxIcon icono)
        {
            return MessageBox.Show(owner, Texto(claveMensaje), Texto(claveTitulo), botones, icono);
        }

        public static void MostrarInformacion(IWin32Window owner, string claveMensaje, string claveTitulo)
        {
            MessageBox.Show(owner, Texto(claveMensaje), Texto(claveTitulo), MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static void MostrarAdvertencia(IWin32Window owner, string claveMensaje, string claveTitulo)
        {
            MessageBox.Show(owner, Texto(claveMensaje), Texto(claveTitulo), MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        public static void MostrarError(IWin32Window owner, Exception ex, string claveTitulo, MessageBoxIcon icono)
        {
            MessageBox.Show(owner, TraducirExcepcion(ex), Texto(claveTitulo), MessageBoxButtons.OK, icono);
        }

        public static string TraducirExcepcion(Exception ex)
        {
            if (ex == null)
            {
                return string.Empty;
            }

            if (ex is DniRegistradoException_83KI)
            {
                return Texto("Errores.DniRegistrado");
            }

            if (ex is EmailRegistradoException_83KI)
            {
                return Texto("Errores.EmailRegistrado");
            }

            if (ex is UsuarioNoExisteException_83KI)
            {
                return Texto("Errores.UsuarioNoExiste");
            }

            if (ex is UsuarioActivoActualmenteException_83KI)
            {
                return Texto("Errores.UsuarioActivoActualmente");
            }

            if (ex is UsuarioBloqueadoException_83KI)
            {
                return Texto("Errores.UsuarioBloqueado");
            }

            if (ex is UsuarioDeshabilitadoException_83KI)
            {
                return Texto("Errores.UsuarioDeshabilitado");
            }

            if (ex is UsuarioNoAutenticadoException_83KI)
            {
                return Texto("Errores.UsuarioNoAutenticado");
            }

            if (ex is Coincidir_Actual_Nueva_ContrasenaException)
            {
                return Texto("Errores.ContrasenasNoCoinciden");
            }

            if (ex is ContrasenaInvalidaException_83KI)
            {
                return TraducirContrasenaInvalida(ex.Message);
            }

            string mensaje = ex.Message ?? string.Empty;
            string clave;
            if (ClavesErrores.TryGetValue(mensaje, out clave))
            {
                return Texto(clave);
            }

            if (mensaje.StartsWith("Errores.", StringComparison.OrdinalIgnoreCase) && mensaje.Contains("|"))
            {
                int pipeIndex = mensaje.IndexOf('|');
                string claveTraduccion = mensaje.Substring(0, pipeIndex);
                string argsRaw = mensaje.Substring(pipeIndex + 1);
                string[] args = argsRaw.Split('|');
                return Texto(claveTraduccion, (object[])args);
            }

            if (mensaje.StartsWith("El usuario seleccionado ", StringComparison.OrdinalIgnoreCase))
            {
                string estado = mensaje.Substring("El usuario seleccionado ".Length);
                string claveEstado = estado.IndexOf("habilitado", StringComparison.OrdinalIgnoreCase) >= 0
                    ? "Errores.UsuarioSeleccionadoHabilitado"
                    : "Errores.UsuarioSeleccionadoDeshabilitado";
                return Texto(claveEstado);
            }

            return mensaje;
        }

        public static string TraducirModulo(Modulo modulo)
        {
            return Texto("Dominio.Modulo." + modulo);
        }

        public static string TraducirCriticidad(Criticidad criticidad)
        {
            return Texto("Dominio.Criticidad." + criticidad);
        }

        public static string TraducirEventoBitacora(string evento)
        {
            if (string.IsNullOrWhiteSpace(evento))
            {
                return string.Empty;
            }

            // Resolver primero la descripcion cruda de BD al nombre canonico del catalogo
            string nombreCanonico = EventoBitacoraCatalogo_83KI.ResolverNombre(evento);
            string claveCatalogo = CrearClaveCatalogo(nombreCanonico ?? evento);

            return Texto("Dominio.EventoBitacora." + claveCatalogo);
        }

        public static string CrearClaveCatalogo(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
            {
                return string.Empty;
            }

            string clave = Regex.Replace(texto.Normalize(NormalizationForm.FormD), @"\p{Mn}", string.Empty);
            clave = Regex.Replace(clave, @"[^A-Za-z0-9]+", string.Empty);
            return clave;
        }

        private static string TraducirContrasenaInvalida(string mensaje)
        {
            string detalle = mensaje ?? string.Empty;
            const string prefijo = "La contraseña es invalida:";

            if (detalle.StartsWith(prefijo, StringComparison.OrdinalIgnoreCase))
            {
                detalle = detalle.Substring(prefijo.Length).Trim();
            }

            Match intentos = Regex.Match(detalle, @"Intento\s+(\d+)\s+de\s+(\d+)", RegexOptions.IgnoreCase);
            if (intentos.Success)
            {
                return Texto("Errores.ContrasenaInvalidaIntento", intentos.Groups[1].Value, intentos.Groups[2].Value);
            }

            if (detalle.IndexOf("actual ingresada no coincide", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return Texto("Errores.ContrasenaActualInvalida");
            }

            return Texto("Errores.ContrasenaInvalida");
        }
    }

    internal sealed class ComboItemIdioma_83KI
    {
        public ComboItemIdioma_83KI(object valor, string texto)
        {
            Valor = valor;
            Texto = texto;
        }

        public object Valor { get; private set; }
        public string Texto { get; private set; }

        public override string ToString()
        {
            return Texto;
        }
    }
}

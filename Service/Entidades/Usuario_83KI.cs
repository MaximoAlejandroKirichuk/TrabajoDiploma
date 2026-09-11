using System;
using System.Text.RegularExpressions;

namespace Service.Entidades
{
    public class Usuario_83KI
    {
        private const int DniMinimoValido = 1000000;
        private const string PatronEmail = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

        //pk
        public int DNI { get; private set; }
        public string Apellido { get; private set; }
        public string Nombre { get; private set; }
        public Rol_83KI Rol { get; private set; }
        public string Contrasena { get; private set; }
        public bool Activo { get; private set; }
        public bool Bloqueado { get; private set; }
        public string Email { get; private set; }
        public string IdiomaId { get; private set; }
        public int IntentosRealizados { get; private set; }
        public DateTime? FechaUltimoIntento { get; private set; }

        public string UserName => $"{DNI}{Nombre}";

        private Usuario_83KI()
        {
        }

        public static Usuario_83KI CrearNuevo(string nombre, string apellido, int dni, string email, Rol_83KI rol, string contrasenaHash)
        {
            return new Usuario_83KI
            {
                DNI = ValidarDni(dni),
                Nombre = ValidarTextoObligatorio(nombre, nameof(nombre)),
                Apellido = ValidarTextoObligatorio(apellido, nameof(apellido)),
                Email = ValidarEmail(email),
                Rol = ValidarRol(rol),
                Contrasena = ValidarContrasena(contrasenaHash),
                Activo = true,
                Bloqueado = false,
                IdiomaId = GestorIdioma_83KI.IdiomaPorDefecto,
                IntentosRealizados = 0,
                FechaUltimoIntento = null,
            };
        }

        public static Usuario_83KI ReconstruirDesdePersistencia(
            int dni,
            string nombre,
            string apellido,
            string email,
            string contrasenaHash,
            Rol_83KI rol,
            bool activo,
            bool bloqueado,
            string idiomaId,
            int intentosRealizados,
            DateTime? fechaUltimoIntento
            )
        {

            return new Usuario_83KI
            {
                DNI = ValidarDni(dni),
                Nombre = ValidarTextoObligatorio(nombre, nameof(nombre)),
                Apellido = ValidarTextoObligatorio(apellido, nameof(apellido)),
                Email = ValidarEmail(email),
                Contrasena = ValidarContrasena(contrasenaHash),
                Rol = ValidarRol(rol),
                Activo = activo,
                Bloqueado = bloqueado,
                IdiomaId = idiomaId,
                IntentosRealizados = ValidarIntentosRealizados(intentosRealizados),
                FechaUltimoIntento = fechaUltimoIntento,
            };
        }

        public void Bloquear()
        {
            Bloqueado = true;
        }

        public void Desbloquear(string contrasenaHash)
        {
            CambiarContrasena(contrasenaHash);
            Bloqueado = false;
            ReiniciarIntentosFallidos();
        }

        public void RegistrarIntentoFallido(DateTime fecha)
        {
            IntentosRealizados++;
            FechaUltimoIntento = fecha;
        }

        public void ReiniciarIntentosFallidos()
        {
            IntentosRealizados = 0;
            FechaUltimoIntento = null;
        }

        public static string    EstablecerContrasenaPorDefecto(string apellido, int dni)
        {
            string apellidoNormalizado = ValidarTextoObligatorio(apellido, nameof(apellido));
            int dniValidado = ValidarDni(dni);

            return $"{apellidoNormalizado}{dniValidado}";
        }

        public void CambiarContrasena(string contrasenaHash)
        {
            Contrasena = ValidarContrasena(contrasenaHash);
        }

        public void ModificarEmailYRol(string email, Rol_83KI rol)
        {
            Email = ValidarEmail(email);
            Rol = ValidarRol(rol);
        }

        public void AsignarRol(Rol_83KI rol)
        {
            Rol = ValidarRol(rol);
        }

        public void ModificarEmail(string email)
        {
            Email = ValidarEmail(email);
        }

        public void CambiarIdioma(string idiomaId)
        {
            IdiomaId = (idiomaId);
        }

        public void Habilitar()
        {
            Activo = true;
        }

        public void Deshabilitar()
        {
            Activo = false;
        }

        private static int ValidarDni(int dni)
        {
            if (dni < DniMinimoValido)
            {
                throw new ArgumentException("El DNI debe ser un numero valido.", nameof(dni));
            }

            return dni;
        }

        private static string ValidarTextoObligatorio(string valor, string nombreParametro)
        {
            if (string.IsNullOrWhiteSpace(valor))
            {
                throw new ArgumentException("El valor es obligatorio.", nombreParametro);
            }

            return valor.Trim();
        }

        private static string ValidarEmail(string email)
        {
            string emailNormalizado = ValidarTextoObligatorio(email, nameof(email));

            if (!Regex.IsMatch(emailNormalizado, PatronEmail))
            {
                throw new ArgumentException("El formato del email no es valido.", nameof(email));
            }

            return emailNormalizado;
        }

        private static string ValidarContrasena(string contrasenaHash)
        {
            //luki: el name of ayuda a que las excepciones digan que parametro fallo.
            return ValidarTextoObligatorio(contrasenaHash, nameof(contrasenaHash));
        }

        private static Rol_83KI ValidarRol(Rol_83KI rol)
        {
            if (rol == null)
            {
                throw new ArgumentException("El rol de usuario no es valido.", nameof(rol));
            }
            if (rol.CodigoRol <= 0) 
                throw new ArgumentException("El codigo de rol del usuario tiene que ser valido");
            return rol;
        }

        private static int ValidarIntentosRealizados(int intentosRealizados)
        {
            if (intentosRealizados < 0)
            {
                throw new ArgumentException("La cantidad de intentos realizados no puede ser negativa.", nameof(intentosRealizados));
            }

            return intentosRealizados;
        }
    }
}

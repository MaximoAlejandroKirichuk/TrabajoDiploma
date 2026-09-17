using System;
using System.Text.RegularExpressions;

namespace BE.Entidades
{
    public class Profesor_83KI
    {
        private const string PatronEmail = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

        public int IdProfesor { get; private set; }
        public string DNI { get; private set; }
        public string Nombre { get; private set; }
        public string Apellido { get; private set; }
        public string NombreCompleto { get { return $"{Apellido}, {Nombre}"; } }
        public string Email { get; private set; }
        public bool EstadoActivo { get; private set; }
        public string DVH { get; private set; }

        private Profesor_83KI() { }

        public static Profesor_83KI CrearNuevo(string dni, string nombre, string apellido, string email)
        {
            return new Profesor_83KI
            {
                DNI = ValidarTexto(dni, "Errores.ProfesorDniObligatorio"),
                Nombre = ValidarTexto(nombre, "Errores.ProfesorNombreObligatorio"),
                Apellido = ValidarTexto(apellido, "Errores.ProfesorApellidoObligatorio"),
                Email = ValidarEmail(email),
                EstadoActivo = true,
                DVH = string.Empty
            };
        }

        public static Profesor_83KI ReconstruirDesdePersistencia(int idProfesor, string dni, string nombre, string apellido, string email, bool estadoActivo, string dvh)
        {
            var profesor = CrearNuevo(dni, nombre, apellido, email);
            profesor.IdProfesor = idProfesor;
            profesor.EstadoActivo = estadoActivo;
            profesor.DVH = dvh ?? string.Empty;
            return profesor;
        }

        public void Modificar(string nombre, string apellido, string email)
        {
            Nombre = ValidarTexto(nombre, "Errores.ProfesorNombreObligatorio");
            Apellido = ValidarTexto(apellido, "Errores.ProfesorApellidoObligatorio");
            Email = ValidarEmail(email);
        }

        public void Activar() { EstadoActivo = true; }
        public void Desactivar() { EstadoActivo = false; }

        private static string ValidarTexto(string valor, string claveError)
        {
            if (string.IsNullOrWhiteSpace(valor))
                throw new ArgumentException(claveError);
            return valor.Trim();
        }

        private static string ValidarEmail(string email)
        {
            string normalizado = ValidarTexto(email, "Errores.ProfesorEmailObligatorio");
            if (!Regex.IsMatch(normalizado, PatronEmail))
                throw new ArgumentException("Errores.EmailFormatoInvalido");
            return normalizado;
        }
    }
}

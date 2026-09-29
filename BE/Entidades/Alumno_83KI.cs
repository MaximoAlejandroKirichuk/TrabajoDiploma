using System;

namespace BE.Entidades
{
    public class Alumno_83KI
    {
        public const string EstadoActivo = "activo";

        public int IdAlumno { get; private set; }
        public string DNI { get; private set; }
        public string Nombre { get; private set; }
        public string Apellido { get; private set; }
        public string Email { get; private set; }
        public string Telefono { get; private set; }
        public DateTime FechaAlta { get; private set; }
        public string Estado { get; private set; }
        public string DVH { get; private set; }

        private Alumno_83KI() { }

        public string NombreCompleto { get { return (Nombre + " " + Apellido).Trim(); } }

        public static Alumno_83KI Crear(string dni, string nombre, string apellido, string email, string telefono)
        {
            Validar(dni, nombre, apellido, email, telefono);
            return new Alumno_83KI
            {
                DNI = Normalizar(dni),
                Nombre = Normalizar(nombre),
                Apellido = Normalizar(apellido),
                Email = Normalizar(email).ToLowerInvariant(),
                Telefono = NormalizarTelefono(telefono),
                FechaAlta = DateTime.Now,
                Estado = EstadoActivo,
                DVH = string.Empty
            };
        }

        public static Alumno_83KI ReconstruirDesdePersistencia(int idAlumno, string dni, string nombre, string apellido, string email, string telefono, DateTime fechaAlta, string estado, string dvh)
        {
            var alumno = Crear(dni, nombre, apellido, email, telefono);
            alumno.IdAlumno = idAlumno;
            alumno.FechaAlta = fechaAlta;
            alumno.Estado = string.IsNullOrWhiteSpace(estado) ? EstadoActivo : estado;
            alumno.DVH = dvh ?? string.Empty;
            return alumno;
        }

        private static void Validar(string dni, string nombre, string apellido, string email, string telefono)
        {
            if (string.IsNullOrWhiteSpace(dni)) throw new ArgumentException("Errores.AlumnoDniObligatorio", nameof(dni));
            if (string.IsNullOrWhiteSpace(nombre)) throw new ArgumentException("Errores.AlumnoNombreObligatorio", nameof(nombre));
            if (string.IsNullOrWhiteSpace(apellido)) throw new ArgumentException("Errores.AlumnoApellidoObligatorio", nameof(apellido));
            if (string.IsNullOrWhiteSpace(email) || !email.Contains("@")) throw new ArgumentException("Errores.AlumnoEmailInvalido", nameof(email));
            if (string.IsNullOrWhiteSpace(telefono)) throw new ArgumentException("Errores.AlumnoTelefonoObligatorio", nameof(telefono));
        }

        private static string Normalizar(string value) { return (value ?? string.Empty).Trim(); }
        private static string NormalizarTelefono(string value) { return Normalizar(value).Replace(" ", string.Empty).Replace("-", string.Empty); }
    }
}

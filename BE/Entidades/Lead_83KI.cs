using System;

namespace BE.Entidades
{
    public class Lead_83KI
    {
        public const string EstadoActivo = "activo";

        public int IdLead { get; private set; }
        public string DNI { get; private set; }
        public string Nombre { get; private set; }
        public string Apellido { get; private set; }
        public string Email { get; private set; }
        public string Telefono { get; private set; }
        public DateTime FechaAlta { get; private set; }
        public string Estado { get; private set; }
        public string DVH { get; private set; }

        private Lead_83KI() { }

        public string NombreCompleto { get { return (Nombre + " " + Apellido).Trim(); } }

        public static Lead_83KI Crear(string dni, string nombre, string apellido, string email, string telefono)
        {
            Validar(nombre, apellido, email, telefono);
            return new Lead_83KI
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

        public static Lead_83KI ReconstruirDesdePersistencia(int idLead, string dni, string nombre, string apellido, string email, string telefono, DateTime fechaAlta, string estado, string dvh)
        {
            var lead = Crear(dni, nombre, apellido, email, telefono);
            lead.IdLead = idLead;
            lead.FechaAlta = fechaAlta;
            lead.Estado = string.IsNullOrWhiteSpace(estado) ? EstadoActivo : estado;
            lead.DVH = dvh ?? string.Empty;
            return lead;
        }

        private static void Validar(string nombre, string apellido, string email, string telefono)
        {
            if (string.IsNullOrWhiteSpace(nombre)) throw new ArgumentException("Errores.LeadNombreObligatorio", nameof(nombre));
            if (string.IsNullOrWhiteSpace(apellido)) throw new ArgumentException("Errores.LeadApellidoObligatorio", nameof(apellido));
            if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Errores.LeadEmailObligatorio", nameof(email));
            if (string.IsNullOrWhiteSpace(telefono)) throw new ArgumentException("Errores.LeadTelefonoObligatorio", nameof(telefono));
            if (!email.Contains("@")) throw new ArgumentException("Errores.LeadEmailInvalido", nameof(email));
        }

        private static string Normalizar(string value) { return (value ?? string.Empty).Trim(); }
        private static string NormalizarTelefono(string value) { return Normalizar(value).Replace(" ", string.Empty).Replace("-", string.Empty); }
    }
}

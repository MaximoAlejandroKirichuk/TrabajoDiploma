using System;

namespace BE.Entidades
{
    public class ConsultaLead_83KI
    {
        public const string EstadoRegistrada = "registrada";

        public int IdConsultaLead { get; private set; }
        public string Codigo { get; private set; }
        public int IdLead { get; private set; }
        public int IdComision { get; private set; }
        public DateTime FechaConsulta { get; private set; }
        public string MedioContacto { get; private set; }
        public string Motivo { get; private set; }
        public string Observaciones { get; private set; }
        public string Estado { get; private set; }
        public string DVH { get; private set; }

        private ConsultaLead_83KI() { }

        public static ConsultaLead_83KI Crear(int idLead, int idComision, string medioContacto, string motivo, string observaciones)
        {
            Validar(idLead, idComision, medioContacto, motivo);
            return new ConsultaLead_83KI
            {
                IdLead = idLead,
                IdComision = idComision,
                FechaConsulta = DateTime.Now,
                MedioContacto = Normalizar(medioContacto),
                Motivo = Normalizar(motivo),
                Observaciones = Normalizar(observaciones),
                Estado = EstadoRegistrada,
                Codigo = string.Empty,
                DVH = string.Empty
            };
        }

        public static ConsultaLead_83KI ReconstruirDesdePersistencia(int idConsultaLead, string codigo, int idLead, int idComision, DateTime fechaConsulta, string medioContacto, string motivo, string observaciones, string estado, string dvh)
        {
            var consulta = Crear(idLead, idComision, medioContacto, motivo, observaciones);
            consulta.IdConsultaLead = idConsultaLead;
            consulta.Codigo = codigo ?? string.Empty;
            consulta.FechaConsulta = fechaConsulta;
            consulta.Estado = string.IsNullOrWhiteSpace(estado) ? EstadoRegistrada : estado;
            consulta.DVH = dvh ?? string.Empty;
            return consulta;
        }

        private static void Validar(int idLead, int idComision, string medioContacto, string motivo)
        {
            if (idLead <= 0) throw new ArgumentException("Errores.LeadObligatorio", nameof(idLead));
            if (idComision <= 0) throw new ArgumentException("Errores.ComisionObligatoria", nameof(idComision));
            if (string.IsNullOrWhiteSpace(medioContacto)) throw new ArgumentException("Errores.MedioContactoObligatorio", nameof(medioContacto));
            if (string.IsNullOrWhiteSpace(motivo)) throw new ArgumentException("Errores.MotivoConsultaObligatorio", nameof(motivo));
        }

        private static string Normalizar(string value) { return (value ?? string.Empty).Trim(); }
    }
}

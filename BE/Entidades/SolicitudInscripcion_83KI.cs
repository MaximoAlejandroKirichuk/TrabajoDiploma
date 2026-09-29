using System;

namespace BE.Entidades
{
    public class SolicitudInscripcion_83KI
    {
        public const string EstadoPendiente = "pendiente";
        public const string EstadoConfirmada = "confirmada";
        public const string EstadoActiva = "activa";
        public const string EstadoCancelada = "cancelada";
        public const string EstadoAnulada = "anulada";

        public int IdSolicitudInscripcion { get; private set; }
        public string Codigo { get; private set; }
        public int IdAlumno { get; private set; }
        public int? IdLeadOrigen { get; private set; }
        public int IdComision { get; private set; }
        public int IdPlanDePago { get; private set; }
        public decimal RecargoPlanSnapshot { get; private set; }
        public DateTime FechaSolicitud { get; private set; }
        public string Estado { get; private set; }
        public string Observaciones { get; private set; }
        public string DVH { get; private set; }

        private SolicitudInscripcion_83KI() { }

        public static SolicitudInscripcion_83KI ReconstruirDesdePersistencia(int idSolicitudInscripcion, string codigo, int idAlumno, int? idLeadOrigen, int idComision, int idPlanDePago, decimal recargoPlanSnapshot, DateTime fechaSolicitud, string estado, string observaciones, string dvh)
        {
            if (idSolicitudInscripcion <= 0) throw new ArgumentException("Errores.SolicitudInscripcionNoRegistrada", nameof(idSolicitudInscripcion));
            if (idAlumno <= 0) throw new ArgumentException("Errores.AlumnoObligatorio", nameof(idAlumno));
            if (idComision <= 0) throw new ArgumentException("Errores.ComisionObligatoria", nameof(idComision));
            if (idPlanDePago <= 0) throw new ArgumentException("Errores.PlanDePagoObligatorio", nameof(idPlanDePago));

            return new SolicitudInscripcion_83KI
            {
                IdSolicitudInscripcion = idSolicitudInscripcion,
                Codigo = codigo ?? string.Empty,
                IdAlumno = idAlumno,
                IdLeadOrigen = idLeadOrigen,
                IdComision = idComision,
                IdPlanDePago = idPlanDePago,
                RecargoPlanSnapshot = recargoPlanSnapshot,
                FechaSolicitud = fechaSolicitud,
                Estado = string.IsNullOrWhiteSpace(estado) ? EstadoPendiente : estado,
                Observaciones = observaciones ?? string.Empty,
                DVH = dvh ?? string.Empty
            };
        }
    }
}

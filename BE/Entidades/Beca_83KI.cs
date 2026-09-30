using System;

namespace BE.Entidades
{
    public class Beca_83KI
    {
        public const string EstadoAprobada = "Aprobada";
        public const string EstadoDenegada = "Denegada";

        public int IdBeca { get; private set; }
        public int IdAlumno { get; private set; }
        public int IdComision { get; private set; }
        public int IdSolicitudInscripcion { get; private set; }
        public string TipoBeneficio { get; private set; }
        public DateTime FechaSolicitud { get; private set; }
        public string EstadoBeca { get; private set; }
        public string MotivoDecision { get; private set; }
        public string DVH { get; private set; }

        private Beca_83KI() { }

        public static Beca_83KI Crear(int idAlumno, int idComision, int idSolicitudInscripcion, string tipoBeneficio, DateTime fechaSolicitud, string estadoBeca, string motivoDecision)
        {
            Validar(idAlumno, idComision, idSolicitudInscripcion, tipoBeneficio, fechaSolicitud, estadoBeca, motivoDecision);
            return new Beca_83KI
            {
                IdAlumno = idAlumno,
                IdComision = idComision,
                IdSolicitudInscripcion = idSolicitudInscripcion,
                TipoBeneficio = Normalizar(tipoBeneficio),
                FechaSolicitud = fechaSolicitud,
                EstadoBeca = Normalizar(estadoBeca),
                MotivoDecision = Normalizar(motivoDecision),
                DVH = string.Empty
            };
        }

        public static Beca_83KI ReconstruirDesdePersistencia(int idBeca, int idAlumno, int idComision, int idSolicitudInscripcion, string tipoBeneficio, DateTime fechaSolicitud, string estadoBeca, string motivoDecision, string dvh)
        {
            if (idBeca <= 0) throw new ArgumentException("Errores.BecaNoRegistrada", nameof(idBeca));
            var beca = Crear(idAlumno, idComision, idSolicitudInscripcion, tipoBeneficio, fechaSolicitud, estadoBeca, motivoDecision);
            beca.IdBeca = idBeca;
            beca.DVH = dvh ?? string.Empty;
            return beca;
        }

        private static void Validar(int idAlumno, int idComision, int idSolicitudInscripcion, string tipoBeneficio, DateTime fechaSolicitud, string estadoBeca, string motivoDecision)
        {
            if (idAlumno <= 0) throw new ArgumentException("Errores.AlumnoObligatorio", nameof(idAlumno));
            if (idComision <= 0) throw new ArgumentException("Errores.ComisionObligatoria", nameof(idComision));
            if (idSolicitudInscripcion <= 0) throw new ArgumentException("Errores.SolicitudInscripcionNoRegistrada", nameof(idSolicitudInscripcion));
            if (string.IsNullOrWhiteSpace(tipoBeneficio)) throw new ArgumentException("Errores.BecaTipoBeneficioObligatorio", nameof(tipoBeneficio));
            if (fechaSolicitud == default(DateTime)) throw new ArgumentException("Errores.BecaFechaSolicitudObligatoria", nameof(fechaSolicitud));
            if (string.IsNullOrWhiteSpace(estadoBeca) || (!string.Equals(estadoBeca, EstadoAprobada, StringComparison.OrdinalIgnoreCase) && !string.Equals(estadoBeca, EstadoDenegada, StringComparison.OrdinalIgnoreCase))) throw new ArgumentException("Errores.BecaEstadoInvalido", nameof(estadoBeca));
            if (string.IsNullOrWhiteSpace(motivoDecision)) throw new ArgumentException("Errores.BecaMotivoObligatorio", nameof(motivoDecision));
        }

        private static string Normalizar(string value) { return (value ?? string.Empty).Trim(); }
    }
}

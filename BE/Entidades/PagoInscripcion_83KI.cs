using System;

namespace BE.Entidades
{
    public class PagoInscripcion_83KI
    {
        public const string MetodoTransferencia = "transferencia";
        public const string MetodoTarjeta = "tarjeta";
        public const string MetodoBilleteraVirtual = "billetera virtual";

        public int IdPagoInscripcion { get; private set; }
        public int IdAlumno { get; private set; }
        public int IdSolicitudInscripcion { get; private set; }
        public int IdCuota { get; private set; }
        public string MetodoPago { get; private set; }
        public decimal MontoPagado { get; private set; }
        public string NumeroReferencia { get; private set; }
        public DateTime FechaPago { get; private set; }
        public string DVH { get; private set; }

        private PagoInscripcion_83KI() { }

        public static PagoInscripcion_83KI Crear(int idAlumno, int idSolicitudInscripcion, int idCuota, string metodoPago, decimal montoPagado, string numeroReferencia, DateTime fechaPago)
        {
            Validar(idAlumno, idSolicitudInscripcion, idCuota, metodoPago, montoPagado, numeroReferencia, fechaPago);
            return new PagoInscripcion_83KI
            {
                IdAlumno = idAlumno,
                IdSolicitudInscripcion = idSolicitudInscripcion,
                IdCuota = idCuota,
                MetodoPago = Normalizar(metodoPago).ToLowerInvariant(),
                MontoPagado = montoPagado,
                NumeroReferencia = Normalizar(numeroReferencia),
                FechaPago = fechaPago,
                DVH = string.Empty
            };
        }

        public static PagoInscripcion_83KI ReconstruirDesdePersistencia(int idPagoInscripcion, int idAlumno, int idSolicitudInscripcion, int idCuota, string metodoPago, decimal montoPagado, string numeroReferencia, DateTime fechaPago, string dvh)
        {
            if (idPagoInscripcion <= 0) throw new ArgumentException("Errores.PagoNoRegistrado", nameof(idPagoInscripcion));
            var pago = Crear(idAlumno, idSolicitudInscripcion, idCuota, metodoPago, montoPagado, numeroReferencia, fechaPago);
            pago.IdPagoInscripcion = idPagoInscripcion;
            pago.DVH = dvh ?? string.Empty;
            return pago;
        }

        public static bool MetodoValido(string metodoPago)
        {
            return string.Equals(metodoPago, MetodoTransferencia, StringComparison.OrdinalIgnoreCase)
                || string.Equals(metodoPago, MetodoTarjeta, StringComparison.OrdinalIgnoreCase)
                || string.Equals(metodoPago, MetodoBilleteraVirtual, StringComparison.OrdinalIgnoreCase);
        }

        private static void Validar(int idAlumno, int idSolicitudInscripcion, int idCuota, string metodoPago, decimal montoPagado, string numeroReferencia, DateTime fechaPago)
        {
            if (idAlumno <= 0) throw new ArgumentException("Errores.AlumnoObligatorio", nameof(idAlumno));
            if (idSolicitudInscripcion <= 0) throw new ArgumentException("Errores.SolicitudInscripcionNoRegistrada", nameof(idSolicitudInscripcion));
            if (idCuota <= 0) throw new ArgumentException("Errores.PagoCuotaObligatoria", nameof(idCuota));
            if (montoPagado <= 0) throw new ArgumentException("Errores.MontoMatriculaInvalido", nameof(montoPagado));
            if (!MetodoValido(Normalizar(metodoPago))) throw new ArgumentException("Errores.PagoMetodoInvalido", nameof(metodoPago));
            if (string.IsNullOrWhiteSpace(numeroReferencia)) throw new ArgumentException("Errores.PagoReferenciaObligatoria", nameof(numeroReferencia));
            if (fechaPago == default(DateTime)) throw new ArgumentException("Errores.PagoFechaObligatoria", nameof(fechaPago));
        }

        private static string Normalizar(string value) { return (value ?? string.Empty).Trim(); }
    }
}

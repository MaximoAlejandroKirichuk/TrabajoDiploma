using System;

namespace BE.Entidades
{
    public class Cuota_83KI
    {
        public const string EstadoPendiente = "pendiente";
        public const string EstadoPagada = "pagada";

        public int IdCuota { get; private set; }
        public int IdSolicitudInscripcion { get; private set; }
        public int NumeroCuota { get; private set; }
        public decimal MontoOriginal { get; private set; }
        public decimal BalanceAdeudado { get; private set; }
        public DateTime FechaVencimiento { get; private set; }
        public string Estado { get; private set; }
        public string DVH { get; private set; }

        private Cuota_83KI() { }

        public static Cuota_83KI ReconstruirDesdePersistencia(int idCuota, int idSolicitudInscripcion, int numeroCuota, decimal montoOriginal, decimal balanceAdeudado, DateTime fechaVencimiento, string estado, string dvh)
        {
            if (idCuota <= 0) throw new ArgumentException("Errores.CuotaNoRegistrada", nameof(idCuota));
            if (idSolicitudInscripcion <= 0) throw new ArgumentException("Errores.SolicitudInscripcionNoRegistrada", nameof(idSolicitudInscripcion));
            if (numeroCuota <= 0) throw new ArgumentException("Errores.NumeroCuotaInvalido", nameof(numeroCuota));
            if (montoOriginal <= 0) throw new ArgumentException("Errores.MontoMatriculaInvalido", nameof(montoOriginal));
            if (balanceAdeudado < 0) throw new ArgumentException("Errores.BalanceCuotaInvalido", nameof(balanceAdeudado));

            return new Cuota_83KI
            {
                IdCuota = idCuota,
                IdSolicitudInscripcion = idSolicitudInscripcion,
                NumeroCuota = numeroCuota,
                MontoOriginal = montoOriginal,
                BalanceAdeudado = balanceAdeudado,
                FechaVencimiento = fechaVencimiento,
                Estado = string.IsNullOrWhiteSpace(estado) ? EstadoPendiente : estado,
                DVH = dvh ?? string.Empty
            };
        }
    }
}

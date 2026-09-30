using System;

namespace BE.Entidades
{
    public class ActaCierreComision_83KI
    {
        public int IdActaCierreComision { get; private set; }
        public int IdComision { get; private set; }
        public DateTime FechaCierre { get; private set; }
        public string NumeroActa { get; private set; }
        public string Motivo { get; private set; }
        public string Observaciones { get; private set; }
        public string UsuarioRegistro { get; private set; }
        public DateTime FechaRegistro { get; private set; }
        public string DVH { get; private set; }

        private ActaCierreComision_83KI() { }

        public static ActaCierreComision_83KI ReconstruirDesdePersistencia(int idActaCierreComision, int idComision, DateTime fechaCierre, string numeroActa, string motivo, string observaciones, string usuarioRegistro, DateTime fechaRegistro, string dvh)
        {
            if (idActaCierreComision <= 0) throw new ArgumentException("Errores.ActaCierreNoRegistrada", nameof(idActaCierreComision));
            if (idComision <= 0) throw new ArgumentException("Errores.ComisionObligatoria", nameof(idComision));
            return new ActaCierreComision_83KI
            {
                IdActaCierreComision = idActaCierreComision,
                IdComision = idComision,
                FechaCierre = fechaCierre,
                NumeroActa = numeroActa ?? string.Empty,
                Motivo = motivo ?? string.Empty,
                Observaciones = observaciones ?? string.Empty,
                UsuarioRegistro = usuarioRegistro ?? string.Empty,
                FechaRegistro = fechaRegistro,
                DVH = dvh ?? string.Empty
            };
        }
    }
}

using System;

namespace BE.Entidades
{
    public class AltaOficialComision_83KI
    {
        public int IdAltaOficialComision { get; private set; }
        public int IdComision { get; private set; }
        public DateTime FechaAlta { get; private set; }
        public string NumeroActa { get; private set; }
        public string Observaciones { get; private set; }
        public string UsuarioRegistro { get; private set; }
        public DateTime FechaRegistro { get; private set; }
        public string DVH { get; private set; }

        private AltaOficialComision_83KI() { }

        public static AltaOficialComision_83KI ReconstruirDesdePersistencia(int idAltaOficialComision, int idComision, DateTime fechaAlta, string numeroActa, string observaciones, string usuarioRegistro, DateTime fechaRegistro, string dvh)
        {
            if (idAltaOficialComision <= 0) throw new ArgumentException("Errores.AltaOficialNoRegistrada", nameof(idAltaOficialComision));
            if (idComision <= 0) throw new ArgumentException("Errores.ComisionObligatoria", nameof(idComision));
            return new AltaOficialComision_83KI
            {
                IdAltaOficialComision = idAltaOficialComision,
                IdComision = idComision,
                FechaAlta = fechaAlta,
                NumeroActa = numeroActa ?? string.Empty,
                Observaciones = observaciones ?? string.Empty,
                UsuarioRegistro = usuarioRegistro ?? string.Empty,
                FechaRegistro = fechaRegistro,
                DVH = dvh ?? string.Empty
            };
        }
    }
}

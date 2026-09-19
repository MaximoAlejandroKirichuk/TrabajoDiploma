using System;

namespace BE.Entidades
{
    public class Comision_83KI
    {
        public const string EstadoPreapertura = "preapertura";

        public int IdComision { get; private set; }
        public string Codigo { get; private set; }
        public int IdCurso { get; private set; }
        public int IdProfesor { get; private set; }
        public DayOfWeek DiaSemana { get; private set; }
        public TimeSpan HoraInicio { get; private set; }
        public TimeSpan HoraFin { get; private set; }
        public int CupoMinimo { get; private set; }
        public int CupoMaximo { get; private set; }
        public DateTime FechaLimitePago { get; private set; }
        public string Estado { get; private set; }
        public string DVH { get; private set; }

        private Comision_83KI() { }

        public static Comision_83KI CrearPreapertura(int idCurso, int idProfesor, DayOfWeek diaSemana, TimeSpan horaInicio, TimeSpan horaFin, int cupoMinimo, int cupoMaximo, DateTime fechaLimitePago)
        {
            Validar(idCurso, idProfesor, horaInicio, horaFin, cupoMinimo, cupoMaximo, fechaLimitePago);
            return new Comision_83KI
            {
                IdCurso = idCurso,
                IdProfesor = idProfesor,
                DiaSemana = diaSemana,
                HoraInicio = horaInicio,
                HoraFin = horaFin,
                CupoMinimo = cupoMinimo,
                CupoMaximo = cupoMaximo,
                FechaLimitePago = fechaLimitePago.Date,
                Estado = EstadoPreapertura,
                Codigo = string.Empty,
                DVH = string.Empty
            };
        }

        public static Comision_83KI ReconstruirDesdePersistencia(int idComision, string codigo, int idCurso, int idProfesor, DayOfWeek diaSemana, TimeSpan horaInicio, TimeSpan horaFin, int cupoMinimo, int cupoMaximo, DateTime fechaLimitePago, string estado, string dvh)
        {
            var comision = CrearPreapertura(idCurso, idProfesor, diaSemana, horaInicio, horaFin, cupoMinimo, cupoMaximo, fechaLimitePago);
            comision.IdComision = idComision;
            comision.Codigo = codigo ?? string.Empty;
            comision.Estado = estado ?? EstadoPreapertura;
            comision.DVH = dvh ?? string.Empty;
            return comision;
        }

        private static void Validar(int idCurso, int idProfesor, TimeSpan horaInicio, TimeSpan horaFin, int cupoMinimo, int cupoMaximo, DateTime fechaLimitePago)
        {
            if (idCurso <= 0) throw new ArgumentException("Errores.CursoObligatorio", nameof(idCurso));
            if (idProfesor <= 0) throw new ArgumentException("Errores.ProfesorObligatorio", nameof(idProfesor));
            if (horaInicio >= horaFin) throw new ArgumentException("Errores.HorarioInvalido");
            if (cupoMinimo <= 0) throw new ArgumentException("Errores.CupoMinimoInvalido", nameof(cupoMinimo));
            if (cupoMaximo < cupoMinimo) throw new ArgumentException("Errores.CupoMaximoInvalido", nameof(cupoMaximo));
            if (fechaLimitePago.Date < DateTime.Today) throw new ArgumentException("Errores.FechaLimitePagoInvalida", nameof(fechaLimitePago));
        }
    }
}

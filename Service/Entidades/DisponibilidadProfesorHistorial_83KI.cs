using System;

namespace Service.Entidades
{
    public sealed class DisponibilidadProfesorHistorial_83KI
    {
        public long IdVersion { get; private set; }
        public int IdDisponibilidadProfesor { get; private set; }
        public int IdProfesor { get; private set; }
        public DayOfWeek DiaSemana { get; private set; }
        public TimeSpan HoraInicio { get; private set; }
        public TimeSpan HoraFin { get; private set; }
        public bool EstadoActivo { get; private set; }
        public string DVH { get; private set; }
        public bool Act { get; private set; }
        public DateTime FechaRegistro { get; private set; }
        public string TipoOperacion { get; private set; }
        public string Usuario { get; private set; }

        public DisponibilidadProfesorHistorial_83KI(long idVersion, int idDisponibilidadProfesor, int idProfesor, DayOfWeek diaSemana, TimeSpan horaInicio, TimeSpan horaFin, bool estadoActivo, string dvh, bool activa, DateTime fechaRegistro, string tipoOperacion, string usuario)
        {
            IdVersion = idVersion;
            IdDisponibilidadProfesor = idDisponibilidadProfesor;
            IdProfesor = idProfesor;
            DiaSemana = diaSemana;
            HoraInicio = horaInicio;
            HoraFin = horaFin;
            EstadoActivo = estadoActivo;
            DVH = dvh;
            Act = activa;
            FechaRegistro = fechaRegistro;
            TipoOperacion = tipoOperacion;
            Usuario = usuario;
        }
    }
}

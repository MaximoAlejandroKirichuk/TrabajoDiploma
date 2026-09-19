using System;

namespace BE.Entidades
{
    public class DisponibilidadProfesor_83KI
    {
        public int IdDisponibilidadProfesor { get; private set; }
        public int IdProfesor { get; private set; }
        public DayOfWeek DiaSemana { get; private set; }
        public TimeSpan HoraInicio { get; private set; }
        public TimeSpan HoraFin { get; private set; }
        public bool EstadoActivo { get; private set; }
        public string DVH { get; private set; }

        private DisponibilidadProfesor_83KI() { }

        public static DisponibilidadProfesor_83KI CrearNueva(int idProfesor, DayOfWeek diaSemana, TimeSpan horaInicio, TimeSpan horaFin)
        {
            if (idProfesor <= 0) throw new ArgumentException("Errores.ProfesorObligatorio", nameof(idProfesor));
            if (horaInicio >= horaFin) throw new ArgumentException("Errores.HorarioInvalido");
            return new DisponibilidadProfesor_83KI
            {
                IdProfesor = idProfesor,
                DiaSemana = diaSemana,
                HoraInicio = horaInicio,
                HoraFin = horaFin,
                EstadoActivo = true,
                DVH = string.Empty
            };
        }

        public static DisponibilidadProfesor_83KI ReconstruirDesdePersistencia(int idDisponibilidadProfesor, int idProfesor, DayOfWeek diaSemana, TimeSpan horaInicio, TimeSpan horaFin, bool estadoActivo, string dvh)
        {
            var disponibilidad = CrearNueva(idProfesor, diaSemana, horaInicio, horaFin);
            disponibilidad.IdDisponibilidadProfesor = idDisponibilidadProfesor;
            disponibilidad.EstadoActivo = estadoActivo;
            disponibilidad.DVH = dvh ?? string.Empty;
            return disponibilidad;
        }

        public bool Cubre(DayOfWeek diaSemana, TimeSpan horaInicio, TimeSpan horaFin)
        {
            return EstadoActivo && DiaSemana == diaSemana && HoraInicio <= horaInicio && HoraFin >= horaFin;
        }

        public void ModificarHorario(DayOfWeek diaSemana, TimeSpan horaInicio, TimeSpan horaFin)
        {
            if (horaInicio >= horaFin) throw new ArgumentException("Errores.HorarioInvalido");
            DiaSemana = diaSemana;
            HoraInicio = horaInicio;
            HoraFin = horaFin;
        }

        public void Activar()
        {
            EstadoActivo = true;
        }

        public void Desactivar()
        {
            EstadoActivo = false;
        }
    }
}

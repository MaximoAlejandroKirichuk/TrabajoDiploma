using System;

namespace Service.DTOs
{
    public sealed class DisponibilidadProfesorHistorial_83KI
    {
        public long IdVersion { get; set; }
        public int IdDisponibilidadProfesor { get; set; }
        public int IdProfesor { get; set; }
        public string DiaSemana { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }
        public bool EstadoActivo { get; set; }
        public string DVH { get; set; }
        public bool Act { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string TipoOperacion { get; set; }
        public string Usuario { get; set; }
    }
}

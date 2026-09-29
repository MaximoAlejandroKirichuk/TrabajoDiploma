using System;

namespace Service.DTOs
{
    public class ComisionListado_83KI
    {
        public int IdComision { get; set; }
        public string Codigo { get; set; }
        public int IdCurso { get; set; }
        public string Curso { get; set; }
        public int IdProfesor { get; set; }
        public string Profesor { get; set; }
        public DayOfWeek DiaSemana { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }
        public int CupoMinimo { get; set; }
        public int CupoMaximo { get; set; }
        public DateTime FechaLimitePago { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public decimal ArancelBase { get; set; }
        public string Estado { get; set; }

        public bool Eliminada
        {
            get { return string.Equals(Estado, "eliminada", StringComparison.OrdinalIgnoreCase); }
        }
    }
}

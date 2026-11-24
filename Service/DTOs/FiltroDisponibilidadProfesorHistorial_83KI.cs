using System;

namespace Service.DTOs
{
    public sealed class FiltroDisponibilidadProfesorHistorial_83KI
    {
        public int IdProfesor { get; private set; }
        public int? IdDisponibilidadProfesor { get; private set; }
        public DateTime? FechaDesde { get; private set; }
        public DateTime? FechaHasta { get; private set; }

        public FiltroDisponibilidadProfesorHistorial_83KI(int idProfesor, int? idDisponibilidadProfesor, DateTime? fechaDesde, DateTime? fechaHasta)
        {
            if (idProfesor <= 0) throw new ArgumentException("Errores.ProfesorObligatorio", nameof(idProfesor));
            if (fechaDesde.HasValue && fechaHasta.HasValue && fechaDesde.Value.Date > fechaHasta.Value.Date)
                throw new ArgumentException("Errores.RangoFechasComisionInvalido");
            IdProfesor = idProfesor;
            IdDisponibilidadProfesor = idDisponibilidadProfesor;
            FechaDesde = fechaDesde?.Date;
            FechaHasta = fechaHasta?.Date;
        }
    }
}

using Service.DTOs;
using System;
using System.Collections.Generic;

namespace Service.Interfaces
{
    public interface IDisponibilidadProfesorHistorialBLL_83KI
    {
        IEnumerable<Service.DTOs.DisponibilidadProfesorHistorial_83KI> ObtenerHistorial(int idProfesor, int? idDisponibilidadProfesor, DateTime? fechaDesde, DateTime? fechaHasta);
        void ActivarVersion(long idVersion, int idProfesor);
    }
}

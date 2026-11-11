using BE.DTOs.Disponibilidades;
using System;
using System.Collections.Generic;

namespace Service.Interfaces
{
    public interface IDisponibilidadProfesorHistorialBLL_83KI
    {
        IEnumerable<BE.DTOs.Disponibilidades.DisponibilidadProfesorHistorial_83KI> ObtenerHistorial(int idProfesor, int? idDisponibilidadProfesor, DateTime? fechaDesde, DateTime? fechaHasta);
        void ActivarVersion(long idVersion, int idProfesor);
    }
}

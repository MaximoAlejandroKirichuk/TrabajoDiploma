using BE.Entidades;
using Service.DTOs;
using System;
using System.Collections.Generic;

namespace Service.Interfaces
{
    public interface IDisponibilidadProfesorDAL_83KI
    {
        IEnumerable<DisponibilidadProfesor_83KI> ObtenerPorProfesor(int idProfesor);
        int Insertar(DisponibilidadProfesor_83KI disponibilidad, string usuario = null);
        void Actualizar(DisponibilidadProfesor_83KI disponibilidad, string usuario = null);
        IEnumerable<Service.Entidades.DisponibilidadProfesorHistorial_83KI> ListarHistorial(FiltroDisponibilidadProfesorHistorial_83KI filtro);
        void ActivarVersion(long idVersion, string usuario = null);
    }
}

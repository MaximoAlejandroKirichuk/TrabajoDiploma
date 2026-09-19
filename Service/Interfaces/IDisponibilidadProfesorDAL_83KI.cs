using BE.Entidades;
using System;
using System.Collections.Generic;

namespace Service.Interfaces
{
    public interface IDisponibilidadProfesorDAL_83KI
    {
        IEnumerable<DisponibilidadProfesor_83KI> ObtenerPorProfesor(int idProfesor);
        int Insertar(DisponibilidadProfesor_83KI disponibilidad);
        void Actualizar(DisponibilidadProfesor_83KI disponibilidad);
    }
}

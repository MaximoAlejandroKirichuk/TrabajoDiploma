using BE.Entidades;
using System;
using System.Collections.Generic;

namespace Service.Interfaces
{
    public interface IComisionDAL_83KI
    {
        IEnumerable<Curso_83KI> ObtenerCursosActivosParaPreapertura();
        IEnumerable<Profesor_83KI> ObtenerProfesoresDisponibles(int idCurso, DayOfWeek diaSemana, TimeSpan horaInicio, TimeSpan horaFin);
        bool ValidarCursoProfesorDisponibilidad(int idCurso, int idProfesor, DayOfWeek diaSemana, TimeSpan horaInicio, TimeSpan horaFin);
        Comision_83KI RegistrarPreapertura(Comision_83KI comision);
    }
}

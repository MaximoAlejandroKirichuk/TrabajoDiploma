using BE.Entidades;
using System;
using System.Collections.Generic;

namespace Service.Interfaces
{
    public interface IGestorComision_83KI
    {
        IEnumerable<Curso_83KI> ObtenerCursosActivosParaPreapertura();
        IEnumerable<Profesor_83KI> ObtenerProfesoresDisponibles(int idCurso, DayOfWeek diaSemana, TimeSpan horaInicio, TimeSpan horaFin);
        string RegistrarPreapertura(int idCurso, int idProfesor, DayOfWeek diaSemana, TimeSpan horaInicio, TimeSpan horaFin, int cupoMinimo, int cupoMaximo, DateTime fechaLimitePago);
    }
}

using BE.Entidades;
using System;
using System.Collections.Generic;

namespace Service.Interfaces
{
    public interface IGestorProfesor_83KI
    {
        IEnumerable<Profesor_83KI> ObtenerProfesores();
        IEnumerable<Profesor_83KI> ObtenerProfesoresActivos();
        Profesor_83KI ObtenerPorId(int idProfesor);
        void CrearProfesor(string dni, string nombre, string apellido, string email);
        void ModificarProfesor(int idProfesor, string nombre, string apellido, string email);
        void ActivarProfesor(int idProfesor);
        void DesactivarProfesor(int idProfesor);
        IEnumerable<DisponibilidadProfesor_83KI> ObtenerDisponibilidades(int idProfesor);
        void AgregarDisponibilidad(int idProfesor, DayOfWeek diaSemana, TimeSpan horaInicio, TimeSpan horaFin);
        void ModificarDisponibilidad(int idDisponibilidadProfesor, int idProfesor, DayOfWeek diaSemana, TimeSpan horaInicio, TimeSpan horaFin);
        void ActivarDisponibilidad(int idDisponibilidadProfesor, int idProfesor);
        void DesactivarDisponibilidad(int idDisponibilidadProfesor, int idProfesor);
    }
}

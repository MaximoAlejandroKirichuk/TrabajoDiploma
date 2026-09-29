using BE.Entidades;
using Service.DTOs;
using System;
using System.Collections.Generic;

namespace Service.Interfaces
{
    public interface IComisionDAL_83KI
    {
        IEnumerable<Curso_83KI> ObtenerCursosActivosParaPreapertura();
        IEnumerable<Profesor_83KI> ObtenerProfesoresActivosParaFiltro();
        IEnumerable<Profesor_83KI> ObtenerProfesoresDisponibles(int idCurso, DayOfWeek diaSemana, TimeSpan horaInicio, TimeSpan horaFin);
        IEnumerable<PlanDePago_83KI> ObtenerPlanesDePagoActivos();
        bool ValidarCursoProfesorDisponibilidad(int idCurso, int idProfesor, DayOfWeek diaSemana, TimeSpan horaInicio, TimeSpan horaFin);
        bool ExisteSolapamientoComision(int idProfesor, DayOfWeek diaSemana, TimeSpan horaInicio, TimeSpan horaFin, int? idComisionIgnorar = null);
        Comision_83KI RegistrarPreapertura(Comision_83KI comision);
        IEnumerable<ComisionListado_83KI> ListarComisiones(FiltroComision_83KI filtro);
        Comision_83KI ObtenerComision(int idComision);
        void ModificarComision(Comision_83KI comision);
        void EliminarComision(int idComision);
    }
}

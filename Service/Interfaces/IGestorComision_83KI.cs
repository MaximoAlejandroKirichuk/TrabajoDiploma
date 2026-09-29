using BE.Entidades;
using Service.DTOs;
using System;
using System.Collections.Generic;

namespace Service.Interfaces
{
    public interface IGestorComision_83KI
    {
        IEnumerable<Curso_83KI> ObtenerCursosActivosParaPreapertura();
        IEnumerable<Profesor_83KI> ObtenerProfesoresActivosParaFiltro();
        IEnumerable<Profesor_83KI> ObtenerProfesoresDisponibles(int idCurso, DayOfWeek diaSemana, TimeSpan horaInicio, TimeSpan horaFin);
        IEnumerable<PlanDePago_83KI> ObtenerPlanesDePagoActivos();
        string RegistrarPreapertura(int idCurso, int idProfesor, DayOfWeek diaSemana, TimeSpan horaInicio, TimeSpan horaFin, int cupoMinimo, int cupoMaximo, DateTime fechaLimitePago, DateTime fechaInicio, DateTime fechaFin, decimal arancelBase, int idPlanDePago, decimal recargoPlanSnapshot);
        IEnumerable<ComisionListado_83KI> ListarComisiones(FiltroComision_83KI filtro);
        Comision_83KI ObtenerComision(int idComision);
        void ModificarComision(Comision_83KI comision);
        void EliminarComision(int idComision);
    }
}

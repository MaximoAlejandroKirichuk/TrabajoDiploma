using System;
using System.Collections.Generic;
using Service.DTOs;

namespace Service.Interfaces
{
    public interface IResultadoEvaluacionDAL_83KI
    {
        List<AlumnoEvaluacionListado_83KI> BuscarAlumnosHabilitados(int idComision, DateTime fechaEvaluacion);
        bool ExisteResultado(int idInscripcion, DateTime fechaEvaluacion);
        void RegistrarResultados(ResultadoEvaluacionRequest_83KI request);
    }
}

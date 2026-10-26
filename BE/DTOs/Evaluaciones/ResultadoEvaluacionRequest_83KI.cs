using System;
using System.Collections.Generic;

namespace BE.DTOs.Evaluaciones
{
    public class ResultadoEvaluacionRequest_83KI
    {
        public int IdComision { get; set; }
        public DateTime FechaEvaluacion { get; set; }
        public List<ResultadoAlumnoRequest_83KI> Resultados { get; set; } = new List<ResultadoAlumnoRequest_83KI>();
        public bool AuditoriaFallida { get; set; }
    }
}

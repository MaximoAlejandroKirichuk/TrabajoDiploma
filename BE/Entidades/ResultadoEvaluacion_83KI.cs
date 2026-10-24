using System;

namespace BE.Entidades
{
    public class ResultadoEvaluacion_83KI
    {
        public int IdResultadoEvaluacion { get; set; }
        public int IdComision { get; set; }
        public int IdInscripcion { get; set; }
        public DateTime FechaEvaluacion { get; set; }
        public decimal Calificacion { get; set; }
        public string Observaciones { get; set; }
        public string CondicionFinal { get; set; }
        public string DVH { get; set; }

        public string DeterminarCondicionFinal()
        {
            return Calificacion >= 7m ? "Aprobado" : "Desaprobado";
        }
    }
}

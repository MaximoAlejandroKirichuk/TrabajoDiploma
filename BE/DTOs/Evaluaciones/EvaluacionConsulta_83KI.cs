using System;

namespace Service.DTOs
{
    // Instantánea de selección: la interfaz no puede alterar el contexto ni los valores originales.
    public sealed class EvaluacionConsulta_83KI
    {
        public int IdResultado { get; private set; }
        public int IdInscripcion { get; private set; }
        public int IdComision { get; private set; }
        public string Alumno { get; private set; }
        public decimal Calificacion { get; private set; }
        public DateTime FechaEvaluacion { get; private set; }
        public string Observaciones { get; private set; }
        public string CondicionFinal { get; private set; }
        public string DVH { get; private set; }
        public DateTime Inicio { get; private set; }
        public DateTime Fin { get; private set; }
        public DateTime Hoy { get; private set; }
        public bool TieneCertificado { get; private set; }
        public int CantidadNotas { get; private set; }

        public EvaluacionConsulta_83KI(int id, int inscripcion, int comision, string alumno, decimal nota,
            DateTime fecha, string observaciones, string condicion, string dvh, DateTime inicio, DateTime fin,
            DateTime hoy, bool certificado, int cantidad)
        {
            IdResultado = id; IdInscripcion = inscripcion; IdComision = comision; Alumno = alumno;
            Calificacion = nota; FechaEvaluacion = fecha.Date; Observaciones = observaciones ?? "";
            CondicionFinal = condicion; DVH = dvh; Inicio = inicio.Date; Fin = fin.Date; Hoy = hoy.Date;
            TieneCertificado = certificado; CantidadNotas = cantidad;
        }
    }

    public sealed class ModificarEvaluacionRequest_83KI
    {
        public decimal Calificacion { get; set; }
        public DateTime FechaEvaluacion { get; set; }
        public string Observaciones { get; set; }
    }

    public sealed class ModificarEvaluacionResultado_83KI
    {
        public bool Modificado { get; set; }
        public bool AuditoriaFallida { get; set; }
    }
}

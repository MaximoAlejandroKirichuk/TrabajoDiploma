using BE.Entidades;

namespace Service.DTOs
{
    public class DecisionBecaResultado_83KI
    {
        public Beca_83KI Beca { get; set; }
        public Alumno_83KI Alumno { get; set; }
        public SolicitudInscripcion_83KI Solicitud { get; set; }
        public string CodigoComision { get; set; }
        public string CodigoSolicitud { get; set; }
        public bool AuditoriaFallida { get; set; }
    }
}

using BE.Entidades;

namespace Service.DTOs
{
    public class PagoInscripcionResultado_83KI
    {
        public PagoInscripcion_83KI Pago { get; set; }
        public Cuota_83KI Cuota { get; set; }
        public SolicitudInscripcion_83KI Solicitud { get; set; }
        public Alumno_83KI Alumno { get; set; }
        public string CodigoComision { get; set; }
        public string Curso { get; set; }
        public string CodigoSolicitud { get; set; }
        public bool AuditoriaFallida { get; set; }
    }
}

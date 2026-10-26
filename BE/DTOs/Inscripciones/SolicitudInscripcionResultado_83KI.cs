using BE.Entidades;

namespace Service.DTOs
{
    public class SolicitudInscripcionResultado_83KI
    {
        public Alumno_83KI Alumno { get; set; }
        public SolicitudInscripcion_83KI Solicitud { get; set; }
        public Cuota_83KI CuotaInicial { get; set; }
        public string CodigoSolicitud { get; set; }
        public int VacantesDisponibles { get; set; }
    }
}

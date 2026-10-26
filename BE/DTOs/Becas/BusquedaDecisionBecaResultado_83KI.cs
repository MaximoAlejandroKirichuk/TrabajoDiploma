using BE.Entidades;

namespace Service.DTOs
{
    public class BusquedaDecisionBecaResultado_83KI
    {
        public Alumno_83KI Alumno { get; set; }
        public SolicitudInscripcion_83KI Solicitud { get; set; }
        public int IdComision { get; set; }
        public string CodigoComision { get; set; }
        public string Curso { get; set; }

        public int IdAlumno { get { return Alumno != null ? Alumno.IdAlumno : 0; } }
        public int IdSolicitudInscripcion { get { return Solicitud != null ? Solicitud.IdSolicitudInscripcion : 0; } }
        public string Display { get { return string.Format("{0} {1} ({2}) - {3} {4}", Alumno?.Nombre, Alumno?.Apellido, Alumno?.DNI, CodigoComision, Curso); } }
    }
}

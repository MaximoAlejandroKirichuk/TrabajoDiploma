using BE.Entidades;

namespace Service.DTOs
{
    public class BusquedaInscripcionPersonaResultado_83KI
    {
        public string Tipo { get; set; }
        public Alumno_83KI Alumno { get; set; }
        public Lead_83KI Lead { get; set; }
        public int? IdComisionSugerida { get; set; }

        public int? IdAlumno { get { return Alumno != null ? (int?)Alumno.IdAlumno : null; } }
        public int? IdLead { get { return Lead != null ? (int?)Lead.IdLead : null; } }
        public string DNI { get { return Alumno != null ? Alumno.DNI : Lead != null ? Lead.DNI : string.Empty; } }
        public string Nombre { get { return Alumno != null ? Alumno.Nombre : Lead != null ? Lead.Nombre : string.Empty; } }
        public string Apellido { get { return Alumno != null ? Alumno.Apellido : Lead != null ? Lead.Apellido : string.Empty; } }
        public string Email { get { return Alumno != null ? Alumno.Email : Lead != null ? Lead.Email : string.Empty; } }
        public string Telefono { get { return Alumno != null ? Alumno.Telefono : Lead != null ? Lead.Telefono : string.Empty; } }
        public string Display { get { return string.Format("{0} - {1} {2} ({3})", Tipo, Nombre, Apellido, DNI); } }
    }
}

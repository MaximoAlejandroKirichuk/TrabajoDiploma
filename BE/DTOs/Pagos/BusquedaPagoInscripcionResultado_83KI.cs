using BE.Entidades;
using System.Collections.Generic;

namespace BE.DTOs.Pagos
{
    public class BusquedaPagoInscripcionResultado_83KI
    {
        public Alumno_83KI Alumno { get; set; }
        public SolicitudInscripcion_83KI Solicitud { get; set; }
        public List<Cuota_83KI> CuotasPendientes { get; set; }
        public List<Cuota_83KI> Cuotas { get; set; }
        public List<MatriculaPendiente_83KI> MatriculasPendientes { get; set; }
        public string NombrePlanDePago { get; set; }
        public bool MatriculaPagada { get; set; }
        public string CodigoComision { get; set; }
        public string Curso { get; set; }

        public BusquedaPagoInscripcionResultado_83KI()
        {
            CuotasPendientes = new List<Cuota_83KI>();
            Cuotas = new List<Cuota_83KI>();
            MatriculasPendientes = new List<MatriculaPendiente_83KI>();
        }
    }

    public class MatriculaPendiente_83KI
    {
        public SolicitudInscripcion_83KI Solicitud { get; set; }
        public Cuota_83KI Cuota { get; set; }
        public string CodigoComision { get; set; }
        public string Curso { get; set; }
    }
}

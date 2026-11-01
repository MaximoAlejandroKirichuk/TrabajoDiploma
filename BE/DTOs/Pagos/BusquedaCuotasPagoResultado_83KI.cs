using BE.Entidades;
using System.Collections.Generic;

namespace BE.DTOs.Pagos
{
    public class BusquedaCuotasPagoResultado_83KI
    {
        public Alumno_83KI Alumno { get; set; }
        public SolicitudInscripcion_83KI Solicitud { get; set; }
        public List<Cuota_83KI> CuotasPendientes { get; set; }
        public List<Cuota_83KI> Cuotas { get; set; }
        public string NombrePlanDePago { get; set; }
        public bool MatriculaPagada { get; set; }
        public string CodigoComision { get; set; }
        public string Curso { get; set; }

        public BusquedaCuotasPagoResultado_83KI()
        {
            CuotasPendientes = new List<Cuota_83KI>();
            Cuotas = new List<Cuota_83KI>();
        }
    }
}

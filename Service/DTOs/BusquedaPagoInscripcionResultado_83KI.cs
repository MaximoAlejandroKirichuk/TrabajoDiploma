using BE.Entidades;
using System.Collections.Generic;

namespace Service.DTOs
{
    public class BusquedaPagoInscripcionResultado_83KI
    {
        public Alumno_83KI Alumno { get; set; }
        public SolicitudInscripcion_83KI Solicitud { get; set; }
        public List<Cuota_83KI> CuotasPendientes { get; set; }
        public string CodigoComision { get; set; }
        public string Curso { get; set; }

        public BusquedaPagoInscripcionResultado_83KI()
        {
            CuotasPendientes = new List<Cuota_83KI>();
        }
    }
}

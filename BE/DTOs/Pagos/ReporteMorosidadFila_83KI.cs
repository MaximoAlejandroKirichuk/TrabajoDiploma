using BE.Entidades;
using System;
using System.Collections.Generic;

namespace BE.DTOs.Pagos
{
    // Una fila del informe de morosidad: una inscripción vigente con sus cuotas con saldo.
    // La DAL completa los datos leídos de la base; la BLL calcula los totales.
    public class ReporteMorosidadFila_83KI
    {
        // datos leídos de la base
        public int IdSolicitudInscripcion { get; set; }
        public string CodigoInscripcion { get; set; }
        public string EstadoInscripcion { get; set; }
        public string DNI { get; set; }
        public string Apellido { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public int IdComision { get; set; }
        public string CodigoComision { get; set; }
        public string EstadoComision { get; set; }
        public int IdCurso { get; set; }
        public string Curso { get; set; }
        public string PlanDePago { get; set; }
        public List<Cuota_83KI> CuotasConSaldo { get; set; } = new List<Cuota_83KI>();

        // datos calculados por la BLL
        public int CantidadCuotasVencidas { get; set; }
        public DateTime? VencimientoImpagoMasAntiguo { get; set; }
        public int DiasAtraso { get; set; }
        public decimal DeudaVencida { get; set; }
        public decimal DeudaTotal { get; set; }
        public string RangoAtraso { get; set; }

        public string ApellidoYNombre
        {
            get { return (Apellido + ", " + Nombre).Trim(' ', ','); }
        }
    }
}

using System;

namespace BE.DTOs.Pagos
{
    public class PagoInscripcionRequest_83KI
    {
        public string DNI { get; set; }
        public string CodigoComision { get; set; }
        public int IdCuota { get; set; }
        public string MetodoPago { get; set; }
        public decimal MontoRecibido { get; set; }
        public string NumeroReferencia { get; set; }
        public DateTime FechaPago { get; set; }
        public DateTime FechaAcreditacion { get; set; }
    }
}

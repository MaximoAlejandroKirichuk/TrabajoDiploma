using System;

namespace Service.DTOs
{
    public class PagoInscripcionRequest_83KI
    {
        public string DNI { get; set; }
        public int IdCuota { get; set; }
        public string MetodoPago { get; set; }
        public decimal MontoRecibido { get; set; }
        public string NumeroReferencia { get; set; }
        public DateTime FechaPago { get; set; }
    }
}

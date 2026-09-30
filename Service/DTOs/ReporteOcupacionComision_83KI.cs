using System;

namespace Service.DTOs
{
    public class ReporteOcupacionComision_83KI
    {
        public int IdComision { get; set; }
        public string Codigo { get; set; }
        public string Curso { get; set; }
        public string Profesor { get; set; }
        public DateTime FechaLimitePago { get; set; }
        public int CupoMinimo { get; set; }
        public int CupoMaximo { get; set; }
        public string Estado { get; set; }
        public int Inscriptos { get; set; }
        public int Pagaron { get; set; }
        public int CuotasReintegro { get; set; }
        public decimal MontoReintegro { get; set; }
        public string NumeroActa { get; set; }

        public int PendientesPago
        {
            get { return Math.Max(0, Inscriptos - Pagaron); }
        }

        public int VacantesLibres
        {
            get
            {
                if (Estado == "cancelada_falta_quorum") return 0;
                return Math.Max(0, CupoMaximo - Inscriptos);
            }
        }

        // solo tiene sentido mientras la comision sigue en preapertura
        public int FaltanParaQuorum
        {
            get
            {
                if (Estado != "preapertura") return 0;
                return Math.Max(0, CupoMinimo - Pagaron);
            }
        }
    }
}

using System;

namespace Service.DTOs
{
    public class ComisionEstadoDefinitivoListado_83KI
    {
        public int IdComision { get; set; }
        public string Codigo { get; set; }
        public string Curso { get; set; }
        public string Profesor { get; set; }
        public int CupoMinimo { get; set; }
        public int CupoMaximo { get; set; }
        public DateTime FechaLimitePago { get; set; }
        public int VacantesRegularizadas { get; set; }
        public bool CumpleQuorum { get { return VacantesRegularizadas >= CupoMinimo; } }
        public string Estado { get; set; }
    }
}

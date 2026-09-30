namespace Service.DTOs
{
    public class EvaluacionQuorumComision_83KI
    {
        public int IdComision { get; set; }
        public string Codigo { get; set; }
        public int CupoMinimo { get; set; }
        public int VacantesRegularizadas { get; set; }
        public bool CumpleQuorum { get { return VacantesRegularizadas >= CupoMinimo; } }
        public string Estado { get; set; }
    }
}

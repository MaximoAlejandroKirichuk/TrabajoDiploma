namespace BE.DTOs.Pagos
{
    // Filtros del informe de morosidad (RF2). Un valor null o vacío significa "sin filtro".
    public class FiltroReporteMorosidad_83KI
    {
        public int? IdCurso { get; set; }
        public int? IdComision { get; set; }
        public string DNI { get; set; }
        public int? DiasAtrasoMinimo { get; set; }
        public decimal? DeudaVencidaMinima { get; set; }
    }
}

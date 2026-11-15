namespace BE.DTOs.Recorridos
{
    public sealed class RecorridoListado_83KI
    {
        public int IdRecorridoCurso { get; set; }
        public int IdCursoPrevio { get; set; }
        public int IdCursoRecomendado { get; set; }
        public string CursoPrevio { get; set; }
        public string CursoRecomendado { get; set; }
        public int Orden { get; set; }
        public bool Activo { get; set; }
    }
}

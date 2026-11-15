using System;

namespace BE.Entidades
{
    public sealed class RecorridoCurso_83KI
    {
        public int IdRecorridoCurso { get; }
        public int IdCursoPrevio { get; }
        public int IdCursoRecomendado { get; }
        public int Orden { get; }

        public RecorridoCurso_83KI(int id, int previo, int recomendado, int orden)
        {
            if (id < 0 || previo <= 0 || recomendado <= 0 || previo == recomendado || orden <= 0)
                throw new ArgumentException("Inteligente.RecorridoInvalido");
            IdRecorridoCurso = id;
            IdCursoPrevio = previo;
            IdCursoRecomendado = recomendado;
            Orden = orden;
        }
    }
}

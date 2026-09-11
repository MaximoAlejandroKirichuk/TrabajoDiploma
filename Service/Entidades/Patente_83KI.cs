using System.Collections.Generic;

namespace Service.Entidades
{
    public class Patente_83KI : ComponentePermiso_83KI
    {
        public int CodigoPatente => Codigo;

        public Patente_83KI(int codigoPatente, string nombre)
            : base(codigoPatente, nombre)
        {
        }

        public override IEnumerable<Patente_83KI> ObtenerPatentes()
        {
            yield return this;
        }
    }
}

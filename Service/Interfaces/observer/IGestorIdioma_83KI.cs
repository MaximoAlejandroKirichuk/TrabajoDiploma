using System.Collections.Generic;

namespace Service.Interfaces
{
    public interface IGestorIdioma_83KI : ISujetoIdioma
    {
        IIdioma IdiomaActual { get; }
        IEnumerable<IIdioma> ObtenerIdiomas();
        void CambiarIdioma(string idiomaId);
        string ObtenerTexto(string clave);
    }
}

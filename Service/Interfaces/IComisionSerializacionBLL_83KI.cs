using Service.DTOs;
using System.Collections.Generic;

namespace Service.Interfaces
{
    public interface IComisionSerializacionBLL_83KI
    {
        void Serializar(string ruta, List<ComisionListado_83KI> comisiones);
        List<ComisionXml_83KI> Deserializar(string ruta);
    }
}

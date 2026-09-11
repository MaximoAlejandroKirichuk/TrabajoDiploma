using Service.DTOs;
using System.Collections.Generic;

namespace UI.Exportacion
{
    public interface IBitacoraEventosExporter_83KI
    {
        bool PuedeExportar(out string mensaje);
        void Exportar(IEnumerable<BitacoraEventoVista_83KI> eventos, string rutaArchivo);
    }
}

using Service.DTOs;
using BE.DTOs.Comisiones;
using BE.DTOs.Becas;
using System.Collections.Generic;

namespace Service.Interfaces
{
    public interface IGestorBeca_83KI
    {
        IEnumerable<ComisionListado_83KI> ListarComisionesElegibles();
        BusquedaDecisionBecaResultado_83KI BuscarAlumnoConInscripcion(string dni, string codigoComision);
        DecisionBecaResultado_83KI RegistrarDecisionBeca(DecisionBecaRequest_83KI request);
    }
}

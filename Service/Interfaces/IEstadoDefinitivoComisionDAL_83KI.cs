using Service.DTOs;
using System.Collections.Generic;

namespace Service.Interfaces
{
    public interface IEstadoDefinitivoComisionDAL_83KI
    {
        IEnumerable<ComisionEstadoDefinitivoListado_83KI> ListarComisionesVencidas();
        EvaluacionQuorumComision_83KI Evaluar(int idComision);
        ResultadoEstadoDefinitivoComision_83KI RegistrarAltaOficial(AltaOficialComisionRequest_83KI request);
        ResultadoEstadoDefinitivoComision_83KI RegistrarActaCierre(ActaCierreComisionRequest_83KI request);
    }
}

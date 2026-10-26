using System.Collections.Generic;
using BE.Entidades;
using Service.DTOs;

namespace Service.Interfaces
{
    public interface IEvaluacionesDAL_83KI
    {
        List<ComisionListado_83KI> ListarComisionesConResultados();
        List<EvaluacionConsulta_83KI> BuscarEvaluaciones(int idComision);
        EvaluacionConsulta_83KI ObtenerEvaluacion(int idResultado);
        bool ModificarEvaluacion(EvaluacionConsulta_83KI original, ModificarEvaluacionRequest_83KI cambios);
    }
}

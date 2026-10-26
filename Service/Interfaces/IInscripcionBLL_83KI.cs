using BE.Entidades;
using BE.DTOs.Pagos;
using BE.DTOs.Inscripciones;
using Service.DTOs;
using BE.DTOs.Comisiones;
using System.Collections.Generic;

namespace Service.Interfaces
{
    public interface IInscripcionBLL_83KI
    {
        IEnumerable<BusquedaInscripcionPersonaResultado_83KI> BuscarPersonas(string texto);
        IEnumerable<ComisionListado_83KI> ListarComisionesElegibles();
        IEnumerable<PlanDePago_83KI> ObtenerPlanesComision(int idComision);
        CuotaPreview_83KI PrevisualizarCuotaInicial(int idComision);
        SolicitudInscripcionResultado_83KI RegistrarSolicitud(SolicitudInscripcionRequest_83KI request);
    }
}

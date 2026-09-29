using BE.Entidades;
using Service.DTOs;
using System.Collections.Generic;

namespace Service.Interfaces
{
    public interface IInscripcionDAL_83KI
    {
        IEnumerable<BusquedaInscripcionPersonaResultado_83KI> BuscarPersonas(string texto);
        IEnumerable<ComisionListado_83KI> ListarComisionesElegibles();
        IEnumerable<PlanDePago_83KI> ObtenerPlanesComision(int idComision);
        SolicitudInscripcionResultado_83KI RegistrarSolicitud(SolicitudInscripcionRequest_83KI request);
    }
}

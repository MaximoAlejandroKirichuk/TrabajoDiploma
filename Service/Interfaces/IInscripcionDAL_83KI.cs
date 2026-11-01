using BE.Entidades;
using BE.DTOs.Inscripciones;
using Service.DTOs;
using BE.DTOs.Comisiones;
using System.Collections.Generic;

namespace Service.Interfaces
{
    public interface IInscripcionDAL_83KI
    {
        SolicitudInscripcion_83KI ObtenerPorDniYComision(string dni, string codigoComision);
        IEnumerable<BusquedaInscripcionPersonaResultado_83KI> BuscarPersonas(string texto);
        IEnumerable<ComisionListado_83KI> ListarComisionesElegibles();
        IEnumerable<PlanDePago_83KI> ObtenerPlanesComision(int idComision);
        SolicitudInscripcionResultado_83KI RegistrarSolicitud(SolicitudInscripcionRequest_83KI request);
    }
}

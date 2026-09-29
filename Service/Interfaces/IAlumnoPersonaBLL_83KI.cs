using BE.Entidades;
using Service.DTOs;

namespace Service.Interfaces
{
    public interface IAlumnoPersonaBLL_83KI
    {
        BusquedaLeadResultado_83KI BuscarLead(string dni, string email, string telefono);
        Lead_83KI ResolverLeadUnico(ConsultaLeadRequest_83KI request);
    }
}

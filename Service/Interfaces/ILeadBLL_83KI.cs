using Service.DTOs;

namespace Service.Interfaces
{
    public interface ILeadBLL_83KI
    {
        BusquedaLeadResultado_83KI BuscarLead(string dni, string email, string telefono);
        ConsultaLeadResultado_83KI RegistrarConsulta(ConsultaLeadRequest_83KI request);
    }
}

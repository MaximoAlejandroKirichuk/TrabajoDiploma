using BE.Entidades;
using Service.DTOs;

namespace Service.Interfaces
{
    public interface ILeadDAL_83KI
    {
        BusquedaLeadResultado_83KI BuscarPorIdentidad(string dni, string email, string telefono);
        Lead_83KI ObtenerPorId(int idLead);
    }
}

using Service.DTOs;

namespace Service.Interfaces
{
    public interface IPagoInscripcionBLL_83KI
    {
        BusquedaPagoInscripcionResultado_83KI BuscarCuotasPendientes(string dni);
        PagoInscripcionResultado_83KI RegistrarPagoInscripcion(PagoInscripcionRequest_83KI request);
    }
}

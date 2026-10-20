using Service.DTOs;

namespace Service.Interfaces
{
    public interface IPagoInscripcionBLL_83KI
    {
        System.Collections.Generic.IEnumerable<ComisionPagoOpcion_83KI> ListarComisionesConCuotasPendientes(string dni);
        BusquedaPagoInscripcionResultado_83KI BuscarCuotasPendientes(string dni, string codigoComision);
        PagoInscripcionResultado_83KI RegistrarPagoInscripcion(PagoInscripcionRequest_83KI request);
    }
}

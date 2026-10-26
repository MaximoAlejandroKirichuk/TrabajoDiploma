using BE.DTOs.Pagos;
using Service.DTOs;
using BE.DTOs.Comisiones;
using System.Collections.Generic;

namespace Service.Interfaces
{
    public interface IPagoCuotaBLL_83KI
    {
        IEnumerable<ComisionPagoOpcion_83KI> ListarComisionesConCuotasPendientes(string dni);
        BusquedaPagoInscripcionResultado_83KI BuscarCuotasPendientes(string dni, string codigoComision);
        PagoInscripcionResultado_83KI RegistrarPagoCuota(PagoInscripcionRequest_83KI request);
    }
}

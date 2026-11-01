using BE.DTOs.Pagos;
using Service.DTOs;
using BE.DTOs.Comisiones;
using System.Collections.Generic;

namespace Service.Interfaces
{
    public interface IPagoCuotaBLL_83KI
    {
        IEnumerable<ComisionPagoOpcion_83KI> ListarComisionesConCuotasPendientes(string dni);
        BusquedaCuotasPagoResultado_83KI BuscarCuotasPendientes(string dni, string codigoComision);
        PagoCuotaResultado_83KI RegistrarPagoCuota(PagoCuotaRequest_83KI request);
    }
}

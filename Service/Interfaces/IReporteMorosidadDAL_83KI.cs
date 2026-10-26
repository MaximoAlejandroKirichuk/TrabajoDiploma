using BE.DTOs.Pagos;
using Service.DTOs;
using BE.DTOs.Comisiones;
using System.Collections.Generic;

namespace Service.Interfaces
{
    public interface IReporteMorosidadDAL_83KI
    {
        List<ReporteMorosidadFila_83KI> ObtenerInscripcionesConSaldo(FiltroReporteMorosidad_83KI filtro);
        List<ComisionListado_83KI> ListarComisionesParaFiltro();
    }
}

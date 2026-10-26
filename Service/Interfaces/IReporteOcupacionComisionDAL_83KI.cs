using Service.DTOs;
using BE.DTOs.Comisiones;
using System.Collections.Generic;

namespace Service.Interfaces
{
    public interface IReporteOcupacionComisionDAL_83KI
    {
        IEnumerable<ReporteOcupacionComision_83KI> Generar(string estado);
    }
}

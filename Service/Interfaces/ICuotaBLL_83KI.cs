using BE.Entidades;
using BE.DTOs.Pagos;
using Service.DTOs;
using System;

namespace Service.Interfaces
{
    public interface ICuotaBLL_83KI
    {
        CuotaPreview_83KI PrevisualizarCuotaInicial(Comision_83KI comision, DateTime fechaSolicitud);
    }
}

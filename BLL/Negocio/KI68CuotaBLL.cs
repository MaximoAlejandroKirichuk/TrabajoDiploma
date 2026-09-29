using BE.Entidades;
using Service.DTOs;
using Service.Interfaces;
using System;

namespace BLL
{
    public class KI68CuotaBLL : ICuotaBLL_83KI
    {
        public CuotaPreview_83KI PrevisualizarCuotaInicial(Comision_83KI comision, DateTime fechaSolicitud)
        {
            if (comision == null) throw new ArgumentNullException(nameof(comision));
            if (comision.MontoMatricula <= 0) throw new ArgumentException("Errores.MontoMatriculaInvalido");
            return new CuotaPreview_83KI
            {
                NumeroCuota = 1,
                MontoOriginal = comision.MontoMatricula,
                BalanceAdeudado = comision.MontoMatricula,
                FechaVencimiento = fechaSolicitud.Date.AddDays(7),
                Estado = Cuota_83KI.EstadoPendiente
            };
        }
    }
}

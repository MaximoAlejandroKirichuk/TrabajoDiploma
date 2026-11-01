using BE.Entidades;
using BE.DTOs.Pagos;
using Service.DTOs;
using Service.Interfaces;
using System;

namespace BLL
{
    public class KI68CuotaBLL : ICuotaBLL_83KI
    {
        public void ValidarPagoCompleto(Cuota_83KI cuota, decimal monto)
        {
            if (cuota == null) throw new InvalidOperationException("Errores.PagoCuotaObligatoria");
            if (cuota.EsMatricula || cuota.Estado != Cuota_83KI.EstadoPendiente || cuota.BalanceAdeudado <= 0)
                throw new InvalidOperationException("Errores.PagoCuotaNoPendiente");
            if (monto != cuota.BalanceAdeudado) throw new ArgumentException("Errores.PagoMontoNoCoincide");
        }

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

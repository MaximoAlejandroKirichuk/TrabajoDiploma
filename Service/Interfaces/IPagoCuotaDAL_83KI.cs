using BE.Entidades;
using Service.DTOs;
using System.Collections.Generic;

namespace Service.Interfaces
{
    public interface IPagoCuotaDAL_83KI
    {
        IEnumerable<ComisionPagoOpcion_83KI> ListarComisionesConCuotasPendientes(string dni);
        BusquedaPagoInscripcionResultado_83KI BuscarCuotasPendientes(string dni, string codigoComision);
        bool ExisteReferenciaPago(string numeroReferencia);
        PagoInscripcionResultado_83KI RegistrarPago(PagoInscripcion_83KI pago);
    }
}

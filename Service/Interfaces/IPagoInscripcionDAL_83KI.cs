using BE.Entidades;
using Service.DTOs;

namespace Service.Interfaces
{
    public interface IPagoInscripcionDAL_83KI
    {
        System.Collections.Generic.IEnumerable<ComisionPagoOpcion_83KI> ListarComisionesConCuotasPendientes(string dni);
        BusquedaPagoInscripcionResultado_83KI BuscarCuotasPendientesPorDni(string dni);
        BusquedaPagoInscripcionResultado_83KI BuscarCuotasPendientes(string dni, string codigoComision);
        bool ExisteReferenciaPago(string numeroReferencia);
        PagoInscripcionResultado_83KI RegistrarPago(PagoInscripcion_83KI pago);
    }
}

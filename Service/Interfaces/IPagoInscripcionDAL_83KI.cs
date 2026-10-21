using BE.Entidades;
using Service.DTOs;

namespace Service.Interfaces
{
    public interface IPagoInscripcionDAL_83KI
    {
        BusquedaPagoInscripcionResultado_83KI BuscarCuotasPendientesPorDni(string dni);
        bool ExisteReferenciaPago(string numeroReferencia);
        PagoInscripcionResultado_83KI RegistrarPago(PagoInscripcion_83KI pago);
    }
}

using Service.DTOs;

namespace Service.Interfaces
{
    /// <summary>
    /// contrato para verificacion de integridad y recalculo de hashes.
    /// usado por la puerta de login (bll) y la ui de recuperacion del admin.
    /// </summary>
    public interface IIntegridadDatosService_83KI
    {
        /// <summary>
        /// verifica la integridad de todas las tablas protegidas comparando el dvv
        /// almacenado contra el dvv calculado desde los valores dvh actuales.
        /// </summary>
        IntegridadSistemaEstado_83KI Verificar();

        /// <summary>
        /// recalcula todos los valores dvh desde los datos vivos, regenera todas las
        /// entradas dvv y devuelve el nuevo estado sano.
        /// </summary>
        /// <param name="actor">username que ejecuta el recalculo (para auditoria).</param>
        IntegridadSistemaEstado_83KI RecalcularTodo(string actor);
    }
}

using System.Collections.Generic;

namespace Service.DTOs
{
    /// <summary>
    /// resultado de integridad por tabla comparando el dvv almacenado contra el dvv calculado.
    /// cuando la tabla es invalida, FilasInconsistentes trae el detalle por fila
    /// para ayudar al administrador a identificar que filas y columnas estan afectadas.
    /// </summary>
    public class IntegridadTablaEstado_83KI
    {
        public string NombreTabla { get; set; }
        public string DVVActual { get; set; }
        public string DVVEsperado { get; set; }
        public bool EsValido { get; set; }

        /// <summary>
        /// detalle de inconsistencia por fila (discrepancia dvh almacenado vs calculado).
        /// se llena solo para tablas donde EsValido es false.
        /// </summary>
        public List<IntegridadFilaInconsistencia_83KI> FilasInconsistentes { get; set; }
            = new List<IntegridadFilaInconsistencia_83KI>();
    }
}

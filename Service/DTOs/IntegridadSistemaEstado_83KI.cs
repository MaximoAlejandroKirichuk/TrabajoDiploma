using System.Collections.Generic;

namespace Service.DTOs
{
    /// <summary>
    /// resultado agregado de salud para el sistema de verificacion de integridad.
    /// EstaSano = true cuando todas las tablas protegidas coinciden con su dvv almacenado.
    /// </summary>
    public class IntegridadSistemaEstado_83KI
    {
        public bool EstaSano { get; set; }
        public bool RequiereRecuperacion { get; set; }
        public List<IntegridadTablaEstado_83KI> Tablas { get; set; } = new List<IntegridadTablaEstado_83KI>();
    }
}

using System.Collections.Generic;

namespace Service.DTOs
{
    /// <summary>
    /// clasificacion heuristica para una discrepancia dvh a nivel fila.
    /// la identificacion exacta de columna cambiada no es posible con hashing sha-256 por fila,
    /// pero la comparacion dvh-almacenado vs dvh-calculado puede sugerir la causa mas probable.
    /// </summary>
    public enum TipoInconsistenciaFilas_83KI
    {
        /// <summary>
        /// dvh almacenado existe (no vacio) y difiere del dvh calculado.
        /// los datos de la fila probablemente fueron modificados sin actualizar su dvh.
        /// </summary>
        Modificacion,

        /// <summary>
        /// dvh almacenado es null, vacio o un valor por defecto mientras que el
        /// dvh calculado es valido. sugiere que la fila fue insertada (ej. via sql
        /// directo) sin setear la columna dvh.
        /// </summary>
        Insercion,

        /// <summary>
        /// no se puede determinar una clasificacion confiable a partir de los hashes disponibles.
        /// se usa cuando ambos dvh (almacenado y calculado) estan vacios, o para otros
        /// casos limite ambiguos.
        /// </summary>
        Desconocido
    }

    /// <summary>
    /// detalle de inconsistencia por fila producido durante la verificacion de integridad.
    /// identifica que filas especificas en una tabla protegida tienen discrepancia
    /// dvh-almacenado vs dvh-calculado, el tipo probable de inconsistencia y las
    /// columnas candidatas afectadas.
    /// </summary>
    public class IntegridadFilaInconsistencia_83KI
    {
        /// <summary>nombre de tabla protegida (ej. "Usuarios").</summary>
        public string NombreTabla { get; set; }

        /// <summary>
        /// identificador de clave primaria para la fila afectada
        /// (ej. "DNI=12345678", "CodigoRol=1, CodigoPatente=5").
        /// </summary>
        public string ClavePrimaria { get; set; }

        /// <summary>valor dvh actualmente almacenado en la base para esta fila.</summary>
        public string DVHAlmacenado { get; set; }

        /// <summary>dvh recien calculado desde los datos vivos de columna para esta fila.</summary>
        public string DVHCalculado { get; set; }

        /// <summary>
        /// clasificacion heuristica de esta inconsistencia a nivel fila.
        /// basada en comparar dvh almacenado vs calculado — la identificacion exacta
        /// a nivel columna no es posible con hashing sha-256 unidireccional por fila.
        /// </summary>
        public TipoInconsistenciaFilas_83KI Tipo { get; set; }

        /// <summary>
        /// todos los nombres de columna (excepto dvh) definidos en el conjunto canonico de esta tabla.
        /// como dvh es un hash unidireccional no podemos precisar las columnas exactas alteradas;
        /// reportamos el conjunto completo de columnas candidatas para la fila.
        /// </summary>
        public List<string> ColumnasAfectadas { get; set; } = new List<string>();
    }
}

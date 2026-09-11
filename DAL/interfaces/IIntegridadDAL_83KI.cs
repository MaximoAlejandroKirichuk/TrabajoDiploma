using System.Collections.Generic;
using System.Data.SqlClient;
using Service.DTOs;

namespace DAL.interfaces
{
    /// <summary>
    /// contrato dal para operaciones de integridad dvh/dvv.
    /// maneja hashing sha-256 por fila, agregacion dvv por tabla, consultas de verificacion
    /// y recomputacion transaccional dvh+dvv en escrituras protegidas.
    /// </summary>
    public interface IIntegridadDAL_83KI
    {
        /// <summary>
        /// calcula el dvh sha-256 hex en minuscula para una sola fila.
        /// </summary>
        /// <param name="nombreTabla">nombre de tabla protegida (debe existir en IntegridadTablaConfig_83KI).</param>
        /// <param name="valoresColumna">diccionario nombre columna → valor. debe incluir todas las columnas canonicas.</param>
        /// <returns>string hex de 64 caracteres en minuscula.</returns>
        string CalcularDVH(string nombreTabla, Dictionary<string, object> valoresColumna);

        /// <summary>
        /// obtiene los valores dvv almacenados actuales para todas las tablas protegidas.
        /// </summary>
        /// <returns>diccionario nombre tabla → string hex dvv.</returns>
        Dictionary<string, string> ObtenerDVVActuales();

        /// <summary>
        /// calcula el dvv agregado actual para una tabla recomputando dvh frescos
        /// desde los datos vivos de las filas, y luego hasheando la concatenacion ordenada de esos dvh.
        /// </summary>
        /// <param name="nombreTabla">nombre de tabla protegida.</param>
        /// <returns>string hex de 64 caracteres en minuscula.</returns>
        string CalcularDVVPorTabla(string nombreTabla);

        /// <summary>
        /// devuelve el detalle de discrepancia dvh por fila para una tabla protegida.
        /// lee todas las filas vivas, calcula dvh fresco desde los datos de columna, compara contra
        /// el dvh almacenado y devuelve una lista de filas donde los hashes difieren.
        /// </summary>
        /// <param name="nombreTabla">nombre de tabla protegida.</param>
        /// <returns>lista de filas inconsistentes (vacia si todas las filas coinciden).</returns>
        List<IntegridadFilaInconsistencia_83KI> ObtenerFilasInconsistentes(string nombreTabla);

        /// <summary>
        /// actualiza el dvh de una fila y refresca el dvv de la tabla dentro de una transaccion existente.
        /// usado por UsuarioDAL_83KI y RolDAL_83KI durante escrituras protegidas.
        /// </summary>
        /// <param name="nombreTabla">nombre de tabla protegida.</param>
        /// <param name="valoresColumna">diccionario nombre columna → valor para la fila.</param>
        /// <param name="whereClause">clausula where sql para identificar la fila especifica (ej. "DNI = @dni").</param>
        /// <param name="whereParams">parametros sql para la clausula where.</param>
        /// <param name="conn">conexion sql abierta (transaccional).</param>
        /// <param name="tran">transaccion sql activa.</param>
        void ActualizarDVHyDVVTransaccional(
            string nombreTabla,
            Dictionary<string, object> valoresColumna,
            string whereClause,
            List<SqlParameter> whereParams,
            SqlConnection conn,
            SqlTransaction tran);

        /// <summary>
        /// recalcula todos los valores dvh para cada fila de cada tabla protegida,
        /// luego regenera todas las entradas dvv. usado por el recalculo del admin.
        /// </summary>
        void RecalcularTodo();
    }
}

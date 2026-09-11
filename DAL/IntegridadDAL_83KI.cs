using DAL.DAL;
using DAL.interfaces;
using Service.DTOs;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace DAL
{
    /// <summary>
    /// dal nucleo para operaciones de integridad dvh/dvv.
    /// la canonicalizacion sha-256 refleja el script sql de backfill (002_backfill.sql):
    ///   - encoding: utf-16 le (coincide con sql server nvarchar / hashbytes)
    ///   - null → '∅' (u+2205)
    ///   - bool → '1'/'0'
    ///   - datetime → iso-8601 (yyyy-MM-ddTHH:mm:ss.fff)
    ///   - numeros → cultura invariante
    ///   - strings → trim
    ///   - formato: Col=Val|Col=Val|... (orden alfabetico segun IntegridadTablaConfig)
    /// </summary>
    public class IntegridadDAL_83KI : IIntegridadDAL_83KI
    {
        private readonly AccesoDAL_83KI _accesoDAL = new AccesoDAL_83KI();
        private readonly IEncriptador_83KI _encriptador;

        public IntegridadDAL_83KI(IEncriptador_83KI encriptador)
        {
            _encriptador = encriptador ?? throw new ArgumentNullException(nameof(encriptador));
        }

        // columnas de clave primaria por tabla protegida.
        // debe coincidir con el esquema real de la base de datos.
        private static readonly IReadOnlyDictionary<string, string[]> PK =
            new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
            {
                { "Usuarios",        new[] { "DNI" } },
                { "Roles",           new[] { "CodigoRol" } },
                { "Familias",        new[] { "CodigoFamilia" } },
                { "Patentes",        new[] { "CodigoPatente" } },
                { "RolPatente",      new[] { "CodigoRol", "CodigoPatente" } },
                { "RolFamilia",      new[] { "CodigoRol", "CodigoFamilia" } },
                { "FamiliaPatente",  new[] { "CodigoFamilia", "CodigoPatente" } },
                { "FamiliaFamilia",  new[] { "CodigoFamiliaPadre", "CodigoFamiliaHija" } },
            };

        // ------------------------------------------------------------------ //
        //  metodos publicos de la interfaz
        // ------------------------------------------------------------------ //

        public string CalcularDVH(string nombreTabla, Dictionary<string, object> valoresColumna)
        {
            var config = ResolverConfig(nombreTabla);
            string canonical = CanonicalizarFila(config, valoresColumna);
            return _encriptador.CalcularHashIntegridad(canonical);
        }

        public Dictionary<string, string> ObtenerDVVActuales()
        {
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            string sql = "SELECT NombreTabla, DVV FROM DigitoVerificador_83KI";
            var ds = _accesoDAL.Leer(sql);
            if (ds != null && ds.Tables.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    result[row["NombreTabla"].ToString()] = row["DVV"].ToString();
                }
            }
            return result;
        }

        /// <summary>
        /// calcula el dvv actual para una tabla recomputando dvh frescos desde
        /// los datos VIVOS de las filas (no desde los dvh almacenados). esta es la
        /// correccion critica: ediciones directas sql de columnas cambian los datos
        /// vivos, lo que produce un dvh distinto al almacenado, lo que a su vez
        /// cambia el dvv calculado.
        /// </summary>
        public string CalcularDVVPorTabla(string nombreTabla)
        {
            var config = ResolverConfig(nombreTabla);
            var dvhList = LeerDVHFresco(config);
            dvhList.Sort(StringComparer.Ordinal);
            return _encriptador.CalcularHashIntegridad(string.Concat(dvhList));
        }

        /// <summary>
        /// devuelve el detalle de discrepancia dvh por fila para una tabla protegida.
        /// lee todas las filas vivas, calcula dvh fresco desde los datos de columna,
        /// compara contra el dvh almacenado y devuelve las filas donde los hashes difieren.
        /// </summary>
        public List<IntegridadFilaInconsistencia_83KI> ObtenerFilasInconsistentes(string nombreTabla)
        {
            var config = ResolverConfig(nombreTabla);
            var pkCols = PK[nombreTabla];
            var inconsistencias = new List<IntegridadFilaInconsistencia_83KI>();

            string sql = $"SELECT * FROM {nombreTabla}";
            var ds = _accesoDAL.Leer(sql);
            if (ds == null || ds.Tables.Count == 0)
                return inconsistencias;

            DataTable table = ds.Tables[0];

            // construye mapa nombre-columna → indice (excluyendo dvh)
            var colIndex = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            int dvhIndex = -1;
            for (int i = 0; i < table.Columns.Count; i++)
            {
                string cname = table.Columns[i].ColumnName;
                if (cname.Equals("DVH", StringComparison.OrdinalIgnoreCase))
                {
                    dvhIndex = i;
                }
                else
                {
                    colIndex[cname] = i;
                }
            }

            foreach (DataRow row in table.Rows)
            {
                // lee valores de columna (excluyendo dvh)
                var valoresColumna = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                foreach (var kv in colIndex)
                {
                    valoresColumna[kv.Key] = row.IsNull(kv.Value) ? null : row[kv.Value];
                }

                string dvhCalculado = CalcularDVH(nombreTabla, valoresColumna);
                string dvhAlmacenado = dvhIndex >= 0 && !row.IsNull(dvhIndex)
                    ? row[dvhIndex].ToString() : string.Empty;

                if (!string.Equals(dvhCalculado, dvhAlmacenado, StringComparison.OrdinalIgnoreCase))
                {
                    // construye string de pk
                    var pkParts = new List<string>();
                    foreach (var pkCol in pkCols)
                    {
                        object val = valoresColumna.TryGetValue(pkCol, out object v) ? v : null;
                        pkParts.Add($"{pkCol}={val}");
                    }

                    var tipo = ClasificarInconsistencia(dvhAlmacenado, dvhCalculado);

                    inconsistencias.Add(new IntegridadFilaInconsistencia_83KI
                    {
                        NombreTabla = nombreTabla,
                        ClavePrimaria = string.Join(", ", pkParts),
                        DVHAlmacenado = dvhAlmacenado,
                        DVHCalculado = dvhCalculado,
                        Tipo = tipo,
                        ColumnasAfectadas = config.ColumnasOrdenadas.ToList()
                    });
                }
            }

            return inconsistencias;
        }

        public void ActualizarDVHyDVVTransaccional(
            string nombreTabla,
            Dictionary<string, object> valoresColumna,
            string whereClause,
            List<SqlParameter> whereParams,
            SqlConnection conn,
            SqlTransaction tran)
        {
            string dvh = CalcularDVH(nombreTabla, valoresColumna);

            // 1. actualiza la columna dvh de la fila objetivo
            string updateDvhSql = $"UPDATE {nombreTabla} SET DVH = @dvh WHERE {whereClause}";
            var dvhParams = new List<SqlParameter> { new SqlParameter("@dvh", dvh) };
            if (whereParams != null) dvhParams.AddRange(whereParams);
            AccesoDAL_83KI.EscribirTransaccional(conn, tran, updateDvhSql, dvhParams);

            // 2. refresca el dvv de la tabla
            ActualizarDVVTransaccional(nombreTabla, conn, tran);
        }

        public void RecalcularTodo()
        {
            _accesoDAL.EjecutarTransaccion((conn, tran) =>
            {
                foreach (var config in IntegridadTablaConfig_83KI.TablasProtegidas)
                {
                    RecalcularTablaTransaccional(config, conn, tran);
                }
            });
        }

        // ------------------------------------------------------------------ //
        //  helpers internos usables por UsuarioDAL / RolDAL (mismo namespace)
        // ------------------------------------------------------------------ //

        /// <summary>
        /// lee los datos actuales de una fila dentro de una transaccion
        /// y devuelve un diccionario nombre-columna → valor.
        /// la columna "DVH" se excluye del conjunto devuelto.
        /// </summary>
        internal static Dictionary<string, object> LeerFilaTransaccional(
            string nombreTabla,
            string whereClause,
            List<SqlParameter> whereParams,
            SqlConnection conn,
            SqlTransaction tran)
        {
            var config = IntegridadTablaConfig_83KI.ObtenerPorNombre(nombreTabla);
            if (config == null) return new Dictionary<string, object>();

            string sql = $"SELECT * FROM {nombreTabla} WHERE {whereClause}";
            var result = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

            using (var cmd = new SqlCommand(sql, conn, tran))
            {
                if (whereParams != null)
                    cmd.Parameters.AddRange(whereParams.ToArray());

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            string colName = reader.GetName(i);
                            if (colName.Equals("DVH", StringComparison.OrdinalIgnoreCase))
                                continue;
                            result[colName] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// refresca el dvv de una tabla recomputandolo desde los dvh actuales
        /// dentro de una transaccion existente. ahora usa datos vivos (no dvh almacenados).
        /// </summary>
        internal void ActualizarDVVTransaccional(
            string nombreTabla,
            SqlConnection conn,
            SqlTransaction tran)
        {
            string dvv = CalcularDVVPorTablaTransaccional(nombreTabla, conn, tran);

            string upsertSql = @"
                IF EXISTS (SELECT 1 FROM DigitoVerificador_83KI WHERE NombreTabla = @nombreTabla)
                    UPDATE DigitoVerificador_83KI
                    SET DVV = @dvv, FechaActualizacion = GETDATE()
                    WHERE NombreTabla = @nombreTabla
                ELSE
                    INSERT INTO DigitoVerificador_83KI (NombreTabla, DVV, FechaActualizacion)
                    VALUES (@nombreTabla, @dvv, GETDATE())";

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@nombreTabla", nombreTabla),
                new SqlParameter("@dvv", dvv),
            };
            AccesoDAL_83KI.EscribirTransaccional(conn, tran, upsertSql, parametros);
        }

        /// <summary>
        /// recalcula y actualiza el dvh de una sola fila identificada por su pk,
        /// luego refresca el dvv de la tabla. el llamador debe proveer los valores actuales de la fila.
        /// </summary>
        internal void RecomputarIntegridadFila(
            string nombreTabla,
            Dictionary<string, object> valoresColumna,
            SqlConnection conn,
            SqlTransaction tran)
        {
            var config = ResolverConfig(nombreTabla);
            var pkCols = PK[nombreTabla];

            // construye clausula where desde las columnas pk
            var whereClauses = new List<string>();
            var whereParams = new List<SqlParameter>();
            foreach (var pkCol in pkCols)
            {
                whereClauses.Add($"{pkCol} = @pk_{pkCol}");
                whereParams.Add(new SqlParameter($"@pk_{pkCol}", valoresColumna[pkCol] ?? DBNull.Value));
            }
            string whereClause = string.Join(" AND ", whereClauses);

            ActualizarDVHyDVVTransaccional(nombreTabla, valoresColumna, whereClause, whereParams, conn, tran);
        }

        // ------------------------------------------------------------------ //
        //  helpers privados
        // ------------------------------------------------------------------ //

        private static IntegridadTablaConfig_83KI ResolverConfig(string nombreTabla)
        {
            var config = IntegridadTablaConfig_83KI.ObtenerPorNombre(nombreTabla);
            if (config == null)
                throw new ArgumentException($"Table '{nombreTabla}' is not a protected table.");
            return config;
        }

        /// <summary>
        /// canonicaliza una fila al formato string deterministico usado para dvh.
        /// formato: Col=Val|Col=Val|...  (columnas en orden alfabetico segun config,
        /// excluyendo dvh). las reglas coinciden exactamente con 002_backfill.sql.
        /// </summary>
        private static string CanonicalizarFila(
            IntegridadTablaConfig_83KI config,
            Dictionary<string, object> valoresColumna)
        {
            var sb = new StringBuilder();
            bool first = true;

            foreach (string columna in config.ColumnasOrdenadas)
            {
                if (!first) sb.Append('|');
                first = false;

                sb.Append(columna);
                sb.Append('=');

                if (!valoresColumna.TryGetValue(columna, out object valor) || valor == null || valor == DBNull.Value)
                {
                    sb.Append('\u2205'); // ∅
                }
                else
                {
                    sb.Append(CanonicalizarValor(valor));
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// convierte un valor de columna a su representacion string canonica.
        /// </summary>
        private static string CanonicalizarValor(object valor)
        {
            if (valor is bool b)
                return b ? "1" : "0";

            if (valor is DateTime dt)
                return dt.ToString("yyyy-MM-ddTHH:mm:ss.fff");

            if (valor is DateTimeOffset dto)
                return dto.ToString("yyyy-MM-ddTHH:mm:ss.fff");

            if (valor is string s)
                return s.Trim();

            // numeros (int, decimal, etc.) → cultura invariante
            if (valor is IFormattable formattable)
                return formattable.ToString(null, System.Globalization.CultureInfo.InvariantCulture);

            return valor.ToString();
        }

        /// <summary>
        /// clasificacion heuristica de una discrepancia dvh.
        /// 
        /// - modificacion: dvh almacenado no vacio y distinto del dvh calculado
        ///   → los datos de la fila fueron alterados sin actualizar el hash.
        /// - insercion:    dvh almacenado es null/vacio/whitespace y el calculado es valido
        ///   → la fila fue insertada (prob. via sql directo) sin setear el dvh.
        /// - desconocido:  caso ambiguo (ambos vacios, ambos null, etc.).
        /// 
        /// es inherentemente heuristico porque sha-256 es un hash unidireccional;
        /// no podemos revertir el diff para identificar que columnas especificas cambiaron.
        /// </summary>
        private static TipoInconsistenciaFilas_83KI ClasificarInconsistencia(
            string dvhAlmacenado,
            string dvhCalculado)
        {
            bool almacenadoVacio = string.IsNullOrWhiteSpace(dvhAlmacenado);
            bool calculadoVacio  = string.IsNullOrWhiteSpace(dvhCalculado);

            if (almacenadoVacio && !calculadoVacio)
                return TipoInconsistenciaFilas_83KI.Insercion;

            if (!almacenadoVacio && !calculadoVacio)
                return TipoInconsistenciaFilas_83KI.Modificacion;

            // ambos vacios, o almacenado presente pero calculado vacio (inusual)
            return TipoInconsistenciaFilas_83KI.Desconocido;
        }

        /// <summary>
        /// lee todas las filas de una tabla protegida (excluyendo dvh), calcula dvh
        /// fresco para cada fila desde los datos vivos de columna, y devuelve la lista
        /// de strings dvh hex. usado tanto para verificacion (comparacion dvv) como
        /// para deteccion de inconsistencias a nivel fila.
        /// </summary>
        private List<string> LeerDVHFresco(IntegridadTablaConfig_83KI config)
        {
            string nombreTabla = config.NombreTabla;
            var dvhList = new List<string>();

            string sql = $"SELECT * FROM {nombreTabla}";
            var ds = _accesoDAL.Leer(sql);
            if (ds == null || ds.Tables.Count == 0)
                return dvhList;

            DataTable table = ds.Tables[0];

            // construye mapa nombre-columna → indice (excluyendo dvh)
            var colIndex = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < table.Columns.Count; i++)
            {
                string cname = table.Columns[i].ColumnName;
                if (!cname.Equals("DVH", StringComparison.OrdinalIgnoreCase))
                    colIndex[cname] = i;
            }

            foreach (DataRow row in table.Rows)
            {
                var valoresColumna = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                foreach (var kv in colIndex)
                {
                    valoresColumna[kv.Key] = row.IsNull(kv.Value) ? null : row[kv.Value];
                }
                dvhList.Add(CalcularDVH(nombreTabla, valoresColumna));
            }

            return dvhList;
        }

        /// <summary>
        /// calcula el dvv de una tabla dentro de una transaccion existente recomputando
        /// dvh frescos desde datos VIVOS de las filas (no dvh almacenados). esto asegura
        /// que se detecten ediciones directas sql de columnas.
        /// </summary>
        private string CalcularDVVPorTablaTransaccional(
            string nombreTabla,
            SqlConnection conn,
            SqlTransaction tran)
        {
            var config = ResolverConfig(nombreTabla);
            var dvhList = new List<string>();

            string sql = $"SELECT * FROM {nombreTabla}";
            var colIndex = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            using (var cmd = new SqlCommand(sql, conn, tran))
            using (var reader = cmd.ExecuteReader())
            {
                // construye mapa nombre-columna → indice (excluyendo dvh)
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    string cname = reader.GetName(i);
                    if (!cname.Equals("DVH", StringComparison.OrdinalIgnoreCase))
                        colIndex[cname] = i;
                }

                while (reader.Read())
                {
                    var valoresColumna = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                    foreach (var kv in colIndex)
                    {
                        valoresColumna[kv.Key] = reader.IsDBNull(kv.Value) ? null : reader.GetValue(kv.Value);
                    }
                    dvhList.Add(CalcularDVH(nombreTabla, valoresColumna));
                }
            }

            dvhList.Sort(StringComparer.Ordinal);
            return _encriptador.CalcularHashIntegridad(string.Concat(dvhList));
        }

        /// <summary>
        /// recalcula todos los valores dvh para cada fila de una tabla protegida,
        /// luego refresca su dvv. se ejecuta dentro de una transaccion existente.
        /// </summary>
        private void RecalcularTablaTransaccional(
            IntegridadTablaConfig_83KI config,
            SqlConnection conn,
            SqlTransaction tran)
        {
            string nombreTabla = config.NombreTabla;
            string[] pkCols = PK[nombreTabla];

            // 1. lee todas las filas (excluyendo dvh) de la tabla
            string selectSql = $"SELECT * FROM {nombreTabla}";
            var filas = new List<Dictionary<string, object>>();

            using (var cmd = new SqlCommand(selectSql, conn, tran))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var rowValues = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        string colName = reader.GetName(i);
                        if (colName.Equals("DVH", StringComparison.OrdinalIgnoreCase))
                            continue;
                        rowValues[colName] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                    }
                    filas.Add(rowValues);
                }
            } // el reader debe cerrarse antes del siguiente comando

            // 2. actualiza dvh para cada fila
            foreach (var row in filas)
            {
                string dvh = CalcularDVH(nombreTabla, row);

                var whereClauses = new List<string>();
                var whereParams = new List<SqlParameter>();
                foreach (var pkCol in pkCols)
                {
                    whereClauses.Add($"{pkCol} = @r_{pkCol}");
                    object val = row.ContainsKey(pkCol) ? row[pkCol] : null;
                    whereParams.Add(new SqlParameter($"@r_{pkCol}", val ?? DBNull.Value));
                }
                string whereClause = string.Join(" AND ", whereClauses);

                string updateSql = $"UPDATE {nombreTabla} SET DVH = @dvh WHERE {whereClause}";
                var updateParams = new List<SqlParameter> { new SqlParameter("@dvh", dvh) };
                updateParams.AddRange(whereParams);

                AccesoDAL_83KI.EscribirTransaccional(conn, tran, updateSql, updateParams);
            }

            // 3. refresca dvv para esta tabla
            ActualizarDVVTransaccional(nombreTabla, conn, tran);
        }
    }
}

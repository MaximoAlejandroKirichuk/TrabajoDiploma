using DAL.DAL;
using Service;
using Service.DTOs;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    public class EstadoDefinitivoComisionDAL_83KI : IEstadoDefinitivoComisionDAL_83KI
    {
        private readonly AccesoDAL_83KI _accesoDAL = new AccesoDAL_83KI();
        private readonly IntegridadDAL_83KI _integridadDAL = new IntegridadDAL_83KI(new Encriptador_83KI());

        public IEnumerable<ComisionEstadoDefinitivoListado_83KI> ListarComisionesVencidas()
        {
            return MapearListado(_accesoDAL.LeerStoredProcedure("sp_CUN06_ListarComisionesVencidas"));
        }

        public EvaluacionQuorumComision_83KI Evaluar(int idComision)
        {
            var ds = _accesoDAL.LeerStoredProcedure("sp_CUN06_EvaluarQuorum", new List<SqlParameter> { new SqlParameter("@IdComision", idComision) });
            if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0) return null;
            return MapearEvaluacion(ds.Tables[0].Rows[0]);
        }

        public ResultadoEstadoDefinitivoComision_83KI RegistrarAltaOficial(AltaOficialComisionRequest_83KI request)
        {
            ResultadoEstadoDefinitivoComision_83KI resultado = null;
            _accesoDAL.EjecutarTransaccion((conn, tran) =>
            {
                var ds = AccesoDAL_83KI.LeerStoredProcedureTransaccional(conn, tran, "sp_CUN06_RegistrarAltaOficial", ParametrosAlta(request));
                if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0) throw new InvalidOperationException("Errores.EstadoDefinitivoNoRegistrado");
                resultado = MapearResultado(ds.Tables[0].Rows[0]);
                Recomputar("Comision", "IdComision", resultado.IdComision, conn, tran);
                Recomputar("AltaOficialComision", "IdAltaOficialComision", resultado.IdActa, conn, tran);
            });
            return resultado;
        }

        public ResultadoEstadoDefinitivoComision_83KI RegistrarActaCierre(ActaCierreComisionRequest_83KI request)
        {
            ResultadoEstadoDefinitivoComision_83KI resultado = null;
            _accesoDAL.EjecutarTransaccion((conn, tran) =>
            {
                var ds = AccesoDAL_83KI.LeerStoredProcedureTransaccional(conn, tran, "sp_CUN06_RegistrarActaCierre", ParametrosCierre(request));
                if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0) throw new InvalidOperationException("Errores.EstadoDefinitivoNoRegistrado");
                resultado = MapearResultado(ds.Tables[0].Rows[0]);
                Recomputar("Comision", "IdComision", resultado.IdComision, conn, tran);
                Recomputar("ActaCierreComision", "IdActaCierreComision", resultado.IdActa, conn, tran);
                RecomputarAfectados("SolicitudInscripcion", "IdComision = @id AND Estado = N'cancelada'", resultado.IdComision, conn, tran);
                RecomputarAfectados("Cuota", "IdSolicitudInscripcion IN (SELECT IdSolicitudInscripcion FROM dbo.SolicitudInscripcion WHERE IdComision = @id)", resultado.IdComision, conn, tran);
            });
            return resultado;
        }

        private static List<SqlParameter> ParametrosAlta(AltaOficialComisionRequest_83KI request)
        {
            return new List<SqlParameter> {
                new SqlParameter("@IdComision", request.IdComision),
                new SqlParameter("@FechaAlta", request.FechaAlta),
                new SqlParameter("@NumeroActa", Normalizar(request.NumeroActa)),
                new SqlParameter("@Observaciones", Normalizar(request.Observaciones)),
                new SqlParameter("@UsuarioRegistro", Normalizar(request.UsuarioRegistro)) };
        }

        private static List<SqlParameter> ParametrosCierre(ActaCierreComisionRequest_83KI request)
        {
            return new List<SqlParameter> {
                new SqlParameter("@IdComision", request.IdComision),
                new SqlParameter("@FechaCierre", request.FechaCierre),
                new SqlParameter("@NumeroActa", Normalizar(request.NumeroActa)),
                new SqlParameter("@Motivo", Normalizar(request.Motivo)),
                new SqlParameter("@Observaciones", Normalizar(request.Observaciones)),
                new SqlParameter("@UsuarioRegistro", Normalizar(request.UsuarioRegistro)) };
        }

        private static IEnumerable<ComisionEstadoDefinitivoListado_83KI> MapearListado(DataSet ds)
        {
            var resultado = new List<ComisionEstadoDefinitivoListado_83KI>();
            if (ds == null || ds.Tables.Count == 0) return resultado;
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                resultado.Add(new ComisionEstadoDefinitivoListado_83KI
                {
                    IdComision = Convert.ToInt32(row["IdComision"]),
                    Codigo = row["Codigo"].ToString(),
                    Curso = row["Curso"].ToString(),
                    Profesor = row["Profesor"].ToString(),
                    CupoMinimo = Convert.ToInt32(row["CupoMinimo"]),
                    CupoMaximo = Convert.ToInt32(row["CupoMaximo"]),
                    FechaLimitePago = Convert.ToDateTime(row["FechaLimitePago"]),
                    VacantesRegularizadas = Convert.ToInt32(row["VacantesRegularizadas"]),
                    Estado = row["Estado"].ToString()
                });
            }
            return resultado;
        }

        private static EvaluacionQuorumComision_83KI MapearEvaluacion(DataRow row)
        {
            return new EvaluacionQuorumComision_83KI
            {
                IdComision = Convert.ToInt32(row["IdComision"]),
                Codigo = row["Codigo"].ToString(),
                CupoMinimo = Convert.ToInt32(row["CupoMinimo"]),
                VacantesRegularizadas = Convert.ToInt32(row["VacantesRegularizadas"]),
                Estado = row["Estado"].ToString()
            };
        }

        private static ResultadoEstadoDefinitivoComision_83KI MapearResultado(DataRow row)
        {
            return new ResultadoEstadoDefinitivoComision_83KI
            {
                IdComision = Convert.ToInt32(row["IdComision"]),
                CodigoComision = row["CodigoComision"].ToString(),
                EstadoDefinitivo = row["EstadoDefinitivo"].ToString(),
                VacantesRegularizadas = Convert.ToInt32(row["VacantesRegularizadas"]),
                CupoMinimo = Convert.ToInt32(row["CupoMinimo"]),
                IdActa = Convert.ToInt32(row["IdActa"]),
                NumeroActa = row["NumeroActa"].ToString(),
                SolicitudesCanceladas = row.Table.Columns.Contains("SolicitudesCanceladas") ? Convert.ToInt32(row["SolicitudesCanceladas"]) : 0,
                CuotasReintegroPendiente = row.Table.Columns.Contains("CuotasReintegroPendiente") ? Convert.ToInt32(row["CuotasReintegroPendiente"]) : 0
            };
        }

        private void Recomputar(string tabla, string pk, int id, SqlConnection conn, SqlTransaction tran)
        {
            var valores = IntegridadDAL_83KI.LeerFilaTransaccional(tabla, pk + " = @id", new List<SqlParameter> { new SqlParameter("@id", id) }, conn, tran);
            if (valores.Count > 0) _integridadDAL.RecomputarIntegridadFila(tabla, valores, conn, tran);
        }

        private void RecomputarAfectados(string tabla, string where, int idComision, SqlConnection conn, SqlTransaction tran)
        {
            var ds = AccesoDAL_83KI.LeerTransaccional(conn, tran, "SELECT * FROM dbo." + tabla + " WHERE " + where, new List<SqlParameter> { new SqlParameter("@id", idComision) });
            if (ds == null || ds.Tables.Count == 0) return;
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                var valores = new Dictionary<string, object>();
                foreach (DataColumn col in ds.Tables[0].Columns) valores[col.ColumnName] = row[col];
                _integridadDAL.RecomputarIntegridadFila(tabla, valores, conn, tran);
            }
        }

        private static string Normalizar(string value) { return (value ?? string.Empty).Trim(); }
    }
}

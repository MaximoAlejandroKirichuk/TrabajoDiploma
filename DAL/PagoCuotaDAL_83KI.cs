using BE.Entidades;
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
    public class PagoCuotaDAL_83KI : IPagoCuotaDAL_83KI
    {
        private readonly AccesoDAL_83KI _accesoDAL = new AccesoDAL_83KI();
        private readonly IntegridadDAL_83KI _integridadDAL = new IntegridadDAL_83KI(new Encriptador_83KI());

        public IEnumerable<ComisionPagoOpcion_83KI> ListarComisionesConCuotasPendientes(string dni)
        {
            var ds = _accesoDAL.LeerStoredProcedure("sp_CUN07_ListarComisionesPago", new List<SqlParameter>
            {
                new SqlParameter("@DNI", Normalizar(dni))
            });
            var resultado = new List<ComisionPagoOpcion_83KI>();
            if (ds == null || ds.Tables.Count == 0) return resultado;
            foreach (DataRow row in ds.Tables[0].Rows)
                resultado.Add(new ComisionPagoOpcion_83KI { Codigo = row["CodigoComision"].ToString(), Descripcion = row["Descripcion"].ToString() });
            return resultado;
        }

        public BusquedaPagoInscripcionResultado_83KI BuscarCuotasPendientes(string dni, string codigoComision)
        {
            var ds = _accesoDAL.LeerStoredProcedure("sp_CUN07_BuscarCuotasPendientes", new List<SqlParameter>
            {
                new SqlParameter("@DNI", Normalizar(dni)),
                new SqlParameter("@CodigoComision", Normalizar(codigoComision))
            });
            if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0) return null;
            BusquedaPagoInscripcionResultado_83KI resultado = null;
            int? idSolicitud = null;
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                int rowIdSolicitud = Convert.ToInt32(row["IdSolicitudInscripcion"]);
                if (idSolicitud.HasValue && idSolicitud.Value != rowIdSolicitud)
                    throw new InvalidOperationException("Errores.PagoSolicitudesAmbiguas");
                idSolicitud = rowIdSolicitud;
                if (resultado == null) resultado = MapearBusqueda(row);
                var cuota = MapearCuota(row);
                resultado.Cuotas.Add(cuota);
                if (cuota.Estado == Cuota_83KI.EstadoPendiente && cuota.BalanceAdeudado > 0)
                    resultado.CuotasPendientes.Add(cuota);
            }
            return resultado;
        }

        public bool ExisteReferenciaPago(string numeroReferencia)
        {
            const string sql = @"SELECT CASE WHEN EXISTS (SELECT 1 FROM dbo.PagoInscripcion WHERE NumeroReferencia = @referencia) THEN 1 ELSE 0 END";
            var ds = _accesoDAL.Leer(sql, new List<SqlParameter> { new SqlParameter("@referencia", Normalizar(numeroReferencia)) });
            return ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0 && Convert.ToInt32(ds.Tables[0].Rows[0][0]) == 1;
        }

        public PagoInscripcionResultado_83KI RegistrarPago(PagoInscripcion_83KI pago)
        {
            if (pago == null) throw new ArgumentNullException(nameof(pago));
            PagoInscripcionResultado_83KI resultado = null;
            _accesoDAL.EjecutarTransaccion((conn, tran) =>
            {
                var ds = AccesoDAL_83KI.LeerStoredProcedureTransaccional(conn, tran, "sp_CUN07_RegistrarPago", CrearParametrosRegistro(pago));
                if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0) throw new InvalidOperationException("Errores.PagoNoRegistrado");
                resultado = MapearResultado(ds.Tables[0].Rows[0]);
                RecomputarIntegridad("PagoInscripcion", "IdPagoInscripcion", resultado.Pago.IdPagoInscripcion, conn, tran);
                RecomputarIntegridad("Cuota", "IdCuota", resultado.Cuota.IdCuota, conn, tran);
                RecomputarIntegridad("SolicitudInscripcion", "IdSolicitudInscripcion", resultado.Solicitud.IdSolicitudInscripcion, conn, tran);
                RecomputarIntegridad("FacturaCUN07", "IdFactura", resultado.IdFactura, conn, tran);
            });
            return resultado;
        }

        private static List<SqlParameter> CrearParametrosRegistro(PagoInscripcion_83KI pago)
        {
            return new List<SqlParameter>
            {
                new SqlParameter("@IdAlumno", pago.IdAlumno), new SqlParameter("@IdSolicitudInscripcion", pago.IdSolicitudInscripcion),
                new SqlParameter("@IdCuota", pago.IdCuota), new SqlParameter("@CodigoComision", Normalizar(pago.CodigoComision)),
                new SqlParameter("@MetodoPago", Normalizar(pago.MetodoPago)), new SqlParameter("@MontoRecibido", pago.MontoPagado),
                new SqlParameter("@NumeroReferencia", Normalizar(pago.NumeroReferencia)), new SqlParameter("@FechaPago", pago.FechaPago),
                new SqlParameter("@FechaAcreditacion", pago.FechaAcreditacion)
            };
        }

        private static BusquedaPagoInscripcionResultado_83KI MapearBusqueda(DataRow row)
        {
            var alumno = Alumno_83KI.ReconstruirDesdePersistencia(Convert.ToInt32(row["IdAlumno"]), row["AlumnoDNI"].ToString(), row["AlumnoNombre"].ToString(), row["AlumnoApellido"].ToString(), row["AlumnoEmail"].ToString(), row["AlumnoTelefono"].ToString(), Convert.ToDateTime(row["AlumnoFechaAlta"]), row["AlumnoEstado"].ToString(), row["AlumnoDVH"].ToString());
            int? idLead = row["IdLeadOrigen"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["IdLeadOrigen"]);
            var solicitud = MapearSolicitud(row, alumno.IdAlumno, idLead);
            return new BusquedaPagoInscripcionResultado_83KI { Alumno = alumno, Solicitud = solicitud, CodigoComision = row["CodigoComision"].ToString(), Curso = row["Curso"].ToString(), NombrePlanDePago = row["NombrePlanDePago"].ToString(), MatriculaPagada = Convert.ToBoolean(row["MatriculaPagada"]) };
        }

        private static Cuota_83KI MapearCuota(DataRow row)
        {
            return Cuota_83KI.ReconstruirDesdePersistencia(Convert.ToInt32(row["IdCuota"]), Convert.ToInt32(row["IdSolicitudInscripcion"]), Convert.ToInt32(row["NumeroCuota"]), Convert.ToDecimal(row["MontoOriginal"]), Convert.ToDecimal(row["BalanceAdeudado"]), Convert.ToDateTime(row["FechaVencimiento"]), row["CuotaEstado"].ToString(), row["CuotaDVH"].ToString(), row["TipoConcepto"].ToString(), row["Periodo"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["Periodo"]));
        }

        private static PagoInscripcionResultado_83KI MapearResultado(DataRow row)
        {
            var alumno = Alumno_83KI.ReconstruirDesdePersistencia(Convert.ToInt32(row["IdAlumno"]), row["AlumnoDNI"].ToString(), row["AlumnoNombre"].ToString(), row["AlumnoApellido"].ToString(), row["AlumnoEmail"].ToString(), row["AlumnoTelefono"].ToString(), Convert.ToDateTime(row["AlumnoFechaAlta"]), row["AlumnoEstado"].ToString(), row["AlumnoDVH"].ToString());
            int? idLead = row["IdLeadOrigen"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["IdLeadOrigen"]);
            var solicitud = MapearSolicitud(row, alumno.IdAlumno, idLead);
            var cuota = MapearCuota(row);
            var pago = PagoInscripcion_83KI.ReconstruirDesdePersistencia(Convert.ToInt32(row["IdPagoInscripcion"]), alumno.IdAlumno, solicitud.IdSolicitudInscripcion, cuota.IdCuota, row["MetodoPago"].ToString(), Convert.ToDecimal(row["MontoPagado"]), row["NumeroReferencia"].ToString(), Convert.ToDateTime(row["FechaPago"]), Convert.ToDateTime(row["FechaAcreditacion"]), row["CodigoComision"].ToString(), row["PagoDVH"].ToString());
            return new PagoInscripcionResultado_83KI { Pago = pago, Alumno = alumno, Solicitud = solicitud, Cuota = cuota, CodigoComision = row["CodigoComision"].ToString(), Curso = row["Curso"].ToString(), CodigoSolicitud = solicitud.Codigo, NumeroReciboInstitucional = row["NumeroReciboInstitucional"].ToString(), FacturaConcepto = row["ConceptoFactura"].ToString(), FacturaTotal = Convert.ToDecimal(row["TotalFactura"]), FacturaFecha = Convert.ToDateTime(row["FechaTransaccion"]), IdFactura = Convert.ToInt32(row["IdFactura"]), FacturaTexto = string.Format("{0}|{1}|{2}|{3}", row["NumeroReciboInstitucional"], row["ConceptoFactura"], row["TotalFactura"], row["FechaTransaccion"]) };
        }

        private void RecomputarIntegridad(string tabla, string pk, int id, SqlConnection conn, SqlTransaction tran)
        {
            var valores = IntegridadDAL_83KI.LeerFilaTransaccional(tabla, pk + " = @id", new List<SqlParameter> { new SqlParameter("@id", id) }, conn, tran);
            if (valores.Count > 0) _integridadDAL.RecomputarIntegridadFila(tabla, valores, conn, tran);
        }

        private static SolicitudInscripcion_83KI MapearSolicitud(DataRow row, int idAlumno, int? idLead)
        {
            return SolicitudInscripcion_83KI.ReconstruirDesdePersistencia(Convert.ToInt32(row["IdSolicitudInscripcion"]), row["CodigoSolicitud"].ToString(), idAlumno, idLead,
                Convert.ToInt32(row["IdComision"]), Convert.ToInt32(row["IdPlanDePago"]), Convert.ToDecimal(row["RecargoPlanSnapshot"]),
                Convert.ToDateTime(row["FechaSolicitudInscripcion"]), row["SolicitudEstado"].ToString(), row["SolicitudObservaciones"].ToString(), row["SolicitudDVH"].ToString(),
                Convert.ToDecimal(row["ArancelCursoSnapshot"]), Convert.ToInt32(row["CantidadCuotasSnapshot"]), Convert.ToDecimal(row["TotalCursoSnapshot"]),
                Convert.ToDateTime(row["FechaPrimerVencimientoSnapshot"]), row["NombrePlanSnapshot"].ToString());
        }

        private static string Normalizar(string value) { return (value ?? string.Empty).Trim(); }
    }
}

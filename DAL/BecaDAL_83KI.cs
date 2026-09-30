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
    public class BecaDAL_83KI : IBecaDAL_83KI
    {
        private readonly AccesoDAL_83KI _accesoDAL = new AccesoDAL_83KI();
        private readonly IntegridadDAL_83KI _integridadDAL = new IntegridadDAL_83KI(new Encriptador_83KI());

        public BusquedaDecisionBecaResultado_83KI BuscarSolicitudPorDniComision(string dni, string codigoComision)
        {
            var ds = _accesoDAL.LeerStoredProcedure("sp_CUN04_BuscarSolicitudPorDniComision", new List<SqlParameter>
            {
                new SqlParameter("@DNI", Normalizar(dni)),
                new SqlParameter("@CodigoComision", Normalizar(codigoComision))
            });

            if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0) return null;
            return MapearBusqueda(ds.Tables[0].Rows[0]);
        }

        public bool ExisteBecaParaInscripcion(string dniAlumno, string codigoComision)
        {
            const string sql = @"
SELECT CASE WHEN EXISTS (
    SELECT 1
    FROM dbo.Beca b
    INNER JOIN dbo.SolicitudInscripcion si ON si.IdSolicitudInscripcion = b.IdSolicitudInscripcion
    INNER JOIN dbo.Alumno a ON a.IdAlumno = si.IdAlumno
    INNER JOIN dbo.Comision c ON c.IdComision = b.IdComision
    WHERE a.DNI = @dni AND c.Codigo = @codigoComision
) THEN 1 ELSE 0 END";

            var ds = _accesoDAL.Leer(sql, new List<SqlParameter>
            {
                new SqlParameter("@dni", Normalizar(dniAlumno)),
                new SqlParameter("@codigoComision", Normalizar(codigoComision))
            });

            return ds != null
                && ds.Tables.Count > 0
                && ds.Tables[0].Rows.Count > 0
                && Convert.ToInt32(ds.Tables[0].Rows[0][0]) == 1;
        }

        public Beca_83KI RegistrarBeca(Beca_83KI beca)
        {
            if (beca == null) throw new ArgumentNullException(nameof(beca));

            Beca_83KI registrada = null;
            _accesoDAL.EjecutarTransaccion((conn, tran) =>
            {
                var ds = AccesoDAL_83KI.LeerStoredProcedureTransaccional(conn, tran, "sp_CUN04_RegistrarDecisionBeca", CrearParametrosRegistro(beca));
                if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0) throw new InvalidOperationException("Errores.BecaNoRegistrada");

                registrada = MapearBeca(ds.Tables[0].Rows[0]);
                RecomputarIntegridad("Beca", "IdBeca", registrada.IdBeca, conn, tran);
            });
            return registrada;
        }

        private static List<SqlParameter> CrearParametrosRegistro(Beca_83KI beca)
        {
            return new List<SqlParameter>
            {
                new SqlParameter("@IdAlumno", beca.IdAlumno),
                new SqlParameter("@IdComision", beca.IdComision),
                new SqlParameter("@IdSolicitudInscripcion", beca.IdSolicitudInscripcion),
                new SqlParameter("@TipoBeneficio", Normalizar(beca.TipoBeneficio)),
                new SqlParameter("@FechaSolicitud", beca.FechaSolicitud),
                new SqlParameter("@EstadoBeca", Normalizar(beca.EstadoBeca)),
                new SqlParameter("@MotivoDecision", Normalizar(beca.MotivoDecision))
            };
        }

        private static BusquedaDecisionBecaResultado_83KI MapearBusqueda(DataRow row)
        {
            var alumno = Alumno_83KI.ReconstruirDesdePersistencia(
                Convert.ToInt32(row["IdAlumno"]),
                row["AlumnoDNI"].ToString(),
                row["AlumnoNombre"].ToString(),
                row["AlumnoApellido"].ToString(),
                row["AlumnoEmail"].ToString(),
                row["AlumnoTelefono"].ToString(),
                Convert.ToDateTime(row["AlumnoFechaAlta"]),
                row["AlumnoEstado"].ToString(),
                row["AlumnoDVH"].ToString());

            int? idLead = row["IdLeadOrigen"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["IdLeadOrigen"]);
            var solicitud = SolicitudInscripcion_83KI.ReconstruirDesdePersistencia(
                Convert.ToInt32(row["IdSolicitudInscripcion"]),
                row["CodigoSolicitud"].ToString(),
                alumno.IdAlumno,
                idLead,
                Convert.ToInt32(row["IdComision"]),
                Convert.ToInt32(row["IdPlanDePago"]),
                Convert.ToDecimal(row["RecargoPlanSnapshot"]),
                Convert.ToDateTime(row["FechaSolicitudInscripcion"]),
                row["SolicitudEstado"].ToString(),
                row["SolicitudObservaciones"].ToString(),
                row["SolicitudDVH"].ToString());

            return new BusquedaDecisionBecaResultado_83KI
            {
                Alumno = alumno,
                Solicitud = solicitud,
                IdComision = Convert.ToInt32(row["IdComision"]),
                CodigoComision = row["CodigoComision"].ToString(),
                Curso = row["Curso"].ToString()
            };
        }

        private static Beca_83KI MapearBeca(DataRow row)
        {
            return Beca_83KI.ReconstruirDesdePersistencia(
                Convert.ToInt32(row["IdBeca"]),
                Convert.ToInt32(row["IdAlumno"]),
                Convert.ToInt32(row["IdComision"]),
                Convert.ToInt32(row["IdSolicitudInscripcion"]),
                row["TipoBeneficio"].ToString(),
                Convert.ToDateTime(row["FechaSolicitud"]),
                row["EstadoBeca"].ToString(),
                row["MotivoDecision"].ToString(),
                row["DVH"].ToString());
        }

        private void RecomputarIntegridad(string tabla, string pk, int id, SqlConnection conn, SqlTransaction tran)
        {
            var valores = IntegridadDAL_83KI.LeerFilaTransaccional(tabla, pk + " = @id", new List<SqlParameter> { new SqlParameter("@id", id) }, conn, tran);
            if (valores.Count > 0) _integridadDAL.RecomputarIntegridadFila(tabla, valores, conn, tran);
        }

        private static string Normalizar(string value) { return (value ?? string.Empty).Trim(); }
    }
}

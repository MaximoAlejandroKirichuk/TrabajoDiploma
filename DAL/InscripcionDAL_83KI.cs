using BE.Entidades;
using DAL.DAL;
using Service;
using Service.DTOs;
using Service.Entidades;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    public class InscripcionDAL_83KI : IInscripcionDAL_83KI
    {
        private readonly AccesoDAL_83KI _accesoDAL = new AccesoDAL_83KI();
        private readonly IntegridadDAL_83KI _integridadDAL = new IntegridadDAL_83KI(new Encriptador_83KI());
        private readonly AlumnoDAL_83KI _alumnoDAL = new AlumnoDAL_83KI();

        public IEnumerable<BusquedaInscripcionPersonaResultado_83KI> BuscarPersonas(string texto)
        {
            return _alumnoDAL.BuscarParaInscripcion(texto);
        }

        public IEnumerable<ComisionListado_83KI> ListarComisionesElegibles()
        {
            return MapearListado(_accesoDAL.LeerStoredProcedure("sp_CUN03_ListarComisionesElegibles"));
        }

        public IEnumerable<PlanDePago_83KI> ObtenerPlanesComision(int idComision)
        {
            return MapearPlanes(_accesoDAL.LeerStoredProcedure("sp_CUN03_ObtenerPlanesComision", new List<SqlParameter> { new SqlParameter("@IdComision", idComision) }));
        }

        public SolicitudInscripcionResultado_83KI RegistrarSolicitud(SolicitudInscripcionRequest_83KI request)
        {
            SolicitudInscripcionResultado_83KI resultado = null;
            _accesoDAL.EjecutarTransaccion((conn, tran) =>
            {
                var ds = AccesoDAL_83KI.LeerStoredProcedureTransaccional(conn, tran, "sp_CUN03_RegistrarSolicitudInscripcion", CrearParametrosRegistro(request));
                if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0) throw new InvalidOperationException("Errores.SolicitudInscripcionNoRegistrada");
                resultado = MapearResultado(ds.Tables[0].Rows[0]);
                RegistrarAuditoriaTransaccional(resultado, request.AuditoriaUsername, conn, tran);
                RecomputarIntegridad("Alumno", "IdAlumno", resultado.Alumno.IdAlumno, conn, tran);
                RecomputarIntegridad("SolicitudInscripcion", "IdSolicitudInscripcion", resultado.Solicitud.IdSolicitudInscripcion, conn, tran);
                RecomputarIntegridad("Cuota", "IdCuota", resultado.CuotaInicial.IdCuota, conn, tran);
            });
            return resultado;
        }

        private static List<SqlParameter> CrearParametrosRegistro(SolicitudInscripcionRequest_83KI request)
        {
            return new List<SqlParameter>
            {
                new SqlParameter("@DNI", Normalizar(request.DNI)),
                new SqlParameter("@Nombre", Normalizar(request.Nombre)),
                new SqlParameter("@Apellido", Normalizar(request.Apellido)),
                new SqlParameter("@Email", Normalizar(request.Email).ToLowerInvariant()),
                new SqlParameter("@Telefono", NormalizarTelefono(request.Telefono)),
                new SqlParameter("@IdLead", (object)request.IdLead ?? DBNull.Value),
                new SqlParameter("@IdAlumno", (object)request.IdAlumno ?? DBNull.Value),
                new SqlParameter("@IdComision", request.IdComision),
                new SqlParameter("@IdPlanDePago", request.IdPlanDePago),
                new SqlParameter("@Observaciones", string.IsNullOrWhiteSpace(request.Observaciones) ? (object)DBNull.Value : request.Observaciones.Trim())
            };
        }

        private static SolicitudInscripcionResultado_83KI MapearResultado(DataRow row)
        {
            var alumno = Alumno_83KI.ReconstruirDesdePersistencia(Convert.ToInt32(row["IdAlumno"]), row["AlumnoDNI"].ToString(), row["AlumnoNombre"].ToString(), row["AlumnoApellido"].ToString(), row["AlumnoEmail"].ToString(), row["AlumnoTelefono"].ToString(), Convert.ToDateTime(row["AlumnoFechaAlta"]), row["AlumnoEstado"].ToString(), row["AlumnoDVH"].ToString());
            int? idLead = row["IdLeadOrigen"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["IdLeadOrigen"]);
            var solicitud = SolicitudInscripcion_83KI.ReconstruirDesdePersistencia(Convert.ToInt32(row["IdSolicitudInscripcion"]), row["Codigo"].ToString(), alumno.IdAlumno, idLead, Convert.ToInt32(row["IdComision"]), Convert.ToInt32(row["IdPlanDePago"]), Convert.ToDecimal(row["RecargoPlanSnapshot"]), Convert.ToDateTime(row["FechaSolicitud"]), row["SolicitudEstado"].ToString(), row["Observaciones"].ToString(), row["SolicitudDVH"].ToString());
            var cuota = Cuota_83KI.ReconstruirDesdePersistencia(Convert.ToInt32(row["IdCuota"]), solicitud.IdSolicitudInscripcion, Convert.ToInt32(row["NumeroCuota"]), Convert.ToDecimal(row["MontoOriginal"]), Convert.ToDecimal(row["BalanceAdeudado"]), Convert.ToDateTime(row["FechaVencimiento"]), row["CuotaEstado"].ToString(), row["CuotaDVH"].ToString());
            return new SolicitudInscripcionResultado_83KI { Alumno = alumno, Solicitud = solicitud, CuotaInicial = cuota, CodigoSolicitud = solicitud.Codigo, VacantesDisponibles = Convert.ToInt32(row["VacantesDisponibles"]) };
        }

        private static IEnumerable<PlanDePago_83KI> MapearPlanes(DataSet ds)
        {
            var planes = new List<PlanDePago_83KI>();
            if (ds == null || ds.Tables.Count == 0) return planes;
            foreach (DataRow row in ds.Tables[0].Rows) planes.Add(PlanDePago_83KI.ReconstruirDesdePersistencia(Convert.ToInt32(row["IdPlanDePago"]), row["Nombre"].ToString(), Convert.ToBoolean(row["EstadoActivo"]), Convert.ToDecimal(row["RecargoPorcentaje"])));
            return planes;
        }

        private static IEnumerable<ComisionListado_83KI> MapearListado(DataSet ds)
        {
            var resultado = new List<ComisionListado_83KI>();
            if (ds == null || ds.Tables.Count == 0) return resultado;
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                decimal arancelBase = Convert.ToDecimal(row["ArancelBase"]);
                resultado.Add(new ComisionListado_83KI
                {
                    IdComision = Convert.ToInt32(row["IdComision"]),
                    Codigo = row["Codigo"].ToString(),
                    IdCurso = Convert.ToInt32(row["IdCurso"]),
                    Curso = row["Curso"].ToString(),
                    IdProfesor = Convert.ToInt32(row["IdProfesor"]),
                    Profesor = row["Profesor"].ToString(),
                    DiaSemana = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), row["DiaSemana"].ToString()),
                    HoraInicio = (TimeSpan)row["HoraInicio"],
                    HoraFin = (TimeSpan)row["HoraFin"],
                    CupoMinimo = Convert.ToInt32(row["CupoMinimo"]),
                    CupoMaximo = Convert.ToInt32(row["CupoMaximo"]),
                    FechaLimitePago = Convert.ToDateTime(row["FechaLimitePago"]),
                    FechaInicio = Convert.ToDateTime(row["FechaInicio"]),
                    FechaFin = Convert.ToDateTime(row["FechaFin"]),
                    ArancelBase = arancelBase,
                    MontoMatricula = row.Table.Columns.Contains("MontoMatricula") && row["MontoMatricula"] != DBNull.Value ? Convert.ToDecimal(row["MontoMatricula"]) : arancelBase,
                    VacantesDisponibles = row.Table.Columns.Contains("VacantesDisponibles") ? Convert.ToInt32(row["VacantesDisponibles"]) : 0,
                    Estado = row["Estado"].ToString()
                });
            }
            return resultado;
        }

        private void RecomputarIntegridad(string tabla, string pk, int id, SqlConnection conn, SqlTransaction tran)
        {
            var valores = IntegridadDAL_83KI.LeerFilaTransaccional(tabla, pk + " = @id", new List<SqlParameter> { new SqlParameter("@id", id) }, conn, tran);
            if (valores.Count > 0) _integridadDAL.RecomputarIntegridadFila(tabla, valores, conn, tran);
        }

        private static void RegistrarAuditoriaTransaccional(SolicitudInscripcionResultado_83KI resultado, string username, SqlConnection conn, SqlTransaction tran)
        {
            if (resultado == null) throw new ArgumentNullException(nameof(resultado));

            const string sql = "INSERT INTO dbo.BitacoraEventos (Criticidad, Descripcion, Fecha, Modulo, Username) VALUES (@crit, @desc, @fecha, @mod, @username)";
            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@crit", (int)Criticidad.Medio),
                new SqlParameter("@desc", $"Registro de solicitud de inscripción: {resultado.CodigoSolicitud}. Alumno {resultado.Alumno.IdAlumno}."),
                new SqlParameter("@fecha", DateTime.Now),
                new SqlParameter("@mod", Modulo.PreInscripcion.ToString()),
                new SqlParameter("@username", string.IsNullOrWhiteSpace(username) ? "Sistema" : username.Trim())
            };

            AccesoDAL_83KI.EscribirTransaccional(conn, tran, sql, parametros);
        }

        private static string Normalizar(string value) { return (value ?? string.Empty).Trim(); }
        private static string NormalizarTelefono(string value) { return Normalizar(value).Replace(" ", string.Empty).Replace("-", string.Empty); }
    }
}

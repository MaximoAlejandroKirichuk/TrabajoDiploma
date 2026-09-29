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
    public class ComisionDAL_83KI : IComisionDAL_83KI
    {
        private readonly AccesoDAL_83KI _accesoDAL = new AccesoDAL_83KI();
        private readonly IntegridadDAL_83KI _integridadDAL = new IntegridadDAL_83KI(new Encriptador_83KI());

        public IEnumerable<Curso_83KI> ObtenerCursosActivosParaPreapertura()
        {
            return MapearCursos(_accesoDAL.LeerStoredProcedure("sp_CUN01_ListarCursosActivos"));
        }

        public IEnumerable<Profesor_83KI> ObtenerProfesoresActivosParaFiltro()
        {
            return MapearProfesores(_accesoDAL.Leer("SELECT IdProfesor, DNI, Nombre, Apellido, Email, EstadoActivo, DVH FROM Profesor WHERE EstadoActivo = 1 ORDER BY Apellido, Nombre"));
        }

        public IEnumerable<Profesor_83KI> ObtenerProfesoresDisponibles(int idCurso, DayOfWeek diaSemana, TimeSpan horaInicio, TimeSpan horaFin)
        {
            return MapearProfesores(_accesoDAL.LeerStoredProcedure("sp_CUN01_ListarProfesoresDisponibles", ParametrosDisponibilidad(idCurso, 0, diaSemana, horaInicio, horaFin)));
        }

        public bool ValidarCursoProfesorDisponibilidad(int idCurso, int idProfesor, DayOfWeek diaSemana, TimeSpan horaInicio, TimeSpan horaFin)
        {
            object resultado = _accesoDAL.LeerEscalarStoredProcedure("sp_CUN01_ValidarCursoProfesorDisponibilidad", ParametrosDisponibilidad(idCurso, idProfesor, diaSemana, horaInicio, horaFin));
            return Convert.ToInt32(resultado) > 0;
        }

        public bool ExisteSolapamientoComision(int idProfesor, DayOfWeek diaSemana, TimeSpan horaInicio, TimeSpan horaFin, int? idComisionIgnorar = null)
        {
            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@IdProfesor", idProfesor),
                new SqlParameter("@DiaSemana", diaSemana.ToString()),
                new SqlParameter("@HoraInicio", horaInicio),
                new SqlParameter("@HoraFin", horaFin),
                new SqlParameter("@IdComisionIgnorar", (object)idComisionIgnorar ?? DBNull.Value)
            };
            object resultado = _accesoDAL.LeerEscalarStoredProcedure("sp_CUN01_ValidarSolapamientoProfesor", parametros);
            return Convert.ToInt32(resultado) > 0;
        }

        public IEnumerable<PlanDePago_83KI> ObtenerPlanesDePagoActivos()
        {
            return MapearPlanes(_accesoDAL.LeerStoredProcedure("sp_CUN01_ListarPlanesDePagoActivos"));
        }

        public Comision_83KI RegistrarPreapertura(Comision_83KI comision)
        {
            Comision_83KI registrada = null;
            _accesoDAL.EjecutarTransaccion((conn, tran) =>
            {
                DataSet ds = AccesoDAL_83KI.LeerStoredProcedureTransaccional(conn, tran, "sp_CUN01_RegistrarPreaperturaComision", CrearParametrosRegistro(comision));
                if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0) throw new InvalidOperationException("Errores.ComisionNoRegistrada");
                registrada = MapearComision(ds.Tables[0].Rows[0]);
                RecomputarIntegridadComision(registrada.IdComision, conn, tran);
            });
            return registrada;
        }

        public IEnumerable<ComisionListado_83KI> ListarComisiones(FiltroComision_83KI filtro)
        {
            return MapearListado(_accesoDAL.LeerStoredProcedure("sp_CUN01_ListarComisiones", CrearParametrosFiltro(filtro)));
        }

        public Comision_83KI ObtenerComision(int idComision)
        {
            var ds = _accesoDAL.LeerStoredProcedure("sp_CUN01_ObtenerComision", new List<SqlParameter> { new SqlParameter("@IdComision", idComision) });
            if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0) return null;
            return MapearComision(ds.Tables[0].Rows[0]);
        }

        public void ModificarComision(Comision_83KI comision)
        {
            _accesoDAL.EjecutarTransaccion((conn, tran) =>
            {
                AccesoDAL_83KI.EscribirStoredProcedureTransaccional(conn, tran, "sp_CUN01_ModificarComision", CrearParametrosModificacion(comision));
                RecomputarIntegridadComision(comision.IdComision, conn, tran);
            });
        }

        public void EliminarComision(int idComision)
        {
            _accesoDAL.EjecutarTransaccion((conn, tran) =>
            {
                AccesoDAL_83KI.EscribirStoredProcedureTransaccional(conn, tran, "sp_CUN01_EliminarComisionLogica", new List<SqlParameter> { new SqlParameter("@IdComision", idComision) });
                RecomputarIntegridadComision(idComision, conn, tran);
            });
        }

        public IEnumerable<ComisionListado_83KI> ListarComisionesPreapertura()
        {
            return MapearListado(_accesoDAL.LeerStoredProcedure("sp_CUN02_ListarComisionesPreapertura"));
        }

        public bool ExisteComisionPreapertura(int idComision)
        {
            object resultado = _accesoDAL.LeerEscalar("SELECT CASE WHEN EXISTS (SELECT 1 FROM dbo.Comision WHERE IdComision = @id AND Estado = N'preapertura' AND FechaLimitePago >= CONVERT(date, GETDATE())) THEN 1 ELSE 0 END", new List<SqlParameter> { new SqlParameter("@id", idComision) });
            return Convert.ToInt32(resultado) > 0;
        }

        private static List<SqlParameter> ParametrosDisponibilidad(int idCurso, int idProfesor, DayOfWeek diaSemana, TimeSpan horaInicio, TimeSpan horaFin)
        {
            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@IdCurso", idCurso),
                new SqlParameter("@DiaSemana", diaSemana.ToString()),
                new SqlParameter("@HoraInicio", horaInicio),
                new SqlParameter("@HoraFin", horaFin)
            };
            if (idProfesor > 0) parametros.Add(new SqlParameter("@IdProfesor", idProfesor));
            return parametros;
        }

        private static List<SqlParameter> CrearParametrosRegistro(Comision_83KI comision)
        {
            return new List<SqlParameter>
            {
                new SqlParameter("@IdCurso", comision.IdCurso),
                new SqlParameter("@IdProfesor", comision.IdProfesor),
                new SqlParameter("@DiaSemana", comision.DiaSemana.ToString()),
                new SqlParameter("@HoraInicio", comision.HoraInicio),
                new SqlParameter("@HoraFin", comision.HoraFin),
                new SqlParameter("@CupoMinimo", comision.CupoMinimo),
                new SqlParameter("@CupoMaximo", comision.CupoMaximo),
                new SqlParameter("@FechaLimitePago", comision.FechaLimitePago),
                new SqlParameter("@FechaInicio", comision.FechaInicio),
                new SqlParameter("@FechaFin", comision.FechaFin),
                new SqlParameter("@ArancelBase", SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = comision.ArancelBase },
                new SqlParameter("@MontoMatricula", SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = comision.MontoMatricula },
                new SqlParameter("@IdPlanDePago", comision.IdPlanDePago),
                new SqlParameter("@RecargoPlanSnapshot", SqlDbType.Decimal) { Precision = 9, Scale = 4, Value = comision.RecargoPlanSnapshot }
            };
        }

        private static List<SqlParameter> CrearParametrosFiltro(FiltroComision_83KI filtro)
        {
            filtro = filtro ?? new FiltroComision_83KI();
            return new List<SqlParameter>
            {
                new SqlParameter("@IdCurso", (object)filtro.IdCurso ?? DBNull.Value),
                new SqlParameter("@IdProfesor", (object)filtro.IdProfesor ?? DBNull.Value),
                new SqlParameter("@Estado", string.IsNullOrWhiteSpace(filtro.Estado) ? (object)DBNull.Value : filtro.Estado),
                new SqlParameter("@FechaInicioDesde", (object)filtro.FechaInicioDesde ?? DBNull.Value),
                new SqlParameter("@FechaFinHasta", (object)filtro.FechaFinHasta ?? DBNull.Value),
                new SqlParameter("@Codigo", string.IsNullOrWhiteSpace(filtro.Codigo) ? (object)DBNull.Value : filtro.Codigo.Trim())
            };
        }

        private static List<SqlParameter> CrearParametrosModificacion(Comision_83KI comision)
        {
            return new List<SqlParameter>
            {
                new SqlParameter("@IdComision", comision.IdComision),
                new SqlParameter("@IdCurso", comision.IdCurso),
                new SqlParameter("@IdProfesor", comision.IdProfesor),
                new SqlParameter("@DiaSemana", comision.DiaSemana.ToString()),
                new SqlParameter("@HoraInicio", comision.HoraInicio),
                new SqlParameter("@HoraFin", comision.HoraFin),
                new SqlParameter("@CupoMinimo", comision.CupoMinimo),
                new SqlParameter("@CupoMaximo", comision.CupoMaximo),
                new SqlParameter("@FechaLimitePago", comision.FechaLimitePago),
                new SqlParameter("@FechaInicio", comision.FechaInicio),
                new SqlParameter("@FechaFin", comision.FechaFin),
                new SqlParameter("@ArancelBase", SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = comision.ArancelBase }
            };
        }

        private static IEnumerable<PlanDePago_83KI> MapearPlanes(DataSet ds)
        {
            var planes = new List<PlanDePago_83KI>();
            if (ds == null || ds.Tables.Count == 0) return planes;
            foreach (DataRow row in ds.Tables[0].Rows)
                planes.Add(PlanDePago_83KI.ReconstruirDesdePersistencia(Convert.ToInt32(row["IdPlanDePago"]), row["Nombre"].ToString(), Convert.ToBoolean(row["EstadoActivo"]), Convert.ToDecimal(row["RecargoPorcentaje"])));
            return planes;
        }

        private static IEnumerable<Curso_83KI> MapearCursos(DataSet ds)
        {
            var cursos = new List<Curso_83KI>();
            if (ds == null || ds.Tables.Count == 0) return cursos;
            foreach (DataRow row in ds.Tables[0].Rows)
                cursos.Add(Curso_83KI.ReconstruirDesdePersistencia(Convert.ToInt32(row["IdCurso"]), row["Nombre"].ToString(), row["Descripcion"].ToString(), Convert.ToInt32(row["CargaHoraria"]), Convert.ToBoolean(row["EstadoActivo"]), row["DVH"].ToString()));
            return cursos;
        }

        private static IEnumerable<Profesor_83KI> MapearProfesores(DataSet ds)
        {
            var profesores = new List<Profesor_83KI>();
            if (ds == null || ds.Tables.Count == 0) return profesores;
            foreach (DataRow row in ds.Tables[0].Rows)
                profesores.Add(Profesor_83KI.ReconstruirDesdePersistencia(Convert.ToInt32(row["IdProfesor"]), row["DNI"].ToString(), row["Nombre"].ToString(), row["Apellido"].ToString(), row["Email"].ToString(), Convert.ToBoolean(row["EstadoActivo"]), row["DVH"].ToString()));
            return profesores;
        }

        private static Comision_83KI MapearComision(DataRow row)
        {
            decimal arancelBase = Convert.ToDecimal(row["ArancelBase"]);
            decimal montoMatricula = row.Table.Columns.Contains("MontoMatricula") && row["MontoMatricula"] != DBNull.Value ? Convert.ToDecimal(row["MontoMatricula"]) : arancelBase;
            return Comision_83KI.ReconstruirDesdePersistencia(Convert.ToInt32(row["IdComision"]), row["Codigo"].ToString(), Convert.ToInt32(row["IdCurso"]), Convert.ToInt32(row["IdProfesor"]), (DayOfWeek)Enum.Parse(typeof(DayOfWeek), row["DiaSemana"].ToString()), (TimeSpan)row["HoraInicio"], (TimeSpan)row["HoraFin"], Convert.ToInt32(row["CupoMinimo"]), Convert.ToInt32(row["CupoMaximo"]), Convert.ToDateTime(row["FechaLimitePago"]), Convert.ToDateTime(row["FechaInicio"]), Convert.ToDateTime(row["FechaFin"]), arancelBase, montoMatricula, Convert.ToInt32(row["IdPlanDePago"]), Convert.ToDecimal(row["RecargoPlanSnapshot"]), row["Estado"].ToString(), row["DVH"].ToString());
        }

        private static IEnumerable<ComisionListado_83KI> MapearListado(DataSet ds)
        {
            var resultado = new List<ComisionListado_83KI>();
            if (ds == null || ds.Tables.Count == 0) return resultado;
            foreach (DataRow row in ds.Tables[0].Rows)
            {
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
                    ArancelBase = Convert.ToDecimal(row["ArancelBase"]),
                    MontoMatricula = row.Table.Columns.Contains("MontoMatricula") && row["MontoMatricula"] != DBNull.Value ? Convert.ToDecimal(row["MontoMatricula"]) : Convert.ToDecimal(row["ArancelBase"]),
                    Estado = row["Estado"].ToString()
                });
            }
            return resultado;
        }

        private static IEnumerable<Comision_83KI> MapearComisiones(DataSet ds)
        {
            var comisiones = new List<Comision_83KI>();
            if (ds == null || ds.Tables.Count == 0) return comisiones;
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                comisiones.Add(MapearComision(row));
            }
            return comisiones;
        }

        private void RecomputarIntegridadComision(int idComision, SqlConnection conn, SqlTransaction tran)
        {
            var valores = IntegridadDAL_83KI.LeerFilaTransaccional("Comision", "IdComision = @id", new List<SqlParameter> { new SqlParameter("@id", idComision) }, conn, tran);
            if (valores.Count > 0) _integridadDAL.RecomputarIntegridadFila("Comision", valores, conn, tran);
        }
    }
}

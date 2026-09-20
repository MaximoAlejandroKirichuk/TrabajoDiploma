using DAL.DAL;
using DAL.interfaces;
using Service;
using BE.Entidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    public class KI68CursoProfesorDAL : ICursoProfesorDAL_83KI
    {
        private readonly AccesoDAL_83KI _accesoDAL = new AccesoDAL_83KI();
        private readonly IntegridadDAL_83KI _integridadDAL = new IntegridadDAL_83KI(new Encriptador_83KI());

        public IEnumerable<Curso_83KI> ObtenerCursosActivosParaRelacion()
        {
            return MapearCursos(_accesoDAL.Leer("SELECT IdCurso, Nombre, Descripcion, CargaHoraria, EstadoActivo, DVH FROM Curso WHERE EstadoActivo = 1 ORDER BY Nombre"));
        }

        public IEnumerable<Profesor_83KI> ObtenerProfesoresActivosParaRelacion()
        {
            return MapearProfesores(_accesoDAL.Leer("SELECT IdProfesor, DNI, Nombre, Apellido, Email, EstadoActivo, DVH FROM Profesor WHERE EstadoActivo = 1 ORDER BY Apellido, Nombre"));
        }

        public IEnumerable<CursoProfesor_83KI> ObtenerRelaciones()
        {
            return MapearLista(_accesoDAL.Leer(SqlBase() + " ORDER BY c.Nombre, p.Apellido, p.Nombre"));
        }

        public IEnumerable<CursoProfesor_83KI> ObtenerProfesoresHabilitadosParaCurso(int idCurso)
        {
            string sql = SqlBase() + @" WHERE cp.IdCurso = @idCurso AND cp.EstadoActivo = 1 AND c.EstadoActivo = 1 AND p.EstadoActivo = 1
                                      ORDER BY p.Apellido, p.Nombre";
            return MapearLista(_accesoDAL.Leer(sql, new List<SqlParameter> { new SqlParameter("@idCurso", idCurso) }));
        }

        public CursoProfesor_83KI ObtenerPorIds(int idCurso, int idProfesor)
        {
            var ds = _accesoDAL.Leer(SqlBase() + " WHERE cp.IdCurso = @idCurso AND cp.IdProfesor = @idProfesor", CrearParametrosClave(idCurso, idProfesor));
            if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0) return null;
            return Mapear(ds.Tables[0].Rows[0]);
        }

        public bool ExisteCursoActivo(int idCurso)
        {
            object total = _accesoDAL.LeerEscalar("SELECT COUNT(1) FROM Curso WHERE IdCurso = @idCurso AND EstadoActivo = 1", new List<SqlParameter> { new SqlParameter("@idCurso", idCurso) });
            return Convert.ToInt32(total) > 0;
        }

        public bool ExisteProfesorActivo(int idProfesor)
        {
            object total = _accesoDAL.LeerEscalar("SELECT COUNT(1) FROM Profesor WHERE IdProfesor = @idProfesor AND EstadoActivo = 1", new List<SqlParameter> { new SqlParameter("@idProfesor", idProfesor) });
            return Convert.ToInt32(total) > 0;
        }

        public bool EstaProfesorHabilitadoParaCurso(int idCurso, int idProfesor)
        {
            object total = _accesoDAL.LeerEscalar(@"SELECT COUNT(1)
                                                    FROM CursoProfesor cp
                                                    INNER JOIN Curso c ON c.IdCurso = cp.IdCurso
                                                    INNER JOIN Profesor p ON p.IdProfesor = cp.IdProfesor
                                                    WHERE cp.IdCurso = @idCurso
                                                      AND cp.IdProfesor = @idProfesor
                                                      AND cp.EstadoActivo = 1
                                                      AND c.EstadoActivo = 1
                                                      AND p.EstadoActivo = 1", CrearParametrosClave(idCurso, idProfesor));
            return Convert.ToInt32(total) > 0;
        }

        public void CrearRelacion(CursoProfesor_83KI relacion)
        {
            _accesoDAL.EjecutarTransaccion((conn, tran) =>
            {
                AccesoDAL_83KI.EscribirTransaccional(conn, tran,
                    "INSERT INTO CursoProfesor (IdCurso, IdProfesor, EstadoActivo) VALUES (@idCurso, @idProfesor, @activo)",
                    new List<SqlParameter>
                    {
                        new SqlParameter("@idCurso", relacion.IdCurso),
                        new SqlParameter("@idProfesor", relacion.IdProfesor),
                        new SqlParameter("@activo", relacion.EstadoActivo)
                    });
                RecomputarIntegridadRelacion(relacion.IdCurso, relacion.IdProfesor, conn, tran);
            });
        }

        public void ActualizarEstadoActivo(int idCurso, int idProfesor, bool activo)
        {
            _accesoDAL.EjecutarTransaccion((conn, tran) =>
            {
                AccesoDAL_83KI.EscribirTransaccional(conn, tran,
                    "UPDATE CursoProfesor SET EstadoActivo = @activo WHERE IdCurso = @idCurso AND IdProfesor = @idProfesor",
                    new List<SqlParameter>
                    {
                        new SqlParameter("@activo", activo),
                        new SqlParameter("@idCurso", idCurso),
                        new SqlParameter("@idProfesor", idProfesor)
                    });
                RecomputarIntegridadRelacion(idCurso, idProfesor, conn, tran);
            });
        }

        private static string SqlBase()
        {
            return @"SELECT cp.IdCurso, cp.IdProfesor, cp.EstadoActivo, cp.DVH,
                           c.Nombre AS CursoNombre, p.Apellido + ', ' + p.Nombre AS ProfesorNombre,
                           c.EstadoActivo AS CursoActivo, p.EstadoActivo AS ProfesorActivo
                    FROM CursoProfesor cp
                    INNER JOIN Curso c ON c.IdCurso = cp.IdCurso
                    INNER JOIN Profesor p ON p.IdProfesor = cp.IdProfesor";
        }

        private static List<SqlParameter> CrearParametrosClave(int idCurso, int idProfesor)
        {
            return new List<SqlParameter> { new SqlParameter("@idCurso", idCurso), new SqlParameter("@idProfesor", idProfesor) };
        }

        private IEnumerable<CursoProfesor_83KI> MapearLista(DataSet ds)
        {
            var relaciones = new List<CursoProfesor_83KI>();
            if (ds == null || ds.Tables.Count == 0) return relaciones;
            foreach (DataRow row in ds.Tables[0].Rows) relaciones.Add(Mapear(row));
            return relaciones;
        }

        private CursoProfesor_83KI Mapear(DataRow row)
        {
            return CursoProfesor_83KI.ReconstruirDesdePersistencia(
                Convert.ToInt32(row["IdCurso"]),
                Convert.ToInt32(row["IdProfesor"]),
                Convert.ToBoolean(row["EstadoActivo"]),
                row["DVH"].ToString(),
                row["CursoNombre"].ToString(),
                row["ProfesorNombre"].ToString(),
                Convert.ToBoolean(row["CursoActivo"]),
                Convert.ToBoolean(row["ProfesorActivo"]));
        }

        private IEnumerable<Curso_83KI> MapearCursos(DataSet ds)
        {
            var cursos = new List<Curso_83KI>();
            if (ds == null || ds.Tables.Count == 0) return cursos;
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                cursos.Add(Curso_83KI.ReconstruirDesdePersistencia(Convert.ToInt32(row["IdCurso"]), row["Nombre"].ToString(), row["Descripcion"].ToString(), Convert.ToInt32(row["CargaHoraria"]), Convert.ToBoolean(row["EstadoActivo"]), row["DVH"].ToString()));
            }
            return cursos;
        }

        private IEnumerable<Profesor_83KI> MapearProfesores(DataSet ds)
        {
            var profesores = new List<Profesor_83KI>();
            if (ds == null || ds.Tables.Count == 0) return profesores;
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                profesores.Add(Profesor_83KI.ReconstruirDesdePersistencia(Convert.ToInt32(row["IdProfesor"]), row["DNI"].ToString(), row["Nombre"].ToString(), row["Apellido"].ToString(), row["Email"].ToString(), Convert.ToBoolean(row["EstadoActivo"]), row["DVH"].ToString()));
            }
            return profesores;
        }

        private void RecomputarIntegridadRelacion(int idCurso, int idProfesor, SqlConnection conn, SqlTransaction tran)
        {
            var valores = IntegridadDAL_83KI.LeerFilaTransaccional(
                "CursoProfesor",
                "IdCurso = @idCurso AND IdProfesor = @idProfesor",
                CrearParametrosClave(idCurso, idProfesor),
                conn,
                tran);
            if (valores.Count > 0) _integridadDAL.RecomputarIntegridadFila("CursoProfesor", valores, conn, tran);
        }
    }
}

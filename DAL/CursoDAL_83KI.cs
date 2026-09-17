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
    public class CursoDAL_83KI : ICursoDAL_83KI
    {
        private readonly AccesoDAL_83KI _accesoDAL = new AccesoDAL_83KI();
        private readonly IntegridadDAL_83KI _integridadDAL = new IntegridadDAL_83KI(new Encriptador_83KI());

        public IEnumerable<Curso_83KI> ObtenerTodos()
        {
            return MapearLista(_accesoDAL.Leer("SELECT IdCurso, Nombre, Descripcion, CargaHoraria, EstadoActivo, DVH FROM Curso ORDER BY Nombre"));
        }

        public IEnumerable<Curso_83KI> ObtenerActivos()
        {
            return MapearLista(_accesoDAL.Leer("SELECT IdCurso, Nombre, Descripcion, CargaHoraria, EstadoActivo, DVH FROM Curso WHERE EstadoActivo = 1 ORDER BY Nombre"));
        }

        public Curso_83KI ObtenerPorId(int idCurso)
        {
            var ds = _accesoDAL.Leer("SELECT IdCurso, Nombre, Descripcion, CargaHoraria, EstadoActivo, DVH FROM Curso WHERE IdCurso = @id", new List<SqlParameter> { new SqlParameter("@id", idCurso) });
            if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0) return null;
            return Mapear(ds.Tables[0].Rows[0]);
        }

        public bool ExisteNombreParaOtroCurso(string nombre, int idCurso)
        {
            object total = _accesoDAL.LeerEscalar("SELECT COUNT(1) FROM Curso WHERE UPPER(LTRIM(RTRIM(Nombre))) = UPPER(LTRIM(RTRIM(@nombre))) AND IdCurso <> @id", new List<SqlParameter> { new SqlParameter("@nombre", nombre), new SqlParameter("@id", idCurso) });
            return Convert.ToInt32(total) > 0;
        }

        public int Crear(Curso_83KI curso)
        {
            int idGenerado = 0;
            _accesoDAL.EjecutarTransaccion((conn, tran) =>
            {
                string sql = @"INSERT INTO Curso (Nombre, Descripcion, CargaHoraria, EstadoActivo)
                               VALUES (@nombre, @descripcion, @cargaHoraria, @activo);
                               SELECT CAST(SCOPE_IDENTITY() AS int);";
                idGenerado = Convert.ToInt32(AccesoDAL_83KI.LeerEscalarTransaccional(conn, tran, sql, CrearParametros(curso)));
                RecomputarIntegridadCurso(idGenerado, conn, tran);
            });
            return idGenerado;
        }

        public void Modificar(Curso_83KI curso)
        {
            _accesoDAL.EjecutarTransaccion((conn, tran) =>
            {
                string sql = "UPDATE Curso SET Nombre = @nombre, Descripcion = @descripcion, CargaHoraria = @cargaHoraria WHERE IdCurso = @id";
                var parametros = CrearParametros(curso);
                parametros.Add(new SqlParameter("@id", curso.IdCurso));
                AccesoDAL_83KI.EscribirTransaccional(conn, tran, sql, parametros);
                RecomputarIntegridadCurso(curso.IdCurso, conn, tran);
            });
        }

        public void ActualizarEstadoActivo(int idCurso, bool activo)
        {
            _accesoDAL.EjecutarTransaccion((conn, tran) =>
            {
                AccesoDAL_83KI.EscribirTransaccional(conn, tran, "UPDATE Curso SET EstadoActivo = @activo WHERE IdCurso = @id", new List<SqlParameter> { new SqlParameter("@activo", activo), new SqlParameter("@id", idCurso) });
                RecomputarIntegridadCurso(idCurso, conn, tran);
            });
        }

        private static List<SqlParameter> CrearParametros(Curso_83KI curso)
        {
            return new List<SqlParameter> { new SqlParameter("@nombre", curso.Nombre), new SqlParameter("@descripcion", curso.Descripcion), new SqlParameter("@cargaHoraria", curso.CargaHoraria), new SqlParameter("@activo", curso.EstadoActivo) };
        }

        private IEnumerable<Curso_83KI> MapearLista(DataSet ds)
        {
            var cursos = new List<Curso_83KI>();
            if (ds == null || ds.Tables.Count == 0) return cursos;
            foreach (DataRow row in ds.Tables[0].Rows) cursos.Add(Mapear(row));
            return cursos;
        }

        private Curso_83KI Mapear(DataRow row)
        {
            return Curso_83KI.ReconstruirDesdePersistencia(Convert.ToInt32(row["IdCurso"]), row["Nombre"].ToString(), row["Descripcion"].ToString(), Convert.ToInt32(row["CargaHoraria"]), Convert.ToBoolean(row["EstadoActivo"]), row["DVH"].ToString());
        }

        private void RecomputarIntegridadCurso(int idCurso, SqlConnection conn, SqlTransaction tran)
        {
            var valores = IntegridadDAL_83KI.LeerFilaTransaccional("Curso", "IdCurso = @id", new List<SqlParameter> { new SqlParameter("@id", idCurso) }, conn, tran);
            if (valores.Count > 0) _integridadDAL.RecomputarIntegridadFila("Curso", valores, conn, tran);
        }
    }
}

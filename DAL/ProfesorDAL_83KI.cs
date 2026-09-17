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
    public class ProfesorDAL_83KI : IProfesorDAL_83KI
    {
        private readonly AccesoDAL_83KI _accesoDAL = new AccesoDAL_83KI();
        private readonly IntegridadDAL_83KI _integridadDAL = new IntegridadDAL_83KI(new Encriptador_83KI());

        public IEnumerable<Profesor_83KI> ObtenerTodos()
        {
            return MapearLista(_accesoDAL.Leer("SELECT IdProfesor, DNI, Nombre, Apellido, Email, EstadoActivo, DVH FROM Profesor ORDER BY Apellido, Nombre"));
        }

        public IEnumerable<Profesor_83KI> ObtenerActivos()
        {
            return MapearLista(_accesoDAL.Leer("SELECT IdProfesor, DNI, Nombre, Apellido, Email, EstadoActivo, DVH FROM Profesor WHERE EstadoActivo = 1 ORDER BY Apellido, Nombre"));
        }

        public Profesor_83KI ObtenerPorId(int idProfesor)
        {
            var ds = _accesoDAL.Leer("SELECT IdProfesor, DNI, Nombre, Apellido, Email, EstadoActivo, DVH FROM Profesor WHERE IdProfesor = @id", new List<SqlParameter> { new SqlParameter("@id", idProfesor) });
            if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0) return null;
            return Mapear(ds.Tables[0].Rows[0]);
        }

        public bool ExisteDniParaOtroProfesor(string dni, int idProfesor)
        {
            object total = _accesoDAL.LeerEscalar("SELECT COUNT(1) FROM Profesor WHERE DNI = @dni AND IdProfesor <> @id", new List<SqlParameter> { new SqlParameter("@dni", dni), new SqlParameter("@id", idProfesor) });
            return Convert.ToInt32(total) > 0;
        }

        public int Crear(Profesor_83KI profesor)
        {
            int idGenerado = 0;
            _accesoDAL.EjecutarTransaccion((conn, tran) =>
            {
                string sql = @"INSERT INTO Profesor (DNI, Nombre, Apellido, Email, EstadoActivo)
                               VALUES (@dni, @nombre, @apellido, @email, @activo);
                               SELECT CAST(SCOPE_IDENTITY() AS int);";
                idGenerado = Convert.ToInt32(AccesoDAL_83KI.LeerEscalarTransaccional(conn, tran, sql, CrearParametros(profesor)));
                RecomputarIntegridadProfesor(idGenerado, conn, tran);
            });
            return idGenerado;
        }

        public void Modificar(Profesor_83KI profesor)
        {
            _accesoDAL.EjecutarTransaccion((conn, tran) =>
            {
                string sql = "UPDATE Profesor SET Nombre = @nombre, Apellido = @apellido, Email = @email WHERE IdProfesor = @id";
                var parametros = CrearParametros(profesor);
                parametros.Add(new SqlParameter("@id", profesor.IdProfesor));
                AccesoDAL_83KI.EscribirTransaccional(conn, tran, sql, parametros);
                RecomputarIntegridadProfesor(profesor.IdProfesor, conn, tran);
            });
        }

        public void ActualizarEstadoActivo(int idProfesor, bool activo)
        {
            _accesoDAL.EjecutarTransaccion((conn, tran) =>
            {
                AccesoDAL_83KI.EscribirTransaccional(conn, tran, "UPDATE Profesor SET EstadoActivo = @activo WHERE IdProfesor = @id", new List<SqlParameter> { new SqlParameter("@activo", activo), new SqlParameter("@id", idProfesor) });
                RecomputarIntegridadProfesor(idProfesor, conn, tran);
            });
        }

        private static List<SqlParameter> CrearParametros(Profesor_83KI profesor)
        {
            return new List<SqlParameter> { new SqlParameter("@dni", profesor.DNI), new SqlParameter("@nombre", profesor.Nombre), new SqlParameter("@apellido", profesor.Apellido), new SqlParameter("@email", profesor.Email), new SqlParameter("@activo", profesor.EstadoActivo) };
        }

        private IEnumerable<Profesor_83KI> MapearLista(DataSet ds)
        {
            var profesores = new List<Profesor_83KI>();
            if (ds == null || ds.Tables.Count == 0) return profesores;
            foreach (DataRow row in ds.Tables[0].Rows) profesores.Add(Mapear(row));
            return profesores;
        }

        private Profesor_83KI Mapear(DataRow row)
        {
            return Profesor_83KI.ReconstruirDesdePersistencia(Convert.ToInt32(row["IdProfesor"]), row["DNI"].ToString(), row["Nombre"].ToString(), row["Apellido"].ToString(), row["Email"].ToString(), Convert.ToBoolean(row["EstadoActivo"]), row["DVH"].ToString());
        }

        private void RecomputarIntegridadProfesor(int idProfesor, SqlConnection conn, SqlTransaction tran)
        {
            var valores = IntegridadDAL_83KI.LeerFilaTransaccional("Profesor", "IdProfesor = @id", new List<SqlParameter> { new SqlParameter("@id", idProfesor) }, conn, tran);
            if (valores.Count > 0) _integridadDAL.RecomputarIntegridadFila("Profesor", valores, conn, tran);
        }
    }
}

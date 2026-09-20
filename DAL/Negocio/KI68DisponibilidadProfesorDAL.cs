using BE.Entidades;
using DAL.DAL;
using Service;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    public class KI68DisponibilidadProfesorDAL : IDisponibilidadProfesorDAL_83KI
    {
        private readonly AccesoDAL_83KI _accesoDAL = new AccesoDAL_83KI();
        private readonly IntegridadDAL_83KI _integridadDAL = new IntegridadDAL_83KI(new Encriptador_83KI());

        public IEnumerable<DisponibilidadProfesor_83KI> ObtenerPorProfesor(int idProfesor)
        {
            var ds = _accesoDAL.LeerStoredProcedure("sp_CUN01_ListarDisponibilidadProfesor", new List<SqlParameter> { new SqlParameter("@IdProfesor", idProfesor) });
            var disponibilidades = new List<DisponibilidadProfesor_83KI>();
            if (ds == null || ds.Tables.Count == 0) return disponibilidades;
            foreach (DataRow row in ds.Tables[0].Rows) disponibilidades.Add(Mapear(row));
            return disponibilidades;
        }

        public int Insertar(DisponibilidadProfesor_83KI disponibilidad)
        {
            int id = 0;
            _accesoDAL.EjecutarTransaccion((conn, tran) =>
            {
                var ds = AccesoDAL_83KI.LeerStoredProcedureTransaccional(conn, tran, "sp_CUN01_InsertarDisponibilidadProfesor", CrearParametros(disponibilidad));
                id = Convert.ToInt32(ds.Tables[0].Rows[0]["IdDisponibilidadProfesor"]);
                RecomputarIntegridad(id, conn, tran);
            });
            return id;
        }

        public void Actualizar(DisponibilidadProfesor_83KI disponibilidad)
        {
            _accesoDAL.EjecutarTransaccion((conn, tran) =>
            {
                AccesoDAL_83KI.EscribirStoredProcedureTransaccional(conn, tran, "sp_CUN01_ActualizarDisponibilidadProfesor", CrearParametros(disponibilidad, true));
                RecomputarIntegridad(disponibilidad.IdDisponibilidadProfesor, conn, tran);
            });
        }

        private static List<SqlParameter> CrearParametros(DisponibilidadProfesor_83KI disponibilidad, bool incluirId = false)
        {
            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@IdProfesor", disponibilidad.IdProfesor),
                new SqlParameter("@DiaSemana", disponibilidad.DiaSemana.ToString()),
                new SqlParameter("@HoraInicio", disponibilidad.HoraInicio),
                new SqlParameter("@HoraFin", disponibilidad.HoraFin),
                new SqlParameter("@EstadoActivo", disponibilidad.EstadoActivo)
            };
            if (incluirId) parametros.Insert(0, new SqlParameter("@IdDisponibilidadProfesor", disponibilidad.IdDisponibilidadProfesor));
            return parametros;
        }

        private static DisponibilidadProfesor_83KI Mapear(DataRow row)
        {
            return DisponibilidadProfesor_83KI.ReconstruirDesdePersistencia(Convert.ToInt32(row["IdDisponibilidadProfesor"]), Convert.ToInt32(row["IdProfesor"]), (DayOfWeek)Enum.Parse(typeof(DayOfWeek), row["DiaSemana"].ToString()), (TimeSpan)row["HoraInicio"], (TimeSpan)row["HoraFin"], Convert.ToBoolean(row["EstadoActivo"]), row["DVH"].ToString());
        }

        private void RecomputarIntegridad(int id, SqlConnection conn, SqlTransaction tran)
        {
            var valores = IntegridadDAL_83KI.LeerFilaTransaccional("DisponibilidadProfesor", "IdDisponibilidadProfesor = @id", new List<SqlParameter> { new SqlParameter("@id", id) }, conn, tran);
            if (valores.Count > 0) _integridadDAL.RecomputarIntegridadFila("DisponibilidadProfesor", valores, conn, tran);
        }
    }
}

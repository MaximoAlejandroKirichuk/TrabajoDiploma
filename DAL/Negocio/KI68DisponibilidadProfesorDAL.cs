using BE.Entidades;
using BE.DTOs.Disponibilidades;
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

        public int Insertar(DisponibilidadProfesor_83KI disponibilidad, string usuario = null)
        {
            int id = 0;
            EjecutarConUsuario(usuario, (conn, tran) =>
            {
                PrepararTransaccion(conn, tran);
                var ds = AccesoDAL_83KI.LeerStoredProcedureTransaccional(conn, tran, "sp_CUN01_InsertarDisponibilidadProfesor", CrearParametros(disponibilidad));
                id = Convert.ToInt32(ds.Tables[0].Rows[0]["IdDisponibilidadProfesor"]);
                RecomputarIntegridad(id, conn, tran);
            });
            return id;
        }

        public void Actualizar(DisponibilidadProfesor_83KI disponibilidad, string usuario = null)
        {
            EjecutarConUsuario(usuario, (conn, tran) =>
            {
                PrepararTransaccion(conn, tran);
                AccesoDAL_83KI.EscribirStoredProcedureTransaccional(conn, tran, "sp_CUN01_ActualizarDisponibilidadProfesor", CrearParametros(disponibilidad, true));
                RecomputarIntegridad(disponibilidad.IdDisponibilidadProfesor, conn, tran);
            });
        }

        public IEnumerable<BE.Entidades.DisponibilidadProfesorHistorial_83KI> ListarHistorial(FiltroDisponibilidadProfesorHistorial_83KI filtro)
        {
            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@IdProfesor", filtro.IdProfesor),
                new SqlParameter("@IdDisponibilidadProfesor", (object)filtro.IdDisponibilidadProfesor ?? DBNull.Value),
                new SqlParameter("@FechaDesde", (object)filtro.FechaDesde ?? DBNull.Value),
                new SqlParameter("@FechaHasta", (object)filtro.FechaHasta ?? DBNull.Value)
            };
            var ds = _accesoDAL.LeerStoredProcedure("sp_DisponibilidadProfesor_ListarHistorial", parametros);
            var resultado = new List<BE.Entidades.DisponibilidadProfesorHistorial_83KI>();
            if (ds == null || ds.Tables.Count == 0) return resultado;
            foreach (DataRow row in ds.Tables[0].Rows)
                    resultado.Add(new BE.Entidades.DisponibilidadProfesorHistorial_83KI(Convert.ToInt64(row["IdVersion"]), Convert.ToInt32(row["IdDisponibilidadProfesor"]), Convert.ToInt32(row["IdProfesor"]), (DayOfWeek)Enum.Parse(typeof(DayOfWeek), row["DiaSemana"].ToString()), (TimeSpan)row["HoraInicio"], (TimeSpan)row["HoraFin"], Convert.ToBoolean(row["EstadoActivo"]), row["DVH"].ToString(), Convert.ToBoolean(row["Act"]), Convert.ToDateTime(row["FechaRegistro"]), row["TipoOperacion"].ToString(), row.IsNull("Usuario") ? null : row["Usuario"].ToString()));
            return resultado;
        }

        public void ActivarVersion(long idVersion, string usuario = null)
        {
            EjecutarConUsuario(usuario, (conn, tran) =>
            {
                PrepararTransaccion(conn, tran);
                int idDisponibilidad;
                using (var command = new SqlCommand("SELECT IdDisponibilidadProfesor FROM dbo.DisponibilidadProfesor_C WITH (UPDLOCK, HOLDLOCK) WHERE IdVersion = @IdVersion", conn, tran))
                {
                    command.Parameters.AddWithValue("@IdVersion", idVersion);
                    object id = command.ExecuteScalar();
                    if (id == null || id == DBNull.Value) throw new InvalidOperationException("Errores.DisponibilidadProfesorVersionNoEncontrada");
                    idDisponibilidad = Convert.ToInt32(id);
                }
                AccesoDAL_83KI.EscribirStoredProcedureTransaccional(conn, tran, "sp_DisponibilidadProfesor_ActivarVersion", new List<SqlParameter> { new SqlParameter("@IdVersion", idVersion) });
                RecomputarIntegridad(idDisponibilidad, conn, tran);
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
            if (valores.Count > 0)
            {
                _integridadDAL.RecomputarIntegridadFila("DisponibilidadProfesor", valores, conn, tran);
                using (var cmd = new SqlCommand("UPDATE h SET DVH = d.DVH FROM dbo.DisponibilidadProfesor_C h INNER JOIN dbo.DisponibilidadProfesor d ON d.IdDisponibilidadProfesor = h.IdDisponibilidadProfesor WHERE h.IdDisponibilidadProfesor = @id AND h.Act = 1", conn, tran))
                {
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void EjecutarConUsuario(string usuario, Action<SqlConnection, SqlTransaction> operacion)
        {
            if (usuario != null && usuario.Length > 100) throw new ArgumentOutOfRangeException(nameof(usuario));
            _accesoDAL.EjecutarTransaccion((conn, tran) =>
            {
                EstablecerUsuario(conn, tran, usuario);
                try { operacion(conn, tran); }
                catch
                {
                    try { EstablecerUsuario(conn, tran, null); }
                    catch { SqlConnection.ClearPool(conn); }
                    throw;
                }
                EstablecerUsuario(conn, tran, null);
            });
        }

        private static void EstablecerUsuario(SqlConnection conn, SqlTransaction tran, string usuario)
        {
            using (var cmd = new SqlCommand("EXEC sys.sp_set_session_context @key=N'DisponibilidadProfesorUsuario', @value=@usuario;", conn, tran))
            {
                cmd.Parameters.Add("@usuario", SqlDbType.NVarChar, 100).Value = (object)usuario ?? DBNull.Value;
                cmd.ExecuteNonQuery();
            }
        }

        private static void PrepararTransaccion(SqlConnection conn, SqlTransaction tran)
        {
            using (var cmd = new SqlCommand("SET ANSI_NULLS ON; SET ANSI_PADDING ON; SET ANSI_WARNINGS ON; SET ARITHABORT ON; SET CONCAT_NULL_YIELDS_NULL ON; SET QUOTED_IDENTIFIER ON; SET NUMERIC_ROUNDABORT OFF;", conn, tran))
                cmd.ExecuteNonQuery();
        }
    }
}

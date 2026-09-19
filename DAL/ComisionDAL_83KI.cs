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
    public class ComisionDAL_83KI : IComisionDAL_83KI
    {
        private readonly AccesoDAL_83KI _accesoDAL = new AccesoDAL_83KI();
        private readonly IntegridadDAL_83KI _integridadDAL = new IntegridadDAL_83KI(new Encriptador_83KI());

        public IEnumerable<Curso_83KI> ObtenerCursosActivosParaPreapertura()
        {
            return MapearCursos(_accesoDAL.LeerStoredProcedure("sp_CUN01_ListarCursosActivos"));
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
                new SqlParameter("@FechaLimitePago", comision.FechaLimitePago)
            };
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
            return Comision_83KI.ReconstruirDesdePersistencia(Convert.ToInt32(row["IdComision"]), row["Codigo"].ToString(), Convert.ToInt32(row["IdCurso"]), Convert.ToInt32(row["IdProfesor"]), (DayOfWeek)Enum.Parse(typeof(DayOfWeek), row["DiaSemana"].ToString()), (TimeSpan)row["HoraInicio"], (TimeSpan)row["HoraFin"], Convert.ToInt32(row["CupoMinimo"]), Convert.ToInt32(row["CupoMaximo"]), Convert.ToDateTime(row["FechaLimitePago"]), row["Estado"].ToString(), row["DVH"].ToString());
        }

        private void RecomputarIntegridadComision(int idComision, SqlConnection conn, SqlTransaction tran)
        {
            var valores = IntegridadDAL_83KI.LeerFilaTransaccional("Comision", "IdComision = @id", new List<SqlParameter> { new SqlParameter("@id", idComision) }, conn, tran);
            if (valores.Count > 0) _integridadDAL.RecomputarIntegridadFila("Comision", valores, conn, tran);
        }
    }
}

using BE.Entidades;
using DAL.DAL;
using Service.DTOs;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    public class AlumnoDAL_83KI : IAlumnoDAL_83KI
    {
        private readonly AccesoDAL_83KI _accesoDAL = new AccesoDAL_83KI();

        public Alumno_83KI ObtenerPorId(int idAlumno)
        {
            var ds = _accesoDAL.Leer("SELECT IdAlumno,DNI,Nombre,Apellido,Email,Telefono,FechaAlta,Estado,DVH FROM dbo.Alumno WHERE IdAlumno=@id", new List<SqlParameter> { new SqlParameter("@id", idAlumno) });
            if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0) return null;
            return MapearAlumno(ds.Tables[0].Rows[0]);
        }

        public Alumno_83KI ObtenerPorDni(string dni)
        {
            var ds = _accesoDAL.Leer("SELECT IdAlumno,DNI,Nombre,Apellido,Email,Telefono,FechaAlta,Estado,DVH FROM dbo.Alumno WHERE DNI=@dni", new List<SqlParameter> { new SqlParameter("@dni", Normalizar(dni)) });
            if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0) return null;
            return MapearAlumno(ds.Tables[0].Rows[0]);
        }

        public IEnumerable<BusquedaInscripcionPersonaResultado_83KI> BuscarParaInscripcion(string texto)
        {
            var ds = _accesoDAL.LeerStoredProcedure("sp_CUN03_BuscarAlumnoLead", new List<SqlParameter> { new SqlParameter("@Texto", Normalizar(texto)) });
            return MapearBusqueda(ds);
        }

        internal static Alumno_83KI MapearAlumno(DataRow row)
        {
            return Alumno_83KI.ReconstruirDesdePersistencia(Convert.ToInt32(row["IdAlumno"]), row["DNI"].ToString(), row["Nombre"].ToString(), row["Apellido"].ToString(), row["Email"].ToString(), row["Telefono"].ToString(), Convert.ToDateTime(row["FechaAlta"]), row["Estado"].ToString(), row["DVH"].ToString());
        }

        private static IEnumerable<BusquedaInscripcionPersonaResultado_83KI> MapearBusqueda(DataSet ds)
        {
            var resultados = new List<BusquedaInscripcionPersonaResultado_83KI>();
            if (ds == null || ds.Tables.Count == 0) return resultados;
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                string tipo = row["Tipo"].ToString();
                if (string.Equals(tipo, "Alumno", StringComparison.OrdinalIgnoreCase))
                {
                    resultados.Add(new BusquedaInscripcionPersonaResultado_83KI
                    {
                        Tipo = "Alumno",
                        Alumno = Alumno_83KI.ReconstruirDesdePersistencia(Convert.ToInt32(row["IdPersona"]), row["DNI"].ToString(), row["Nombre"].ToString(), row["Apellido"].ToString(), row["Email"].ToString(), row["Telefono"].ToString(), Convert.ToDateTime(row["FechaAlta"]), row["Estado"].ToString(), row["DVH"].ToString())
                    });
                }
                else
                {
                    resultados.Add(new BusquedaInscripcionPersonaResultado_83KI
                    {
                        Tipo = "Lead",
                        Lead = Lead_83KI.ReconstruirDesdePersistencia(Convert.ToInt32(row["IdPersona"]), row["DNI"].ToString(), row["Nombre"].ToString(), row["Apellido"].ToString(), row["Email"].ToString(), row["Telefono"].ToString(), Convert.ToDateTime(row["FechaAlta"]), row["Estado"].ToString(), row["DVH"].ToString()),
                        IdComisionSugerida = row.Table.Columns.Contains("IdComisionSugerida") && row["IdComisionSugerida"] != DBNull.Value ? (int?)Convert.ToInt32(row["IdComisionSugerida"]) : null
                    });
                }
            }
            return resultados;
        }

        private static string Normalizar(string value) { return (value ?? string.Empty).Trim(); }
    }
}

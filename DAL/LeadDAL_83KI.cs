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
    public class LeadDAL_83KI : ILeadDAL_83KI
    {
        private readonly AccesoDAL_83KI _accesoDAL = new AccesoDAL_83KI();

        public BusquedaLeadResultado_83KI BuscarPorIdentidad(string dni, string email, string telefono)
        {
            var ds = _accesoDAL.LeerStoredProcedure("sp_CUN02_BuscarLead", new List<SqlParameter>
            {
                new SqlParameter("@DNI", Normalizar(dni)),
                new SqlParameter("@Email", Normalizar(email).ToLowerInvariant()),
                new SqlParameter("@Telefono", NormalizarTelefono(telefono))
            });
            return MapearBusqueda(ds);
        }

        public Lead_83KI ObtenerPorId(int idLead)
        {
            var ds = _accesoDAL.Leer("SELECT IdLead,DNI,Nombre,Apellido,Email,Telefono,FechaAlta,Estado,DVH FROM dbo.Lead WHERE IdLead=@id", new List<SqlParameter> { new SqlParameter("@id", idLead) });
            if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0) return null;
            return MapearLead(ds.Tables[0].Rows[0]);
        }

        internal static BusquedaLeadResultado_83KI MapearBusqueda(DataSet ds)
        {
            var resultado = new BusquedaLeadResultado_83KI();
            if (ds == null || ds.Tables.Count == 0) return resultado;
            foreach (DataRow row in ds.Tables[0].Rows) resultado.Coincidencias.Add(MapearLead(row));
            if (resultado.Coincidencias.Count == 1) resultado.Lead = resultado.Coincidencias[0];
            return resultado;
        }

        internal static Lead_83KI MapearLead(DataRow row)
        {
            return Lead_83KI.ReconstruirDesdePersistencia(Convert.ToInt32(row["IdLead"]), row["DNI"].ToString(), row["Nombre"].ToString(), row["Apellido"].ToString(), row["Email"].ToString(), row["Telefono"].ToString(), Convert.ToDateTime(row["FechaAlta"]), row["Estado"].ToString(), row["DVH"].ToString());
        }

        private static string Normalizar(string value) { return (value ?? string.Empty).Trim(); }
        private static string NormalizarTelefono(string value) { return Normalizar(value).Replace(" ", string.Empty).Replace("-", string.Empty); }
    }
}

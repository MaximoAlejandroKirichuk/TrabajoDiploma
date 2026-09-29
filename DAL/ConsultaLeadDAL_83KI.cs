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
    public class ConsultaLeadDAL_83KI : IConsultaLeadDAL_83KI
    {
        private readonly AccesoDAL_83KI _accesoDAL = new AccesoDAL_83KI();
        private readonly IntegridadDAL_83KI _integridadDAL = new IntegridadDAL_83KI(new Encriptador_83KI());

        public ConsultaLeadResultado_83KI RegistrarConsulta(ConsultaLeadRequest_83KI request)
        {
            ConsultaLeadResultado_83KI resultado = null;
            _accesoDAL.EjecutarTransaccion((conn, tran) =>
            {
                var ds = AccesoDAL_83KI.LeerStoredProcedureTransaccional(conn, tran, "sp_CUN02_RegistrarConsultaLead", CrearParametros(request));
                if (ds == null || ds.Tables.Count < 2 || ds.Tables[0].Rows.Count == 0 || ds.Tables[1].Rows.Count == 0)
                    throw new InvalidOperationException("Errores.ConsultaLeadNoRegistrada");

                var lead = LeadDAL_83KI.MapearLead(ds.Tables[0].Rows[0]);
                var consulta = MapearConsulta(ds.Tables[1].Rows[0]);
                RecomputarIntegridad("Lead", "IdLead = @id", "@id", lead.IdLead, conn, tran);
                RecomputarIntegridad("ConsultaLead", "IdConsultaLead = @id", "@id", consulta.IdConsultaLead, conn, tran);
                resultado = new ConsultaLeadResultado_83KI { Lead = lead, Consulta = consulta, LeadCreado = Convert.ToBoolean(ds.Tables[0].Rows[0]["LeadCreado"]) };
            });
            return resultado;
        }

        private static List<SqlParameter> CrearParametros(ConsultaLeadRequest_83KI request)
        {
            return new List<SqlParameter>
            {
                new SqlParameter("@DNI", Normalizar(request.DNI)),
                new SqlParameter("@Nombre", Normalizar(request.Nombre)),
                new SqlParameter("@Apellido", Normalizar(request.Apellido)),
                new SqlParameter("@Email", Normalizar(request.Email).ToLowerInvariant()),
                new SqlParameter("@Telefono", NormalizarTelefono(request.Telefono)),
                new SqlParameter("@IdComision", request.IdComision),
                new SqlParameter("@MedioContacto", Normalizar(request.MedioContacto)),
                new SqlParameter("@Motivo", Normalizar(request.Motivo)),
                new SqlParameter("@Observaciones", (object)Normalizar(request.Observaciones) ?? DBNull.Value)
            };
        }

        private void RecomputarIntegridad(string tabla, string where, string paramName, int id, SqlConnection conn, SqlTransaction tran)
        {
            var valores = IntegridadDAL_83KI.LeerFilaTransaccional(tabla, where, new List<SqlParameter> { new SqlParameter(paramName, id) }, conn, tran);
            if (valores.Count > 0) _integridadDAL.RecomputarIntegridadFila(tabla, valores, conn, tran);
        }

        private static ConsultaLead_83KI MapearConsulta(DataRow row)
        {
            return ConsultaLead_83KI.ReconstruirDesdePersistencia(Convert.ToInt32(row["IdConsultaLead"]), row["Codigo"].ToString(), Convert.ToInt32(row["IdLead"]), Convert.ToInt32(row["IdComision"]), Convert.ToDateTime(row["FechaConsulta"]), row["MedioContacto"].ToString(), row["Motivo"].ToString(), row["Observaciones"].ToString(), row["Estado"].ToString(), row["DVH"].ToString());
        }

        private static string Normalizar(string value) { return (value ?? string.Empty).Trim(); }
        private static string NormalizarTelefono(string value) { return Normalizar(value).Replace(" ", string.Empty).Replace("-", string.Empty); }
    }
}

using DAL.DAL;
using Service.DTOs;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    public class ReporteOcupacionComisionDAL_83KI : IReporteOcupacionComisionDAL_83KI
    {
        private readonly AccesoDAL_83KI _accesoDAL = new AccesoDAL_83KI();

        public IEnumerable<ReporteOcupacionComision_83KI> Generar(string estado)
        {
            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@Estado", string.IsNullOrWhiteSpace(estado) ? (object)DBNull.Value : estado)
            };
            return Mapear(_accesoDAL.LeerStoredProcedure("sp_Reporte_OcupacionComisiones", parametros));
        }

        private static IEnumerable<ReporteOcupacionComision_83KI> Mapear(DataSet ds)
        {
            var lista = new List<ReporteOcupacionComision_83KI>();
            if (ds == null || ds.Tables.Count == 0) return lista;

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                lista.Add(new ReporteOcupacionComision_83KI
                {
                    IdComision = Convert.ToInt32(row["IdComision"]),
                    Codigo = row["Codigo"].ToString(),
                    Curso = row["Curso"].ToString(),
                    Profesor = row["Profesor"].ToString(),
                    FechaLimitePago = Convert.ToDateTime(row["FechaLimitePago"]),
                    CupoMinimo = Convert.ToInt32(row["CupoMinimo"]),
                    CupoMaximo = Convert.ToInt32(row["CupoMaximo"]),
                    Estado = row["Estado"].ToString(),
                    Inscriptos = Convert.ToInt32(row["Inscriptos"]),
                    Pagaron = Convert.ToInt32(row["Pagaron"]),
                    CuotasReintegro = Convert.ToInt32(row["CuotasReintegro"]),
                    MontoReintegro = Convert.ToDecimal(row["MontoReintegro"]),
                    NumeroActa = row["NumeroActa"].ToString()
                });
            }
            return lista;
        }
    }
}

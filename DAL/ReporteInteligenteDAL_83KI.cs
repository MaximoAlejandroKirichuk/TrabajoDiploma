using BE.DTOs.Reportes;
using BE.Entidades;
using DAL.DAL;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace DAL
{
    public sealed class ReporteInteligenteDAL_83KI : IReporteInteligenteDAL_83KI
    {
        private readonly AccesoDAL_83KI _acceso = new AccesoDAL_83KI();
        public List<Curso_83KI> Cursos() { return new CursoDAL_83KI().ObtenerTodos().ToList(); }
        public ReporteInteligente_83KI Generar(FiltroReporteInteligente_83KI filtro)
        {
            var ds = _acceso.LeerStoredProcedure("sp_Reporte_Inteligente", new List<SqlParameter>
            {
                new SqlParameter("@Desde", (object)filtro.Desde?.Date ?? DBNull.Value),
                new SqlParameter("@Hasta", (object)filtro.Hasta?.Date ?? DBNull.Value),
                new SqlParameter("@IdCurso", (object)filtro.IdCurso ?? DBNull.Value)
            });
            if (ds.Tables.Count != 6) throw new InvalidOperationException("Inteligente.EsquemaInvalido");
            return new ReporteInteligente_83KI
            {
                FechaCenso = Convert.ToDateTime(ds.Tables[0].Rows[0]["FechaCenso"]),
                Demanda = Mapear<DemandaCurso_83KI>(ds.Tables[1]),
                OcupacionHistorica = Mapear<OfertaCurso_83KI>(ds.Tables[2]),
                OfertaActual = Mapear<OfertaCurso_83KI>(ds.Tables[3]),
                Candidatos = Mapear<CandidatoContinuidad_83KI>(ds.Tables[4]),
                Cumplimiento = Mapear<CumplimientoCurso_83KI>(ds.Tables[5])
            };
        }
        private static List<T> Mapear<T>(DataTable tabla) where T : new()
        {
            var propiedades = typeof(T).GetProperties().Where(p => p.CanWrite && tabla.Columns.Contains(p.Name)).ToArray();
            var resultado = new List<T>();
            foreach (DataRow fila in tabla.Rows)
            {
                var item = new T();
                foreach (var propiedad in propiedades)
                    propiedad.SetValue(item, fila.IsNull(propiedad.Name) ? null : fila[propiedad.Name]);
                resultado.Add(item);
            }
            return resultado;
        }
    }
}

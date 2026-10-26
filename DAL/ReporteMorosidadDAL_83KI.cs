using BE.Entidades;
using DAL.DAL;
using BE.DTOs.Pagos;
using Service.DTOs;
using BE.DTOs.Comisiones;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    // RF2 - Informe de morosidad por alumno y comisión.
    // Solo lee: devuelve las inscripciones con cuotas con saldo. Qué es "vencida", "moroso"
    // o "vigente" lo decide KI68ReporteMorosidadBLL.
    public class ReporteMorosidadDAL_83KI : IReporteMorosidadDAL_83KI
    {
        private readonly AccesoDAL_83KI _accesoDAL = new AccesoDAL_83KI();

        private const string ConsultaInscripcionesConSaldo = @"
SELECT si.IdSolicitudInscripcion, si.Codigo AS CodigoInscripcion, si.Estado AS EstadoInscripcion,
       a.DNI, a.Apellido, a.Nombre, a.Email, a.Telefono,
       c.IdComision, c.Codigo AS CodigoComision, c.Estado AS EstadoComision,
       cur.IdCurso, cur.Nombre AS Curso,
       COALESCE(si.NombrePlanSnapshot, pp.Nombre) AS PlanDePago,
       cu.IdCuota, cu.NumeroCuota, cu.TipoConcepto, cu.Periodo, cu.MontoOriginal,
       cu.BalanceAdeudado, cu.FechaVencimiento, cu.Estado AS EstadoCuota, cu.DVH AS DVHCuota
FROM dbo.SolicitudInscripcion si
INNER JOIN dbo.Alumno a ON a.IdAlumno = si.IdAlumno
INNER JOIN dbo.Comision c ON c.IdComision = si.IdComision
INNER JOIN dbo.Curso cur ON cur.IdCurso = c.IdCurso
INNER JOIN dbo.PlanesDePago pp ON pp.IdPlanDePago = si.IdPlanDePago
INNER JOIN dbo.Cuota cu ON cu.IdSolicitudInscripcion = si.IdSolicitudInscripcion
WHERE cu.BalanceAdeudado > 0
  AND (@IdCurso IS NULL OR c.IdCurso = @IdCurso)
  AND (@IdComision IS NULL OR c.IdComision = @IdComision)
  AND (@DNI IS NULL OR a.DNI LIKE @DNI + '%')
ORDER BY si.IdSolicitudInscripcion, cu.NumeroCuota";

        private const string ConsultaComisiones = @"
SELECT c.IdComision, c.Codigo, c.Estado, c.IdCurso, cur.Nombre AS Curso
FROM dbo.Comision c
INNER JOIN dbo.Curso cur ON cur.IdCurso = c.IdCurso
ORDER BY cur.Nombre, c.Codigo";

        public List<ReporteMorosidadFila_83KI> ObtenerInscripcionesConSaldo(FiltroReporteMorosidad_83KI filtro)
        {
            if (filtro == null) filtro = new FiltroReporteMorosidad_83KI();

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@IdCurso", SqlDbType.Int) { Value = (object)filtro.IdCurso ?? DBNull.Value },
                new SqlParameter("@IdComision", SqlDbType.Int) { Value = (object)filtro.IdComision ?? DBNull.Value },
                new SqlParameter("@DNI", SqlDbType.VarChar, 20)
                {
                    Value = string.IsNullOrWhiteSpace(filtro.DNI) ? (object)DBNull.Value : filtro.DNI.Trim()
                }
            };

            DataSet ds = _accesoDAL.Leer(ConsultaInscripcionesConSaldo, parametros);
            return MapearFilas(ds);
        }

        public List<ComisionListado_83KI> ListarComisionesParaFiltro()
        {
            var lista = new List<ComisionListado_83KI>();
            DataSet ds = _accesoDAL.Leer(ConsultaComisiones);
            if (ds == null || ds.Tables.Count == 0) return lista;

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                lista.Add(new ComisionListado_83KI
                {
                    IdComision = Convert.ToInt32(row["IdComision"]),
                    Codigo = Texto(row, "Codigo"),
                    Estado = Texto(row, "Estado"),
                    IdCurso = Convert.ToInt32(row["IdCurso"]),
                    Curso = Texto(row, "Curso")
                });
            }
            return lista;
        }

        // La consulta viene ordenada por inscripción: cada cambio de IdSolicitudInscripcion abre una fila nueva.
        private static List<ReporteMorosidadFila_83KI> MapearFilas(DataSet ds)
        {
            var filas = new List<ReporteMorosidadFila_83KI>();
            if (ds == null || ds.Tables.Count == 0) return filas;

            ReporteMorosidadFila_83KI actual = null;
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                int idSolicitud = Convert.ToInt32(row["IdSolicitudInscripcion"]);
                if (actual == null || actual.IdSolicitudInscripcion != idSolicitud)
                {
                    actual = new ReporteMorosidadFila_83KI
                    {
                        IdSolicitudInscripcion = idSolicitud,
                        CodigoInscripcion = Texto(row, "CodigoInscripcion"),
                        EstadoInscripcion = Texto(row, "EstadoInscripcion"),
                        DNI = Texto(row, "DNI"),
                        Apellido = Texto(row, "Apellido"),
                        Nombre = Texto(row, "Nombre"),
                        Email = Texto(row, "Email"),
                        Telefono = Texto(row, "Telefono"),
                        IdComision = Convert.ToInt32(row["IdComision"]),
                        CodigoComision = Texto(row, "CodigoComision"),
                        EstadoComision = Texto(row, "EstadoComision"),
                        IdCurso = Convert.ToInt32(row["IdCurso"]),
                        Curso = Texto(row, "Curso"),
                        PlanDePago = Texto(row, "PlanDePago")
                    };
                    filas.Add(actual);
                }

                actual.CuotasConSaldo.Add(Cuota_83KI.ReconstruirDesdePersistencia(
                    Convert.ToInt32(row["IdCuota"]),
                    idSolicitud,
                    Convert.ToInt32(row["NumeroCuota"]),
                    Convert.ToDecimal(row["MontoOriginal"]),
                    Convert.ToDecimal(row["BalanceAdeudado"]),
                    Convert.ToDateTime(row["FechaVencimiento"]),
                    Texto(row, "EstadoCuota"),
                    Texto(row, "DVHCuota"),
                    row["TipoConcepto"] == DBNull.Value ? null : row["TipoConcepto"].ToString(),
                    row["Periodo"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["Periodo"])));
            }
            return filas;
        }

        private static string Texto(DataRow row, string columna)
        {
            return row[columna] == DBNull.Value ? string.Empty : row[columna].ToString().Trim();
        }
    }
}

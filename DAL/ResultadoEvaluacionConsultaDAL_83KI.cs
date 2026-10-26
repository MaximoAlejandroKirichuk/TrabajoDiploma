using BE.Entidades;
using DAL.DAL;
using Service.DTOs;
using BE.DTOs.Comisiones;
using BE.DTOs.Evaluaciones;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    public partial class ResultadoEvaluacionDAL_83KI
    {
        private const string ConsultaEvaluaciones = @"SELECT r.*,c.FechaInicio,c.FechaFin,CONVERT(date,GETDATE()) Hoy,
            a.DNI,a.Nombre,a.Apellido,
            (SELECT COUNT(*) FROM dbo.ResultadoEvaluacion n WITH(HOLDLOCK) WHERE n.IdInscripcion=r.IdInscripcion) Cantidad,
            CONVERT(bit,CASE WHEN EXISTS(SELECT 1 FROM dbo.CertificadoFinalizacion cf WITH(HOLDLOCK)
                WHERE cf.IdInscripcion=r.IdInscripcion) THEN 1 ELSE 0 END) Certificado
            FROM dbo.ResultadoEvaluacion r WITH(UPDLOCK,HOLDLOCK)
            JOIN dbo.SolicitudInscripcion si WITH(HOLDLOCK) ON si.IdSolicitudInscripcion=r.IdInscripcion AND si.IdComision=r.IdComision
            JOIN dbo.Alumno a WITH(HOLDLOCK) ON a.IdAlumno=si.IdAlumno
            JOIN dbo.Comision c WITH(HOLDLOCK) ON c.IdComision=r.IdComision ";

        public List<ComisionListado_83KI> ListarComisionesConResultados()
        {
            var result = new List<ComisionListado_83KI>();
            _accesoDAL.EjecutarTransaccion((conn, tran) =>
            {
                AutorizarConsulta(conn, tran, false);
                foreach (DataRow r in AccesoDAL_83KI.LeerTransaccional(conn, tran, @"SELECT c.IdComision,c.Codigo FROM dbo.Comision c
                    WHERE EXISTS(SELECT 1 FROM dbo.ResultadoEvaluacion r WHERE r.IdComision=c.IdComision) ORDER BY c.Codigo,c.IdComision").Tables[0].Rows)
                    result.Add(new ComisionListado_83KI { IdComision = Convert.ToInt32(r[0]), Codigo = r[1].ToString() });
            });
            return result;
        }

        public List<EvaluacionConsulta_83KI> BuscarEvaluaciones(int idComision)
        {
            if (idComision <= 0) throw new InvalidOperationException("Evaluaciones.Seleccionar");
            var result = new List<EvaluacionConsulta_83KI>();
            _accesoDAL.EjecutarTransaccion((conn, tran) =>
            {
                BloquearConsulta(conn, tran);
                AutorizarConsulta(conn, tran, false);
                foreach (DataRow r in AccesoDAL_83KI.LeerTransaccional(conn, tran, ConsultaEvaluaciones +
                    " WHERE r.IdComision=@id ORDER BY a.Apellido,a.Nombre,a.DNI,r.FechaEvaluacion,r.IdResultadoEvaluacion", IdConsulta(idComision)).Tables[0].Rows)
                    result.Add(MapearEvaluacion(r));
            });
            return result;
        }

        public EvaluacionConsulta_83KI ObtenerEvaluacion(int idResultado)
        {
            EvaluacionConsulta_83KI result = null;
            _accesoDAL.EjecutarTransaccion((conn, tran) =>
            {
                BloquearConsulta(conn, tran);
                AutorizarConsulta(conn, tran, false);
                result = LeerEvaluacion(idResultado, conn, tran);
            });
            return result;
        }

        public bool ModificarEvaluacion(EvaluacionConsulta_83KI original, ModificarEvaluacionRequest_83KI cambios)
        {
            ValidarCambios(cambios);
            if (original == null) throw new InvalidOperationException("Evaluaciones.Seleccionar");
            bool modificado = false;
            try
            {
                _accesoDAL.EjecutarTransaccion((conn, tran) =>
                {
                    // Mismo orden que emisión: certificado -> resultados -> permisos -> filas -> integridad.
                    BloquearConsulta(conn, tran);
                    AutorizarConsulta(conn, tran, true);
                    var actual = LeerEvaluacion(original.IdResultado, conn, tran);
                    if (actual == null) throw new InvalidOperationException("Evaluaciones.NoExiste");
                    if (actual.IdInscripcion != original.IdInscripcion || actual.IdComision != original.IdComision || actual.Alumno != original.Alumno
                        || actual.DVH != original.DVH || actual.Calificacion != original.Calificacion
                        || actual.FechaEvaluacion != original.FechaEvaluacion || actual.Observaciones != original.Observaciones
                        || actual.CondicionFinal != original.CondicionFinal)
                        throw new InvalidOperationException("Evaluaciones.Conflicto");
                    if (actual.TieneCertificado) throw new InvalidOperationException("Evaluaciones.Certificado");
                    if (actual.CantidadNotas > 2) throw new InvalidOperationException("Evaluaciones.Legado");
                    if (cambios.FechaEvaluacion.Date < actual.Inicio || cambios.FechaEvaluacion.Date > actual.Fin)
                        throw new InvalidOperationException("Cun08.FechaInvalida");
                    if (cambios.FechaEvaluacion.Date > actual.Hoy) throw new InvalidOperationException("Cun08.FechaFutura");
                    string obs = cambios.Observaciones ?? "";
                    if (actual.Calificacion == cambios.Calificacion && actual.FechaEvaluacion == cambios.FechaEvaluacion.Date
                        && actual.Observaciones == obs) return;
                    var p = IdConsulta(actual.IdResultado);
                    p.Add(new SqlParameter("@inscripcion", actual.IdInscripcion));
                    p.Add(new SqlParameter("@fecha", SqlDbType.Date) { Value = cambios.FechaEvaluacion.Date });
                    if (Convert.ToInt32(AccesoDAL_83KI.LeerEscalarTransaccional(conn, tran, @"SELECT COUNT(*) FROM dbo.ResultadoEvaluacion WITH(UPDLOCK,HOLDLOCK)
                        WHERE IdInscripcion=@inscripcion AND FechaEvaluacion=@fecha AND IdResultadoEvaluacion<>@id", p)) > 0)
                        throw new InvalidOperationException("Evaluaciones.Duplicada");
                    p = IdConsulta(actual.IdResultado);
                    p.Add(new SqlParameter("@fecha", SqlDbType.Date) { Value = cambios.FechaEvaluacion.Date });
                    p.Add(new SqlParameter("@nota", SqlDbType.Decimal) { Precision = 4, Scale = 2, Value = cambios.Calificacion });
                    p.Add(new SqlParameter("@obs", SqlDbType.NVarChar, 500) { Value = obs });
                    p.Add(new SqlParameter("@condicion", new ResultadoEvaluacion_83KI { Calificacion = cambios.Calificacion }.DeterminarCondicionFinal()));
                    if (AccesoDAL_83KI.EscribirTransaccional(conn, tran, @"UPDATE dbo.ResultadoEvaluacion SET Calificacion=@nota,
                        FechaEvaluacion=@fecha,Observaciones=@obs,CondicionFinal=@condicion WHERE IdResultadoEvaluacion=@id", p) != 1)
                        throw new InvalidOperationException("Evaluaciones.Conflicto");
                    RecomputarIntegridadResultados(new List<int> { actual.IdResultado }, conn, tran);
                    modificado = true;
                });
            }
            catch (SqlException ex) when (ex.Number == 2601 || ex.Number == 2627)
            { throw new InvalidOperationException("Evaluaciones.Duplicada", ex); }
            return modificado;
        }

        private static void ValidarCambios(ModificarEvaluacionRequest_83KI r)
        {
            if (r == null) throw new InvalidOperationException("Evaluaciones.Seleccionar");
            if (r.Calificacion < 1 || r.Calificacion > 10 || decimal.Round(r.Calificacion, 2) != r.Calificacion)
                throw new InvalidOperationException("Cun08.CalificacionInvalida");
            if ((r.Observaciones ?? "").Length > 500) throw new InvalidOperationException("Cun08.ObservacionesLargas");
        }

        private static void BloquearConsulta(SqlConnection conn, SqlTransaction tran)
        {
            int code = Convert.ToInt32(AccesoDAL_83KI.LeerEscalarTransaccional(conn, tran, @"DECLARE @r int;
                EXEC @r=sys.sp_getapplock @Resource=N'CUN09:Certificado',@LockMode='Exclusive',@LockOwner='Transaction',@LockTimeout=15000;
                IF @r>=0 EXEC @r=sys.sp_getapplock @Resource=N'CUN08:ResultadoEvaluacion',@LockMode='Exclusive',@LockOwner='Transaction',@LockTimeout=15000;
                SELECT @r;"));
            if (code < 0) throw new InvalidOperationException("Cun08.Concurrente");
        }

        private void AutorizarConsulta(SqlConnection conn, SqlTransaction tran, bool modificar)
        {
            int dni = _dniActual();
            if (dni <= 0) throw new InvalidOperationException("Errores.SinPermisos");
            AccesoDAL_83KI.EscribirTransaccional(conn, tran, @"DECLARE @rol int;
                SELECT @rol=CodigoRol FROM dbo.Usuarios WITH(HOLDLOCK)
                    WHERE CONVERT(nvarchar(50),DNI) COLLATE Latin1_General_100_BIN2=@dni
                    AND DATALENGTH(CONVERT(nvarchar(50),DNI))=DATALENGTH(@dni) AND Activo=1 AND Bloqueado=0;
                SELECT CodigoRol,CodigoPatente INTO #erp FROM dbo.RolPatente WITH(HOLDLOCK);
                SELECT CodigoFamilia,CodigoPatente INTO #efp FROM dbo.FamiliaPatente WITH(HOLDLOCK);
                SELECT CodigoFamiliaPadre,CodigoFamiliaHija INTO #eff FROM dbo.FamiliaFamilia WITH(HOLDLOCK);
                CREATE TABLE #ef(id int PRIMARY KEY);
                INSERT #ef SELECT DISTINCT CodigoFamilia FROM dbo.RolFamilia WITH(HOLDLOCK) WHERE CodigoRol=@rol;
                WHILE 1=1 BEGIN
                    INSERT #ef SELECT DISTINCT x.CodigoFamiliaHija FROM #eff x JOIN #ef p ON p.id=x.CodigoFamiliaPadre
                        WHERE NOT EXISTS(SELECT 1 FROM #ef e WHERE e.id=x.CodigoFamiliaHija);
                    IF @@ROWCOUNT=0 BREAK;
                END;
                IF @rol IS NULL OR NOT EXISTS(SELECT 1 FROM dbo.Patentes WITH(HOLDLOCK) WHERE CodigoPatente=73)
                    OR (NOT EXISTS(SELECT 1 FROM #erp WHERE CodigoRol=@rol AND CodigoPatente=73)
                    AND NOT EXISTS(SELECT 1 FROM #efp p JOIN #ef f ON f.id=p.CodigoFamilia WHERE p.CodigoPatente=73))
                    THROW 52930,N'Errores.SinPermisos',1;
                IF @modificar=1 AND (NOT EXISTS(SELECT 1 FROM dbo.Patentes WITH(HOLDLOCK) WHERE CodigoPatente=74)
                    OR (NOT EXISTS(SELECT 1 FROM #erp WHERE CodigoRol=@rol AND CodigoPatente=74)
                    AND NOT EXISTS(SELECT 1 FROM #efp p JOIN #ef f ON f.id=p.CodigoFamilia WHERE p.CodigoPatente=74)))
                    THROW 52930,N'Errores.SinPermisos',1;
                DROP TABLE #erp; DROP TABLE #efp; DROP TABLE #eff; DROP TABLE #ef;",
                new List<SqlParameter> { new SqlParameter("@dni", SqlDbType.NVarChar, 50) { Value = dni.ToString(System.Globalization.CultureInfo.InvariantCulture) }, new SqlParameter("@modificar", modificar) });
        }

        private static EvaluacionConsulta_83KI LeerEvaluacion(int id, SqlConnection conn, SqlTransaction tran)
        {
            var rows = AccesoDAL_83KI.LeerTransaccional(conn, tran, ConsultaEvaluaciones + " WHERE r.IdResultadoEvaluacion=@id", IdConsulta(id)).Tables[0].Rows;
            return rows.Count == 1 ? MapearEvaluacion(rows[0]) : null;
        }
        private static List<SqlParameter> IdConsulta(int id) { return new List<SqlParameter> { new SqlParameter("@id", id) }; }
        private static EvaluacionConsulta_83KI MapearEvaluacion(DataRow r)
        {
            return new EvaluacionConsulta_83KI(Convert.ToInt32(r["IdResultadoEvaluacion"]), Convert.ToInt32(r["IdInscripcion"]),
                Convert.ToInt32(r["IdComision"]), r["DNI"] + " - " + r["Apellido"] + ", " + r["Nombre"], Convert.ToDecimal(r["Calificacion"]),
                Convert.ToDateTime(r["FechaEvaluacion"]), r["Observaciones"].ToString(), r["CondicionFinal"].ToString(), r["DVH"].ToString(),
                Convert.ToDateTime(r["FechaInicio"]), Convert.ToDateTime(r["FechaFin"]), Convert.ToDateTime(r["Hoy"]),
                Convert.ToBoolean(r["Certificado"]), Convert.ToInt32(r["Cantidad"]));
        }
    }
}

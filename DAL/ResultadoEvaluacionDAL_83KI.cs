using BE.Entidades;
using DAL.DAL;
using Service.DTOs;
using BE.DTOs.Comisiones;
using BE.DTOs.Evaluaciones;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace DAL
{
    public partial class ResultadoEvaluacionDAL_83KI : IResultadoEvaluacionDAL_83KI, IEvaluacionesDAL_83KI
    {
        private readonly AccesoDAL_83KI _accesoDAL = new AccesoDAL_83KI();
        private readonly Func<int> _dniActual;
        private readonly Action<List<int>, SqlConnection, SqlTransaction> _integridad;

        public ResultadoEvaluacionDAL_83KI(Func<int> dniActual, Action<List<int>, SqlConnection, SqlTransaction> integridad)
        {
            _dniActual = dniActual ?? throw new ArgumentNullException(nameof(dniActual));
            _integridad = integridad ?? throw new ArgumentNullException(nameof(integridad));
        }

        private const string Habilitado = @"si.Estado IN (N'activa')
            AND NOT EXISTS (SELECT 1 FROM dbo.Cuota cu WITH (UPDLOCK,HOLDLOCK)
                WHERE cu.IdSolicitudInscripcion=si.IdSolicitudInscripcion AND cu.BalanceAdeudado>0
                AND cu.FechaVencimiento<CONVERT(date,GETDATE()))
            AND NOT EXISTS (SELECT 1 FROM dbo.ResultadoEvaluacion re WITH (UPDLOCK,HOLDLOCK)
                WHERE re.IdInscripcion=si.IdSolicitudInscripcion AND re.FechaEvaluacion=@fecha)
            AND (SELECT COUNT(*) FROM dbo.ResultadoEvaluacion re WITH (UPDLOCK,HOLDLOCK)
                WHERE re.IdInscripcion=si.IdSolicitudInscripcion)<2";

        public List<AlumnoEvaluacionListado_83KI> BuscarAlumnosHabilitados(int idComision, DateTime fechaEvaluacion)
        {
            var resultado = new List<AlumnoEvaluacionListado_83KI>();
            _accesoDAL.EjecutarTransaccion((conn, tran) =>
            {
                ValidarContexto(idComision, fechaEvaluacion, conn, tran);
                var ds = AccesoDAL_83KI.LeerTransaccional(conn, tran, @"SELECT si.IdSolicitudInscripcion,si.IdAlumno,a.DNI,a.Nombre,a.Apellido
                    FROM dbo.SolicitudInscripcion si WITH (UPDLOCK,HOLDLOCK)
                    JOIN dbo.Alumno a ON a.IdAlumno=si.IdAlumno WHERE si.IdComision=@comision AND " + Habilitado + " ORDER BY a.Apellido,a.Nombre,a.DNI", Parametros(idComision, fechaEvaluacion));
                foreach (DataRow row in ds.Tables[0].Rows)
                    resultado.Add(new AlumnoEvaluacionListado_83KI { IdComision = idComision, IdInscripcion = Convert.ToInt32(row["IdSolicitudInscripcion"]),
                        IdAlumno = Convert.ToInt32(row["IdAlumno"]), DNI = row["DNI"].ToString(), Nombre = row["Nombre"].ToString(), Apellido = row["Apellido"].ToString() });
            });
            return resultado;
        }

        public bool ExisteResultado(int idInscripcion, DateTime fechaEvaluacion)
        {
            return Convert.ToInt32(_accesoDAL.LeerEscalar("SELECT COUNT(*) FROM dbo.ResultadoEvaluacion WHERE IdInscripcion=@id AND FechaEvaluacion=@fecha",
                new List<SqlParameter> { new SqlParameter("@id", idInscripcion), new SqlParameter("@fecha", SqlDbType.Date) { Value = fechaEvaluacion.Date } })) > 0;
        }

        public void RegistrarResultados(ResultadoEvaluacionRequest_83KI request)
        {
            if (request == null || request.Resultados == null || request.Resultados.Count == 0) throw new InvalidOperationException("Cun08.TandaVacia");
            if (request.Resultados.Any(r => r == null || r.IdInscripcion <= 0 || r.Calificacion < 1m || r.Calificacion > 10m
                || decimal.Round(r.Calificacion, 2) != r.Calificacion || (r.Observaciones ?? "").Length > 500)
                || request.Resultados.Select(r => r.IdInscripcion).Distinct().Count() != request.Resultados.Count)
                throw new InvalidOperationException("Cun08.AlumnosInvalidos");
            _accesoDAL.EjecutarTransaccion((conn, tran) =>
            {
                // El DVV abarca toda la tabla; serializar todas las operaciones de escritura de CUN08, no solo las de una comisión.
                int lockResult = Convert.ToInt32(AccesoDAL_83KI.LeerEscalarTransaccional(conn, tran, @"DECLARE @r int;
                    EXEC @r=sys.sp_getapplock @Resource=N'CUN08:ResultadoEvaluacion',@LockMode='Exclusive',@LockOwner='Transaction',@LockTimeout=15000; SELECT @r;"));
                if (lockResult < 0) throw new InvalidOperationException("Cun08.Concurrente");
                ValidarContexto(request.IdComision, request.FechaEvaluacion, conn, tran);
                // Validar todas las filas antes de escribir cualquiera; los bloqueos se mantienen hasta confirmar o revertir la transacción.
                foreach (var r in request.Resultados.OrderBy(r => r.IdInscripcion))
                {
                    var p = Parametros(request.IdComision, request.FechaEvaluacion);
                    p.Add(new SqlParameter("@id", r.IdInscripcion));
                    int count = Convert.ToInt32(AccesoDAL_83KI.LeerEscalarTransaccional(conn, tran,
                        "SELECT COUNT(*) FROM dbo.SolicitudInscripcion si WITH (UPDLOCK,HOLDLOCK) WHERE si.IdComision=@comision AND si.IdSolicitudInscripcion=@id AND " + Habilitado, p));
                    if (count != 1) throw new InvalidOperationException("Cun08.AlumnosInvalidos");
                }
                var ids = new List<int>();
                foreach (var r in request.Resultados)
                {
                    var p = Parametros(request.IdComision, request.FechaEvaluacion);
                    p.Add(new SqlParameter("@id", r.IdInscripcion));
                    p.Add(new SqlParameter("@nota", SqlDbType.Decimal) { Precision = 4, Scale = 2, Value = r.Calificacion });
                    p.Add(new SqlParameter("@observaciones", SqlDbType.NVarChar, 500) { Value = r.Observaciones ?? string.Empty });
                    p.Add(new SqlParameter("@condicion", new ResultadoEvaluacion_83KI { Calificacion = r.Calificacion }.DeterminarCondicionFinal()));
                    ids.Add(Convert.ToInt32(AccesoDAL_83KI.LeerEscalarTransaccional(conn, tran, @"DECLARE @insertados TABLE(Id int);
                        INSERT dbo.ResultadoEvaluacion
                        (IdComision,IdInscripcion,FechaEvaluacion,Calificacion,Observaciones,CondicionFinal,DVH)
                        OUTPUT inserted.IdResultadoEvaluacion INTO @insertados
                        VALUES(@comision,@id,@fecha,@nota,@observaciones,@condicion,''); SELECT Id FROM @insertados;", p)));
                }
                RecomputarIntegridadResultados(ids, conn, tran);
            });
        }

        private void RecomputarIntegridadResultados(List<int> idsResultado, SqlConnection conn, SqlTransaction tran)
        {
            _integridad(idsResultado, conn, tran);
        }

        private void ValidarContexto(int idComision, DateTime fecha, SqlConnection conn, SqlTransaction tran)
        {
            int dni = _dniActual();
            if (dni < 1000000) throw new InvalidOperationException("Cun08.SinAutorizacion");
            var p = Parametros(idComision, fecha);
            p.Add(new SqlParameter("@dni", SqlDbType.NVarChar, 50) { Value = dni.ToString(System.Globalization.CultureInfo.InvariantCulture) });
            AccesoDAL_83KI.EscribirTransaccional(conn, tran, @"
                DECLARE @rol int,@profesor int,@inicio date,@fin date;
                SELECT @rol=CodigoRol FROM dbo.Usuarios WITH (UPDLOCK,HOLDLOCK)
                    WHERE CONVERT(nvarchar(50),DNI) COLLATE Latin1_General_100_BIN2=@dni
                    AND DATALENGTH(CONVERT(nvarchar(50),DNI))=DATALENGTH(@dni) AND Activo=1 AND Bloqueado=0;
                SELECT @profesor=IdProfesor FROM dbo.Profesor WITH (UPDLOCK,HOLDLOCK)
                    WHERE CONVERT(nvarchar(50),DNI) COLLATE Latin1_General_100_BIN2=@dni
                    AND DATALENGTH(CONVERT(nvarchar(50),DNI))=DATALENGTH(@dni) AND EstadoActivo=1;
                IF @rol IS NULL OR @profesor IS NULL THROW 52801,N'Cun08.SinAutorizacion',1;
                -- Materializar y bloquear el grafo de permisos, incluidas las asignaciones directas ausentes.
                SELECT CodigoRol,CodigoPatente INTO #rp FROM dbo.RolPatente WITH (UPDLOCK,HOLDLOCK);
                SELECT CodigoRol,CodigoFamilia INTO #rf FROM dbo.RolFamilia WITH (UPDLOCK,HOLDLOCK);
                SELECT CodigoFamilia,CodigoPatente INTO #fp FROM dbo.FamiliaPatente WITH (UPDLOCK,HOLDLOCK);
                SELECT CodigoFamiliaPadre,CodigoFamiliaHija INTO #ff FROM dbo.FamiliaFamilia WITH (UPDLOCK,HOLDLOCK);
                CREATE TABLE #familias(id int PRIMARY KEY);
                INSERT #familias SELECT DISTINCT CodigoFamilia FROM #rf WHERE CodigoRol=@rol;
                WHILE 1=1 BEGIN
                    INSERT #familias SELECT DISTINCT f.CodigoFamiliaHija FROM #ff f JOIN #familias p ON p.id=f.CodigoFamiliaPadre
                        WHERE NOT EXISTS(SELECT 1 FROM #familias x WHERE x.id=f.CodigoFamiliaHija);
                    IF @@ROWCOUNT=0 BREAK;
                END;
                IF NOT EXISTS(SELECT 1 FROM dbo.Patentes WITH (UPDLOCK,HOLDLOCK) WHERE CodigoPatente=71)
                    OR (NOT EXISTS(SELECT 1 FROM #rp WHERE CodigoRol=@rol AND CodigoPatente=71)
                    AND NOT EXISTS(SELECT 1 FROM #fp p JOIN #familias f ON f.id=p.CodigoFamilia WHERE p.CodigoPatente=71))
                    THROW 52801,N'Cun08.SinAutorizacion',1;
                DROP TABLE #rp; DROP TABLE #rf; DROP TABLE #fp; DROP TABLE #ff; DROP TABLE #familias;
                SELECT @inicio=FechaInicio,@fin=FechaFin FROM dbo.Comision WITH (UPDLOCK,HOLDLOCK)
                    WHERE IdComision=@comision AND IdProfesor=@profesor;
                IF @fin IS NULL THROW 52801,N'Cun08.SinAutorizacion',1;
                IF @fecha<@inicio OR @fecha>@fin THROW 52803,N'Cun08.FechaInvalida',1;
                IF @fecha>CONVERT(date,GETDATE()) THROW 52802,N'Cun08.FechaFutura',1;", p);
        }

        private static List<SqlParameter> Parametros(int idComision, DateTime fecha)
        {
            return new List<SqlParameter> { new SqlParameter("@comision", idComision), new SqlParameter("@fecha", SqlDbType.Date) { Value = fecha.Date } };
        }
    }
}

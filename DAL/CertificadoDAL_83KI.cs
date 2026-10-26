using BE.Entidades;
using BE.DTOs.Certificaciones;
using DAL.DAL;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    public sealed class CertificadoDAL_83KI : ICertificadoDAL_83KI
    {
        private readonly AccesoDAL_83KI _acceso = new AccesoDAL_83KI();
        private readonly Func<int> _dni;
        private readonly Action<int, SqlConnection, SqlTransaction> _integridad;
        public CertificadoDAL_83KI(Func<int> dni, Action<int, SqlConnection, SqlTransaction> integridad)
        { _dni = dni ?? throw new ArgumentNullException(nameof(dni)); _integridad = integridad ?? throw new ArgumentNullException(nameof(integridad)); }

        public List<Comision_83KI> ListarFinalizadas()
        {
            var result = new List<Comision_83KI>();
            var rows = _acceso.Leer("SELECT IdComision FROM dbo.Comision WHERE CONVERT(date,FechaFin)<=CONVERT(date,GETDATE()) ORDER BY Codigo,IdComision").Tables[0].Rows;
            var commissions = new ComisionDAL_83KI();
            foreach (DataRow row in rows) result.Add(commissions.ObtenerComision(Convert.ToInt32(row[0])));
            return result;
        }
        private const string AlumnosSql = @"SELECT si.IdSolicitudInscripcion,a.DNI,a.Nombre,a.Apellido,a.Email,cur.Nombre Curso,
            c.Codigo CodigoComision,c.FechaFin FechaFinComision,CONVERT(date,GETDATE()) Hoy,
            (SELECT r.Calificacion FROM dbo.ResultadoEvaluacion r WITH(HOLDLOCK)
                WHERE r.IdInscripcion=si.IdSolicitudInscripcion AND r.IdComision=si.IdComision
                ORDER BY r.FechaEvaluacion OFFSET 0 ROWS FETCH NEXT 1 ROWS ONLY) Evaluacion1,
            (SELECT r.Calificacion FROM dbo.ResultadoEvaluacion r WITH(HOLDLOCK)
                WHERE r.IdInscripcion=si.IdSolicitudInscripcion AND r.IdComision=si.IdComision
                ORDER BY r.FechaEvaluacion OFFSET 1 ROWS FETCH NEXT 1 ROWS ONLY) Evaluacion2,
            (SELECT COUNT(*) FROM dbo.ResultadoEvaluacion r WITH(HOLDLOCK)
                WHERE r.IdInscripcion=si.IdSolicitudInscripcion AND r.IdComision=si.IdComision) CantidadNotas
            FROM dbo.SolicitudInscripcion si WITH(UPDLOCK,HOLDLOCK)
            JOIN dbo.Alumno a WITH(HOLDLOCK) ON a.IdAlumno=si.IdAlumno
            JOIN dbo.Comision c WITH(HOLDLOCK) ON c.IdComision=si.IdComision
            JOIN dbo.Curso cur WITH(HOLDLOCK) ON cur.IdCurso=c.IdCurso
            WHERE CONVERT(date,c.FechaFin)<=CONVERT(date,GETDATE()) ";
        public List<AlumnoCertificacion_83KI> Buscar(int idComision)
        {
            var result = new List<AlumnoCertificacion_83KI>();
            _acceso.EjecutarTransaccion((conn, tran) =>
            {
                Autorizar(conn, tran);
                var rows = AccesoDAL_83KI.LeerTransaccional(conn, tran, AlumnosSql + " AND si.IdComision=@id ORDER BY a.Apellido,a.Nombre", P(idComision)).Tables[0].Rows;
                foreach (DataRow row in rows) { var a = Mapear(row); if (a.CantidadNotas == 2) result.Add(a); }
            });
            return result;
        }
        public AlumnoCertificacion_83KI RevalidarEnTransaccion(int idInscripcion, Func<AlumnoCertificacion_83KI, CertificadoFinalizacion_83KI> resolver)
        {
            AlumnoCertificacion_83KI actual = null;
            _acceso.EjecutarTransaccion((conn, tran) =>
            {
                // Un único bloqueo de escritura para toda la institución protege los folios únicos Y el DVV de toda la tabla.
                int code = Convert.ToInt32(AccesoDAL_83KI.LeerEscalarTransaccional(conn, tran, @"DECLARE @r int;
                    EXEC @r=sys.sp_getapplock @Resource=N'CUN09:Certificado',@LockMode='Exclusive',@LockOwner='Transaction',@LockTimeout=15000; SELECT @r;"));
                if (code < 0) throw new InvalidOperationException("Cun09.Concurrente");
                int resultadosLock = Convert.ToInt32(AccesoDAL_83KI.LeerEscalarTransaccional(conn, tran, @"DECLARE @r int;
                    EXEC @r=sys.sp_getapplock @Resource=N'CUN08:ResultadoEvaluacion',@LockMode='Exclusive',@LockOwner='Transaction',@LockTimeout=15000; SELECT @r;"));
                if (resultadosLock < 0) throw new InvalidOperationException("Cun09.Concurrente");
                Autorizar(conn, tran);
                var rows = AccesoDAL_83KI.LeerTransaccional(conn, tran, AlumnosSql + " AND si.IdSolicitudInscripcion=@id", P(idInscripcion)).Tables[0].Rows;
                if (rows.Count != 1) throw new InvalidOperationException("Cun09.NoHabilitado");
                actual = Mapear(rows[0]);
                if (actual.CantidadNotas != 2) throw new InvalidOperationException("Cun09.SinNotas");
                foreach (DataRow row in AccesoDAL_83KI.LeerTransaccional(conn, tran,
                    "SELECT NumeroCuota,BalanceAdeudado,FechaVencimiento FROM dbo.Cuota WITH(UPDLOCK,HOLDLOCK) WHERE IdSolicitudInscripcion=@id ORDER BY NumeroCuota", P(idInscripcion)).Tables[0].Rows)
                {
                    decimal balance = Convert.ToDecimal(row["BalanceAdeudado"]);
                    actual.Balance += balance;
                    if (balance > 0) actual.Cuotas.Add(new CuotaCertificacion_83KI { Numero = Convert.ToInt32(row["NumeroCuota"]), Balance = balance, Vencimiento = Convert.ToDateTime(row["FechaVencimiento"]) });
                }
                var observations = new List<string>();
                foreach (DataRow row in AccesoDAL_83KI.LeerTransaccional(conn, tran,
                    "SELECT Observaciones FROM dbo.ResultadoEvaluacion WITH(HOLDLOCK) WHERE IdInscripcion=@id ORDER BY FechaEvaluacion,IdResultadoEvaluacion", P(idInscripcion)).Tables[0].Rows)
                    if (!string.IsNullOrWhiteSpace(row[0].ToString())) observations.Add(row[0].ToString());
                actual.ObservacionesProfesor = string.Join(Environment.NewLine, observations);
                var certificates = AccesoDAL_83KI.LeerTransaccional(conn, tran,
                    "SELECT * FROM dbo.CertificadoFinalizacion WITH(UPDLOCK,HOLDLOCK) WHERE IdInscripcion=@id", P(idInscripcion)).Tables[0].Rows;
                if (certificates.Count > 0) actual.Certificado = MapearCertificado(certificates[0]);
                var snapshot = resolver(actual);
                if (snapshot == null) return;
                if (actual.Certificado != null || !actual.Aprobado || actual.Balance != 0 || snapshot.IdInscripcion != idInscripcion || snapshot.Fecha.Date != actual.Hoy.Date)
                    throw new InvalidOperationException("Cun09.NoHabilitado");
                var p = Valores(snapshot);
                try
                {
                    // La restricción existente decide la unicidad con la intercalación real de SQL, sin comparación alternativa en memoria.
                    snapshot.IdCertificado = Convert.ToInt32(AccesoDAL_83KI.LeerEscalarTransaccional(conn, tran, @"INSERT dbo.CertificadoFinalizacion
                        (IdInscripcion,Nombre,Apellido,Curso,Promedio,Firmante,Cargo,Folio,Fecha,Modalidad,Email,Mencion,Observaciones,DVH)
                        OUTPUT inserted.IdCertificado VALUES(@id,@nombre,@apellido,@curso,@promedio,@firmante,@cargo,@folio,@fecha,@modalidad,@email,@mencion,@obs,'')", p));
                }
                catch (SqlException ex) when ((ex.Number == 2601 || ex.Number == 2627) && ex.Message.IndexOf("UQ_Certificado_Folio", StringComparison.OrdinalIgnoreCase) >= 0)
                { throw new InvalidOperationException("Cun09.FolioDuplicado", ex); }
                _integridad(snapshot.IdCertificado, conn, tran);
                actual.Certificado = MapearCertificado(AccesoDAL_83KI.LeerTransaccional(conn, tran,
                    "SELECT * FROM dbo.CertificadoFinalizacion WHERE IdInscripcion=@id", P(idInscripcion)).Tables[0].Rows[0]);
            });
            return actual;
        }
        private void Autorizar(SqlConnection conn, SqlTransaction tran)
        {
            int dni = _dni();
            if (dni <= 0) throw new InvalidOperationException("Errores.SinPermisos");
            AccesoDAL_83KI.EscribirTransaccional(conn, tran, @"DECLARE @rol int;
                SELECT @rol=CodigoRol FROM dbo.Usuarios WITH(HOLDLOCK) WHERE DNI=@dni AND Activo=1 AND Bloqueado=0;
                DECLARE @permitido int=0;
                ;WITH f AS (
                    SELECT CodigoFamilia FROM dbo.RolFamilia WITH(HOLDLOCK) WHERE CodigoRol=@rol
                    UNION ALL SELECT ff.CodigoFamiliaHija FROM dbo.FamiliaFamilia ff WITH(HOLDLOCK) JOIN f ON ff.CodigoFamiliaPadre=f.CodigoFamilia
                ) SELECT @permitido=COUNT(*) FROM dbo.FamiliaPatente fp WITH(HOLDLOCK) JOIN f ON f.CodigoFamilia=fp.CodigoFamilia WHERE fp.CodigoPatente=72 OPTION(MAXRECURSION 100);
                IF @rol IS NULL OR (@permitido=0 AND NOT EXISTS(SELECT 1 FROM dbo.RolPatente WITH(HOLDLOCK) WHERE CodigoRol=@rol AND CodigoPatente=72))
                    THROW 52921,N'Errores.SinPermisos',1;", new List<SqlParameter> { new SqlParameter("@dni", dni) });
        }
        private static List<SqlParameter> P(int id) { return new List<SqlParameter> { new SqlParameter("@id", id) }; }
        private static AlumnoCertificacion_83KI Mapear(DataRow r)
        {
            return new AlumnoCertificacion_83KI { IdInscripcion = Convert.ToInt32(r["IdSolicitudInscripcion"]), DNI = r["DNI"].ToString(), Nombre = r["Nombre"].ToString(),
                Apellido = r["Apellido"].ToString(), Email = r.IsNull("Email") ? "" : r["Email"].ToString(), Curso = r["Curso"].ToString(),
                CodigoComision = r["CodigoComision"].ToString(), FechaFinComision = Convert.ToDateTime(r["FechaFinComision"]),
                CantidadNotas = Convert.ToInt32(r["CantidadNotas"]),
                Evaluacion1 = r.IsNull("Evaluacion1") ? 0 : Convert.ToDecimal(r["Evaluacion1"]),
                Evaluacion2 = r.IsNull("Evaluacion2") ? 0 : Convert.ToDecimal(r["Evaluacion2"]), Hoy = Convert.ToDateTime(r["Hoy"]) };
        }
        private static CertificadoFinalizacion_83KI MapearCertificado(DataRow r)
        {
            return new CertificadoFinalizacion_83KI { IdCertificado = Convert.ToInt32(r["IdCertificado"]), IdInscripcion = Convert.ToInt32(r["IdInscripcion"]),
                Nombre = r["Nombre"].ToString(), Apellido = r["Apellido"].ToString(), Curso = r["Curso"].ToString(), Promedio = Convert.ToDecimal(r["Promedio"]),
                Firmante = r["Firmante"].ToString(), Cargo = r["Cargo"].ToString(), Folio = r["Folio"].ToString(), Fecha = Convert.ToDateTime(r["Fecha"]),
                Modalidad = r["Modalidad"].ToString(), Email = r["Email"].ToString(), Mencion = r["Mencion"].ToString(), Observaciones = r["Observaciones"].ToString() };
        }
        private static List<SqlParameter> Valores(CertificadoFinalizacion_83KI s)
        {
            return new List<SqlParameter> { new SqlParameter("@id", s.IdInscripcion), new SqlParameter("@nombre", s.Nombre), new SqlParameter("@apellido", s.Apellido),
                new SqlParameter("@curso", s.Curso), new SqlParameter("@promedio", SqlDbType.Decimal) { Precision=28, Scale=16, Value=s.Promedio },
                new SqlParameter("@firmante", s.Firmante), new SqlParameter("@cargo", s.Cargo), new SqlParameter("@folio", s.Folio),
                new SqlParameter("@fecha", SqlDbType.Date) { Value=s.Fecha.Date }, new SqlParameter("@modalidad", s.Modalidad), new SqlParameter("@email", s.Email),
                new SqlParameter("@mencion", s.Mencion), new SqlParameter("@obs", s.Observaciones) };
        }
    }
}

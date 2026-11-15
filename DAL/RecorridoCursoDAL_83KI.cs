using BE.DTOs.Recorridos;
using BE.Entidades;
using DAL.DAL;
using Service;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace DAL
{
    public sealed class RecorridoCursoDAL_83KI : IRecorridoCursoDAL_83KI
    {
        private readonly AccesoDAL_83KI _acceso = new AccesoDAL_83KI();
        private readonly IntegridadDAL_83KI _integridad = new IntegridadDAL_83KI(new Encriptador_83KI());
        public List<Curso_83KI> Cursos() { return new CursoDAL_83KI().ObtenerTodos().ToList(); }
        public List<RecorridoListado_83KI> Listar()
        {
            var lista = new List<RecorridoListado_83KI>();
            var ds = _acceso.Leer(@"SELECT r.*, p.Nombre CursoPrevio, c.Nombre CursoRecomendado,
                CAST(CASE WHEN p.EstadoActivo=1 AND c.EstadoActivo=1 THEN 1 ELSE 0 END AS bit) Activo
                FROM dbo.RecorridoCurso r JOIN dbo.Curso p ON p.IdCurso=r.IdCursoPrevio
                JOIN dbo.Curso c ON c.IdCurso=r.IdCursoRecomendado ORDER BY p.Nombre,r.Orden,r.IdRecorridoCurso");
            foreach (DataRow row in ds.Tables[0].Rows) lista.Add(new RecorridoListado_83KI
            {
                IdRecorridoCurso = (int)row["IdRecorridoCurso"], IdCursoPrevio = (int)row["IdCursoPrevio"],
                IdCursoRecomendado = (int)row["IdCursoRecomendado"], Orden = (int)row["Orden"],
                CursoPrevio = (string)row["CursoPrevio"], CursoRecomendado = (string)row["CursoRecomendado"], Activo = (bool)row["Activo"]
            });
            return lista;
        }
        private static void Bloquear(SqlConnection conn, SqlTransaction tran)
        {
            AccesoDAL_83KI.EscribirTransaccional(conn, tran, @"DECLARE @r int;
                EXEC @r=sys.sp_getapplock @Resource=N'RecorridoCurso:Maestros',@LockMode='Exclusive',@LockOwner='Transaction',@LockTimeout=15000;
                IF @r<0 THROW 53001,N'Inteligente.Concurrente',1;", new List<SqlParameter>());
        }
        public void Guardar(RecorridoCurso_83KI recorrido)
        {
            try
            {
                _acceso.EjecutarTransaccion((conn, tran) =>
                {
                    Bloquear(conn, tran);
                    var p = new List<SqlParameter> { new SqlParameter("@id", recorrido.IdRecorridoCurso),
                        new SqlParameter("@previo", recorrido.IdCursoPrevio), new SqlParameter("@siguiente", recorrido.IdCursoRecomendado), new SqlParameter("@orden", recorrido.Orden) };
                    string sql = @"IF NOT EXISTS(SELECT 1 FROM dbo.Curso WITH(UPDLOCK,HOLDLOCK) WHERE IdCurso=@previo AND EstadoActivo=1)
                        OR NOT EXISTS(SELECT 1 FROM dbo.Curso WITH(UPDLOCK,HOLDLOCK) WHERE IdCurso=@siguiente AND EstadoActivo=1)
                        THROW 53002,N'Inteligente.CursoInactivo',1;
                        IF @id=0 BEGIN INSERT dbo.RecorridoCurso(IdCursoPrevio,IdCursoRecomendado,Orden,DVH) VALUES(@previo,@siguiente,@orden,''); SELECT CONVERT(int,SCOPE_IDENTITY()); END
                        ELSE BEGIN UPDATE dbo.RecorridoCurso SET IdCursoPrevio=@previo,IdCursoRecomendado=@siguiente,Orden=@orden WHERE IdRecorridoCurso=@id;
                        IF @@ROWCOUNT=0 THROW 53003,N'Inteligente.NoEncontrado',1; SELECT @id; END";
                    int id = Convert.ToInt32(AccesoDAL_83KI.LeerEscalarTransaccional(conn, tran, sql, p));
                    Recalcular("RecorridoCurso", "IdRecorridoCurso", id, conn, tran);
                });
            }
            catch (SqlException ex) when (ex.Number == 2601 || ex.Number == 2627) { throw new InvalidOperationException("Inteligente.Duplicado", ex); }
        }
        public void Eliminar(int id)
        {
            _acceso.EjecutarTransaccion((conn, tran) =>
            {
                Bloquear(conn, tran);
                AccesoDAL_83KI.EscribirTransaccional(conn, tran, @"DELETE dbo.RecorridoCurso WHERE IdRecorridoCurso=@id;
                    IF @@ROWCOUNT=0 THROW 53003,N'Inteligente.NoEncontrado',1;", new List<SqlParameter> { new SqlParameter("@id", id) });
                _integridad.ActualizarDVVTransaccional("RecorridoCurso", conn, tran);
            });
        }
        public void GuardarReferencia(int idCurso, int? minimo)
        {
            _acceso.EjecutarTransaccion((conn, tran) =>
            {
                Bloquear(conn, tran);
                AccesoDAL_83KI.EscribirTransaccional(conn, tran, @"UPDATE dbo.Curso SET CupoMinimoReferencia=@minimo WHERE IdCurso=@id;
                    IF @@ROWCOUNT=0 THROW 53003,N'Inteligente.NoEncontrado',1;", new List<SqlParameter>
                    { new SqlParameter("@id", idCurso), new SqlParameter("@minimo", (object)minimo ?? DBNull.Value) });
                Recalcular("Curso", "IdCurso", idCurso, conn, tran);
            });
        }
        private void Recalcular(string tabla, string clave, int id, SqlConnection conn, SqlTransaction tran)
        {
            var valores = IntegridadDAL_83KI.LeerFilaTransaccional(tabla, clave + "=@id", new List<SqlParameter> { new SqlParameter("@id", id) }, conn, tran);
            _integridad.RecomputarIntegridadFila(tabla, valores, conn, tran);
        }
    }
}

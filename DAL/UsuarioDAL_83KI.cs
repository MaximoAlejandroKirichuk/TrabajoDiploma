using Service.Entidades;
using DAL.DAL;
using DAL.interfaces;
using Service;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class UsuarioDAL_83KI : IUsuarioDAL_83KI
    {
        private AccesoDAL_83KI _accesoDAL = new AccesoDAL_83KI();
        private IntegridadDAL_83KI _integridadDAL = new IntegridadDAL_83KI(new Encriptador_83KI());

        public Usuario_83KI ObtenerPorDni(int dni)
        {
            string sql = ConsultaUsuariosConRoles() + " WHERE u.DNI = @dni";
            var parametros = new List<SqlParameter> { new SqlParameter("@dni", dni) };

            DataSet ds = _accesoDAL.Leer(sql, parametros);
            return MapearUsuario(ds);
        }

        public Usuario_83KI ObtenerPorUserName(string userName)
        {
            string sql = ConsultaUsuariosConRoles() + " WHERE u.Username = @userName";
            var parametros = new List<SqlParameter> { new SqlParameter("@userName", userName) };

            DataSet ds = _accesoDAL.Leer(sql, parametros);
            return MapearUsuario(ds);
        }



        public void BloquearUsuario(Usuario_83KI usuario)
        {
            _accesoDAL.EjecutarTransaccion((conn, tran) =>
            {
                string consulta = "UPDATE Usuarios SET Bloqueado = 1, IntentosRealizados = @intentosRealizados, FechaUltimoIntento = @fechaUltimoIntento WHERE DNI = @dni";

                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    new SqlParameter("@dni", usuario.DNI),
                    new SqlParameter("@intentosRealizados", usuario.IntentosRealizados),
                    new SqlParameter("@fechaUltimoIntento", (object)usuario.FechaUltimoIntento ?? DBNull.Value)
                };

                AccesoDAL_83KI.EscribirTransaccional(conn, tran, consulta, parametros);
                RecomputarIntegridadUsuario(usuario.DNI, conn, tran);
            });
        }

        public void CrearUsuario(Usuario_83KI usuario)
        {
            _accesoDAL.EjecutarTransaccion((conn, tran) =>
            {
                string consulta = @"INSERT INTO Usuarios (Username, Nombre, Apellido, DNI, Email, CodigoRol, Contrasena, Activo, Bloqueado, IntentosRealizados, FechaUltimoIntento, IdiomaId) 
                            VALUES (@userName ,@nombre, @apellido, @dni, @email, @codigoRol, @pass, @activo, @bloqueado, @intentosRealizados, @fechaUltimoIntento, @idiomaId)";

                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    new SqlParameter("@username", usuario.UserName),
                    new SqlParameter("@nombre", usuario.Nombre),
                    new SqlParameter("@apellido", usuario.Apellido),
                    new SqlParameter("@dni",usuario.DNI),
                    new SqlParameter("@email", usuario.Email),
                    new SqlParameter("@codigoRol", usuario.Rol.CodigoRol),
                    new SqlParameter("@pass", usuario.Contrasena),
                    new SqlParameter("@activo", usuario.Activo),
                    new SqlParameter("@bloqueado", usuario.Bloqueado),
                    new SqlParameter("@intentosRealizados", usuario.IntentosRealizados),
                    new SqlParameter("@fechaUltimoIntento", (object)usuario.FechaUltimoIntento ?? DBNull.Value),
                    new SqlParameter("@idiomaId", usuario.IdiomaId)
                };

                AccesoDAL_83KI.EscribirTransaccional(conn, tran, consulta, parametros);
                RecomputarIntegridadUsuario(usuario.DNI, conn, tran);
            });
        }

        public void ModificarUsuario(int dni, string email, Rol_83KI rol)
        {
            _accesoDAL.EjecutarTransaccion((conn, tran) =>
            {
                string consulta = "UPDATE Usuarios SET Email = @email, CodigoRol = @codigoRol WHERE DNI = @dni";

                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    new SqlParameter("@dni", dni),
                    new SqlParameter("@email", email),
                    new SqlParameter("@codigoRol", rol.CodigoRol)
                };

                AccesoDAL_83KI.EscribirTransaccional(conn, tran, consulta, parametros);
                RecomputarIntegridadUsuario(dni, conn, tran);
            });
        }

        public void ActualizarContrasena(Usuario_83KI usuario)
        {
            _accesoDAL.EjecutarTransaccion((conn, tran) =>
            {
                string consulta = "UPDATE Usuarios SET Contrasena = @contrasena WHERE DNI = @dni";

                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    new SqlParameter("@dni", usuario.DNI),
                    new SqlParameter("@contrasena", usuario.Contrasena)
                };

                AccesoDAL_83KI.EscribirTransaccional(conn, tran, consulta, parametros);
                RecomputarIntegridadUsuario(usuario.DNI, conn, tran);
            });
        }

        public void ActualizarIdioma(int dni, string idiomaId)
        {
            _accesoDAL.EjecutarTransaccion((conn, tran) =>
            {
                string consulta = "UPDATE Usuarios SET IdiomaId = @idiomaId WHERE DNI = @dni";

                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    new SqlParameter("@dni", dni),
                    new SqlParameter("@idiomaId", idiomaId)
                };

                AccesoDAL_83KI.EscribirTransaccional(conn, tran, consulta, parametros);
                RecomputarIntegridadUsuario(dni, conn, tran);
            });
        }

        public bool ExisteDni(int dni)
        {
            string query = "SELECT COUNT(1) AS Total FROM Usuarios WHERE DNI = @dni";

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@dni", dni)
            };

            DataSet ds = _accesoDAL.Leer(query, parametros);

            // Verificamos si tiene datos y accedemos a la primera tabla, primera fila
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                int total = Convert.ToInt32(ds.Tables[0].Rows[0]["Total"]);
                return total > 0;
            }

            return false;
        }

        public bool ExisteEmail(string email)
        {
            string query = "SELECT COUNT(1) AS Total FROM Usuarios WHERE Email = @email";

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@email",email)
            };

            DataSet ds = _accesoDAL.Leer(query, parametros);

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                int total = Convert.ToInt32(ds.Tables[0].Rows[0]["Total"]);
                return total > 0;
            }
            return false;
        }

        public bool ExisteEmailParaOtroUsuario(string email, int dni)
        {
            //dni distinto al que estoy mandando <>
            string query = "SELECT COUNT(1) AS Total FROM Usuarios WHERE Email = @email AND DNI <> @dni";

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@email", email),
                new SqlParameter("@dni", dni)
            };

            DataSet ds = _accesoDAL.Leer(query, parametros);

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                int total = Convert.ToInt32(ds.Tables[0].Rows[0]["Total"]);
                return total > 0;
            }

            return false;
        }

        public IEnumerable<Usuario_83KI> ObtenerUsuarios()
        {
            string query = ConsultaUsuariosConRoles();
            DataSet ds = _accesoDAL.Leer(query);
            List<Usuario_83KI> listaTemporal = new List<Usuario_83KI>();

            if (ds != null && ds.Tables.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    listaTemporal.Add(MapearUsuario(row));
                }
            }
            return listaTemporal;
        }

        public void DesbloquearCuenta(Usuario_83KI usuario)
        {
            _accesoDAL.EjecutarTransaccion((conn, tran) =>
            {
                string consulta = @"UPDATE Usuarios
                                    SET Bloqueado = 0,
                                        Contrasena = @contrasena,
                                        IntentosRealizados = 0,
                                        FechaUltimoIntento = NULL
                                    WHERE DNI = @dni";

                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    new SqlParameter("@dni", usuario.DNI),
                    new SqlParameter("@contrasena", usuario.Contrasena)
                };

                AccesoDAL_83KI.EscribirTransaccional(conn, tran, consulta, parametros);
                RecomputarIntegridadUsuario(usuario.DNI, conn, tran);
            });
        }

        public void ActualizarIntentosFallidos(Usuario_83KI usuario)
        {
            _accesoDAL.EjecutarTransaccion((conn, tran) =>
            {
                string consulta = "UPDATE Usuarios SET IntentosRealizados = @intentosRealizados, FechaUltimoIntento = @fechaUltimoIntento WHERE DNI = @dni";

                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    new SqlParameter("@dni", usuario.DNI),
                    new SqlParameter("@intentosRealizados", usuario.IntentosRealizados),
                    new SqlParameter("@fechaUltimoIntento", (object)usuario.FechaUltimoIntento ?? DBNull.Value)
                };

                AccesoDAL_83KI.EscribirTransaccional(conn, tran, consulta, parametros);
                RecomputarIntegridadUsuario(usuario.DNI, conn, tran);
            });
        }

        public void ReiniciarIntentosFallidos(Usuario_83KI usuario)
        {
            _accesoDAL.EjecutarTransaccion((conn, tran) =>
            {
                string consulta = "UPDATE Usuarios SET IntentosRealizados = 0, FechaUltimoIntento = NULL WHERE DNI = @dni";

                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    new SqlParameter("@dni", usuario.DNI)
                };

                AccesoDAL_83KI.EscribirTransaccional(conn, tran, consulta, parametros);
                RecomputarIntegridadUsuario(usuario.DNI, conn, tran);
            });
        }

        public void ActualizarEstadoActivo(int dni, bool activo)
        {
            _accesoDAL.EjecutarTransaccion((conn, tran) =>
            {
                string consulta = "UPDATE Usuarios SET Activo = @activo WHERE DNI = @dni";

                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    new SqlParameter("@dni", dni),
                    new SqlParameter("@activo", activo)
                };

                AccesoDAL_83KI.EscribirTransaccional(conn, tran, consulta, parametros);
                RecomputarIntegridadUsuario(dni, conn, tran);
            });
        }

        public bool EstaBloqueado(int dni)
        {
            string query = "SELECT COUNT(1) AS Total FROM Usuarios WHERE DNI = @dni";

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@dni",dni)
            };

            DataSet ds = _accesoDAL.Leer(query, parametros);

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                int total = Convert.ToInt32(ds.Tables[0].Rows[0]["Total"]);
                return total > 0;
            }
            return false;
        }

        private string ConsultaUsuariosConRoles()
        {
            return @"SELECT u.Username AS UserName,
                            u.Nombre,
                            u.Apellido,
                            u.DNI,
                            u.Email,
                            u.Contrasena,
                            u.Activo,
                            u.Bloqueado,
                            u.IntentosRealizados,
                            u.FechaUltimoIntento,
                            u.IdiomaId,
                            u.CodigoRol,
                            r.Nombre AS NombreRol
                     FROM Usuarios u
                     INNER JOIN Roles r ON r.CodigoRol = u.CodigoRol";
        }

        private Usuario_83KI MapearUsuario(DataSet ds)
        {
            if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
            {
                return null;
            }

            return MapearUsuario(ds.Tables[0].Rows[0]);
        }

        private Usuario_83KI MapearUsuario(DataRow row)
        {
            return Usuario_83KI.ReconstruirDesdePersistencia(
                (int)Convert.ToInt64(row["DNI"]),
                ObtenerTexto(row, "Nombre"),
                ObtenerTexto(row, "Apellido"),
                ObtenerTexto(row, "Email"),
                ObtenerTexto(row, "Contrasena"),
                MapearRolDesdeUsuario(row),
                Convert.ToBoolean(row["Activo"]),
                Convert.ToBoolean(row["Bloqueado"]),
                ObtenerTexto(row, "IdiomaId"),
                ObtenerEntero(row, "IntentosRealizados"),
                ObtenerFechaNullable(row, "FechaUltimoIntento")
            );
        }

        private Rol_83KI MapearRolDesdeUsuario(DataRow row)
        {
            return new Rol_83KI(
                Convert.ToInt32(row["CodigoRol"]),
                ObtenerTexto(row, "NombreRol")
            );
        }

        private string ObtenerTexto(DataRow row, string columna)
        {
            if (!row.Table.Columns.Contains(columna) || row[columna] == DBNull.Value)
            {
                return string.Empty;
            }

            return row[columna].ToString();
        }

        private int ObtenerEntero(DataRow row, string columna)
        {
            if (!row.Table.Columns.Contains(columna) || row[columna] == DBNull.Value)
            {
                return 0;
            }

            return Convert.ToInt32(row[columna]);
        }

        private DateTime? ObtenerFechaNullable(DataRow row, string columna)
        {
            if (!row.Table.Columns.Contains(columna) || row[columna] == DBNull.Value)
            {
                return null;
            }

            return Convert.ToDateTime(row[columna]);
        }

        /// <summary>
        /// lee la fila actual de usuarios y recalcula su dvh + dvv de tabla
        /// dentro de la transaccion dada.
        /// </summary>
        private void RecomputarIntegridadUsuario(int dni, SqlConnection conn, SqlTransaction tran)
        {
            var valores = IntegridadDAL_83KI.LeerFilaTransaccional(
                "Usuarios",
                "DNI = @dni",
                new List<SqlParameter> { new SqlParameter("@dni", dni) },
                conn,
                tran);

            if (valores.Count > 0)
            {
                _integridadDAL.RecomputarIntegridadFila("Usuarios", valores, conn, tran);
            }
        }
    }
}


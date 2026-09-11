using DAL.DAL;
using DAL.interfaces;
using Service;
using Service.Entidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace DAL
{
    public class RolDAL_83KI : IRolDAL_83KI
    {
        private readonly AccesoDAL_83KI _accesoDAL = new AccesoDAL_83KI();
        private readonly IntegridadDAL_83KI _integridadDAL = new IntegridadDAL_83KI(new Encriptador_83KI());

        public IEnumerable<Rol_83KI> ObtenerRoles()
        {
            string query = "SELECT CodigoRol, Nombre FROM Roles ORDER BY Nombre";
            DataSet ds = _accesoDAL.Leer(query);
            List<Rol_83KI> roles = new List<Rol_83KI>();

            if (ds != null && ds.Tables.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    roles.Add(MapearRol(row));
                }
            }

            return roles;
        }

        public IEnumerable<Rol_83KI> ObtenerRolesConPermisos()
        {
            Dictionary<int, Rol_83KI> roles = ObtenerRoles().ToDictionary(r => r.CodigoRol);
            Dictionary<int, Familia_83KI> familias = ObtenerFamilias().ToDictionary(f => f.CodigoFamilia);
            Dictionary<int, Patente_83KI> patentes = ObtenerPatentes().ToDictionary(p => p.CodigoPatente);

            CargarRelacionesFamilias(familias, patentes);
            CargarRelacionesRoles(roles, familias, patentes);

            return roles.Values.OrderBy(r => r.Nombre).ToList();
        }

        public IEnumerable<Familia_83KI> ObtenerFamilias()
        {
            string query = "SELECT CodigoFamilia, Nombre FROM Familias ORDER BY Nombre";
            DataSet ds = _accesoDAL.Leer(query);
            Dictionary<int, Familia_83KI> familias = new Dictionary<int, Familia_83KI>();

            if (ds != null && ds.Tables.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    Familia_83KI familia = MapearFamilia(row);
                    familias[familia.CodigoFamilia] = familia;
                }
            }

            Dictionary<int, Patente_83KI> patentes = ObtenerPatentes().ToDictionary(p => p.CodigoPatente);
            CargarRelacionesFamilias(familias, patentes);
            return familias.Values.OrderBy(f => f.Nombre).ToList();
        }

        public IEnumerable<Patente_83KI> ObtenerPatentes()
        {
            string query = "SELECT CodigoPatente, Nombre FROM Patentes ORDER BY Nombre";
            DataSet ds = _accesoDAL.Leer(query);
            List<Patente_83KI> patentes = new List<Patente_83KI>();

            if (ds != null && ds.Tables.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    patentes.Add(MapearPatente(row));
                }
            }

            return patentes;
        }

        public Rol_83KI CrearRol(string nombre)
        {
            Rol_83KI rol = null;
            _accesoDAL.EjecutarTransaccion((conn, tran) =>
            {
                string consulta = @"INSERT INTO Roles (Nombre)
                                    OUTPUT INSERTED.CodigoRol
                                    VALUES (@nombre)";
                object codigo = AccesoDAL_83KI.LeerEscalarTransaccional(conn, tran, consulta,
                    new List<SqlParameter> { new SqlParameter("@nombre", nombre) });
                int codigoRol = Convert.ToInt32(codigo);
                rol = new Rol_83KI(codigoRol, nombre);

                RecomputarIntegridadRol(codigoRol, conn, tran);
            });
            return rol;
        }

        public Familia_83KI CrearFamilia(string nombre)
        {
            Familia_83KI familia = null;
            _accesoDAL.EjecutarTransaccion((conn, tran) =>
            {
                string consulta = @"INSERT INTO Familias (Nombre)
                                    OUTPUT INSERTED.CodigoFamilia
                                    VALUES (@nombre)";
                object codigo = AccesoDAL_83KI.LeerEscalarTransaccional(conn, tran, consulta,
                    new List<SqlParameter> { new SqlParameter("@nombre", nombre) });
                int codigoFamilia = Convert.ToInt32(codigo);
                familia = new Familia_83KI(codigoFamilia, nombre);

                RecomputarIntegridadFamilia(codigoFamilia, conn, tran);
            });
            return familia;
        }

        public Rol_83KI CrearRolConComponentes(string nombre, List<int> codigosPatentes, List<int> codigosFamilias)
        {
            Rol_83KI rol = null;

            _accesoDAL.EjecutarTransaccion((conn, tran) =>
            {
                // Crear el rol
                string consultaRol = @"INSERT INTO Roles (Nombre)
                                       OUTPUT INSERTED.CodigoRol
                                       VALUES (@nombre)";
                object codigo = AccesoDAL_83KI.LeerEscalarTransaccional(conn, tran, consultaRol,
                    new List<SqlParameter> { new SqlParameter("@nombre", nombre) });
                int codigoRol = Convert.ToInt32(codigo);
                rol = new Rol_83KI(codigoRol, nombre);

                // Asignar patentes
                if (codigosPatentes != null)
                {
                    foreach (int codigoPatente in codigosPatentes)
                    {
                        AccesoDAL_83KI.EscribirTransaccional(conn, tran,
                            "INSERT INTO RolPatente (CodigoRol, CodigoPatente) VALUES (@codigoRol, @codigoPatente)",
                            new List<SqlParameter>
                            {
                                new SqlParameter("@codigoRol", codigoRol),
                                new SqlParameter("@codigoPatente", codigoPatente)
                            });
                        RecomputarIntegridadJoinRolPatente(codigoRol, codigoPatente, conn, tran);
                    }
                }

                // Asignar familias
                if (codigosFamilias != null)
                {
                    foreach (int codigoFamilia in codigosFamilias)
                    {
                        AccesoDAL_83KI.EscribirTransaccional(conn, tran,
                            "INSERT INTO RolFamilia (CodigoRol, CodigoFamilia) VALUES (@codigoRol, @codigoFamilia)",
                            new List<SqlParameter>
                            {
                                new SqlParameter("@codigoRol", codigoRol),
                                new SqlParameter("@codigoFamilia", codigoFamilia)
                            });
                        RecomputarIntegridadJoinRolFamilia(codigoRol, codigoFamilia, conn, tran);
                    }
                }

                // integridad: recalcular dvh + dvv de roles
                RecomputarIntegridadRol(codigoRol, conn, tran);
            });

            return rol;
        }

        public Familia_83KI CrearFamiliaConComponentes(string nombre, List<int> codigosPatentes, List<int> codigosFamilias)
        {
            Familia_83KI familia = null;

            _accesoDAL.EjecutarTransaccion((conn, tran) =>
            {
                // Crear la familia
                string consultaFamilia = @"INSERT INTO Familias (Nombre)
                                           OUTPUT INSERTED.CodigoFamilia
                                           VALUES (@nombre)";
                object codigo = AccesoDAL_83KI.LeerEscalarTransaccional(conn, tran, consultaFamilia,
                    new List<SqlParameter> { new SqlParameter("@nombre", nombre) });
                int codigoFamilia = Convert.ToInt32(codigo);
                familia = new Familia_83KI(codigoFamilia, nombre);

                // Asignar patentes
                if (codigosPatentes != null)
                {
                    foreach (int codigoPatente in codigosPatentes)
                    {
                        AccesoDAL_83KI.EscribirTransaccional(conn, tran,
                            "INSERT INTO FamiliaPatente (CodigoFamilia, CodigoPatente) VALUES (@codigoFamilia, @codigoPatente)",
                            new List<SqlParameter>
                            {
                                new SqlParameter("@codigoFamilia", codigoFamilia),
                                new SqlParameter("@codigoPatente", codigoPatente)
                            });
                        RecomputarIntegridadJoinFamiliaPatente(codigoFamilia, codigoPatente, conn, tran);
                    }
                }

                // Asignar subfamilias
                if (codigosFamilias != null)
                {
                    foreach (int codigoFamiliaHija in codigosFamilias)
                    {
                        AccesoDAL_83KI.EscribirTransaccional(conn, tran,
                            "INSERT INTO FamiliaFamilia (CodigoFamiliaPadre, CodigoFamiliaHija) VALUES (@codigoFamiliaPadre, @codigoFamiliaHija)",
                            new List<SqlParameter>
                            {
                                new SqlParameter("@codigoFamiliaPadre", codigoFamilia),
                                new SqlParameter("@codigoFamiliaHija", codigoFamiliaHija)
                            });
                        RecomputarIntegridadJoinFamiliaFamilia(codigoFamilia, codigoFamiliaHija, conn, tran);
                    }
                }

                // integridad: recalcular dvh + dvv de familias
                RecomputarIntegridadFamilia(codigoFamilia, conn, tran);
            });

            return familia;
        }

        public void EliminarFamilia(int codigoFamilia)
        {
            _accesoDAL.EjecutarTransaccion((conn, tran) =>
            {
                AccesoDAL_83KI.EscribirTransaccional(conn, tran,
                    "DELETE FROM FamiliaPatente WHERE CodigoFamilia = @codigoFamilia",
                    new List<SqlParameter> { new SqlParameter("@codigoFamilia", codigoFamilia) });
                AccesoDAL_83KI.EscribirTransaccional(conn, tran,
                    "DELETE FROM FamiliaFamilia WHERE CodigoFamiliaPadre = @codigoFamilia OR CodigoFamiliaHija = @codigoFamilia",
                    new List<SqlParameter> { new SqlParameter("@codigoFamilia", codigoFamilia) });
                AccesoDAL_83KI.EscribirTransaccional(conn, tran,
                    "DELETE FROM Familias WHERE CodigoFamilia = @codigoFamilia",
                    new List<SqlParameter> { new SqlParameter("@codigoFamilia", codigoFamilia) });

                // refrescar dvv de las tablas join afectadas (la fila en familias ya no existe)
                _integridadDAL.ActualizarDVVTransaccional("FamiliaPatente", conn, tran);
                _integridadDAL.ActualizarDVVTransaccional("FamiliaFamilia", conn, tran);
                _integridadDAL.ActualizarDVVTransaccional("Familias", conn, tran);
            });
        }

        public bool FamiliaAsignadaARol(int codigoFamilia)
        {
            string consulta = "SELECT COUNT(1) FROM RolFamilia WHERE CodigoFamilia = @codigoFamilia";

            object total = _accesoDAL.LeerEscalar(
                consulta,
                new List<SqlParameter> { new SqlParameter("@codigoFamilia", codigoFamilia) });

            return Convert.ToInt32(total) > 0;
        }

        public bool FamiliaEsSubfamiliaDeOtra(int codigoFamilia)
        {
            string consulta = "SELECT COUNT(1) FROM FamiliaFamilia WHERE CodigoFamiliaHija = @codigoFamilia";

            object total = _accesoDAL.LeerEscalar(
                consulta,
                new List<SqlParameter> { new SqlParameter("@codigoFamilia", codigoFamilia) });

            return Convert.ToInt32(total) > 0;
        }

        public bool RolTieneUsuarios(int codigoRol)
        {
            string consulta = "SELECT COUNT(1) FROM Usuarios WHERE CodigoRol = @codigoRol";
            object total = _accesoDAL.LeerEscalar(
                consulta,
                new List<SqlParameter> { new SqlParameter("@codigoRol", codigoRol) });
            return Convert.ToInt32(total) > 0;
        }

        public void EliminarRol(int codigoRol)
        {
            _accesoDAL.EjecutarTransaccion((conn, tran) =>
            {
                AccesoDAL_83KI.EscribirTransaccional(conn, tran,
                    "DELETE FROM RolPatente WHERE CodigoRol = @codigoRol",
                    new List<SqlParameter> { new SqlParameter("@codigoRol", codigoRol) });
                AccesoDAL_83KI.EscribirTransaccional(conn, tran,
                    "DELETE FROM RolFamilia WHERE CodigoRol = @codigoRol",
                    new List<SqlParameter> { new SqlParameter("@codigoRol", codigoRol) });
                AccesoDAL_83KI.EscribirTransaccional(conn, tran,
                    "DELETE FROM Roles WHERE CodigoRol = @codigoRol",
                    new List<SqlParameter> { new SqlParameter("@codigoRol", codigoRol) });

                // refrescar dvv de las tablas afectadas
                _integridadDAL.ActualizarDVVTransaccional("RolPatente", conn, tran);
                _integridadDAL.ActualizarDVVTransaccional("RolFamilia", conn, tran);
                _integridadDAL.ActualizarDVVTransaccional("Roles", conn, tran);
            });
        }

        public void AsignarPatenteAFamilia(int codigoFamilia, int codigoPatente)
        {
            InsertarJoinConIntegridad(
                "FamiliaPatente",
                "INSERT INTO FamiliaPatente (CodigoFamilia, CodigoPatente) VALUES (@codigoFamilia, @codigoPatente)",
                new SqlParameter("@codigoFamilia", codigoFamilia),
                new SqlParameter("@codigoPatente", codigoPatente));
        }

        public void QuitarPatenteDeFamilia(int codigoFamilia, int codigoPatente)
        {
            BorrarJoinConIntegridad(
                "FamiliaPatente",
                "DELETE FROM FamiliaPatente WHERE CodigoFamilia = @codigoFamilia AND CodigoPatente = @codigoPatente",
                new SqlParameter("@codigoFamilia", codigoFamilia),
                new SqlParameter("@codigoPatente", codigoPatente));
        }

        public void AsignarFamiliaAFamilia(int codigoFamiliaPadre, int codigoFamiliaHija)
        {
            InsertarJoinConIntegridad(
                "FamiliaFamilia",
                "INSERT INTO FamiliaFamilia (CodigoFamiliaPadre, CodigoFamiliaHija) VALUES (@padre, @hija)",
                new SqlParameter("@padre", codigoFamiliaPadre),
                new SqlParameter("@hija", codigoFamiliaHija));
        }

        public void QuitarFamiliaDeFamilia(int codigoFamiliaPadre, int codigoFamiliaHija)
        {
            BorrarJoinConIntegridad(
                "FamiliaFamilia",
                "DELETE FROM FamiliaFamilia WHERE CodigoFamiliaPadre = @padre AND CodigoFamiliaHija = @hija",
                new SqlParameter("@padre", codigoFamiliaPadre),
                new SqlParameter("@hija", codigoFamiliaHija));
        }

        public void AsignarPatenteARol(int codigoRol, int codigoPatente)
        {
            InsertarJoinConIntegridad(
                "RolPatente",
                "INSERT INTO RolPatente (CodigoRol, CodigoPatente) VALUES (@codigoRol, @codigoPatente)",
                new SqlParameter("@codigoRol", codigoRol),
                new SqlParameter("@codigoPatente", codigoPatente));
        }

        public void QuitarPatenteDeRol(int codigoRol, int codigoPatente)
        {
            BorrarJoinConIntegridad(
                "RolPatente",
                "DELETE FROM RolPatente WHERE CodigoRol = @codigoRol AND CodigoPatente = @codigoPatente",
                new SqlParameter("@codigoRol", codigoRol),
                new SqlParameter("@codigoPatente", codigoPatente));
        }

        public void AsignarFamiliaARol(int codigoRol, int codigoFamilia)
        {
            InsertarJoinConIntegridad(
                "RolFamilia",
                "INSERT INTO RolFamilia (CodigoRol, CodigoFamilia) VALUES (@codigoRol, @codigoFamilia)",
                new SqlParameter("@codigoRol", codigoRol),
                new SqlParameter("@codigoFamilia", codigoFamilia));
        }

        public void QuitarFamiliaDeRol(int codigoRol, int codigoFamilia)
        {
            BorrarJoinConIntegridad(
                "RolFamilia",
                "DELETE FROM RolFamilia WHERE CodigoRol = @codigoRol AND CodigoFamilia = @codigoFamilia",
                new SqlParameter("@codigoRol", codigoRol),
                new SqlParameter("@codigoFamilia", codigoFamilia));
        }

        private Rol_83KI MapearRol(DataRow row)
        {
            return new Rol_83KI(
                Convert.ToInt32(row["CodigoRol"]),
                ObtenerTexto(row, "Nombre")
            );
        }

        private Familia_83KI MapearFamilia(DataRow row)
        {
            return new Familia_83KI(
                Convert.ToInt32(row["CodigoFamilia"]),
                ObtenerTexto(row, "Nombre")
            );
        }

        private Patente_83KI MapearPatente(DataRow row)
        {
            return new Patente_83KI(
                Convert.ToInt32(row["CodigoPatente"]),
                ObtenerTexto(row, "Nombre")
            );
        }

        private void CargarRelacionesFamilias(Dictionary<int, Familia_83KI> familias, Dictionary<int, Patente_83KI> patentes)
        {
            DataSet dsPatentes = _accesoDAL.Leer("SELECT CodigoFamilia, CodigoPatente FROM FamiliaPatente");

            if (dsPatentes != null && dsPatentes.Tables.Count > 0)
            {
                foreach (DataRow row in dsPatentes.Tables[0].Rows)
                {
                    int codigoFamilia = Convert.ToInt32(row["CodigoFamilia"]);
                    int codigoPatente = Convert.ToInt32(row["CodigoPatente"]);

                    if (familias.ContainsKey(codigoFamilia) && patentes.ContainsKey(codigoPatente))
                    {
                        familias[codigoFamilia].CargarHijoDesdePersistencia(patentes[codigoPatente]);
                    }
                }
            }

            DataSet dsFamilias = _accesoDAL.Leer("SELECT CodigoFamiliaPadre, CodigoFamiliaHija FROM FamiliaFamilia");

            if (dsFamilias != null && dsFamilias.Tables.Count > 0)
            {
                foreach (DataRow row in dsFamilias.Tables[0].Rows)
                {
                    int codigoPadre = Convert.ToInt32(row["CodigoFamiliaPadre"]);
                    int codigoHija = Convert.ToInt32(row["CodigoFamiliaHija"]);

                    if (familias.ContainsKey(codigoPadre) && familias.ContainsKey(codigoHija))
                    {
                        familias[codigoPadre].CargarHijoDesdePersistencia(familias[codigoHija]);
                    }
                }
            }
        }

        private void CargarRelacionesRoles(Dictionary<int, Rol_83KI> roles, Dictionary<int, Familia_83KI> familias, Dictionary<int, Patente_83KI> patentes)
        {
            DataSet dsPatentes = _accesoDAL.Leer("SELECT CodigoRol, CodigoPatente FROM RolPatente");

            if (dsPatentes != null && dsPatentes.Tables.Count > 0)
            {
                foreach (DataRow row in dsPatentes.Tables[0].Rows)
                {
                    int codigoRol = Convert.ToInt32(row["CodigoRol"]);
                    int codigoPatente = Convert.ToInt32(row["CodigoPatente"]);

                    if (roles.ContainsKey(codigoRol) && patentes.ContainsKey(codigoPatente))
                    {
                        roles[codigoRol].CargarPatenteDesdePersistencia(patentes[codigoPatente]);
                    }
                }
            }

            DataSet dsFamilias = _accesoDAL.Leer("SELECT CodigoRol, CodigoFamilia FROM RolFamilia");

            if (dsFamilias != null && dsFamilias.Tables.Count > 0)
            {
                foreach (DataRow row in dsFamilias.Tables[0].Rows)
                {
                    int codigoRol = Convert.ToInt32(row["CodigoRol"]);
                    int codigoFamilia = Convert.ToInt32(row["CodigoFamilia"]);

                    if (roles.ContainsKey(codigoRol) && familias.ContainsKey(codigoFamilia))
                    {
                        roles[codigoRol].CargarFamiliaDesdePersistencia(familias[codigoFamilia]);
                    }
                }
            }
        }

        // ------------------------------------------------------------------ //
        //  helpers de integridad
        // ------------------------------------------------------------------ //

        /// <summary>inserta en tabla join, calcula dvh para la nueva fila y refresca dvv.</summary>
        private void InsertarJoinConIntegridad(string tabla, string sql, params SqlParameter[] parametros)
        {
            _accesoDAL.EjecutarTransaccion((conn, tran) =>
            {
                AccesoDAL_83KI.EscribirTransaccional(conn, tran, sql, parametros.ToList());
                // tras el insert, la nueva fila necesita dvh. se lee de vuelta y se recalcula.
                RecomputarIntegridadJoinDesdeParametros(tabla, parametros, conn, tran);
            });
        }

        /// <summary>elimina de tabla join y solo refresca dvv.</summary>
        private void BorrarJoinConIntegridad(string tabla, string sql, params SqlParameter[] parametros)
        {
            _accesoDAL.EjecutarTransaccion((conn, tran) =>
            {
                AccesoDAL_83KI.EscribirTransaccional(conn, tran, sql, parametros.ToList());
                _integridadDAL.ActualizarDVVTransaccional(tabla, conn, tran);
            });
        }

        /// <summary>
        /// lee de vuelta una fila de tabla join y recalcula su dvh + dvv.
        /// construye la clausula where desde los nombres de parametro (quitando el prefijo @).
        /// </summary>
        private void RecomputarIntegridadJoinDesdeParametros(
            string tabla, SqlParameter[] parametros, SqlConnection conn, SqlTransaction tran)
        {
            var whereClauses = new List<string>();
            var whereParams = new List<SqlParameter>();
            foreach (var p in parametros)
            {
                string colName = p.ParameterName.TrimStart('@');
                whereClauses.Add($"{colName} = @w_{colName}");
                whereParams.Add(new SqlParameter($"@w_{colName}", p.Value));
            }
            string whereClause = string.Join(" AND ", whereClauses);

            var valores = IntegridadDAL_83KI.LeerFilaTransaccional(tabla, whereClause, whereParams, conn, tran);
            if (valores.Count > 0)
            {
                _integridadDAL.RecomputarIntegridadFila(tabla, valores, conn, tran);
            }
        }

        private void RecomputarIntegridadRol(int codigoRol, SqlConnection conn, SqlTransaction tran)
        {
            var valores = IntegridadDAL_83KI.LeerFilaTransaccional(
                "Roles", "CodigoRol = @cr",
                new List<SqlParameter> { new SqlParameter("@cr", codigoRol) },
                conn, tran);
            if (valores.Count > 0)
                _integridadDAL.RecomputarIntegridadFila("Roles", valores, conn, tran);
        }

        private void RecomputarIntegridadFamilia(int codigoFamilia, SqlConnection conn, SqlTransaction tran)
        {
            var valores = IntegridadDAL_83KI.LeerFilaTransaccional(
                "Familias", "CodigoFamilia = @cf",
                new List<SqlParameter> { new SqlParameter("@cf", codigoFamilia) },
                conn, tran);
            if (valores.Count > 0)
                _integridadDAL.RecomputarIntegridadFila("Familias", valores, conn, tran);
        }

        // helpers de recomputo por tabla join usados por CrearRolConComponentes / CrearFamiliaConComponentes.
        private void RecomputarIntegridadJoinRolPatente(int codigoRol, int codigoPatente, SqlConnection conn, SqlTransaction tran)
        {
            RecomputarIntegridadJoinDesdeParametros("RolPatente",
                new[] { new SqlParameter("@CodigoRol", codigoRol), new SqlParameter("@CodigoPatente", codigoPatente) },
                conn, tran);
        }

        private void RecomputarIntegridadJoinRolFamilia(int codigoRol, int codigoFamilia, SqlConnection conn, SqlTransaction tran)
        {
            RecomputarIntegridadJoinDesdeParametros("RolFamilia",
                new[] { new SqlParameter("@CodigoRol", codigoRol), new SqlParameter("@CodigoFamilia", codigoFamilia) },
                conn, tran);
        }

        private void RecomputarIntegridadJoinFamiliaPatente(int codigoFamilia, int codigoPatente, SqlConnection conn, SqlTransaction tran)
        {
            RecomputarIntegridadJoinDesdeParametros("FamiliaPatente",
                new[] { new SqlParameter("@CodigoFamilia", codigoFamilia), new SqlParameter("@CodigoPatente", codigoPatente) },
                conn, tran);
        }

        private void RecomputarIntegridadJoinFamiliaFamilia(int codigoFamiliaPadre, int codigoFamiliaHija, SqlConnection conn, SqlTransaction tran)
        {
            RecomputarIntegridadJoinDesdeParametros("FamiliaFamilia",
                new[] { new SqlParameter("@CodigoFamiliaPadre", codigoFamiliaPadre), new SqlParameter("@CodigoFamiliaHija", codigoFamiliaHija) },
                conn, tran);
        }

        private string ObtenerTexto(DataRow row, string columna)
        {
            if (!row.Table.Columns.Contains(columna) || row[columna] == DBNull.Value)
            {
                return string.Empty;
            }

            return row[columna].ToString();
        }
    }
}

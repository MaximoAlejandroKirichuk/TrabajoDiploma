using DAL.DAL;
using DAL.interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Service.Entidades;

namespace DAL
{
    public class BitacoraEventoDAL_83KI : IBitacoraDAL_83KI
    {
        private readonly AccesoDAL_83KI _acceso = new AccesoDAL_83KI();

        public void Registrar(BitacoraEvento_83KI evento)
        {
            if (evento == null)
            {
                throw new ArgumentNullException(nameof(evento));
            }

            if (!Enum.IsDefined(typeof(Criticidad), evento.Criticidad))
            {
                throw new ArgumentException("La criticidad no es valida.", nameof(evento));
            }

            string consulta = "INSERT INTO BitacoraEventos (Criticidad, Descripcion, Fecha, Modulo, Username) " +
                              "VALUES (@crit, @desc, @fecha, @mod, @username)";

            List<SqlParameter> parametros = new List<SqlParameter>
            {
                new SqlParameter("@crit", (int)evento.Criticidad),
                new SqlParameter("@desc", evento.Descripcion),
                new SqlParameter("@fecha", evento.Fecha),
                new SqlParameter("@mod", evento.Modulo.ToString()),
                new SqlParameter("@username", evento.Username)

            };

            _acceso.Escribir(consulta, parametros);
        }

        public IEnumerable<BitacoraEvento_83KI> Consultar(DateTime desde, DateTime hasta)
        {
            List<SqlParameter> parametros = new List<SqlParameter>
            {
                new SqlParameter("@desde", desde),
                new SqlParameter("@hasta", hasta)
            };
            return ConsultarConParametros("SELECT * FROM BitacoraEventos WHERE Fecha BETWEEN @desde AND @hasta", parametros);
        }

        public IEnumerable<BitacoraEvento_83KI> ConsultarPorModulo(DateTime desde, DateTime hasta, Modulo modulo)
        {
            if (!Enum.IsDefined(typeof(Modulo), modulo))
            {
                throw new ArgumentException("El modulo no es valido.", nameof(modulo));
            }

            List<SqlParameter> parametros = new List<SqlParameter>
            {
                new SqlParameter("@desde", desde),
                new SqlParameter("@hasta", hasta),
                new SqlParameter("@modulo", modulo.ToString())
            };

            // Incluir valores historicos (0=Usuarios, 1=Admin, Seguridad=Admin) para no perder filas viejas
            string consulta = modulo == Modulo.Admin
                ? "SELECT * FROM BitacoraEventos WHERE Fecha BETWEEN @desde AND @hasta AND Modulo IN (@modulo, '1', 'Seguridad')"
                : "SELECT * FROM BitacoraEventos WHERE Fecha BETWEEN @desde AND @hasta AND Modulo IN (@modulo, '0')";

            return ConsultarConParametros(consulta, parametros);
        }

        public IEnumerable<BitacoraEvento_83KI> ConsultarPorCriticidad(DateTime desde, DateTime hasta, Criticidad criticidad)
        {
            if (!Enum.IsDefined(typeof(Criticidad), criticidad))
            {
                throw new ArgumentException("La criticidad no es valida.", nameof(criticidad));
            }

            List<SqlParameter> parametros = new List<SqlParameter>
            {
                new SqlParameter("@desde", desde),
                new SqlParameter("@hasta", hasta),
                new SqlParameter("@criticidad", (int)criticidad)
            };

            return ConsultarConParametros(
                "SELECT * FROM BitacoraEventos WHERE Fecha BETWEEN @desde AND @hasta AND Criticidad = @criticidad",
                parametros);
        }

        public IEnumerable<BitacoraEvento_83KI> ConsultarPorModuloYCriticidad(DateTime desde, DateTime hasta, Modulo modulo, Criticidad criticidad)
        {
            if (!Enum.IsDefined(typeof(Modulo), modulo))
            {
                throw new ArgumentException("El modulo no es valido.", nameof(modulo));
            }

            if (!Enum.IsDefined(typeof(Criticidad), criticidad))
            {
                throw new ArgumentException("La criticidad no es valida.", nameof(criticidad));
            }

            List<SqlParameter> parametros = new List<SqlParameter>
            {
                new SqlParameter("@desde", desde),
                new SqlParameter("@hasta", hasta),
                new SqlParameter("@modulo", modulo.ToString()),
                new SqlParameter("@criticidad", (int)criticidad)
            };

            // Incluir valores historicos (0=Usuarios, 1=Admin, Seguridad=Admin) para no perder filas viejas
            string consulta = modulo == Modulo.Admin
                ? "SELECT * FROM BitacoraEventos WHERE Fecha BETWEEN @desde AND @hasta AND Modulo IN (@modulo, '1', 'Seguridad') AND Criticidad = @criticidad"
                : "SELECT * FROM BitacoraEventos WHERE Fecha BETWEEN @desde AND @hasta AND Modulo IN (@modulo, '0') AND Criticidad = @criticidad";

            return ConsultarConParametros(consulta, parametros);
        }

        private IEnumerable<BitacoraEvento_83KI> ConsultarConParametros(string consulta, List<SqlParameter> parametros)
        {
            List<BitacoraEvento_83KI> lista = new List<BitacoraEvento_83KI>();
            DataSet ds = _acceso.Leer(consulta, parametros);

            if (ds.Tables.Count == 0)
            {
                return lista;
            }

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                lista.Add(MapearBitacora(row));
            }
            return lista;
        }

        private BitacoraEvento_83KI MapearBitacora(DataRow row)
        {
            return BitacoraEvento_83KI.ReconstruirDesdePersistencia(
                Convert.ToInt32(row["Id"]),
                Convert.ToDateTime(row["Fecha"]),
                ObtenerTexto(row, "Descripcion"),
                (Criticidad)Convert.ToInt32(row["Criticidad"]),
                ParsearModulo(row["Modulo"]),
                ObtenerTexto(row, "Username")
            );
        }

        private Modulo ParsearModulo(object valorModulo)
        {
            if (valorModulo == null || valorModulo == DBNull.Value)
            {
                return Modulo.Usuarios;
            }

            string valor = valorModulo.ToString().Trim();

            // Mapeos numericos historicos
            if (valor == "0")
            {
                return Modulo.Usuarios;
            }
            if (valor == "1")
            {
                return Modulo.Admin;
            }

            // Mapeo historico por texto: filas "Seguridad" -> Admin
            if (string.Equals(valor, "Seguridad", StringComparison.OrdinalIgnoreCase))
            {
                return Modulo.Admin;
            }

            // Parseo normal
            if (Enum.TryParse(valor, true, out Modulo modulo))
            {
                return modulo;
            }

            // Valores desconocidos -> valor seguro por defecto
            return Modulo.Usuarios;
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

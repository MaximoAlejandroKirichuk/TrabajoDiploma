using System;

namespace Service.Entidades
{
    public class BitacoraEvento_83KI
    {
        public int Id { get; private set; }
        public DateTime Fecha { get; private set; }
        public string Descripcion { get; private set; }
        public Criticidad Criticidad { get; private set; }
        public Modulo Modulo { get; private set; }    // "Usuario"
        public string Username { get; private set; }

        private BitacoraEvento_83KI()
        {
        }

        public static BitacoraEvento_83KI CrearNuevo(string descripcion, Criticidad criticidad, Modulo modulo, string username)
        {
            return new BitacoraEvento_83KI
            {
                Fecha = DateTime.Now,
                Descripcion = ValidarTextoObligatorio(descripcion, nameof(descripcion)),
                Criticidad = ValidarCriticidad(criticidad),
                Modulo = ValidarModulo(modulo),
                Username = ValidarTextoObligatorio(username, nameof(username))
            };
        }

        public static BitacoraEvento_83KI ReconstruirDesdePersistencia(
            int id,
            DateTime fecha,
            string descripcion,
            Criticidad criticidad,
            Modulo modulo,
            string username)
        {
            if (id < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "El id de bitacora no puede ser negativo.");
            }

            return new BitacoraEvento_83KI
            {
                Id = id,
                Fecha = fecha,
                Descripcion = ValidarTextoObligatorio(descripcion, nameof(descripcion)),
                Criticidad = ValidarCriticidad(criticidad),
                Modulo = ValidarModulo(modulo),
                Username = ValidarTextoObligatorio(username, nameof(username))
            };
        }

        private static string ValidarTextoObligatorio(string valor, string nombreParametro)
        {
            if (string.IsNullOrWhiteSpace(valor))
            {
                throw new ArgumentException("El valor es obligatorio.", nombreParametro);
            }

            return valor.Trim();
        }

        private static Criticidad ValidarCriticidad(Criticidad criticidad)
        {
            if (!Enum.IsDefined(typeof(Criticidad), criticidad))
            {
                throw new ArgumentException("La criticidad no es valida.", nameof(criticidad));
            }

            return criticidad;
        }

        private static Modulo ValidarModulo(Modulo modulo)
        {
            if (!Enum.IsDefined(typeof(Modulo), modulo))
            {
                throw new ArgumentException("El modulo no es valido.", nameof(modulo));
            }

            return modulo;
        }
    }
}

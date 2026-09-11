using System;
using System.Collections.Generic;
using System.Linq;

namespace Service.Entidades
{
    public sealed class EventoBitacoraOpcion_83KI
    {
        private readonly string[] _prefijos;

        public EventoBitacoraOpcion_83KI(Modulo modulo, string nombre, params string[] prefijos)
        {
            Modulo = modulo;
            Nombre = nombre;
            _prefijos = prefijos == null || prefijos.Length == 0
                ? new[] { nombre }
                : prefijos;
        }

        public Modulo Modulo { get; private set; }
        public string Nombre { get; private set; }

        public bool CoincideConDescripcion(string descripcion)
        {
            if (string.IsNullOrWhiteSpace(descripcion))
            {
                return false;
            }

            string descripcionNormalizada = descripcion.Trim();
            return _prefijos.Any(prefijo => descripcionNormalizada.StartsWith(prefijo, StringComparison.OrdinalIgnoreCase));
        }

        public override string ToString()
        {
            return Nombre;
        }
    }

    public static class EventoBitacoraCatalogo_83KI
    {
        private static readonly List<EventoBitacoraOpcion_83KI> Eventos = new List<EventoBitacoraOpcion_83KI>
        {
            // --- Usuarios (5 eventos) ---
            new EventoBitacoraOpcion_83KI(Modulo.Usuarios, "Login exitoso", "Login exitoso:"),
            new EventoBitacoraOpcion_83KI(Modulo.Usuarios, "Logout exitoso", "Logout exitoso:"),
            new EventoBitacoraOpcion_83KI(Modulo.Usuarios, "Intento fallido de login", "Intento fallido de login:"),
            new EventoBitacoraOpcion_83KI(Modulo.Usuarios, "Usuario bloqueado por intentos fallidos", "Usuario bloqueado por intentos fallidos:"),
            new EventoBitacoraOpcion_83KI(Modulo.Usuarios, "Contraseña modificada", "Contraseña modificada:", "ContraseÃ±a modificada:"),

            // --- Admin (19 eventos) ---
            new EventoBitacoraOpcion_83KI(Modulo.Admin, "Usuario bloqueado", "Usuario bloqueado:"),
            new EventoBitacoraOpcion_83KI(Modulo.Admin, "Usuario modificado", "Usuario modificado:"),
            new EventoBitacoraOpcion_83KI(Modulo.Admin, "Nuevo usuario creado", "Nuevo usuario creado:"),
            new EventoBitacoraOpcion_83KI(Modulo.Admin, "Usuario desbloqueado", "Usuario desbloqueado:"),
            new EventoBitacoraOpcion_83KI(Modulo.Admin, "Usuario habilitado", "Usuario habilitado:"),
            new EventoBitacoraOpcion_83KI(Modulo.Admin, "Usuario deshabilitado", "Usuario deshabilitado:"),
            new EventoBitacoraOpcion_83KI(Modulo.Admin, "Nuevo rol creado", "Nuevo rol creado:"),
            new EventoBitacoraOpcion_83KI(Modulo.Admin, "Rol eliminado", "Rol eliminado:"),
            new EventoBitacoraOpcion_83KI(Modulo.Admin, "Nueva familia creada", "Nueva familia creada:"),
            new EventoBitacoraOpcion_83KI(Modulo.Admin, "Familia eliminada", "Familia eliminada:"),
            new EventoBitacoraOpcion_83KI(Modulo.Admin, "Rol modificado",
                "Rol modificado:",
                "Patente asignada a rol:",
                "Patente quitada de rol:",
                "Familia asignada a rol:",
                "Familia quitada de rol:"),
            new EventoBitacoraOpcion_83KI(Modulo.Admin, "Familia modificada",
                "Familia modificada:",
                "Patente asignada a familia:",
                "Patente quitada de familia:",
                "Subfamilia asignada:",
                "Subfamilia quitada:"),
            new EventoBitacoraOpcion_83KI(Modulo.Admin, "Bit\u00e1cora exportada a PDF", "Bit\u00e1cora exportada a PDF:"),

            // --- Admin — Data Integrity & Recovery (4 events) ---
            new EventoBitacoraOpcion_83KI(Modulo.Admin, "Integridad de datos comprometida", "Integridad de datos comprometida:"),
            new EventoBitacoraOpcion_83KI(Modulo.Admin, "Rec\u00e1lculo de hashes ejecutado", "Rec\u00e1lculo de hashes ejecutado:"),
            new EventoBitacoraOpcion_83KI(Modulo.Admin, "Backup de base de datos ejecutado", "Backup de base de datos ejecutado:"),
            new EventoBitacoraOpcion_83KI(Modulo.Admin, "Restauraci\u00f3n de base de datos ejecutada", "Restauraci\u00f3n de base de datos ejecutada:")
        };

        public static IEnumerable<EventoBitacoraOpcion_83KI> ObtenerPorModulo(Modulo modulo)
        {
            return Eventos.Where(evento => evento.Modulo == modulo).ToList();
        }

        public static string ResolverNombre(string descripcion)
        {
            if (string.IsNullOrWhiteSpace(descripcion))
            {
                return null;
            }

            EventoBitacoraOpcion_83KI evento = Eventos.FirstOrDefault(e =>
                e.CoincideConDescripcion(descripcion));

            return evento != null ? evento.Nombre : null;
        }

        public static bool CoincideConEvento(string descripcion, string nombreEvento)
        {
            if (string.IsNullOrWhiteSpace(nombreEvento))
            {
                return true;
            }

            EventoBitacoraOpcion_83KI evento = Eventos.FirstOrDefault(e =>
                string.Equals(e.Nombre, nombreEvento.Trim(), StringComparison.OrdinalIgnoreCase));

            if (evento == null)
            {
                return (descripcion ?? string.Empty).IndexOf(nombreEvento.Trim(), StringComparison.OrdinalIgnoreCase) >= 0;
            }

            return evento.CoincideConDescripcion(descripcion);
        }
    }
}

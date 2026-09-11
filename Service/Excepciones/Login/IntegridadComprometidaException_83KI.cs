using System;
using System.Collections.Generic;

namespace Service.Excepciones
{
    /// <summary>
    /// lanzada durante el login cuando la verificacion de integridad detecta una discrepancia dvv
    /// y el usuario que se esta autenticando no es administrador.
    /// transporta los nombres de tablas afectadas para mostrarlos en el flujo de recuperacion del admin.
    /// </summary>
    public class IntegridadComprometidaException_83KI : Exception
    {
        public IReadOnlyList<string> TablasAfectadas { get; }

        public IntegridadComprometidaException_83KI(IEnumerable<string> tablasAfectadas = null)
            : base("La integridad de los datos esta comprometida. Contacte a un administrador.")
        {
            TablasAfectadas = tablasAfectadas != null
                ? new List<string>(tablasAfectadas).AsReadOnly()
                : new List<string>().AsReadOnly();
        }
    }
}

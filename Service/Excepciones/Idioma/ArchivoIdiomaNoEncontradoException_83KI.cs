using System;

namespace Service.Excepciones.Idioma
{
    public class ArchivoIdiomaNoEncontradoException_83KI : Exception
    {
        public ArchivoIdiomaNoEncontradoException_83KI(string archivo, string directorio)
            : base($"No se encontro el archivo de idioma requerido '{archivo}' en el directorio '{directorio}'.")
        {
        }
    }
}

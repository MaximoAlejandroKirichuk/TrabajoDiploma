using System;

namespace BE.Entidades
{
    public class Curso_83KI
    {
        public int IdCurso { get; private set; }
        public string Nombre { get; private set; }
        public string Descripcion { get; private set; }
        public int CargaHoraria { get; private set; }
        public bool EstadoActivo { get; private set; }
        public string DVH { get; private set; }

        private Curso_83KI() { }

        public static Curso_83KI CrearNuevo(string nombre, string descripcion, int cargaHoraria)
        {
            return new Curso_83KI
            {
                Nombre = ValidarNombre(nombre),
                Descripcion = NormalizarDescripcion(descripcion),
                CargaHoraria = ValidarCargaHoraria(cargaHoraria),
                EstadoActivo = true,
                DVH = string.Empty
            };
        }

        public static Curso_83KI ReconstruirDesdePersistencia(int idCurso, string nombre, string descripcion, int cargaHoraria, bool estadoActivo, string dvh)
        {
            var curso = CrearNuevo(nombre, descripcion, cargaHoraria);
            curso.IdCurso = idCurso;
            curso.EstadoActivo = estadoActivo;
            curso.DVH = dvh ?? string.Empty;
            return curso;
        }

        public void Modificar(string nombre, string descripcion, int cargaHoraria)
        {
            Nombre = ValidarNombre(nombre);
            Descripcion = NormalizarDescripcion(descripcion);
            CargaHoraria = ValidarCargaHoraria(cargaHoraria);
        }

        public void Activar() { EstadoActivo = true; }
        public void Desactivar() { EstadoActivo = false; }

        private static string ValidarNombre(string nombre)
        {
            return ValidarTextoObligatorio(nombre, "Errores.NombreObligatorio");
        }

        private static string ValidarTextoObligatorio(string valor, string claveError)
        {
            if (string.IsNullOrWhiteSpace(valor))
                throw new ArgumentException(claveError);
            return valor.Trim();
        }

        private static string NormalizarDescripcion(string descripcion)
        {
            return string.IsNullOrWhiteSpace(descripcion) ? string.Empty : descripcion.Trim();
        }

        private static int ValidarCargaHoraria(int cargaHoraria)
        {
            if (cargaHoraria <= 0)
                throw new ArgumentException("Errores.CargaHorariaInvalida", nameof(cargaHoraria));
            return cargaHoraria;
        }
    }
}

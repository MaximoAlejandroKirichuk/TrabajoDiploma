using System;

namespace BE.Entidades
{
    public class CursoProfesor_83KI
    {
        public int IdCurso { get; private set; }
        public int IdProfesor { get; private set; }
        public bool EstadoActivo { get; private set; }
        public string DVH { get; private set; }
        public string CursoNombre { get; private set; }
        public string ProfesorNombre { get; private set; }
        public bool CursoActivo { get; private set; }
        public bool ProfesorActivo { get; private set; }
        public bool HabilitadoEfectivo { get { return EstadoActivo && CursoActivo && ProfesorActivo; } }

        private CursoProfesor_83KI() { }

        public static CursoProfesor_83KI CrearNueva(int idCurso, int idProfesor)
        {
            return new CursoProfesor_83KI
            {
                IdCurso = ValidarId(idCurso, nameof(idCurso)),
                IdProfesor = ValidarId(idProfesor, nameof(idProfesor)),
                EstadoActivo = true,
                DVH = string.Empty,
                CursoNombre = string.Empty,
                ProfesorNombre = string.Empty,
                CursoActivo = true,
                ProfesorActivo = true
            };
        }

        public static CursoProfesor_83KI ReconstruirDesdePersistencia(int idCurso, int idProfesor, bool estadoActivo, string dvh, string cursoNombre, string profesorNombre, bool cursoActivo, bool profesorActivo)
        {
            var relacion = CrearNueva(idCurso, idProfesor);
            relacion.EstadoActivo = estadoActivo;
            relacion.DVH = dvh ?? string.Empty;
            relacion.CursoNombre = cursoNombre ?? string.Empty;
            relacion.ProfesorNombre = profesorNombre ?? string.Empty;
            relacion.CursoActivo = cursoActivo;
            relacion.ProfesorActivo = profesorActivo;
            return relacion;
        }

        public void Activar() { EstadoActivo = true; }
        public void Desactivar() { EstadoActivo = false; }

        private static int ValidarId(int id, string parametro)
        {
            if (id <= 0)
                throw new ArgumentException("Errores.IdentificadorInvalido|" + parametro, parametro);
            return id;
        }
    }
}

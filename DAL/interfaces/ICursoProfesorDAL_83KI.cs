using BE.Entidades;
using System.Collections.Generic;

namespace DAL.interfaces
{
    public interface ICursoProfesorDAL_83KI
    {
        IEnumerable<Curso_83KI> ObtenerCursosActivosParaRelacion();
        IEnumerable<Profesor_83KI> ObtenerProfesoresActivosParaRelacion();
        IEnumerable<CursoProfesor_83KI> ObtenerRelaciones();
        IEnumerable<CursoProfesor_83KI> ObtenerProfesoresHabilitadosParaCurso(int idCurso);
        CursoProfesor_83KI ObtenerPorIds(int idCurso, int idProfesor);
        bool ExisteCursoActivo(int idCurso);
        bool ExisteProfesorActivo(int idProfesor);
        bool EstaProfesorHabilitadoParaCurso(int idCurso, int idProfesor);
        void CrearRelacion(CursoProfesor_83KI relacion);
        void ActualizarEstadoActivo(int idCurso, int idProfesor, bool activo);
    }
}

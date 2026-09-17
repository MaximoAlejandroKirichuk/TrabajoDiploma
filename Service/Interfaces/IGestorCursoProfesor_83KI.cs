using BE.Entidades;
using System.Collections.Generic;

namespace Service.Interfaces
{
    public interface IGestorCursoProfesor_83KI
    {
        IEnumerable<Curso_83KI> ObtenerCursosActivosParaRelacion();
        IEnumerable<Profesor_83KI> ObtenerProfesoresActivosParaRelacion();
        IEnumerable<CursoProfesor_83KI> ObtenerRelaciones();
        IEnumerable<CursoProfesor_83KI> ObtenerProfesoresHabilitadosParaCurso(int idCurso);
        CursoProfesor_83KI ObtenerPorIds(int idCurso, int idProfesor);
        bool EstaProfesorHabilitadoParaCurso(int idCurso, int idProfesor);
        void HabilitarProfesorParaCurso(int idCurso, int idProfesor);
        void DeshabilitarProfesorParaCurso(int idCurso, int idProfesor);
    }
}

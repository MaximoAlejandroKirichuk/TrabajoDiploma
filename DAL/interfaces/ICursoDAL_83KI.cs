using BE.Entidades;
using System.Collections.Generic;

namespace DAL.interfaces
{
    public interface ICursoDAL_83KI
    {
        IEnumerable<Curso_83KI> ObtenerTodos();
        IEnumerable<Curso_83KI> ObtenerActivos();
        Curso_83KI ObtenerPorId(int idCurso);
        bool ExisteNombreParaOtroCurso(string nombre, int idCurso);
        int Crear(Curso_83KI curso);
        void Modificar(Curso_83KI curso);
        void ActualizarEstadoActivo(int idCurso, bool activo);
    }
}

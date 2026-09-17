using BE.Entidades;
using System.Collections.Generic;

namespace Service.Interfaces
{
    public interface IGestorCurso_83KI
    {
        IEnumerable<Curso_83KI> ObtenerCursos();
        IEnumerable<Curso_83KI> ObtenerCursosActivos();
        Curso_83KI ObtenerPorId(int idCurso);
        void CrearCurso(string nombre, string descripcion, int cargaHoraria);
        void ModificarCurso(int idCurso, string nombre, string descripcion, int cargaHoraria);
        void ActivarCurso(int idCurso);
        void DesactivarCurso(int idCurso);
    }
}

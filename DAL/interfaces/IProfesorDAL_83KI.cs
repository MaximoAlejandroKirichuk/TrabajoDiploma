using BE.Entidades;
using System.Collections.Generic;

namespace DAL.interfaces
{
    public interface IProfesorDAL_83KI
    {
        IEnumerable<Profesor_83KI> ObtenerTodos();
        IEnumerable<Profesor_83KI> ObtenerActivos();
        Profesor_83KI ObtenerPorId(int idProfesor);
        bool ExisteDniParaOtroProfesor(string dni, int idProfesor);
        int Crear(Profesor_83KI profesor);
        void Modificar(Profesor_83KI profesor);
        void ActualizarEstadoActivo(int idProfesor, bool activo);
    }
}

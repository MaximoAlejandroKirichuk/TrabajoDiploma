using BE.Entidades;
using System.Collections.Generic;

namespace Service.Interfaces
{
    public interface IGestorProfesor_83KI
    {
        IEnumerable<Profesor_83KI> ObtenerProfesores();
        IEnumerable<Profesor_83KI> ObtenerProfesoresActivos();
        Profesor_83KI ObtenerPorId(int idProfesor);
        void CrearProfesor(string dni, string nombre, string apellido, string email);
        void ModificarProfesor(int idProfesor, string nombre, string apellido, string email);
        void ActivarProfesor(int idProfesor);
        void DesactivarProfesor(int idProfesor);
    }
}

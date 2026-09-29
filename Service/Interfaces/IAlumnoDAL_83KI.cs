using BE.Entidades;
using Service.DTOs;
using System.Collections.Generic;

namespace Service.Interfaces
{
    public interface IAlumnoDAL_83KI
    {
        Alumno_83KI ObtenerPorId(int idAlumno);
        Alumno_83KI ObtenerPorDni(string dni);
        IEnumerable<BusquedaInscripcionPersonaResultado_83KI> BuscarParaInscripcion(string texto);
    }
}

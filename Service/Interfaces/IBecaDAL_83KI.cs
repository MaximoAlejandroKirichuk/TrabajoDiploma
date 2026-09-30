using BE.Entidades;
using Service.DTOs;

namespace Service.Interfaces
{
    public interface IBecaDAL_83KI
    {
        BusquedaDecisionBecaResultado_83KI BuscarSolicitudPorDniComision(string dni, string codigoComision);
        bool ExisteBecaParaInscripcion(string dniAlumno, string codigoComision);
        Beca_83KI RegistrarBeca(Beca_83KI beca);
    }
}

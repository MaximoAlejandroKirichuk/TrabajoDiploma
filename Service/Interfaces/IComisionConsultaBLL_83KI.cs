using Service.DTOs;
using BE.DTOs.Comisiones;
using System.Collections.Generic;

namespace Service.Interfaces
{
    public interface IComisionConsultaBLL_83KI
    {
        IEnumerable<ComisionListado_83KI> ListarComisionesPreapertura();
        void ValidarComisionPreapertura(int idComision);
    }
}

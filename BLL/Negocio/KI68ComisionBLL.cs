using BE.Entidades;
using Service.DTOs;
using Service.Interfaces;
using System;
using System.Collections.Generic;

namespace BLL
{
    public class KI68ComisionBLL : IComisionConsultaBLL_83KI
    {
        private readonly IComisionDAL_83KI _comisionDAL;

        public KI68ComisionBLL(IComisionDAL_83KI comisionDAL)
        {
            _comisionDAL = comisionDAL;
        }

        public IEnumerable<ComisionListado_83KI> ListarComisionesPreapertura()
        {
            return _comisionDAL.ListarComisionesPreapertura();
        }

        public void ValidarComisionPreapertura(int idComision)
        {
            if (idComision <= 0 || !_comisionDAL.ExisteComisionPreapertura(idComision))
                throw new InvalidOperationException("Errores.ComisionPreaperturaNoDisponible");
        }
    }
}

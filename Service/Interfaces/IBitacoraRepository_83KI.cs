using BE.Entidades;
using Service.Entidades;
using System;
using System.Collections.Generic;

namespace Service.Interfaces
{
    public interface IBitacoraRepository_83KI
    {
        void Registrar(BitacoraEvento_83KI evento);
        IEnumerable<BitacoraEvento_83KI> Consultar(DateTime desde, DateTime hasta);
        IEnumerable<BitacoraEvento_83KI> ConsultarPorModulo(DateTime desde, DateTime hasta, Modulo modulo);
        IEnumerable<BitacoraEvento_83KI> ConsultarPorCriticidad(DateTime desde, DateTime hasta, Criticidad criticidad);
        IEnumerable<BitacoraEvento_83KI> ConsultarPorModuloYCriticidad(DateTime desde, DateTime hasta, Modulo modulo, Criticidad criticidad);
    }
}

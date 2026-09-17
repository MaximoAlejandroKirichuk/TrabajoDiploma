using System;
using System.Collections.Generic;
using BE.Entidades;
using Service.Entidades;
namespace DAL.interfaces
{
    public interface IBitacoraDAL_83KI
    {
        void Registrar(BitacoraEvento_83KI evento);

        // Para la interfaz de Auditoría con el filtro de 3 días
        IEnumerable<BitacoraEvento_83KI> Consultar(DateTime desde, DateTime hasta);
        IEnumerable<BitacoraEvento_83KI> ConsultarPorModulo(DateTime desde, DateTime hasta, Modulo modulo);
        IEnumerable<BitacoraEvento_83KI> ConsultarPorCriticidad(DateTime desde, DateTime hasta, Criticidad criticidad);
        IEnumerable<BitacoraEvento_83KI> ConsultarPorModuloYCriticidad(DateTime desde, DateTime hasta, Modulo modulo, Criticidad criticidad);
    }
}

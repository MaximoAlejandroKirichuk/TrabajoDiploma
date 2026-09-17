using BLL.Interfaces;
using BE.Entidades;
using Service.Entidades;
using Service.Interfaces;
using System;
using System.Collections.Generic;

namespace Service
{
    public class BitacoraService_83KI : IBitacoraManager_83KI
    {
        private readonly IBitacoraRepository_83KI _bitacoraRepository;

        public BitacoraService_83KI(IBitacoraRepository_83KI bitacoraRepository)
        {
            _bitacoraRepository = bitacoraRepository;
        }

        public IEnumerable<BitacoraEvento_83KI> ListarEventos(DateTime desde, DateTime hasta)
        {
            return _bitacoraRepository.Consultar(desde, hasta);
        }

        public void RegistrarEvento(BitacoraEvento_83KI evento)
        {
            _bitacoraRepository.Registrar(evento);
        }
    }
}

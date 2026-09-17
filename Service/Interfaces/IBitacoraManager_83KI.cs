using BE.Entidades;
using Service.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IBitacoraManager_83KI
    {
        void RegistrarEvento(BitacoraEvento_83KI evento);

        // metodo de consulta en GUI (últimos 3 días por defecto)
        IEnumerable<BitacoraEvento_83KI> ListarEventos(DateTime desde, DateTime hasta);
    }
}

using BE.Entidades;
using Service.Entidades;
using System;

namespace Service.DTOs
{
    public class FiltroBitacoraEventos_83KI
    {
        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Username { get; set; }
        public string Evento { get; set; }
        public Modulo? Modulo { get; set; }
        public Criticidad? Criticidad { get; set; }
    }
}

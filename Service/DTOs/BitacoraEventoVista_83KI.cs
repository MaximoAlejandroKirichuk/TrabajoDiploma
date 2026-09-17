using BE.Entidades;
using Service.Entidades;
using System;

namespace Service.DTOs
{
    public class BitacoraEventoVista_83KI
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public string Username { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public Modulo Modulo { get; set; }
        public Criticidad Criticidad { get; set; }
        public string Evento { get; set; }
    }
}

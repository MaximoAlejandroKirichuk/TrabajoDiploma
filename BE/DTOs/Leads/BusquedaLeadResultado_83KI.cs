using BE.Entidades;
using System.Collections.Generic;

namespace Service.DTOs
{
    public class BusquedaLeadResultado_83KI
    {
        public Lead_83KI Lead { get; set; }
        public List<Lead_83KI> Coincidencias { get; set; }
        public bool EsAmbigua { get { return Coincidencias != null && Coincidencias.Count > 1; } }
        public bool TieneUnico { get { return Lead != null; } }

        public BusquedaLeadResultado_83KI()
        {
            Coincidencias = new List<Lead_83KI>();
        }
    }
}

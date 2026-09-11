using Service.Interfaces;
using System.Collections.Generic;

namespace Service.Entidades
{
    public class Idioma_83KI : IIdioma
    {
        public string Nombre { get; set; }
        public string Id { get; set; }
        public Dictionary<string, string> Traducciones { get; set; }

        public Idioma_83KI()
        {
            Traducciones = new Dictionary<string, string>();
        }
    }
}

using Service.Entidades;
using Service.Excepciones.Idioma;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Script.Serialization;

namespace Service
{
    public class GestorIdioma_83KI : IGestorIdioma_83KI
    {
        public const string IdiomaPorDefecto = "es-AR";
        private static readonly string[] ArchivosRequeridos = { "es-AR.json", "en-US.json" };

        private readonly List<IObservadorIdioma> _observadores = new List<IObservadorIdioma>();
        //significa: voy a guardar varios idiomas en una lista tipo diccionario.
        //guarda los idiomas usando su Id como clave.
        private readonly Dictionary<string, Idioma_83KI> _idiomas = new Dictionary<string, Idioma_83KI>(StringComparer.OrdinalIgnoreCase);

        public IIdioma IdiomaActual { get; private set; }

        public GestorIdioma_83KI()
        {
            CargarIdiomas();
            CambiarIdioma(IdiomaPorDefecto);
        }

        public IEnumerable<IIdioma> ObtenerIdiomas()
        {
            return _idiomas.Values.OrderBy(i => i.Nombre).ToList();
        }

        public void CambiarIdioma(string idiomaId)
        {
            string idNormalizado = string.IsNullOrWhiteSpace(idiomaId) ? IdiomaPorDefecto : idiomaId.Trim();

            //Busca ese idioma dentro del diccionario:
            if (!_idiomas.TryGetValue(idNormalizado, out Idioma_83KI idioma))
            {
                //Si no lo encuentra usa el por defecto:
                idioma = _idiomas[IdiomaPorDefecto];
            }

            //Avisa a las pantallas que cambió el idioma:
            IdiomaActual = idioma;
            Notificar();
        }


        
        public string ObtenerTexto(string clave)
        {
            //Este método busca una traducción en el idioma actual.
           
            if (string.IsNullOrWhiteSpace(clave))
            {
                return string.Empty;
            }
            //Convierte el idioma actual, que está guardado como IIdioma,
            //al tipo real Idioma, porque Idioma_83KI tiene el diccionario Traducciones.
            Idioma_83KI idioma = IdiomaActual as Idioma_83KI;
            
            if (idioma != null &&
                idioma.Traducciones != null &&
                idioma.Traducciones.TryGetValue(clave, out string texto))
            {
                return texto;
            }

            return clave;
        }

        public void Suscribir(IObservadorIdioma observador)
        {
            if (observador == null || _observadores.Contains(observador))
            {
                return;
            }

            _observadores.Add(observador);
            observador.ActualizarIdioma(IdiomaActual);
        }

        public void Desuscribir(IObservadorIdioma observador)
        {
            if (observador == null)
            {
                return;
            }

            _observadores.Remove(observador);
        }

        public void Notificar()
        {
            foreach (IObservadorIdioma observador in _observadores.ToList())
            {
                observador.ActualizarIdioma(IdiomaActual);
            }
        }

        private void CargarIdiomas()
        {
            string directorioIdiomas = ObtenerDirectorioIdiomas();

            if (!Directory.Exists(directorioIdiomas))
            {
                throw new DirectoryNotFoundException($"No se encontro el directorio de idiomas '{directorioIdiomas}'.");
            }

            ValidarArchivosRequeridos(directorioIdiomas);

            JavaScriptSerializer serializer = new JavaScriptSerializer();

            foreach (string archivo in Directory.GetFiles(directorioIdiomas, "*.json"))
            {
                string contenido = File.ReadAllText(archivo);
                Idioma_83KI idioma = serializer.Deserialize<Idioma_83KI>(contenido);

                if (idioma != null && !string.IsNullOrWhiteSpace(idioma.Id))
                {
                    _idiomas[idioma.Id] = idioma;
                }
            }

            if (!_idiomas.ContainsKey(IdiomaPorDefecto))
            {
                throw new ArchivoIdiomaNoEncontradoException_83KI($"{IdiomaPorDefecto}.json", directorioIdiomas);
            }
        }

        private void ValidarArchivosRequeridos(string directorioIdiomas)
        {
            foreach (string archivoRequerido in ArchivosRequeridos)
            {
                string rutaArchivo = Path.Combine(directorioIdiomas, archivoRequerido);

                if (!File.Exists(rutaArchivo))
                {
                    throw new ArchivoIdiomaNoEncontradoException_83KI(archivoRequerido, directorioIdiomas);
                }
            }
        }

        private string ObtenerDirectorioIdiomas()
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string directorioSalida = Path.Combine(baseDirectory, "Idiomas");

            if (Directory.Exists(directorioSalida))
            {
                return directorioSalida;
            }

            return Path.GetFullPath(Path.Combine(baseDirectory, @"..\..\..\Service\Idiomas"));
        }

        private Idioma_83KI CrearIdiomaPorDefecto()
        {
            return new Idioma_83KI
            {
                Id = IdiomaPorDefecto,
                Nombre = "Español"
            };
        }
    }
}

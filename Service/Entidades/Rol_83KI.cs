using System;
using System.Collections.Generic;
using System.Linq;

namespace Service.Entidades
{
    public class Rol_83KI
    {
        private const int CodigoAdministrador = 1;
        private readonly List<Patente_83KI> _patentesDirectas = new List<Patente_83KI>();
        private readonly List<Familia_83KI> _familias = new List<Familia_83KI>();

        public int CodigoRol { get; private set; }
        public string Nombre { get; private set; }
        public IEnumerable<Patente_83KI> PatentesDirectas => _patentesDirectas.AsReadOnly();
        public IEnumerable<Familia_83KI> Familias => _familias.AsReadOnly();

        //Campos Calculados
        //devuelve true si es que el administrador tiene un CodigoRol igual a CodigoAdmnistrador 
        public bool EsAdministrador => CodigoRol == CodigoAdministrador;
        //devuelve true si es que puede gestionar los formularios admin
        public bool PuedeGestionarAdmin => EsAdministrador;

        public Rol_83KI(int codigoRol, string nombre)
        {
            if (codigoRol <= 0)
            {
                throw new ArgumentException("El codigo debe ser valido.", nameof(codigoRol));
            }

            if (string.IsNullOrWhiteSpace(nombre))
            {
                throw new ArgumentException("El nombre es obligatorio.", nameof(nombre));
            }

            CodigoRol = codigoRol;
            Nombre = nombre.Trim();
        }

        public void AgregarPatente(Patente_83KI patente)
        {
            if (patente == null)
            {
                throw new ArgumentNullException(nameof(patente));
            }

            if (_patentesDirectas.Any(p => p.CodigoPatente == patente.CodigoPatente))
            {
                throw new InvalidOperationException("La patente ya se encuentra asignada.");
            }

            if (ObtenerPatentes().Any(p => p.CodigoPatente == patente.CodigoPatente))
            {
                throw new InvalidOperationException("La asignacion duplicaria permisos indirectos.");
            }

            _patentesDirectas.Add(patente);
        }

        public void AgregarFamilia(Familia_83KI familia)
        {
            if (familia == null)
            {
                throw new ArgumentNullException(nameof(familia));
            }

            if (_familias.Any(f => f.CodigoFamilia == familia.CodigoFamilia))
            {
                throw new InvalidOperationException("La familia ya se encuentra asignada.");
            }

            HashSet<int> patentesActuales = new HashSet<int>(ObtenerPatentes().Select(p => p.CodigoPatente));
            if (familia.ObtenerPatentes().Any(p => patentesActuales.Contains(p.CodigoPatente)))
            {
                throw new InvalidOperationException("La asignacion duplicaria permisos indirectos.");
            }

            _familias.Add(familia);
        }

        public void CargarPatenteDesdePersistencia(Patente_83KI patente)
        {
            if (patente == null)
            {
                return;
            }

            if (!_patentesDirectas.Any(p => p.CodigoPatente == patente.CodigoPatente))
            {
                _patentesDirectas.Add(patente);
            }
        }

        public void CargarFamiliaDesdePersistencia(Familia_83KI familia)
        {
            if (familia == null)
            {
                return;
            }

            if (!_familias.Any(f => f.CodigoFamilia == familia.CodigoFamilia))
            {
                _familias.Add(familia);
            }
        }

        public IEnumerable<Patente_83KI> ObtenerPatentes()
        {
            List<Patente_83KI> patentes = new List<Patente_83KI>();
            HashSet<int> codigosAgregados = new HashSet<int>();

            foreach (Patente_83KI patenteDirecta in _patentesDirectas)
            {
                if (codigosAgregados.Add(patenteDirecta.CodigoPatente))
                {
                    patentes.Add(patenteDirecta);
                }
            }

            foreach (Familia_83KI familia in _familias)
            {
                foreach (Patente_83KI patente in familia.ObtenerPatentes())
                {
                    if (codigosAgregados.Add(patente.CodigoPatente))
                    {
                        patentes.Add(patente);
                    }
                }
            }

            return patentes;
        }

        public override string ToString()
        {
            return Nombre;
        }
    }
}

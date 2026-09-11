using System;
using System.Collections.Generic;
using System.Linq;

namespace Service.Entidades
{
    public class Familia_83KI : ComponentePermiso_83KI
    {
        private readonly List<ComponentePermiso_83KI> _hijos = new List<ComponentePermiso_83KI>();

        public int CodigoFamilia => Codigo;
        public IEnumerable<ComponentePermiso_83KI> Hijos => _hijos.AsReadOnly();

        public Familia_83KI(int codigoFamilia, string nombre)
            : base(codigoFamilia, nombre)
        {
        }

        public void Agregar(ComponentePermiso_83KI componente)
        {
            if (componente == null)
            {
                throw new ArgumentNullException(nameof(componente));
            }

            if (ReferenceEquals(this, componente) || componente.Contiene(this))
            {
                throw new InvalidOperationException("No se puede asignar una familia a si misma ni generar ciclos.");
            }

            if (_hijos.Any(h => h.GetType() == componente.GetType() && h.Codigo == componente.Codigo))
            {
                throw new InvalidOperationException("El componente ya se encuentra asignado.");
            }

            HashSet<int> patentesActuales = new HashSet<int>(ObtenerPatentes().Select(p => p.CodigoPatente));
            if (componente.ObtenerPatentes().Any(p => patentesActuales.Contains(p.CodigoPatente)))
            {
                throw new InvalidOperationException("La asignacion duplicaria permisos indirectos.");
            }

            _hijos.Add(componente);
        }

        public void Remover(ComponentePermiso_83KI componente)
        {
            if (componente == null)
            {
                return;
            }

            ComponentePermiso_83KI existente = _hijos.FirstOrDefault(h => h.GetType() == componente.GetType() && h.Codigo == componente.Codigo);
            if (existente != null)
            {
                _hijos.Remove(existente);
            }
        }

        public void CargarHijoDesdePersistencia(ComponentePermiso_83KI componente)
        {
            if (componente == null)
            {
                return;
            }

            bool yaExiste = _hijos.Any(h => h.GetType() == componente.GetType() && h.Codigo == componente.Codigo);
            if (!yaExiste)
            {
                _hijos.Add(componente);
            }
        }

        public override IEnumerable<Patente_83KI> ObtenerPatentes()
        {
            List<Patente_83KI> patentes = new List<Patente_83KI>();
            HashSet<int> codigosAgregados = new HashSet<int>();

            foreach (ComponentePermiso_83KI componente in Hijos)
            {
                foreach (Patente_83KI patente in componente.ObtenerPatentes())
                {
                    if (codigosAgregados.Add(patente.CodigoPatente))
                    {
                        patentes.Add(patente);
                    }
                }
            }

            return patentes;
        }

        protected override IEnumerable<ComponentePermiso_83KI> ObtenerComponentesInternos()
        {
            return _hijos;
        }
    }
}

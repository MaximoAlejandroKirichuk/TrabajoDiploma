using System;
using System.Collections.Generic;

namespace Service.Entidades
{
    public abstract class ComponentePermiso_83KI
    {
        public int Codigo { get; private set; }
        public string Nombre { get; private set; }

        protected ComponentePermiso_83KI(int codigo, string nombre)
        {
            if (codigo <= 0)
            {
                throw new ArgumentException("El codigo debe ser valido.", nameof(codigo));
            }

            if (string.IsNullOrWhiteSpace(nombre))
            {
                throw new ArgumentException("El nombre es obligatorio.", nameof(nombre));
            }

            Codigo = codigo;
            Nombre = nombre.Trim();
        }

        public virtual bool Contiene(ComponentePermiso_83KI componente)
        {
            if (componente == null)
            {
                return false;
            }

            if (GetType() == componente.GetType() && Codigo == componente.Codigo)
            {
                return true;
            }

            foreach (ComponentePermiso_83KI hijo in ObtenerComponentesInternos())
            {
                if (hijo.Contiene(componente))
                {
                    return true;
                }
            }

            return false;
        }

        protected virtual IEnumerable<ComponentePermiso_83KI> ObtenerComponentesInternos()
        {
            yield break;
        }

        public abstract IEnumerable<Patente_83KI> ObtenerPatentes();

        public override string ToString()
        {
            return Nombre;
        }
    }
}

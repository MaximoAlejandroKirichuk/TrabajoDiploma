using System;

namespace BE.Entidades
{
    public class PlanDePago_83KI
    {
        public int IdPlanDePago { get; private set; }
        public string Nombre { get; private set; }
        public bool EstadoActivo { get; private set; }
        public decimal RecargoPorcentaje { get; private set; }

        public string NombreConRecargo
        {
            get { return string.Format("{0} ({1:N2}%)", Nombre, RecargoPorcentaje); }
        }

        private PlanDePago_83KI() { }

        public static PlanDePago_83KI ReconstruirDesdePersistencia(int idPlanDePago, string nombre, bool estadoActivo, decimal recargoPorcentaje)
        {
            if (idPlanDePago <= 0) throw new ArgumentException("Errores.PlanDePagoObligatorio", nameof(idPlanDePago));
            if (string.IsNullOrWhiteSpace(nombre)) throw new ArgumentException("Errores.NombreObligatorio", nameof(nombre));
            if (recargoPorcentaje < 0) throw new ArgumentException("Errores.RecargoPlanInvalido", nameof(recargoPorcentaje));

            return new PlanDePago_83KI
            {
                IdPlanDePago = idPlanDePago,
                Nombre = nombre.Trim(),
                EstadoActivo = estadoActivo,
                RecargoPorcentaje = recargoPorcentaje
            };
        }
    }
}

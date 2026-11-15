using System;
using System.Collections.Generic;

namespace BE.DTOs.Reportes
{
    public sealed class FiltroReporteInteligente_83KI
    {
        public DateTime? Desde { get; set; }
        public DateTime? Hasta { get; set; }
        public int? IdCurso { get; set; }
    }

    public sealed class CandidatoContinuidad_83KI
    {
        public int IdAlumno { get; set; }
        public int IdCursoRecomendado { get; set; }
        public string Alumno { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public string CursoAprobado { get; set; }
        public string CursoRecomendado { get; set; }
        public decimal DeudaVencida { get; set; }
        public decimal SaldoFuturo { get; set; }
        public int DiasAtraso { get; set; }
        public bool Elegible { get { return DeudaVencida == 0; } }
        public string Motivo { get { return Elegible ? "Inteligente.MotivoElegible" : "Inteligente.MotivoCondicionado"; } }
    }

    public sealed class DemandaCurso_83KI
    {
        public int IdCurso { get; set; }
        public string Curso { get; set; }
        public int Registradas { get; set; }
        public int Concretadas { get; set; }
        public int Canceladas { get; set; }
        public int Anuladas { get; set; }
        public int LeadsUnicos { get; set; }
        public int Consultas { get; set; }
    }

    public sealed class OfertaCurso_83KI
    {
        public int IdCurso { get; set; }
        public string Curso { get; set; }
        public string Comision { get; set; }
        public string Estado { get; set; }
        public bool Disponible { get; set; }
        public int? CupoMinimo { get; set; }
        public int CupoMaximo { get; set; }
        public int Ocupadas { get; set; }
        public int Regularizadas { get; set; }
        public int Elegibles { get; set; }
        public int Condicionados { get; set; }
        public int Vacantes { get { return Math.Max(0, CupoMaximo - Ocupadas); } }
        public int? Brecha { get { return Estado == "confirmada" ? null : CupoMinimo.HasValue ? Math.Max(0, CupoMinimo.Value - Regularizadas) : (int?)null; } }
        public int? BrechaConElegibles { get { return Brecha.HasValue ? Math.Max(0, Brecha.Value - (Disponible ? Math.Min(Elegibles, Vacantes) : Elegibles)) : (int?)null; } }
    }

    public sealed class CumplimientoCurso_83KI
    {
        public int IdCurso { get; set; }
        public string Curso { get; set; }
        public decimal DeudaVencida { get; set; }
        public decimal DeudaTotal { get; set; }
        public int CuotasVencidas { get; set; }
        public int DiasAtrasoMaximo { get; set; }
        public int Pagadas { get; set; }
        public int PagadasEnTermino { get; set; }
        public decimal? CumplimientoPorcentaje { get { return Pagadas > 0 ? 100m * PagadasEnTermino / Pagadas : (decimal?)null; } }
    }

    public sealed class ReporteInteligente_83KI
    {
        public DateTime FechaCenso { get; set; }
        public List<DemandaCurso_83KI> Demanda { get; set; } = new List<DemandaCurso_83KI>();
        public List<OfertaCurso_83KI> OcupacionHistorica { get; set; } = new List<OfertaCurso_83KI>();
        public List<OfertaCurso_83KI> OfertaActual { get; set; } = new List<OfertaCurso_83KI>();
        public List<CandidatoContinuidad_83KI> Candidatos { get; set; } = new List<CandidatoContinuidad_83KI>();
        public List<CumplimientoCurso_83KI> Cumplimiento { get; set; } = new List<CumplimientoCurso_83KI>();
    }
}

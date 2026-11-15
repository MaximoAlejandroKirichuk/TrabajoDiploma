using System;
using System.Collections.Generic;
using System.Linq;
using BE.DTOs.Reportes;
using BE.Entidades;
using Service.Entidades;
using Service.Interfaces;

namespace BLL
{
    public sealed class KI68ReporteInteligenteBLL : IReporteInteligenteBLL_83KI
    {
        private readonly IReporteInteligenteDAL_83KI _dal;
        private readonly ISessionManager_83KI _sesion;
        public KI68ReporteInteligenteBLL(IReporteInteligenteDAL_83KI dal, ISessionManager_83KI sesion)
        { _dal = dal; _sesion = sesion; }

        private void Autorizar()
        {
            if (!_sesion.TienePermiso(PermisoSistema_83KI.VerReporteInteligente))
                throw new InvalidOperationException("Errores.SinPermisos");
        }
        public List<Curso_83KI> Cursos() { Autorizar(); return _dal.Cursos(); }
        public ReporteInteligente_83KI Generar(FiltroReporteInteligente_83KI filtro)
        {
            Autorizar();
            filtro = filtro ?? new FiltroReporteInteligente_83KI();
            if (filtro.Desde.HasValue && filtro.Hasta.HasValue && filtro.Desde.Value.Date > filtro.Hasta.Value.Date)
                throw new ArgumentException("Inteligente.FechasInvalidas");
            if (filtro.IdCurso.HasValue && filtro.IdCurso <= 0) throw new ArgumentException("Inteligente.RecorridoInvalido");
            var reporte = _dal.Generar(filtro);
            CompletarProyeccion(reporte);
            return reporte;
        }
        public static void CompletarProyeccion(ReporteInteligente_83KI reporte)
        {
            foreach (var oferta in reporte.OfertaActual)
            {
                var candidatos = reporte.Candidatos.Where(c => c.IdCursoRecomendado == oferta.IdCurso).ToList();
                oferta.Elegibles = candidatos.Count(c => c.Elegible);
                oferta.Condicionados = candidatos.Count(c => !c.Elegible);
            }
        }
    }
}

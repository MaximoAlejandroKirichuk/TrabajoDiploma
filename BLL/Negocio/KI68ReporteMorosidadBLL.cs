using BE.Entidades;
using BE.DTOs.Pagos;
using Service.DTOs;
using BE.DTOs.Comisiones;
using Service.Entidades;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BLL
{
    // RF2 - Informe de morosidad por alumno y comisión (actor: Asistente Administrativo).
    // Las reglas son métodos estáticos puros: reciben los datos y la fecha de hoy, y no tocan la base.
    public class KI68ReporteMorosidadBLL : IReporteMorosidadBLL_83KI
    {
        public const string Rango1a30 = "1-30";
        public const string Rango31a60 = "31-60";
        public const string RangoMasDe60 = ">60";

        // Inscripción "activa" = vigente: los mismos estados que CUN03/CUN07 consideran ocupando vacante o con plan de pagos.
        // La matrícula impaga deja la inscripción en "pendiente", por eso se incluye.
        private static readonly string[] EstadosInscripcionVigentes =
        {
            SolicitudInscripcion_83KI.EstadoPendiente,
            SolicitudInscripcion_83KI.EstadoConfirmada,
            SolicitudInscripcion_83KI.EstadoActiva
        };

        // Una comisión eliminada o cancelada por falta de quórum no genera deuda a cobrar.
        private static readonly string[] EstadosComisionExcluidos =
        {
            Comision_83KI.EstadoEliminada,
            Comision_83KI.EstadoCanceladaFaltaQuorum
        };

        private readonly IReporteMorosidadDAL_83KI _dal;
        private readonly ISessionManager_83KI _sessionManager;

        public KI68ReporteMorosidadBLL(IReporteMorosidadDAL_83KI dal, ISessionManager_83KI sessionManager)
        {
            _dal = dal;
            _sessionManager = sessionManager;
        }

        public List<ReporteMorosidadFila_83KI> Generar(FiltroReporteMorosidad_83KI filtro)
        {
            ValidarPermiso();
            if (filtro == null) filtro = new FiltroReporteMorosidad_83KI();
            ValidarFiltro(filtro);

            List<ReporteMorosidadFila_83KI> filasLeidas;
            try
            {
                filasLeidas = _dal.ObtenerInscripcionesConSaldo(filtro);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Errores.ReporteMorosidadNoDisponible", ex);
            }

            return ArmarReporte(filasLeidas, filtro, DateTime.Today);
        }

        public List<ComisionListado_83KI> ListarComisionesParaFiltro()
        {
            ValidarPermiso();
            try
            {
                return _dal.ListarComisionesParaFiltro().Where(c => EsComisionVigente(c.Estado)).ToList();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Errores.ReporteMorosidadNoDisponible", ex);
            }
        }

        // ------------------------------------------------------------------
        // Reglas del reporte (métodos puros)
        // ------------------------------------------------------------------

        // Arma el reporte: deja solo inscripciones vigentes y morosas que cumplen los mínimos, y las ordena.
        public static List<ReporteMorosidadFila_83KI> ArmarReporte(IEnumerable<ReporteMorosidadFila_83KI> filas, FiltroReporteMorosidad_83KI filtro, DateTime hoy)
        {
            var resultado = new List<ReporteMorosidadFila_83KI>();
            if (filas == null) return resultado;

            foreach (var fila in filas)
            {
                if (fila == null || !EsInscripcionVigente(fila.EstadoInscripcion, fila.EstadoComision)) continue;

                CalcularDeuda(fila, hoy);
                if (EsMorosa(fila) && CumpleMinimos(fila, filtro))
                {
                    resultado.Add(fila);
                }
            }

            return Ordenar(resultado);
        }

        public static bool EsComisionVigente(string estadoComision)
        {
            return !EstadosComisionExcluidos.Any(e => string.Equals(e, (estadoComision ?? string.Empty).Trim(), StringComparison.OrdinalIgnoreCase));
        }

        public static bool EsInscripcionVigente(string estadoInscripcion, string estadoComision)
        {
            string estado = (estadoInscripcion ?? string.Empty).Trim();
            bool inscripcionVigente = EstadosInscripcionVigentes.Any(e => string.Equals(e, estado, StringComparison.OrdinalIgnoreCase));
            return inscripcionVigente && EsComisionVigente(estadoComision);
        }

        // Cuota vencida impaga = saldo mayor a cero y vencimiento anterior a hoy (incluye la matrícula).
        public static bool EsCuotaVencidaImpaga(Cuota_83KI cuota, DateTime hoy)
        {
            return cuota != null && cuota.BalanceAdeudado > 0 && cuota.FechaVencimiento.Date < hoy.Date;
        }

        // El detalle conserva las cuotas reales, no reconstruye conceptos a partir de los totales.
        public static List<Cuota_83KI> ObtenerDetalleDeuda(ReporteMorosidadFila_83KI fila)
        {
            if (fila == null) throw new ArgumentNullException(nameof(fila));
            return (fila.CuotasConSaldo ?? new List<Cuota_83KI>())
                .Where(c => c != null && c.BalanceAdeudado > 0)
                .OrderBy(c => c.FechaVencimiento)
                .ThenBy(c => c.NumeroCuota)
                .ThenBy(c => c.IdCuota)
                .ToList();
        }

        // Calcula las columnas de deuda de una inscripción a partir de sus cuotas.
        public static void CalcularDeuda(ReporteMorosidadFila_83KI fila, DateTime hoy)
        {
            if (fila == null) throw new ArgumentNullException(nameof(fila));

            var cuotas = fila.CuotasConSaldo ?? new List<Cuota_83KI>();
            var vencidas = cuotas.Where(c => EsCuotaVencidaImpaga(c, hoy)).ToList();

            fila.CantidadCuotasVencidas = vencidas.Count;
            fila.DeudaVencida = vencidas.Sum(c => c.BalanceAdeudado);
            fila.DeudaTotal = cuotas.Where(c => c != null && c.BalanceAdeudado > 0).Sum(c => c.BalanceAdeudado);

            if (vencidas.Count > 0)
            {
                DateTime masAntiguo = vencidas.Min(c => c.FechaVencimiento.Date);
                fila.VencimientoImpagoMasAntiguo = masAntiguo;
                fila.DiasAtraso = (hoy.Date - masAntiguo).Days;
            }
            else
            {
                fila.VencimientoImpagoMasAntiguo = null;
                fila.DiasAtraso = 0;
            }

            fila.RangoAtraso = ObtenerRangoAtraso(fila.DiasAtraso);
        }

        // Moroso = inscripción vigente con al menos una cuota vencida impaga.
        public static bool EsMorosa(ReporteMorosidadFila_83KI fila)
        {
            return fila != null && fila.CantidadCuotasVencidas > 0;
        }

        public static bool CumpleMinimos(ReporteMorosidadFila_83KI fila, FiltroReporteMorosidad_83KI filtro)
        {
            if (filtro == null) return true;
            if (filtro.DiasAtrasoMinimo.HasValue && fila.DiasAtraso < filtro.DiasAtrasoMinimo.Value) return false;
            if (filtro.DeudaVencidaMinima.HasValue && fila.DeudaVencida < filtro.DeudaVencidaMinima.Value) return false;
            return true;
        }

        // Rangos de la torta: 1-30, 31-60 y más de 60 días. Sin atraso devuelve vacío.
        public static string ObtenerRangoAtraso(int diasAtraso)
        {
            if (diasAtraso <= 0) return string.Empty;
            if (diasAtraso <= 30) return Rango1a30;
            if (diasAtraso <= 60) return Rango31a60;
            return RangoMasDe60;
        }

        // Orden: primero quien más deuda vencida tiene; a igual deuda, quien tiene más días de atraso.
        public static List<ReporteMorosidadFila_83KI> Ordenar(IEnumerable<ReporteMorosidadFila_83KI> filas)
        {
            return (filas ?? Enumerable.Empty<ReporteMorosidadFila_83KI>())
                .OrderByDescending(f => f.DeudaVencida)
                .ThenByDescending(f => f.DiasAtraso)
                .ThenBy(f => f.Apellido)
                .ThenBy(f => f.Nombre)
                .ThenBy(f => f.CodigoInscripcion)
                .ToList();
        }

        // Cantidad de inscripciones morosas por rango, siempre con los tres rangos y en orden.
        public static Dictionary<string, int> ContarPorRango(IEnumerable<ReporteMorosidadFila_83KI> filas)
        {
            var conteo = new Dictionary<string, int> { { Rango1a30, 0 }, { Rango31a60, 0 }, { RangoMasDe60, 0 } };
            foreach (var fila in filas ?? Enumerable.Empty<ReporteMorosidadFila_83KI>())
            {
                string rango = ObtenerRangoAtraso(fila.DiasAtraso);
                if (conteo.ContainsKey(rango)) conteo[rango]++;
            }
            return conteo;
        }

        // ------------------------------------------------------------------
        // Validaciones
        // ------------------------------------------------------------------

        private void ValidarPermiso()
        {
            if (_sessionManager == null || !_sessionManager.TienePermiso(PermisoSistema_83KI.VerReporteMorosidad))
                throw new InvalidOperationException("Errores.SinPermisos");
        }

        public static void ValidarFiltro(FiltroReporteMorosidad_83KI filtro)
        {
            if (filtro == null) return;

            if (!string.IsNullOrWhiteSpace(filtro.DNI))
            {
                string dni = filtro.DNI.Trim();
                if (dni.Length > 20 || dni.Any(c => c < '0' || c > '9'))
                    throw new ArgumentException("Errores.ReporteMorosidadDniInvalido");
            }

            if (filtro.DiasAtrasoMinimo.HasValue && filtro.DiasAtrasoMinimo.Value < 0)
                throw new ArgumentException("Errores.ReporteMorosidadMinimoInvalido");

            if (filtro.DeudaVencidaMinima.HasValue && filtro.DeudaVencidaMinima.Value < 0)
                throw new ArgumentException("Errores.ReporteMorosidadMinimoInvalido");
        }
    }
}

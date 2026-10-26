using BE.Entidades;
using Service.DTOs;
using BE.DTOs.Comisiones;
using BE.DTOs.Evaluaciones;
using Service.Entidades;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace BLL
{
    public class KI68ResultadoEvaluacionBLL
    {
        private readonly KI68ComisionBLL _comisionBLL;
        private readonly IResultadoEvaluacionDAL_83KI _dal;
        private readonly ISessionManager_83KI _sessionManager;
        private readonly IBitacoraManager_83KI _bitacora;

        public KI68ResultadoEvaluacionBLL(KI68ComisionBLL comisionBLL, IResultadoEvaluacionDAL_83KI dal,
            ISessionManager_83KI sessionManager, IBitacoraManager_83KI bitacora)
        {
            _comisionBLL = comisionBLL ?? throw new ArgumentNullException(nameof(comisionBLL));
            _dal = dal ?? throw new ArgumentNullException(nameof(dal));
            _sessionManager = sessionManager ?? throw new ArgumentNullException(nameof(sessionManager));
            _bitacora = bitacora ?? throw new ArgumentNullException(nameof(bitacora));
        }

        public List<ComisionListado_83KI> ListarComisionesAsignadasParaEvaluacion()
        {
            ValidarSesion();
            return _comisionBLL.ListarComisionesAsignadasParaEvaluacion().ToList();
        }

        public DateTime ObtenerFechaActual()
        {
            ValidarSesion();
            return _comisionBLL.ObtenerFechaActual();
        }

        public List<AlumnoEvaluacionListado_83KI> BuscarAlumnosHabilitados(BusquedaResultadoEvaluacionRequest_83KI request)
        {
            if (request == null) throw new InvalidOperationException("Cun08.CriteriosInvalidos");
            ValidarSesion();
            var comision = _comisionBLL.ObtenerComisionPorCursoYNumero(request.CodigoCurso, request.NumeroComision);
            if (comision == null) throw new InvalidOperationException("Errores.ComisionNoEncontrada");
            comision = ValidarProfesorYComision(comision.IdComision);
            ValidarFecha(comision, request.FechaEvaluacion);
            return _dal.BuscarAlumnosHabilitados(comision.IdComision, request.FechaEvaluacion.Date);
        }

        public void RegistrarResultados(ResultadoEvaluacionRequest_83KI request)
        {
            ValidarDatos(request);
            request.AuditoriaFallida = false;
            var comision = ValidarProfesorYComision(request.IdComision);
            ValidarFecha(comision, request.FechaEvaluacion);
            ValidarAlumnosHabilitados(request);
            foreach (var resultado in request.Resultados) DeterminarCondicionFinal(resultado.Calificacion);
            _dal.RegistrarResultados(request); // Retorna solo después de confirmar los datos de negocio y de integridad.
            try { RegistrarAuditoria(request); }
            catch { request.AuditoriaFallida = true; } // Nunca sugerir un reintento de calificaciones ya confirmadas.
        }

        private void ValidarDatos(ResultadoEvaluacionRequest_83KI request)
        {
            if (request == null || request.IdComision <= 0 || request.Resultados == null || request.Resultados.Count == 0)
                throw new InvalidOperationException("Cun08.TandaVacia");
            if (request.Resultados.Any(r => r == null || r.IdInscripcion <= 0)
                || request.Resultados.Select(r => r.IdInscripcion).Distinct().Count() != request.Resultados.Count)
                throw new InvalidOperationException("Cun08.AlumnosInvalidos");
            foreach (var r in request.Resultados)
            {
                if (r.Calificacion < 1m || r.Calificacion > 10m || decimal.Round(r.Calificacion, 2) != r.Calificacion)
                    throw new InvalidOperationException("Cun08.CalificacionInvalida");
                if ((r.Observaciones ?? string.Empty).Length > 500) throw new InvalidOperationException("Cun08.ObservacionesLargas");
            }
        }

        private void ValidarSesion()
        {
            var usuario = _sessionManager.UsuarioActivo;
            if (usuario == null || !usuario.Activo || usuario.Bloqueado || _sessionManager.RequiereRecuperacionIntegridad
                || !_sessionManager.TienePermiso(PermisoSistema_83KI.RegistrarResultadosEvaluacion))
                throw new InvalidOperationException("Cun08.SinAutorizacion");
        }

        private Comision_83KI ValidarProfesorYComision(int idComision)
        {
            ValidarSesion();
            int profesor = _comisionBLL.ObtenerProfesorPorDni(_sessionManager.UsuarioActivo.DNI.ToString(CultureInfo.InvariantCulture));
            _comisionBLL.ValidarProfesorAsignado(idComision, profesor);
            return _comisionBLL.ObtenerComision(idComision);
        }

        private void ValidarFecha(Comision_83KI comision, DateTime fecha)
        {
            // Las evaluaciones se registran durante la cursada: no se exige que la comisión haya finalizado.
            if (fecha.Date < comision.FechaInicio.Date || fecha.Date > comision.FechaFin.Date)
                throw new InvalidOperationException("Cun08.FechaInvalida");
            if (fecha.Date > _comisionBLL.ObtenerFechaActual().Date)
                throw new InvalidOperationException("Cun08.FechaFutura");
        }

        private void ValidarAlumnosHabilitados(ResultadoEvaluacionRequest_83KI request)
        {
            var habilitados = new HashSet<int>(_dal.BuscarAlumnosHabilitados(request.IdComision, request.FechaEvaluacion.Date).Select(a => a.IdInscripcion));
            foreach (var r in request.Resultados)
                if (!habilitados.Contains(r.IdInscripcion) || _dal.ExisteResultado(r.IdInscripcion, request.FechaEvaluacion.Date))
                    throw new InvalidOperationException("Cun08.AlumnosInvalidos");
        }

        private string DeterminarCondicionFinal(decimal calificacion)
        {
            return new ResultadoEvaluacion_83KI { Calificacion = calificacion }.DeterminarCondicionFinal();
        }

        private void RegistrarAuditoria(ResultadoEvaluacionRequest_83KI request)
        {
            _bitacora.RegistrarEvento(BitacoraEvento_83KI.CrearNuevo(
                string.Format(CultureInfo.InvariantCulture, "CUN08: comisión {0}, evaluación {1:yyyy-MM-dd}, resultados {2}", request.IdComision, request.FechaEvaluacion.Date, request.Resultados.Count),
                Criticidad.Medio, Modulo.PlanificacionAcademica, _sessionManager.UsuarioActivo.UserName));
        }
    }
}

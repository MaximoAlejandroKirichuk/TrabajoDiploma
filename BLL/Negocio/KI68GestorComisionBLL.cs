using BE.Entidades;
using Service.DTOs;
using Service.Entidades;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace BLL
{
    public class KI68GestorComisionBLL : IGestorComision_83KI
    {
        private readonly IComisionDAL_83KI _comisionDAL;
        private readonly ISessionManager_83KI _sessionManager;
        private readonly IBitacoraManager_83KI _bitacora;

        public KI68GestorComisionBLL(IComisionDAL_83KI comisionDAL, ISessionManager_83KI sessionManager, IBitacoraManager_83KI bitacora)
        {
            _comisionDAL = comisionDAL;
            _sessionManager = sessionManager;
            _bitacora = bitacora;
        }

        public IEnumerable<Curso_83KI> ObtenerCursosActivosParaPreapertura()
        {
            ValidarPermisoPlanificacion();
            return _comisionDAL.ObtenerCursosActivosParaPreapertura();
        }

        public IEnumerable<Profesor_83KI> ObtenerProfesoresActivosParaFiltro()
        {
            ValidarPermisoPlanificacion();
            return _comisionDAL.ObtenerProfesoresActivosParaFiltro();
        }

        public IEnumerable<Profesor_83KI> ObtenerProfesoresDisponibles(int idCurso, DayOfWeek diaSemana, TimeSpan horaInicio, TimeSpan horaFin)
        {
            ValidarPermiso();
            ValidarHorario(idCurso, horaInicio, horaFin);
            return _comisionDAL.ObtenerProfesoresDisponibles(idCurso, diaSemana, horaInicio, horaFin);
        }

        public IEnumerable<PlanDePago_83KI> ObtenerPlanesDePagoActivos()
        {
            ValidarPermiso();
            return _comisionDAL.ObtenerPlanesDePagoActivos();
        }

        public string RegistrarPreapertura(int idCurso, int idProfesor, DayOfWeek diaSemana, TimeSpan horaInicio, TimeSpan horaFin, int cupoMinimo, int cupoMaximo, DateTime fechaLimitePago, DateTime fechaInicio, DateTime fechaFin, decimal arancelBase, int idPlanDePago, decimal recargoPlanSnapshot)
        {
            ValidarPermiso(PermisoSistema_83KI.RegistrarPreaperturaComision);
            if (arancelBase <= 0) throw new ArgumentException("Errores.ArancelBaseInvalido", nameof(arancelBase));

            var plan = _comisionDAL.ObtenerPlanesDePagoActivos().FirstOrDefault(p => p.IdPlanDePago == idPlanDePago);
            if (plan == null) throw new InvalidOperationException("Errores.PlanDePagoNoDisponible");

            var comision = Comision_83KI.CrearPreapertura(idCurso, idProfesor, diaSemana, horaInicio, horaFin, cupoMinimo, cupoMaximo, fechaLimitePago, fechaInicio, fechaFin, arancelBase, idPlanDePago, plan.RecargoPorcentaje);
            if (!_comisionDAL.ValidarCursoProfesorDisponibilidad(idCurso, idProfesor, diaSemana, horaInicio, horaFin))
                throw new InvalidOperationException("Errores.ProfesorNoDisponibleParaPreapertura");
            if (_comisionDAL.ExisteSolapamientoComision(idProfesor, diaSemana, horaInicio, horaFin))
                throw new InvalidOperationException("Errores.ProfesorSolapamientoComision");

            Comision_83KI registrada;
            try
            {
                registrada = _comisionDAL.RegistrarPreapertura(comision);
            }
            catch (SqlException ex)
            {
                RegistrarAuditoriaSegura($"Error técnico al registrar preapertura de comisión. Actor: {UsuarioActual}. SQL {ex.Number}", Criticidad.Alto, UsuarioActual);
                throw new InvalidOperationException(ClaveErrorSql(ex) ?? "Errores.ComisionNoRegistrada", ex);
            }

            RegistrarAuditoriaSegura($"Registro de preapertura de comisión: {registrada.Codigo}. Curso {idCurso}, Profesor {idProfesor}. Actor: {UsuarioActual}", Criticidad.Alto, UsuarioActual);
            return registrada.Codigo;
        }

        public IEnumerable<ComisionListado_83KI> ListarComisiones(FiltroComision_83KI filtro)
        {
            ValidarPermiso(PermisoSistema_83KI.VerComisiones);
            ValidarFiltro(filtro);
            return _comisionDAL.ListarComisiones(filtro ?? new FiltroComision_83KI());
        }

        public Comision_83KI ObtenerComision(int idComision)
        {
            ValidarPermiso(PermisoSistema_83KI.VerComisiones);
            if (idComision <= 0) throw new ArgumentException("Errores.ComisionObligatoria", nameof(idComision));
            var comision = _comisionDAL.ObtenerComision(idComision);
            if (comision == null) throw new InvalidOperationException("Errores.ComisionNoEncontrada");
            return comision;
        }

        public void ModificarComision(Comision_83KI comision)
        {
            ValidarPermiso(PermisoSistema_83KI.ModificarComision);
            if (comision == null) throw new ArgumentNullException(nameof(comision));
            if (comision.IdComision <= 0) throw new ArgumentException("Errores.ComisionObligatoria");
            if (comision.EstaEliminada) throw new InvalidOperationException("Errores.ComisionEliminadaNoEditable");
            if (!_comisionDAL.ValidarCursoProfesorDisponibilidad(comision.IdCurso, comision.IdProfesor, comision.DiaSemana, comision.HoraInicio, comision.HoraFin))
                throw new InvalidOperationException("Errores.ProfesorNoDisponibleParaPreapertura");
            if (_comisionDAL.ExisteSolapamientoComision(comision.IdProfesor, comision.DiaSemana, comision.HoraInicio, comision.HoraFin, comision.IdComision))
                throw new InvalidOperationException("Errores.ProfesorSolapamientoComision");
            try
            {
                _comisionDAL.ModificarComision(comision);
            }
            catch (SqlException ex)
            {
                RegistrarAuditoriaSegura($"Error técnico al modificar comisión {comision.Codigo}. Actor: {UsuarioActual}. SQL {ex.Number}", Criticidad.Alto, UsuarioActual);
                throw new InvalidOperationException(ClaveErrorSql(ex) ?? "Errores.ComisionNoModificada", ex);
            }
            RegistrarAuditoriaSegura($"Modificación de comisión: {comision.Codigo}. Actor: {UsuarioActual}", Criticidad.Medio, UsuarioActual);
        }

        public void EliminarComision(int idComision)
        {
            ValidarPermiso(PermisoSistema_83KI.EliminarComision);
            if (idComision <= 0) throw new ArgumentException("Errores.ComisionObligatoria", nameof(idComision));
            var comision = _comisionDAL.ObtenerComision(idComision);
            if (comision == null) throw new InvalidOperationException("Errores.ComisionNoEncontrada");
            if (comision.EstaEliminada) throw new InvalidOperationException("Errores.ComisionYaEliminada");
            try
            {
                _comisionDAL.EliminarComision(idComision);
            }
            catch (SqlException ex)
            {
                RegistrarAuditoriaSegura($"Error técnico al eliminar lógicamente la comisión {comision.Codigo}. Actor: {UsuarioActual}. SQL {ex.Number}", Criticidad.Alto, UsuarioActual);
                throw new InvalidOperationException(ClaveErrorSql(ex) ?? "Errores.ComisionNoEliminada", ex);
            }
            RegistrarAuditoriaSegura($"Eliminación lógica de comisión: {comision.Codigo}. Actor: {UsuarioActual}", Criticidad.Alto, UsuarioActual);
        }

        private static string ClaveErrorSql(SqlException ex)
        {
            string mensaje = ex.Message ?? string.Empty;
            if (mensaje.IndexOf("ProfesorSolapamientoComision", StringComparison.OrdinalIgnoreCase) >= 0)
                return "Errores.ProfesorSolapamientoComision";
            return null;
        }

        private void ValidarHorario(int idCurso, TimeSpan horaInicio, TimeSpan horaFin)
        {
            if (idCurso <= 0) throw new ArgumentException("Errores.CursoObligatorio", nameof(idCurso));
            if (horaInicio >= horaFin) throw new ArgumentException("Errores.HorarioInvalido");
        }

        private void ValidarFiltro(FiltroComision_83KI filtro)
        {
            if (filtro == null) return;
            if (filtro.FechaInicioDesde.HasValue && filtro.FechaFinHasta.HasValue && filtro.FechaInicioDesde.Value.Date > filtro.FechaFinHasta.Value.Date)
                throw new ArgumentException("Errores.RangoFechasComisionInvalido");
            if (!string.IsNullOrWhiteSpace(filtro.Estado)
                && !string.Equals(filtro.Estado, Comision_83KI.EstadoPreapertura, StringComparison.OrdinalIgnoreCase)
                && !string.Equals(filtro.Estado, Comision_83KI.EstadoEliminada, StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Errores.EstadoComisionInvalido");
        }

        private void ValidarPermiso()
        {
            ValidarPermiso(PermisoSistema_83KI.RegistrarPreaperturaComision);
        }

        private void ValidarPermiso(PermisoSistema_83KI permiso)
        {
            if (!_sessionManager.TienePermiso(permiso))
                throw new InvalidOperationException("No tiene permisos para realizar esta accion.");
        }

        private void ValidarPermisoPlanificacion()
        {
            if (!_sessionManager.TienePermiso(PermisoSistema_83KI.RegistrarPreaperturaComision)
                && !_sessionManager.TienePermiso(PermisoSistema_83KI.VerComisiones))
                throw new InvalidOperationException("No tiene permisos para realizar esta accion.");
        }

        private string UsuarioActual { get { return _sessionManager.UsuarioActivo != null ? _sessionManager.UsuarioActivo.UserName : "Sistema"; } }

        private void RegistrarAuditoriaSegura(string descripcion, Criticidad criticidad, string username)
        {
            try { _bitacora.RegistrarEvento(BitacoraEvento_83KI.CrearNuevo(descripcion, criticidad, Modulo.PlanificacionAcademica, username)); }
            catch { }
        }
    }
}

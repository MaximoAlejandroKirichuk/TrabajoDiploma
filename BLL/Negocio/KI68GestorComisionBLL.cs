using BE.Entidades;
using Service.Entidades;
using Service.Interfaces;
using System;
using System.Collections.Generic;
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
            ValidarPermiso();
            return _comisionDAL.ObtenerCursosActivosParaPreapertura();
        }

        public IEnumerable<Profesor_83KI> ObtenerProfesoresDisponibles(int idCurso, DayOfWeek diaSemana, TimeSpan horaInicio, TimeSpan horaFin)
        {
            ValidarPermiso();
            ValidarHorario(idCurso, horaInicio, horaFin);
            return _comisionDAL.ObtenerProfesoresDisponibles(idCurso, diaSemana, horaInicio, horaFin);
        }

        public string RegistrarPreapertura(int idCurso, int idProfesor, DayOfWeek diaSemana, TimeSpan horaInicio, TimeSpan horaFin, int cupoMinimo, int cupoMaximo, DateTime fechaLimitePago)
        {
            ValidarPermiso();
            var comision = Comision_83KI.CrearPreapertura(idCurso, idProfesor, diaSemana, horaInicio, horaFin, cupoMinimo, cupoMaximo, fechaLimitePago);
            if (!_comisionDAL.ValidarCursoProfesorDisponibilidad(idCurso, idProfesor, diaSemana, horaInicio, horaFin))
                throw new InvalidOperationException("Errores.ProfesorNoDisponibleParaPreapertura");

            var registrada = _comisionDAL.RegistrarPreapertura(comision);
            RegistrarAuditoriaSegura($"Registro de preapertura de comisión: {registrada.Codigo}. Curso {idCurso}, Profesor {idProfesor}. Actor: {UsuarioActual}", Criticidad.Alto, UsuarioActual);
            return registrada.Codigo;
        }

        private void ValidarHorario(int idCurso, TimeSpan horaInicio, TimeSpan horaFin)
        {
            if (idCurso <= 0) throw new ArgumentException("Errores.CursoObligatorio", nameof(idCurso));
            if (horaInicio >= horaFin) throw new ArgumentException("Errores.HorarioInvalido");
        }

        private void ValidarPermiso()
        {
            if (!_sessionManager.TienePermiso(PermisoSistema_83KI.RegistrarPreaperturaComision))
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

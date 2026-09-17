using DAL.interfaces;
using BE.Entidades;
using Service.Entidades;
using Service.Excepciones.Maestros;
using Service.Interfaces;
using System;
using System.Collections.Generic;

namespace BLL
{
    public class GestorCursoProfesorBLL_83KI : IGestorCursoProfesor_83KI
    {
        private readonly ICursoProfesorDAL_83KI _dal;
        private readonly ISessionManager_83KI _sessionManager;
        private readonly IBitacoraManager_83KI _bitacora;

        public GestorCursoProfesorBLL_83KI(ICursoProfesorDAL_83KI dal, ISessionManager_83KI sessionManager, IBitacoraManager_83KI bitacora)
        {
            _dal = dal;
            _sessionManager = sessionManager;
            _bitacora = bitacora;
        }

        public IEnumerable<Curso_83KI> ObtenerCursosActivosParaRelacion()
        {
            ValidarPermisoLectura();
            return _dal.ObtenerCursosActivosParaRelacion();
        }

        public IEnumerable<Profesor_83KI> ObtenerProfesoresActivosParaRelacion()
        {
            ValidarPermisoLectura();
            return _dal.ObtenerProfesoresActivosParaRelacion();
        }

        public IEnumerable<CursoProfesor_83KI> ObtenerRelaciones()
        {
            ValidarPermisoLectura();
            return _dal.ObtenerRelaciones();
        }

        public IEnumerable<CursoProfesor_83KI> ObtenerProfesoresHabilitadosParaCurso(int idCurso)
        {
            ValidarPermisoLectura();
            return _dal.ObtenerProfesoresHabilitadosParaCurso(idCurso);
        }

        public CursoProfesor_83KI ObtenerPorIds(int idCurso, int idProfesor)
        {
            ValidarPermisoLectura();
            return _dal.ObtenerPorIds(idCurso, idProfesor) ?? throw new RelacionCursoProfesorNoEncontradaException_83KI();
        }

        public bool EstaProfesorHabilitadoParaCurso(int idCurso, int idProfesor)
        {
            ValidarPermisoLectura();
            return _dal.EstaProfesorHabilitadoParaCurso(idCurso, idProfesor);
        }

        public void HabilitarProfesorParaCurso(int idCurso, int idProfesor)
        {
            ValidarPermiso(PermisoSistema_83KI.HabilitarCursoProfesor);
            ValidarPartesActivas(idCurso, idProfesor);

            var relacion = _dal.ObtenerPorIds(idCurso, idProfesor);
            if (relacion == null)
            {
                _dal.CrearRelacion(CursoProfesor_83KI.CrearNueva(idCurso, idProfesor));
                RegistrarAuditoriaSegura($"Relacion Curso-Profesor creada y habilitada: Curso {idCurso}, Profesor {idProfesor}. Actor: {UsuarioActual}", Criticidad.Alto, UsuarioActual);
                return;
            }

            if (relacion.EstadoActivo) throw new InvalidOperationException("Errores.CursoProfesorYaActivo");
            _dal.ActualizarEstadoActivo(idCurso, idProfesor, true);
            RegistrarAuditoriaSegura($"Relacion Curso-Profesor habilitada: Curso {idCurso}, Profesor {idProfesor}. Actor: {UsuarioActual}", Criticidad.Alto, UsuarioActual);
        }

        public void DeshabilitarProfesorParaCurso(int idCurso, int idProfesor)
        {
            ValidarPermiso(PermisoSistema_83KI.DeshabilitarCursoProfesor);
            var relacion = _dal.ObtenerPorIds(idCurso, idProfesor) ?? throw new RelacionCursoProfesorNoEncontradaException_83KI();
            if (!relacion.EstadoActivo) throw new InvalidOperationException("Errores.CursoProfesorYaInactivo");

            _dal.ActualizarEstadoActivo(idCurso, idProfesor, false);
            RegistrarAuditoriaSegura($"Relacion Curso-Profesor deshabilitada: Curso {idCurso}, Profesor {idProfesor}. Actor: {UsuarioActual}", Criticidad.Alto, UsuarioActual);
        }

        private void ValidarPartesActivas(int idCurso, int idProfesor)
        {
            if (!_dal.ExisteCursoActivo(idCurso)) throw new InvalidOperationException("Errores.CursoNoActivoParaRelacion");
            if (!_dal.ExisteProfesorActivo(idProfesor)) throw new InvalidOperationException("Errores.ProfesorNoActivoParaRelacion");
        }

        private string UsuarioActual { get { return _sessionManager.UsuarioActivo != null ? _sessionManager.UsuarioActivo.UserName : "Sistema"; } }

        private void ValidarPermiso(PermisoSistema_83KI permiso)
        {
            if (!_sessionManager.TienePermiso(permiso)) throw new InvalidOperationException("No tiene permisos para realizar esta accion.");
        }

        private void ValidarPermisoLectura()
        {
            if (!_sessionManager.TienePermiso(PermisoSistema_83KI.VerCursoProfesor)
                && !_sessionManager.TienePermiso(PermisoSistema_83KI.GestionCursoProfesor))
            {
                throw new InvalidOperationException("No tiene permisos para realizar esta accion.");
            }
        }

        private void RegistrarAuditoriaSegura(string descripcion, Criticidad criticidad, string username)
        {
            try { _bitacora.RegistrarEvento(BitacoraEvento_83KI.CrearNuevo(descripcion, criticidad, Modulo.Admin, username)); }
            catch { }
        }
    }
}

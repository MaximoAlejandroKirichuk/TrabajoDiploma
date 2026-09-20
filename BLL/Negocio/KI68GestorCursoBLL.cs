using DAL.interfaces;
using BE.Entidades;
using Service.Entidades;
using Service.Excepciones.Maestros;
using Service.Interfaces;
using System;
using System.Collections.Generic;

namespace BLL
{
    public class KI68GestorCursoBLL : IGestorCurso_83KI
    {
        private readonly ICursoDAL_83KI _dal;
        private readonly ISessionManager_83KI _sessionManager;
        private readonly IBitacoraManager_83KI _bitacora;

        public KI68GestorCursoBLL(ICursoDAL_83KI dal, ISessionManager_83KI sessionManager, IBitacoraManager_83KI bitacora)
        {
            _dal = dal;
            _sessionManager = sessionManager;
            _bitacora = bitacora;
        }

        public IEnumerable<Curso_83KI> ObtenerCursos()
        {
            ValidarPermisoLectura();
            return _dal.ObtenerTodos();
        }

        public IEnumerable<Curso_83KI> ObtenerCursosActivos()
        {
            ValidarPermisoLectura();
            return _dal.ObtenerActivos();
        }

        public Curso_83KI ObtenerPorId(int idCurso)
        {
            ValidarPermisoLectura();
            return _dal.ObtenerPorId(idCurso) ?? throw new CursoNoEncontradoException_83KI();
        }

        public void CrearCurso(string nombre, string descripcion, int cargaHoraria)
        {
            ValidarPermiso(PermisoSistema_83KI.CrearCurso);
            var curso = Curso_83KI.CrearNuevo(nombre, descripcion, cargaHoraria);
            if (_dal.ExisteNombreParaOtroCurso(curso.Nombre, 0)) throw new NombreCursoDuplicadoException_83KI();
            int id = _dal.Crear(curso);
            RegistrarAuditoriaSegura($"Curso creado: {curso.Nombre}. Id: {id}. Actor: {UsuarioActual}", Criticidad.Alto, UsuarioActual);
        }

        public void ModificarCurso(int idCurso, string nombre, string descripcion, int cargaHoraria)
        {
            ValidarPermiso(PermisoSistema_83KI.ModificarCurso);
            var curso = _dal.ObtenerPorId(idCurso) ?? throw new CursoNoEncontradoException_83KI();
            curso.Modificar(nombre, descripcion, cargaHoraria);
            if (_dal.ExisteNombreParaOtroCurso(curso.Nombre, curso.IdCurso)) throw new NombreCursoDuplicadoException_83KI();
            _dal.Modificar(curso);
            RegistrarAuditoriaSegura($"Curso modificado: {curso.Nombre}. Id: {curso.IdCurso}. Actor: {UsuarioActual}", Criticidad.Alto, UsuarioActual);
        }

        public void ActivarCurso(int idCurso) { CambiarEstado(idCurso, true); }
        public void DesactivarCurso(int idCurso) { CambiarEstado(idCurso, false); }

        private void CambiarEstado(int idCurso, bool activo)
        {
            ValidarPermiso(activo ? PermisoSistema_83KI.ActivarCurso : PermisoSistema_83KI.DesactivarCurso);
            var curso = _dal.ObtenerPorId(idCurso) ?? throw new CursoNoEncontradoException_83KI();
            if (curso.EstadoActivo == activo) throw new InvalidOperationException(activo ? "Errores.CursoYaActivo" : "Errores.CursoYaInactivo");
            if (activo) curso.Activar(); else curso.Desactivar();
            _dal.ActualizarEstadoActivo(idCurso, curso.EstadoActivo);
            RegistrarAuditoriaSegura($"Curso {(activo ? "activado" : "desactivado")}: {curso.Nombre}. Id: {curso.IdCurso}. Actor: {UsuarioActual}", Criticidad.Alto, UsuarioActual);
        }

        private string UsuarioActual => _sessionManager.UsuarioActivo != null ? _sessionManager.UsuarioActivo.UserName : "Sistema";

        private void ValidarPermiso(PermisoSistema_83KI permiso)
        {
            if (!_sessionManager.TienePermiso(permiso)) throw new InvalidOperationException("No tiene permisos para realizar esta accion.");
        }

        private void ValidarPermisoLectura()
        {
            if (!_sessionManager.TienePermiso(PermisoSistema_83KI.VerCursos)
                && !_sessionManager.TienePermiso(PermisoSistema_83KI.GestionCursos))
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

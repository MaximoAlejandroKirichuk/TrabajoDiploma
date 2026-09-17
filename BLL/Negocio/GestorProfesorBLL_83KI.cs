using DAL.interfaces;
using BE.Entidades;
using Service.Entidades;
using Service.Excepciones.Maestros;
using Service.Interfaces;
using System;
using System.Collections.Generic;

namespace BLL
{
    public class GestorProfesorBLL_83KI : IGestorProfesor_83KI
    {
        private readonly IProfesorDAL_83KI _dal;
        private readonly ISessionManager_83KI _sessionManager;
        private readonly IBitacoraManager_83KI _bitacora;

        public GestorProfesorBLL_83KI(IProfesorDAL_83KI dal, ISessionManager_83KI sessionManager, IBitacoraManager_83KI bitacora)
        {
            _dal = dal;
            _sessionManager = sessionManager;
            _bitacora = bitacora;
        }

        public IEnumerable<Profesor_83KI> ObtenerProfesores()
        {
            ValidarPermisoLectura();
            return _dal.ObtenerTodos();
        }

        public IEnumerable<Profesor_83KI> ObtenerProfesoresActivos()
        {
            ValidarPermisoLectura();
            return _dal.ObtenerActivos();
        }

        public Profesor_83KI ObtenerPorId(int idProfesor)
        {
            ValidarPermisoLectura();
            return _dal.ObtenerPorId(idProfesor) ?? throw new ProfesorNoEncontradoException_83KI();
        }

        public void CrearProfesor(string dni, string nombre, string apellido, string email)
        {
            ValidarPermiso(PermisoSistema_83KI.CrearProfesor);
            var profesor = Profesor_83KI.CrearNuevo(dni, nombre, apellido, email);
            if (_dal.ExisteDniParaOtroProfesor(profesor.DNI, 0)) throw new DniProfesorDuplicadoException_83KI();
            int id = _dal.Crear(profesor);
            RegistrarAuditoriaSegura($"Profesor creado: DNI {profesor.DNI}. Id: {id}. Actor: {UsuarioActual}", Criticidad.Alto, UsuarioActual);
        }

        public void ModificarProfesor(int idProfesor, string nombre, string apellido, string email)
        {
            ValidarPermiso(PermisoSistema_83KI.ModificarProfesor);
            var profesor = _dal.ObtenerPorId(idProfesor) ?? throw new ProfesorNoEncontradoException_83KI();
            profesor.Modificar(nombre, apellido, email);
            _dal.Modificar(profesor);
            RegistrarAuditoriaSegura($"Profesor modificado: DNI {profesor.DNI}. Id: {profesor.IdProfesor}. Actor: {UsuarioActual}", Criticidad.Alto, UsuarioActual);
        }

        public void ActivarProfesor(int idProfesor) { CambiarEstado(idProfesor, true); }
        public void DesactivarProfesor(int idProfesor) { CambiarEstado(idProfesor, false); }

        private void CambiarEstado(int idProfesor, bool activo)
        {
            ValidarPermiso(activo ? PermisoSistema_83KI.ActivarProfesor : PermisoSistema_83KI.DesactivarProfesor);
            var profesor = _dal.ObtenerPorId(idProfesor) ?? throw new ProfesorNoEncontradoException_83KI();
            if (profesor.EstadoActivo == activo) throw new InvalidOperationException(activo ? "Errores.ProfesorYaActivo" : "Errores.ProfesorYaInactivo");
            if (activo) profesor.Activar(); else profesor.Desactivar();
            _dal.ActualizarEstadoActivo(idProfesor, profesor.EstadoActivo);
            RegistrarAuditoriaSegura($"Profesor {(activo ? "activado" : "desactivado")}: DNI {profesor.DNI}. Id: {profesor.IdProfesor}. Actor: {UsuarioActual}", Criticidad.Alto, UsuarioActual);
        }

        private string UsuarioActual => _sessionManager.UsuarioActivo != null ? _sessionManager.UsuarioActivo.UserName : "Sistema";

        private void ValidarPermiso(PermisoSistema_83KI permiso)
        {
            if (!_sessionManager.TienePermiso(permiso)) throw new InvalidOperationException("No tiene permisos para realizar esta accion.");
        }

        private void ValidarPermisoLectura()
        {
            if (!_sessionManager.TienePermiso(PermisoSistema_83KI.VerProfesores)
                && !_sessionManager.TienePermiso(PermisoSistema_83KI.GestionProfesores))
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

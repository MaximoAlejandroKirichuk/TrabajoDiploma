using DAL.interfaces;
using BE.Entidades;
using Service.Entidades;
using Service.Excepciones.Maestros;
using Service.Interfaces;
using System;
using System.Collections.Generic;

namespace BLL
{
    public class KI68GestorProfesorBLL : IGestorProfesor_83KI
    {
        private readonly IProfesorDAL_83KI _dal;
        private readonly IDisponibilidadProfesorDAL_83KI _disponibilidadDal;
        private readonly ISessionManager_83KI _sessionManager;
        private readonly IBitacoraManager_83KI _bitacora;

        public KI68GestorProfesorBLL(IProfesorDAL_83KI dal, ISessionManager_83KI sessionManager, IBitacoraManager_83KI bitacora)
            : this(dal, new DAL.KI68DisponibilidadProfesorDAL(), sessionManager, bitacora)
        {
        }

        public KI68GestorProfesorBLL(IProfesorDAL_83KI dal, IDisponibilidadProfesorDAL_83KI disponibilidadDal, ISessionManager_83KI sessionManager, IBitacoraManager_83KI bitacora)
        {
            _dal = dal;
            _disponibilidadDal = disponibilidadDal;
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

        public IEnumerable<DisponibilidadProfesor_83KI> ObtenerDisponibilidades(int idProfesor)
        {
            ValidarPermisoLectura();
            ValidarProfesorExistente(idProfesor);
            return _disponibilidadDal.ObtenerPorProfesor(idProfesor);
        }

        public void AgregarDisponibilidad(int idProfesor, DayOfWeek diaSemana, TimeSpan horaInicio, TimeSpan horaFin)
        {
            ValidarPermiso(PermisoSistema_83KI.ModificarProfesor);
            ValidarProfesorExistente(idProfesor);
            var disponibilidad = DisponibilidadProfesor_83KI.CrearNueva(idProfesor, diaSemana, horaInicio, horaFin);
            ValidarDisponibilidadActivaNoDuplicada(disponibilidad);
            int id = _disponibilidadDal.Insertar(disponibilidad);
            RegistrarAuditoriaSegura($"Disponibilidad de profesor creada. IdProfesor: {idProfesor}. IdDisponibilidad: {id}. Actor: {UsuarioActual}", Criticidad.Alto, UsuarioActual);
        }

        public void ModificarDisponibilidad(int idDisponibilidadProfesor, int idProfesor, DayOfWeek diaSemana, TimeSpan horaInicio, TimeSpan horaFin)
        {
            ValidarPermiso(PermisoSistema_83KI.ModificarProfesor);
            var disponibilidad = ObtenerDisponibilidadRequerida(idDisponibilidadProfesor, idProfesor);
            disponibilidad.ModificarHorario(diaSemana, horaInicio, horaFin);
            ValidarDisponibilidadActivaNoDuplicada(disponibilidad);
            _disponibilidadDal.Actualizar(disponibilidad);
            RegistrarAuditoriaSegura($"Disponibilidad de profesor modificada. IdProfesor: {idProfesor}. IdDisponibilidad: {idDisponibilidadProfesor}. Actor: {UsuarioActual}", Criticidad.Alto, UsuarioActual);
        }

        public void ActivarDisponibilidad(int idDisponibilidadProfesor, int idProfesor)
        {
            CambiarEstadoDisponibilidad(idDisponibilidadProfesor, idProfesor, true);
        }

        public void DesactivarDisponibilidad(int idDisponibilidadProfesor, int idProfesor)
        {
            CambiarEstadoDisponibilidad(idDisponibilidadProfesor, idProfesor, false);
        }

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

        private void CambiarEstadoDisponibilidad(int idDisponibilidadProfesor, int idProfesor, bool activo)
        {
            ValidarPermiso(PermisoSistema_83KI.ModificarProfesor);
            var disponibilidad = ObtenerDisponibilidadRequerida(idDisponibilidadProfesor, idProfesor);
            if (activo) disponibilidad.Activar(); else disponibilidad.Desactivar();
            ValidarDisponibilidadActivaNoDuplicada(disponibilidad);
            _disponibilidadDal.Actualizar(disponibilidad);
            RegistrarAuditoriaSegura($"Disponibilidad de profesor {(activo ? "activada" : "desactivada")}. IdProfesor: {idProfesor}. IdDisponibilidad: {idDisponibilidadProfesor}. Actor: {UsuarioActual}", Criticidad.Alto, UsuarioActual);
        }

        private void ValidarDisponibilidadActivaNoDuplicada(DisponibilidadProfesor_83KI disponibilidad)
        {
            if (disponibilidad == null || !disponibilidad.EstadoActivo) return;

            foreach (var existente in _disponibilidadDal.ObtenerPorProfesor(disponibilidad.IdProfesor))
            {
                if (!existente.EstadoActivo) continue;
                if (existente.IdDisponibilidadProfesor == disponibilidad.IdDisponibilidadProfesor) continue;

                if (existente.DiaSemana == disponibilidad.DiaSemana
                    && existente.HoraInicio == disponibilidad.HoraInicio
                    && existente.HoraFin == disponibilidad.HoraFin)
                {
                    throw new InvalidOperationException("Errores.DisponibilidadProfesorDuplicada");
                }
            }
        }

        private DisponibilidadProfesor_83KI ObtenerDisponibilidadRequerida(int idDisponibilidadProfesor, int idProfesor)
        {
            ValidarProfesorExistente(idProfesor);
            var disponibilidad = _disponibilidadDal.ObtenerPorProfesor(idProfesor);
            foreach (var item in disponibilidad)
            {
                if (item.IdDisponibilidadProfesor == idDisponibilidadProfesor) return item;
            }

            throw new InvalidOperationException("Errores.DisponibilidadProfesorNoEncontrada");
        }

        private void ValidarProfesorExistente(int idProfesor)
        {
            if (idProfesor <= 0) throw new InvalidOperationException("Errores.ProfesorObligatorio");
            if (_dal.ObtenerPorId(idProfesor) == null) throw new ProfesorNoEncontradoException_83KI();
        }

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

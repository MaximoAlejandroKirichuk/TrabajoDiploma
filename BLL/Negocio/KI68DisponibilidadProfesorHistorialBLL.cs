using Service.DTOs;
using BE.Entidades;
using Service.Entidades;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BLL
{
    public sealed class KI68DisponibilidadProfesorHistorialBLL : IDisponibilidadProfesorHistorialBLL_83KI
    {
        private readonly IDisponibilidadProfesorDAL_83KI _dal;
        private readonly ISessionManager_83KI _sesion;
        private readonly IBitacoraManager_83KI _bitacora;

        public KI68DisponibilidadProfesorHistorialBLL(IDisponibilidadProfesorDAL_83KI dal, ISessionManager_83KI sesion, IBitacoraManager_83KI bitacora)
        {
            _dal = dal ?? throw new ArgumentNullException(nameof(dal));
            _sesion = sesion ?? throw new ArgumentNullException(nameof(sesion));
            _bitacora = bitacora ?? throw new ArgumentNullException(nameof(bitacora));
        }

        public IEnumerable<Service.DTOs.DisponibilidadProfesorHistorial_83KI> ObtenerHistorial(int idProfesor, int? idDisponibilidadProfesor, DateTime? fechaDesde, DateTime? fechaHasta)
        {
            ValidarLectura();
            return _dal.ListarHistorial(new FiltroDisponibilidadProfesorHistorial_83KI(idProfesor, idDisponibilidadProfesor, fechaDesde, fechaHasta))
                .Select(x => new Service.DTOs.DisponibilidadProfesorHistorial_83KI
                {
                    IdVersion = x.IdVersion,
                    IdDisponibilidadProfesor = x.IdDisponibilidadProfesor,
                    IdProfesor = x.IdProfesor,
                    DiaSemana = x.DiaSemana.ToString(),
                    HoraInicio = x.HoraInicio,
                    HoraFin = x.HoraFin,
                    EstadoActivo = x.EstadoActivo,
                    DVH = x.DVH,
                    Act = x.Act,
                    FechaRegistro = x.FechaRegistro,
                    TipoOperacion = x.TipoOperacion,
                    Usuario = x.Usuario
                }).ToList();
        }

        public void ActivarVersion(long idVersion, int idProfesor)
        {
            if (!_sesion.TienePermiso(PermisoSistema_83KI.ModificarProfesor)) throw new InvalidOperationException("Errores.SinPermisos");
            if (idVersion <= 0) throw new ArgumentOutOfRangeException(nameof(idVersion));
            var versionProfesor = _dal.ListarHistorial(new FiltroDisponibilidadProfesorHistorial_83KI(idProfesor, null, null, null));
            var version = versionProfesor.FirstOrDefault(x => x.IdVersion == idVersion);
            if (version == null) throw new InvalidOperationException("Errores.DisponibilidadProfesorVersionNoEncontrada");
            if (version.Act) return;
            var disponibilidad = DisponibilidadProfesor_83KI.ReconstruirDesdePersistencia(
                version.IdDisponibilidadProfesor, version.IdProfesor, version.DiaSemana,
                version.HoraInicio, version.HoraFin, version.EstadoActivo, version.DVH);
            ReglasDisponibilidadProfesor_83KI.ValidarActivaNoDuplicada(disponibilidad, _dal.ObtenerPorProfesor(idProfesor));
            string usuario = _sesion.UsuarioActivo != null ? _sesion.UsuarioActivo.UserName : null;
            _dal.ActivarVersion(idVersion, usuario);
            try
            {
                _bitacora.RegistrarEvento(BitacoraEvento_83KI.CrearNuevo(
                    $"Activación de versión histórica de disponibilidad. IdVersion: {idVersion}. IdProfesor: {idProfesor}. Actor: {usuario}",
                    Criticidad.Alto, Modulo.Admin, usuario));
            }
            catch
            {
                throw new InvalidOperationException("Errores.AuditoriaFallidaCambioConfirmado");
            }
        }

        private void ValidarLectura()
        {
            if (!_sesion.TienePermiso(PermisoSistema_83KI.VerProfesores) && !_sesion.TienePermiso(PermisoSistema_83KI.GestionProfesores))
                throw new InvalidOperationException("Errores.SinPermisos");
        }
    }
}

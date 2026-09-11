using Service.DTOs;
using Service;
using Service.Entidades;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BLL
{
    public class ConsultaBitacoraEventos_83KI : IConsultaBitacoraEventos_83KI
    {
        private readonly IGestorUsuario_83KI _gestorUsuario;
        private readonly IBitacoraManager_83KI _bitacoraManager;
        private readonly ISessionManager_83KI _sessionManager;

        public ConsultaBitacoraEventos_83KI(IGestorUsuario_83KI gestorUsuario, IBitacoraManager_83KI bitacoraManager)
            : this(gestorUsuario, bitacoraManager, SessionManager_83KI.Instancia)
        {
        }

        public ConsultaBitacoraEventos_83KI(IGestorUsuario_83KI gestorUsuario, IBitacoraManager_83KI bitacoraManager, ISessionManager_83KI sessionManager)
        {
            _gestorUsuario = gestorUsuario;
            _bitacoraManager = bitacoraManager;
            _sessionManager = sessionManager;
        }

        public IEnumerable<BitacoraEventoVista_83KI> Consultar(FiltroBitacoraEventos_83KI filtro)
        {
            if (!_sessionManager.TienePermiso(PermisoSistema_83KI.VerBitacoraEventos)
                && !_sessionManager.TienePermiso(PermisoSistema_83KI.ConsultarBitacoraEventos))
            {
                throw new InvalidOperationException("No tiene permisos para realizar esta accion.");
            }

            if (filtro == null)
            {
                throw new ArgumentNullException(nameof(filtro));
            }

            IEnumerable<Usuario_83KI> usuarios = _gestorUsuario.ObtenerUsuarios();

            return _bitacoraManager.ListarEventos(filtro.FechaDesde, filtro.FechaHasta)
                .Select(evento => CrearVista(evento, usuarios))
                .Where(evento => CumpleFiltros(evento, filtro))
                .OrderByDescending(evento => evento.Fecha)
                .ToList();
        }

        private BitacoraEventoVista_83KI CrearVista(BitacoraEvento_83KI eventoBitacora, IEnumerable<Usuario_83KI> usuarios)
        {
            Usuario_83KI usuario = usuarios.FirstOrDefault(u => string.Equals(u.UserName, eventoBitacora.Username, StringComparison.OrdinalIgnoreCase));

            return new BitacoraEventoVista_83KI
            {
                Id = eventoBitacora.Id,
                Fecha = eventoBitacora.Fecha,
                Username = eventoBitacora.Username,
                Nombre = usuario != null ? usuario.Nombre : string.Empty,
                Apellido = usuario != null ? usuario.Apellido : string.Empty,
                Modulo = eventoBitacora.Modulo,
                Criticidad = eventoBitacora.Criticidad,
                Evento = eventoBitacora.Descripcion
            };
        }

        private bool CumpleFiltros(BitacoraEventoVista_83KI evento, FiltroBitacoraEventos_83KI filtro)
        {
            return Contiene(evento.Nombre, filtro.Nombre)
                && Contiene(evento.Apellido, filtro.Apellido)
                && Contiene(evento.Username, filtro.Username)
                && EventoBitacoraCatalogo_83KI.CoincideConEvento(evento.Evento, filtro.Evento)
                && CoincideConModulo(evento, filtro.Modulo)
                && (!filtro.Criticidad.HasValue || evento.Criticidad.Equals(filtro.Criticidad.Value));
        }

        private bool CoincideConModulo(BitacoraEventoVista_83KI evento, Modulo? moduloFiltro)
        {
            if (!moduloFiltro.HasValue)
            {
                return true;
            }

            if (evento.Modulo.Equals(moduloFiltro.Value))
            {
                return true;
            }

            string nombreCanonico = EventoBitacoraCatalogo_83KI.ResolverNombre(evento.Evento);
            if (string.IsNullOrWhiteSpace(nombreCanonico))
            {
                return false;
            }

            return EventoBitacoraCatalogo_83KI.ObtenerPorModulo(moduloFiltro.Value)
                .Any(opcion => string.Equals(opcion.Nombre, nombreCanonico, StringComparison.OrdinalIgnoreCase));
        }

        private bool Contiene(string valor, string filtro)
        {
            if (string.IsNullOrWhiteSpace(filtro))
            {
                return true;
            }

            return (valor ?? string.Empty).IndexOf(filtro.Trim(), StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private void ValidarPermiso(PermisoSistema_83KI permiso)
        {
            if (!_sessionManager.TienePermiso(permiso))
            {
                throw new InvalidOperationException("No tiene permisos para realizar esta accion.");
            }
        }
    }
}

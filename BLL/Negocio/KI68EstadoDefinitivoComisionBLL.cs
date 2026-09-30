using Service.DTOs;
using Service.Entidades;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace BLL
{
    public class KI68EstadoDefinitivoComisionBLL : IEstadoDefinitivoComisionBLL_83KI
    {
        private readonly IEstadoDefinitivoComisionDAL_83KI _dal;
        private readonly ISessionManager_83KI _sessionManager;
        private readonly IBitacoraManager_83KI _bitacora;

        public KI68EstadoDefinitivoComisionBLL(IEstadoDefinitivoComisionDAL_83KI dal, ISessionManager_83KI sessionManager, IBitacoraManager_83KI bitacora)
        {
            _dal = dal;
            _sessionManager = sessionManager;
            _bitacora = bitacora;
        }

        public IEnumerable<ComisionEstadoDefinitivoListado_83KI> ListarComisionesVencidas()
        {
            ValidarPermiso();
            return _dal.ListarComisionesVencidas();
        }

        public EvaluacionQuorumComision_83KI Evaluar(int idComision)
        {
            ValidarPermiso();
            if (idComision <= 0) throw new ArgumentException("Errores.ComisionObligatoria");
            return _dal.Evaluar(idComision);
        }

        public ResultadoEstadoDefinitivoComision_83KI RegistrarAltaOficial(AltaOficialComisionRequest_83KI request)
        {
            ValidarPermiso();
            ValidarAlta(request);
            request.UsuarioRegistro = string.IsNullOrWhiteSpace(request.UsuarioRegistro) ? UsuarioActual : request.UsuarioRegistro;
            var resultado = Ejecutar(() => _dal.RegistrarAltaOficial(request));
            resultado.AuditoriaFallida = !RegistrarAuditoriaSegura("Registro de alta oficial de comisión", resultado);
            return resultado;
        }

        public ResultadoEstadoDefinitivoComision_83KI RegistrarActaCierre(ActaCierreComisionRequest_83KI request)
        {
            ValidarPermiso();
            ValidarCierre(request);
            request.UsuarioRegistro = string.IsNullOrWhiteSpace(request.UsuarioRegistro) ? UsuarioActual : request.UsuarioRegistro;
            var resultado = Ejecutar(() => _dal.RegistrarActaCierre(request));
            resultado.AuditoriaFallida = !RegistrarAuditoriaSegura("Registro de cierre de comisión", resultado);
            return resultado;
        }

        private void ValidarPermiso()
        {
            if (!_sessionManager.TienePermiso(PermisoSistema_83KI.RegistrarEstadoDefinitivoComision)) throw new InvalidOperationException("Errores.SinPermisos");
        }

        private static void ValidarAlta(AltaOficialComisionRequest_83KI request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (request.IdComision <= 0) throw new ArgumentException("Errores.ComisionObligatoria");
            if (request.FechaAlta == default(DateTime)) throw new ArgumentException("Errores.EstadoDefinitivoFechaObligatoria");
            if (string.IsNullOrWhiteSpace(request.NumeroActa)) throw new ArgumentException("Errores.EstadoDefinitivoNumeroActaObligatorio");
        }

        private static void ValidarCierre(ActaCierreComisionRequest_83KI request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (request.IdComision <= 0) throw new ArgumentException("Errores.ComisionObligatoria");
            if (request.FechaCierre == default(DateTime)) throw new ArgumentException("Errores.EstadoDefinitivoFechaObligatoria");
            if (string.IsNullOrWhiteSpace(request.NumeroActa)) throw new ArgumentException("Errores.EstadoDefinitivoNumeroActaObligatorio");
            if (string.IsNullOrWhiteSpace(request.Motivo)) throw new ArgumentException("Errores.EstadoDefinitivoMotivoObligatorio");
        }

        private static ResultadoEstadoDefinitivoComision_83KI Ejecutar(Func<ResultadoEstadoDefinitivoComision_83KI> action)
        {
            try { return action(); }
            catch (SqlException ex) { throw new InvalidOperationException(TraducirSqlException(ex), ex); }
        }

        private static string TraducirSqlException(SqlException ex)
        {
            foreach (SqlError error in ex.Errors)
            {
                string mensaje = error.Message ?? string.Empty;
                if ((error.Number == 2601 || error.Number == 2627)
                    && (mensaje.IndexOf("UQ_AltaOficialComision_NumeroActa", StringComparison.OrdinalIgnoreCase) >= 0
                        || mensaje.IndexOf("UQ_ActaCierreComision_NumeroActa", StringComparison.OrdinalIgnoreCase) >= 0))
                {
                    return "Errores.EstadoDefinitivoNumeroActaDuplicado";
                }

                if (mensaje.StartsWith("Errores.", StringComparison.OrdinalIgnoreCase))
                {
                    return mensaje;
                }
            }

            return ex.Message;
        }

        private bool RegistrarAuditoriaSegura(string evento, ResultadoEstadoDefinitivoComision_83KI resultado)
        {
            try
            {
                string descripcion = string.Format("{0}: comisión {1}, estado {2}, acta {3}, vacantes regularizadas {4}/{5}.", evento, resultado.CodigoComision, resultado.EstadoDefinitivo, resultado.NumeroActa, resultado.VacantesRegularizadas, resultado.CupoMinimo);
                _bitacora.RegistrarEvento(BitacoraEvento_83KI.CrearNuevo(descripcion, Criticidad.Medio, Modulo.PlanificacionAcademica, UsuarioActual));
                return true;
            }
            catch { return false; }
        }

        private string UsuarioActual { get { return _sessionManager.UsuarioActivo != null ? _sessionManager.UsuarioActivo.UserName : "Sistema"; } }
    }
}

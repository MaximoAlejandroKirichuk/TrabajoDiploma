using BE.Entidades;
using Service.DTOs;
using Service.Entidades;
using Service.Interfaces;
using System;
using System.Collections.Generic;

namespace BLL
{
    public class KI68GestorBecaBLL : IGestorBeca_83KI
    {
        private readonly IBecaDAL_83KI _becaDAL;
        private readonly IInscripcionDAL_83KI _inscripcionDAL;
        private readonly ISessionManager_83KI _sessionManager;
        private readonly IBitacoraManager_83KI _bitacora;

        public KI68GestorBecaBLL(IBecaDAL_83KI becaDAL, IInscripcionDAL_83KI inscripcionDAL, ISessionManager_83KI sessionManager, IBitacoraManager_83KI bitacora)
        {
            _becaDAL = becaDAL;
            _inscripcionDAL = inscripcionDAL;
            _sessionManager = sessionManager;
            _bitacora = bitacora;
        }

        public IEnumerable<ComisionListado_83KI> ListarComisionesElegibles()
        {
            ValidarPermiso();
            return _inscripcionDAL.ListarComisionesElegibles();
        }

        public BusquedaDecisionBecaResultado_83KI BuscarAlumnoConInscripcion(string dni, string codigoComision)
        {
            ValidarPermiso();
            if (string.IsNullOrWhiteSpace(dni)) throw new ArgumentException("Errores.BecaDniObligatorio");
            if (string.IsNullOrWhiteSpace(codigoComision)) throw new ArgumentException("Errores.BecaCodigoComisionObligatorio");

            return _becaDAL.BuscarSolicitudPorDniComision(dni, codigoComision);
        }

        public DecisionBecaResultado_83KI RegistrarDecisionBeca(DecisionBecaRequest_83KI request)
        {
            ValidarPermiso();
            ValidarDatosDecision(request);
            var busqueda = ValidarAlumnoInscripcion(request.DNI, request.CodigoComision);
            var beca = CrearBeca(busqueda, request);

            var registrada = RegistrarBeca(beca);
            bool auditoriaFallida = !RegistrarAuditoriaSegura(registrada, busqueda, UsuarioActual);

            return new DecisionBecaResultado_83KI
            {
                Beca = registrada,
                Alumno = busqueda.Alumno,
                Solicitud = busqueda.Solicitud,
                CodigoComision = busqueda.CodigoComision,
                CodigoSolicitud = busqueda.Solicitud.Codigo,
                AuditoriaFallida = auditoriaFallida
            };
        }

        private void ValidarPermiso()
        {
            if (!_sessionManager.TienePermiso(PermisoSistema_83KI.RegistrarDecisionBeca))
                throw new InvalidOperationException("Errores.SinPermisos");
        }

        private static void ValidarDatosDecision(DecisionBecaRequest_83KI request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (string.IsNullOrWhiteSpace(request.DNI)) throw new ArgumentException("Errores.BecaDniObligatorio");
            if (string.IsNullOrWhiteSpace(request.CodigoComision)) throw new ArgumentException("Errores.BecaCodigoComisionObligatorio");
            if (string.IsNullOrWhiteSpace(request.TipoBeneficio)) throw new ArgumentException("Errores.BecaTipoBeneficioObligatorio");
            if (request.FechaSolicitud == default(DateTime)) throw new ArgumentException("Errores.BecaFechaSolicitudObligatoria");
            if (!string.Equals(request.EstadoBeca, Beca_83KI.EstadoAprobada, StringComparison.OrdinalIgnoreCase)
                && !string.Equals(request.EstadoBeca, Beca_83KI.EstadoDenegada, StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Errores.BecaEstadoInvalido");
            if (string.IsNullOrWhiteSpace(request.MotivoDecision)) throw new ArgumentException("Errores.BecaMotivoObligatorio");
        }

        private BusquedaDecisionBecaResultado_83KI ValidarAlumnoInscripcion(string dni, string codigoComision)
        {
            var busqueda = _becaDAL.BuscarSolicitudPorDniComision(dni, codigoComision);
            if (busqueda == null || busqueda.Alumno == null || busqueda.Solicitud == null)
                throw new InvalidOperationException("Errores.BecaSinInscripcionValida");

            if (_becaDAL.ExisteBecaParaInscripcion(dni, codigoComision))
                throw new InvalidOperationException("Errores.BecaDuplicada");

            return busqueda;
        }

        private static Beca_83KI CrearBeca(BusquedaDecisionBecaResultado_83KI busqueda, DecisionBecaRequest_83KI request)
        {
            return Beca_83KI.Crear(
                busqueda.IdAlumno,
                busqueda.IdComision,
                busqueda.IdSolicitudInscripcion,
                request.TipoBeneficio,
                request.FechaSolicitud,
                request.EstadoBeca,
                request.MotivoDecision);
        }

        private Beca_83KI RegistrarBeca(Beca_83KI beca)
        {
            return _becaDAL.RegistrarBeca(beca);
        }

        private bool RegistrarAuditoriaSegura(Beca_83KI beca, BusquedaDecisionBecaResultado_83KI busqueda, string username)
        {
            try
            {
                string descripcion = string.Format("Registro de decisión de beca: inscripción {0}, alumno {1}, comisión {2}, estado {3}.", busqueda.Solicitud.Codigo, busqueda.Alumno.IdAlumno, busqueda.CodigoComision, beca.EstadoBeca);
                _bitacora.RegistrarEvento(BitacoraEvento_83KI.CrearNuevo(descripcion, Criticidad.Medio, Modulo.PreInscripcion, username));
                return true;
            }
            catch { return false; }
        }

        private string UsuarioActual { get { return _sessionManager.UsuarioActivo != null ? _sessionManager.UsuarioActivo.UserName : "Sistema"; } }
    }
}

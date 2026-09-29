using BE.Entidades;
using Service.DTOs;
using Service.Entidades;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BLL
{
    public class KI68InscripcionBLL : IInscripcionBLL_83KI
    {
        private readonly IInscripcionDAL_83KI _inscripcionDAL;
        private readonly IComisionDAL_83KI _comisionDAL;
        private readonly ICuotaBLL_83KI _cuotaBLL;
        private readonly ISessionManager_83KI _sessionManager;
        private readonly IBitacoraManager_83KI _bitacora;

        public KI68InscripcionBLL(IInscripcionDAL_83KI inscripcionDAL, IComisionDAL_83KI comisionDAL, ICuotaBLL_83KI cuotaBLL, ISessionManager_83KI sessionManager, IBitacoraManager_83KI bitacora)
        {
            _inscripcionDAL = inscripcionDAL;
            _comisionDAL = comisionDAL;
            _cuotaBLL = cuotaBLL;
            _sessionManager = sessionManager;
            _bitacora = bitacora;
        }

        public IEnumerable<BusquedaInscripcionPersonaResultado_83KI> BuscarPersonas(string texto)
        {
            ValidarPermiso();
            if (string.IsNullOrWhiteSpace(texto)) throw new ArgumentException("Errores.BusquedaInscripcionObligatoria");
            return _inscripcionDAL.BuscarPersonas(texto);
        }

        public IEnumerable<ComisionListado_83KI> ListarComisionesElegibles()
        {
            ValidarPermiso();
            return _inscripcionDAL.ListarComisionesElegibles();
        }

        public IEnumerable<PlanDePago_83KI> ObtenerPlanesComision(int idComision)
        {
            ValidarPermiso();
            if (idComision <= 0) throw new ArgumentException("Errores.ComisionObligatoria");
            return _inscripcionDAL.ObtenerPlanesComision(idComision);
        }

        public CuotaPreview_83KI PrevisualizarCuotaInicial(int idComision)
        {
            ValidarPermiso();
            var comision = _comisionDAL.ObtenerComision(idComision);
            if (comision == null) throw new InvalidOperationException("Errores.ComisionNoEncontrada");
            return _cuotaBLL.PrevisualizarCuotaInicial(comision, DateTime.Now);
        }

        public SolicitudInscripcionResultado_83KI RegistrarSolicitud(SolicitudInscripcionRequest_83KI request)
        {
            ValidarPermiso();
            ValidarRequest(request);
            ValidarPlanComision(request.IdComision, request.IdPlanDePago);

            request.AuditoriaUsername = UsuarioActual;
            return _inscripcionDAL.RegistrarSolicitud(request);
        }

        private void ValidarPermiso()
        {
            if (!_sessionManager.TienePermiso(PermisoSistema_83KI.RegistrarSolicitudInscripcion))
                throw new InvalidOperationException("Errores.SinPermisos");
        }

        private static void ValidarRequest(SolicitudInscripcionRequest_83KI request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (string.IsNullOrWhiteSpace(request.DNI)) throw new ArgumentException("Errores.AlumnoDniObligatorio");
            if (string.IsNullOrWhiteSpace(request.Nombre)) throw new ArgumentException("Errores.AlumnoNombreObligatorio");
            if (string.IsNullOrWhiteSpace(request.Apellido)) throw new ArgumentException("Errores.AlumnoApellidoObligatorio");
            if (string.IsNullOrWhiteSpace(request.Email) || !request.Email.Contains("@")) throw new ArgumentException("Errores.AlumnoEmailInvalido");
            if (string.IsNullOrWhiteSpace(request.Telefono)) throw new ArgumentException("Errores.AlumnoTelefonoObligatorio");
            if (request.IdComision <= 0) throw new ArgumentException("Errores.ComisionObligatoria");
            if (request.IdPlanDePago <= 0) throw new ArgumentException("Errores.PlanDePagoObligatorio");
        }

        private void ValidarPlanComision(int idComision, int idPlanDePago)
        {
            bool existe = _inscripcionDAL.ObtenerPlanesComision(idComision).Any(p => p.IdPlanDePago == idPlanDePago && p.EstadoActivo);
            if (!existe) throw new InvalidOperationException("Errores.PlanDePagoNoDisponible");
        }

        private string UsuarioActual { get { return _sessionManager.UsuarioActivo != null ? _sessionManager.UsuarioActivo.UserName : "Sistema"; } }
    }
}

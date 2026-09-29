using Service.DTOs;
using Service.Entidades;
using Service.Interfaces;
using System;

namespace BLL
{
    public class KI68LeadBLL : ILeadBLL_83KI
    {
        private readonly IAlumnoPersonaBLL_83KI _alumnoPersonaBLL;
        private readonly IComisionConsultaBLL_83KI _comisionBLL;
        private readonly IConsultaLeadDAL_83KI _consultaLeadDAL;
        private readonly ISessionManager_83KI _sessionManager;
        private readonly IBitacoraManager_83KI _bitacora;

        public KI68LeadBLL(IAlumnoPersonaBLL_83KI alumnoPersonaBLL, IComisionConsultaBLL_83KI comisionBLL, IConsultaLeadDAL_83KI consultaLeadDAL, ISessionManager_83KI sessionManager, IBitacoraManager_83KI bitacora)
        {
            _alumnoPersonaBLL = alumnoPersonaBLL;
            _comisionBLL = comisionBLL;
            _consultaLeadDAL = consultaLeadDAL;
            _sessionManager = sessionManager;
            _bitacora = bitacora;
        }

        public BusquedaLeadResultado_83KI BuscarLead(string dni, string email, string telefono)
        {
            ValidarPermiso();
            return _alumnoPersonaBLL.BuscarLead(dni, email, telefono);
        }

        public ConsultaLeadResultado_83KI RegistrarConsulta(ConsultaLeadRequest_83KI request)
        {
            ValidarPermiso();
            ValidarRequest(request);
            _alumnoPersonaBLL.ResolverLeadUnico(request);
            _comisionBLL.ValidarComisionPreapertura(request.IdComision);
            var resultado = _consultaLeadDAL.RegistrarConsulta(request);
            RegistrarAuditoriaSegura($"Registro de consulta lead: {resultado.Consulta.Codigo}. Lead {resultado.Lead.IdLead}. Actor: {UsuarioActual}", Criticidad.Medio, UsuarioActual);
            return resultado;
        }

        private void ValidarPermiso()
        {
            if (!_sessionManager.TienePermiso(PermisoSistema_83KI.RegistrarConsultaLead))
                throw new InvalidOperationException("Errores.SinPermisos");
        }

        private static void ValidarRequest(ConsultaLeadRequest_83KI request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (string.IsNullOrWhiteSpace(request.Nombre)) throw new ArgumentException("Errores.LeadNombreObligatorio");
            if (string.IsNullOrWhiteSpace(request.Apellido)) throw new ArgumentException("Errores.LeadApellidoObligatorio");
            if (string.IsNullOrWhiteSpace(request.Email) || !request.Email.Contains("@")) throw new ArgumentException("Errores.LeadEmailInvalido");
            if (string.IsNullOrWhiteSpace(request.Telefono)) throw new ArgumentException("Errores.LeadTelefonoObligatorio");
            if (request.IdComision <= 0) throw new ArgumentException("Errores.ComisionObligatoria");
            if (string.IsNullOrWhiteSpace(request.MedioContacto)) throw new ArgumentException("Errores.MedioContactoObligatorio");
            if (string.IsNullOrWhiteSpace(request.Motivo)) throw new ArgumentException("Errores.MotivoConsultaObligatorio");
        }

        private string UsuarioActual { get { return _sessionManager.UsuarioActivo != null ? _sessionManager.UsuarioActivo.UserName : "Sistema"; } }

        private void RegistrarAuditoriaSegura(string descripcion, Criticidad criticidad, string username)
        {
            try { _bitacora.RegistrarEvento(BitacoraEvento_83KI.CrearNuevo(descripcion, criticidad, Modulo.PreInscripcion, username)); }
            catch { }
        }
    }
}

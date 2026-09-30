using BE.Entidades;
using Service.DTOs;
using Service.Entidades;
using Service.Interfaces;
using System;
using System.Linq;

namespace BLL
{
    public class KI68PagoInscripcionBLL : IPagoInscripcionBLL_83KI
    {
        private readonly IPagoInscripcionDAL_83KI _pagoDAL;
        private readonly ISessionManager_83KI _sessionManager;
        private readonly IBitacoraManager_83KI _bitacora;

        public KI68PagoInscripcionBLL(IPagoInscripcionDAL_83KI pagoDAL, ISessionManager_83KI sessionManager, IBitacoraManager_83KI bitacora)
        {
            _pagoDAL = pagoDAL;
            _sessionManager = sessionManager;
            _bitacora = bitacora;
        }

        public BusquedaPagoInscripcionResultado_83KI BuscarCuotasPendientes(string dni)
        {
            ValidarPermiso();
            if (string.IsNullOrWhiteSpace(dni)) throw new ArgumentException("Errores.PagoDniObligatorio");

            return _pagoDAL.BuscarCuotasPendientesPorDni(dni);
        }

        public PagoInscripcionResultado_83KI RegistrarPagoInscripcion(PagoInscripcionRequest_83KI request)
        {
            ValidarPermiso();
            ValidarDatosPago(request);

            var busqueda = ValidarCuota(request);
            var cuota = busqueda.CuotasPendientes.First(c => c.IdCuota == request.IdCuota);

            if (_pagoDAL.ExisteReferenciaPago(request.NumeroReferencia))
                throw new InvalidOperationException("Errores.PagoReferenciaDuplicada");

            var pago = PagoInscripcion_83KI.Crear(
                busqueda.Alumno.IdAlumno,
                busqueda.Solicitud.IdSolicitudInscripcion,
                request.IdCuota,
                request.MetodoPago,
                request.MontoRecibido,
                request.NumeroReferencia,
                request.FechaPago);

            var resultado = _pagoDAL.RegistrarPago(pago);
            bool auditoriaFallida = !RegistrarAuditoriaSegura(resultado);
            resultado.AuditoriaFallida = auditoriaFallida;

            return resultado;
        }

        private void ValidarPermiso()
        {
            if (!_sessionManager.TienePermiso(PermisoSistema_83KI.RegistrarPagoInscripcion))
                throw new InvalidOperationException("Errores.SinPermisos");
        }

        private static void ValidarDatosPago(PagoInscripcionRequest_83KI request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (string.IsNullOrWhiteSpace(request.DNI)) throw new ArgumentException("Errores.PagoDniObligatorio");
            if (request.IdCuota <= 0) throw new ArgumentException("Errores.PagoCuotaObligatoria");
            if (request.MontoRecibido <= 0) throw new ArgumentException("Errores.MontoMatriculaInvalido");
            if (string.IsNullOrWhiteSpace(request.MetodoPago) || !PagoInscripcion_83KI.MetodoValido(request.MetodoPago))
                throw new ArgumentException("Errores.PagoMetodoInvalido");
            if (string.IsNullOrWhiteSpace(request.NumeroReferencia)) throw new ArgumentException("Errores.PagoReferenciaObligatoria");
            if (request.FechaPago == default(DateTime)) throw new ArgumentException("Errores.PagoFechaObligatoria");
        }

        private BusquedaPagoInscripcionResultado_83KI ValidarCuota(PagoInscripcionRequest_83KI request)
        {
            var busqueda = _pagoDAL.BuscarCuotasPendientesPorDni(request.DNI);
            if (busqueda == null || busqueda.Alumno == null || busqueda.Solicitud == null || busqueda.CuotasPendientes == null || busqueda.CuotasPendientes.Count == 0)
                throw new InvalidOperationException("Errores.PagoCuotaNoPendiente");

            var cuota = busqueda.CuotasPendientes.FirstOrDefault(c => c.IdCuota == request.IdCuota);
            if (cuota == null) throw new InvalidOperationException("Errores.PagoCuotaObligatoria");
            if (request.MontoRecibido != cuota.BalanceAdeudado) throw new ArgumentException("Errores.PagoMontoNoCoincide");

            return busqueda;
        }

        private bool RegistrarAuditoriaSegura(PagoInscripcionResultado_83KI resultado)
        {
            try
            {
                string descripcion = string.Format("Registro de pago de inscripción: inscripción {0}, alumno {1}, comisión {2}, cuota {3}, referencia {4}.",
                    resultado.CodigoSolicitud, resultado.Alumno.IdAlumno, resultado.CodigoComision, resultado.Cuota.IdCuota, resultado.Pago.NumeroReferencia);
                _bitacora.RegistrarEvento(BitacoraEvento_83KI.CrearNuevo(descripcion, Criticidad.Medio, Modulo.PreInscripcion, UsuarioActual));
                return true;
            }
            catch { return false; }
        }

        private string UsuarioActual { get { return _sessionManager.UsuarioActivo != null ? _sessionManager.UsuarioActivo.UserName : "Sistema"; } }
    }
}

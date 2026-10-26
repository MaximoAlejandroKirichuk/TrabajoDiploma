using BE.Entidades;
using BE.DTOs.Pagos;
using Service.DTOs;
using BE.DTOs.Comisiones;
using Service.Entidades;
using Service.Interfaces;
using System;
using System.Linq;

namespace BLL
{
    public class KI68PagoCuotaBLL_83KI : IPagoCuotaBLL_83KI
    {
        private readonly IPagoCuotaDAL_83KI _pagoDAL;
        private readonly ISessionManager_83KI _sessionManager;
        private readonly IBitacoraManager_83KI _bitacora;

        public KI68PagoCuotaBLL_83KI(IPagoCuotaDAL_83KI pagoDAL, ISessionManager_83KI sessionManager, IBitacoraManager_83KI bitacora)
        {
            _pagoDAL = pagoDAL;
            _sessionManager = sessionManager;
            _bitacora = bitacora;
        }

        public System.Collections.Generic.IEnumerable<ComisionPagoOpcion_83KI> ListarComisionesConCuotasPendientes(string dni)
        {
            ValidarPermiso();
            if (string.IsNullOrWhiteSpace(dni)) throw new ArgumentException("Errores.PagoDniObligatorio");
            return _pagoDAL.ListarComisionesConCuotasPendientes(dni);
        }

        public BusquedaPagoInscripcionResultado_83KI BuscarCuotasPendientes(string dni, string codigoComision)
        {
            ValidarPermiso();
            if (string.IsNullOrWhiteSpace(dni)) throw new ArgumentException("Errores.PagoDniObligatorio");
            if (string.IsNullOrWhiteSpace(codigoComision)) throw new ArgumentException("Errores.PagoComisionObligatoria");
            return _pagoDAL.BuscarCuotasPendientes(dni, codigoComision);
        }

        public PagoInscripcionResultado_83KI RegistrarPagoCuota(PagoInscripcionRequest_83KI request)
        {
            ValidarPermiso();
            ValidarDatosPago(request);
            var busqueda = _pagoDAL.BuscarCuotasPendientes(request.DNI, request.CodigoComision);
            if (busqueda == null || busqueda.Alumno == null || busqueda.Solicitud == null || busqueda.CuotasPendientes == null)
                throw new InvalidOperationException("Errores.PagoCuotaNoPendiente");
            if (!busqueda.MatriculaPagada) throw new InvalidOperationException("Errores.PagoMatriculaPendiente");
            var cuota = busqueda.CuotasPendientes.FirstOrDefault(c => c.IdCuota == request.IdCuota);
            if (cuota == null) throw new InvalidOperationException("Errores.PagoCuotaObligatoria");
            if (cuota.EsMatricula) throw new InvalidOperationException("Errores.PagoCuotaNoPendiente");
            if (busqueda.Cuotas.Any(c => !c.EsMatricula && c.NumeroCuota < cuota.NumeroCuota && c.BalanceAdeudado > 0))
                throw new InvalidOperationException("Errores.PagoCuotaAnteriorPendiente");
            if (request.MontoRecibido != cuota.BalanceAdeudado) throw new ArgumentException("Errores.PagoMontoNoCoincide");
            if (_pagoDAL.ExisteReferenciaPago(request.NumeroReferencia)) throw new InvalidOperationException("Errores.PagoReferenciaDuplicada");

            var pago = PagoInscripcion_83KI.Crear(busqueda.Alumno.IdAlumno, busqueda.Solicitud.IdSolicitudInscripcion, request.IdCuota,
                request.MetodoPago, request.MontoRecibido, request.NumeroReferencia, request.FechaPago, request.FechaAcreditacion, request.CodigoComision);
            var resultado = _pagoDAL.RegistrarPago(pago);
            try
            {
                string descripcion = string.Format("Registro de pago de cuota: inscripción {0}, alumno {1}, comisión {2}, cuota {3}, referencia {4}.",
                    resultado.CodigoSolicitud, resultado.Alumno.IdAlumno, resultado.CodigoComision, resultado.Cuota.IdCuota, resultado.Pago.NumeroReferencia);
                _bitacora.RegistrarEvento(BitacoraEvento_83KI.CrearNuevo(descripcion, Criticidad.Medio, Modulo.CobrosMorosidadActas,
                    _sessionManager.UsuarioActivo != null ? _sessionManager.UsuarioActivo.UserName : "Sistema"));
                resultado.AuditoriaFallida = false;
            }
            catch { resultado.AuditoriaFallida = true; }
            return resultado;
        }

        private void ValidarPermiso()
        {
            if (!_sessionManager.TienePermiso(PermisoSistema_83KI.RegistrarPagoCuotas))
                throw new InvalidOperationException("Errores.SinPermisos");
        }

        private static void ValidarDatosPago(PagoInscripcionRequest_83KI request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (string.IsNullOrWhiteSpace(request.DNI)) throw new ArgumentException("Errores.PagoDniObligatorio");
            if (string.IsNullOrWhiteSpace(request.CodigoComision)) throw new ArgumentException("Errores.PagoComisionObligatoria");
            if (request.IdCuota <= 0) throw new ArgumentException("Errores.PagoCuotaObligatoria");
            if (request.MontoRecibido <= 0) throw new ArgumentException("Errores.MontoMatriculaInvalido");
            if (string.IsNullOrWhiteSpace(request.MetodoPago) || !PagoInscripcion_83KI.MetodoValido(request.MetodoPago)) throw new ArgumentException("Errores.PagoMetodoInvalido");
            if (string.IsNullOrWhiteSpace(request.NumeroReferencia)) throw new ArgumentException("Errores.PagoReferenciaObligatoria");
            if (request.FechaPago == default(DateTime)) throw new ArgumentException("Errores.PagoFechaObligatoria");
        }
    }
}

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
        private readonly IInscripcionBLL_83KI _inscripcionBLL;
        private readonly ICuotaBLL_83KI _cuotaBLL;

        public KI68PagoCuotaBLL_83KI(IPagoCuotaDAL_83KI pagoDAL, ISessionManager_83KI sessionManager, IBitacoraManager_83KI bitacora,
            IInscripcionBLL_83KI inscripcionBLL, ICuotaBLL_83KI cuotaBLL)
        {
            _pagoDAL = pagoDAL;
            _sessionManager = sessionManager;
            _bitacora = bitacora;
            _inscripcionBLL = inscripcionBLL ?? throw new ArgumentNullException(nameof(inscripcionBLL));
            _cuotaBLL = cuotaBLL ?? throw new ArgumentNullException(nameof(cuotaBLL));
        }

        public System.Collections.Generic.IEnumerable<ComisionPagoOpcion_83KI> ListarComisionesConCuotasPendientes(string dni)
        {
            ValidarPermiso();
            if (string.IsNullOrWhiteSpace(dni)) throw new ArgumentException("Errores.PagoDniObligatorio");
            return _pagoDAL.ListarComisionesConCuotasPendientes(dni);
        }

        public BusquedaCuotasPagoResultado_83KI BuscarCuotasPendientes(string dni, string codigoComision)
        {
            ValidarPermiso();
            if (string.IsNullOrWhiteSpace(dni)) throw new ArgumentException("Errores.PagoDniObligatorio");
            if (string.IsNullOrWhiteSpace(codigoComision)) throw new ArgumentException("Errores.PagoComisionObligatoria");
            var solicitud = _inscripcionBLL.ObtenerPorDniYComision(dni, codigoComision);
            if (solicitud == null) return null;
            var busqueda = _pagoDAL.BuscarCuotasPendientes(dni, codigoComision);
            ValidarSolicitudBusqueda(solicitud, busqueda);
            return busqueda;
        }

        public PagoCuotaResultado_83KI RegistrarPagoCuota(PagoCuotaRequest_83KI request)
        {
            ValidarPermiso();
            ValidarDatosPago(request);
            var busqueda = BuscarCuotasPendientes(request.DNI, request.CodigoComision);
            if (busqueda == null || busqueda.Alumno == null || busqueda.Solicitud == null || busqueda.CuotasPendientes == null)
                throw new InvalidOperationException("Errores.PagoCuotaNoPendiente");
            if (!busqueda.MatriculaPagada) throw new InvalidOperationException("Errores.PagoMatriculaPendiente");
            var cuota = busqueda.CuotasPendientes.FirstOrDefault(c => c.IdCuota == request.IdCuota);
            if (cuota == null) throw new InvalidOperationException("Errores.PagoCuotaObligatoria");
            if (busqueda.Cuotas.Any(c => !c.EsMatricula && c.NumeroCuota < cuota.NumeroCuota && c.BalanceAdeudado > 0))
                throw new InvalidOperationException("Errores.PagoCuotaAnteriorPendiente");
            _cuotaBLL.ValidarPagoCompleto(cuota, request.MontoRecibido);
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

        private static void ValidarSolicitudBusqueda(SolicitudInscripcion_83KI solicitud, BusquedaCuotasPagoResultado_83KI busqueda)
        {
            if (busqueda == null) return;
            if (busqueda.Solicitud == null || busqueda.Solicitud.IdSolicitudInscripcion != solicitud.IdSolicitudInscripcion
                || busqueda.Alumno == null || busqueda.Alumno.IdAlumno != solicitud.IdAlumno
                || (busqueda.Cuotas != null && busqueda.Cuotas.Any(c => c.IdSolicitudInscripcion != solicitud.IdSolicitudInscripcion))
                || (busqueda.CuotasPendientes != null && busqueda.CuotasPendientes.Any(c => c.IdSolicitudInscripcion != solicitud.IdSolicitudInscripcion)))
                throw new InvalidOperationException("Errores.PagoSolicitudesAmbiguas");
        }

        private void ValidarPermiso()
        {
            if (!_sessionManager.TienePermiso(PermisoSistema_83KI.RegistrarPagoCuotas))
                throw new InvalidOperationException("Errores.SinPermisos");
        }

        private static void ValidarDatosPago(PagoCuotaRequest_83KI request)
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

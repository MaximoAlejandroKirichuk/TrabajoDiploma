using BE.Entidades;
using Service.DTOs;
using Service.Entidades;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace BLL
{
    public sealed class KI68EvaluacionesBLL
    {
        private readonly IEvaluacionesDAL_83KI _dal;
        private readonly ISessionManager_83KI _session;
        private readonly IBitacoraManager_83KI _audit;
        private EvaluacionConsulta_83KI _original;

        public KI68EvaluacionesBLL(IEvaluacionesDAL_83KI dal, ISessionManager_83KI session, IBitacoraManager_83KI audit)
        { _dal = dal ?? throw new ArgumentNullException(nameof(dal)); _session = session ?? throw new ArgumentNullException(nameof(session)); _audit = audit ?? throw new ArgumentNullException(nameof(audit)); }

        private void Autorizar(bool modificar = false)
        {
            var u = _session.UsuarioActivo;
            if (u == null || !u.Activo || u.Bloqueado || _session.RequiereRecuperacionIntegridad
                || !_session.TienePermiso(PermisoSistema_83KI.VerEvaluaciones)
                || (modificar && !_session.TienePermiso(PermisoSistema_83KI.ModificarEvaluaciones)))
                throw new InvalidOperationException("Errores.SinPermisos");
        }
        public bool PuedeModificar()
        {
            Autorizar();
            return _session.TienePermiso(PermisoSistema_83KI.ModificarEvaluaciones);
        }
        public List<ComisionListado_83KI> ListarComisionesConResultados() { Autorizar(); return _dal.ListarComisionesConResultados(); }
        public List<EvaluacionConsulta_83KI> Buscar(int idComision)
        {
            Cancelar(); Autorizar();
            if (idComision <= 0) throw new InvalidOperationException("Evaluaciones.Seleccionar");
            return _dal.BuscarEvaluaciones(idComision);
        }
        public EvaluacionConsulta_83KI Seleccionar(int idResultado)
        {
            Cancelar(); Autorizar();
            _original = _dal.ObtenerEvaluacion(idResultado);
            if (_original == null) throw new InvalidOperationException("Evaluaciones.NoExiste");
            return _original;
        }
        public void Cancelar() { _original = null; }

        public ModificarEvaluacionResultado_83KI Modificar(ModificarEvaluacionRequest_83KI r)
        {
            Autorizar(true);
            var original = _original;
            if (original == null || r == null) throw new InvalidOperationException("Evaluaciones.Seleccionar");
            var cambios = new ModificarEvaluacionRequest_83KI { Calificacion = r.Calificacion, FechaEvaluacion = r.FechaEvaluacion.Date, Observaciones = r.Observaciones ?? "" };
            if (cambios.Calificacion < 1 || cambios.Calificacion > 10 || decimal.Round(cambios.Calificacion, 2) != cambios.Calificacion)
                throw new InvalidOperationException("Cun08.CalificacionInvalida");
            if (cambios.Observaciones.Length > 500) throw new InvalidOperationException("Cun08.ObservacionesLargas");
            if (cambios.FechaEvaluacion < original.Inicio || cambios.FechaEvaluacion > original.Fin)
                throw new InvalidOperationException("Cun08.FechaInvalida");
            // La fecha actual, el certificado y los permisos se revalidan en SQL, nunca con el reloj del cliente.
            string actor = _session.UsuarioActivo.UserName;
            var result = new ModificarEvaluacionResultado_83KI { Modificado = _dal.ModificarEvaluacion(original, cambios) };
            Cancelar();
            if (!result.Modificado) return result;
            try
            {
                _audit.RegistrarEvento(BitacoraEvento_83KI.CrearNuevo(string.Format(CultureInfo.InvariantCulture,
                    "Evaluaciones: evaluación {0}; inscripción {1}; comisión {2}; nota {3:0.00} -> {4:0.00}; fecha {5:yyyy-MM-dd} -> {6:yyyy-MM-dd}; observaciones {7} -> {8}",
                    original.IdResultado, original.IdInscripcion, original.IdComision, original.Calificacion, cambios.Calificacion,
                    original.FechaEvaluacion, cambios.FechaEvaluacion, ResumenObservaciones(original.Observaciones), ResumenObservaciones(cambios.Observaciones)),
                    Criticidad.Medio, Modulo.PlanificacionAcademica, actor));
            }
            catch { result.AuditoriaFallida = true; } // Los datos ya están confirmados; no invitar a repetir la modificación.
            return result;
        }

        private static string ResumenObservaciones(string texto)
        {
            // CUN08 no copia notas personales a bitácora. Registrar evidencia del antes/después sin su contenido.
            using (var sha = SHA256.Create())
                return texto.Length.ToString(CultureInfo.InvariantCulture) + ":SHA256=" + BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(texto))).Replace("-", "");
        }
    }
}

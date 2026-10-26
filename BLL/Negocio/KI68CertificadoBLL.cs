using BE.Entidades;
using BE.DTOs.Certificaciones;
using Service.Entidades;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Globalization;
using System.Text;

namespace BLL
{
    public sealed class KI68CertificadoBLL
    {
        public const string CargoResponsable = "Coordinadora Academica";
        private readonly ICertificadoDAL_83KI _dal;
        private readonly ISessionManager_83KI _session;
        private readonly IBitacoraManager_83KI _audit;
        private sealed class EstadoRetencion
        {
            public string MotivosVistos;
            public bool Registrada;
        }
        private readonly Dictionary<int, EstadoRetencion> _retenciones = new Dictionary<int, EstadoRetencion>();
        private readonly Dictionary<int, string> _prefijosVistos = new Dictionary<int, string>();
        public KI68CertificadoBLL(ICertificadoDAL_83KI dal, ISessionManager_83KI session, IBitacoraManager_83KI audit)
        { _dal = dal ?? throw new ArgumentNullException(nameof(dal)); _session = session ?? throw new ArgumentNullException(nameof(session)); _audit = audit ?? throw new ArgumentNullException(nameof(audit)); }

        private void Autorizar()
        {
            var u = _session.UsuarioActivo;
            if (u == null || !u.Activo || u.Bloqueado || _session.RequiereRecuperacionIntegridad ||
                !_session.TienePermiso(PermisoSistema_83KI.GenerarCertificadoFinalizacion))
                throw new InvalidOperationException("Errores.SinPermisos");
        }
        public List<Comision_83KI> ListarFinalizadas() { Autorizar(); return _dal.ListarFinalizadas(); }
        public List<AlumnoCertificacion_83KI> Buscar(int idComision)
        { Autorizar(); if (idComision <= 0) throw new InvalidOperationException("Cun09.Seleccionar"); return _dal.Buscar(idComision); }
        public AlumnoCertificacion_83KI Seleccionar(int id)
        {
            Autorizar();
            var a = _dal.RevalidarEnTransaccion(id, actual => { ValidarEvaluaciones(actual); return null; });
            EstadoRetencion estado;
            if (!_retenciones.TryGetValue(id, out estado))
                _retenciones[id] = estado = new EstadoRetencion();
            estado.MotivosVistos = a.Motivos; // Volver a seleccionar no debe reiniciar una retención ya registrada en bitácora.
            if (a.Certificado == null && a.Aprobado && a.Balance == 0) _prefijosVistos[id] = PrefijoFolio(a);
            return a;
        }
        public static string PrefijoFolio(AlumnoCertificacion_83KI a)
        {
            if (a == null || string.IsNullOrWhiteSpace(a.CodigoComision) || a.FechaFinComision == default(DateTime))
                throw new InvalidOperationException("Cun09.ContextoFolio");
            var curso = new StringBuilder();
            bool separador = false;
            foreach (char c in (a.Curso ?? "").Normalize(NormalizationForm.FormD).ToUpperInvariant())
            {
                var categoria = CharUnicodeInfo.GetUnicodeCategory(c);
                if (categoria == UnicodeCategory.NonSpacingMark || categoria == UnicodeCategory.SpacingCombiningMark || categoria == UnicodeCategory.EnclosingMark) continue;
                if ((c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9'))
                {
                    if (separador && curso.Length > 0) curso.Append('-');
                    curso.Append(c); separador = false;
                }
                else separador = true;
            }
            if (curso.Length == 0) throw new InvalidOperationException("Cun09.ContextoFolio");
            return a.CodigoComision + "-" + curso + "-" + a.FechaFinComision.ToString("yyyyMMdd", CultureInfo.InvariantCulture) + "-";
        }
        private static long ValidarNumero(string texto)
        {
            long numero;
            if (string.IsNullOrEmpty(texto)) throw new InvalidOperationException("Cun09.NumeroFolioInvalido");
            foreach (char c in texto) if (c < '0' || c > '9') throw new InvalidOperationException("Cun09.NumeroFolioInvalido");
            if (!long.TryParse(texto, NumberStyles.None, CultureInfo.InvariantCulture, out numero) || numero <= 0)
                throw new InvalidOperationException("Cun09.NumeroFolioInvalido");
            return numero;
        }
        public static string ComponerFolio(AlumnoCertificacion_83KI a, string numero)
        {
            string folio = PrefijoFolio(a) + ValidarNumero(numero).ToString("D4", CultureInfo.InvariantCulture);
            if (folio.Length > 200) throw new InvalidOperationException("Cun09.FolioLargo");
            return folio;
        }
        public static void Validar(GenerarCertificadoRequest_83KI r)
        {
            if (r == null || r.IdInscripcion <= 0 ||
                (r.Modalidad != "Solo digital" && r.Modalidad != "Digital e impresa") || string.IsNullOrWhiteSpace(r.Email) || r.Email.Length > 254 ||
                (r.Mencion ?? "").Length > 200 || (r.Observaciones ?? "").Length > 500)
                throw new InvalidOperationException("Cun09.DatosInvalidos");
            ValidarNumero(r.NumeroFolio);
            try { if (new MailAddress(r.Email).Address != r.Email || r.Email.IndexOfAny(new[] { '\r', '\n' }) >= 0) throw new FormatException(); }
            catch (FormatException) { throw new InvalidOperationException("Cun09.DatosInvalidos"); }
        }
        private static void ValidarEvaluaciones(AlumnoCertificacion_83KI a)
        {
            if (a.CantidadNotas != 2) throw new InvalidOperationException("Cun09.SinNotas");
        }
        public ResultadoCertificacion_83KI Generar(GenerarCertificadoRequest_83KI r)
        {
            Autorizar(); Validar(r);
            var result = new ResultadoCertificacion_83KI();
            result.Alumno = _dal.RevalidarEnTransaccion(r.IdInscripcion, a =>
            {
                ValidarEvaluaciones(a);
                if (a.Certificado != null) { result.Existente = true; return null; }
                if (!a.Aprobado || a.Balance != 0) throw new InvalidOperationException("Cun09.NoHabilitado");
                string prefijoVisto;
                if (!_prefijosVistos.TryGetValue(a.IdInscripcion, out prefijoVisto) ||
                    !string.Equals(prefijoVisto, PrefijoFolio(a), StringComparison.Ordinal))
                    throw new InvalidOperationException("Cun09.PrefijoCambio");
                string folio = ComponerFolio(a, r.NumeroFolio);
                return new CertificadoFinalizacion_83KI { IdInscripcion = a.IdInscripcion, Nombre = a.Nombre, Apellido = a.Apellido,
                    Curso = a.Curso, Promedio = a.Promedio, Fecha = a.Hoy,
                    Firmante = (_session.UsuarioActivo.Nombre + " " + _session.UsuarioActivo.Apellido).Trim(), Cargo = CargoResponsable,
                    Folio = folio, Modalidad = r.Modalidad, Email = r.Email, Mencion = r.Mencion ?? "", Observaciones = r.Observaciones ?? "" };
            });
            if (!result.Existente)
            {
                try { Registrar("certificado emitido", result.Alumno, result.Alumno.Certificado.Folio); }
                catch { result.AuditoriaFallida = true; } // La transacción de negocio ya está confirmada.
            }
            return result;
        }
        public ResultadoCertificacion_83KI Retener(int id, string observaciones)
        {
            Autorizar();
            if ((observaciones ?? "").Length > 500) throw new InvalidOperationException("Cun09.DatosInvalidos");
            EstadoRetencion estado;
            if (!_retenciones.TryGetValue(id, out estado)) throw new InvalidOperationException("Cun09.Seleccionar");
            if (estado.Registrada) throw new InvalidOperationException("Cun09.YaRetenido");
            var result = new ResultadoCertificacion_83KI();
            result.Alumno = _dal.RevalidarEnTransaccion(id, a =>
            {
                ValidarEvaluaciones(a);
                if (a.Motivos != estado.MotivosVistos || a.Motivos == "") { result.Reconfirmar = true; return null; }
                // No se realizan escrituras de negocio: se mantienen los bloqueos de elegibilidad hasta que finalice el intento de auditoría.
                Registrar("certificado retenido", a, a.Motivos + "; " + (observaciones ?? ""));
                estado.Registrada = true; // Aunque falle la limpieza de la transacción, nunca sugerir intentos de auditoría duplicados.
                result.Retenido = true;
                return null;
            });
            estado.MotivosVistos = result.Alumno.Motivos;
            return result;
        }
        private void Registrar(string accion, AlumnoCertificacion_83KI a, string detalle)
        {
            _audit.RegistrarEvento(BitacoraEvento_83KI.CrearNuevo("CUN09: " + accion + "; inscripción " + a.IdInscripcion +
                "; DNI " + a.DNI + "; " + detalle, Criticidad.Medio, Modulo.PlanificacionAcademica, _session.UsuarioActivo.UserName));
        }
    }
}

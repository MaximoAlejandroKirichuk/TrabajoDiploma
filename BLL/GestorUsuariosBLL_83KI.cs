using System;
using System.Collections.Generic;
using System.Linq;
using DAL;
using DAL.interfaces;
using Service;
using Service.DTOs;
using Service.Entidades;
using Service.Excepciones;
using Service.Excepciones.CrearUsuario;
using Service.Excepciones.Login;
using Service.Interfaces;

namespace BLL
{
    internal class GestorUsuarioBLL_83KI : IGestorUsuario_83KI
    {
        private const int IntentosPermitidos = 3;
        private const int MinutosParaReiniciarIntentos = 30;

        private readonly IUsuarioDAL_83KI _dal;
        private readonly IRolDAL_83KI _rolDal;
        private readonly IEncriptador_83KI _encriptador;
        private readonly ISessionManager_83KI _sessionManager;
        private readonly IBitacoraManager_83KI _bitacora;
        private readonly IGestorIdioma_83KI _gestorIdioma;
        private readonly IIntegridadDatosService_83KI _integridadService;

        public GestorUsuarioBLL_83KI(IUsuarioDAL_83KI dal, IEncriptador_83KI encriptador, ISessionManager_83KI sessionManager, IBitacoraManager_83KI bitacora)
            : this(dal, encriptador, sessionManager, bitacora, new RolDAL_83KI(), new GestorIdioma_83KI(),
                   new IntegridadBLL_83KI(new IntegridadDAL_83KI(new Encriptador_83KI()), bitacora))
        {
        }

        public GestorUsuarioBLL_83KI(IUsuarioDAL_83KI dal, IEncriptador_83KI encriptador, ISessionManager_83KI sessionManager, IBitacoraManager_83KI bitacora, IRolDAL_83KI rolDal, IGestorIdioma_83KI gestorIdioma, IIntegridadDatosService_83KI integridadService)
        {
            _dal = dal;
            _encriptador = encriptador;
            _sessionManager = sessionManager;
            _bitacora = bitacora;
            _rolDal = rolDal;
            _gestorIdioma = gestorIdioma;
            _integridadService = integridadService ?? throw new ArgumentNullException(nameof(integridadService));
        }

        public void Login(string userName, string contrasena)
        {
            if (_sessionManager.UsuarioActivo != null)
            {
                throw new UsuarioActivoActualmenteException_83KI();
            }

            var usuario = _dal.ObtenerPorUserName(userName);
            if (usuario == null)
            {
                RegistrarAuditoriaSegura(
                    $"Intento fallido de login: usuario inexistente '{userName}'",
                    Criticidad.Alto,
                    Modulo.Usuarios,
                    userName
                );
                throw new UsuarioNoExisteException_83KI();
            }

            if (!usuario.Activo)
            {
                throw new UsuarioDeshabilitadoException_83KI();
            }

            if (usuario.Bloqueado)
            {
                throw new UsuarioBloqueadoException_83KI();
            }

            // --- puerta de integridad: verifica ANTES de validar contrasena ---
            bool integridadSana = true;
            IntegridadSistemaEstado_83KI estadoIntegridad = null;

            try
            {
                estadoIntegridad = _integridadService.Verificar();
                integridadSana = estadoIntegridad.EstaSano;
            }
            catch
            {
                // si la verificacion misma falla, tratar como no sana
                integridadSana = false;
            }

            if (!integridadSana)
            {
                Rol_83KI rolConPatentes = null;
                bool tienePatentesRecuperacion = false;

                if (usuario.Rol != null)
                {
                    // carga el rol enriquecido para evaluar patentes de recuperacion en vez de EsAdministrador fijo.
                    // el rol plano de ObtenerPorUserName no tiene patentes, hay que enriquecerlo primero.
                    rolConPatentes = _rolDal.ObtenerRolesConPermisos()
                        .FirstOrDefault(r => r.CodigoRol == usuario.Rol.CodigoRol);

                    tienePatentesRecuperacion = rolConPatentes != null
                        && rolConPatentes.ObtenerPatentes().Any(p =>
                            p.CodigoPatente == (int)PermisoSistema_83KI.RecalcularHashes
                            || p.CodigoPatente == (int)PermisoSistema_83KI.EjecutarBackup
                            || p.CodigoPatente == (int)PermisoSistema_83KI.EjecutarRestore);
                }

                if (!tienePatentesRecuperacion)
                {
                    // usuario estandar bloqueado por falla de integridad — NO incrementar contador de intentos
                    var tablasAfectadas = estadoIntegridad?.Tablas
                        ?.Where(t => !t.EsValido)
                        ?.Select(t => t.NombreTabla)
                        ?.ToList()
                        ?? new List<string>();

                    RegistrarAuditoriaSegura(
                        "Integridad de datos comprometida",
                        Criticidad.Alto,
                        Modulo.Admin,
                        userName
                    );

                    throw new IntegridadComprometidaException_83KI(tablasAfectadas);
                }

                // el usuario tiene patentes de recuperacion — asigna el rol enriquecido temprano y sigue con validacion de contrasena
                usuario.AsignarRol(rolConPatentes);
            }

            // solo resetea intentos por tiempo cuando la integridad esta sana.
            // con integridad daniada, escribir en Usuarios via dal
            // recalcularia los hashes y legitimaria datos adulterados.
            if (integridadSana)
            {
                ReiniciarIntentosSiCorresponde(usuario);
            }

            string hash = _encriptador.HashContrasena(contrasena);

            if (usuario.Contrasena != hash)
            {
                // con integridad daniada, NO escribir estado de intentos fallidos
                // en Usuarios. las escrituras recalculan hashes
                // y legitimarian datos adulterados. rechazar el login de forma segura.
                if (integridadSana)
                {
                    RegistrarIntentoFallido(usuario);

                    if (SuperoIntentosPermitidos(usuario))
                    {
                        BloquearPorIntentosFallidos(usuario);
                        throw new UsuarioBloqueadoException_83KI();
                    }
                }

                throw new ContrasenaInvalidaException_83KI($"Intento {usuario.IntentosRealizados} de {IntentosPermitidos}.");
            }

            if (integridadSana)
            {
                usuario.AsignarRol(ObtenerRolConPermisos(usuario.Rol));
                ReiniciarIntentosFallidos(usuario);
            }
            // cuando la integridad no era sana pero el usuario tenia patentes de recuperacion,
            // el rol enriquecido ya fue asignado durante la verificacion de integridad.
            // NO llamar ReiniciarIntentosFallidos con integridad daniada — escribir
            // en Usuarios via dal recalcularia hashes y
            // legitimaria datos adulterados antes de la recuperacion.
            _sessionManager.IniciarSesion(usuario);

            // si el usuario con patentes de recuperacion entro con integridad daniada, establecer estado de recuperacion
            if (!integridadSana)
            {
                var tablasAfectadas = estadoIntegridad?.Tablas
                    ?.Where(t => !t.EsValido)
                    ?.Select(t => t.NombreTabla)
                    ?.ToList();
                _sessionManager.EstablecerRecuperacionIntegridad(tablasAfectadas);
            }

            // asigna el idioma del sistema segun la tabla usuario
            _gestorIdioma.CambiarIdioma(usuario.IdiomaId);
            RegistrarAuditoriaSegura(
                $"Login exitoso: {usuario.UserName}",
                Criticidad.Bajo,
                Modulo.Usuarios,
                userName
            );
        }

        private Rol_83KI ObtenerRolConPermisos(Rol_83KI rol)
        {
            Rol_83KI rolConPermisos = _rolDal.ObtenerRolesConPermisos()
                .FirstOrDefault(r => r.CodigoRol == rol.CodigoRol);

            if (rolConPermisos == null)
            {
                throw new InvalidOperationException("El rol del usuario no existe o no pudo cargarse con permisos.");
            }

            return rolConPermisos;
        }

        private void ReiniciarIntentosSiCorresponde(Usuario_83KI usuario)
        {
            if (!usuario.FechaUltimoIntento.HasValue)
            {
                return;
            }

            TimeSpan tiempoDesdeUltimoIntento = DateTime.Now - usuario.FechaUltimoIntento.Value;

            if (tiempoDesdeUltimoIntento.TotalMinutes >= MinutosParaReiniciarIntentos)
            {
                ReiniciarIntentosFallidos(usuario);
            }
        }

        private void RegistrarIntentoFallido(Usuario_83KI usuario)
        {
            usuario.RegistrarIntentoFallido(DateTime.Now);
            _dal.ActualizarIntentosFallidos(usuario);
            RegistrarAuditoriaSegura(
                $"Intento fallido de login: {usuario.UserName}",
                Criticidad.Alto,
                Modulo.Usuarios,
                usuario.UserName
            );
        }

        private bool SuperoIntentosPermitidos(Usuario_83KI usuario)
        {
            return usuario.IntentosRealizados >= IntentosPermitidos;
        }

        private void BloquearPorIntentosFallidos(Usuario_83KI usuario)
        {
            usuario.Bloquear();
            _dal.BloquearUsuario(usuario);
            RegistrarAuditoriaSegura(
                $"Usuario bloqueado por intentos fallidos: {usuario.UserName}",
                Criticidad.Alto,
                Modulo.Usuarios,
                usuario.UserName
            );
        }

        private void ReiniciarIntentosFallidos(Usuario_83KI usuario)
        {
            usuario.ReiniciarIntentosFallidos();
            _dal.ReiniciarIntentosFallidos(usuario);
        }

        public void Logout()
        {
            var usuario = _sessionManager.UsuarioActivo;
            if (usuario != null)
            {
                // Persiste el idioma solo si cambio durante la sesion
                string idiomaActual = _gestorIdioma.IdiomaActual.Id;
                if (!string.Equals(usuario.IdiomaId, idiomaActual, StringComparison.OrdinalIgnoreCase))
                {
                    usuario.CambiarIdioma(idiomaActual);
                    _dal.ActualizarIdioma(usuario.DNI, usuario.IdiomaId);
                }
                _sessionManager.CerrarSesion();
                RegistrarAuditoriaSegura(
                    $"Logout exitoso: {usuario.UserName}",
                    Criticidad.Bajo,
                    Modulo.Usuarios,
                    usuario.UserName
                );
            }
        }


        public void CambiarContrasenaUsuarioActual(string contrasenaActual, string nuevaContrasena)
        {
            var usuarioActivo = _sessionManager.UsuarioActivo ?? throw new UsuarioNoAutenticadoException_83KI();
            CambiarContrasena(usuarioActivo.UserName, contrasenaActual, nuevaContrasena);
        }

        public void CambiarIdiomaUsuarioActual(string idiomaId)
        {
            var usuarioActivo = _sessionManager.UsuarioActivo ?? throw new UsuarioNoAutenticadoException_83KI();
            ValidarPermiso(PermisoSistema_83KI.CambiarIdioma);
            _gestorIdioma.CambiarIdioma(idiomaId);
        }

        private void CambiarContrasena(string userName, string contrasenaActual, string nuevaContrasena)
        {
            var usuario = _dal.ObtenerPorUserName(userName) ?? throw new UsuarioNoExisteException_83KI();
            string hashContrasenaActual = _encriptador.HashContrasena(contrasenaActual);

            if (usuario.Contrasena != hashContrasenaActual)
            {
                throw new ContrasenaInvalidaException_83KI("La contraseña actual ingresada no coincide con la registrada.");
            }

            usuario.CambiarContrasena(_encriptador.HashContrasena(nuevaContrasena));
            _dal.ActualizarContrasena(usuario);

            if (_sessionManager.UsuarioActivo != null && _sessionManager.UsuarioActivo.UserName == usuario.UserName)
            {
                _sessionManager.UsuarioActivo.CambiarContrasena(usuario.Contrasena);
            }

            RegistrarAuditoriaSegura(
                $"Contraseña modificada: {usuario.UserName} (Actor: {_sessionManager.UsuarioActivo.UserName})",
                Criticidad.Alto,
                Modulo.Usuarios,
                usuario.UserName
            );
        }

        public void ModificarUsuario(int dni, string email, Rol_83KI rol)
        {
            var usuarioActivo = _sessionManager.UsuarioActivo ?? throw new UsuarioNoAutenticadoException_83KI();
            ValidarPermiso(PermisoSistema_83KI.ModificarUsuario);

            var usuarioModificado = _dal.ObtenerPorDni(dni) ?? throw new UsuarioNoExisteException_83KI();
            usuarioModificado.ModificarEmailYRol(email, rol);

            if (_dal.ExisteEmailParaOtroUsuario(usuarioModificado.Email, usuarioModificado.DNI))
            {
                throw new EmailRegistradoException_83KI();
            }

            bool esAutoedicion = usuarioActivo.DNI == usuarioModificado.DNI;
            if (esAutoedicion && usuarioActivo.Rol.CodigoRol != usuarioModificado.Rol.CodigoRol)
            {
                throw new InvalidOperationException("No puede modificar su propio rol.");
            }

            _dal.ModificarUsuario(usuarioModificado.DNI, usuarioModificado.Email, usuarioModificado.Rol);

            if (esAutoedicion)
            {
                _sessionManager.UsuarioActivo.ModificarEmail(usuarioModificado.Email);
            }

            RegistrarAuditoriaSegura(
                $"Usuario modificado: DNI {usuarioModificado.DNI}. Email: {usuarioModificado.Email}. Rol: {usuarioModificado.Rol}. Actor: {usuarioActivo.UserName}",
                Criticidad.Alto,
                Modulo.Admin,
                usuarioModificado.UserName
            );
        }

        public void CrearUsuario(string nombre, string apellido, int dni, string email, Rol_83KI rol)
        {
            ValidarPermiso(PermisoSistema_83KI.CrearUsuario);

            if (_dal.ExisteDni(dni))
            {
                throw new DniRegistradoException_83KI();
            }

            if (_dal.ExisteEmail(email))
            {
                throw new EmailRegistradoException_83KI();
            }

            string contrasenaPorDefecto = Usuario_83KI.EstablecerContrasenaPorDefecto(apellido, dni);
            string contrasenaHash = _encriptador.HashContrasena(contrasenaPorDefecto);

            Usuario_83KI usuario = Usuario_83KI.CrearNuevo(nombre, apellido, dni, email, rol, contrasenaHash);

            _dal.CrearUsuario(usuario);

            RegistrarAuditoriaSegura(
                $"Nuevo usuario creado: {usuario.UserName} (Rol: {usuario.Rol}) (Actor: {_sessionManager.UsuarioActivo.UserName})",
                Criticidad.Alto,
                Modulo.Admin,
                usuario.UserName
            );
        }

        public IEnumerable<Usuario_83KI> ObtenerUsuarios()
        {
            return _dal.ObtenerUsuarios();
        }

        public void DesbloquearCuenta(int dni)
        {
            ValidarPermiso(PermisoSistema_83KI.DesbloquearUsuario);

            var usuario = _dal.ObtenerPorDni(dni) ?? throw new UsuarioNoExisteException_83KI();
            string contrasenaPorDefecto = Usuario_83KI.EstablecerContrasenaPorDefecto(usuario.Apellido, usuario.DNI);
            usuario.Desbloquear(_encriptador.HashContrasena(contrasenaPorDefecto));
            _dal.DesbloquearCuenta(usuario);
            RegistrarAuditoriaSegura(
                $"Usuario desbloqueado: {usuario.UserName} (Rol: {usuario.Rol}) (Actor: {_sessionManager.UsuarioActivo.UserName})",
                Criticidad.Alto,
                Modulo.Admin,
                usuario.UserName
            );
        }

        public void HabilitarUsuario(int dni)
        {
            CambiarEstadoUsuario(dni, true);
        }

        public void DeshabilitarUsuario(int dni)
        {
            CambiarEstadoUsuario(dni, false);
        }

        private void CambiarEstadoUsuario(int dni, bool activo)
        {
            //ACA VALIDO QUE NO ME PUEDO DESHABILITAR A MI MISMO.
            var usuarioActivo = _sessionManager.UsuarioActivo ?? throw new UsuarioNoAutenticadoException_83KI();
            ValidarPermiso(activo ? PermisoSistema_83KI.HabilitarUsuario : PermisoSistema_83KI.DeshabilitarUsuario);

            if (usuarioActivo.DNI == dni && !activo)
            {
                throw new InvalidOperationException("No puede deshabilitar su propio usuario.");
            }

            var usuarioGestionado = _dal.ObtenerPorDni(dni);
            if (usuarioGestionado == null)
            {
                throw new UsuarioNoExisteException_83KI();
            }

            if (usuarioGestionado.Activo == activo)
            {
                string mensajeEstado = activo ? "ya esta habilitado." : "ya esta deshabilitado.";
                throw new InvalidOperationException($"El usuario seleccionado {mensajeEstado}");
            }

            if (activo)
            {
                usuarioGestionado.Habilitar();
            }
            else
            {
                usuarioGestionado.Deshabilitar();
            }

            _dal.ActualizarEstadoActivo(usuarioGestionado.DNI, usuarioGestionado.Activo);

            string accion = activo ? "habilitado" : "deshabilitado";
            RegistrarAuditoriaSegura(
                $"Usuario {accion}: DNI {dni}. Actor: {usuarioActivo.UserName}",
                Criticidad.Alto,
                Modulo.Admin,
                usuarioGestionado.UserName
            );
        }

        private void RegistrarAuditoriaSegura(string descripcion, Criticidad criticidad, Modulo modulo, string username)
        {
            try
            {
                _bitacora.RegistrarEvento(
                    BitacoraEvento_83KI.CrearNuevo(descripcion, criticidad, modulo, username)
                );
            }
            catch
            {
                // La auditoría no debe interrumpir el flujo de negocio
            }
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

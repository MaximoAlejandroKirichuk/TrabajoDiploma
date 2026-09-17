using BE;
using BE.Entidades;
using Service.Entidades;
using BLL.Excepciones;
using BLL.Excepciones.CrearUsuario;
using BLL.Excepciones.Login;
using BLL.Interfaces;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Service
{
    public class GestorUsuarioService_83KI : IGestorUsuario_83KI
    {
        private readonly IUsuarioRepository_83KI _usuarioRepository;
        private readonly IEncriptador_83KI _encriptador;
        private readonly ISessionManager_83KI _sessionManager;
        private readonly IBitacoraManager_83KI _bitacora;
        private const int NumeroIntentos = 3;

        public GestorUsuarioService_83KI(IUsuarioRepository_83KI usuarioRepository, IBitacoraManager_83KI bitacora)
            : this(usuarioRepository, new Encriptador_83KI(), SessionManager_83KI.Instancia, bitacora)
        {
        }

        internal GestorUsuarioService_83KI(
            IUsuarioRepository_83KI usuarioRepository,
            IEncriptador_83KI encriptador,
            ISessionManager_83KI sessionManager,
            IBitacoraManager_83KI bitacora)
        {
            _usuarioRepository = usuarioRepository;
            _encriptador = encriptador;
            _sessionManager = sessionManager;
            _bitacora = bitacora;
        }

        public void Login(string userName, string contrasena)
        {
            if (_sessionManager.UsuarioActivo != null)
            {
                throw new UsuarioActivoActualmenteException_83KI();
            }

            var usuario = _usuarioRepository.ObtenerPorUserName(userName) ?? throw new UsuarioNoExisteException_83KI();

            if (usuario.Bloqueado)
            {
                throw new UsuarioBloqueadoException_83KI();
            }

            string hash = _encriptador.HashContrasena(contrasena);

            if (usuario.Contrasena != hash)
            {
                usuario.Intentos++;
                if (usuario.Intentos >= NumeroIntentos)
                {
                    BloqueoCuentaUsuario(usuario);
                }

                _usuarioRepository.ActualizarIntentos(usuario);
                throw new ContrasenaInvalidaException_83KI($"Intento {usuario.Intentos} de {NumeroIntentos}.");
            }

            _sessionManager.IniciarSesion(usuario);
            usuario.Intentos = 0;
            _usuarioRepository.ActualizarIntentos(usuario);
            _bitacora.RegistrarEvento(
                new BitacoraEvento_83KI(
                    $"Login exitoso: {usuario.UserName}",
                    3,
                    Modulo.Usuarios,
                    usuario.UserName
                )
            );
        }

        public void Logout()
        {
            var usuario = _sessionManager.UsuarioActivo;
            if (usuario != null)
            {
                _sessionManager.CerrarSesion();
                _bitacora.RegistrarEvento(
                    new BitacoraEvento_83KI(
                        $"Logout exitoso: {usuario.UserName}",
                        3,
                        Modulo.Usuarios,
                        usuario.UserName
                    )
                );
            }
        }

        public void CambiarContrasenaUsuarioActual(string contrasenaActual, string nuevaContrasena)
        {
            throw new NotImplementedException();
        }

        public void ModificarUsuario(int dni, string email, Rol_83KI rol)
        {
            var usuarioActivo = _sessionManager.UsuarioActivo ?? throw new UsuarioNoAutenticadoException_83KI();

            if (!usuarioActivo.Rol.EsAdministrador)
            {
                throw new InvalidOperationException("Solo un administrador puede modificar usuarios.");
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                throw new InvalidOperationException("El email es obligatorio.");
            }

            string patronEmail = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            string emailNormalizado = email.Trim();

            if (!Regex.IsMatch(emailNormalizado, patronEmail))
            {
                throw new InvalidOperationException("El formato del email no es valido.");
            }

            if (_usuarioRepository.ExisteEmailParaOtroUsuario(emailNormalizado, dni))
            {
                throw new EmailRegistradoException_83KI();
            }

            bool esAutoedicion = usuarioActivo.DNI == dni;
            if (esAutoedicion && usuarioActivo.Rol.CodigoRol != rol.CodigoRol)
            {
                throw new InvalidOperationException("No puede modificar su propio rol.");
            }

            _usuarioRepository.ModificarUsuario(dni, emailNormalizado, rol);

            if (esAutoedicion)
            {
                _sessionManager.UsuarioActivo.Email = emailNormalizado;
            }

            _bitacora.RegistrarEvento(
                new BitacoraEvento_83KI(
                    $"Usuario modificado: DNI {dni}. Email: {emailNormalizado}. Rol: {rol}. Actor: {usuarioActivo.UserName}",
                    2,
                    Modulo.Usuarios,
                    usuarioActivo.UserName
                )
            );
        }

        public void BloqueoCuentaUsuario(Usuario_83KI usuario)
        {
            usuario.Bloqueado = true;
            _usuarioRepository.BloquearUsuario(usuario);
            _bitacora.RegistrarEvento(
                new BitacoraEvento_83KI(
                    $"Usuario bloqueado: {usuario.UserName}",
                    1,
                    Modulo.Usuarios,
                    usuario.UserName
                )
            );
            throw new UsuarioBloqueadoException_83KI();
        }

        public void CrearUsuario(Usuario_83KI usuario)
        {
            if (_usuarioRepository.ExisteDni(usuario.DNI))
            {
                throw new DniRegistradoException_83KI();
            }

            if (_usuarioRepository.ExisteEmail(usuario.Email))
            {
                throw new EmailRegistradoException_83KI();
            }

            string contrasenaPorDefecto = EstablecerContrasenaPorDefecto(usuario.Nombre, usuario.DNI);
            usuario.Contrasena = _encriptador.HashContrasena(contrasenaPorDefecto);

            _usuarioRepository.CrearUsuario(usuario);
            _bitacora.RegistrarEvento(
                new BitacoraEvento_83KI(
                    $"Nuevo usuario creado: {usuario.UserName} (Rol: {usuario.Rol})",
                    1,
                    Modulo.Usuarios,
                    usuario.UserName
                )
            );
        }

        public IEnumerable<Usuario_83KI> ObtenerUsuarios()
        {
            return _usuarioRepository.ObtenerUsuarios();
        }

        public void DesbloquearCuenta(Usuario_83KI usuario)
        {
            usuario.Intentos = 0;
            usuario.Bloqueado = false;

            string contrasenaPorDefecto = EstablecerContrasenaPorDefecto(usuario.Nombre, usuario.DNI);
            usuario.Contrasena = _encriptador.HashContrasena(contrasenaPorDefecto);
            _usuarioRepository.DesbloquearCuenta(usuario);
            _bitacora.RegistrarEvento(
                new BitacoraEvento_83KI(
                    $"Usuario desbloqueado: {usuario.UserName} (Rol: {usuario.Rol})",
                    1,
                    Modulo.Usuarios,
                    usuario.UserName
                )
            );
        }

        private string EstablecerContrasenaPorDefecto(string nombre, int dni)
        {
            return $"{nombre}{dni % 1000:D3}";
        }

    }
}

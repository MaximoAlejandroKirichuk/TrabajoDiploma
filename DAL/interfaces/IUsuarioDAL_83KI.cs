using Service.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.interfaces
{
    public interface IUsuarioDAL_83KI
    {
        Usuario_83KI ObtenerPorDni(int dni);
        Usuario_83KI ObtenerPorUserName(string userName);
        void BloquearUsuario(Usuario_83KI usuario);
        void CrearUsuario(Usuario_83KI usuario);
        void ModificarUsuario(int dni, string email, Rol_83KI rol);
        void ActualizarContrasena(Usuario_83KI usuario);
        void ActualizarIdioma(int dni, string idiomaId);
        bool ExisteDni(int dni);
        bool ExisteEmail(string email);
        bool ExisteEmailParaOtroUsuario(string email, int dni);
        bool EstaBloqueado(int dni);
        void ActualizarIntentosFallidos(Usuario_83KI usuario);
        void ReiniciarIntentosFallidos(Usuario_83KI usuario);
        void ActualizarEstadoActivo(int dni, bool activo);
        void DesbloquearCuenta(Usuario_83KI usuario);
        IEnumerable<Usuario_83KI> ObtenerUsuarios();
    }
}

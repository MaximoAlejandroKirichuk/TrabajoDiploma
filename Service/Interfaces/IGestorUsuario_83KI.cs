using Service.Entidades;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IGestorUsuario_83KI
    {
        void Login(string email, string contrasena);
        void Logout();
        void CrearUsuario(string nombre, string apellido, int dni, string email, Rol_83KI rol);
        void ModificarUsuario(int dni, string email, Rol_83KI rol);
        void CambiarContrasenaUsuarioActual(string contrasenaActual, string nuevaContrasena);
        void CambiarIdiomaUsuarioActual(string idiomaId);
        void DesbloquearCuenta(int dni);
        void HabilitarUsuario(int dni);
        void DeshabilitarUsuario(int dni);
        IEnumerable<Usuario_83KI> ObtenerUsuarios();
    }
}

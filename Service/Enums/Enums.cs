using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Entidades
{
    public enum Modulo
    {
        Usuarios,
        Admin
    }

    public enum Criticidad
    {
        Alto = 1,
        Medio = 2,
        Bajo = 3
    }

    public enum PermisoSistema_83KI
    {
        CrearUsuario = 1,
        ModificarUsuario = 2,
        HabilitarUsuario = 3,
        DeshabilitarUsuario = 4,
        DesbloquearUsuario = 5,
        VerBitacoraEventos = 6,
        CerrarSesion = 7,
        ReLogin = 8,
        Ayuda = 9,
        CambiarContrasena = 10,
        GestionUsuarios = 14,
        VerUsuarios = 15,
        GestionFamilias = 16,
        VerFamilias = 17,
        CrearFamilia = 18,
        EliminarFamilia = 19,
        AgregarPatenteFamilia = 20,
        QuitarPatenteFamilia = 21,
        AgregarSubfamilia = 22,
        QuitarSubfamilia = 23,
        GestionRoles = 24,
        VerRoles = 25,
        VerPermisosEfectivosRol = 26,
        AgregarFamiliaRol = 27,
        QuitarFamiliaRol = 28,
        AsignarPatenteRol = 29,
        QuitarPatenteRol = 30,
        ConsultarBitacoraEventos = 31,
        FiltrarBitacoraEventos = 32,
        LimpiarFiltrosBitacora = 33,
        ExportarBitacoraPdf = 34,
        GestionAdmin = 35,
        EliminarRol = 36,
        CambiarIdioma = 37,
        EjecutarBackup = 38,
        EjecutarRestore = 39,
        RecalcularHashes = 40
    }
}

using Service.Entidades;
using System.Collections.Generic;

namespace Service.Interfaces
{
    public interface IGestorRol_83KI
    {
        IEnumerable<Rol_83KI> ObtenerRoles();
        IEnumerable<Rol_83KI> ObtenerRolesConPermisos();
        IEnumerable<Familia_83KI> ObtenerFamilias();
        IEnumerable<Patente_83KI> ObtenerPatentes();
        Rol_83KI CrearRol(string nombre, List<int> codigosPatentes, List<int> codigosFamilias);
        Familia_83KI CrearFamilia(string nombre, List<int> codigosPatentes, List<int> codigosFamilias);
        void EliminarFamilia(int codigoFamilia);
        void AsignarPatenteAFamilia(int codigoFamilia, int codigoPatente);
        void QuitarPatenteDeFamilia(int codigoFamilia, int codigoPatente);
        void AsignarFamiliaAFamilia(int codigoFamiliaPadre, int codigoFamiliaHija);
        void QuitarFamiliaDeFamilia(int codigoFamiliaPadre, int codigoFamiliaHija);
        void AsignarPatenteARol(int codigoRol, int codigoPatente);
        void QuitarPatenteDeRol(int codigoRol, int codigoPatente);
        void AsignarFamiliaARol(int codigoRol, int codigoFamilia);
        void QuitarFamiliaDeRol(int codigoRol, int codigoFamilia);
        void EliminarRol(int codigoRol);
    }
}

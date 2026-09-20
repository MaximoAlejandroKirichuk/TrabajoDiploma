using BLL;
using DAL;
using Service;
using Service.Interfaces;

namespace UI.Modulos.Maestros
{
    internal static class ComposicionMaestros_83KI
    {
        public static IGestorProfesor_83KI CrearGestorProfesor()
        {
            return new KI68GestorProfesorBLL(
                new KI68ProfesorDAL(),
                SessionManager_83KI.Instancia,
                new BitacoraBLL_83KI(new BitacoraEventoDAL_83KI()));
        }

        public static IGestorCurso_83KI CrearGestorCurso()
        {
            return new KI68GestorCursoBLL(
                new CursoDAL_83KI(),
                SessionManager_83KI.Instancia,
                new BitacoraBLL_83KI(new BitacoraEventoDAL_83KI()));
        }

        public static IGestorCursoProfesor_83KI CrearGestorCursoProfesor()
        {
            return new KI68GestorCursoProfesorBLL(
                new KI68CursoProfesorDAL(),
                SessionManager_83KI.Instancia,
                new BitacoraBLL_83KI(new BitacoraEventoDAL_83KI()));
        }
    }
}

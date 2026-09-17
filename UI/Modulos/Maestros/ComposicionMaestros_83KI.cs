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
            return new GestorProfesorBLL_83KI(
                new ProfesorDAL_83KI(),
                SessionManager_83KI.Instancia,
                new BitacoraBLL_83KI(new BitacoraEventoDAL_83KI()));
        }

        public static IGestorCurso_83KI CrearGestorCurso()
        {
            return new GestorCursoBLL_83KI(
                new CursoDAL_83KI(),
                SessionManager_83KI.Instancia,
                new BitacoraBLL_83KI(new BitacoraEventoDAL_83KI()));
        }

        public static IGestorCursoProfesor_83KI CrearGestorCursoProfesor()
        {
            return new GestorCursoProfesorBLL_83KI(
                new CursoProfesorDAL_83KI(),
                SessionManager_83KI.Instancia,
                new BitacoraBLL_83KI(new BitacoraEventoDAL_83KI()));
        }
    }
}

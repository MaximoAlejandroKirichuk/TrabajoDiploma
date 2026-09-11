using System;
using DAL.interfaces;
using Service;
using Service.Entidades;
using Service.Interfaces;

namespace BLL
{
    /// <summary>
    /// logica de negocio para backup y restore completo de la base de datos.
    /// implementa IRecuperacionBaseDatosService_83KI — el contrato de contingencia del admin.
    /// </summary>
    internal class RecuperacionBaseDatosBLL_83KI : IRecuperacionBaseDatosService_83KI
    {
        private readonly IRecuperacionDAL_83KI _recuperacionDal;
        private readonly IBitacoraManager_83KI _bitacora;

        public RecuperacionBaseDatosBLL_83KI(IRecuperacionDAL_83KI recuperacionDal, IBitacoraManager_83KI bitacora)
        {
            _recuperacionDal = recuperacionDal ?? throw new ArgumentNullException(nameof(recuperacionDal));
            _bitacora = bitacora ?? throw new ArgumentNullException(nameof(bitacora));
        }

        /// <summary>
        /// crea un backup completo (.bak) de la base GestionUsuarios.
        /// audita la accion cuando es exitosa.
        /// </summary>
        public void GenerarBackup(string ruta, string actor)
        {
            _recuperacionDal.EjecutarBackup(ruta);

            RegistrarAuditoriaSegura(
                string.Format("Backup de base de datos ejecutado: {0} — Ruta: {1}", actor, ruta),
                Criticidad.Alto,
                Modulo.Admin,
                actor
            );
        }

        /// <summary>
        /// restaura toda la base GestionUsuarios desde un archivo de backup.
        /// usa una conexion dedicada a master. al terminar el restore,
        /// audita la accion, cierra la sesion y senala reinicio.
        /// </summary>
        public void RestaurarBackup(string ruta, string actor)
        {
            _recuperacionDal.EjecutarRestore(ruta);

            // despues del restore la conexion principal puede quedar invalida.
            // se escribe la auditoria con una conexion fresca antes de senalar reinicio.
            try
            {
                RegistrarAuditoriaSegura(
                    string.Format("Restauracion de base de datos ejecutada: {0} — Ruta: {1}", actor, ruta),
                    Criticidad.Alto,
                    Modulo.Admin,
                    actor
                );
            }
            catch
            {
                // auditoria mejor-esfuerzo despues del restore — la base puede seguir recuperandose
            }

            // senala que la aplicacion debe cerrarse y mostrar reinicio
            var session = SessionManager_83KI.Instancia;
            session.CerrarSesion();
            session.ReinicioRequeridoDespuesDeRestore = true;
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
                // la auditoria no debe interrumpir el flujo de negocio
            }
        }
    }
}

namespace Service.Interfaces
{
    /// <summary>
    /// contrato para operaciones de backup y restore completo de la base de datos.
    /// el restore invalida conexiones activas y requiere reinicio de la aplicacion.
    /// </summary>
    public interface IRecuperacionBaseDatosService_83KI
    {
        /// <summary>
        /// crea un backup completo (.bak) de la base GestionUsuarios.
        /// </summary>
        /// <param name="ruta">ruta del archivo de salida para el backup.</param>
        /// <param name="actor">username que ejecuta el backup (para auditoria).</param>
        void GenerarBackup(string ruta, string actor);

        /// <summary>
        /// restaura toda la base GestionUsuarios desde un archivo de backup.
        /// usa una conexion dedicada a master. luego del restore exitoso,
        /// la aplicacion debe cerrarse y reabrirse.
        /// </summary>
        /// <param name="ruta">ruta al archivo de backup.</param>
        /// <param name="actor">username que ejecuta el restore (para auditoria).</param>
        void RestaurarBackup(string ruta, string actor);
    }
}

namespace DAL.interfaces
{
    /// <summary>
    /// contrato dal minimo para operaciones de backup y restore de base de datos.
    /// el restore se ejecuta contra la base master.
    /// </summary>
    public interface IRecuperacionDAL_83KI
    {
        /// <summary>
        /// ejecuta BACKUP DATABASE a un archivo en disco usando la conexion principal.
        /// </summary>
        void EjecutarBackup(string rutaArchivo);

        /// <summary>
        /// ejecuta RESTORE DATABASE desde un archivo en disco usando una conexion dedicada a master.
        /// </summary>
        void EjecutarRestore(string rutaArchivo);
    }
}

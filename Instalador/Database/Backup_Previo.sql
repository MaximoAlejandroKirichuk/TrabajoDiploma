/* =============================================================================
   Backup_Previo.sql - copia de seguridad antes de actualizar la base.

   Marcas que imprime (el instalador las busca en el log):
     BACKUP_SIN_BASE       la base no existe: es instalacion nueva, no hay nada que resguardar
     BACKUP_OK             backup escrito en la carpeta indicada (ProgramData)
     BACKUP_OK_CARPETA_SQL backup escrito en la carpeta de backups predeterminada de la instancia

   TRY/CATCH + THROW es obligatorio: sin eso un BACKUP fallido no corta el lote
   y se imprimiria BACKUP_OK aunque el archivo no exista.

   Ejecucion manual:
     sqlcmd -S "<instancia>" -E -b -l 30 -v BaseDatos="InstitutoVisionEnAccion" CarpetaBackup="C:\ProgramData\InstitutoVisionEnAccion\Backups" ArchivoBackup="copia.bak" -i Backup_Previo.sql
   ============================================================================= */
SET NOCOUNT ON;

IF DB_ID(N'$(BaseDatos)') IS NULL
BEGIN
    PRINT 'BACKUP_SIN_BASE';
END
ELSE
BEGIN
    BEGIN TRY
        BACKUP DATABASE [$(BaseDatos)]
            TO DISK = N'$(CarpetaBackup)\$(ArchivoBackup)'
            WITH INIT, COPY_ONLY, CHECKSUM, NAME = N'$(BaseDatos) - backup previo a la actualizacion';
        PRINT 'BACKUP_OK';
    END TRY
    BEGIN CATCH
        PRINT 'Aviso: no se pudo escribir el backup en $(CarpetaBackup): ' + ERROR_MESSAGE();
        PRINT 'Se reintenta en la carpeta de backups predeterminada de la instancia.';
        BEGIN TRY
            -- solo el nombre del archivo: SQL Server lo guarda en su carpeta de backups predeterminada
            BACKUP DATABASE [$(BaseDatos)]
                TO DISK = N'$(ArchivoBackup)'
                WITH INIT, COPY_ONLY, CHECKSUM, NAME = N'$(BaseDatos) - backup previo a la actualizacion';
            PRINT 'BACKUP_OK_CARPETA_SQL';
        END TRY
        BEGIN CATCH
            THROW;
        END CATCH
    END CATCH
END

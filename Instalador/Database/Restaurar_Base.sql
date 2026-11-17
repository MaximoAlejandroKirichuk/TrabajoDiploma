/* =============================================================================
   Restaurar_Base.sql - restaura la base desde una copia de seguridad (.bak).

   Se usa en la reinstalacion cuando el usuario elige "Restaurar desde una copia".
   El instalador SIEMPRE hace antes un backup de la base actual (Backup_Previo.sql),
   y despues de restaurar ejecuta el Deploy para dejar el esquema al dia.

   Marca que imprime si termino bien (el instalador la busca en el log): RESTORE_OK

   El .bak tiene que ser de esta misma instancia (por ejemplo, uno de los que genera
   el instalador): los archivos de la base se restauran en su ubicacion original.

   Ejecucion manual:
     sqlcmd -S "<instancia>" -E -b -l 30 -v BaseDatos="InstitutoVisionEnAccion" ArchivoBak="C:\ProgramData\InstitutoVisionEnAccion\Backups\copia.bak" -i Restaurar_Base.sql
   ============================================================================= */
SET NOCOUNT ON;
USE [master];

BEGIN TRY
    -- 1) comprobar que el archivo es un backup legible ANTES de tocar la base actual
    RESTORE VERIFYONLY FROM DISK = N'$(ArchivoBak)';

    -- 2) cerrar las conexiones abiertas a la base (si existe)
    IF DB_ID(N'$(BaseDatos)') IS NOT NULL
        ALTER DATABASE [$(BaseDatos)] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;

    -- 3) restaurar reemplazando la base actual
    RESTORE DATABASE [$(BaseDatos)] FROM DISK = N'$(ArchivoBak)' WITH REPLACE, RECOVERY;

    ALTER DATABASE [$(BaseDatos)] SET MULTI_USER;
    PRINT 'RESTORE_OK';
END TRY
BEGIN CATCH
    PRINT 'Error al restaurar: ' + ERROR_MESSAGE();
    -- si la base quedo en modo de un solo usuario, se vuelve a abrir para todos
    IF DB_ID(N'$(BaseDatos)') IS NOT NULL AND DATABASEPROPERTYEX(N'$(BaseDatos)', 'UserAccess') = 'SINGLE_USER'
    BEGIN
        BEGIN TRY
            ALTER DATABASE [$(BaseDatos)] SET MULTI_USER;
        END TRY
        BEGIN CATCH
            PRINT 'Aviso: no se pudo volver a MULTI_USER: ' + ERROR_MESSAGE();
        END CATCH
    END;
    THROW;
END CATCH

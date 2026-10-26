-- Detener las operaciones de escritura de la aplicación y crear primero una copia de seguridad. Nunca se eliminan calificaciones registradas.
SET NOCOUNT ON;
SET XACT_ABORT ON;
BEGIN TRY
    BEGIN TRANSACTION;
    IF OBJECT_ID(N'dbo.ResultadoEvaluacion',N'U') IS NOT NULL
    BEGIN
        IF EXISTS(SELECT 1 FROM dbo.ResultadoEvaluacion WITH (TABLOCKX,HOLDLOCK))
            THROW 52815,N'Rollback refused: recorded grades exist. Restore a backup or retain the additive schema.',1;
        DROP TABLE dbo.ResultadoEvaluacion;
    END;
    DELETE dbo.DigitoVerificador_83KI WHERE NombreTabla=N'ResultadoEvaluacion';
    IF EXISTS(SELECT 1 FROM dbo.Patentes WHERE CodigoPatente=71 AND Nombre=N'Registrar resultados de evaluación')
    BEGIN
        DELETE dbo.RolPatente WHERE CodigoPatente=71;
        DELETE dbo.FamiliaPatente WHERE CodigoPatente=71;
        DELETE dbo.Patentes WHERE CodigoPatente=71;
        DECLARE @tabla SYSNAME,@hashes NVARCHAR(MAX),@sql NVARCHAR(MAX);
        DECLARE tablas CURSOR LOCAL FAST_FORWARD FOR SELECT n FROM (VALUES(N'Patentes'),(N'RolPatente'),(N'FamiliaPatente')) t(n);
        OPEN tablas;
        FETCH NEXT FROM tablas INTO @tabla;
        WHILE @@FETCH_STATUS=0
        BEGIN
            SET @sql=N'SELECT @h=COALESCE((SELECT CONVERT(nvarchar(max),DVH) AS [text()] FROM dbo.'+QUOTENAME(@tabla)+
                N' ORDER BY DVH FOR XML PATH(''''),TYPE).value(''.'',''nvarchar(max)''),N'''')';
            EXEC sys.sp_executesql @sql,N'@h nvarchar(max) OUTPUT',@h=@hashes OUTPUT;
            UPDATE dbo.DigitoVerificador_83KI SET DVV=LOWER(CONVERT(VARCHAR(64),HASHBYTES('SHA2_256',@hashes),2)),FechaActualizacion=GETDATE() WHERE NombreTabla=@tabla;
            FETCH NEXT FROM tablas INTO @tabla;
        END;
        CLOSE tablas; DEALLOCATE tablas;
    END;
    IF EXISTS(SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.SolicitudInscripcion') AND name=N'UX_CUN08_InscripcionComision')
        DROP INDEX UX_CUN08_InscripcionComision ON dbo.SolicitudInscripcion;
    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT>0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;

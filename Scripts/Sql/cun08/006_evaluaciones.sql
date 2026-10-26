-- Aplicar después de CUN08 y CUN09, con las escrituras detenidas y una copia de seguridad verificada.
-- No concede permisos a ningún rol ni modifica resultados o certificados históricos.
SET NOCOUNT ON;
SET XACT_ABORT ON;
BEGIN TRY
    BEGIN TRANSACTION;
    IF OBJECT_ID(N'dbo.ResultadoEvaluacion',N'U') IS NULL OR OBJECT_ID(N'dbo.CertificadoFinalizacion',N'U') IS NULL
        THROW 52931,N'Evaluaciones requires the CUN08 and CUN09 migrations.',1;
    IF EXISTS(SELECT 1 FROM dbo.Patentes WHERE
        (CodigoPatente=73 AND Nombre COLLATE Latin1_General_100_BIN2<>N'Ver evaluaciones') OR
        (CodigoPatente=74 AND Nombre COLLATE Latin1_General_100_BIN2<>N'Modificar evaluaciones'))
        OR EXISTS(SELECT 1 FROM dbo.Patentes WHERE
            (Nombre=N'Ver evaluaciones' AND CodigoPatente<>73) OR (Nombre=N'Modificar evaluaciones' AND CodigoPatente<>74))
        THROW 52932,N'Evaluaciones permission ID/name collision. Review before deployment.',1;
    DECLARE @nuevas TABLE(id int,nombre nvarchar(100));
    INSERT @nuevas SELECT id,nombre FROM (VALUES(73,N'Ver evaluaciones'),(74,N'Modificar evaluaciones')) p(id,nombre)
        WHERE NOT EXISTS(SELECT 1 FROM dbo.Patentes WHERE CodigoPatente=p.id);
    IF EXISTS(SELECT 1 FROM @nuevas)
    BEGIN
        SET IDENTITY_INSERT dbo.Patentes ON;
        INSERT dbo.Patentes(CodigoPatente,Nombre,DVH)
        SELECT id,nombre,LOWER(CONVERT(varchar(64),HASHBYTES('SHA2_256',CONVERT(nvarchar(max),
            N'CodigoPatente='+CONVERT(nvarchar(20),id)+N'|Nombre='+nombre)),2)) FROM @nuevas;
        SET IDENTITY_INSERT dbo.Patentes OFF;
        DECLARE @dv nvarchar(max)=COALESCE((SELECT CONVERT(nvarchar(max),DVH) AS [text()]
            FROM dbo.Patentes ORDER BY DVH FOR XML PATH(''),TYPE).value('.','nvarchar(max)'),N'');
        UPDATE dbo.DigitoVerificador_83KI SET DVV=LOWER(CONVERT(varchar(64),HASHBYTES('SHA2_256',@dv),2)),FechaActualizacion=GETDATE()
            WHERE NombreTabla=N'Patentes';
        IF @@ROWCOUNT<>1 THROW 52933,N'Patent integrity registration missing.',1;
    END;
    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT>0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;

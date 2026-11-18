-- Asigna al rol Admin (CodigoRol = 1) TODAS las patentes que existen en dbo.Patentes.
-- Es idempotente: solo inserta las que faltan y recalcula los digitos verificadores.
-- Debe ejecutarse al final, despues de las migraciones que crean patentes nuevas.
IF EXISTS (SELECT 1 FROM dbo.Roles WHERE CodigoRol = 1)
BEGIN
    INSERT INTO dbo.RolPatente (CodigoRol, CodigoPatente, DVH)
    SELECT 1, p.CodigoPatente, ''
    FROM dbo.Patentes p
    WHERE NOT EXISTS (SELECT 1 FROM dbo.RolPatente rp WHERE rp.CodigoRol = 1 AND rp.CodigoPatente = p.CodigoPatente);

    UPDATE dbo.RolPatente
    SET DVH = LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', CONCAT(
        N'CodigoPatente=', CONVERT(NVARCHAR(128), CodigoPatente), N'|',
        N'CodigoRol=', CONVERT(NVARCHAR(128), CodigoRol)
    )), 2))
    WHERE CodigoRol = 1 AND DVH = '';

    UPDATE dbo.DigitoVerificador_83KI
    SET DVV = LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', ISNULL(CAST((SELECT CAST(N'' AS NVARCHAR(MAX)) + DVH FROM dbo.RolPatente ORDER BY DVH FOR XML PATH(N'')) AS NVARCHAR(MAX)), N'')), 2)),
        FechaActualizacion = GETDATE()
    WHERE NombreTabla = N'RolPatente';
END
GO

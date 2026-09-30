SET NOCOUNT ON;
GO

DECLARE @PermisosSerializacion TABLE (CodigoPatente INT NOT NULL PRIMARY KEY, Nombre NVARCHAR(100) NOT NULL);
INSERT INTO @PermisosSerializacion (CodigoPatente, Nombre) VALUES
(68, N'Serializar comisiones'),
(69, N'Deserializar comisiones');

IF EXISTS (
    SELECT 1
    FROM dbo.Patentes p
    INNER JOIN @PermisosSerializacion ps ON ps.CodigoPatente = p.CodigoPatente
    WHERE p.Nombre <> ps.Nombre
)
BEGIN
    RAISERROR(N'No se pueden insertar permisos de serialización: uno de los códigos 68-69 ya existe con otro nombre.', 16, 1);
    RETURN;
END

IF EXISTS (
    SELECT 1
    FROM dbo.Patentes p
    INNER JOIN @PermisosSerializacion ps ON ps.Nombre = p.Nombre
    WHERE p.CodigoPatente <> ps.CodigoPatente
)
BEGIN
    RAISERROR(N'No se pueden insertar permisos de serialización: existe un permiso de serialización con otro código. Revise/repárelo manualmente antes de continuar.', 16, 1);
    RETURN;
END

SET IDENTITY_INSERT dbo.Patentes ON;

INSERT INTO dbo.Patentes (CodigoPatente, Nombre, DVH)
SELECT ps.CodigoPatente, ps.Nombre, ''
FROM @PermisosSerializacion ps
WHERE NOT EXISTS (SELECT 1 FROM dbo.Patentes p WHERE p.CodigoPatente = ps.CodigoPatente);

SET IDENTITY_INSERT dbo.Patentes OFF;

INSERT INTO dbo.RolPatente (CodigoRol, CodigoPatente, DVH)
SELECT 1, ps.CodigoPatente, ''
FROM @PermisosSerializacion ps
WHERE EXISTS (SELECT 1 FROM dbo.Roles WHERE CodigoRol = 1)
  AND EXISTS (SELECT 1 FROM dbo.Patentes p WHERE p.CodigoPatente = ps.CodigoPatente AND p.Nombre = ps.Nombre)
  AND NOT EXISTS (SELECT 1 FROM dbo.RolPatente rp WHERE rp.CodigoRol = 1 AND rp.CodigoPatente = ps.CodigoPatente);

UPDATE dbo.Patentes
SET DVH = LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', CONCAT(
    N'CodigoPatente=', CONVERT(NVARCHAR(128), CodigoPatente), N'|',
    N'Nombre=', LTRIM(RTRIM(ISNULL(CONVERT(NVARCHAR(MAX), Nombre), N'∅')))
)), 2))
WHERE CodigoPatente IN (68, 69);

UPDATE dbo.RolPatente
SET DVH = LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', CONCAT(
    N'CodigoPatente=', CONVERT(NVARCHAR(128), CodigoPatente), N'|',
    N'CodigoRol=', CONVERT(NVARCHAR(128), CodigoRol)
)), 2))
WHERE CodigoRol = 1 AND CodigoPatente IN (68, 69);

UPDATE dbo.DigitoVerificador_83KI
SET DVV = LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', ISNULL(CAST((SELECT CAST(N'' AS NVARCHAR(MAX)) + DVH FROM dbo.Patentes ORDER BY DVH FOR XML PATH(N'')) AS NVARCHAR(MAX)), N'')), 2)),
    FechaActualizacion = GETDATE()
WHERE NombreTabla = N'Patentes';

UPDATE dbo.DigitoVerificador_83KI
SET DVV = LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', ISNULL(CAST((SELECT CAST(N'' AS NVARCHAR(MAX)) + DVH FROM dbo.RolPatente ORDER BY DVH FOR XML PATH(N'')) AS NVARCHAR(MAX)), N'')), 2)),
    FechaActualizacion = GETDATE()
WHERE NombreTabla = N'RolPatente';
GO

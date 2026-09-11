-- =============================================================================
-- 002_backfill.sql — semilla dvh y dvv para filas existentes
-- calcula dvh sha-256 hex en minuscula por fila y dvv agregado por tabla.
-- excluye: BitacoraEventos (no esta en el alcance protegido).
-- prerequisitos: 001_schema.sql debe haberse aplicado primero.
-- =============================================================================

-- helper: canonicaliza un valor de columna para hashing dvh.
-- reglas (coinciden con el contrato canonico de Service/DTOs/IntegridadTablaConfig_83KI):
--   NULL      → N'∅'  (U+2205)
--   bit/bool  → N'1' or N'0'
--   datetime  → CONVERT(NVARCHAR(25), val, 126)  (ISO-8601)
--   numeros   → CONVERT(NVARCHAR, val)
--   strings   → LTRIM(RTRIM(val))
--   formato: Col=Val|Col=Val|...  (separado por pipes)
--
-- CRITICO: todos los literales string usan prefijo N'...' y todas las llamadas
-- CONVERT apuntan a NVARCHAR para que CONCAT produzca salida NVARCHAR. HASHBYTES
-- sobre NVARCHAR usa encoding UTF-16 LE, que coincide con C# Encoding.Unicode.
-- =============================================================================

SET NOCOUNT ON;
GO

-- ---------------------------------------------------------------------------
-- 1. backfill Usuarios (12 columnas canonicas, orden alfabetico segun contrato)
--    orden: Activo, Apellido, Bloqueado, CodigoRol, Contrasena, DNI, Email,
--           FechaUltimoIntento, IdiomaId, IntentosRealizados, Nombre, Username
-- ---------------------------------------------------------------------------
UPDATE dbo.Usuarios
SET DVH = LOWER(CONVERT(VARCHAR(64),
    HASHBYTES('SHA2_256',
        CONCAT(
            N'Activo=',          CASE WHEN Activo = 1 THEN N'1' ELSE N'0' END, N'|',
            N'Apellido=',        LTRIM(RTRIM(ISNULL(CONVERT(NVARCHAR(MAX), Apellido), N'∅'))), N'|',
            N'Bloqueado=',       CASE WHEN Bloqueado = 1 THEN N'1' ELSE N'0' END, N'|',
            N'CodigoRol=',       CONVERT(NVARCHAR(128), CodigoRol), N'|',
            N'Contrasena=',      LTRIM(RTRIM(ISNULL(CONVERT(NVARCHAR(MAX), Contrasena), N'∅'))), N'|',
            N'DNI=',             CONVERT(NVARCHAR(128), DNI), N'|',
            N'Email=',           LTRIM(RTRIM(ISNULL(CONVERT(NVARCHAR(MAX), Email), N'∅'))), N'|',
            N'FechaUltimoIntento=', ISNULL(CONVERT(NVARCHAR(25), FechaUltimoIntento, 126), N'∅'), N'|',
            N'IdiomaId=',        LTRIM(RTRIM(ISNULL(CONVERT(NVARCHAR(MAX), IdiomaId), N'∅'))), N'|',
            N'IntentosRealizados=', CONVERT(NVARCHAR(128), IntentosRealizados), N'|',
            N'Nombre=',          LTRIM(RTRIM(ISNULL(CONVERT(NVARCHAR(MAX), Nombre), N'∅'))), N'|',
            N'Username=',        LTRIM(RTRIM(ISNULL(CONVERT(NVARCHAR(MAX), Username), N'∅')))
        )
    ), 2));
GO

-- ---------------------------------------------------------------------------
-- 2. backfill Roles (2 columnas: CodigoRol, Nombre)
-- ---------------------------------------------------------------------------
UPDATE dbo.Roles
SET DVH = LOWER(CONVERT(VARCHAR(64),
    HASHBYTES('SHA2_256',
        CONCAT(
            N'CodigoRol=', CONVERT(NVARCHAR(128), CodigoRol), N'|',
            N'Nombre=',    LTRIM(RTRIM(ISNULL(CONVERT(NVARCHAR(MAX), Nombre), N'∅')))
        )
    ), 2));
GO

-- ---------------------------------------------------------------------------
-- 3. backfill Familias (2 columnas: CodigoFamilia, Nombre)
-- ---------------------------------------------------------------------------
UPDATE dbo.Familias
SET DVH = LOWER(CONVERT(VARCHAR(64),
    HASHBYTES('SHA2_256',
        CONCAT(
            N'CodigoFamilia=', CONVERT(NVARCHAR(128), CodigoFamilia), N'|',
            N'Nombre=',        LTRIM(RTRIM(ISNULL(CONVERT(NVARCHAR(MAX), Nombre), N'∅')))
        )
    ), 2));
GO

-- ---------------------------------------------------------------------------
-- 4. backfill Patentes (2 columnas: CodigoPatente, Nombre)
-- ---------------------------------------------------------------------------
UPDATE dbo.Patentes
SET DVH = LOWER(CONVERT(VARCHAR(64),
    HASHBYTES('SHA2_256',
        CONCAT(
            N'CodigoPatente=', CONVERT(NVARCHAR(128), CodigoPatente), N'|',
            N'Nombre=',        LTRIM(RTRIM(ISNULL(CONVERT(NVARCHAR(MAX), Nombre), N'∅')))
        )
    ), 2));
GO

-- ---------------------------------------------------------------------------
-- 5. backfill RolPatente (2 columnas: CodigoPatente, CodigoRol)
-- ---------------------------------------------------------------------------
UPDATE dbo.RolPatente
SET DVH = LOWER(CONVERT(VARCHAR(64),
    HASHBYTES('SHA2_256',
        CONCAT(
            N'CodigoPatente=', CONVERT(NVARCHAR(128), CodigoPatente), N'|',
            N'CodigoRol=',     CONVERT(NVARCHAR(128), CodigoRol)
        )
    ), 2));
GO

-- ---------------------------------------------------------------------------
-- 6. backfill RolFamilia (2 columnas: CodigoFamilia, CodigoRol)
-- ---------------------------------------------------------------------------
UPDATE dbo.RolFamilia
SET DVH = LOWER(CONVERT(VARCHAR(64),
    HASHBYTES('SHA2_256',
        CONCAT(
            N'CodigoFamilia=', CONVERT(NVARCHAR(128), CodigoFamilia), N'|',
            N'CodigoRol=',     CONVERT(NVARCHAR(128), CodigoRol)
        )
    ), 2));
GO

-- ---------------------------------------------------------------------------
-- 7. backfill FamiliaPatente (2 columnas: CodigoFamilia, CodigoPatente)
-- ---------------------------------------------------------------------------
UPDATE dbo.FamiliaPatente
SET DVH = LOWER(CONVERT(VARCHAR(64),
    HASHBYTES('SHA2_256',
        CONCAT(
            N'CodigoFamilia=', CONVERT(NVARCHAR(128), CodigoFamilia), N'|',
            N'CodigoPatente=', CONVERT(NVARCHAR(128), CodigoPatente)
        )
    ), 2));
GO

-- ---------------------------------------------------------------------------
-- 8. backfill FamiliaFamilia (2 columnas: CodigoFamiliaHija, CodigoFamiliaPadre)
-- ---------------------------------------------------------------------------
UPDATE dbo.FamiliaFamilia
SET DVH = LOWER(CONVERT(VARCHAR(64),
    HASHBYTES('SHA2_256',
        CONCAT(
            N'CodigoFamiliaHija=',  CONVERT(NVARCHAR(128), CodigoFamiliaHija), N'|',
            N'CodigoFamiliaPadre=', CONVERT(NVARCHAR(128), CodigoFamiliaPadre)
        )
    ), 2));
GO

-- ---------------------------------------------------------------------------
-- 9. semilla DigitoVerificador_83KI con agregados dvv por tabla
--    dvv = sha-256 de todos los dvh concatenados (ordenados por dvh para determinismo)
--    usa CAST(N'' AS NVARCHAR(MAX)) para que FOR XML PATH y HASHBYTES usen UTF-16 LE.
-- ---------------------------------------------------------------------------

-- Usuarios DVV
INSERT INTO dbo.DigitoVerificador_83KI (NombreTabla, DVV, FechaActualizacion)
SELECT N'Usuarios',
    LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256',
        ISNULL(
            CAST((SELECT CAST(N'' AS NVARCHAR(MAX)) + DVH FROM dbo.Usuarios ORDER BY DVH FOR XML PATH(N'')) AS NVARCHAR(MAX)),
            N'')
    ), 2)),
    GETDATE()
WHERE NOT EXISTS (SELECT 1 FROM dbo.DigitoVerificador_83KI WHERE NombreTabla = N'Usuarios');
GO

-- Roles DVV
INSERT INTO dbo.DigitoVerificador_83KI (NombreTabla, DVV, FechaActualizacion)
SELECT N'Roles',
    LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256',
        ISNULL(
            CAST((SELECT CAST(N'' AS NVARCHAR(MAX)) + DVH FROM dbo.Roles ORDER BY DVH FOR XML PATH(N'')) AS NVARCHAR(MAX)),
            N'')
    ), 2)),
    GETDATE()
WHERE NOT EXISTS (SELECT 1 FROM dbo.DigitoVerificador_83KI WHERE NombreTabla = N'Roles');
GO

-- Familias DVV
INSERT INTO dbo.DigitoVerificador_83KI (NombreTabla, DVV, FechaActualizacion)
SELECT N'Familias',
    LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256',
        ISNULL(
            CAST((SELECT CAST(N'' AS NVARCHAR(MAX)) + DVH FROM dbo.Familias ORDER BY DVH FOR XML PATH(N'')) AS NVARCHAR(MAX)),
            N'')
    ), 2)),
    GETDATE()
WHERE NOT EXISTS (SELECT 1 FROM dbo.DigitoVerificador_83KI WHERE NombreTabla = N'Familias');
GO

-- Patentes DVV
INSERT INTO dbo.DigitoVerificador_83KI (NombreTabla, DVV, FechaActualizacion)
SELECT N'Patentes',
    LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256',
        ISNULL(
            CAST((SELECT CAST(N'' AS NVARCHAR(MAX)) + DVH FROM dbo.Patentes ORDER BY DVH FOR XML PATH(N'')) AS NVARCHAR(MAX)),
            N'')
    ), 2)),
    GETDATE()
WHERE NOT EXISTS (SELECT 1 FROM dbo.DigitoVerificador_83KI WHERE NombreTabla = N'Patentes');
GO

-- RolPatente DVV
INSERT INTO dbo.DigitoVerificador_83KI (NombreTabla, DVV, FechaActualizacion)
SELECT N'RolPatente',
    LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256',
        ISNULL(
            CAST((SELECT CAST(N'' AS NVARCHAR(MAX)) + DVH FROM dbo.RolPatente ORDER BY DVH FOR XML PATH(N'')) AS NVARCHAR(MAX)),
            N'')
    ), 2)),
    GETDATE()
WHERE NOT EXISTS (SELECT 1 FROM dbo.DigitoVerificador_83KI WHERE NombreTabla = N'RolPatente');
GO

-- RolFamilia DVV
INSERT INTO dbo.DigitoVerificador_83KI (NombreTabla, DVV, FechaActualizacion)
SELECT N'RolFamilia',
    LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256',
        ISNULL(
            CAST((SELECT CAST(N'' AS NVARCHAR(MAX)) + DVH FROM dbo.RolFamilia ORDER BY DVH FOR XML PATH(N'')) AS NVARCHAR(MAX)),
            N'')
    ), 2)),
    GETDATE()
WHERE NOT EXISTS (SELECT 1 FROM dbo.DigitoVerificador_83KI WHERE NombreTabla = N'RolFamilia');
GO

-- FamiliaPatente DVV
INSERT INTO dbo.DigitoVerificador_83KI (NombreTabla, DVV, FechaActualizacion)
SELECT N'FamiliaPatente',
    LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256',
        ISNULL(
            CAST((SELECT CAST(N'' AS NVARCHAR(MAX)) + DVH FROM dbo.FamiliaPatente ORDER BY DVH FOR XML PATH(N'')) AS NVARCHAR(MAX)),
            N'')
    ), 2)),
    GETDATE()
WHERE NOT EXISTS (SELECT 1 FROM dbo.DigitoVerificador_83KI WHERE NombreTabla = N'FamiliaPatente');
GO

-- FamiliaFamilia DVV
INSERT INTO dbo.DigitoVerificador_83KI (NombreTabla, DVV, FechaActualizacion)
SELECT N'FamiliaFamilia',
    LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256',
        ISNULL(
            CAST((SELECT CAST(N'' AS NVARCHAR(MAX)) + DVH FROM dbo.FamiliaFamilia ORDER BY DVH FOR XML PATH(N'')) AS NVARCHAR(MAX)),
            N'')
    ), 2)),
    GETDATE()
WHERE NOT EXISTS (SELECT 1 FROM dbo.DigitoVerificador_83KI WHERE NombreTabla = N'FamiliaFamilia');
GO

PRINT '002_backfill.sql aplicado: DVH backfilleado + semillas DVV pobladas para las 8 tablas protegidas (unicode-safe).';
GO

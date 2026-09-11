-- =============================================================================
-- 003_restore-smoke.sql — verificacion de integridad con recomputacion dvh desde datos vivos
-- recalcula dvh por fila desde valores de columna actuales usando la misma canonicalizacion
-- que 002_backfill.sql, luego agrega los dvh recalculados en dvv para comparacion.
-- detecta ediciones directas sql de columnas (ej. UPDATE sin refrescar dvh).
-- prerequisitos: 001_schema.sql y 002_backfill.sql aplicados.
--
-- CRITICO: todos los literales string usan prefijo N'...'; todas las llamadas CONVERT
-- apuntan a NVARCHAR para que CONCAT → HASHBYTES use UTF-16 LE (coincide con C# Encoding.Unicode).
-- =============================================================================

SET NOCOUNT ON;
GO

-- =============================================================================
-- PARTE A: verificacion dvh por fila (PASS/FAIL a nivel fila)
-- recalcula dvh desde datos vivos, compara contra dvh almacenado.
-- =============================================================================
PRINT '=== Verificacion DVH por fila (datos vivos vs DVH almacenado) ===';
GO

-- Usuarios ----------------------------------------------------------------
SELECT N'Usuarios' AS Tabla, DNI AS PK, DVH AS DVHAlmacenado,
    LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256',
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
    ), 2)) AS DVHCalculado,
    CASE WHEN DVH = LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256',
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
    ), 2)) THEN 'PASS' ELSE 'FAIL' END AS Estado
FROM dbo.Usuarios
WHERE DVH <> LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256',
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

-- Roles -------------------------------------------------------------------
SELECT N'Roles' AS Tabla, CodigoRol AS PK, DVH AS DVHAlmacenado,
    LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256',
        CONCAT(
            N'CodigoRol=', CONVERT(NVARCHAR(128), CodigoRol), N'|',
            N'Nombre=',    LTRIM(RTRIM(ISNULL(CONVERT(NVARCHAR(MAX), Nombre), N'∅')))
        )
    ), 2)) AS DVHCalculado,
    CASE WHEN DVH = LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256',
        CONCAT(
            N'CodigoRol=', CONVERT(NVARCHAR(128), CodigoRol), N'|',
            N'Nombre=',    LTRIM(RTRIM(ISNULL(CONVERT(NVARCHAR(MAX), Nombre), N'∅')))
        )
    ), 2)) THEN 'PASS' ELSE 'FAIL' END AS Estado
FROM dbo.Roles
WHERE DVH <> LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256',
    CONCAT(
        N'CodigoRol=', CONVERT(NVARCHAR(128), CodigoRol), N'|',
        N'Nombre=',    LTRIM(RTRIM(ISNULL(CONVERT(NVARCHAR(MAX), Nombre), N'∅')))
    )
), 2));
GO

-- Familias -----------------------------------------------------------------
SELECT N'Familias' AS Tabla, CodigoFamilia AS PK, DVH AS DVHAlmacenado,
    LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256',
        CONCAT(
            N'CodigoFamilia=', CONVERT(NVARCHAR(128), CodigoFamilia), N'|',
            N'Nombre=',        LTRIM(RTRIM(ISNULL(CONVERT(NVARCHAR(MAX), Nombre), N'∅')))
        )
    ), 2)) AS DVHCalculado,
    CASE WHEN DVH = LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256',
        CONCAT(
            N'CodigoFamilia=', CONVERT(NVARCHAR(128), CodigoFamilia), N'|',
            N'Nombre=',        LTRIM(RTRIM(ISNULL(CONVERT(NVARCHAR(MAX), Nombre), N'∅')))
        )
    ), 2)) THEN 'PASS' ELSE 'FAIL' END AS Estado
FROM dbo.Familias
WHERE DVH <> LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256',
    CONCAT(
        N'CodigoFamilia=', CONVERT(NVARCHAR(128), CodigoFamilia), N'|',
        N'Nombre=',        LTRIM(RTRIM(ISNULL(CONVERT(NVARCHAR(MAX), Nombre), N'∅')))
    )
), 2));
GO

-- Patentes -----------------------------------------------------------------
SELECT N'Patentes' AS Tabla, CodigoPatente AS PK, DVH AS DVHAlmacenado,
    LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256',
        CONCAT(
            N'CodigoPatente=', CONVERT(NVARCHAR(128), CodigoPatente), N'|',
            N'Nombre=',        LTRIM(RTRIM(ISNULL(CONVERT(NVARCHAR(MAX), Nombre), N'∅')))
        )
    ), 2)) AS DVHCalculado,
    CASE WHEN DVH = LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256',
        CONCAT(
            N'CodigoPatente=', CONVERT(NVARCHAR(128), CodigoPatente), N'|',
            N'Nombre=',        LTRIM(RTRIM(ISNULL(CONVERT(NVARCHAR(MAX), Nombre), N'∅')))
        )
    ), 2)) THEN 'PASS' ELSE 'FAIL' END AS Estado
FROM dbo.Patentes
WHERE DVH <> LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256',
    CONCAT(
        N'CodigoPatente=', CONVERT(NVARCHAR(128), CodigoPatente), N'|',
        N'Nombre=',        LTRIM(RTRIM(ISNULL(CONVERT(NVARCHAR(MAX), Nombre), N'∅')))
    )
), 2));
GO

-- RolPatente ---------------------------------------------------------------
SELECT N'RolPatente' AS Tabla,
    CONCAT(N'Rol=', CodigoRol, N', Patente=', CodigoPatente) AS PK,
    DVH AS DVHAlmacenado,
    LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256',
        CONCAT(
            N'CodigoPatente=', CONVERT(NVARCHAR(128), CodigoPatente), N'|',
            N'CodigoRol=',     CONVERT(NVARCHAR(128), CodigoRol)
        )
    ), 2)) AS DVHCalculado,
    CASE WHEN DVH = LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256',
        CONCAT(
            N'CodigoPatente=', CONVERT(NVARCHAR(128), CodigoPatente), N'|',
            N'CodigoRol=',     CONVERT(NVARCHAR(128), CodigoRol)
        )
    ), 2)) THEN 'PASS' ELSE 'FAIL' END AS Estado
FROM dbo.RolPatente
WHERE DVH <> LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256',
    CONCAT(
        N'CodigoPatente=', CONVERT(NVARCHAR(128), CodigoPatente), N'|',
        N'CodigoRol=',     CONVERT(NVARCHAR(128), CodigoRol)
    )
), 2));
GO

-- RolFamilia ---------------------------------------------------------------
SELECT N'RolFamilia' AS Tabla,
    CONCAT(N'Rol=', CodigoRol, N', Familia=', CodigoFamilia) AS PK,
    DVH AS DVHAlmacenado,
    LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256',
        CONCAT(
            N'CodigoFamilia=', CONVERT(NVARCHAR(128), CodigoFamilia), N'|',
            N'CodigoRol=',     CONVERT(NVARCHAR(128), CodigoRol)
        )
    ), 2)) AS DVHCalculado,
    CASE WHEN DVH = LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256',
        CONCAT(
            N'CodigoFamilia=', CONVERT(NVARCHAR(128), CodigoFamilia), N'|',
            N'CodigoRol=',     CONVERT(NVARCHAR(128), CodigoRol)
        )
    ), 2)) THEN 'PASS' ELSE 'FAIL' END AS Estado
FROM dbo.RolFamilia
WHERE DVH <> LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256',
    CONCAT(
        N'CodigoFamilia=', CONVERT(NVARCHAR(128), CodigoFamilia), N'|',
        N'CodigoRol=',     CONVERT(NVARCHAR(128), CodigoRol)
    )
), 2));
GO

-- FamiliaPatente -----------------------------------------------------------
SELECT N'FamiliaPatente' AS Tabla,
    CONCAT(N'Familia=', CodigoFamilia, N', Patente=', CodigoPatente) AS PK,
    DVH AS DVHAlmacenado,
    LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256',
        CONCAT(
            N'CodigoFamilia=', CONVERT(NVARCHAR(128), CodigoFamilia), N'|',
            N'CodigoPatente=', CONVERT(NVARCHAR(128), CodigoPatente)
        )
    ), 2)) AS DVHCalculado,
    CASE WHEN DVH = LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256',
        CONCAT(
            N'CodigoFamilia=', CONVERT(NVARCHAR(128), CodigoFamilia), N'|',
            N'CodigoPatente=', CONVERT(NVARCHAR(128), CodigoPatente)
        )
    ), 2)) THEN 'PASS' ELSE 'FAIL' END AS Estado
FROM dbo.FamiliaPatente
WHERE DVH <> LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256',
    CONCAT(
        N'CodigoFamilia=', CONVERT(NVARCHAR(128), CodigoFamilia), N'|',
        N'CodigoPatente=', CONVERT(NVARCHAR(128), CodigoPatente)
    )
), 2));
GO

-- FamiliaFamilia -----------------------------------------------------------
SELECT N'FamiliaFamilia' AS Tabla,
    CONCAT(N'Padre=', CodigoFamiliaPadre, N', Hija=', CodigoFamiliaHija) AS PK,
    DVH AS DVHAlmacenado,
    LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256',
        CONCAT(
            N'CodigoFamiliaHija=',  CONVERT(NVARCHAR(128), CodigoFamiliaHija), N'|',
            N'CodigoFamiliaPadre=', CONVERT(NVARCHAR(128), CodigoFamiliaPadre)
        )
    ), 2)) AS DVHCalculado,
    CASE WHEN DVH = LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256',
        CONCAT(
            N'CodigoFamiliaHija=',  CONVERT(NVARCHAR(128), CodigoFamiliaHija), N'|',
            N'CodigoFamiliaPadre=', CONVERT(NVARCHAR(128), CodigoFamiliaPadre)
        )
    ), 2)) THEN 'PASS' ELSE 'FAIL' END AS Estado
FROM dbo.FamiliaFamilia
WHERE DVH <> LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256',
    CONCAT(
        N'CodigoFamiliaHija=',  CONVERT(NVARCHAR(128), CodigoFamiliaHija), N'|',
        N'CodigoFamiliaPadre=', CONVERT(NVARCHAR(128), CodigoFamiliaPadre)
    )
), 2));
GO

-- =============================================================================
-- PARTE B: verificacion dvv por tabla (PASS/FAIL a nivel tabla)
-- recalcula dvv desde dvh recien recalculados, compara contra dvv almacenado.
-- los valores dvh se agregan y hashean usando NVARCHAR para coincidir con C# UTF-16 LE.
-- =============================================================================
PRINT '=== Verificacion DVV por tabla (DVH recalculado -> DVV vs DVV almacenado) ===';
GO

WITH UsuariosDVH AS (
    SELECT LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256',
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
    ), 2)) AS DVH
    FROM dbo.Usuarios
),
RolesDVH AS (
    SELECT LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256',
        CONCAT(N'CodigoRol=', CONVERT(NVARCHAR(128), CodigoRol), N'|', N'Nombre=', LTRIM(RTRIM(ISNULL(CONVERT(NVARCHAR(MAX), Nombre), N'∅'))))
    ), 2)) AS DVH FROM dbo.Roles
),
FamiliasDVH AS (
    SELECT LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256',
        CONCAT(N'CodigoFamilia=', CONVERT(NVARCHAR(128), CodigoFamilia), N'|', N'Nombre=', LTRIM(RTRIM(ISNULL(CONVERT(NVARCHAR(MAX), Nombre), N'∅'))))
    ), 2)) AS DVH FROM dbo.Familias
),
PatentesDVH AS (
    SELECT LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256',
        CONCAT(N'CodigoPatente=', CONVERT(NVARCHAR(128), CodigoPatente), N'|', N'Nombre=', LTRIM(RTRIM(ISNULL(CONVERT(NVARCHAR(MAX), Nombre), N'∅'))))
    ), 2)) AS DVH FROM dbo.Patentes
),
RolPatenteDVH AS (
    SELECT LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256',
        CONCAT(N'CodigoPatente=', CONVERT(NVARCHAR(128), CodigoPatente), N'|', N'CodigoRol=', CONVERT(NVARCHAR(128), CodigoRol))
    ), 2)) AS DVH FROM dbo.RolPatente
),
RolFamiliaDVH AS (
    SELECT LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256',
        CONCAT(N'CodigoFamilia=', CONVERT(NVARCHAR(128), CodigoFamilia), N'|', N'CodigoRol=', CONVERT(NVARCHAR(128), CodigoRol))
    ), 2)) AS DVH FROM dbo.RolFamilia
),
FamiliaPatenteDVH AS (
    SELECT LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256',
        CONCAT(N'CodigoFamilia=', CONVERT(NVARCHAR(128), CodigoFamilia), N'|', N'CodigoPatente=', CONVERT(NVARCHAR(128), CodigoPatente))
    ), 2)) AS DVH FROM dbo.FamiliaPatente
),
FamiliaFamiliaDVH AS (
    SELECT LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256',
        CONCAT(N'CodigoFamiliaHija=', CONVERT(NVARCHAR(128), CodigoFamiliaHija), N'|', N'CodigoFamiliaPadre=', CONVERT(NVARCHAR(128), CodigoFamiliaPadre))
    ), 2)) AS DVH FROM dbo.FamiliaFamilia
)
SELECT NombreTabla, DVVCalculado, DVVAlmacenado,
    CASE
        WHEN DVVCalculado = DVVAlmacenado THEN 'PASS'
        WHEN DVVAlmacenado IS NULL THEN 'NO ENTRY'
        ELSE 'FAIL'
    END AS Estado
FROM (
    SELECT N'Usuarios' AS NombreTabla,
        LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256',
            ISNULL(CAST((SELECT CAST(N'' AS NVARCHAR(MAX)) + DVH FROM UsuariosDVH ORDER BY DVH FOR XML PATH(N'')) AS NVARCHAR(MAX)), N'')
        ), 2)) AS DVVCalculado,
        (SELECT DVV FROM dbo.DigitoVerificador_83KI WHERE NombreTabla = N'Usuarios') AS DVVAlmacenado
    UNION ALL
    SELECT N'Roles',
        LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256',
            ISNULL(CAST((SELECT CAST(N'' AS NVARCHAR(MAX)) + DVH FROM RolesDVH ORDER BY DVH FOR XML PATH(N'')) AS NVARCHAR(MAX)), N'')
        ), 2)),
        (SELECT DVV FROM dbo.DigitoVerificador_83KI WHERE NombreTabla = N'Roles')
    UNION ALL
    SELECT N'Familias',
        LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256',
            ISNULL(CAST((SELECT CAST(N'' AS NVARCHAR(MAX)) + DVH FROM FamiliasDVH ORDER BY DVH FOR XML PATH(N'')) AS NVARCHAR(MAX)), N'')
        ), 2)),
        (SELECT DVV FROM dbo.DigitoVerificador_83KI WHERE NombreTabla = N'Familias')
    UNION ALL
    SELECT N'Patentes',
        LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256',
            ISNULL(CAST((SELECT CAST(N'' AS NVARCHAR(MAX)) + DVH FROM PatentesDVH ORDER BY DVH FOR XML PATH(N'')) AS NVARCHAR(MAX)), N'')
        ), 2)),
        (SELECT DVV FROM dbo.DigitoVerificador_83KI WHERE NombreTabla = N'Patentes')
    UNION ALL
    SELECT N'RolPatente',
        LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256',
            ISNULL(CAST((SELECT CAST(N'' AS NVARCHAR(MAX)) + DVH FROM RolPatenteDVH ORDER BY DVH FOR XML PATH(N'')) AS NVARCHAR(MAX)), N'')
        ), 2)),
        (SELECT DVV FROM dbo.DigitoVerificador_83KI WHERE NombreTabla = N'RolPatente')
    UNION ALL
    SELECT N'RolFamilia',
        LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256',
            ISNULL(CAST((SELECT CAST(N'' AS NVARCHAR(MAX)) + DVH FROM RolFamiliaDVH ORDER BY DVH FOR XML PATH(N'')) AS NVARCHAR(MAX)), N'')
        ), 2)),
        (SELECT DVV FROM dbo.DigitoVerificador_83KI WHERE NombreTabla = N'RolFamilia')
    UNION ALL
    SELECT N'FamiliaPatente',
        LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256',
            ISNULL(CAST((SELECT CAST(N'' AS NVARCHAR(MAX)) + DVH FROM FamiliaPatenteDVH ORDER BY DVH FOR XML PATH(N'')) AS NVARCHAR(MAX)), N'')
        ), 2)),
        (SELECT DVV FROM dbo.DigitoVerificador_83KI WHERE NombreTabla = N'FamiliaPatente')
    UNION ALL
    SELECT N'FamiliaFamilia',
        LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256',
            ISNULL(CAST((SELECT CAST(N'' AS NVARCHAR(MAX)) + DVH FROM FamiliaFamiliaDVH ORDER BY DVH FOR XML PATH(N'')) AS NVARCHAR(MAX)), N'')
        ), 2)),
        (SELECT DVV FROM dbo.DigitoVerificador_83KI WHERE NombreTabla = N'FamiliaFamilia')
) AS Resultados
ORDER BY NombreTabla;
GO

PRINT '003_restore-smoke.sql: verificacion desde datos vivos completa (unicode-safe).';
PRINT 'Parte A muestra discrepancias DVH por fila (vacio = todas las filas PASS).';
PRINT 'Parte B muestra discrepancias DVV por tabla (recalculado desde datos vivos).';
GO

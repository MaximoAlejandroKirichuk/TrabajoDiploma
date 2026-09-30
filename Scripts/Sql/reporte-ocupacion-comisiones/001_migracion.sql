SET NOCOUNT ON;
GO

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

-- Reporte de ocupacion y quorum de comisiones
-- Requiere CUN01, CUN03, CUN05 y CUN06 aplicados.

IF OBJECT_ID(N'dbo.AltaOficialComision', N'U') IS NULL OR OBJECT_ID(N'dbo.ActaCierreComision', N'U') IS NULL OR OBJECT_ID(N'dbo.PagoInscripcion', N'U') IS NULL
BEGIN
    RAISERROR(N'Faltan tablas de CUN05/CUN06: aplique esas migraciones antes del reporte.', 16, 1);
    RETURN;
END
GO

-- Permiso 70
IF EXISTS (SELECT 1 FROM dbo.Patentes WHERE CodigoPatente = 70 AND Nombre <> N'Ver reporte de ocupación de comisiones')
BEGIN
    RAISERROR(N'El código de permiso 70 ya existe con otro nombre.', 16, 1);
    RETURN;
END

SET IDENTITY_INSERT dbo.Patentes ON;
INSERT INTO dbo.Patentes (CodigoPatente, Nombre, DVH)
SELECT 70, N'Ver reporte de ocupación de comisiones', ''
WHERE NOT EXISTS (SELECT 1 FROM dbo.Patentes WHERE CodigoPatente = 70);
SET IDENTITY_INSERT dbo.Patentes OFF;

INSERT INTO dbo.RolPatente (CodigoRol, CodigoPatente, DVH)
SELECT 1, 70, ''
WHERE EXISTS (SELECT 1 FROM dbo.Roles WHERE CodigoRol = 1)
  AND NOT EXISTS (SELECT 1 FROM dbo.RolPatente WHERE CodigoRol = 1 AND CodigoPatente = 70);

UPDATE dbo.Patentes
SET DVH = LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', CONCAT(
    N'CodigoPatente=', CONVERT(NVARCHAR(128), CodigoPatente), N'|',
    N'Nombre=', LTRIM(RTRIM(ISNULL(CONVERT(NVARCHAR(MAX), Nombre), N'∅')))
)), 2))
WHERE CodigoPatente = 70;

UPDATE dbo.RolPatente
SET DVH = LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', CONCAT(
    N'CodigoPatente=', CONVERT(NVARCHAR(128), CodigoPatente), N'|',
    N'CodigoRol=', CONVERT(NVARCHAR(128), CodigoRol)
)), 2))
WHERE CodigoRol = 1 AND CodigoPatente = 70;

UPDATE dbo.DigitoVerificador_83KI
SET DVV = LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', ISNULL(CAST((SELECT CAST(N'' AS NVARCHAR(MAX)) + DVH FROM dbo.Patentes ORDER BY DVH FOR XML PATH(N'')) AS NVARCHAR(MAX)), N'')), 2)),
    FechaActualizacion = GETDATE()
WHERE NombreTabla = N'Patentes';

UPDATE dbo.DigitoVerificador_83KI
SET DVV = LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', ISNULL(CAST((SELECT CAST(N'' AS NVARCHAR(MAX)) + DVH FROM dbo.RolPatente ORDER BY DVH FOR XML PATH(N'')) AS NVARCHAR(MAX)), N'')), 2)),
    FechaActualizacion = GETDATE()
WHERE NombreTabla = N'RolPatente';
GO

IF OBJECT_ID(N'dbo.sp_Reporte_OcupacionComisiones', N'P') IS NOT NULL DROP PROCEDURE dbo.sp_Reporte_OcupacionComisiones;
GO
CREATE PROCEDURE dbo.sp_Reporte_OcupacionComisiones
    @Estado NVARCHAR(30) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT c.IdComision,
           c.Codigo,
           cur.Nombre AS Curso,
           CONCAT(p.Apellido, N', ', p.Nombre) AS Profesor,
           c.FechaLimitePago,
           c.CupoMinimo,
           c.CupoMaximo,
           c.Estado,
           ISNULL(ins.Inscriptos, 0) AS Inscriptos,
           ISNULL(ins.Pagaron, 0) AS Pagaron,
           ISNULL(rei.CuotasReintegro, 0) AS CuotasReintegro,
           ISNULL(rei.MontoReintegro, 0) AS MontoReintegro,
           COALESCE(alta.NumeroActa, cierre.NumeroActa, N'') AS NumeroActa
    FROM dbo.Comision c
    INNER JOIN dbo.Curso cur ON cur.IdCurso = c.IdCurso
    INNER JOIN dbo.Profesor p ON p.IdProfesor = c.IdProfesor
    -- inscriptos y cuantos pagaron la cuota 1
    -- (si la comision se cancelo, las solicitudes quedan 'cancelada' y la cuota en 'reintegro_pendiente')
    OUTER APPLY (
        SELECT COUNT(1) AS Inscriptos,
               SUM(CASE WHEN cu.Estado IN (N'pagada', N'reintegro_pendiente') THEN 1 ELSE 0 END) AS Pagaron
        FROM dbo.SolicitudInscripcion si
        LEFT JOIN dbo.Cuota cu ON cu.IdSolicitudInscripcion = si.IdSolicitudInscripcion AND cu.NumeroCuota = 1
        WHERE si.IdComision = c.IdComision
          AND si.Estado IN (N'pendiente', N'confirmada', N'activa', N'cancelada')
    ) ins
    -- lo que hay que devolver: lo efectivamente pagado de las cuotas marcadas por CUN06
    OUTER APPLY (
        SELECT COUNT(DISTINCT cu.IdCuota) AS CuotasReintegro,
               SUM(pg.MontoPagado) AS MontoReintegro
        FROM dbo.Cuota cu
        INNER JOIN dbo.SolicitudInscripcion si ON si.IdSolicitudInscripcion = cu.IdSolicitudInscripcion
        LEFT JOIN dbo.PagoInscripcion pg ON pg.IdCuota = cu.IdCuota
        WHERE si.IdComision = c.IdComision
          AND cu.Estado = N'reintegro_pendiente'
    ) rei
    OUTER APPLY (
        SELECT TOP 1 a.NumeroActa FROM dbo.AltaOficialComision a
        WHERE a.IdComision = c.IdComision ORDER BY a.IdAltaOficialComision DESC
    ) alta
    OUTER APPLY (
        SELECT TOP 1 ac.NumeroActa FROM dbo.ActaCierreComision ac
        WHERE ac.IdComision = c.IdComision ORDER BY ac.IdActaCierreComision DESC
    ) cierre
    WHERE c.Estado <> N'eliminada'
      AND (@Estado IS NULL OR c.Estado = @Estado)
    ORDER BY c.FechaLimitePago, c.Codigo;
END
GO

PRINT 'Reporte de ocupacion de comisiones aplicado.';
GO

SET NOCOUNT ON;
GO

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

IF OBJECT_ID(N'dbo.Comision', N'U') IS NULL OR OBJECT_ID(N'dbo.SolicitudInscripcion', N'U') IS NULL OR OBJECT_ID(N'dbo.Cuota', N'U') IS NULL
BEGIN
    RAISERROR(N'Tablas base CUN01/CUN03 no disponibles: aplique el esquema completo antes de CUN06.', 16, 1);
    RETURN;
END
GO

IF OBJECT_ID(N'dbo.AltaOficialComision', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.AltaOficialComision(
        IdAltaOficialComision INT IDENTITY(1,1) NOT NULL,
        IdComision INT NOT NULL,
        FechaAlta DATE NOT NULL,
        NumeroActa NVARCHAR(50) NOT NULL,
        Observaciones NVARCHAR(500) NOT NULL CONSTRAINT DF_AltaOficialComision_Observaciones DEFAULT (N''),
        UsuarioRegistro NVARCHAR(100) NOT NULL,
        FechaRegistro DATETIME NOT NULL CONSTRAINT DF_AltaOficialComision_FechaRegistro DEFAULT (GETDATE()),
        DVH VARCHAR(64) NOT NULL CONSTRAINT DF_AltaOficialComision_DVH DEFAULT (''),
        CONSTRAINT PK_AltaOficialComision PRIMARY KEY CLUSTERED (IdAltaOficialComision),
        CONSTRAINT FK_AltaOficialComision_Comision FOREIGN KEY (IdComision) REFERENCES dbo.Comision(IdComision),
        CONSTRAINT UQ_AltaOficialComision_Comision UNIQUE (IdComision),
        CONSTRAINT UQ_AltaOficialComision_NumeroActa UNIQUE (NumeroActa)
    );
END
GO

IF OBJECT_ID(N'dbo.ActaCierreComision', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ActaCierreComision(
        IdActaCierreComision INT IDENTITY(1,1) NOT NULL,
        IdComision INT NOT NULL,
        FechaCierre DATE NOT NULL,
        NumeroActa NVARCHAR(50) NOT NULL,
        Motivo NVARCHAR(250) NOT NULL,
        Observaciones NVARCHAR(500) NOT NULL CONSTRAINT DF_ActaCierreComision_Observaciones DEFAULT (N''),
        UsuarioRegistro NVARCHAR(100) NOT NULL,
        FechaRegistro DATETIME NOT NULL CONSTRAINT DF_ActaCierreComision_FechaRegistro DEFAULT (GETDATE()),
        DVH VARCHAR(64) NOT NULL CONSTRAINT DF_ActaCierreComision_DVH DEFAULT (''),
        CONSTRAINT PK_ActaCierreComision PRIMARY KEY CLUSTERED (IdActaCierreComision),
        CONSTRAINT FK_ActaCierreComision_Comision FOREIGN KEY (IdComision) REFERENCES dbo.Comision(IdComision),
        CONSTRAINT UQ_ActaCierreComision_Comision UNIQUE (IdComision),
        CONSTRAINT UQ_ActaCierreComision_NumeroActa UNIQUE (NumeroActa)
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_AltaOficialComision_Comision' AND object_id = OBJECT_ID(N'dbo.AltaOficialComision'))
    CREATE INDEX IX_AltaOficialComision_Comision ON dbo.AltaOficialComision(IdComision);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_ActaCierreComision_Comision' AND object_id = OBJECT_ID(N'dbo.ActaCierreComision'))
    CREATE INDEX IX_ActaCierreComision_Comision ON dbo.ActaCierreComision(IdComision);
GO

IF NOT EXISTS (SELECT 1 FROM dbo.DigitoVerificador_83KI WHERE NombreTabla = N'AltaOficialComision')
    INSERT INTO dbo.DigitoVerificador_83KI (NombreTabla, DVV, FechaActualizacion) VALUES (N'AltaOficialComision', '', GETDATE());
IF NOT EXISTS (SELECT 1 FROM dbo.DigitoVerificador_83KI WHERE NombreTabla = N'ActaCierreComision')
    INSERT INTO dbo.DigitoVerificador_83KI (NombreTabla, DVV, FechaActualizacion) VALUES (N'ActaCierreComision', '', GETDATE());
GO

UPDATE p SET Nombre = N'Registrar estado definitivo de comisión' FROM dbo.Patentes p WHERE p.CodigoPatente = 67 AND p.Nombre <> N'Registrar estado definitivo de comisión';
SET IDENTITY_INSERT dbo.Patentes ON;
INSERT INTO dbo.Patentes (CodigoPatente, Nombre, DVH)
SELECT 67, N'Registrar estado definitivo de comisión', ''
WHERE NOT EXISTS (SELECT 1 FROM dbo.Patentes WHERE CodigoPatente = 67);
SET IDENTITY_INSERT dbo.Patentes OFF;

INSERT INTO dbo.RolPatente (CodigoRol, CodigoPatente, DVH)
SELECT 1, 67, ''
WHERE EXISTS (SELECT 1 FROM dbo.Roles WHERE CodigoRol = 1)
  AND NOT EXISTS (SELECT 1 FROM dbo.RolPatente WHERE CodigoRol = 1 AND CodigoPatente = 67);
GO

-- Recalculate integrity metadata for CUN06-protected additions and permission seeds.
-- This keeps startup verification healthy immediately after running the migration,
-- before any manual integrity recovery action is available.
UPDATE dbo.Patentes
SET DVH = LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', CONCAT(
    N'CodigoPatente=', CONVERT(NVARCHAR(128), CodigoPatente), N'|',
    N'Nombre=', LTRIM(RTRIM(ISNULL(CONVERT(NVARCHAR(MAX), Nombre), N'∅')))
)), 2))
WHERE CodigoPatente = 67;

UPDATE dbo.RolPatente
SET DVH = LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', CONCAT(
    N'CodigoPatente=', CONVERT(NVARCHAR(128), CodigoPatente), N'|',
    N'CodigoRol=', CONVERT(NVARCHAR(128), CodigoRol)
)), 2))
WHERE CodigoRol = 1 AND CodigoPatente = 67;

UPDATE dbo.AltaOficialComision
SET DVH = LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', CONCAT(
    N'FechaAlta=', ISNULL(CONVERT(NVARCHAR(23), CAST(FechaAlta AS DATETIME), 126), N'∅'), N'|',
    N'FechaRegistro=', ISNULL(CONVERT(NVARCHAR(23), FechaRegistro, 126), N'∅'), N'|',
    N'IdAltaOficialComision=', CONVERT(NVARCHAR(128), IdAltaOficialComision), N'|',
    N'IdComision=', CONVERT(NVARCHAR(128), IdComision), N'|',
    N'NumeroActa=', LTRIM(RTRIM(ISNULL(CONVERT(NVARCHAR(MAX), NumeroActa), N'∅'))), N'|',
    N'Observaciones=', LTRIM(RTRIM(ISNULL(CONVERT(NVARCHAR(MAX), Observaciones), N'∅'))), N'|',
    N'UsuarioRegistro=', LTRIM(RTRIM(ISNULL(CONVERT(NVARCHAR(MAX), UsuarioRegistro), N'∅')))
)), 2));

UPDATE dbo.ActaCierreComision
SET DVH = LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', CONCAT(
    N'FechaCierre=', ISNULL(CONVERT(NVARCHAR(23), CAST(FechaCierre AS DATETIME), 126), N'∅'), N'|',
    N'FechaRegistro=', ISNULL(CONVERT(NVARCHAR(23), FechaRegistro, 126), N'∅'), N'|',
    N'IdActaCierreComision=', CONVERT(NVARCHAR(128), IdActaCierreComision), N'|',
    N'IdComision=', CONVERT(NVARCHAR(128), IdComision), N'|',
    N'Motivo=', LTRIM(RTRIM(ISNULL(CONVERT(NVARCHAR(MAX), Motivo), N'∅'))), N'|',
    N'NumeroActa=', LTRIM(RTRIM(ISNULL(CONVERT(NVARCHAR(MAX), NumeroActa), N'∅'))), N'|',
    N'Observaciones=', LTRIM(RTRIM(ISNULL(CONVERT(NVARCHAR(MAX), Observaciones), N'∅'))), N'|',
    N'UsuarioRegistro=', LTRIM(RTRIM(ISNULL(CONVERT(NVARCHAR(MAX), UsuarioRegistro), N'∅')))
)), 2));

UPDATE dbo.DigitoVerificador_83KI
SET DVV = LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', ISNULL(CAST((SELECT CAST(N'' AS NVARCHAR(MAX)) + DVH FROM dbo.Patentes ORDER BY DVH FOR XML PATH(N'')) AS NVARCHAR(MAX)), N'')), 2)),
    FechaActualizacion = GETDATE()
WHERE NombreTabla = N'Patentes';

UPDATE dbo.DigitoVerificador_83KI
SET DVV = LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', ISNULL(CAST((SELECT CAST(N'' AS NVARCHAR(MAX)) + DVH FROM dbo.RolPatente ORDER BY DVH FOR XML PATH(N'')) AS NVARCHAR(MAX)), N'')), 2)),
    FechaActualizacion = GETDATE()
WHERE NombreTabla = N'RolPatente';

UPDATE dbo.DigitoVerificador_83KI
SET DVV = LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', ISNULL(CAST((SELECT CAST(N'' AS NVARCHAR(MAX)) + DVH FROM dbo.AltaOficialComision ORDER BY DVH FOR XML PATH(N'')) AS NVARCHAR(MAX)), N'')), 2)),
    FechaActualizacion = GETDATE()
WHERE NombreTabla = N'AltaOficialComision';

UPDATE dbo.DigitoVerificador_83KI
SET DVV = LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', ISNULL(CAST((SELECT CAST(N'' AS NVARCHAR(MAX)) + DVH FROM dbo.ActaCierreComision ORDER BY DVH FOR XML PATH(N'')) AS NVARCHAR(MAX)), N'')), 2)),
    FechaActualizacion = GETDATE()
WHERE NombreTabla = N'ActaCierreComision';
GO

IF OBJECT_ID(N'dbo.sp_CUN06_ListarComisionesVencidas', N'P') IS NOT NULL DROP PROCEDURE dbo.sp_CUN06_ListarComisionesVencidas;
GO
CREATE PROCEDURE dbo.sp_CUN06_ListarComisionesVencidas
AS
BEGIN
    SET NOCOUNT ON;
    SELECT c.IdComision, c.Codigo, cur.Nombre AS Curso, CONCAT(p.Apellido, N', ', p.Nombre) AS Profesor,
           c.CupoMinimo, c.CupoMaximo, c.FechaLimitePago, c.Estado,
           ISNULL(v.VacantesRegularizadas, 0) AS VacantesRegularizadas
    FROM dbo.Comision c
    INNER JOIN dbo.Curso cur ON cur.IdCurso = c.IdCurso
    INNER JOIN dbo.Profesor p ON p.IdProfesor = c.IdProfesor
    OUTER APPLY (
        SELECT COUNT(1) AS VacantesRegularizadas
        FROM dbo.SolicitudInscripcion si
        INNER JOIN dbo.Cuota cu ON cu.IdSolicitudInscripcion = si.IdSolicitudInscripcion AND cu.NumeroCuota = 1 AND cu.Estado = N'pagada'
        WHERE si.IdComision = c.IdComision AND si.Estado = N'activa'
    ) v
    WHERE c.Estado = N'preapertura'
      AND c.FechaLimitePago < CONVERT(date, GETDATE())
    ORDER BY c.FechaLimitePago, c.Codigo;
END
GO

IF OBJECT_ID(N'dbo.sp_CUN06_EvaluarQuorum', N'P') IS NOT NULL DROP PROCEDURE dbo.sp_CUN06_EvaluarQuorum;
GO
CREATE PROCEDURE dbo.sp_CUN06_EvaluarQuorum @IdComision INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT c.IdComision, c.Codigo, c.CupoMinimo, c.Estado,
           ISNULL(v.VacantesRegularizadas, 0) AS VacantesRegularizadas
    FROM dbo.Comision c
    OUTER APPLY (
        SELECT COUNT(1) AS VacantesRegularizadas
        FROM dbo.SolicitudInscripcion si
        INNER JOIN dbo.Cuota cu ON cu.IdSolicitudInscripcion = si.IdSolicitudInscripcion AND cu.NumeroCuota = 1 AND cu.Estado = N'pagada'
        WHERE si.IdComision = c.IdComision AND si.Estado = N'activa'
    ) v
    WHERE c.IdComision = @IdComision
      AND c.Estado = N'preapertura'
      AND c.FechaLimitePago < CONVERT(date, GETDATE());
END
GO

IF OBJECT_ID(N'dbo.sp_CUN06_RegistrarAltaOficial', N'P') IS NOT NULL DROP PROCEDURE dbo.sp_CUN06_RegistrarAltaOficial;
GO
CREATE PROCEDURE dbo.sp_CUN06_RegistrarAltaOficial
    @IdComision INT, @FechaAlta DATE, @NumeroActa NVARCHAR(50), @Observaciones NVARCHAR(500), @UsuarioRegistro NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @Started BIT = 0, @IdActa INT, @CupoMinimo INT, @Regularizadas INT, @Codigo NVARCHAR(30);
    IF @@TRANCOUNT = 0 BEGIN SET @Started = 1; BEGIN TRANSACTION; END
    BEGIN TRY
        SELECT @CupoMinimo = CupoMinimo, @Codigo = Codigo FROM dbo.Comision WITH (UPDLOCK, HOLDLOCK)
        WHERE IdComision = @IdComision AND Estado = N'preapertura' AND FechaLimitePago < CONVERT(date, GETDATE());
        IF @CupoMinimo IS NULL THROW 52701, N'Errores.ComisionPreaperturaNoDisponible', 1;

        SELECT @Regularizadas = COUNT(1)
        FROM dbo.SolicitudInscripcion si WITH (UPDLOCK, HOLDLOCK)
        INNER JOIN dbo.Cuota cu WITH (UPDLOCK, HOLDLOCK) ON cu.IdSolicitudInscripcion = si.IdSolicitudInscripcion AND cu.NumeroCuota = 1 AND cu.Estado = N'pagada'
        WHERE si.IdComision = @IdComision AND si.Estado = N'activa';

        IF @Regularizadas < @CupoMinimo THROW 52702, N'Errores.EstadoDefinitivoQuorumInsuficiente', 1;

        INSERT INTO dbo.AltaOficialComision (IdComision, FechaAlta, NumeroActa, Observaciones, UsuarioRegistro, DVH)
        VALUES (@IdComision, @FechaAlta, @NumeroActa, ISNULL(@Observaciones, N''), @UsuarioRegistro, N'');
        SET @IdActa = SCOPE_IDENTITY();

        UPDATE dbo.Comision SET Estado = N'confirmada', DVH = N'' WHERE IdComision = @IdComision;

        IF @Started = 1 COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @Started = 1 AND XACT_STATE() <> 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH;

    SELECT @IdComision AS IdComision, @Codigo AS CodigoComision, N'confirmada' AS EstadoDefinitivo,
           @Regularizadas AS VacantesRegularizadas, @CupoMinimo AS CupoMinimo, @IdActa AS IdActa,
           @NumeroActa AS NumeroActa, 0 AS SolicitudesCanceladas, 0 AS CuotasReintegroPendiente;
END
GO

IF OBJECT_ID(N'dbo.sp_CUN06_RegistrarActaCierre', N'P') IS NOT NULL DROP PROCEDURE dbo.sp_CUN06_RegistrarActaCierre;
GO
CREATE PROCEDURE dbo.sp_CUN06_RegistrarActaCierre
    @IdComision INT, @FechaCierre DATE, @NumeroActa NVARCHAR(50), @Motivo NVARCHAR(250), @Observaciones NVARCHAR(500), @UsuarioRegistro NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @Started BIT = 0, @IdActa INT, @CupoMinimo INT, @Regularizadas INT, @Codigo NVARCHAR(30), @Canceladas INT = 0, @Reintegro INT = 0;
    IF @@TRANCOUNT = 0 BEGIN SET @Started = 1; BEGIN TRANSACTION; END
    BEGIN TRY
        SELECT @CupoMinimo = CupoMinimo, @Codigo = Codigo FROM dbo.Comision WITH (UPDLOCK, HOLDLOCK)
        WHERE IdComision = @IdComision AND Estado = N'preapertura' AND FechaLimitePago < CONVERT(date, GETDATE());
        IF @CupoMinimo IS NULL THROW 52701, N'Errores.ComisionPreaperturaNoDisponible', 1;

        SELECT @Regularizadas = COUNT(1)
        FROM dbo.SolicitudInscripcion si WITH (UPDLOCK, HOLDLOCK)
        INNER JOIN dbo.Cuota cu WITH (UPDLOCK, HOLDLOCK) ON cu.IdSolicitudInscripcion = si.IdSolicitudInscripcion AND cu.NumeroCuota = 1 AND cu.Estado = N'pagada'
        WHERE si.IdComision = @IdComision AND si.Estado = N'activa';

        IF @Regularizadas >= @CupoMinimo THROW 52703, N'Errores.EstadoDefinitivoQuorumAlcanzado', 1;

        INSERT INTO dbo.ActaCierreComision (IdComision, FechaCierre, NumeroActa, Motivo, Observaciones, UsuarioRegistro, DVH)
        VALUES (@IdComision, @FechaCierre, @NumeroActa, @Motivo, ISNULL(@Observaciones, N''), @UsuarioRegistro, N'');
        SET @IdActa = SCOPE_IDENTITY();

        UPDATE dbo.Cuota SET Estado = N'reintegro_pendiente', DVH = N''
        WHERE Estado = N'pagada' AND IdSolicitudInscripcion IN (SELECT IdSolicitudInscripcion FROM dbo.SolicitudInscripcion WHERE IdComision = @IdComision AND Estado = N'activa');
        SET @Reintegro = @@ROWCOUNT;

        UPDATE dbo.SolicitudInscripcion SET Estado = N'cancelada', DVH = N'' WHERE IdComision = @IdComision AND Estado = N'activa';
        SET @Canceladas = @@ROWCOUNT;

        UPDATE dbo.Comision SET Estado = N'cancelada_falta_quorum', DVH = N'' WHERE IdComision = @IdComision;

        IF @Started = 1 COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @Started = 1 AND XACT_STATE() <> 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH;

    SELECT @IdComision AS IdComision, @Codigo AS CodigoComision, N'cancelada_falta_quorum' AS EstadoDefinitivo,
           @Regularizadas AS VacantesRegularizadas, @CupoMinimo AS CupoMinimo, @IdActa AS IdActa,
           @NumeroActa AS NumeroActa, @Canceladas AS SolicitudesCanceladas, @Reintegro AS CuotasReintegroPendiente;
END
GO

PRINT 'CUN06 migration applied.';
GO

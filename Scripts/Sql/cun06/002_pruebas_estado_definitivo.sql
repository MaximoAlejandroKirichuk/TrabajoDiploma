SET NOCOUNT ON;
SET XACT_ABORT ON;
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

IF OBJECT_ID(N'dbo.sp_CUN06_RegistrarActaCierre', N'P') IS NULL
   OR OBJECT_ID(N'dbo.sp_CUN06_RegistrarAltaOficial', N'P') IS NULL
   OR OBJECT_ID(N'dbo.sp_CUN06_EvaluarQuorum', N'P') IS NULL
BEGIN
    RAISERROR(N'CUN06 no está migrado. Ejecute Scripts\Sql\cun06\001_migracion.sql primero.', 16, 1);
    RETURN;
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Patentes WHERE CodigoPatente = 67 AND ISNULL(DVH, '') <> '')
BEGIN
    RAISERROR(N'Falta patente 67 o su DVH no fue calculado.', 16, 1);
    RETURN;
END

IF EXISTS (SELECT 1 FROM dbo.Roles WHERE CodigoRol = 1)
   AND NOT EXISTS (SELECT 1 FROM dbo.RolPatente WHERE CodigoRol = 1 AND CodigoPatente = 67 AND ISNULL(DVH, '') <> '')
BEGIN
    RAISERROR(N'Falta RolPatente admin/patente 67 o su DVH no fue calculado.', 16, 1);
    RETURN;
END

IF NOT EXISTS (SELECT 1 FROM dbo.DigitoVerificador_83KI WHERE NombreTabla = N'AltaOficialComision' AND ISNULL(DVV, '') <> '')
   OR NOT EXISTS (SELECT 1 FROM dbo.DigitoVerificador_83KI WHERE NombreTabla = N'ActaCierreComision' AND ISNULL(DVV, '') <> '')
BEGIN
    RAISERROR(N'Falta metadata DVV de AltaOficialComision/ActaCierreComision.', 16, 1);
    RETURN;
END

PRINT N'OK: CUN06 permission and act-table integrity metadata was populated.';
GO

DECLARE @IdCurso INT = (SELECT TOP (1) IdCurso FROM dbo.Curso ORDER BY IdCurso);
DECLARE @IdProfesor INT = (SELECT TOP (1) IdProfesor FROM dbo.Profesor ORDER BY IdProfesor);
DECLARE @IdPlanDePago INT = (SELECT TOP (1) IdPlanDePago FROM dbo.PlanesDePago ORDER BY IdPlanDePago);

IF @IdCurso IS NULL OR @IdProfesor IS NULL OR @IdPlanDePago IS NULL
BEGIN
    RAISERROR(N'No hay Curso/Profesor/PlanDePago base para crear fixture CUN06.', 16, 1);
    RETURN;
END

DECLARE @Tag NVARCHAR(30) = N'CUN06SMOKE';
DECLARE @IdComision INT;
DECLARE @IdAlumno INT;
DECLARE @IdSolicitud INT;
DECLARE @IdCuota INT;
DECLARE @PagoAntes INT;
DECLARE @PagoDespues INT;
DECLARE @Regularizadas INT;
DECLARE @CodigoComision NVARCHAR(30) = @Tag + N'-COM';
DECLARE @CodigoSolicitud NVARCHAR(30) = @Tag + N'-SOL';
DECLARE @NumeroActa NVARCHAR(50) = @Tag + N'-ACTA';
DECLARE @NumeroReferencia NVARCHAR(100);
DECLARE @FechaCierre DATE = CONVERT(date, GETDATE());

BEGIN TRANSACTION;
BEGIN TRY
    INSERT INTO dbo.Comision
        (Codigo, IdCurso, IdProfesor, DiaSemana, HoraInicio, HoraFin, CupoMinimo, CupoMaximo,
         FechaLimitePago, FechaInicio, FechaFin, ArancelBase, MontoMatricula, IdPlanDePago,
         RecargoPlanSnapshot, Estado, DVH)
    VALUES
        (@CodigoComision, @IdCurso, @IdProfesor, N'Lunes', '09:00', '11:00', 2, 10,
         DATEADD(DAY, -2, CONVERT(date, GETDATE())), DATEADD(DAY, 7, CONVERT(date, GETDATE())),
         DATEADD(MONTH, 4, CONVERT(date, GETDATE())), 1000.00, 1000.00, @IdPlanDePago,
         0, N'preapertura', N'');
    SET @IdComision = SCOPE_IDENTITY();

    INSERT INTO dbo.Alumno (DNI, Nombre, Apellido, Email, Telefono, Estado, DVH)
    VALUES (N'99000067', N'Smoke', N'CUN06', N'cun06-smoke@example.com', N'11111111', N'activo', N'');
    SET @IdAlumno = SCOPE_IDENTITY();

    INSERT INTO dbo.SolicitudInscripcion
        (Codigo, IdAlumno, IdLeadOrigen, IdComision, IdPlanDePago, RecargoPlanSnapshot,
         FechaSolicitud, Estado, Observaciones, DVH)
    VALUES
        (@CodigoSolicitud, @IdAlumno, NULL, @IdComision, @IdPlanDePago, 0,
         DATEADD(DAY, -10, GETDATE()), N'activa', N'Smoke CUN06 rollback-safe', N'');
    SET @IdSolicitud = SCOPE_IDENTITY();

    INSERT INTO dbo.Cuota
        (IdSolicitudInscripcion, NumeroCuota, MontoOriginal, BalanceAdeudado, FechaVencimiento, Estado, DVH)
    VALUES
        (@IdSolicitud, 1, 1000.00, 0.00, DATEADD(DAY, -1, CONVERT(date, GETDATE())), N'pagada', N'');
    SET @IdCuota = SCOPE_IDENTITY();

    SET @NumeroReferencia = @Tag + N'-REF-' + CONVERT(NVARCHAR(30), @IdCuota);

    INSERT INTO dbo.PagoInscripcion
        (IdAlumno, IdSolicitudInscripcion, IdCuota, MetodoPago, MontoPagado, NumeroReferencia, FechaPago, DVH)
    VALUES
        (@IdAlumno, @IdSolicitud, @IdCuota, N'transferencia', 1000.00,
         @NumeroReferencia, DATEADD(DAY, -1, GETDATE()), N'');

    SELECT @PagoAntes = COUNT(1)
    FROM dbo.PagoInscripcion
    WHERE IdSolicitudInscripcion = @IdSolicitud;

    EXEC dbo.sp_CUN06_EvaluarQuorum @IdComision = @IdComision;

    EXEC dbo.sp_CUN06_RegistrarActaCierre
        @IdComision = @IdComision,
        @FechaCierre = @FechaCierre,
        @NumeroActa = @NumeroActa,
        @Motivo = N'Smoke test falta de quórum',
        @Observaciones = N'Rollback-safe smoke test',
        @UsuarioRegistro = N'smoke-cun06';

    SELECT @PagoDespues = COUNT(1)
    FROM dbo.PagoInscripcion
    WHERE IdSolicitudInscripcion = @IdSolicitud;

    IF @PagoAntes <> 1 OR @PagoDespues <> @PagoAntes
        THROW 52801, N'CUN06 smoke failed: PagoInscripcion was not preserved.', 1;

    IF NOT EXISTS (SELECT 1 FROM dbo.Cuota WHERE IdCuota = @IdCuota AND Estado = N'reintegro_pendiente')
        THROW 52802, N'CUN06 smoke failed: paid cuota was not marked reintegro_pendiente.', 1;

    IF NOT EXISTS (SELECT 1 FROM dbo.SolicitudInscripcion WHERE IdSolicitudInscripcion = @IdSolicitud AND Estado = N'cancelada')
        THROW 52803, N'CUN06 smoke failed: solicitud was not cancelled.', 1;

    IF NOT EXISTS (SELECT 1 FROM dbo.Comision WHERE IdComision = @IdComision AND Estado = N'cancelada_falta_quorum')
        THROW 52804, N'CUN06 smoke failed: commission was not cancelled by quorum.', 1;

    IF NOT EXISTS (SELECT 1 FROM dbo.ActaCierreComision WHERE IdComision = @IdComision AND NumeroActa = @NumeroActa)
        THROW 52805, N'CUN06 smoke failed: closing act was not inserted.', 1;

    PRINT N'OK: cancellation preserves PagoInscripcion and marks paid cuota reintegro_pendiente.';

    ROLLBACK TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
GO

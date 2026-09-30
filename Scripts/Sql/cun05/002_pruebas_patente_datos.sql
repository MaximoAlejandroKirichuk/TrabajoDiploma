-- =============================================================================
-- 002_pruebas_patente_datos.sql — pruebas de patente de datos para CUN05
-- Crea una fixture CUN05 segura si la base local no tiene alumno/solicitud/cuota
-- pendiente para el DNI de prueba. Los escenarios se ejecutan dentro de
-- transacciones revertidas y la fixture se limpia al final.
-- =============================================================================

SET NOCOUNT ON;
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;

DECLARE @DniConPendiente VARCHAR(20) = N'12345678';
DECLARE @DniSinPendiente VARCHAR(20) = N'99999999';
DECLARE @IdCuota INT;
DECLARE @IdSolicitud INT;
DECLARE @IdAlumno INT;
DECLARE @Balance DECIMAL(18,2);
DECLARE @Monto DECIMAL(18,2);
DECLARE @FixtureCreada BIT = 0;
DECLARE @TestTag NVARCHAR(30) = N'CUN05-SMOKE';
DECLARE @IdPlanFixture INT;
DECLARE @IdCursoFixture INT;
DECLARE @IdProfesorFixture INT;
DECLARE @IdComisionFixture INT;
DECLARE @IdSolicitudFixture INT;
DECLARE @IdAlumnoFixture INT;

IF OBJECT_ID(N'dbo.PagoInscripcion', N'U') IS NULL
   OR OBJECT_ID(N'dbo.sp_CUN05_RegistrarPagoInscripcion', N'P') IS NULL
   OR OBJECT_ID(N'dbo.sp_CUN05_BuscarCuotasPendientesPorDni', N'P') IS NULL
BEGIN
    PRINT 'No estan aplicados los objetos CUN05 (tabla/procedimientos). Ejecute 001_migracion.sql antes de estas pruebas.';
    RETURN;
END

-- ---------------------------------------------------------------------------
-- 1. busqueda de cuotas pendientes por dni
-- ---------------------------------------------------------------------------
SELECT TOP 1 @IdAlumno = a.IdAlumno, @IdSolicitud = si.IdSolicitudInscripcion, @IdCuota = cu.IdCuota, @Balance = cu.BalanceAdeudado
FROM dbo.Alumno a
INNER JOIN dbo.SolicitudInscripcion si ON si.IdAlumno = a.IdAlumno
INNER JOIN dbo.Cuota cu ON cu.IdSolicitudInscripcion = si.IdSolicitudInscripcion
WHERE a.DNI = @DniConPendiente AND cu.Estado = N'pendiente';

IF @IdCuota IS NULL
BEGIN
    PRINT 'No hay datos de prueba disponibles. Creando fixture CUN05 autocontenida.';

    -- Limpieza defensiva de residuos de ejecuciones anteriores del mismo script.
    DELETE p
    FROM dbo.PagoInscripcion p
    INNER JOIN dbo.Alumno a ON a.IdAlumno = p.IdAlumno
    WHERE a.DNI = @DniConPendiente;

    DELETE cu
    FROM dbo.Cuota cu
    INNER JOIN dbo.SolicitudInscripcion si ON si.IdSolicitudInscripcion = cu.IdSolicitudInscripcion
    INNER JOIN dbo.Alumno a ON a.IdAlumno = si.IdAlumno
    WHERE a.DNI = @DniConPendiente;

    DELETE si
    FROM dbo.SolicitudInscripcion si
    INNER JOIN dbo.Alumno a ON a.IdAlumno = si.IdAlumno
    WHERE a.DNI = @DniConPendiente;

    DELETE FROM dbo.Alumno WHERE DNI = @DniConPendiente;
    DELETE FROM dbo.Comision WHERE Codigo = @TestTag + N'-COM';
    DELETE FROM dbo.CursoProfesor
    WHERE IdCurso IN (SELECT IdCurso FROM dbo.Curso WHERE Nombre = @TestTag + N' Curso')
       OR IdProfesor IN (SELECT IdProfesor FROM dbo.Profesor WHERE DNI = N'99123005');
    DELETE FROM dbo.Curso WHERE Nombre = @TestTag + N' Curso';
    DELETE FROM dbo.Profesor WHERE DNI = N'99123005';
    DELETE FROM dbo.PlanesDePago WHERE Nombre = @TestTag + N' Contado';

    INSERT INTO dbo.PlanesDePago (Nombre, RecargoPorcentaje, EstadoActivo)
    VALUES (@TestTag + N' Contado', 0.0000, 1);
    SET @IdPlanFixture = SCOPE_IDENTITY();

    INSERT INTO dbo.Curso (Nombre, Descripcion, CargaHoraria, EstadoActivo, DVH)
    VALUES (@TestTag + N' Curso', N'Fixture temporal para smoke test CUN05.', 12, 1, '');
    SET @IdCursoFixture = SCOPE_IDENTITY();

    INSERT INTO dbo.Profesor (DNI, Nombre, Apellido, Email, EstadoActivo, DVH)
    VALUES (N'99123005', N'Profesor', N'CUN05 Smoke', N'profesor.cun05.smoke@example.com', 1, '');
    SET @IdProfesorFixture = SCOPE_IDENTITY();

    INSERT INTO dbo.CursoProfesor (IdCurso, IdProfesor, EstadoActivo, DVH)
    VALUES (@IdCursoFixture, @IdProfesorFixture, 1, '');

    INSERT INTO dbo.Comision (Codigo, IdCurso, IdProfesor, DiaSemana, HoraInicio, HoraFin, CupoMinimo, CupoMaximo, FechaLimitePago, FechaInicio, FechaFin, ArancelBase, MontoMatricula, IdPlanDePago, RecargoPlanSnapshot, Estado, DVH)
    VALUES (@TestTag + N'-COM', @IdCursoFixture, @IdProfesorFixture, N'Monday', CAST(N'18:00' AS TIME), CAST(N'20:00' AS TIME), 1, 10, DATEADD(day, 30, CONVERT(date, GETDATE())), CONVERT(date, GETDATE()), DATEADD(month, 2, CONVERT(date, GETDATE())), 50000.00, 15000.00, @IdPlanFixture, 0.0000, N'preapertura', '');
    SET @IdComisionFixture = SCOPE_IDENTITY();

    INSERT INTO dbo.Alumno (DNI, Nombre, Apellido, Email, Telefono, FechaAlta, Estado, DVH)
    VALUES (@DniConPendiente, N'Alumno', N'CUN05 Smoke', N'alumno.cun05.smoke@example.com', N'5491100000005', GETDATE(), N'activo', '');
    SET @IdAlumnoFixture = SCOPE_IDENTITY();

    INSERT INTO dbo.SolicitudInscripcion (Codigo, IdAlumno, IdLeadOrigen, IdComision, IdPlanDePago, RecargoPlanSnapshot, FechaSolicitud, Estado, Observaciones, DVH)
    VALUES (@TestTag + N'-SOL', @IdAlumnoFixture, NULL, @IdComisionFixture, @IdPlanFixture, 0.0000, GETDATE(), N'pendiente', N'Fixture temporal para smoke test CUN05.', '');
    SET @IdSolicitudFixture = SCOPE_IDENTITY();

    -- Dos cuotas permiten probar referencia duplicada contra otra cuota pendiente.
    INSERT INTO dbo.Cuota (IdSolicitudInscripcion, NumeroCuota, MontoOriginal, BalanceAdeudado, FechaVencimiento, Estado, DVH)
    VALUES (@IdSolicitudFixture, 1, 15000.00, 15000.00, DATEADD(day, 7, CONVERT(date, GETDATE())), N'pendiente', '');
    INSERT INTO dbo.Cuota (IdSolicitudInscripcion, NumeroCuota, MontoOriginal, BalanceAdeudado, FechaVencimiento, Estado, DVH)
    VALUES (@IdSolicitudFixture, 2, 15000.00, 15000.00, DATEADD(day, 37, CONVERT(date, GETDATE())), N'pendiente', '');

    SET @FixtureCreada = 1;

    SELECT TOP 1 @IdAlumno = a.IdAlumno, @IdSolicitud = si.IdSolicitudInscripcion, @IdCuota = cu.IdCuota, @Balance = cu.BalanceAdeudado
    FROM dbo.Alumno a
    INNER JOIN dbo.SolicitudInscripcion si ON si.IdAlumno = a.IdAlumno
    INNER JOIN dbo.Cuota cu ON cu.IdSolicitudInscripcion = si.IdSolicitudInscripcion
    WHERE a.DNI = @DniConPendiente AND cu.Estado = N'pendiente'
    ORDER BY cu.NumeroCuota;
END

SET @Monto = @Balance;

-- ---------------------------------------------------------------------------
-- 2. pago valido: monto exacto, metodo valido, referencia nueva
-- ---------------------------------------------------------------------------
BEGIN TRANSACTION;
BEGIN TRY
    DECLARE @ReferenciaValida NVARCHAR(100) = N'REF-' + CONVERT(NVARCHAR(36), NEWID());
    DECLARE @MetodoValido NVARCHAR(30) = N'transferencia';
    DECLARE @FechaValida DATETIME = GETDATE();

    EXEC dbo.sp_CUN05_RegistrarPagoInscripcion
        @IdAlumno = @IdAlumno,
        @IdSolicitudInscripcion = @IdSolicitud,
        @IdCuota = @IdCuota,
        @MetodoPago = @MetodoValido,
        @MontoRecibido = @Monto,
        @NumeroReferencia = @ReferenciaValida,
        @FechaPago = @FechaValida;

    IF EXISTS (SELECT 1 FROM dbo.Cuota WHERE IdCuota = @IdCuota AND Estado = N'pagada' AND BalanceAdeudado = 0)
       AND EXISTS (SELECT 1 FROM dbo.SolicitudInscripcion WHERE IdSolicitudInscripcion = @IdSolicitud AND Estado = N'activa')
       AND EXISTS (SELECT 1 FROM dbo.PagoInscripcion WHERE IdCuota = @IdCuota AND NumeroReferencia = @ReferenciaValida)
        PRINT 'OK: pago valido registrado, cuota pagada, solicitud activa y comprobante persistido dentro de la transaccion.';
    ELSE
        PRINT 'FAIL: el pago valido no dejo pago/cuota/solicitud en estado esperado.';

    ROLLBACK TRANSACTION;
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
    PRINT N'FAIL: pago valido genero error: ' + ERROR_MESSAGE();
END CATCH;

-- ---------------------------------------------------------------------------
-- 3. rechazo por monto distinto
-- ---------------------------------------------------------------------------
BEGIN TRANSACTION;
BEGIN TRY
    DECLARE @IdCuotaMonto INT, @IdSolicitudMonto INT, @IdAlumnoMonto INT, @BalanceMonto DECIMAL(18,2);
    DECLARE @MontoIncorrecto DECIMAL(18,2);
    DECLARE @ReferenciaMonto NVARCHAR(100);
    DECLARE @FechaMonto DATETIME;
    SELECT TOP 1 @IdAlumnoMonto = a.IdAlumno, @IdSolicitudMonto = si.IdSolicitudInscripcion, @IdCuotaMonto = cu.IdCuota, @BalanceMonto = cu.BalanceAdeudado
    FROM dbo.Alumno a
    INNER JOIN dbo.SolicitudInscripcion si ON si.IdAlumno = a.IdAlumno
    INNER JOIN dbo.Cuota cu ON cu.IdSolicitudInscripcion = si.IdSolicitudInscripcion
    WHERE a.DNI = @DniConPendiente AND cu.Estado = N'pendiente';

    SET @MontoIncorrecto = @BalanceMonto + 1;
    SET @ReferenciaMonto = N'REF-MONTO-' + CONVERT(NVARCHAR(36), NEWID());
    SET @FechaMonto = GETDATE();

    EXEC dbo.sp_CUN05_RegistrarPagoInscripcion
        @IdAlumno = @IdAlumnoMonto,
        @IdSolicitudInscripcion = @IdSolicitudMonto,
        @IdCuota = @IdCuotaMonto,
        @MetodoPago = N'transferencia',
        @MontoRecibido = @MontoIncorrecto,
        @NumeroReferencia = @ReferenciaMonto,
        @FechaPago = @FechaMonto;

    PRINT 'FAIL: deberia haber rechazado el monto incorrecto.';
    ROLLBACK TRANSACTION;
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
    IF ERROR_MESSAGE() LIKE N'%Errores.PagoMontoNoCoincide%'
        PRINT 'OK: monto incorrecto rechazado.';
    ELSE
        PRINT N'FAIL: error inesperado en rechazo de monto: ' + ERROR_MESSAGE();
END CATCH;

-- ---------------------------------------------------------------------------
-- 4. rechazo por metodo invalido
-- ---------------------------------------------------------------------------
BEGIN TRANSACTION;
BEGIN TRY
    DECLARE @IdCuotaMetodo INT, @IdSolicitudMetodo INT, @IdAlumnoMetodo INT, @BalanceMetodo DECIMAL(18,2);
    DECLARE @ReferenciaMetodo NVARCHAR(100);
    DECLARE @FechaMetodo DATETIME;
    SELECT TOP 1 @IdAlumnoMetodo = a.IdAlumno, @IdSolicitudMetodo = si.IdSolicitudInscripcion, @IdCuotaMetodo = cu.IdCuota, @BalanceMetodo = cu.BalanceAdeudado
    FROM dbo.Alumno a
    INNER JOIN dbo.SolicitudInscripcion si ON si.IdAlumno = a.IdAlumno
    INNER JOIN dbo.Cuota cu ON cu.IdSolicitudInscripcion = si.IdSolicitudInscripcion
    WHERE a.DNI = @DniConPendiente AND cu.Estado = N'pendiente';

    SET @ReferenciaMetodo = N'REF-METODO-' + CONVERT(NVARCHAR(36), NEWID());
    SET @FechaMetodo = GETDATE();

    EXEC dbo.sp_CUN05_RegistrarPagoInscripcion
        @IdAlumno = @IdAlumnoMetodo,
        @IdSolicitudInscripcion = @IdSolicitudMetodo,
        @IdCuota = @IdCuotaMetodo,
        @MetodoPago = N'efectivo',
        @MontoRecibido = @BalanceMetodo,
        @NumeroReferencia = @ReferenciaMetodo,
        @FechaPago = @FechaMetodo;

    PRINT 'FAIL: deberia haber rechazado el metodo invalido.';
    ROLLBACK TRANSACTION;
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
    IF ERROR_MESSAGE() LIKE N'%Errores.PagoMetodoInvalido%'
        PRINT 'OK: metodo invalido rechazado.';
    ELSE
        PRINT N'FAIL: error inesperado en rechazo de metodo: ' + ERROR_MESSAGE();
END CATCH;

-- ---------------------------------------------------------------------------
-- 5. rechazo por referencia duplicada
-- ---------------------------------------------------------------------------
BEGIN TRANSACTION;
BEGIN TRY
    DECLARE @IdCuotaDup INT, @IdSolicitudDup INT, @IdAlumnoDup INT, @BalanceDup DECIMAL(18,2);
    SELECT TOP 1 @IdAlumnoDup = a.IdAlumno, @IdSolicitudDup = si.IdSolicitudInscripcion, @IdCuotaDup = cu.IdCuota, @BalanceDup = cu.BalanceAdeudado
    FROM dbo.Alumno a
    INNER JOIN dbo.SolicitudInscripcion si ON si.IdAlumno = a.IdAlumno
    INNER JOIN dbo.Cuota cu ON cu.IdSolicitudInscripcion = si.IdSolicitudInscripcion
    WHERE a.DNI = @DniConPendiente AND cu.Estado = N'pendiente';

    DECLARE @ReferenciaDup NVARCHAR(100) = N'REF-DUP-' + CONVERT(NVARCHAR(36), NEWID());
    DECLARE @FechaDup DATETIME = GETDATE();

    EXEC dbo.sp_CUN05_RegistrarPagoInscripcion
        @IdAlumno = @IdAlumnoDup,
        @IdSolicitudInscripcion = @IdSolicitudDup,
        @IdCuota = @IdCuotaDup,
        @MetodoPago = N'transferencia',
        @MontoRecibido = @BalanceDup,
        @NumeroReferencia = @ReferenciaDup,
        @FechaPago = @FechaDup;

    -- intento duplicado sobre otra cuota pendiente de la misma persona
    DECLARE @IdCuotaDup2 INT, @BalanceDup2 DECIMAL(18,2);
    SELECT TOP 1 @IdCuotaDup2 = cu.IdCuota, @BalanceDup2 = cu.BalanceAdeudado
    FROM dbo.Cuota cu
    WHERE cu.IdSolicitudInscripcion = @IdSolicitudDup AND cu.Estado = N'pendiente' AND cu.IdCuota <> @IdCuotaDup;

    IF @IdCuotaDup2 IS NOT NULL
    BEGIN
        EXEC dbo.sp_CUN05_RegistrarPagoInscripcion
            @IdAlumno = @IdAlumnoDup,
            @IdSolicitudInscripcion = @IdSolicitudDup,
            @IdCuota = @IdCuotaDup2,
            @MetodoPago = N'transferencia',
            @MontoRecibido = @BalanceDup2,
            @NumeroReferencia = @ReferenciaDup,
            @FechaPago = @FechaDup;
    END

    PRINT 'FAIL: deberia haber rechazado la referencia duplicada.';
    ROLLBACK TRANSACTION;
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
    IF ERROR_MESSAGE() LIKE N'%Errores.PagoReferenciaDuplicada%'
        PRINT 'OK: referencia duplicada rechazada.';
    ELSE
        PRINT N'FAIL: error inesperado en rechazo de referencia duplicada: ' + ERROR_MESSAGE();
END CATCH;

-- ---------------------------------------------------------------------------
-- 6. rollback completo ante error (la cuota debe seguir pendiente)
-- ---------------------------------------------------------------------------
BEGIN TRANSACTION;
BEGIN TRY
    DECLARE @IdCuotaRb INT, @IdSolicitudRb INT, @IdAlumnoRb INT, @BalanceRb DECIMAL(18,2);
    DECLARE @ReferenciaRb NVARCHAR(100);
    DECLARE @FechaRb DATETIME;
    SELECT TOP 1 @IdAlumnoRb = a.IdAlumno, @IdSolicitudRb = si.IdSolicitudInscripcion, @IdCuotaRb = cu.IdCuota, @BalanceRb = cu.BalanceAdeudado
    FROM dbo.Alumno a
    INNER JOIN dbo.SolicitudInscripcion si ON si.IdAlumno = a.IdAlumno
    INNER JOIN dbo.Cuota cu ON cu.IdSolicitudInscripcion = si.IdSolicitudInscripcion
    WHERE a.DNI = @DniConPendiente AND cu.Estado = N'pendiente';

    SET @ReferenciaRb = N'REF-RB-' + CONVERT(NVARCHAR(36), NEWID());
    SET @FechaRb = GETDATE();

    EXEC dbo.sp_CUN05_RegistrarPagoInscripcion
        @IdAlumno = @IdAlumnoRb,
        @IdSolicitudInscripcion = @IdSolicitudRb,
        @IdCuota = @IdCuotaRb,
        @MetodoPago = N'efectivo',
        @MontoRecibido = @BalanceRb,
        @NumeroReferencia = @ReferenciaRb,
        @FechaPago = @FechaRb;

    ROLLBACK TRANSACTION;
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
    IF EXISTS (SELECT 1 FROM dbo.Cuota WHERE IdCuota = @IdCuotaRb AND Estado = N'pendiente' AND BalanceAdeudado = @BalanceRb)
        PRINT 'OK: rollback completo, la cuota permanece pendiente.';
    ELSE
        PRINT 'FAIL: la cuota cambio de estado a pesar del rollback.';
END CATCH;

-- ---------------------------------------------------------------------------
-- 7. integridad: verificar que PagoInscripcion figure en DigitoVerificador
-- ---------------------------------------------------------------------------
IF EXISTS (SELECT 1 FROM dbo.DigitoVerificador_83KI WHERE NombreTabla = N'PagoInscripcion')
    PRINT 'OK: PagoInscripcion esta protegida por DVH/DVV.';
ELSE
    PRINT 'FAIL: PagoInscripcion no tiene entrada en DigitoVerificador_83KI.';

-- ---------------------------------------------------------------------------
-- 8. limpieza de fixture autocontenida
-- ---------------------------------------------------------------------------
IF @FixtureCreada = 1
BEGIN
    DELETE p
    FROM dbo.PagoInscripcion p
    INNER JOIN dbo.Alumno a ON a.IdAlumno = p.IdAlumno
    WHERE a.DNI = @DniConPendiente;

    DELETE cu
    FROM dbo.Cuota cu
    INNER JOIN dbo.SolicitudInscripcion si ON si.IdSolicitudInscripcion = cu.IdSolicitudInscripcion
    INNER JOIN dbo.Alumno a ON a.IdAlumno = si.IdAlumno
    WHERE a.DNI = @DniConPendiente;

    DELETE si
    FROM dbo.SolicitudInscripcion si
    INNER JOIN dbo.Alumno a ON a.IdAlumno = si.IdAlumno
    WHERE a.DNI = @DniConPendiente;

    DELETE FROM dbo.Alumno WHERE DNI = @DniConPendiente;
    DELETE FROM dbo.Comision WHERE Codigo = @TestTag + N'-COM';
    DELETE FROM dbo.CursoProfesor
    WHERE IdCurso = @IdCursoFixture OR IdProfesor = @IdProfesorFixture;
    DELETE FROM dbo.Curso WHERE IdCurso = @IdCursoFixture;
    DELETE FROM dbo.Profesor WHERE IdProfesor = @IdProfesorFixture;
    DELETE FROM dbo.PlanesDePago WHERE IdPlanDePago = @IdPlanFixture;

    IF EXISTS (SELECT 1 FROM dbo.Alumno WHERE DNI = @DniConPendiente)
        PRINT 'FAIL: la fixture CUN05 no pudo limpiarse completamente.';
    ELSE
        PRINT 'OK: fixture CUN05 limpiada sin dejar datos de prueba.';
END

PRINT 'CUN05 smoke tests completados.';

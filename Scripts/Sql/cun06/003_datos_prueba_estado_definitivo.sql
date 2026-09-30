SET NOCOUNT ON;
SET XACT_ABORT ON;
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;

IF OBJECT_ID(N'dbo.sp_CUN06_ListarComisionesVencidas', N'P') IS NULL
   OR OBJECT_ID(N'dbo.sp_CUN06_EvaluarQuorum', N'P') IS NULL
   OR OBJECT_ID(N'dbo.PagoInscripcion', N'U') IS NULL
BEGIN
    RAISERROR(N'CUN06/CUN05 no está migrado. Ejecute primero los scripts de migración CUN05 y CUN06.', 16, 1);
    RETURN;
END;

DECLARE @IdCurso INT = (SELECT TOP (1) IdCurso FROM dbo.Curso ORDER BY IdCurso);
DECLARE @IdProfesor INT = (SELECT TOP (1) IdProfesor FROM dbo.Profesor ORDER BY IdProfesor);
DECLARE @IdPlanDePago INT = (SELECT TOP (1) IdPlanDePago FROM dbo.PlanesDePago ORDER BY IdPlanDePago);

IF @IdCurso IS NULL OR @IdProfesor IS NULL OR @IdPlanDePago IS NULL
BEGIN
    RAISERROR(N'Faltan datos base: debe existir al menos un Curso, un Profesor y un PlanDePago.', 16, 1);
    RETURN;
END;

BEGIN TRANSACTION;
BEGIN TRY
    -------------------------------------------------------------------------
    -- Limpieza idempotente de fixtures CUN06 anteriores.
    -------------------------------------------------------------------------
    DELETE pi
    FROM dbo.PagoInscripcion pi
    INNER JOIN dbo.SolicitudInscripcion si ON si.IdSolicitudInscripcion = pi.IdSolicitudInscripcion
    INNER JOIN dbo.Comision c ON c.IdComision = si.IdComision
    WHERE c.Codigo LIKE N'CUN06-P%';

    DELETE cu
    FROM dbo.Cuota cu
    INNER JOIN dbo.SolicitudInscripcion si ON si.IdSolicitudInscripcion = cu.IdSolicitudInscripcion
    INNER JOIN dbo.Comision c ON c.IdComision = si.IdComision
    WHERE c.Codigo LIKE N'CUN06-P%';

    DELETE ac
    FROM dbo.ActaCierreComision ac
    INNER JOIN dbo.Comision c ON c.IdComision = ac.IdComision
    WHERE c.Codigo LIKE N'CUN06-P%';

    DELETE ao
    FROM dbo.AltaOficialComision ao
    INNER JOIN dbo.Comision c ON c.IdComision = ao.IdComision
    WHERE c.Codigo LIKE N'CUN06-P%';

    DELETE si
    FROM dbo.SolicitudInscripcion si
    INNER JOIN dbo.Comision c ON c.IdComision = si.IdComision
    WHERE c.Codigo LIKE N'CUN06-P%';

    DELETE FROM dbo.Alumno WHERE DNI LIKE N'99006%';
    DELETE FROM dbo.Comision WHERE Codigo LIKE N'CUN06-P%';

    -------------------------------------------------------------------------
    -- Casos creados:
    -- P01: aparece y permite alta oficial (3 regularizadas / mínimo 2).
    -- P02: aparece y permite cierre por falta de quórum (1 / mínimo 3).
    -- P03: aparece y permite alta oficial justo en el mínimo (2 / mínimo 2).
    -- P04: aparece y permite cierre por cero regularizadas (0 / mínimo 1).
    -- P05: NO debe aparecer: fecha límite futura.
    -- P06: NO debe aparecer: ya confirmada.
    -------------------------------------------------------------------------
    DECLARE @Casos TABLE
    (
        Codigo NVARCHAR(30) NOT NULL,
        CupoMinimo INT NOT NULL,
        Pagas INT NOT NULL,
        Pendientes INT NOT NULL,
        FechaLimiteOffset INT NOT NULL,
        Estado NVARCHAR(30) NOT NULL,
        Descripcion NVARCHAR(100) NOT NULL
    );

    INSERT INTO @Casos (Codigo, CupoMinimo, Pagas, Pendientes, FechaLimiteOffset, Estado, Descripcion)
    VALUES
        (N'CUN06-P01-OK', 2, 3, 1, -3, N'preapertura', N'Alta oficial: supera el cupo mínimo'),
        (N'CUN06-P02-NOQ', 3, 1, 2, -3, N'preapertura', N'Cierre: falta de quórum'),
        (N'CUN06-P03-JUSTO', 2, 2, 0, -2, N'preapertura', N'Alta oficial: justo en el mínimo'),
        (N'CUN06-P04-CERO', 1, 0, 2, -2, N'preapertura', N'Cierre: cero regularizadas'),
        (N'CUN06-P05-FUTURA', 1, 1, 0, 7, N'preapertura', N'No listar: fecha límite futura'),
        (N'CUN06-P06-CONF', 1, 1, 0, -5, N'confirmada', N'No listar: ya confirmada');

    DECLARE @Codigo NVARCHAR(30), @CupoMinimo INT, @Pagas INT, @Pendientes INT,
            @FechaLimiteOffset INT, @Estado NVARCHAR(30), @Descripcion NVARCHAR(100),
            @IdComision INT, @i INT, @IdAlumno INT, @IdSolicitud INT, @IdCuota INT,
            @Dni VARCHAR(20), @AlumnoSuffix NVARCHAR(20), @Referencia NVARCHAR(100);

    DECLARE cur CURSOR LOCAL FAST_FORWARD FOR
        SELECT Codigo, CupoMinimo, Pagas, Pendientes, FechaLimiteOffset, Estado, Descripcion
        FROM @Casos
        ORDER BY Codigo;

    OPEN cur;
    FETCH NEXT FROM cur INTO @Codigo, @CupoMinimo, @Pagas, @Pendientes, @FechaLimiteOffset, @Estado, @Descripcion;
    WHILE @@FETCH_STATUS = 0
    BEGIN
        INSERT INTO dbo.Comision
            (Codigo, IdCurso, IdProfesor, DiaSemana, HoraInicio, HoraFin, CupoMinimo, CupoMaximo,
             FechaLimitePago, FechaInicio, FechaFin, ArancelBase, MontoMatricula, IdPlanDePago,
             RecargoPlanSnapshot, Estado, DVH)
        VALUES
            (@Codigo, @IdCurso, @IdProfesor, N'Lunes', '09:00', '11:00', @CupoMinimo, 12,
             DATEADD(DAY, @FechaLimiteOffset, CONVERT(date, GETDATE())), DATEADD(DAY, 10, CONVERT(date, GETDATE())),
             DATEADD(MONTH, 4, CONVERT(date, GETDATE())), 1000.00, 1000.00, @IdPlanDePago,
             0, @Estado, N'');
        SET @IdComision = SCOPE_IDENTITY();

        SET @i = 1;
        WHILE @i <= @Pagas
        BEGIN
            SET @AlumnoSuffix = RIGHT(N'000' + CONVERT(NVARCHAR(10), @IdComision), 3) + RIGHT(N'00' + CONVERT(NVARCHAR(10), @i), 2);
            SET @Dni = '99006' + @AlumnoSuffix;

            INSERT INTO dbo.Alumno (DNI, Nombre, Apellido, Email, Telefono, Estado, DVH)
            VALUES (@Dni, N'Alumno', N'CUN06 ' + @Codigo + N' Paga ' + CONVERT(NVARCHAR(10), @i),
                    LOWER(@Codigo) + N'-paga' + CONVERT(NVARCHAR(10), @i) + N'@example.com', N'11000000', N'activo', N'');
            SET @IdAlumno = SCOPE_IDENTITY();

            INSERT INTO dbo.SolicitudInscripcion
                (Codigo, IdAlumno, IdLeadOrigen, IdComision, IdPlanDePago, RecargoPlanSnapshot,
                 FechaSolicitud, Estado, Observaciones, DVH)
            VALUES
                (@Codigo + N'-SOL-P' + CONVERT(NVARCHAR(10), @i), @IdAlumno, NULL, @IdComision, @IdPlanDePago, 0,
                 DATEADD(DAY, -12, GETDATE()), N'activa', @Descripcion, N'');
            SET @IdSolicitud = SCOPE_IDENTITY();

            INSERT INTO dbo.Cuota
                (IdSolicitudInscripcion, NumeroCuota, MontoOriginal, BalanceAdeudado, FechaVencimiento, Estado, DVH)
            VALUES
                (@IdSolicitud, 1, 1000.00, 0.00, DATEADD(DAY, -1, CONVERT(date, GETDATE())), N'pagada', N'');
            SET @IdCuota = SCOPE_IDENTITY();

            SET @Referencia = @Codigo + N'-REF-P' + CONVERT(NVARCHAR(10), @i);
            INSERT INTO dbo.PagoInscripcion
                (IdAlumno, IdSolicitudInscripcion, IdCuota, MetodoPago, MontoPagado, NumeroReferencia, FechaPago, DVH)
            VALUES
                (@IdAlumno, @IdSolicitud, @IdCuota, N'transferencia', 1000.00, @Referencia, DATEADD(DAY, -1, GETDATE()), N'');

            SET @i = @i + 1;
        END;

        SET @i = 1;
        WHILE @i <= @Pendientes
        BEGIN
            SET @AlumnoSuffix = RIGHT(N'000' + CONVERT(NVARCHAR(10), @IdComision), 3) + N'8' + RIGHT(N'0' + CONVERT(NVARCHAR(10), @i), 1);
            SET @Dni = '99006' + @AlumnoSuffix;

            INSERT INTO dbo.Alumno (DNI, Nombre, Apellido, Email, Telefono, Estado, DVH)
            VALUES (@Dni, N'Alumno', N'CUN06 ' + @Codigo + N' Pendiente ' + CONVERT(NVARCHAR(10), @i),
                    LOWER(@Codigo) + N'-pendiente' + CONVERT(NVARCHAR(10), @i) + N'@example.com', N'11000000', N'activo', N'');
            SET @IdAlumno = SCOPE_IDENTITY();

            INSERT INTO dbo.SolicitudInscripcion
                (Codigo, IdAlumno, IdLeadOrigen, IdComision, IdPlanDePago, RecargoPlanSnapshot,
                 FechaSolicitud, Estado, Observaciones, DVH)
            VALUES
                (@Codigo + N'-SOL-D' + CONVERT(NVARCHAR(10), @i), @IdAlumno, NULL, @IdComision, @IdPlanDePago, 0,
                 DATEADD(DAY, -12, GETDATE()), N'pendiente', @Descripcion, N'');
            SET @IdSolicitud = SCOPE_IDENTITY();

            INSERT INTO dbo.Cuota
                (IdSolicitudInscripcion, NumeroCuota, MontoOriginal, BalanceAdeudado, FechaVencimiento, Estado, DVH)
            VALUES
                (@IdSolicitud, 1, 1000.00, 1000.00, DATEADD(DAY, -1, CONVERT(date, GETDATE())), N'pendiente', N'');

            SET @i = @i + 1;
        END;

        FETCH NEXT FROM cur INTO @Codigo, @CupoMinimo, @Pagas, @Pendientes, @FechaLimiteOffset, @Estado, @Descripcion;
    END;
    CLOSE cur;
    DEALLOCATE cur;

    -------------------------------------------------------------------------
    -- Recalcular DVH/DVV para evitar pantalla de recuperación de integridad.
    -------------------------------------------------------------------------
    UPDATE dbo.Comision
    SET DVH = LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', CONCAT(
        N'ArancelBase=', CONVERT(NVARCHAR(128), ArancelBase), N'|',
        N'Codigo=', LTRIM(RTRIM(ISNULL(CONVERT(NVARCHAR(MAX), Codigo), N'∅'))), N'|',
        N'CupoMaximo=', CONVERT(NVARCHAR(128), CupoMaximo), N'|',
        N'CupoMinimo=', CONVERT(NVARCHAR(128), CupoMinimo), N'|',
        N'DiaSemana=', LTRIM(RTRIM(ISNULL(CONVERT(NVARCHAR(MAX), DiaSemana), N'∅'))), N'|',
        N'Estado=', LTRIM(RTRIM(ISNULL(CONVERT(NVARCHAR(MAX), Estado), N'∅'))), N'|',
        N'FechaFin=', ISNULL(CONVERT(NVARCHAR(23), CAST(FechaFin AS DATETIME), 126), N'∅'), N'|',
        N'FechaInicio=', ISNULL(CONVERT(NVARCHAR(23), CAST(FechaInicio AS DATETIME), 126), N'∅'), N'|',
        N'FechaLimitePago=', ISNULL(CONVERT(NVARCHAR(23), CAST(FechaLimitePago AS DATETIME), 126), N'∅'), N'|',
        N'HoraFin=', CONVERT(NVARCHAR(128), HoraFin), N'|',
        N'HoraInicio=', CONVERT(NVARCHAR(128), HoraInicio), N'|',
        N'IdComision=', CONVERT(NVARCHAR(128), IdComision), N'|',
        N'IdCurso=', CONVERT(NVARCHAR(128), IdCurso), N'|',
        N'IdPlanDePago=', CONVERT(NVARCHAR(128), IdPlanDePago), N'|',
        N'IdProfesor=', CONVERT(NVARCHAR(128), IdProfesor), N'|',
        N'MontoMatricula=', CONVERT(NVARCHAR(128), MontoMatricula), N'|',
        N'RecargoPlanSnapshot=', CONVERT(NVARCHAR(128), RecargoPlanSnapshot)
    )), 2));

    UPDATE dbo.Alumno
    SET DVH = LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', CONCAT(
        N'Apellido=', LTRIM(RTRIM(ISNULL(CONVERT(NVARCHAR(MAX), Apellido), N'∅'))), N'|',
        N'DNI=', LTRIM(RTRIM(ISNULL(CONVERT(NVARCHAR(MAX), DNI), N'∅'))), N'|',
        N'Email=', LTRIM(RTRIM(ISNULL(CONVERT(NVARCHAR(MAX), Email), N'∅'))), N'|',
        N'Estado=', LTRIM(RTRIM(ISNULL(CONVERT(NVARCHAR(MAX), Estado), N'∅'))), N'|',
        N'FechaAlta=', ISNULL(CONVERT(NVARCHAR(23), FechaAlta, 126), N'∅'), N'|',
        N'IdAlumno=', CONVERT(NVARCHAR(128), IdAlumno), N'|',
        N'Nombre=', LTRIM(RTRIM(ISNULL(CONVERT(NVARCHAR(MAX), Nombre), N'∅'))), N'|',
        N'Telefono=', LTRIM(RTRIM(ISNULL(CONVERT(NVARCHAR(MAX), Telefono), N'∅')))
    )), 2));

    UPDATE dbo.SolicitudInscripcion
    SET DVH = LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', CONCAT(
        N'Codigo=', LTRIM(RTRIM(ISNULL(CONVERT(NVARCHAR(MAX), Codigo), N'∅'))), N'|',
        N'Estado=', LTRIM(RTRIM(ISNULL(CONVERT(NVARCHAR(MAX), Estado), N'∅'))), N'|',
        N'FechaSolicitud=', ISNULL(CONVERT(NVARCHAR(23), FechaSolicitud, 126), N'∅'), N'|',
        N'IdAlumno=', CONVERT(NVARCHAR(128), IdAlumno), N'|',
        N'IdComision=', CONVERT(NVARCHAR(128), IdComision), N'|',
        N'IdLeadOrigen=', ISNULL(CONVERT(NVARCHAR(128), IdLeadOrigen), N'∅'), N'|',
        N'IdPlanDePago=', CONVERT(NVARCHAR(128), IdPlanDePago), N'|',
        N'IdSolicitudInscripcion=', CONVERT(NVARCHAR(128), IdSolicitudInscripcion), N'|',
        N'Observaciones=', LTRIM(RTRIM(ISNULL(CONVERT(NVARCHAR(MAX), Observaciones), N'∅'))), N'|',
        N'RecargoPlanSnapshot=', CONVERT(NVARCHAR(128), RecargoPlanSnapshot)
    )), 2));

    UPDATE dbo.Cuota
    SET DVH = LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', CONCAT(
        N'BalanceAdeudado=', CONVERT(NVARCHAR(128), BalanceAdeudado), N'|',
        N'Estado=', LTRIM(RTRIM(ISNULL(CONVERT(NVARCHAR(MAX), Estado), N'∅'))), N'|',
        N'FechaVencimiento=', ISNULL(CONVERT(NVARCHAR(23), CAST(FechaVencimiento AS DATETIME), 126), N'∅'), N'|',
        N'IdCuota=', CONVERT(NVARCHAR(128), IdCuota), N'|',
        N'IdSolicitudInscripcion=', CONVERT(NVARCHAR(128), IdSolicitudInscripcion), N'|',
        N'MontoOriginal=', CONVERT(NVARCHAR(128), MontoOriginal), N'|',
        N'NumeroCuota=', CONVERT(NVARCHAR(128), NumeroCuota)
    )), 2));

    UPDATE dbo.PagoInscripcion
    SET DVH = LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', CONCAT(
        N'FechaPago=', ISNULL(CONVERT(NVARCHAR(23), FechaPago, 126), N'∅'), N'|',
        N'IdAlumno=', CONVERT(NVARCHAR(128), IdAlumno), N'|',
        N'IdCuota=', CONVERT(NVARCHAR(128), IdCuota), N'|',
        N'IdPagoInscripcion=', CONVERT(NVARCHAR(128), IdPagoInscripcion), N'|',
        N'IdSolicitudInscripcion=', CONVERT(NVARCHAR(128), IdSolicitudInscripcion), N'|',
        N'MetodoPago=', LTRIM(RTRIM(ISNULL(CONVERT(NVARCHAR(MAX), MetodoPago), N'∅'))), N'|',
        N'MontoPagado=', CONVERT(NVARCHAR(128), MontoPagado), N'|',
        N'NumeroReferencia=', LTRIM(RTRIM(ISNULL(CONVERT(NVARCHAR(MAX), NumeroReferencia), N'∅')))
    )), 2));

    UPDATE dbo.DigitoVerificador_83KI
    SET DVV = LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', ISNULL(CAST((SELECT CAST(N'' AS NVARCHAR(MAX)) + DVH FROM dbo.Comision ORDER BY DVH FOR XML PATH(N'')) AS NVARCHAR(MAX)), N'')), 2)), FechaActualizacion = GETDATE()
    WHERE NombreTabla = N'Comision';

    UPDATE dbo.DigitoVerificador_83KI
    SET DVV = LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', ISNULL(CAST((SELECT CAST(N'' AS NVARCHAR(MAX)) + DVH FROM dbo.Alumno ORDER BY DVH FOR XML PATH(N'')) AS NVARCHAR(MAX)), N'')), 2)), FechaActualizacion = GETDATE()
    WHERE NombreTabla = N'Alumno';

    UPDATE dbo.DigitoVerificador_83KI
    SET DVV = LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', ISNULL(CAST((SELECT CAST(N'' AS NVARCHAR(MAX)) + DVH FROM dbo.SolicitudInscripcion ORDER BY DVH FOR XML PATH(N'')) AS NVARCHAR(MAX)), N'')), 2)), FechaActualizacion = GETDATE()
    WHERE NombreTabla = N'SolicitudInscripcion';

    UPDATE dbo.DigitoVerificador_83KI
    SET DVV = LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', ISNULL(CAST((SELECT CAST(N'' AS NVARCHAR(MAX)) + DVH FROM dbo.Cuota ORDER BY DVH FOR XML PATH(N'')) AS NVARCHAR(MAX)), N'')), 2)), FechaActualizacion = GETDATE()
    WHERE NombreTabla = N'Cuota';

    UPDATE dbo.DigitoVerificador_83KI
    SET DVV = LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', ISNULL(CAST((SELECT CAST(N'' AS NVARCHAR(MAX)) + DVH FROM dbo.PagoInscripcion ORDER BY DVH FOR XML PATH(N'')) AS NVARCHAR(MAX)), N'')), 2)), FechaActualizacion = GETDATE()
    WHERE NombreTabla = N'PagoInscripcion';

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;

PRINT N'Datos de prueba CUN06 creados correctamente.';
PRINT N'En la pantalla deben aparecer P01, P02, P03 y P04. No deben aparecer P05 ni P06.';

EXEC dbo.sp_CUN06_ListarComisionesVencidas;

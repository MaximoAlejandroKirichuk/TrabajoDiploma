/* Ejecutar en una base desechable después de aplicar 001_auditoria.sql.
   Se revierten todos los datos de prueba, incluida la transacción del procedimiento. */
SET NOCOUNT ON;
SET XACT_ABORT OFF;
SET ANSI_NULLS ON;
SET ANSI_PADDING ON;
SET ANSI_WARNINGS ON;
SET ARITHABORT ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET QUOTED_IDENTIFIER ON;
SET NUMERIC_ROUNDABORT OFF;
SET QUOTED_IDENTIFIER ON;
DECLARE @IdProfesor INT = (SELECT TOP (1) IdProfesor FROM dbo.Profesor ORDER BY IdProfesor);
IF @IdProfesor IS NULL THROW 52990, N'Smoke requires at least one disposable Profesor row.', 1;
DECLARE @Antes INT = (SELECT COUNT(*) FROM dbo.DisponibilidadProfesor_C);
DECLARE @Ids TABLE (Id INT PRIMARY KEY);
BEGIN TRANSACTION;
BEGIN TRY
    EXEC sys.sp_set_session_context @key=N'DisponibilidadProfesorUsuario', @value=N'Ana auditoría';
    INSERT dbo.DisponibilidadProfesor (IdProfesor, DiaSemana, HoraInicio, HoraFin, EstadoActivo, DVH)
    OUTPUT inserted.IdDisponibilidadProfesor INTO @Ids
    VALUES (@IdProfesor, N'SmokeAuditA', '01:00', '02:00', 1, ''),
           (@IdProfesor, N'SmokeAuditB', '03:00', '04:00', 1, '');
    IF (SELECT COUNT(*) FROM dbo.DisponibilidadProfesor_C WHERE IdDisponibilidadProfesor IN (SELECT Id FROM @Ids) AND Act=1) <> 2
        THROW 52991, N'Multirow insert did not create active snapshots.', 1;
    IF EXISTS (SELECT 1 FROM dbo.DisponibilidadProfesor_C WHERE IdDisponibilidadProfesor IN (SELECT Id FROM @Ids)
               AND (TipoOperacion<>N'INSERT' OR ISNULL(Usuario,N'')<>N'Ana auditoría'))
        THROW 53010, N'Insert snapshots did not capture the operation and application user.', 1;

    DECLARE @Id INT=(SELECT MIN(Id) FROM @Ids), @Version BIGINT;
    SELECT @Version=IdVersion FROM dbo.DisponibilidadProfesor_C WHERE IdDisponibilidadProfesor=@Id AND Act=1;
    INSERT dbo.Comision(Codigo,IdProfesor,DiaSemana,HoraInicio,HoraFin,Estado,FechaFin)
    VALUES(N'SMOKE-AUDIT-COMMISSION',@IdProfesor,N'SmokeAuditA','01:15','01:45',N'preapertura',DATEADD(day,30,CONVERT(date,GETDATE())));
    EXEC sys.sp_set_session_context @key=N'DisponibilidadProfesorUsuario', @value=N'Pedro';
    UPDATE dbo.DisponibilidadProfesor SET HoraInicio='01:15' WHERE IdDisponibilidadProfesor=@Id;
    IF (SELECT COUNT(*) FROM dbo.DisponibilidadProfesor_C WHERE IdDisponibilidadProfesor=@Id) <> 2
        THROW 52992, N'Business update did not create exactly one snapshot.', 1;
    IF NOT EXISTS (SELECT 1 FROM dbo.DisponibilidadProfesor_C WHERE IdDisponibilidadProfesor=@Id AND Act=1 AND TipoOperacion=N'UPDATE' AND Usuario=N'Pedro')
        THROW 53011, N'Update did not capture the changed actor.', 1;
    UPDATE dbo.DisponibilidadProfesor SET DVH=DVH WHERE IdDisponibilidadProfesor=@Id;
    UPDATE dbo.DisponibilidadProfesor SET HoraInicio='01:15' WHERE IdDisponibilidadProfesor=@Id;
    IF (SELECT COUNT(*) FROM dbo.DisponibilidadProfesor_C WHERE IdDisponibilidadProfesor=@Id) <> 2
        THROW 52993, N'No-op/DVH-only updates created snapshots.', 1;

    DELETE dbo.Comision WHERE Codigo=N'SMOKE-AUDIT-COMMISSION';
    UPDATE dbo.DisponibilidadProfesor SET EstadoActivo=0 WHERE IdDisponibilidadProfesor=@Id;
    IF NOT EXISTS (SELECT 1 FROM dbo.DisponibilidadProfesor_C WHERE IdDisponibilidadProfesor=@Id AND Act=1 AND TipoOperacion=N'BAJA_LOGICA' AND Usuario=N'Pedro')
        THROW 53012, N'Logical deactivation was not classified.', 1;
    UPDATE dbo.DisponibilidadProfesor SET EstadoActivo=1 WHERE IdDisponibilidadProfesor=@Id;
    IF NOT EXISTS (SELECT 1 FROM dbo.DisponibilidadProfesor_C WHERE IdDisponibilidadProfesor=@Id AND Act=1 AND TipoOperacion=N'REACTIVACION' AND Usuario=N'Pedro')
        THROW 53013, N'Reactivation was not classified.', 1;
    EXEC dbo.sp_DisponibilidadProfesor_ActivarVersion @IdVersion=@Version;
    IF NOT EXISTS (SELECT 1 FROM dbo.DisponibilidadProfesor_C WHERE IdVersion=@Version AND TipoOperacion=N'INSERT' AND Usuario=N'Ana auditoría')
        THROW 53014, N'Restoration overwrote the original snapshot attribution.', 1;
    EXEC sys.sp_set_session_context @key=N'DisponibilidadProfesorUsuario', @value=NULL;
    IF NOT EXISTS (SELECT 1 FROM dbo.DisponibilidadProfesor d JOIN dbo.DisponibilidadProfesor_C h ON h.IdDisponibilidadProfesor=d.IdDisponibilidadProfesor AND h.Act=1
                   WHERE d.IdDisponibilidadProfesor=@Id AND d.HoraInicio='01:00' AND h.IdVersion=@Version)
        THROW 52994, N'Historical version restoration did not synchronize current state.', 1;

    UPDATE dbo.DisponibilidadProfesor SET HoraInicio='01:05' WHERE IdDisponibilidadProfesor=@Id;
    IF NOT EXISTS (SELECT 1 FROM dbo.DisponibilidadProfesor_C WHERE IdDisponibilidadProfesor=@Id AND Act=1 AND TipoOperacion=N'UPDATE' AND Usuario IS NULL)
        THROW 53015, N'A write without application identity inherited a previous actor.', 1;
    UPDATE dbo.DisponibilidadProfesor_C SET Act=0 WHERE IdDisponibilidadProfesor=@Id AND Act=1;
    UPDATE dbo.DisponibilidadProfesor_C SET Act=1 WHERE IdVersion=@Version AND Act=0;
    IF NOT EXISTS (SELECT 1 FROM dbo.DisponibilidadProfesor d JOIN dbo.DisponibilidadProfesor_C h ON h.IdDisponibilidadProfesor=d.IdDisponibilidadProfesor AND h.Act=1
                   WHERE d.IdDisponibilidadProfesor=@Id AND d.HoraInicio='01:00' AND h.IdVersion=@Version)
        THROW 53002, N'Direct historical activation did not synchronize current state.', 1;
    IF (SELECT COUNT(*) FROM dbo.DisponibilidadProfesor_C WHERE IdDisponibilidadProfesor=@Id AND Act=1) <> 1
        THROW 53003, N'Historical activation did not preserve one active version.', 1;

    ROLLBACK TRANSACTION;
    IF (SELECT COUNT(*) FROM dbo.DisponibilidadProfesor_C) <> @Antes
        THROW 52995, N'Smoke rollback did not restore history.', 1;
    /* Un error de trigger puede invalidar la transacción por XACT_ABORT.
       Por eso, el rechazo de cobertura se prueba dentro de otra transacción desechable. */
    DECLARE @IdNegativo INT, @Rechazado BIT=0;
    BEGIN TRANSACTION;
    INSERT dbo.DisponibilidadProfesor (IdProfesor,DiaSemana,HoraInicio,HoraFin,EstadoActivo,DVH)
    VALUES(@IdProfesor,N'SmokeAuditNegative','01:00','02:00',1,'');
    SET @IdNegativo=CONVERT(INT,SCOPE_IDENTITY());
    INSERT dbo.Comision(Codigo,IdProfesor,DiaSemana,HoraInicio,HoraFin,Estado,FechaFin)
    VALUES(N'SMOKE-AUDIT-NEGATIVE',@IdProfesor,N'SmokeAuditNegative','01:15','01:45',N'preapertura',DATEADD(day,30,CONVERT(date,GETDATE())));
    BEGIN TRY
        UPDATE dbo.DisponibilidadProfesor SET HoraInicio='02:00',HoraFin='02:30' WHERE IdDisponibilidadProfesor=@IdNegativo;
    END TRY
    BEGIN CATCH
        IF ERROR_NUMBER()<>52801 THROW;
        SET @Rechazado=1;
        IF XACT_STATE()<>0 ROLLBACK TRANSACTION;
    END CATCH;
    IF @Rechazado=0 BEGIN ROLLBACK TRANSACTION; THROW 52996, N'Uncovered commission change was not rejected.', 1; END;
    IF (SELECT COUNT(*) FROM dbo.DisponibilidadProfesor_C) <> @Antes
        THROW 52998, N'Rejected update did not roll back its fixture/history writes.', 1;

    DECLARE @IdCambio INT, @IdReemplazo INT, @IdParcial INT, @RechazoParcial BIT=0;
    BEGIN TRANSACTION;
    INSERT dbo.DisponibilidadProfesor (IdProfesor,DiaSemana,HoraInicio,HoraFin,EstadoActivo,DVH)
    VALUES (@IdProfesor,N'SmokeFallback','09:00','13:00',1,''),
           (@IdProfesor,N'SmokeFallback','09:00','13:00',1,'');
    SET @IdCambio=CONVERT(INT,SCOPE_IDENTITY());
    SET @IdReemplazo=@IdCambio-1;
    INSERT dbo.Comision(Codigo,IdProfesor,DiaSemana,HoraInicio,HoraFin,Estado,FechaFin)
    VALUES(N'SMOKE-AUDIT-FALLBACK',@IdProfesor,N'SmokeFallback','10:00','12:00',N'confirmada',DATEADD(day,30,CONVERT(date,GETDATE())));
    UPDATE dbo.DisponibilidadProfesor SET DiaSemana=N'SmokeChanged' WHERE IdDisponibilidadProfesor=@IdCambio;
    IF NOT EXISTS (SELECT 1 FROM dbo.DisponibilidadProfesor WHERE IdDisponibilidadProfesor=@IdReemplazo AND DiaSemana=N'SmokeFallback' AND EstadoActivo=1)
        THROW 52999, N'An independent full-coverage availability did not permit the change.', 1;

    INSERT dbo.DisponibilidadProfesor (IdProfesor,DiaSemana,HoraInicio,HoraFin,EstadoActivo,DVH)
    VALUES (@IdProfesor,N'SmokePartial','08:00','11:00',1,''),
           (@IdProfesor,N'SmokePartial','08:00','09:00',1,''),
           (@IdProfesor,N'SmokePartial','09:00','11:00',1,'');
    SET @IdParcial=CONVERT(INT,SCOPE_IDENTITY())-2;
    INSERT dbo.Comision(Codigo,IdProfesor,DiaSemana,HoraInicio,HoraFin,Estado,FechaFin)
    VALUES(N'SMOKE-AUDIT-PARTIAL',@IdProfesor,N'SmokePartial','08:30','10:30',N'preapertura',DATEADD(day,30,CONVERT(date,GETDATE())));
    BEGIN TRY
        UPDATE dbo.DisponibilidadProfesor SET EstadoActivo=0 WHERE IdDisponibilidadProfesor=@IdParcial;
    END TRY
    BEGIN CATCH
        IF ERROR_NUMBER()<>52801 THROW;
        SET @RechazoParcial=1;
        IF XACT_STATE()<>0 ROLLBACK TRANSACTION;
    END CATCH;
    IF @RechazoParcial=0 BEGIN ROLLBACK TRANSACTION; THROW 53000, N'Partial intervals were combined to cover a commission.', 1; END;
    IF (SELECT COUNT(*) FROM dbo.DisponibilidadProfesor_C) <> @Antes
        THROW 53001, N'Coverage fixtures were not rolled back.', 1;
    DECLARE @IdOriginal INT, @VersionDuplicada BIGINT, @DuplicadoRechazado BIT=0;
    BEGIN TRANSACTION;
    INSERT dbo.DisponibilidadProfesor (IdProfesor,DiaSemana,HoraInicio,HoraFin,EstadoActivo,DVH)
    VALUES(@IdProfesor,N'SmokeRestoreDup','01:00','02:00',1,'');
    SET @IdOriginal=CONVERT(INT,SCOPE_IDENTITY());
    SELECT @VersionDuplicada=IdVersion FROM dbo.DisponibilidadProfesor_C WHERE IdDisponibilidadProfesor=@IdOriginal AND Act=1;
    UPDATE dbo.DisponibilidadProfesor SET HoraInicio='03:00',HoraFin='04:00' WHERE IdDisponibilidadProfesor=@IdOriginal;
    INSERT dbo.DisponibilidadProfesor (IdProfesor,DiaSemana,HoraInicio,HoraFin,EstadoActivo,DVH)
    VALUES(@IdProfesor,N'SmokeRestoreDup','01:00','02:00',1,'');
    BEGIN TRY
        EXEC dbo.sp_DisponibilidadProfesor_ActivarVersion @IdVersion=@VersionDuplicada;
    END TRY
    BEGIN CATCH
        IF ERROR_NUMBER()<>52803 THROW;
        SET @DuplicadoRechazado=1;
    END CATCH;
    IF XACT_STATE()<>0 ROLLBACK TRANSACTION;
    IF @DuplicadoRechazado=0 THROW 53019, N'Se permitió restaurar una disponibilidad activa duplicada.', 1;
    IF (SELECT COUNT(*) FROM dbo.DisponibilidadProfesor_C) <> @Antes
        THROW 53020, N'La prueba de restauración duplicada dejó cambios persistidos.', 1;
    PRINT N'Pruebas de auditoría correctas: cambios revertidos y rechazos de cobertura y duplicados verificados.';
END TRY
BEGIN CATCH
    EXEC sys.sp_set_session_context @key=N'DisponibilidadProfesorUsuario', @value=NULL;
    IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;

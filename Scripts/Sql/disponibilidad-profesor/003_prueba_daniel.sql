/* Pruebas SQL controladas para Daniel. Ejecutar con sqlcmd -b o desde SSMS.
   Los cambios se revierten y al finalizar se comparan sus disponibilidades e historial originales. */
SET NOCOUNT ON;
SET XACT_ABORT OFF;
SET ANSI_NULLS ON;
SET ANSI_PADDING ON;
SET ANSI_WARNINGS ON;
SET ARITHABORT ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET QUOTED_IDENTIFIER ON;
SET NUMERIC_ROUNDABORT OFF;

DECLARE @IdProfesor INT = (SELECT IdProfesor FROM dbo.Profesor WHERE DNI = N'28188200');
IF @IdProfesor IS NULL THROW 53100, N'No se encontró al profesor DNI 28188200.', 1;
IF DB_NAME() <> N'TrabajoDiploma9/13'
    THROW 53101, N'Esta prueba requiere TrabajoDiploma9/13.', 1;

SELECT * INTO #DisponibilidadesAntes FROM dbo.DisponibilidadProfesor WHERE IdProfesor = @IdProfesor;
SELECT * INTO #HistorialAntes FROM dbo.DisponibilidadProfesor_C WHERE IdProfesor = @IdProfesor;
DECLARE @Id INT, @VersionInicial BIGINT, @Cantidad INT;

BEGIN TRY
    BEGIN TRANSACTION;
    EXEC sys.sp_set_session_context @key=N'DisponibilidadProfesorUsuario', @value=N'Prueba automatizada Daniel';
    INSERT dbo.DisponibilidadProfesor(IdProfesor,DiaSemana,HoraInicio,HoraFin,EstadoActivo,DVH)
    VALUES(@IdProfesor,N'Sunday','06:00','08:00',1,'');
    SET @Id=CONVERT(INT,SCOPE_IDENTITY());
    SELECT @VersionInicial=IdVersion FROM dbo.DisponibilidadProfesor_C
    WHERE IdDisponibilidadProfesor=@Id AND Act=1 AND TipoOperacion=N'INSERT' AND Usuario=N'Prueba automatizada Daniel';
    IF @VersionInicial IS NULL THROW 53102, N'Falló alta con operación y usuario.', 1;
    PRINT N'PASS 01: alta con operación y usuario.';

    UPDATE dbo.DisponibilidadProfesor SET HoraFin='09:00' WHERE IdDisponibilidadProfesor=@Id;
    IF NOT EXISTS (SELECT 1 FROM dbo.DisponibilidadProfesor_C WHERE IdDisponibilidadProfesor=@Id AND Act=1
                   AND HoraFin='09:00' AND TipoOperacion=N'UPDATE' AND Usuario=N'Prueba automatizada Daniel')
        THROW 53103, N'Falló modificación.', 1;
    SELECT @Cantidad=COUNT(*) FROM dbo.DisponibilidadProfesor_C WHERE IdDisponibilidadProfesor=@Id;
    IF @Cantidad<>2 THROW 53104, N'La modificación duplicó versiones.', 1;
    PRINT N'PASS 02: modificación efectiva, una versión nueva.';

    UPDATE dbo.DisponibilidadProfesor SET HoraFin='09:00' WHERE IdDisponibilidadProfesor=@Id;
    UPDATE dbo.DisponibilidadProfesor SET DVH=DVH WHERE IdDisponibilidadProfesor=@Id;
    IF (SELECT COUNT(*) FROM dbo.DisponibilidadProfesor_C WHERE IdDisponibilidadProfesor=@Id)<>@Cantidad
        THROW 53105, N'Guardar sin cambios generó una versión.', 1;
    PRINT N'PASS 03: guardar sin cambios y DVH-only no generan versiones.';

    UPDATE dbo.DisponibilidadProfesor SET EstadoActivo=0 WHERE IdDisponibilidadProfesor=@Id;
    IF NOT EXISTS (SELECT 1 FROM dbo.DisponibilidadProfesor_C WHERE IdDisponibilidadProfesor=@Id AND Act=1 AND EstadoActivo=0 AND TipoOperacion=N'BAJA_LOGICA')
        THROW 53106, N'Falló baja lógica.', 1;
    PRINT N'PASS 04: baja lógica registrada.';
    UPDATE dbo.DisponibilidadProfesor SET EstadoActivo=1 WHERE IdDisponibilidadProfesor=@Id;
    IF NOT EXISTS (SELECT 1 FROM dbo.DisponibilidadProfesor_C WHERE IdDisponibilidadProfesor=@Id AND Act=1 AND EstadoActivo=1 AND TipoOperacion=N'REACTIVACION')
        THROW 53107, N'Falló reactivación.', 1;
    PRINT N'PASS 05: reactivación registrada.';

    SELECT @Cantidad=COUNT(*) FROM dbo.DisponibilidadProfesor_C WHERE IdDisponibilidadProfesor=@Id;
    EXEC sys.sp_set_session_context @key=N'DisponibilidadProfesorUsuario', @value=N'Otro actor de prueba';
    EXEC dbo.sp_DisponibilidadProfesor_ActivarVersion @IdVersion=@VersionInicial;
    IF NOT EXISTS (SELECT 1 FROM dbo.DisponibilidadProfesor d JOIN dbo.DisponibilidadProfesor_C h
                   ON h.IdDisponibilidadProfesor=d.IdDisponibilidadProfesor
                   WHERE d.IdDisponibilidadProfesor=@Id AND d.HoraFin='08:00' AND h.IdVersion=@VersionInicial
                     AND h.Act=1 AND h.TipoOperacion=N'INSERT' AND h.Usuario=N'Prueba automatizada Daniel')
        THROW 53108, N'La activación no recuperó datos o cambió la autoría original.', 1;
    IF (SELECT COUNT(*) FROM dbo.DisponibilidadProfesor_C WHERE IdDisponibilidadProfesor=@Id)<>@Cantidad
        THROW 53109, N'La activación generó una versión duplicada.', 1;
    PRINT N'PASS 06: activación histórica sin duplicado y con autoría original.';
    EXEC dbo.sp_DisponibilidadProfesor_ActivarVersion @IdVersion=@VersionInicial;
    IF (SELECT COUNT(*) FROM dbo.DisponibilidadProfesor_C WHERE IdDisponibilidadProfesor=@Id)<>@Cantidad
        THROW 53110, N'Activar la versión vigente generó una versión.', 1;
    PRINT N'PASS 07: activar la vigente no agrega versiones.';

    UPDATE dbo.DisponibilidadProfesor SET HoraFin='10:00' WHERE IdDisponibilidadProfesor=@Id;
    IF NOT EXISTS (SELECT 1 FROM dbo.DisponibilidadProfesor_C WHERE IdDisponibilidadProfesor=@Id AND Act=1
                   AND HoraFin='10:00' AND TipoOperacion=N'UPDATE' AND Usuario=N'Otro actor de prueba')
        THROW 53111, N'Falló modificar después de activar o atribuir al nuevo usuario.', 1;
    IF (SELECT COUNT(*) FROM dbo.DisponibilidadProfesor_C WHERE IdDisponibilidadProfesor=@Id AND Act=1)<>1
        THROW 53112, N'Hay más de una versión vigente.', 1;
    PRINT N'PASS 08: modificación posterior y usuario nuevo.';
    ROLLBACK TRANSACTION;
    EXEC sys.sp_set_session_context @key=N'DisponibilidadProfesorUsuario', @value=NULL;

    IF NOT EXISTS (SELECT 1 FROM dbo.Comision WHERE IdProfesor=@IdProfesor AND DiaSemana=N'Monday'
                   AND Estado IN (N'preapertura',N'confirmada') AND FechaFin>=CONVERT(date,GETDATE()))
        THROW 53113, N'No existe una comisión vigente del lunes para verificar el rechazo.', 1;
    DECLARE @Rechazado BIT=0;
    BEGIN TRANSACTION;
    BEGIN TRY
        UPDATE dbo.DisponibilidadProfesor SET EstadoActivo=0
        WHERE IdProfesor=@IdProfesor AND DiaSemana=N'Monday' AND EstadoActivo=1;
    END TRY
    BEGIN CATCH
        IF ERROR_NUMBER()<>52801 THROW;
        PRINT N'PASS 09: pérdida de cobertura rechazada: ' + ERROR_MESSAGE();
        SET @Rechazado=1;
    END CATCH;
    IF XACT_STATE()<>0 ROLLBACK TRANSACTION;
    IF @Rechazado=0 THROW 53114, N'No se rechazó la pérdida de cobertura.', 1;

    IF EXISTS (SELECT * FROM #DisponibilidadesAntes EXCEPT SELECT * FROM dbo.DisponibilidadProfesor WHERE IdProfesor=@IdProfesor)
       OR EXISTS (SELECT * FROM dbo.DisponibilidadProfesor WHERE IdProfesor=@IdProfesor EXCEPT SELECT * FROM #DisponibilidadesAntes)
       OR EXISTS (SELECT * FROM #HistorialAntes EXCEPT SELECT * FROM dbo.DisponibilidadProfesor_C WHERE IdProfesor=@IdProfesor)
       OR EXISTS (SELECT * FROM dbo.DisponibilidadProfesor_C WHERE IdProfesor=@IdProfesor EXCEPT SELECT * FROM #HistorialAntes)
        THROW 53115, N'Los datos finales difieren de los originales.', 1;
    PRINT N'PASS 10: horarios, estados, DVH e historial de Daniel conservados íntegramente.';
END TRY
BEGIN CATCH
    IF XACT_STATE()<>0 ROLLBACK TRANSACTION;
    EXEC sys.sp_set_session_context @key=N'DisponibilidadProfesorUsuario', @value=NULL;
    THROW;
END CATCH;
DROP TABLE #DisponibilidadesAntes;
DROP TABLE #HistorialAntes;

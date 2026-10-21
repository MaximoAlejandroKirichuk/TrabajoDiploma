-- Regression fixtures for multiple enrollment fees belonging to the same student.
SET NOCOUNT ON;
IF DB_NAME() NOT LIKE N'FinanceTests[_]%' THROW 52900,N'Use the isolated financing test runner.',1;
DECLARE @plan INT,@curso INT=(SELECT TOP 1 IdCurso FROM dbo.Curso),@profesor INT=(SELECT TOP 1 IdProfesor FROM dbo.Profesor),
    @comision INT,@alumno INT,@solicitud INT,@cuota INT,@future DATETIME=DATEADD(DAY,1,CONVERT(date,GETDATE()));
INSERT dbo.PlanesDePago(Nombre,RecargoPorcentaje,EstadoActivo,CantidadCuotas) VALUES(N'Review Contado',0,1,1);
SET @plan=SCOPE_IDENTITY();
INSERT dbo.Comision(Codigo,IdCurso,IdProfesor,DiaSemana,HoraInicio,HoraFin,CupoMinimo,CupoMaximo,FechaLimitePago,FechaInicio,FechaFin,ArancelBase,MontoMatricula,IdPlanDePago,RecargoPlanSnapshot,Estado,DVH)
VALUES(N'REVIEW-COM-A',@curso,@profesor,N'Monday','18:00','20:00',1,30,DATEADD(DAY,10,GETDATE()),'20280101','20280701',100000,10000,@plan,0,N'preapertura','');
SET @comision=SCOPE_IDENTITY();
EXEC dbo.sp_CUN03_RegistrarSolicitudInscripcion N'99885511',N'Review',N'Student',N'review.student@example.com',N'123456',NULL,NULL,@comision,@plan,N'First enrollment';
SELECT @alumno=IdAlumno FROM dbo.Alumno WHERE DNI=N'99885511';
INSERT dbo.Comision(Codigo,IdCurso,IdProfesor,DiaSemana,HoraInicio,HoraFin,CupoMinimo,CupoMaximo,FechaLimitePago,FechaInicio,FechaFin,ArancelBase,MontoMatricula,IdPlanDePago,RecargoPlanSnapshot,Estado,DVH)
VALUES(N'REVIEW-COM-B',@curso,@profesor,N'Monday','18:00','20:00',1,30,DATEADD(DAY,10,GETDATE()),'20280101','20280701',200000,20000,@plan,0,N'preapertura','');
SET @comision=SCOPE_IDENTITY();
EXEC dbo.sp_CUN03_RegistrarSolicitudInscripcion N'99885511',N'Review',N'Student',N'review.student@example.com',N'123456',NULL,@alumno,@comision,@plan,N'Second enrollment';
SELECT @solicitud=IdSolicitudInscripcion FROM dbo.SolicitudInscripcion WHERE IdAlumno=@alumno AND IdComision=@comision;
SELECT @cuota=IdCuota FROM dbo.Cuota WHERE IdSolicitudInscripcion=@solicitud AND TipoConcepto=N'matricula';
BEGIN TRY
    EXEC dbo.sp_CUN05_RegistrarPagoInscripcion @alumno,@solicitud,@cuota,N'transferencia',20000,N'REVIEW-FUTURE',@future;
    THROW 52924,N'CUN05 accepted a future payment date.',1;
END TRY BEGIN CATCH IF ERROR_NUMBER()<>52607 THROW; END CATCH;
IF EXISTS(SELECT 1 FROM dbo.PagoInscripcion WHERE NumeroReferencia=N'REVIEW-FUTURE')
    THROW 52925,N'Future payment was persisted.',1;
IF NOT EXISTS(SELECT 1 FROM dbo.SolicitudInscripcion WHERE IdSolicitudInscripcion=@solicitud AND Estado=N'pendiente')
    THROW 52926,N'A rejected future payment activated enrollment.',1;
PRINT 'PASS: CUN05 rejects future payment dates without changing enrollment or payments.';
GO

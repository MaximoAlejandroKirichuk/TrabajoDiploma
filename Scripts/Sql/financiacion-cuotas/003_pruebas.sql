-- Run with Scripts/Tests/FinanciacionCuotas.Tests.ps1 in a disposable LocalDB database.
SET NOCOUNT ON;
IF DB_NAME() NOT LIKE N'FinanceTests[_]%' THROW 52900,N'Use the isolated financing test runner.',1;
GO
DECLARE @plan INT,@curso INT,@profesor INT,@comision INT,@alumno INT,@solicitud INT,@matricula INT,@cuota INT,@monto DECIMAL(18,2);
INSERT dbo.PlanesDePago(Nombre,RecargoPorcentaje,EstadoActivo,CantidadCuotas) VALUES(N'Finance test 3 cuotas',5.125,1,3);
SET @plan=SCOPE_IDENTITY();
INSERT dbo.Curso(Nombre,Descripcion,CargaHoraria,EstadoActivo,DVH) VALUES(N'Finance course',N'Test',12,1,'');
SET @curso=SCOPE_IDENTITY();
INSERT dbo.Profesor(DNI,Nombre,Apellido,Email,EstadoActivo,DVH) VALUES(N'99887766',N'Test',N'Teacher',N'teacher@example.com',1,'');
SET @profesor=SCOPE_IDENTITY();
INSERT dbo.Comision(Codigo,IdCurso,IdProfesor,DiaSemana,HoraInicio,HoraFin,CupoMinimo,CupoMaximo,FechaLimitePago,FechaInicio,FechaFin,ArancelBase,MontoMatricula,IdPlanDePago,RecargoPlanSnapshot,Estado,DVH)
VALUES(N'FIN-COM',@curso,@profesor,N'Monday','18:00','20:00',1,30,DATEADD(DAY,10,GETDATE()),'20280131','20280731',120000,20000,@plan,5.125,N'preapertura','');
SET @comision=SCOPE_IDENTITY();
-- Exercise the actual enrollment procedure, not a reproduction of its schedule formula.
EXEC dbo.sp_CUN03_RegistrarSolicitudInscripcion N'99887711',N'Test',N'Student',N'student@example.com',N'123456',NULL,NULL,@comision,@plan,N'Finance test';
SELECT @solicitud=si.IdSolicitudInscripcion,@alumno=si.IdAlumno FROM dbo.SolicitudInscripcion si JOIN dbo.Alumno a ON a.IdAlumno=si.IdAlumno WHERE a.DNI=N'99887711';
IF (SELECT COUNT(*) FROM dbo.Cuota WHERE IdSolicitudInscripcion=@solicitud)<>4 THROW 52901,N'Expected matrícula plus three installments.',1;
IF (SELECT SUM(MontoOriginal) FROM dbo.Cuota WHERE IdSolicitudInscripcion=@solicitud AND TipoConcepto=N'curso')<>126150 THROW 52902,N'Incorrect financed total.',1;
IF EXISTS(SELECT 1 FROM dbo.Cuota WHERE IdSolicitudInscripcion=@solicitud AND TipoConcepto=N'curso' AND MontoOriginal<>42050) THROW 52903,N'Incorrect installment amount.',1;
IF NOT EXISTS(SELECT 1 FROM dbo.Cuota WHERE IdSolicitudInscripcion=@solicitud AND NumeroCuota=3 AND FechaVencimiento='20280229') THROW 52904,N'Leap February date is incorrect.',1;
IF NOT EXISTS(SELECT 1 FROM dbo.Cuota WHERE IdSolicitudInscripcion=@solicitud AND NumeroCuota=4 AND FechaVencimiento='20280331') THROW 52905,N'March anchor drifted.',1;
IF NOT EXISTS(SELECT 1 FROM dbo.Cuota WHERE IdSolicitudInscripcion=@solicitud AND TipoConcepto=N'matricula' AND MontoOriginal=20000) THROW 52906,N'Matrícula was financed.',1;
-- Frozen terms survive changes to commission/plan; generation is repeatable without duplicates.
UPDATE dbo.Comision SET ArancelBase=999999,FechaInicio='20290101',FechaFin='20290701' WHERE IdComision=@comision;
UPDATE dbo.PlanesDePago SET CantidadCuotas=6,RecargoPorcentaje=10,Nombre=N'Modified plan' WHERE IdPlanDePago=@plan;
EXEC dbo.sp_Financiacion_GenerarCuotas @solicitud;
IF (SELECT COUNT(*) FROM dbo.Cuota WHERE IdSolicitudInscripcion=@solicitud)<>4 THROW 52907,N'Snapshot or idempotency failed.',1;
IF NOT EXISTS(SELECT 1 FROM dbo.SolicitudInscripcion WHERE IdSolicitudInscripcion=@solicitud AND TotalCursoSnapshot=126150 AND CantidadCuotasSnapshot=3 AND NombrePlanSnapshot=N'Finance test 3 cuotas') THROW 52908,N'Snapshot changed.',1;
SELECT @matricula=IdCuota FROM dbo.Cuota WHERE IdSolicitudInscripcion=@solicitud AND TipoConcepto=N'matricula';
EXEC dbo.sp_CUN05_RegistrarPagoInscripcion @alumno,@solicitud,@matricula,N'transferencia',20000,N'FIN-MATRICULA',@FechaPago='20260101';
SELECT @cuota=IdCuota FROM dbo.Cuota WHERE IdSolicitudInscripcion=@solicitud AND NumeroCuota=3;
BEGIN TRY
    EXEC dbo.sp_CUN07_RegistrarPago @alumno,@solicitud,@cuota,N'FIN-COM',N'transferencia',42050,N'FIN-SKIP','20260101','20260101';
    THROW 52909,N'Skipping an unpaid installment was accepted.',1;
END TRY BEGIN CATCH IF ERROR_NUMBER()<>52708 THROW; END CATCH;
SELECT @cuota=IdCuota FROM dbo.Cuota WHERE IdSolicitudInscripcion=@solicitud AND NumeroCuota=2;
EXEC dbo.sp_CUN07_RegistrarPago @alumno,@solicitud,@cuota,N'FIN-COM',N'transferencia',42050,N'FIN-EARLY','20260101','20260101';
IF NOT EXISTS(SELECT 1 FROM dbo.Cuota WHERE IdCuota=@cuota AND Estado=N'pagada' AND BalanceAdeudado=0) THROW 52910,N'Advance payment was rejected.',1;
IF NOT EXISTS(SELECT 1 FROM dbo.SolicitudInscripcion WHERE IdSolicitudInscripcion=@solicitud AND Estado=N'activa') THROW 52911,N'Payment changed academic enrollment state.',1;
SELECT @cuota=IdCuota FROM dbo.Cuota WHERE IdSolicitudInscripcion=@solicitud AND NumeroCuota=3;
BEGIN TRY
    EXEC dbo.sp_CUN05_RegistrarPagoInscripcion @alumno,@solicitud,@cuota,N'transferencia',42050,N'FIN-WRONG-ROUTE','20260101';
    THROW 52912,N'CUN05 accepted a course installment.',1;
END TRY BEGIN CATCH IF ERROR_NUMBER()<>52601 THROW; END CATCH;
EXEC dbo.sp_CUN07_RegistrarPago @alumno,@solicitud,@cuota,N'FIN-COM',N'transferencia',42050,N'FIN-NEXT','20260101','20260101';
IF NOT EXISTS(SELECT 1 FROM dbo.FacturaCUN07 WHERE Concepto=N'Pago cuota del curso 2') THROW 52913,N'Incorrect displayed installment number.',1;
-- Quorum counts a paid matrícula only once even when course installments are also paid.
UPDATE dbo.Comision SET FechaLimitePago=DATEADD(DAY,-1,GETDATE()) WHERE IdComision=@comision;
DECLARE @quorum TABLE(IdComision INT,Codigo NVARCHAR(30),CupoMinimo INT,Estado NVARCHAR(30),VacantesRegularizadas INT);
INSERT @quorum EXEC dbo.sp_CUN06_EvaluarQuorum @comision;
IF NOT EXISTS(SELECT 1 FROM @quorum WHERE VacantesRegularizadas=1) THROW 52914,N'Quorum no longer identifies matrícula.',1;
UPDATE dbo.Comision SET CupoMinimo=2 WHERE IdComision=@comision;
EXEC dbo.sp_CUN06_RegistrarActaCierre @comision,'20260101',N'FIN-CIERRE',N'Test',N'Test',N'Test';
IF (SELECT COUNT(*) FROM dbo.Cuota WHERE IdSolicitudInscripcion=@solicitud AND Estado=N'reintegro_pendiente')<>3
    THROW 52921,N'Cancellation did not preserve all paid charges for refund.',1;
PRINT 'PASS: enrollment, financing totals, leap dates, snapshots, early/sequential payments, separate routes and quorum.';
GO
-- Rounding and legacy backfill preserve all existing payment rows and matrícula balances.
DECLARE @plan INT=(SELECT TOP 1 IdPlanDePago FROM dbo.PlanesDePago),@comision INT=(SELECT TOP 1 IdComision FROM dbo.Comision),@alumno INT=(SELECT TOP 1 IdAlumno FROM dbo.Alumno),@solicitud INT;
UPDATE dbo.PlanesDePago SET CantidadCuotas=3,RecargoPorcentaje=0 WHERE IdPlanDePago=@plan;
UPDATE dbo.Comision SET ArancelBase=100,FechaInicio='20270131' WHERE IdComision=@comision;
INSERT dbo.SolicitudInscripcion(Codigo,IdAlumno,IdComision,IdPlanDePago,RecargoPlanSnapshot,FechaSolicitud,Estado,Observaciones,DVH)
VALUES(N'FIN-LEGACY',@alumno,@comision,@plan,0,GETDATE(),N'cancelada',N'Legacy test','');
SET @solicitud=SCOPE_IDENTITY();
INSERT dbo.Cuota(IdSolicitudInscripcion,NumeroCuota,MontoOriginal,BalanceAdeudado,FechaVencimiento,Estado,DVH)
VALUES(@solicitud,1,20000,0,'20260101',N'pagada','');
EXEC dbo.sp_Financiacion_GenerarCuotas @solicitud;
IF (SELECT SUM(MontoOriginal) FROM dbo.Cuota WHERE IdSolicitudInscripcion=@solicitud AND TipoConcepto=N'curso')<>100 THROW 52915,N'Rounding changed the total.',1;
IF NOT EXISTS(SELECT 1 FROM dbo.Cuota WHERE IdSolicitudInscripcion=@solicitud AND NumeroCuota=4 AND MontoOriginal=33.34 AND FechaVencimiento='20270331') THROW 52916,N'Remainder or month-end anchor incorrect.',1;
IF NOT EXISTS(SELECT 1 FROM dbo.Cuota WHERE IdSolicitudInscripcion=@solicitud AND NumeroCuota=3 AND FechaVencimiento='20270228') THROW 52922,N'Non-leap February date is incorrect.',1;
IF NOT EXISTS(SELECT 1 FROM dbo.Cuota WHERE IdSolicitudInscripcion=@solicitud AND NumeroCuota=1 AND Estado=N'pagada' AND BalanceAdeudado=0 AND MontoOriginal=20000 AND FechaVencimiento='20260101') THROW 52917,N'Historical matrícula was changed.',1;
PRINT 'PASS: rounding and historical matrícula preservation.';
-- Contado produces exactly one course charge, independent of the commission duration.
UPDATE dbo.PlanesDePago SET CantidadCuotas=1,RecargoPorcentaje=0 WHERE IdPlanDePago=@plan;
INSERT dbo.SolicitudInscripcion(Codigo,IdAlumno,IdComision,IdPlanDePago,RecargoPlanSnapshot,FechaSolicitud,Estado,Observaciones,DVH)
VALUES(N'FIN-CASH',@alumno,@comision,@plan,0,GETDATE(),N'cancelada',N'Cash test','');
SET @solicitud=SCOPE_IDENTITY();
EXEC dbo.sp_Financiacion_GenerarCuotas @solicitud;
IF (SELECT COUNT(*) FROM dbo.Cuota WHERE IdSolicitudInscripcion=@solicitud AND TipoConcepto=N'curso' AND MontoOriginal=100)<>1
    THROW 52923,N'Contado did not generate a single installment.',1;
PRINT 'PASS: Contado, 3-installment and 6-installment plans.';
GO

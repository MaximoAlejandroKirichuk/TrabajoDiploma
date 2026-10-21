-- Isolated regression fixture: a paid matrícula that predates course financing snapshots.
IF DB_NAME() NOT LIKE N'FinanceTests[_]%' THROW 52900,N'Use the isolated financing test runner.',1;
DECLARE @plan INT,@curso INT,@profesor INT,@comision INT,@alumno INT,@solicitud INT,@cuota INT;
INSERT dbo.PlanesDePago(Nombre,RecargoPorcentaje,EstadoActivo,CantidadCuotas) VALUES(N'Legacy 6 cuotas',10,1,6);
SET @plan=SCOPE_IDENTITY();
INSERT dbo.Curso(Nombre,Descripcion,CargaHoraria,EstadoActivo,DVH) VALUES(N'Legacy course',N'Test',12,1,'');
SET @curso=SCOPE_IDENTITY();
INSERT dbo.Profesor(DNI,Nombre,Apellido,Email,EstadoActivo,DVH) VALUES(N'99776655',N'Legacy',N'Teacher',N'legacy.teacher@example.com',1,'');
SET @profesor=SCOPE_IDENTITY();
INSERT dbo.Comision(Codigo,IdCurso,IdProfesor,DiaSemana,HoraInicio,HoraFin,CupoMinimo,CupoMaximo,FechaLimitePago,FechaInicio,FechaFin,ArancelBase,MontoMatricula,IdPlanDePago,RecargoPlanSnapshot,Estado,DVH)
VALUES(N'FIN-LEG-COM',@curso,@profesor,N'Monday','18:00','20:00',1,30,DATEADD(DAY,10,GETDATE()),'20270131','20270731',120000,20000,@plan,10,N'preapertura','');
SET @comision=SCOPE_IDENTITY();
INSERT dbo.Alumno(DNI,Nombre,Apellido,Email,Telefono,FechaAlta,Estado,DVH) VALUES(N'99776611',N'Legacy',N'Student',N'legacy.student@example.com',N'123456',GETDATE(),N'activo','');
SET @alumno=SCOPE_IDENTITY();
INSERT dbo.SolicitudInscripcion(Codigo,IdAlumno,IdComision,IdPlanDePago,RecargoPlanSnapshot,FechaSolicitud,Estado,Observaciones,DVH)
VALUES(N'FIN-LEG-SOL',@alumno,@comision,@plan,10,GETDATE(),N'Al día',N'Legacy fixture','');
SET @solicitud=SCOPE_IDENTITY();
INSERT dbo.Cuota(IdSolicitudInscripcion,NumeroCuota,MontoOriginal,BalanceAdeudado,FechaVencimiento,Estado,DVH)
VALUES(@solicitud,1,20000,0,'20260101',N'pagada','');
SET @cuota=SCOPE_IDENTITY();
INSERT dbo.PagoInscripcion(IdAlumno,IdSolicitudInscripcion,IdCuota,MetodoPago,MontoPagado,NumeroReferencia,FechaPago,FechaAcreditacion,NumeroReciboInstitucional,DVH)
VALUES(@alumno,@solicitud,@cuota,N'transferencia',20000,N'LEGACY-PAYMENT','20260101','20260101',N'LEGACY-RECEIPT',N'legacy-payment-hash');
GO

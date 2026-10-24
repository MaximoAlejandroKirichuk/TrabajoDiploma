SET NOCOUNT ON;
-- Ejecutar todos los lotes en una misma conexión y detenerse ante el primer error (sqlcmd -b).
SET XACT_ABORT ON;
BEGIN TRANSACTION;
GO

IF OBJECT_ID(N'dbo.CUN07_Migraciones',N'U') IS NULL
    CREATE TABLE dbo.CUN07_Migraciones(Version INT NOT NULL PRIMARY KEY, FechaAplicacion DATETIME NOT NULL);
GO

IF OBJECT_ID(N'dbo.PagoInscripcion', N'U') IS NULL
    THROW 52700, N'CUN07 requiere dbo.PagoInscripcion; aplicar EsquemaCompleto/CUN05 primero.', 1;
GO

IF OBJECT_ID(N'dbo.SolicitudInscripcion', N'U') IS NOT NULL AND OBJECT_ID(N'dbo.Cuota', N'U') IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_SolicitudInscripcion_AlumnoComision' AND object_id = OBJECT_ID(N'dbo.SolicitudInscripcion'))
        CREATE INDEX IX_SolicitudInscripcion_AlumnoComision ON dbo.SolicitudInscripcion (IdAlumno, IdComision) INCLUDE (Estado, IdSolicitudInscripcion);
    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Cuota_SolicitudEstadoBalance' AND object_id = OBJECT_ID(N'dbo.Cuota'))
        CREATE INDEX IX_Cuota_SolicitudEstadoBalance ON dbo.Cuota (IdSolicitudInscripcion, Estado, BalanceAdeudado);
END
GO

IF COL_LENGTH(N'dbo.PagoInscripcion', N'FechaAcreditacion') IS NULL
    ALTER TABLE dbo.PagoInscripcion ADD FechaAcreditacion DATETIME NULL;
IF COL_LENGTH(N'dbo.PagoInscripcion', N'NumeroReciboInstitucional') IS NULL
    ALTER TABLE dbo.PagoInscripcion ADD NumeroReciboInstitucional NVARCHAR(40) NULL;
GO

CREATE TABLE #Cun07ChangedPayments(IdPagoInscripcion INT NOT NULL);
DECLARE @legacyBase BIGINT, @missing BIGINT;
-- Serializar la asignación con las operaciones de escritura; nunca reutilizar un número histórico ya asignado.
SELECT @legacyBase=COALESCE(MAX(TRY_CONVERT(BIGINT,SUBSTRING(NumeroReciboInstitucional,11,12))),0)
FROM dbo.PagoInscripcion WITH(TABLOCKX,HOLDLOCK)
WHERE NumeroReciboInstitucional LIKE N'CUN07-LEG-%' AND LEN(NumeroReciboInstitucional)=22
  AND SUBSTRING(NumeroReciboInstitucional,11,12) COLLATE Latin1_General_100_BIN2 NOT LIKE N'%[^0-9]%';
SELECT @missing=COUNT_BIG(*) FROM dbo.PagoInscripcion WHERE NumeroReciboInstitucional IS NULL;
IF @legacyBase+@missing>999999999999
    THROW 52710,N'CUN07: espacio de recibos historicos agotado; no se modificaron los recibos.',1;
;WITH faltantes AS (
    SELECT IdPagoInscripcion, ROW_NUMBER() OVER(ORDER BY IdPagoInscripcion) AS rn
    FROM dbo.PagoInscripcion WHERE NumeroReciboInstitucional IS NULL
)
UPDATE p SET FechaAcreditacion=COALESCE(p.FechaAcreditacion,p.FechaPago),
    NumeroReciboInstitucional=CONCAT(N'CUN07-LEG-',RIGHT(REPLICATE(N'0',12)+CONVERT(NVARCHAR(20),@legacyBase+f.rn),12))
OUTPUT inserted.IdPagoInscripcion INTO #Cun07ChangedPayments
FROM dbo.PagoInscripcion p JOIN faltantes f ON f.IdPagoInscripcion=p.IdPagoInscripcion;
UPDATE dbo.PagoInscripcion SET FechaAcreditacion=COALESCE(FechaAcreditacion,FechaPago)
OUTPUT inserted.IdPagoInscripcion INTO #Cun07ChangedPayments
WHERE FechaAcreditacion IS NULL AND FechaPago IS NOT NULL;
IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.PagoInscripcion') AND name=N'UX_PagoInscripcion_ReciboInstitucional')
    CREATE UNIQUE INDEX UX_PagoInscripcion_ReciboInstitucional ON dbo.PagoInscripcion(NumeroReciboInstitucional) WHERE NumeroReciboInstitucional IS NOT NULL;
GO

IF OBJECT_ID(N'dbo.FacturaCUN07', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.FacturaCUN07(
        IdFactura INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_FacturaCUN07 PRIMARY KEY,
        IdPagoInscripcion INT NOT NULL CONSTRAINT UQ_FacturaCUN07_Pago UNIQUE,
        NumeroReciboInstitucional NVARCHAR(40) NOT NULL CONSTRAINT UQ_FacturaCUN07_Recibo UNIQUE,
        Concepto NVARCHAR(200) NOT NULL,
        Total DECIMAL(18,2) NOT NULL,
        FechaTransaccion DATETIME NOT NULL,
        DVH VARCHAR(64) NOT NULL CONSTRAINT DF_FacturaCUN07_DVH DEFAULT(''),
        CONSTRAINT FK_FacturaCUN07_Pago FOREIGN KEY(IdPagoInscripcion) REFERENCES dbo.PagoInscripcion(IdPagoInscripcion)
    );
END
GO

IF OBJECT_ID(N'dbo.SeqReciboInstitucionalCUN07', N'SO') IS NULL
    EXEC(N'CREATE SEQUENCE dbo.SeqReciboInstitucionalCUN07 AS BIGINT START WITH 1 INCREMENT BY 1 NO CYCLE');
GO

IF OBJECT_ID(N'dbo.DigitoVerificador_83KI', N'U') IS NOT NULL AND NOT EXISTS(SELECT 1 FROM dbo.DigitoVerificador_83KI WHERE NombreTabla=N'FacturaCUN07')
    INSERT dbo.DigitoVerificador_83KI(NombreTabla,DVV,FechaActualizacion) VALUES(N'FacturaCUN07','',GETDATE());
GO

-- Los nuevos datos de entrada para el hash de pagos requieren recalcular todos los hashes de pagos históricos antes del DVV.
DECLARE @firstApplication BIT=CASE WHEN NOT EXISTS(SELECT 1 FROM dbo.CUN07_Migraciones WHERE Version=1) THEN 1 ELSE 0 END;
IF @firstApplication=1 OR EXISTS(SELECT 1 FROM #Cun07ChangedPayments)
BEGIN
UPDATE p SET DVH=LOWER(CONVERT(VARCHAR(64),HASHBYTES('SHA2_256',CONVERT(NVARCHAR(MAX),
    N'FechaAcreditacion='+REPLACE(CONVERT(NVARCHAR(23),p.FechaAcreditacion,121),N' ',N'T')+N'|FechaPago='+REPLACE(CONVERT(NVARCHAR(23),p.FechaPago,121),N' ',N'T')+
    N'|IdAlumno='+CONVERT(NVARCHAR(20),p.IdAlumno)+N'|IdCuota='+CONVERT(NVARCHAR(20),p.IdCuota)+
    N'|IdPagoInscripcion='+CONVERT(NVARCHAR(20),p.IdPagoInscripcion)+N'|IdSolicitudInscripcion='+CONVERT(NVARCHAR(20),p.IdSolicitudInscripcion)+
    N'|MetodoPago='+LTRIM(RTRIM(p.MetodoPago))+N'|MontoPagado='+CONVERT(NVARCHAR(40),p.MontoPagado)+
    N'|NumeroReciboInstitucional='+LTRIM(RTRIM(p.NumeroReciboInstitucional))+N'|NumeroReferencia='+LTRIM(RTRIM(p.NumeroReferencia)))),2))
FROM dbo.PagoInscripcion p
WHERE @firstApplication=1 OR EXISTS(SELECT 1 FROM #Cun07ChangedPayments c WHERE c.IdPagoInscripcion=p.IdPagoInscripcion);
IF @firstApplication=1
UPDATE f SET DVH=LOWER(CONVERT(VARCHAR(64),HASHBYTES('SHA2_256',CONVERT(NVARCHAR(MAX),
    N'Concepto='+LTRIM(RTRIM(f.Concepto))+N'|FechaTransaccion='+REPLACE(CONVERT(NVARCHAR(23),f.FechaTransaccion,121),N' ',N'T')+
    N'|IdFactura='+CONVERT(NVARCHAR(20),f.IdFactura)+N'|IdPagoInscripcion='+CONVERT(NVARCHAR(20),f.IdPagoInscripcion)+
    N'|NumeroReciboInstitucional='+LTRIM(RTRIM(f.NumeroReciboInstitucional))+N'|Total='+CONVERT(NVARCHAR(40),f.Total))),2))
FROM dbo.FacturaCUN07 f;
DECLARE @paymentHashes NVARCHAR(MAX),@invoiceHashes NVARCHAR(MAX);
SELECT @paymentHashes=COALESCE((SELECT CONVERT(NVARCHAR(MAX),DVH) AS [text()] FROM dbo.PagoInscripcion ORDER BY DVH FOR XML PATH(''),TYPE).value('.','NVARCHAR(MAX)'),N'');
SELECT @invoiceHashes=COALESCE((SELECT CONVERT(NVARCHAR(MAX),DVH) AS [text()] FROM dbo.FacturaCUN07 ORDER BY DVH FOR XML PATH(''),TYPE).value('.','NVARCHAR(MAX)'),N'');
UPDATE dbo.DigitoVerificador_83KI SET DVV=LOWER(CONVERT(VARCHAR(64),HASHBYTES('SHA2_256',@paymentHashes),2)),FechaActualizacion=GETDATE() WHERE NombreTabla=N'PagoInscripcion';
IF @firstApplication=1
BEGIN
    UPDATE dbo.DigitoVerificador_83KI SET DVV=LOWER(CONVERT(VARCHAR(64),HASHBYTES('SHA2_256',@invoiceHashes),2)),FechaActualizacion=GETDATE() WHERE NombreTabla=N'FacturaCUN07';
    INSERT dbo.CUN07_Migraciones(Version,FechaAplicacion) VALUES(1,GETDATE());
END
END
DROP TABLE #Cun07ChangedPayments;
GO

IF OBJECT_ID(N'dbo.sp_CUN03_RegistrarSolicitudInscripcion', N'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_CUN03_RegistrarSolicitudInscripcion;
GO
CREATE PROCEDURE dbo.sp_CUN03_RegistrarSolicitudInscripcion
    @DNI VARCHAR(20), @Nombre NVARCHAR(80), @Apellido NVARCHAR(80), @Email NVARCHAR(200), @Telefono NVARCHAR(50),
    @IdLead INT = NULL, @IdAlumno INT = NULL, @IdComision INT, @IdPlanDePago INT, @Observaciones NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @StartedTransaction BIT = 0, @IdSolicitud INT, @IdCuota INT, @Codigo NVARCHAR(30),
            @MontoMatricula DECIMAL(18,2), @Recargo DECIMAL(9,4), @VacantesDisponibles INT;
    IF @@TRANCOUNT = 0 BEGIN SET @StartedTransaction = 1; BEGIN TRANSACTION; END
    BEGIN TRY
        SELECT @MontoMatricula = c.MontoMatricula FROM dbo.Comision c WITH (UPDLOCK, HOLDLOCK)
        WHERE c.IdComision = @IdComision AND c.Estado <> N'eliminada'
          AND c.FechaLimitePago >= CONVERT(date, GETDATE()) AND c.MontoMatricula > 0;
        IF @MontoMatricula IS NULL THROW 52001, N'Errores.ComisionElegibleNoDisponible', 1;
        SELECT @Recargo = pp.RecargoPorcentaje FROM dbo.Comision c
        INNER JOIN dbo.PlanesDePago pp WITH(HOLDLOCK) ON pp.IdPlanDePago = c.IdPlanDePago
        WHERE c.IdComision = @IdComision AND pp.IdPlanDePago = @IdPlanDePago AND pp.EstadoActivo = 1;
        IF @Recargo IS NULL THROW 52002, N'Errores.PlanDePagoNoDisponible', 1;
        IF @IdAlumno IS NULL SELECT @IdAlumno = IdAlumno FROM dbo.Alumno WITH (UPDLOCK, HOLDLOCK) WHERE DNI = @DNI;
        IF @IdAlumno IS NULL
        BEGIN
            INSERT dbo.Alumno (DNI, Nombre, Apellido, Email, Telefono, FechaAlta, Estado, DVH)
            VALUES (@DNI, @Nombre, @Apellido, @Email, @Telefono, GETDATE(), N'activo', N'');
            SET @IdAlumno = SCOPE_IDENTITY();
        END
        IF EXISTS (SELECT 1 FROM dbo.SolicitudInscripcion WITH (UPDLOCK, HOLDLOCK)
                   WHERE IdAlumno = @IdAlumno AND IdComision = @IdComision
                     AND Estado IN (N'pendiente', N'confirmada', N'activa'))
            THROW 52003, N'Errores.InscripcionDuplicada', 1;
        IF EXISTS (
            SELECT 1 FROM dbo.SolicitudInscripcion si WITH (UPDLOCK, HOLDLOCK, INDEX(IX_SolicitudInscripcion_AlumnoComision))
            INNER JOIN dbo.Cuota cu WITH (UPDLOCK, HOLDLOCK, INDEX(IX_Cuota_SolicitudEstadoBalance))
                ON cu.IdSolicitudInscripcion = si.IdSolicitudInscripcion
            WHERE si.IdAlumno = @IdAlumno AND si.IdComision = @IdComision
              AND cu.Estado = N'pendiente' AND cu.BalanceAdeudado > 0
        ) THROW 52005, N'Errores.InscripcionPendienteDeuda', 1;
        SELECT @VacantesDisponibles = c.CupoMaximo - COUNT(si.IdSolicitudInscripcion)
        FROM dbo.Comision c WITH (UPDLOCK, HOLDLOCK)
        LEFT JOIN dbo.SolicitudInscripcion si ON si.IdComision = c.IdComision
            AND si.Estado IN (N'pendiente', N'confirmada', N'activa')
        WHERE c.IdComision = @IdComision GROUP BY c.CupoMaximo;
        IF ISNULL(@VacantesDisponibles, 0) <= 0 THROW 52004, N'Errores.SinVacantesComision', 1;
        INSERT dbo.SolicitudInscripcion
            (Codigo, IdAlumno, IdLeadOrigen, IdComision, IdPlanDePago, RecargoPlanSnapshot, FechaSolicitud, Estado, Observaciones, DVH)
        VALUES (N'', @IdAlumno, @IdLead, @IdComision, @IdPlanDePago, @Recargo, GETDATE(), N'pendiente', ISNULL(@Observaciones, N''), N'');
        SET @IdSolicitud = SCOPE_IDENTITY();
        SET @Codigo = CONCAT(N'SI-', RIGHT(CONCAT(N'000000', @IdSolicitud), 6));
        UPDATE dbo.SolicitudInscripcion SET Codigo = @Codigo WHERE IdSolicitudInscripcion = @IdSolicitud;
        INSERT dbo.Cuota (IdSolicitudInscripcion, NumeroCuota, MontoOriginal, BalanceAdeudado, FechaVencimiento, Estado, DVH)
        VALUES (@IdSolicitud, 1, @MontoMatricula, @MontoMatricula, DATEADD(day, 7, CONVERT(date, GETDATE())), N'pendiente', N'');
        SET @IdCuota = SCOPE_IDENTITY();
        EXEC dbo.sp_Financiacion_GenerarCuotas @IdSolicitud;
        SET @VacantesDisponibles = @VacantesDisponibles - 1;
        IF @StartedTransaction = 1 COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @StartedTransaction = 1 AND XACT_STATE() <> 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
    SELECT a.IdAlumno, a.DNI AS AlumnoDNI, a.Nombre AS AlumnoNombre, a.Apellido AS AlumnoApellido,
           a.Email AS AlumnoEmail, a.Telefono AS AlumnoTelefono, a.FechaAlta AS AlumnoFechaAlta, a.Estado AS AlumnoEstado, a.DVH AS AlumnoDVH,
           si.IdSolicitudInscripcion, si.Codigo, si.IdLeadOrigen, si.IdComision, si.IdPlanDePago, si.RecargoPlanSnapshot,
           si.FechaSolicitud, si.Estado AS SolicitudEstado, si.Observaciones, si.DVH AS SolicitudDVH,
           si.ArancelCursoSnapshot,si.CantidadCuotasSnapshot,si.TotalCursoSnapshot,si.FechaPrimerVencimientoSnapshot,si.NombrePlanSnapshot,
           cu.IdCuota, cu.NumeroCuota, cu.MontoOriginal, cu.BalanceAdeudado, cu.FechaVencimiento, cu.Estado AS CuotaEstado,
           cu.DVH AS CuotaDVH, @VacantesDisponibles AS VacantesDisponibles
    FROM dbo.SolicitudInscripcion si
    INNER JOIN dbo.Alumno a ON a.IdAlumno = si.IdAlumno
    INNER JOIN dbo.Cuota cu ON cu.IdSolicitudInscripcion = si.IdSolicitudInscripcion AND cu.TipoConcepto = N'matricula'
    WHERE si.IdSolicitudInscripcion = @IdSolicitud;
END
GO

IF OBJECT_ID(N'dbo.sp_CUN07_BuscarCuotasPendientes', N'P') IS NOT NULL DROP PROCEDURE dbo.sp_CUN07_BuscarCuotasPendientes;
GO
IF OBJECT_ID(N'dbo.sp_CUN07_ListarComisionesPago', N'P') IS NOT NULL DROP PROCEDURE dbo.sp_CUN07_ListarComisionesPago;
GO
CREATE PROCEDURE dbo.sp_CUN07_ListarComisionesPago @DNI VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT DISTINCT c.Codigo CodigoComision, c.Codigo + N' - ' + cur.Nombre Descripcion
    FROM dbo.Alumno a
    JOIN dbo.SolicitudInscripcion si ON si.IdAlumno=a.IdAlumno
    JOIN dbo.Comision c ON c.IdComision=si.IdComision
    JOIN dbo.Curso cur ON cur.IdCurso=c.IdCurso
    WHERE a.DNI=LTRIM(RTRIM(@DNI))
      AND si.Estado IN(N'pendiente',N'confirmada',N'activa',N'Al día',N'Con deuda')
      AND EXISTS(SELECT 1 FROM dbo.Cuota cu WHERE cu.IdSolicitudInscripcion=si.IdSolicitudInscripcion AND cu.TipoConcepto=N'curso')
    ORDER BY CodigoComision;
END
GO
CREATE PROCEDURE dbo.sp_CUN07_BuscarCuotasPendientes @DNI VARCHAR(20), @CodigoComision NVARCHAR(30)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT a.IdAlumno,a.DNI AlumnoDNI,a.Nombre AlumnoNombre,a.Apellido AlumnoApellido,a.Email AlumnoEmail,a.Telefono AlumnoTelefono,a.FechaAlta AlumnoFechaAlta,a.Estado AlumnoEstado,a.DVH AlumnoDVH,
      si.IdSolicitudInscripcion,si.Codigo CodigoSolicitud,si.IdLeadOrigen,si.IdComision,si.IdPlanDePago,si.RecargoPlanSnapshot,si.FechaSolicitud FechaSolicitudInscripcion,si.Estado SolicitudEstado,si.Observaciones SolicitudObservaciones,si.DVH SolicitudDVH,
      si.ArancelCursoSnapshot,si.CantidadCuotasSnapshot,si.TotalCursoSnapshot,si.FechaPrimerVencimientoSnapshot,si.NombrePlanSnapshot,
      CONVERT(bit,CASE WHEN EXISTS(SELECT 1 FROM dbo.Cuota m WHERE m.IdSolicitudInscripcion=si.IdSolicitudInscripcion AND m.TipoConcepto=N'matricula' AND m.Estado=N'pagada') THEN 1 ELSE 0 END) MatriculaPagada,
      cu.IdCuota,cu.NumeroCuota,cu.TipoConcepto,cu.Periodo,cu.MontoOriginal,cu.BalanceAdeudado,cu.FechaVencimiento,cu.Estado CuotaEstado,cu.DVH CuotaDVH,c.Codigo CodigoComision,cur.Nombre Curso,si.NombrePlanSnapshot NombrePlanDePago
    FROM dbo.Alumno a JOIN dbo.SolicitudInscripcion si ON si.IdAlumno=a.IdAlumno JOIN dbo.Comision c ON c.IdComision=si.IdComision JOIN dbo.Curso cur ON cur.IdCurso=c.IdCurso
      JOIN dbo.Cuota cu ON cu.IdSolicitudInscripcion=si.IdSolicitudInscripcion
      JOIN dbo.PlanesDePago pp ON pp.IdPlanDePago=si.IdPlanDePago
    WHERE a.DNI=LTRIM(RTRIM(@DNI)) AND c.Codigo=LTRIM(RTRIM(@CodigoComision)) AND cu.TipoConcepto=N'curso'
      AND si.Estado IN(N'pendiente',N'confirmada',N'activa',N'Al día',N'Con deuda') ORDER BY cu.NumeroCuota,cu.FechaVencimiento;
END
GO

IF OBJECT_ID(N'dbo.sp_CUN07_RegistrarPago', N'P') IS NOT NULL DROP PROCEDURE dbo.sp_CUN07_RegistrarPago;
GO
CREATE PROCEDURE dbo.sp_CUN07_RegistrarPago
 @IdAlumno INT,@IdSolicitudInscripcion INT,@IdCuota INT,@CodigoComision NVARCHAR(30),@MetodoPago NVARCHAR(30),@MontoRecibido DECIMAL(18,2),
 @NumeroReferencia NVARCHAR(100),@FechaPago DATETIME,@FechaAcreditacion DATETIME
AS
BEGIN
 SET NOCOUNT ON;
  DECLARE @balance DECIMAL(18,2),@numeroCuota INT,@codigo NVARCHAR(40),@idPago INT,@concepto NVARCHAR(200);
 IF @IdAlumno<=0 OR @IdSolicitudInscripcion<=0 OR @IdCuota<=0 OR @MontoRecibido<=0 OR NULLIF(LTRIM(RTRIM(@NumeroReferencia)),N'') IS NULL OR @FechaPago IS NULL OR @FechaAcreditacion IS NULL
   THROW 52701,N'Errores.PagoDatosInvalidos',1;
 BEGIN TRY
 BEGIN TRANSACTION;
 SELECT @balance=cu.BalanceAdeudado,@numeroCuota=cu.NumeroCuota FROM dbo.Cuota cu WITH(UPDLOCK,HOLDLOCK)
   JOIN dbo.SolicitudInscripcion si WITH(UPDLOCK,HOLDLOCK) ON si.IdSolicitudInscripcion=cu.IdSolicitudInscripcion
   JOIN dbo.Comision c WITH(UPDLOCK,HOLDLOCK) ON c.IdComision=si.IdComision
   WHERE cu.IdCuota=@IdCuota AND cu.IdSolicitudInscripcion=@IdSolicitudInscripcion AND si.IdAlumno=@IdAlumno AND c.Codigo=@CodigoComision
      AND cu.TipoConcepto=N'curso' AND cu.Estado=N'pendiente' AND cu.BalanceAdeudado>0 AND si.Estado IN(N'activa',N'Al día',N'Con deuda');
  IF @balance IS NULL THROW 52702,N'Errores.PagoCuotaNoPendiente',1;
  IF NOT EXISTS(SELECT 1 FROM dbo.Cuota WITH(UPDLOCK,HOLDLOCK)
                WHERE IdSolicitudInscripcion=@IdSolicitudInscripcion AND TipoConcepto=N'matricula' AND Estado=N'pagada')
    THROW 52709,N'Errores.PagoMatriculaPendiente',1;
  IF EXISTS(SELECT 1 FROM dbo.Cuota WITH(UPDLOCK,HOLDLOCK)
            WHERE IdSolicitudInscripcion=@IdSolicitudInscripcion AND TipoConcepto=N'curso' AND NumeroCuota<@numeroCuota AND BalanceAdeudado>0)
    THROW 52708,N'Errores.PagoCuotaAnteriorPendiente',1;
  IF @MontoRecibido<>@balance THROW 52703,N'Errores.PagoMontoNoCoincide',1;
   IF CONVERT(date,@FechaPago)>CONVERT(date,GETDATE()) OR CONVERT(date,@FechaAcreditacion)>CONVERT(date,GETDATE())
     THROW 52707,N'Errores.PagoFechaFutura',1;
   IF CONVERT(date,@FechaAcreditacion)<CONVERT(date,@FechaPago) THROW 52706,N'Errores.PagoFechaAcreditacionAnterior',1;
 IF @MetodoPago NOT IN(N'transferencia',N'tarjeta',N'billetera virtual') THROW 52704,N'Errores.PagoMetodoInvalido',1;
 IF EXISTS(SELECT 1 FROM dbo.PagoInscripcion WITH(UPDLOCK,HOLDLOCK) WHERE NumeroReferencia=@NumeroReferencia) THROW 52705,N'Errores.PagoReferenciaDuplicada',1;
 SET @codigo=N'CUN07-' + RIGHT(REPLICATE(N'0',12)+CONVERT(NVARCHAR(20),NEXT VALUE FOR dbo.SeqReciboInstitucionalCUN07),12);
  SET @concepto=N'Pago cuota del curso ' + CONVERT(NVARCHAR(12),@numeroCuota-1);
 INSERT dbo.PagoInscripcion(IdAlumno,IdSolicitudInscripcion,IdCuota,MetodoPago,MontoPagado,NumeroReferencia,FechaPago,FechaAcreditacion,NumeroReciboInstitucional,DVH)
 VALUES(@IdAlumno,@IdSolicitudInscripcion,@IdCuota,@MetodoPago,@MontoRecibido,@NumeroReferencia,@FechaPago,@FechaAcreditacion,@codigo,'');
 SET @idPago=SCOPE_IDENTITY();
 UPDATE dbo.Cuota SET BalanceAdeudado=0,Estado=N'pagada',DVH='' WHERE IdCuota=@IdCuota;
  -- La situación financiera es independiente del estado académico de inscripción utilizado para el cupo mínimo y la cancelación.
 INSERT dbo.FacturaCUN07(IdPagoInscripcion,NumeroReciboInstitucional,Concepto,Total,FechaTransaccion,DVH) VALUES(@idPago,@codigo,@concepto,@MontoRecibido,@FechaPago,'');
 COMMIT TRANSACTION;
  SELECT p.IdPagoInscripcion,p.IdAlumno,p.IdSolicitudInscripcion,p.IdCuota,p.MetodoPago,p.MontoPagado,p.NumeroReferencia,p.FechaPago,p.FechaAcreditacion,p.DVH PagoDVH,
 a.DNI AlumnoDNI,a.Nombre AlumnoNombre,a.Apellido AlumnoApellido,a.Email AlumnoEmail,a.Telefono AlumnoTelefono,a.FechaAlta AlumnoFechaAlta,a.Estado AlumnoEstado,a.DVH AlumnoDVH,
  si.Codigo CodigoSolicitud,si.IdLeadOrigen,si.IdComision,si.IdPlanDePago,si.RecargoPlanSnapshot,si.FechaSolicitud FechaSolicitudInscripcion,si.Estado SolicitudEstado,si.Observaciones SolicitudObservaciones,si.DVH SolicitudDVH,
  si.ArancelCursoSnapshot,si.CantidadCuotasSnapshot,si.TotalCursoSnapshot,si.FechaPrimerVencimientoSnapshot,si.NombrePlanSnapshot,
  cu.NumeroCuota,cu.TipoConcepto,cu.Periodo,cu.MontoOriginal,cu.BalanceAdeudado,cu.FechaVencimiento,cu.Estado CuotaEstado,cu.DVH CuotaDVH,c.Codigo CodigoComision,cur.Nombre Curso,
 @codigo NumeroReciboInstitucional,@concepto ConceptoFactura,@MontoRecibido TotalFactura,@FechaPago FechaTransaccion,(SELECT IdFactura FROM dbo.FacturaCUN07 WHERE IdPagoInscripcion=@idPago) IdFactura
 FROM dbo.PagoInscripcion p JOIN dbo.Alumno a ON a.IdAlumno=p.IdAlumno JOIN dbo.SolicitudInscripcion si ON si.IdSolicitudInscripcion=p.IdSolicitudInscripcion
 JOIN dbo.Cuota cu ON cu.IdCuota=p.IdCuota JOIN dbo.Comision c ON c.IdComision=si.IdComision JOIN dbo.Curso cur ON cur.IdCurso=c.IdCurso WHERE p.IdPagoInscripcion=@idPago;
 END TRY
 BEGIN CATCH
 IF XACT_STATE()<>0 ROLLBACK TRANSACTION;
 THROW;
 END CATCH
END
GO
COMMIT TRANSACTION;
GO

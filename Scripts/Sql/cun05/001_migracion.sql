SET NOCOUNT ON;
GO

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

IF OBJECT_ID(N'dbo.Cuota', N'U') IS NULL
BEGIN
    RAISERROR(N'dbo.Cuota no existe en esta base: use EsquemaCompleto.sql para una base nueva.', 16, 1);
    RETURN;
END
GO

IF OBJECT_ID(N'dbo.PagoInscripcion', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.PagoInscripcion(
        IdPagoInscripcion INT IDENTITY(1,1) NOT NULL,
        IdAlumno INT NOT NULL,
        IdSolicitudInscripcion INT NOT NULL,
        IdCuota INT NOT NULL,
        MetodoPago NVARCHAR(30) NOT NULL,
        MontoPagado DECIMAL(18,2) NOT NULL,
        NumeroReferencia NVARCHAR(100) NOT NULL,
        FechaPago DATETIME NOT NULL,
        DVH VARCHAR(64) NOT NULL CONSTRAINT DF_PagoInscripcion_DVH DEFAULT (''),
        CONSTRAINT PK_PagoInscripcion PRIMARY KEY CLUSTERED (IdPagoInscripcion),
        CONSTRAINT FK_PagoInscripcion_Alumno FOREIGN KEY (IdAlumno) REFERENCES dbo.Alumno(IdAlumno),
        CONSTRAINT FK_PagoInscripcion_SolicitudInscripcion FOREIGN KEY (IdSolicitudInscripcion) REFERENCES dbo.SolicitudInscripcion(IdSolicitudInscripcion),
        CONSTRAINT FK_PagoInscripcion_Cuota FOREIGN KEY (IdCuota) REFERENCES dbo.Cuota(IdCuota),
        CONSTRAINT CK_PagoInscripcion_MetodoPago CHECK (MetodoPago IN (N'transferencia', N'tarjeta', N'billetera virtual'))
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_PagoInscripcion_Referencia' AND object_id = OBJECT_ID(N'dbo.PagoInscripcion'))
BEGIN
    SET QUOTED_IDENTIFIER ON;
    CREATE UNIQUE INDEX UX_PagoInscripcion_Referencia ON dbo.PagoInscripcion (NumeroReferencia);
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.DigitoVerificador_83KI WHERE NombreTabla = N'PagoInscripcion')
    INSERT INTO dbo.DigitoVerificador_83KI (NombreTabla, DVV, FechaActualizacion) VALUES (N'PagoInscripcion', '', GETDATE());
GO

UPDATE p
SET Nombre = N'Registrar pago de inscripción'
FROM dbo.Patentes p
WHERE p.CodigoPatente = 66
  AND p.Nombre <> N'Registrar pago de inscripción';

SET IDENTITY_INSERT dbo.Patentes ON;
INSERT INTO dbo.Patentes (CodigoPatente, Nombre, DVH)
SELECT 66, N'Registrar pago de inscripción', ''
WHERE NOT EXISTS (SELECT 1 FROM dbo.Patentes p WHERE p.CodigoPatente = 66);
SET IDENTITY_INSERT dbo.Patentes OFF;

DECLARE @RolesCun05 TABLE (CodigoRol INT NOT NULL PRIMARY KEY);

INSERT INTO @RolesCun05 (CodigoRol)
SELECT 1
WHERE EXISTS (SELECT 1 FROM dbo.Roles WHERE CodigoRol = 1)
UNION
SELECT r.CodigoRol
FROM dbo.Roles r
WHERE r.Nombre COLLATE Latin1_General_CI_AI IN (N'Coordinador Academico', N'Coordinadora Academica', N'Coordinadora Academia')
   OR r.Nombre COLLATE Latin1_General_CI_AI LIKE N'Coordinador%Academic%'
   OR r.Nombre COLLATE Latin1_General_CI_AI LIKE N'Coordinadora%Academic%';

INSERT INTO dbo.RolPatente (CodigoRol, CodigoPatente, DVH)
SELECT r.CodigoRol, 66, ''
FROM @RolesCun05 r
WHERE NOT EXISTS (SELECT 1 FROM dbo.RolPatente rp WHERE rp.CodigoRol = r.CodigoRol AND rp.CodigoPatente = 66);
GO

IF OBJECT_ID(N'dbo.sp_CUN05_BuscarCuotasPendientesPorDni', N'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_CUN05_BuscarCuotasPendientesPorDni;
GO
CREATE PROCEDURE dbo.sp_CUN05_BuscarCuotasPendientesPorDni
    @DNI VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @DniNormalizado VARCHAR(20) = LTRIM(RTRIM(ISNULL(@DNI, N'')));

    SELECT
        a.IdAlumno,
        a.DNI AS AlumnoDNI,
        a.Nombre AS AlumnoNombre,
        a.Apellido AS AlumnoApellido,
        a.Email AS AlumnoEmail,
        a.Telefono AS AlumnoTelefono,
        a.FechaAlta AS AlumnoFechaAlta,
        a.Estado AS AlumnoEstado,
        a.DVH AS AlumnoDVH,
        si.IdSolicitudInscripcion,
        si.Codigo AS CodigoSolicitud,
        si.IdLeadOrigen,
        si.IdComision,
        si.IdPlanDePago,
        si.RecargoPlanSnapshot,
        si.FechaSolicitud AS FechaSolicitudInscripcion,
        si.Estado AS SolicitudEstado,
        si.Observaciones AS SolicitudObservaciones,
        si.DVH AS SolicitudDVH,
        cu.IdCuota,
        cu.NumeroCuota,
        cu.MontoOriginal,
        cu.BalanceAdeudado,
        cu.FechaVencimiento,
        cu.Estado AS CuotaEstado,
        cu.DVH AS CuotaDVH,
        c.Codigo AS CodigoComision,
        cur.Nombre AS Curso
    FROM dbo.Alumno a
    INNER JOIN dbo.SolicitudInscripcion si ON si.IdAlumno = a.IdAlumno
    INNER JOIN dbo.Cuota cu ON cu.IdSolicitudInscripcion = si.IdSolicitudInscripcion
    INNER JOIN dbo.Comision c ON c.IdComision = si.IdComision
    INNER JOIN dbo.Curso cur ON cur.IdCurso = c.IdCurso
    WHERE a.DNI = @DniNormalizado
      AND cu.Estado = N'pendiente'
      AND si.Estado IN (N'pendiente', N'confirmada', N'activa')
    ORDER BY cu.NumeroCuota, cu.FechaVencimiento;
END
GO

IF OBJECT_ID(N'dbo.sp_CUN05_RegistrarPagoInscripcion', N'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_CUN05_RegistrarPagoInscripcion;
GO
CREATE PROCEDURE dbo.sp_CUN05_RegistrarPagoInscripcion
    @IdAlumno INT,
    @IdSolicitudInscripcion INT,
    @IdCuota INT,
    @MetodoPago NVARCHAR(30),
    @MontoRecibido DECIMAL(18,2),
    @NumeroReferencia NVARCHAR(100),
    @FechaPago DATETIME
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @StartedTransaction BIT = 0, @IdPago INT;
    DECLARE @NumeroCuota INT, @BalanceAdeudado DECIMAL(18,2);

    IF @@TRANCOUNT = 0 BEGIN SET @StartedTransaction = 1; BEGIN TRANSACTION; END

    BEGIN TRY
        SELECT @BalanceAdeudado = BalanceAdeudado, @NumeroCuota = NumeroCuota
        FROM dbo.Cuota WITH (UPDLOCK, HOLDLOCK)
        WHERE IdCuota = @IdCuota AND IdSolicitudInscripcion = @IdSolicitudInscripcion;

        IF @BalanceAdeudado IS NULL
            THROW 52601, N'Errores.PagoCuotaNoPendiente', 1;

        IF @BalanceAdeudado = 0 OR EXISTS (SELECT 1 FROM dbo.Cuota WITH (UPDLOCK, HOLDLOCK) WHERE IdCuota = @IdCuota AND Estado <> N'pendiente')
            THROW 52601, N'Errores.PagoCuotaNoPendiente', 1;

        IF @MontoRecibido <> @BalanceAdeudado
            THROW 52602, N'Errores.PagoMontoNoCoincide', 1;

        IF @MetodoPago NOT IN (N'transferencia', N'tarjeta', N'billetera virtual')
            THROW 52603, N'Errores.PagoMetodoInvalido', 1;

        IF EXISTS (
            SELECT 1 FROM dbo.PagoInscripcion WITH (UPDLOCK, HOLDLOCK)
            WHERE NumeroReferencia = @NumeroReferencia
        )
            THROW 52604, N'Errores.PagoReferenciaDuplicada', 1;

        IF NOT EXISTS (
            SELECT 1 FROM dbo.SolicitudInscripcion WITH (UPDLOCK, HOLDLOCK)
            WHERE IdSolicitudInscripcion = @IdSolicitudInscripcion
              AND IdAlumno = @IdAlumno
              AND Estado IN (N'pendiente', N'confirmada', N'activa')
        )
            THROW 52605, N'Errores.PagoSolicitudNoActivatable', 1;

        INSERT INTO dbo.PagoInscripcion (IdAlumno, IdSolicitudInscripcion, IdCuota, MetodoPago, MontoPagado, NumeroReferencia, FechaPago, DVH)
        VALUES (@IdAlumno, @IdSolicitudInscripcion, @IdCuota, @MetodoPago, @MontoRecibido, @NumeroReferencia, @FechaPago, N'');

        SET @IdPago = SCOPE_IDENTITY();

        UPDATE dbo.Cuota
        SET BalanceAdeudado = 0, Estado = N'pagada', DVH = N''
        WHERE IdCuota = @IdCuota;

        IF @NumeroCuota = 1
        BEGIN
            UPDATE dbo.SolicitudInscripcion
            SET Estado = N'activa', DVH = N''
            WHERE IdSolicitudInscripcion = @IdSolicitudInscripcion
              AND Estado IN (N'pendiente', N'confirmada');
        END

        IF @StartedTransaction = 1 COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @StartedTransaction = 1 AND XACT_STATE() <> 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH;

    SELECT
        p.IdPagoInscripcion,
        p.IdAlumno,
        p.IdSolicitudInscripcion,
        p.IdCuota,
        p.MetodoPago,
        p.MontoPagado,
        p.NumeroReferencia,
        p.FechaPago,
        p.DVH AS PagoDVH,
        a.DNI AS AlumnoDNI,
        a.Nombre AS AlumnoNombre,
        a.Apellido AS AlumnoApellido,
        a.Email AS AlumnoEmail,
        a.Telefono AS AlumnoTelefono,
        a.FechaAlta AS AlumnoFechaAlta,
        a.Estado AS AlumnoEstado,
        a.DVH AS AlumnoDVH,
        si.Codigo AS CodigoSolicitud,
        si.IdLeadOrigen,
        si.IdComision,
        si.IdPlanDePago,
        si.RecargoPlanSnapshot,
        si.FechaSolicitud AS FechaSolicitudInscripcion,
        si.Estado AS SolicitudEstado,
        si.Observaciones AS SolicitudObservaciones,
        si.DVH AS SolicitudDVH,
        cu.NumeroCuota,
        cu.MontoOriginal,
        cu.BalanceAdeudado,
        cu.FechaVencimiento,
        cu.Estado AS CuotaEstado,
        cu.DVH AS CuotaDVH,
        c.Codigo AS CodigoComision,
        cur.Nombre AS Curso
    FROM dbo.PagoInscripcion p
    INNER JOIN dbo.Alumno a ON a.IdAlumno = p.IdAlumno
    INNER JOIN dbo.SolicitudInscripcion si ON si.IdSolicitudInscripcion = p.IdSolicitudInscripcion
    INNER JOIN dbo.Cuota cu ON cu.IdCuota = p.IdCuota
    INNER JOIN dbo.Comision c ON c.IdComision = si.IdComision
    INNER JOIN dbo.Curso cur ON cur.IdCurso = c.IdCurso
    WHERE p.IdPagoInscripcion = @IdPago;
END
GO

PRINT 'CUN05 migration applied. Recalculate integrity from the application after running it.';
GO

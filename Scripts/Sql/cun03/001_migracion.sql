SET NOCOUNT ON;
GO

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

IF OBJECT_ID(N'dbo.Comision', N'U') IS NULL
BEGIN
    RAISERROR(N'dbo.Comision no existe en esta base: use EsquemaCompleto.sql para una base nueva.', 16, 1);
    RETURN;
END
GO

IF COL_LENGTH('dbo.Comision', 'MontoMatricula') IS NULL
BEGIN
    ALTER TABLE dbo.Comision ADD MontoMatricula DECIMAL(18,2) NULL;
END
GO

UPDATE dbo.Comision
SET MontoMatricula = CASE WHEN ISNULL(MontoMatricula, 0) <= 0 THEN ISNULL(NULLIF(ArancelBase, 0), 0.01) ELSE MontoMatricula END
WHERE MontoMatricula IS NULL OR MontoMatricula <= 0;
GO

IF COL_LENGTH('dbo.Comision', 'MontoMatricula') IS NOT NULL
BEGIN
    ALTER TABLE dbo.Comision ALTER COLUMN MontoMatricula DECIMAL(18,2) NOT NULL;
END
GO

IF OBJECT_ID(N'dbo.CK_Comision_MontoMatricula', N'C') IS NULL
BEGIN
    ALTER TABLE dbo.Comision ADD CONSTRAINT CK_Comision_MontoMatricula CHECK (MontoMatricula > 0);
END
GO

IF OBJECT_ID(N'dbo.Alumno', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Alumno(
        IdAlumno INT IDENTITY(1,1) NOT NULL,
        DNI VARCHAR(20) NOT NULL,
        Nombre NVARCHAR(80) NOT NULL,
        Apellido NVARCHAR(80) NOT NULL,
        Email NVARCHAR(200) NOT NULL,
        Telefono NVARCHAR(50) NOT NULL,
        FechaAlta DATETIME NOT NULL CONSTRAINT DF_Alumno_FechaAlta DEFAULT (GETDATE()),
        Estado NVARCHAR(30) NOT NULL CONSTRAINT DF_Alumno_Estado DEFAULT (N'activo'),
        DVH VARCHAR(64) NOT NULL CONSTRAINT DF_Alumno_DVH DEFAULT (''),
        CONSTRAINT PK_Alumno PRIMARY KEY CLUSTERED (IdAlumno),
        CONSTRAINT UQ_Alumno_DNI UNIQUE (DNI)
    );
END
GO

IF OBJECT_ID(N'dbo.SolicitudInscripcion', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SolicitudInscripcion(
        IdSolicitudInscripcion INT IDENTITY(1,1) NOT NULL,
        Codigo NVARCHAR(30) NOT NULL,
        IdAlumno INT NOT NULL,
        IdLeadOrigen INT NULL,
        IdComision INT NOT NULL,
        IdPlanDePago INT NOT NULL,
        RecargoPlanSnapshot DECIMAL(9,4) NOT NULL CONSTRAINT DF_SolicitudInscripcion_Recargo DEFAULT (0),
        FechaSolicitud DATETIME NOT NULL CONSTRAINT DF_SolicitudInscripcion_Fecha DEFAULT (GETDATE()),
        Estado NVARCHAR(30) NOT NULL CONSTRAINT DF_SolicitudInscripcion_Estado DEFAULT (N'pendiente'),
        Observaciones NVARCHAR(500) NOT NULL CONSTRAINT DF_SolicitudInscripcion_Observaciones DEFAULT (N''),
        DVH VARCHAR(64) NOT NULL CONSTRAINT DF_SolicitudInscripcion_DVH DEFAULT (''),
        CONSTRAINT PK_SolicitudInscripcion PRIMARY KEY CLUSTERED (IdSolicitudInscripcion),
        CONSTRAINT UQ_SolicitudInscripcion_Codigo UNIQUE (Codigo),
        CONSTRAINT FK_SolicitudInscripcion_Alumno FOREIGN KEY (IdAlumno) REFERENCES dbo.Alumno(IdAlumno),
        CONSTRAINT FK_SolicitudInscripcion_Lead FOREIGN KEY (IdLeadOrigen) REFERENCES dbo.Lead(IdLead),
        CONSTRAINT FK_SolicitudInscripcion_Comision FOREIGN KEY (IdComision) REFERENCES dbo.Comision(IdComision),
        CONSTRAINT FK_SolicitudInscripcion_Plan FOREIGN KEY (IdPlanDePago) REFERENCES dbo.PlanesDePago(IdPlanDePago)
    );
END
GO

IF OBJECT_ID(N'dbo.Cuota', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Cuota(
        IdCuota INT IDENTITY(1,1) NOT NULL,
        IdSolicitudInscripcion INT NOT NULL,
        NumeroCuota INT NOT NULL,
        MontoOriginal DECIMAL(18,2) NOT NULL,
        BalanceAdeudado DECIMAL(18,2) NOT NULL,
        FechaVencimiento DATE NOT NULL,
        Estado NVARCHAR(30) NOT NULL CONSTRAINT DF_Cuota_Estado DEFAULT (N'pendiente'),
        DVH VARCHAR(64) NOT NULL CONSTRAINT DF_Cuota_DVH DEFAULT (''),
        CONSTRAINT PK_Cuota PRIMARY KEY CLUSTERED (IdCuota),
        CONSTRAINT FK_Cuota_SolicitudInscripcion FOREIGN KEY (IdSolicitudInscripcion) REFERENCES dbo.SolicitudInscripcion(IdSolicitudInscripcion),
        CONSTRAINT CK_Cuota_Numero CHECK (NumeroCuota > 0),
        CONSTRAINT CK_Cuota_Montos CHECK (MontoOriginal > 0 AND BalanceAdeudado >= 0)
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_SolicitudInscripcion_AlumnoComision_Activa' AND object_id = OBJECT_ID(N'dbo.SolicitudInscripcion'))
BEGIN
    SET QUOTED_IDENTIFIER ON;
    CREATE UNIQUE INDEX UX_SolicitudInscripcion_AlumnoComision_Activa
        ON dbo.SolicitudInscripcion (IdAlumno, IdComision)
        WHERE Estado IN (N'pendiente', N'confirmada', N'activa');
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_SolicitudInscripcion_ComisionEstado' AND object_id = OBJECT_ID(N'dbo.SolicitudInscripcion'))
    CREATE INDEX IX_SolicitudInscripcion_ComisionEstado ON dbo.SolicitudInscripcion (IdComision, Estado);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Cuota_Solicitud' AND object_id = OBJECT_ID(N'dbo.Cuota'))
    CREATE INDEX IX_Cuota_Solicitud ON dbo.Cuota (IdSolicitudInscripcion);
GO

IF NOT EXISTS (SELECT 1 FROM dbo.DigitoVerificador_83KI WHERE NombreTabla = N'Alumno')
    INSERT INTO dbo.DigitoVerificador_83KI (NombreTabla, DVV, FechaActualizacion) VALUES (N'Alumno', '', GETDATE());
IF NOT EXISTS (SELECT 1 FROM dbo.DigitoVerificador_83KI WHERE NombreTabla = N'SolicitudInscripcion')
    INSERT INTO dbo.DigitoVerificador_83KI (NombreTabla, DVV, FechaActualizacion) VALUES (N'SolicitudInscripcion', '', GETDATE());
IF NOT EXISTS (SELECT 1 FROM dbo.DigitoVerificador_83KI WHERE NombreTabla = N'Cuota')
    INSERT INTO dbo.DigitoVerificador_83KI (NombreTabla, DVV, FechaActualizacion) VALUES (N'Cuota', '', GETDATE());
GO

DECLARE @PermisoCun03 TABLE (CodigoPatente INT NOT NULL PRIMARY KEY, Nombre NVARCHAR(100) NOT NULL);
INSERT INTO @PermisoCun03 (CodigoPatente, Nombre) VALUES (64, N'Registrar solicitud de inscripción');

UPDATE p
SET Nombre = pc.Nombre
FROM dbo.Patentes p
INNER JOIN @PermisoCun03 pc ON pc.CodigoPatente = p.CodigoPatente
WHERE p.Nombre <> pc.Nombre;

SET IDENTITY_INSERT dbo.Patentes ON;
INSERT INTO dbo.Patentes (CodigoPatente, Nombre, DVH)
SELECT CodigoPatente, Nombre, '' FROM @PermisoCun03 pc
WHERE NOT EXISTS (SELECT 1 FROM dbo.Patentes p WHERE p.CodigoPatente = pc.CodigoPatente);
SET IDENTITY_INSERT dbo.Patentes OFF;

INSERT INTO dbo.RolPatente (CodigoRol, CodigoPatente, DVH)
SELECT 1, CodigoPatente, '' FROM @PermisoCun03 pc
WHERE NOT EXISTS (SELECT 1 FROM dbo.RolPatente rp WHERE rp.CodigoRol = 1 AND rp.CodigoPatente = pc.CodigoPatente);
GO

IF OBJECT_ID(N'dbo.sp_CUN03_BuscarAlumnoLead', N'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_CUN03_BuscarAlumnoLead;
GO
CREATE PROCEDURE dbo.sp_CUN03_BuscarAlumnoLead
    @Texto NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Like NVARCHAR(110) = N'%' + LTRIM(RTRIM(ISNULL(@Texto, N''))) + N'%';

    SELECT N'Alumno' AS Tipo, a.IdAlumno AS IdPersona, a.DNI, a.Nombre, a.Apellido, a.Email, a.Telefono, a.FechaAlta, a.Estado, a.DVH,
           CAST(NULL AS INT) AS IdComisionSugerida
    FROM dbo.Alumno a
    WHERE a.DNI LIKE @Like OR a.Nombre LIKE @Like OR a.Apellido LIKE @Like
    UNION ALL
    SELECT N'Lead' AS Tipo, l.IdLead AS IdPersona, l.DNI, l.Nombre, l.Apellido, l.Email, l.Telefono, l.FechaAlta, l.Estado, l.DVH,
           cl.IdComision AS IdComisionSugerida
    FROM dbo.Lead l
    OUTER APPLY (
        SELECT TOP 1 IdComision
        FROM dbo.ConsultaLead c
        WHERE c.IdLead = l.IdLead AND c.Estado <> N'eliminada'
        ORDER BY c.FechaConsulta DESC, c.IdConsultaLead DESC
    ) cl
    WHERE l.DNI LIKE @Like OR l.Nombre LIKE @Like OR l.Apellido LIKE @Like
    ORDER BY Tipo, Apellido, Nombre;
END
GO

IF OBJECT_ID(N'dbo.sp_CUN03_ListarComisionesElegibles', N'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_CUN03_ListarComisionesElegibles;
GO
CREATE PROCEDURE dbo.sp_CUN03_ListarComisionesElegibles
AS
BEGIN
    SET NOCOUNT ON;
    SELECT c.IdComision, c.Codigo, c.IdCurso, cu.Nombre AS Curso, c.IdProfesor,
           CONCAT(p.Apellido, N', ', p.Nombre) AS Profesor, c.DiaSemana, c.HoraInicio, c.HoraFin,
           c.CupoMinimo, c.CupoMaximo, c.FechaLimitePago, c.FechaInicio, c.FechaFin,
           c.ArancelBase, c.MontoMatricula, c.Estado,
           c.CupoMaximo - ISNULL(s.Ocupadas, 0) AS VacantesDisponibles
    FROM dbo.Comision c
    INNER JOIN dbo.Curso cu ON cu.IdCurso = c.IdCurso
    INNER JOIN dbo.Profesor p ON p.IdProfesor = c.IdProfesor
    OUTER APPLY (
        SELECT COUNT(1) AS Ocupadas
        FROM dbo.SolicitudInscripcion si
        WHERE si.IdComision = c.IdComision AND si.Estado IN (N'pendiente', N'confirmada', N'activa')
    ) s
    WHERE c.Estado <> N'eliminada'
      AND c.FechaLimitePago >= CONVERT(date, GETDATE())
      AND c.MontoMatricula > 0
      AND EXISTS (SELECT 1 FROM dbo.PlanesDePago pp WHERE pp.IdPlanDePago = c.IdPlanDePago AND pp.EstadoActivo = 1)
      AND c.CupoMaximo - ISNULL(s.Ocupadas, 0) > 0
    ORDER BY c.FechaInicio, c.Codigo;
END
GO

IF OBJECT_ID(N'dbo.sp_CUN03_ObtenerPlanesComision', N'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_CUN03_ObtenerPlanesComision;
GO
CREATE PROCEDURE dbo.sp_CUN03_ObtenerPlanesComision
    @IdComision INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT pp.IdPlanDePago, pp.Nombre, pp.EstadoActivo, pp.RecargoPorcentaje
    FROM dbo.Comision c
    INNER JOIN dbo.PlanesDePago pp ON pp.IdPlanDePago = c.IdPlanDePago
    WHERE c.IdComision = @IdComision AND pp.EstadoActivo = 1;
END
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
        SELECT @MontoMatricula = c.MontoMatricula
        FROM dbo.Comision c WITH (UPDLOCK, HOLDLOCK)
        WHERE c.IdComision = @IdComision
          AND c.Estado <> N'eliminada'
          AND c.FechaLimitePago >= CONVERT(date, GETDATE())
          AND c.MontoMatricula > 0;

        IF @MontoMatricula IS NULL THROW 52001, N'Errores.ComisionElegibleNoDisponible', 1;

        SELECT @Recargo = pp.RecargoPorcentaje
        FROM dbo.Comision c
        INNER JOIN dbo.PlanesDePago pp ON pp.IdPlanDePago = c.IdPlanDePago
        WHERE c.IdComision = @IdComision AND pp.IdPlanDePago = @IdPlanDePago AND pp.EstadoActivo = 1;

        IF @Recargo IS NULL THROW 52002, N'Errores.PlanDePagoNoDisponible', 1;

        IF @IdAlumno IS NULL
            SELECT @IdAlumno = IdAlumno FROM dbo.Alumno WITH (UPDLOCK, HOLDLOCK) WHERE DNI = @DNI;

        IF @IdAlumno IS NULL
        BEGIN
            INSERT INTO dbo.Alumno (DNI, Nombre, Apellido, Email, Telefono, FechaAlta, Estado, DVH)
            VALUES (@DNI, @Nombre, @Apellido, @Email, @Telefono, GETDATE(), N'activo', N'');
            SET @IdAlumno = SCOPE_IDENTITY();
        END

        IF EXISTS (SELECT 1 FROM dbo.SolicitudInscripcion WITH (UPDLOCK, HOLDLOCK) WHERE IdAlumno = @IdAlumno AND IdComision = @IdComision AND Estado IN (N'pendiente', N'confirmada', N'activa'))
            THROW 52003, N'Errores.InscripcionDuplicada', 1;

        SELECT @VacantesDisponibles = c.CupoMaximo - COUNT(si.IdSolicitudInscripcion)
        FROM dbo.Comision c WITH (UPDLOCK, HOLDLOCK)
        LEFT JOIN dbo.SolicitudInscripcion si ON si.IdComision = c.IdComision AND si.Estado IN (N'pendiente', N'confirmada', N'activa')
        WHERE c.IdComision = @IdComision
        GROUP BY c.CupoMaximo;

        IF ISNULL(@VacantesDisponibles, 0) <= 0 THROW 52004, N'Errores.SinVacantesComision', 1;

        INSERT INTO dbo.SolicitudInscripcion (Codigo, IdAlumno, IdLeadOrigen, IdComision, IdPlanDePago, RecargoPlanSnapshot, FechaSolicitud, Estado, Observaciones, DVH)
        VALUES (N'', @IdAlumno, @IdLead, @IdComision, @IdPlanDePago, @Recargo, GETDATE(), N'pendiente', ISNULL(@Observaciones, N''), N'');

        SET @IdSolicitud = SCOPE_IDENTITY();
        SET @Codigo = CONCAT(N'SI-', RIGHT(CONCAT(N'000000', @IdSolicitud), 6));
        UPDATE dbo.SolicitudInscripcion SET Codigo = @Codigo WHERE IdSolicitudInscripcion = @IdSolicitud;

        INSERT INTO dbo.Cuota (IdSolicitudInscripcion, NumeroCuota, MontoOriginal, BalanceAdeudado, FechaVencimiento, Estado, DVH)
        VALUES (@IdSolicitud, 1, @MontoMatricula, @MontoMatricula, DATEADD(day, 7, CONVERT(date, GETDATE())), N'pendiente', N'');
        SET @IdCuota = SCOPE_IDENTITY();

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
           cu.IdCuota, cu.NumeroCuota, cu.MontoOriginal, cu.BalanceAdeudado, cu.FechaVencimiento, cu.Estado AS CuotaEstado, cu.DVH AS CuotaDVH,
           @VacantesDisponibles AS VacantesDisponibles
    FROM dbo.SolicitudInscripcion si
    INNER JOIN dbo.Alumno a ON a.IdAlumno = si.IdAlumno
    INNER JOIN dbo.Cuota cu ON cu.IdSolicitudInscripcion = si.IdSolicitudInscripcion AND cu.NumeroCuota = 1
    WHERE si.IdSolicitudInscripcion = @IdSolicitud;
END
GO

IF OBJECT_ID(N'[dbo].[sp_CUN01_RegistrarPreaperturaComision]', N'P') IS NOT NULL
    DROP PROCEDURE [dbo].[sp_CUN01_RegistrarPreaperturaComision];
GO
CREATE PROCEDURE [dbo].[sp_CUN01_RegistrarPreaperturaComision]
    @IdCurso INT, @IdProfesor INT, @DiaSemana NVARCHAR(20), @HoraInicio TIME, @HoraFin TIME,
    @CupoMinimo INT, @CupoMaximo INT, @FechaLimitePago DATE, @FechaInicio DATE, @FechaFin DATE,
    @ArancelBase DECIMAL(18,2), @MontoMatricula DECIMAL(18,2), @IdPlanDePago INT, @RecargoPlanSnapshot DECIMAL(9,4)
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @StartedTransaction BIT = 0, @IdComision INT, @Codigo NVARCHAR(30);
    IF @@TRANCOUNT = 0 BEGIN SET @StartedTransaction = 1; BEGIN TRANSACTION; END
    BEGIN TRY
        IF EXISTS (SELECT 1 FROM dbo.Comision WITH (UPDLOCK, HOLDLOCK) WHERE IdProfesor = @IdProfesor AND DiaSemana = @DiaSemana AND Estado <> N'eliminada' AND HoraInicio < @HoraFin AND HoraFin > @HoraInicio)
            THROW 51001, N'Errores.ProfesorSolapamientoComision', 1;

        INSERT INTO dbo.Comision (Codigo, IdCurso, IdProfesor, DiaSemana, HoraInicio, HoraFin, CupoMinimo, CupoMaximo, FechaLimitePago, FechaInicio, FechaFin, ArancelBase, MontoMatricula, IdPlanDePago, RecargoPlanSnapshot, Estado, DVH)
        VALUES (N'', @IdCurso, @IdProfesor, @DiaSemana, @HoraInicio, @HoraFin, @CupoMinimo, @CupoMaximo, @FechaLimitePago, @FechaInicio, @FechaFin, @ArancelBase, @MontoMatricula, @IdPlanDePago, @RecargoPlanSnapshot, N'preapertura', N'');
        SET @IdComision = SCOPE_IDENTITY();
        SET @Codigo = CONCAT(N'COM-', RIGHT(CONCAT(N'000000', @IdComision), 6));
        UPDATE dbo.Comision SET Codigo = @Codigo WHERE IdComision = @IdComision;
        IF @StartedTransaction = 1 COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @StartedTransaction = 1 AND XACT_STATE() <> 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
    SELECT IdComision, Codigo, IdCurso, IdProfesor, DiaSemana, HoraInicio, HoraFin, CupoMinimo, CupoMaximo, FechaLimitePago, FechaInicio, FechaFin, ArancelBase, MontoMatricula, IdPlanDePago, RecargoPlanSnapshot, Estado, DVH
    FROM dbo.Comision WHERE IdComision = @IdComision;
END
GO

IF OBJECT_ID(N'[dbo].[sp_CUN01_ObtenerComision]', N'P') IS NOT NULL
    DROP PROCEDURE [dbo].[sp_CUN01_ObtenerComision];
GO
CREATE PROCEDURE [dbo].[sp_CUN01_ObtenerComision]
    @IdComision INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT IdComision, Codigo, IdCurso, IdProfesor, DiaSemana, HoraInicio, HoraFin, CupoMinimo, CupoMaximo, FechaLimitePago, FechaInicio, FechaFin, ArancelBase, MontoMatricula, IdPlanDePago, RecargoPlanSnapshot, Estado, DVH
    FROM dbo.Comision WHERE IdComision = @IdComision;
END
GO

IF OBJECT_ID(N'[dbo].[sp_CUN01_ListarComisiones]', N'P') IS NOT NULL
    DROP PROCEDURE [dbo].[sp_CUN01_ListarComisiones];
GO
CREATE PROCEDURE [dbo].[sp_CUN01_ListarComisiones]
    @IdCurso INT = NULL, @IdProfesor INT = NULL, @Estado NVARCHAR(30) = NULL, @FechaInicioDesde DATE = NULL, @FechaFinHasta DATE = NULL, @Codigo NVARCHAR(30) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT c.IdComision, c.Codigo, c.IdCurso, cu.Nombre AS Curso, c.IdProfesor,
           CONCAT(p.Apellido, N', ', p.Nombre) AS Profesor, c.DiaSemana, c.HoraInicio, c.HoraFin,
           c.CupoMinimo, c.CupoMaximo, c.FechaLimitePago, c.FechaInicio, c.FechaFin, c.ArancelBase, c.MontoMatricula, c.Estado
    FROM dbo.Comision c
    INNER JOIN dbo.Curso cu ON cu.IdCurso = c.IdCurso
    INNER JOIN dbo.Profesor p ON p.IdProfesor = c.IdProfesor
    WHERE (@IdCurso IS NULL OR c.IdCurso = @IdCurso) AND (@IdProfesor IS NULL OR c.IdProfesor = @IdProfesor)
      AND (@Estado IS NULL OR c.Estado = @Estado) AND (@FechaInicioDesde IS NULL OR c.FechaInicio >= @FechaInicioDesde)
      AND (@FechaFinHasta IS NULL OR c.FechaFin <= @FechaFinHasta) AND (@Codigo IS NULL OR c.Codigo LIKE N'%' + @Codigo + N'%')
    ORDER BY c.FechaInicio DESC, c.Codigo;
END
GO

PRINT 'CUN03 migration applied. Recalculate integrity from the application after running it.';
GO

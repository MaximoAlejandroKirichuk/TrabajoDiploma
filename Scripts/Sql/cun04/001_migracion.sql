SET NOCOUNT ON;
GO

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

IF OBJECT_ID(N'dbo.SolicitudInscripcion', N'U') IS NULL
BEGIN
    RAISERROR(N'dbo.SolicitudInscripcion no existe en esta base: use EsquemaCompleto.sql para una base nueva.', 16, 1);
    RETURN;
END
GO

IF OBJECT_ID(N'dbo.Beca', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Beca(
        IdBeca INT IDENTITY(1,1) NOT NULL,
        IdAlumno INT NOT NULL,
        IdComision INT NOT NULL,
        IdSolicitudInscripcion INT NOT NULL,
        TipoBeneficio NVARCHAR(100) NOT NULL,
        FechaSolicitud DATETIME NOT NULL,
        EstadoBeca NVARCHAR(30) NOT NULL,
        MotivoDecision NVARCHAR(500) NOT NULL,
        DVH VARCHAR(64) NOT NULL CONSTRAINT DF_Beca_DVH DEFAULT (''),
        CONSTRAINT PK_Beca PRIMARY KEY CLUSTERED (IdBeca),
        CONSTRAINT FK_Beca_Alumno FOREIGN KEY (IdAlumno) REFERENCES dbo.Alumno(IdAlumno),
        CONSTRAINT FK_Beca_Comision FOREIGN KEY (IdComision) REFERENCES dbo.Comision(IdComision),
        CONSTRAINT FK_Beca_SolicitudInscripcion FOREIGN KEY (IdSolicitudInscripcion) REFERENCES dbo.SolicitudInscripcion(IdSolicitudInscripcion),
        CONSTRAINT CK_Beca_EstadoBeca CHECK (EstadoBeca IN (N'Aprobada', N'Denegada'))
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_Beca_SolicitudComision' AND object_id = OBJECT_ID(N'dbo.Beca'))
BEGIN
    SET QUOTED_IDENTIFIER ON;
    CREATE UNIQUE INDEX UX_Beca_SolicitudComision ON dbo.Beca (IdSolicitudInscripcion, IdComision);
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.DigitoVerificador_83KI WHERE NombreTabla = N'Beca')
    INSERT INTO dbo.DigitoVerificador_83KI (NombreTabla, DVV, FechaActualizacion) VALUES (N'Beca', '', GETDATE());
GO

DECLARE @PermisoCun04 TABLE (CodigoPatente INT NOT NULL PRIMARY KEY, Nombre NVARCHAR(100) NOT NULL);
INSERT INTO @PermisoCun04 (CodigoPatente, Nombre) VALUES (65, N'Registrar decisión de beca');

UPDATE p
SET Nombre = pc.Nombre
FROM dbo.Patentes p
INNER JOIN @PermisoCun04 pc ON pc.CodigoPatente = p.CodigoPatente
WHERE p.Nombre <> pc.Nombre;

SET IDENTITY_INSERT dbo.Patentes ON;
INSERT INTO dbo.Patentes (CodigoPatente, Nombre, DVH)
SELECT CodigoPatente, Nombre, '' FROM @PermisoCun04 pc
WHERE NOT EXISTS (SELECT 1 FROM dbo.Patentes p WHERE p.CodigoPatente = pc.CodigoPatente);
SET IDENTITY_INSERT dbo.Patentes OFF;

DECLARE @RolesCun04 TABLE (CodigoRol INT NOT NULL PRIMARY KEY);

INSERT INTO @RolesCun04 (CodigoRol)
SELECT 1
WHERE EXISTS (SELECT 1 FROM dbo.Roles WHERE CodigoRol = 1)
UNION
SELECT r.CodigoRol
FROM dbo.Roles r
WHERE r.Nombre COLLATE Latin1_General_CI_AI IN (N'Coordinador Academico', N'Coordinadora Academica', N'Coordinadora Academia')
   OR r.Nombre COLLATE Latin1_General_CI_AI LIKE N'Coordinador%Academic%'
   OR r.Nombre COLLATE Latin1_General_CI_AI LIKE N'Coordinadora%Academic%';

INSERT INTO dbo.RolPatente (CodigoRol, CodigoPatente, DVH)
SELECT r.CodigoRol, pc.CodigoPatente, ''
FROM @RolesCun04 r
CROSS JOIN @PermisoCun04 pc
WHERE NOT EXISTS (SELECT 1 FROM dbo.RolPatente rp WHERE rp.CodigoRol = r.CodigoRol AND rp.CodigoPatente = pc.CodigoPatente);
GO

IF OBJECT_ID(N'dbo.sp_CUN04_BuscarSolicitudPorDniComision', N'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_CUN04_BuscarSolicitudPorDniComision;
GO
CREATE PROCEDURE dbo.sp_CUN04_BuscarSolicitudPorDniComision
    @DNI VARCHAR(20),
    @CodigoComision NVARCHAR(30)
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @DniNormalizado VARCHAR(20) = LTRIM(RTRIM(ISNULL(@DNI, N'')));
    DECLARE @CodigoNormalizado NVARCHAR(30) = LTRIM(RTRIM(ISNULL(@CodigoComision, N'')));

    SELECT TOP 1
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
        c.Codigo AS CodigoComision,
        cu.Nombre AS Curso
    FROM dbo.Alumno a
    INNER JOIN dbo.SolicitudInscripcion si ON si.IdAlumno = a.IdAlumno
    INNER JOIN dbo.Comision c ON c.IdComision = si.IdComision
    INNER JOIN dbo.Curso cu ON cu.IdCurso = c.IdCurso
    WHERE a.DNI = @DniNormalizado
      AND c.Codigo = @CodigoNormalizado
      AND si.Estado IN (N'pendiente', N'confirmada', N'activa');
END
GO

IF OBJECT_ID(N'dbo.sp_CUN04_RegistrarDecisionBeca', N'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_CUN04_RegistrarDecisionBeca;
GO
CREATE PROCEDURE dbo.sp_CUN04_RegistrarDecisionBeca
    @IdAlumno INT,
    @IdComision INT,
    @IdSolicitudInscripcion INT,
    @TipoBeneficio NVARCHAR(100),
    @FechaSolicitud DATETIME,
    @EstadoBeca NVARCHAR(30),
    @MotivoDecision NVARCHAR(500)
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @StartedTransaction BIT = 0, @IdBeca INT;

    IF @@TRANCOUNT = 0 BEGIN SET @StartedTransaction = 1; BEGIN TRANSACTION; END

    BEGIN TRY
        IF NOT EXISTS (
            SELECT 1 FROM dbo.SolicitudInscripcion WITH (UPDLOCK, HOLDLOCK)
            WHERE IdSolicitudInscripcion = @IdSolicitudInscripcion
              AND IdAlumno = @IdAlumno
              AND IdComision = @IdComision
              AND Estado IN (N'pendiente', N'confirmada', N'activa')
        )
            THROW 52502, N'Errores.BecaSolicitudInscripcionInactiva', 1;

        IF EXISTS (
            SELECT 1 FROM dbo.Beca WITH (UPDLOCK, HOLDLOCK)
            WHERE IdSolicitudInscripcion = @IdSolicitudInscripcion AND IdComision = @IdComision
        )
            THROW 52501, N'Errores.BecaDuplicada', 1;

        INSERT INTO dbo.Beca (IdAlumno, IdComision, IdSolicitudInscripcion, TipoBeneficio, FechaSolicitud, EstadoBeca, MotivoDecision, DVH)
        VALUES (@IdAlumno, @IdComision, @IdSolicitudInscripcion, @TipoBeneficio, @FechaSolicitud, @EstadoBeca, @MotivoDecision, N'');

        SET @IdBeca = SCOPE_IDENTITY();

        IF @StartedTransaction = 1 COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @StartedTransaction = 1 AND XACT_STATE() <> 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH;

    SELECT b.IdBeca, b.IdAlumno, b.IdComision, b.IdSolicitudInscripcion, b.TipoBeneficio, b.FechaSolicitud, b.EstadoBeca, b.MotivoDecision, b.DVH
    FROM dbo.Beca b
    WHERE b.IdBeca = @IdBeca;
END
GO

PRINT 'CUN04 migration applied. Recalculate integrity from the application after running it.';
GO

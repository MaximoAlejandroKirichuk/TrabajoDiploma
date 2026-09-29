USE [InstitutoVisionEnAccion]
GO

IF OBJECT_ID(N'dbo.Comision', N'U') IS NULL
BEGIN
    RAISERROR(N'dbo.Comision no existe en esta base: use EsquemaCompleto.sql para una base nueva.', 16, 1);
    RETURN;
END
GO

IF COL_LENGTH('dbo.Comision', 'FechaInicio') IS NULL
BEGIN
    ALTER TABLE dbo.Comision ADD FechaInicio DATE NULL;
END
GO

IF COL_LENGTH('dbo.Comision', 'FechaFin') IS NULL
BEGIN
    ALTER TABLE dbo.Comision ADD FechaFin DATE NULL;
END
GO

UPDATE dbo.Comision
SET FechaInicio = ISNULL(FechaInicio, FechaLimitePago),
    FechaFin = ISNULL(FechaFin, DATEADD(month, 4, FechaLimitePago))
WHERE FechaInicio IS NULL OR FechaFin IS NULL;
GO

ALTER TABLE dbo.Comision ALTER COLUMN FechaInicio DATE NOT NULL;
GO
ALTER TABLE dbo.Comision ALTER COLUMN FechaFin DATE NOT NULL;
GO

IF OBJECT_ID(N'dbo.CK_Comision_Fechas', N'C') IS NULL
BEGIN
    ALTER TABLE dbo.Comision ADD CONSTRAINT CK_Comision_Fechas CHECK (FechaInicio <= FechaFin);
END
GO

DECLARE @PermisosComisiones TABLE (CodigoPatente INT NOT NULL PRIMARY KEY, Nombre NVARCHAR(100) NOT NULL);
INSERT INTO @PermisosComisiones (CodigoPatente, Nombre) VALUES
(61, N'Ver comisiones'), (62, N'Modificar comisión'), (63, N'Eliminar comisión');

IF EXISTS (
    SELECT 1
    FROM dbo.Patentes p
    INNER JOIN @PermisosComisiones pc ON pc.CodigoPatente = p.CodigoPatente
    WHERE p.Nombre <> pc.Nombre
)
BEGIN
    RAISERROR(N'No se pueden insertar permisos de comisiones: uno de los códigos 61-63 ya existe con otro nombre.', 16, 1);
    RETURN;
END

IF EXISTS (
    SELECT 1
    FROM dbo.Patentes p
    INNER JOIN @PermisosComisiones pc ON pc.Nombre = p.Nombre
    WHERE p.CodigoPatente <> pc.CodigoPatente
)
BEGIN
    RAISERROR(N'No se pueden insertar permisos de comisiones: existe un permiso de comisiones con otro código. Revise/repárelo manualmente antes de continuar.', 16, 1);
    RETURN;
END

SET IDENTITY_INSERT dbo.Patentes ON;

INSERT INTO dbo.Patentes (CodigoPatente, Nombre, DVH)
SELECT pc.CodigoPatente, pc.Nombre, ''
FROM @PermisosComisiones pc
WHERE NOT EXISTS (SELECT 1 FROM dbo.Patentes p WHERE p.CodigoPatente = pc.CodigoPatente);

SET IDENTITY_INSERT dbo.Patentes OFF;

INSERT INTO dbo.RolPatente (CodigoRol, CodigoPatente, DVH)
SELECT 1, pc.CodigoPatente, ''
FROM @PermisosComisiones pc
WHERE EXISTS (SELECT 1 FROM dbo.Patentes p WHERE p.CodigoPatente = pc.CodigoPatente AND p.Nombre = pc.Nombre)
  AND NOT EXISTS (SELECT 1 FROM dbo.RolPatente rp WHERE rp.CodigoRol = 1 AND rp.CodigoPatente = pc.CodigoPatente);
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_CUN01_ListarComisiones]
    @IdCurso INT = NULL,
    @IdProfesor INT = NULL,
    @Estado NVARCHAR(30) = NULL,
    @FechaInicioDesde DATE = NULL,
    @FechaFinHasta DATE = NULL,
    @Codigo NVARCHAR(30) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT c.IdComision, c.Codigo, c.IdCurso, cu.Nombre AS Curso, c.IdProfesor,
           CONCAT(p.Apellido, N', ', p.Nombre) AS Profesor, c.DiaSemana, c.HoraInicio, c.HoraFin,
           c.CupoMinimo, c.CupoMaximo, c.FechaLimitePago, c.FechaInicio, c.FechaFin,
           c.ArancelBase, c.Estado
    FROM dbo.Comision c
    INNER JOIN dbo.Curso cu ON cu.IdCurso = c.IdCurso
    INNER JOIN dbo.Profesor p ON p.IdProfesor = c.IdProfesor
    WHERE (@IdCurso IS NULL OR c.IdCurso = @IdCurso)
      AND (@IdProfesor IS NULL OR c.IdProfesor = @IdProfesor)
      AND (@Estado IS NULL OR c.Estado = @Estado)
      AND (@FechaInicioDesde IS NULL OR c.FechaInicio >= @FechaInicioDesde)
      AND (@FechaFinHasta IS NULL OR c.FechaFin <= @FechaFinHasta)
      AND (@Codigo IS NULL OR c.Codigo LIKE N'%' + @Codigo + N'%')
    ORDER BY c.FechaInicio DESC, c.Codigo;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_CUN01_RegistrarPreaperturaComision]
    @IdCurso INT,
    @IdProfesor INT,
    @DiaSemana NVARCHAR(20),
    @HoraInicio TIME,
    @HoraFin TIME,
    @CupoMinimo INT,
    @CupoMaximo INT,
    @FechaLimitePago DATE,
    @FechaInicio DATE,
    @FechaFin DATE,
    @ArancelBase DECIMAL(18,2),
    @IdPlanDePago INT,
    @RecargoPlanSnapshot DECIMAL(9,4)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @StartedTransaction BIT = 0;
    DECLARE @IdComision INT;
    DECLARE @Codigo NVARCHAR(30);

    IF @@TRANCOUNT = 0
    BEGIN
        SET @StartedTransaction = 1;
        BEGIN TRANSACTION;
    END

    BEGIN TRY
        IF EXISTS (
            SELECT 1
            FROM dbo.Comision WITH (UPDLOCK, HOLDLOCK)
            WHERE IdProfesor = @IdProfesor
              AND DiaSemana = @DiaSemana
              AND Estado <> N'eliminada'
              AND HoraInicio < @HoraFin
              AND HoraFin > @HoraInicio
        )
            THROW 51001, N'Errores.ProfesorSolapamientoComision', 1;

        INSERT INTO dbo.Comision (Codigo, IdCurso, IdProfesor, DiaSemana, HoraInicio, HoraFin, CupoMinimo, CupoMaximo, FechaLimitePago, FechaInicio, FechaFin, ArancelBase, IdPlanDePago, RecargoPlanSnapshot, Estado, DVH)
        VALUES (N'', @IdCurso, @IdProfesor, @DiaSemana, @HoraInicio, @HoraFin, @CupoMinimo, @CupoMaximo, @FechaLimitePago, @FechaInicio, @FechaFin, @ArancelBase, @IdPlanDePago, @RecargoPlanSnapshot, N'preapertura', N'');

        SET @IdComision = SCOPE_IDENTITY();
        SET @Codigo = CONCAT(N'COM-', RIGHT(CONCAT(N'000000', @IdComision), 6));

        UPDATE dbo.Comision SET Codigo = @Codigo WHERE IdComision = @IdComision;

        IF @StartedTransaction = 1
            COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @StartedTransaction = 1 AND XACT_STATE() <> 0
            ROLLBACK TRANSACTION;

        THROW;
    END CATCH;

    SELECT IdComision, Codigo, IdCurso, IdProfesor, DiaSemana, HoraInicio, HoraFin, CupoMinimo, CupoMaximo,
           FechaLimitePago, FechaInicio, FechaFin, ArancelBase, IdPlanDePago, RecargoPlanSnapshot, Estado, DVH
    FROM dbo.Comision
    WHERE IdComision = @IdComision;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_CUN01_ObtenerComision]
    @IdComision INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT IdComision, Codigo, IdCurso, IdProfesor, DiaSemana, HoraInicio, HoraFin, CupoMinimo, CupoMaximo,
           FechaLimitePago, FechaInicio, FechaFin, ArancelBase, IdPlanDePago, RecargoPlanSnapshot, Estado, DVH
    FROM dbo.Comision
    WHERE IdComision = @IdComision;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_CUN01_ModificarComision]
    @IdComision INT, @IdCurso INT, @IdProfesor INT, @DiaSemana NVARCHAR(20), @HoraInicio TIME, @HoraFin TIME,
    @CupoMinimo INT, @CupoMaximo INT, @FechaLimitePago DATE, @FechaInicio DATE, @FechaFin DATE, @ArancelBase DECIMAL(18,2)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @StartedTransaction BIT = 0;

    IF @@TRANCOUNT = 0
    BEGIN
        SET @StartedTransaction = 1;
        BEGIN TRANSACTION;
    END

    BEGIN TRY
        IF EXISTS (
            SELECT 1
            FROM dbo.Comision WITH (UPDLOCK, HOLDLOCK)
            WHERE IdProfesor = @IdProfesor
              AND DiaSemana = @DiaSemana
              AND Estado <> N'eliminada'
              AND IdComision <> @IdComision
              AND HoraInicio < @HoraFin
              AND HoraFin > @HoraInicio
        )
            THROW 51001, N'Errores.ProfesorSolapamientoComision', 1;

        UPDATE dbo.Comision
        SET IdCurso = @IdCurso, IdProfesor = @IdProfesor, DiaSemana = @DiaSemana, HoraInicio = @HoraInicio,
            HoraFin = @HoraFin, CupoMinimo = @CupoMinimo, CupoMaximo = @CupoMaximo, FechaLimitePago = @FechaLimitePago,
            FechaInicio = @FechaInicio, FechaFin = @FechaFin, ArancelBase = @ArancelBase
        WHERE IdComision = @IdComision AND Estado <> N'eliminada';

        IF @StartedTransaction = 1
            COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @StartedTransaction = 1 AND XACT_STATE() <> 0
            ROLLBACK TRANSACTION;

        THROW;
    END CATCH;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_CUN01_EliminarComisionLogica]
    @IdComision INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Comision SET Estado = N'eliminada' WHERE IdComision = @IdComision AND Estado = N'preapertura';
END
GO

IF OBJECT_ID(N'dbo.CK_Comision_EstadoPreapertura', N'C') IS NOT NULL
BEGIN
    ALTER TABLE dbo.Comision DROP CONSTRAINT CK_Comision_EstadoPreapertura;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_CUN01_ValidarSolapamientoProfesor]
    @IdProfesor INT,
    @DiaSemana NVARCHAR(20),
    @HoraInicio TIME,
    @HoraFin TIME,
    @IdComisionIgnorar INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (
        SELECT 1 FROM dbo.Comision
        WHERE IdProfesor = @IdProfesor
          AND DiaSemana = @DiaSemana
          AND Estado <> N'eliminada'
          AND (@IdComisionIgnorar IS NULL OR IdComision <> @IdComisionIgnorar)
          AND HoraInicio < @HoraFin AND HoraFin > @HoraInicio
    )
        SELECT 1 AS Solapamiento;
    ELSE
        SELECT 0 AS Solapamiento;
END
GO

-- After applying this migration, execute the application's integrity recovery
-- action or IntegridadDAL_83KI.RecalcularTodo so Comision, Patentes, and
-- RolPatente DVH/DVV values reflect the new canonical columns and seeds.

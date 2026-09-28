-- =============================================================================
-- 001_migracion.sql — migracion aditiva: aranceles y planes de pago en comisiones
--
-- agrega:
--   1. catalogo PlanesDePago (Contado 0, 3 cuotas 5.1250, 6 cuotas 10)
--   2. columnas Comision.ArancelBase / IdPlanDePago / RecargoPlanSnapshot + FK
--   3. fila DVV de Comision en DigitoVerificador_83KI
--   4. sp_CUN01_ListarPlanesDePagoActivos y sp_CUN01_RegistrarPreaperturaComision
--      con la firma extendida (11 parametros, decimales exactos)
--
-- destino: base de desarrollo EXISTENTE que todavia no tiene este cambio, p.ej.
--   (localdb)\ProjectModels / TrabajoDiploma9/13
--   para una base nueva desde cero usar EsquemaCompleto.sql (ya incluye todo esto).
--
-- seguridad: solo agrega objetos y columnas; no borra ni modifica datos.
--            es idempotente: re-ejecutarlo no duplica nada.
--
-- ejecucion (sqlcmd):
--   sqlcmd -S "(localdb)\ProjectModels" -d "TrabajoDiploma9/13" -i "001_migracion.sql"
--   o abrirlo en ssms conectado a la base correcta.
--
-- despues de aplicar: si Comision ya tenia filas, recomputar integridad desde la
--   aplicacion (IntegridadDAL_83KI.RecalcularTodo()) para que los DVH/DVV de
--   Comision coincidan con la canonicalizacion exacta del contrato en C#.
-- =============================================================================

SET NOCOUNT ON;
GO

-- -----------------------------------------------------------------------------
-- 0. guarda: aborta si esta base no tiene la tabla Comision (base nueva o
--    contexto equivocado: para una base desde cero usar EsquemaCompleto.sql)
-- -----------------------------------------------------------------------------
IF OBJECT_ID(N'dbo.Comision', N'U') IS NULL
BEGIN
    RAISERROR(N'dbo.Comision no existe en esta base: este script migra una base existente. Para una base nueva usar EsquemaCompleto.sql.', 16, 1);
    RETURN;
END
GO

-- -----------------------------------------------------------------------------
-- 1. catalogo PlanesDePago
-- -----------------------------------------------------------------------------
IF OBJECT_ID(N'dbo.Comision', N'U') IS NOT NULL
   AND OBJECT_ID(N'[dbo].[PlanesDePago]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[PlanesDePago](
        [IdPlanDePago] INT IDENTITY(1,1) NOT NULL,
        [Nombre] NVARCHAR(100) NOT NULL,
        [RecargoPorcentaje] DECIMAL(9,4) NOT NULL CONSTRAINT [DF_PlanesDePago_Recargo] DEFAULT (0),
        [EstadoActivo] BIT NOT NULL CONSTRAINT [DF_PlanesDePago_EstadoActivo] DEFAULT (1),
        CONSTRAINT [PK_PlanesDePago] PRIMARY KEY CLUSTERED ([IdPlanDePago] ASC),
        CONSTRAINT [UQ_PlanesDePago_Nombre] UNIQUE ([Nombre]),
        CONSTRAINT [CK_PlanesDePago_Recargo] CHECK ([RecargoPorcentaje] >= 0)
    );
END
GO

-- seeds con ids explicitos para que IdPlanDePago=2 siga siendo "3 cuotas"
IF OBJECT_ID(N'dbo.Comision', N'U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM dbo.PlanesDePago)
BEGIN
    SET IDENTITY_INSERT dbo.PlanesDePago ON;
    INSERT INTO dbo.PlanesDePago (IdPlanDePago, Nombre, RecargoPorcentaje, EstadoActivo) VALUES
        (1, N'Contado',   0.0000, 1),
        (2, N'3 cuotas',  5.1250, 1),
        (3, N'6 cuotas', 10.0000, 1);
    SET IDENTITY_INSERT dbo.PlanesDePago OFF;
END
GO

-- -----------------------------------------------------------------------------
-- 2. columnas de asignacion en Comision (solo si no existen)
-- -----------------------------------------------------------------------------
IF OBJECT_ID(N'[dbo].[Comision]', N'U') IS NOT NULL AND COL_LENGTH('dbo.Comision', 'ArancelBase') IS NULL
BEGIN
    ALTER TABLE [dbo].[Comision]
        ADD [ArancelBase] DECIMAL(18,2) NOT NULL
            CONSTRAINT [DF_Comision_ArancelBase] DEFAULT (0.01);
END
GO

IF OBJECT_ID(N'[dbo].[Comision]', N'U') IS NOT NULL AND COL_LENGTH('dbo.Comision', 'IdPlanDePago') IS NULL
BEGIN
    ALTER TABLE [dbo].[Comision]
        ADD [IdPlanDePago] INT NULL;
END
GO

IF OBJECT_ID(N'[dbo].[Comision]', N'U') IS NOT NULL AND COL_LENGTH('dbo.Comision', 'RecargoPlanSnapshot') IS NULL
BEGIN
    ALTER TABLE [dbo].[Comision]
        ADD [RecargoPlanSnapshot] DECIMAL(9,4) NOT NULL
            CONSTRAINT [DF_Comision_RecargoPlanSnapshot] DEFAULT (0);
END
GO

-- -----------------------------------------------------------------------------
-- 3. foreign key Comision -> PlanesDePago (solo si no existe)
-- -----------------------------------------------------------------------------
IF OBJECT_ID(N'[dbo].[FK_Comision_PlanesDePago]', N'F') IS NULL
BEGIN
    ALTER TABLE [dbo].[Comision]
        ADD CONSTRAINT [FK_Comision_PlanesDePago]
            FOREIGN KEY ([IdPlanDePago]) REFERENCES [dbo].[PlanesDePago]([IdPlanDePago]);
END
GO

-- -----------------------------------------------------------------------------
-- 4. fila DVV de Comision (para que RecalcularTodo pueda actualizar el DVV)
-- -----------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM dbo.DigitoVerificador_83KI WHERE NombreTabla = N'Comision')
BEGIN
    INSERT INTO dbo.DigitoVerificador_83KI (NombreTabla, DVV, FechaActualizacion)
    VALUES (N'Comision', '', GETDATE());
END
GO

-- -----------------------------------------------------------------------------
-- 5. procedure de listado de planes activos
-- -----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE [dbo].[sp_CUN01_ListarPlanesDePagoActivos]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT IdPlanDePago, Nombre, RecargoPorcentaje, EstadoActivo
    FROM dbo.PlanesDePago
    WHERE EstadoActivo = 1
    ORDER BY Nombre;
END
GO

-- -----------------------------------------------------------------------------
-- 6. procedure de registro de preapertura con firma extendida
-- -----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE [dbo].[sp_CUN01_RegistrarPreaperturaComision]
    @IdCurso INT,
    @IdProfesor INT,
    @DiaSemana NVARCHAR(20),
    @HoraInicio TIME,
    @HoraFin TIME,
    @CupoMinimo INT,
    @CupoMaximo INT,
    @FechaLimitePago DATE,
    @ArancelBase DECIMAL(18,2),
    @IdPlanDePago INT,
    @RecargoPlanSnapshot DECIMAL(9,4)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @IdComision INT;
    DECLARE @Codigo NVARCHAR(30);

    INSERT INTO dbo.Comision
        (Codigo, IdCurso, IdProfesor, DiaSemana, HoraInicio, HoraFin,
         CupoMinimo, CupoMaximo, FechaLimitePago, ArancelBase, IdPlanDePago,
         RecargoPlanSnapshot, Estado, DVH)
    VALUES
        (N'', @IdCurso, @IdProfesor, @DiaSemana, @HoraInicio, @HoraFin,
         @CupoMinimo, @CupoMaximo, @FechaLimitePago, @ArancelBase, @IdPlanDePago,
         @RecargoPlanSnapshot, N'preapertura', N'');

    SET @IdComision = SCOPE_IDENTITY();
    SET @Codigo = CONCAT(N'COM-', RIGHT(CONCAT(N'000000', @IdComision), 6));

    UPDATE dbo.Comision SET Codigo = @Codigo WHERE IdComision = @IdComision;

    SELECT IdComision, Codigo, IdCurso, IdProfesor, DiaSemana, HoraInicio, HoraFin,
           CupoMinimo, CupoMaximo, FechaLimitePago, ArancelBase, IdPlanDePago,
           RecargoPlanSnapshot, Estado, DVH
    FROM dbo.Comision
    WHERE IdComision = @IdComision;
END
GO

PRINT 'migracion asignar-aranceles-planes aplicada.';
GO

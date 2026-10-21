-- Requires the existing CUN03/CUN05/CUN06/CUN07 schema. Run aplicar.sql from the repository root.
SET NOCOUNT ON;
SET XACT_ABORT ON;
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_PADDING ON;
SET ANSI_WARNINGS ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET ARITHABORT ON;
SET NUMERIC_ROUNDABORT OFF;
GO
IF COL_LENGTH(N'dbo.PlanesDePago', N'CantidadCuotas') IS NULL
    ALTER TABLE dbo.PlanesDePago ADD CantidadCuotas INT NULL;
GO
-- One-time mapping of the supported legacy names, including seeded test prefixes.
UPDATE dbo.PlanesDePago SET CantidadCuotas = CASE
    WHEN LOWER(RTRIM(Nombre)) LIKE N'%contado' THEN 1
    WHEN LOWER(RTRIM(Nombre)) LIKE N'%3 cuotas' THEN 3
    WHEN LOWER(RTRIM(Nombre)) LIKE N'%6 cuotas' THEN 6 END
WHERE CantidadCuotas IS NULL;
IF EXISTS(SELECT 1 FROM dbo.PlanesDePago WHERE CantidadCuotas IS NULL OR CantidadCuotas NOT BETWEEN 1 AND 120)
    THROW 52801, N'Asigne CantidadCuotas (1..120) a los planes no reconocidos antes de continuar.', 1;
ALTER TABLE dbo.PlanesDePago ALTER COLUMN CantidadCuotas INT NOT NULL;
IF NOT EXISTS(SELECT 1 FROM sys.check_constraints WHERE name=N'CK_PlanesDePago_CantidadCuotas')
    ALTER TABLE dbo.PlanesDePago ADD CONSTRAINT CK_PlanesDePago_CantidadCuotas CHECK(CantidadCuotas BETWEEN 1 AND 120);
GO
IF COL_LENGTH(N'dbo.Cuota', N'TipoConcepto') IS NULL
    ALTER TABLE dbo.Cuota ADD TipoConcepto NVARCHAR(20) NOT NULL CONSTRAINT DF_Cuota_TipoConcepto DEFAULT(N'matricula');
IF COL_LENGTH(N'dbo.Cuota', N'Periodo') IS NULL
    ALTER TABLE dbo.Cuota ADD Periodo DATE NULL;
IF COL_LENGTH(N'dbo.SolicitudInscripcion', N'ArancelCursoSnapshot') IS NULL
    ALTER TABLE dbo.SolicitudInscripcion ADD ArancelCursoSnapshot DECIMAL(18,2) NULL,
        CantidadCuotasSnapshot INT NULL, TotalCursoSnapshot DECIMAL(18,2) NULL,
        FechaPrimerVencimientoSnapshot DATE NULL, NombrePlanSnapshot NVARCHAR(200) NULL;
GO
-- Keep the historical IDs and numbers: matrícula is 1; course installments are 2..N+1.
UPDATE dbo.Cuota SET TipoConcepto=N'curso', Periodo=DATEFROMPARTS(YEAR(FechaVencimiento),MONTH(FechaVencimiento),1)
WHERE NumeroCuota>1 AND TipoConcepto=N'matricula';
IF NOT EXISTS(SELECT 1 FROM sys.check_constraints WHERE name=N'CK_Cuota_ConceptoPeriodo')
    ALTER TABLE dbo.Cuota ADD CONSTRAINT CK_Cuota_ConceptoPeriodo CHECK(
        (TipoConcepto=N'matricula' AND NumeroCuota=1 AND Periodo IS NULL) OR
        (TipoConcepto=N'curso' AND NumeroCuota>1 AND Periodo IS NOT NULL AND DAY(Periodo)=1));
IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.Cuota') AND name=N'UX_Cuota_InscripcionNumero')
    CREATE UNIQUE INDEX UX_Cuota_InscripcionNumero ON dbo.Cuota(IdSolicitudInscripcion,NumeroCuota);
IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.Cuota') AND name=N'UX_Cuota_InscripcionPeriodo')
    CREATE UNIQUE INDEX UX_Cuota_InscripcionPeriodo ON dbo.Cuota(IdSolicitudInscripcion,Periodo) WHERE TipoConcepto=N'curso';
GO
CREATE OR ALTER PROCEDURE dbo.sp_Financiacion_GenerarCuotas @IdSolicitudInscripcion INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    DECLARE @started BIT=0, @arancel DECIMAL(18,2), @recargo DECIMAL(9,4), @cantidad INT,
        @total DECIMAL(18,2), @inicio DATE, @nombre NVARCHAR(200), @base DECIMAL(18,2), @numero INT=1,
        @importe DECIMAL(18,2), @vence DATE, @periodo DATE;
    IF @@TRANCOUNT=0 BEGIN SET @started=1; BEGIN TRANSACTION; END;
    BEGIN TRY
        SELECT @arancel=COALESCE(si.ArancelCursoSnapshot,c.ArancelBase),
            @cantidad=COALESCE(si.CantidadCuotasSnapshot,pp.CantidadCuotas),
            @inicio=COALESCE(si.FechaPrimerVencimientoSnapshot,c.FechaInicio),
            @nombre=COALESCE(si.NombrePlanSnapshot,pp.Nombre), @recargo=si.RecargoPlanSnapshot,
            @total=si.TotalCursoSnapshot
        FROM dbo.SolicitudInscripcion si WITH(UPDLOCK,HOLDLOCK)
        JOIN dbo.Comision c WITH(HOLDLOCK) ON c.IdComision=si.IdComision
        JOIN dbo.PlanesDePago pp WITH(HOLDLOCK) ON pp.IdPlanDePago=si.IdPlanDePago
        WHERE si.IdSolicitudInscripcion=@IdSolicitudInscripcion;
        IF @arancel IS NULL OR @arancel<=0 OR @cantidad IS NULL OR @cantidad NOT BETWEEN 1 AND 120 OR @recargo<0
            THROW 52802,N'Errores.FinanciacionInvalida',1;
        SET @total=COALESCE(@total,ROUND(@arancel*(1+@recargo/100.0),2));
        IF @total<@cantidad*0.01 THROW 52802,N'Errores.FinanciacionInvalida',1;
        SET @base=FLOOR(@total*100/@cantidad)/100;
        IF EXISTS(SELECT 1 FROM dbo.Cuota WHERE IdSolicitudInscripcion=@IdSolicitudInscripcion
            AND TipoConcepto=N'curso' AND NumeroCuota>@cantidad+1)
            THROW 52803,N'Las cuotas existentes no coinciden con el plan; revise la inscripción sin modificar sus pagos.',1;
        UPDATE dbo.SolicitudInscripcion SET ArancelCursoSnapshot=@arancel,CantidadCuotasSnapshot=@cantidad,
            TotalCursoSnapshot=@total,FechaPrimerVencimientoSnapshot=@inicio,NombrePlanSnapshot=@nombre
        WHERE IdSolicitudInscripcion=@IdSolicitudInscripcion;
        WHILE @numero<=@cantidad
        BEGIN
            SET @importe=CASE WHEN @numero=@cantidad THEN @total-@base*(@cantidad-1) ELSE @base END;
            -- Always add months to the original anchor, never to the adjusted February date.
            SET @vence=DATEADD(MONTH,@numero-1,@inicio);
            SET @periodo=DATEFROMPARTS(YEAR(@vence),MONTH(@vence),1);
            IF EXISTS(SELECT 1 FROM dbo.Cuota WITH(UPDLOCK,HOLDLOCK)
                WHERE IdSolicitudInscripcion=@IdSolicitudInscripcion AND NumeroCuota=@numero+1
                AND (TipoConcepto<>N'curso' OR MontoOriginal<>@importe OR FechaVencimiento<>@vence OR Periodo<>@periodo))
                THROW 52803,N'Las cuotas existentes no coinciden con el plan; revise la inscripción sin modificar sus pagos.',1;
            IF NOT EXISTS(SELECT 1 FROM dbo.Cuota WITH(UPDLOCK,HOLDLOCK)
                WHERE IdSolicitudInscripcion=@IdSolicitudInscripcion AND NumeroCuota=@numero+1)
                INSERT dbo.Cuota(IdSolicitudInscripcion,NumeroCuota,TipoConcepto,Periodo,MontoOriginal,BalanceAdeudado,FechaVencimiento,Estado,DVH)
                VALUES(@IdSolicitudInscripcion,@numero+1,N'curso',@periodo,@importe,@importe,@vence,N'pendiente',N'');
            SET @numero+=1;
        END;
        IF @started=1 COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @started=1 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;
GO
CREATE OR ALTER PROCEDURE dbo.sp_CUN01_ListarPlanesDePagoActivos
AS
BEGIN
    SET NOCOUNT ON;
    SELECT IdPlanDePago,Nombre,RecargoPorcentaje,EstadoActivo,CantidadCuotas
    FROM dbo.PlanesDePago WHERE EstadoActivo=1 ORDER BY Nombre;
END;
GO

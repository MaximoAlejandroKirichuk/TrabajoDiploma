-- Existing enrollment fees and payment rows are never rewritten.
-- Legacy enrollments have no historical course price: use the current commission price
-- with their already stored surcharge, then freeze all financing conditions.
SET NOCOUNT ON;
SET XACT_ABORT ON;
IF OBJECT_ID(N'dbo.FinanciacionMigraciones',N'U') IS NULL
    CREATE TABLE dbo.FinanciacionMigraciones(Version INT NOT NULL PRIMARY KEY,FechaAplicacion DATETIME NOT NULL);
GO
BEGIN TRANSACTION;
BEGIN TRY
    DECLARE @id INT;
    CREATE TABLE #afectadas(IdSolicitudInscripcion INT NOT NULL);
    INSERT #afectadas SELECT IdSolicitudInscripcion FROM dbo.SolicitudInscripcion
        WHERE Estado IN(N'pendiente',N'confirmada',N'activa',N'Al día',N'Con deuda') AND ArancelCursoSnapshot IS NULL;
    DECLARE pendientes CURSOR LOCAL FAST_FORWARD FOR
        SELECT IdSolicitudInscripcion FROM #afectadas;
    OPEN pendientes;
    FETCH NEXT FROM pendientes INTO @id;
    WHILE @@FETCH_STATUS=0
    BEGIN
        EXEC dbo.sp_Financiacion_GenerarCuotas @id;
        FETCH NEXT FROM pendientes INTO @id;
    END;
    CLOSE pendientes;
    DEALLOCATE pendientes;

    -- Restore the academic state previously overwritten by CUN07 financial standing.
    UPDATE si SET Estado=N'activa'
    OUTPUT inserted.IdSolicitudInscripcion INTO #afectadas
    FROM dbo.SolicitudInscripcion si
    WHERE si.Estado IN(N'Al día',N'Con deuda')
        AND EXISTS(SELECT 1 FROM dbo.Cuota m WHERE m.IdSolicitudInscripcion=si.IdSolicitudInscripcion
            AND m.TipoConcepto=N'matricula' AND m.Estado=N'pagada');

    -- Add the new protected columns to the original hashes once; do not hide later changes.
    DECLARE @primera BIT=CASE WHEN EXISTS(SELECT 1 FROM dbo.FinanciacionMigraciones WHERE Version=1) THEN 0 ELSE 1 END;
    IF @primera=1 OR EXISTS(SELECT 1 FROM #afectadas)
    BEGIN
        UPDATE dbo.Cuota SET DVH=LOWER(CONVERT(VARCHAR(64),HASHBYTES('SHA2_256',CONVERT(NVARCHAR(MAX),
            N'BalanceAdeudado='+CONVERT(NVARCHAR(40),BalanceAdeudado)+N'|Estado='+LTRIM(RTRIM(Estado))+
            N'|FechaVencimiento='+REPLACE(CONVERT(NVARCHAR(23),CAST(FechaVencimiento AS DATETIME),121),N' ',N'T')+
            N'|IdCuota='+CONVERT(NVARCHAR(20),IdCuota)+N'|IdSolicitudInscripcion='+CONVERT(NVARCHAR(20),IdSolicitudInscripcion)+
            N'|MontoOriginal='+CONVERT(NVARCHAR(40),MontoOriginal)+N'|NumeroCuota='+CONVERT(NVARCHAR(20),NumeroCuota)+
            N'|Periodo='+COALESCE(REPLACE(CONVERT(NVARCHAR(23),CAST(Periodo AS DATETIME),121),N' ',N'T'),N'∅')+
            N'|TipoConcepto='+LTRIM(RTRIM(TipoConcepto)))),2))
        WHERE @primera=1 OR IdSolicitudInscripcion IN(SELECT IdSolicitudInscripcion FROM #afectadas);
        UPDATE dbo.SolicitudInscripcion SET DVH=LOWER(CONVERT(VARCHAR(64),HASHBYTES('SHA2_256',CONVERT(NVARCHAR(MAX),
            N'ArancelCursoSnapshot='+COALESCE(CONVERT(NVARCHAR(40),ArancelCursoSnapshot),N'∅')+
            N'|CantidadCuotasSnapshot='+COALESCE(CONVERT(NVARCHAR(20),CantidadCuotasSnapshot),N'∅')+
            N'|Codigo='+LTRIM(RTRIM(Codigo))+N'|Estado='+LTRIM(RTRIM(Estado))+
            N'|FechaPrimerVencimientoSnapshot='+COALESCE(REPLACE(CONVERT(NVARCHAR(23),CAST(FechaPrimerVencimientoSnapshot AS DATETIME),121),N' ',N'T'),N'∅')+
            N'|FechaSolicitud='+REPLACE(CONVERT(NVARCHAR(23),FechaSolicitud,121),N' ',N'T')+N'|IdAlumno='+CONVERT(NVARCHAR(20),IdAlumno)+
            N'|IdComision='+CONVERT(NVARCHAR(20),IdComision)+N'|IdLeadOrigen='+COALESCE(CONVERT(NVARCHAR(20),IdLeadOrigen),N'∅')+
            N'|IdPlanDePago='+CONVERT(NVARCHAR(20),IdPlanDePago)+N'|IdSolicitudInscripcion='+CONVERT(NVARCHAR(20),IdSolicitudInscripcion)+
            N'|NombrePlanSnapshot='+COALESCE(LTRIM(RTRIM(NombrePlanSnapshot)),N'∅')+
            N'|Observaciones='+LTRIM(RTRIM(Observaciones))+N'|RecargoPlanSnapshot='+CONVERT(NVARCHAR(40),RecargoPlanSnapshot)+
            N'|TotalCursoSnapshot='+COALESCE(CONVERT(NVARCHAR(40),TotalCursoSnapshot),N'∅'))),2))
        WHERE @primera=1 OR IdSolicitudInscripcion IN(SELECT IdSolicitudInscripcion FROM #afectadas);
        DECLARE @cuotas NVARCHAR(MAX),@solicitudes NVARCHAR(MAX);
        SELECT @cuotas=COALESCE((SELECT CONVERT(NVARCHAR(MAX),DVH) AS [text()] FROM dbo.Cuota ORDER BY DVH FOR XML PATH(''),TYPE).value('.','NVARCHAR(MAX)'),N'');
        SELECT @solicitudes=COALESCE((SELECT CONVERT(NVARCHAR(MAX),DVH) AS [text()] FROM dbo.SolicitudInscripcion ORDER BY DVH FOR XML PATH(''),TYPE).value('.','NVARCHAR(MAX)'),N'');
        UPDATE dbo.DigitoVerificador_83KI SET DVV=LOWER(CONVERT(VARCHAR(64),HASHBYTES('SHA2_256',@cuotas),2)),FechaActualizacion=GETDATE() WHERE NombreTabla=N'Cuota';
        UPDATE dbo.DigitoVerificador_83KI SET DVV=LOWER(CONVERT(VARCHAR(64),HASHBYTES('SHA2_256',@solicitudes),2)),FechaActualizacion=GETDATE() WHERE NombreTabla=N'SolicitudInscripcion';
        IF @primera=1 INSERT dbo.FinanciacionMigraciones VALUES(1,GETDATE());
    END;
    DROP TABLE #afectadas;
    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT>0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
GO
SELECT si.Codigo,si.NombrePlanSnapshot,si.ArancelCursoSnapshot,si.RecargoPlanSnapshot,
    si.CantidadCuotasSnapshot,si.TotalCursoSnapshot,cu.TipoConcepto,
    CASE WHEN cu.TipoConcepto=N'curso' THEN cu.NumeroCuota-1 END NumeroMensualidad,
    cu.Periodo,cu.MontoOriginal,cu.BalanceAdeudado,cu.FechaVencimiento,cu.Estado
FROM dbo.SolicitudInscripcion si JOIN dbo.Cuota cu ON cu.IdSolicitudInscripcion=si.IdSolicitudInscripcion
ORDER BY si.IdSolicitudInscripcion,cu.NumeroCuota;
GO

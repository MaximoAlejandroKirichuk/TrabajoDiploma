-- DEMO CUN06 - "Vence" la comision creada en vivo en CUN01 para que aparezca en CUN06.
-- Pone FechaLimitePago = ayer y recalcula DVH (fila) y DVV (tabla Comision)
-- con la misma formula que usa cun06/003_datos_prueba_estado_definitivo.sql.
-- SOLO PARA DEMO. Probar antes en el ensayo.

USE [InstitutoVisionEnAccion];
GO
SET NOCOUNT ON;
SET XACT_ABORT ON;

DECLARE @Codigo NVARCHAR(30) = N'COM-000001';   -- <<< CAMBIAR por el codigo que devolvio CUN01

IF NOT EXISTS (SELECT 1 FROM dbo.Comision WHERE Codigo = @Codigo AND Estado = N'preapertura')
BEGIN
    RAISERROR(N'No existe una comision en preapertura con ese codigo.', 16, 1);
    RETURN;
END;

BEGIN TRANSACTION;

UPDATE dbo.Comision
SET FechaLimitePago = DATEADD(DAY, -1, CONVERT(date, GETDATE()))
WHERE Codigo = @Codigo;

UPDATE dbo.Comision
SET DVH = LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', CONCAT(
    N'ArancelBase=', CONVERT(NVARCHAR(128), ArancelBase), N'|',
    N'Codigo=', LTRIM(RTRIM(ISNULL(CONVERT(NVARCHAR(MAX), Codigo), N'∅'))), N'|',
    N'CupoMaximo=', CONVERT(NVARCHAR(128), CupoMaximo), N'|',
    N'CupoMinimo=', CONVERT(NVARCHAR(128), CupoMinimo), N'|',
    N'DiaSemana=', LTRIM(RTRIM(ISNULL(CONVERT(NVARCHAR(MAX), DiaSemana), N'∅'))), N'|',
    N'Estado=', LTRIM(RTRIM(ISNULL(CONVERT(NVARCHAR(MAX), Estado), N'∅'))), N'|',
    N'FechaFin=', ISNULL(CONVERT(NVARCHAR(23), CAST(FechaFin AS DATETIME), 126), N'∅'), N'|',
    N'FechaInicio=', ISNULL(CONVERT(NVARCHAR(23), CAST(FechaInicio AS DATETIME), 126), N'∅'), N'|',
    N'FechaLimitePago=', ISNULL(CONVERT(NVARCHAR(23), CAST(FechaLimitePago AS DATETIME), 126), N'∅'), N'|',
    N'HoraFin=', CONVERT(NVARCHAR(128), HoraFin), N'|',
    N'HoraInicio=', CONVERT(NVARCHAR(128), HoraInicio), N'|',
    N'IdComision=', CONVERT(NVARCHAR(128), IdComision), N'|',
    N'IdCurso=', CONVERT(NVARCHAR(128), IdCurso), N'|',
    N'IdPlanDePago=', CONVERT(NVARCHAR(128), IdPlanDePago), N'|',
    N'IdProfesor=', CONVERT(NVARCHAR(128), IdProfesor), N'|',
    N'MontoMatricula=', CONVERT(NVARCHAR(128), MontoMatricula), N'|',
    N'RecargoPlanSnapshot=', CONVERT(NVARCHAR(128), RecargoPlanSnapshot)
)), 2))
WHERE Codigo = @Codigo;

UPDATE dbo.DigitoVerificador_83KI
SET DVV = LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', ISNULL(CAST((SELECT CAST(N'' AS NVARCHAR(MAX)) + DVH FROM dbo.Comision ORDER BY DVH FOR XML PATH(N'')) AS NVARCHAR(MAX)), N'')), 2)),
    FechaActualizacion = GETDATE()
WHERE NombreTabla = N'Comision';

COMMIT TRANSACTION;

PRINT N'Comision vencida. En CUN06 presionar "Actualizar".';
EXEC dbo.sp_CUN06_ListarComisionesVencidas;

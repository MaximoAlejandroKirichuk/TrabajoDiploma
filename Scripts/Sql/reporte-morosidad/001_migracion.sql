-- RF2 - Informe de morosidad por alumno y comisión.
-- Migración aditiva: crea la patente 75 "Ver reporte de morosidad" y actualiza el DVV de Patentes.
-- No crea tablas ni procedimientos: el informe usa una consulta parametrizada en ReporteMorosidadDAL_83KI.
-- No concede el permiso a ningún rol: asignarlo desde Admin > Gestión de roles y volver a iniciar sesión.
-- Re-ejecutable: si la patente ya existe con el mismo nombre, no hace cambios.
-- Ejecutar con copia de seguridad previa y con las escrituras de la aplicación detenidas.
SET NOCOUNT ON;
SET XACT_ABORT ON;
BEGIN TRY
    BEGIN TRANSACTION;
    IF OBJECT_ID(N'dbo.Patentes',N'U') IS NULL OR OBJECT_ID(N'dbo.DigitoVerificador_83KI',N'U') IS NULL
       OR OBJECT_ID(N'dbo.Cuota',N'U') IS NULL OR COL_LENGTH(N'dbo.Cuota',N'TipoConcepto') IS NULL
        THROW 52950,N'El informe de morosidad requiere el esquema de financiación de cuotas (CUN03/CUN05/CUN07).',1;
    IF EXISTS(SELECT 1 FROM dbo.Patentes WHERE CodigoPatente=75 AND Nombre COLLATE Latin1_General_100_BIN2<>N'Ver reporte de morosidad')
       OR EXISTS(SELECT 1 FROM dbo.Patentes WHERE Nombre=N'Ver reporte de morosidad' AND CodigoPatente<>75)
        THROW 52951,N'El permiso 75 o su nombre ya están en uso. Revisar antes de aplicar.',1;
    IF NOT EXISTS(SELECT 1 FROM dbo.Patentes WHERE CodigoPatente=75)
    BEGIN
        SET IDENTITY_INSERT dbo.Patentes ON;
        INSERT dbo.Patentes(CodigoPatente,Nombre,DVH)
        VALUES(75,N'Ver reporte de morosidad',LOWER(CONVERT(VARCHAR(64),HASHBYTES('SHA2_256',
            CONVERT(NVARCHAR(MAX),N'CodigoPatente=75|Nombre=Ver reporte de morosidad')),2)));
        SET IDENTITY_INSERT dbo.Patentes OFF;
        DECLARE @dvvPatentesMorosidad NVARCHAR(MAX)=COALESCE((SELECT CONVERT(NVARCHAR(MAX),DVH) AS [text()]
            FROM dbo.Patentes ORDER BY DVH FOR XML PATH(''),TYPE).value('.','nvarchar(max)'),N'');
        UPDATE dbo.DigitoVerificador_83KI
        SET DVV=LOWER(CONVERT(VARCHAR(64),HASHBYTES('SHA2_256',@dvvPatentesMorosidad),2)),FechaActualizacion=GETDATE()
        WHERE NombreTabla=N'Patentes';
        IF @@ROWCOUNT<>1 THROW 52952,N'Falta el registro de integridad de Patentes.',1;
    END;
    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT>0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;

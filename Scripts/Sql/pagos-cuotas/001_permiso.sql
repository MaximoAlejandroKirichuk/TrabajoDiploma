-- M-4: separar el permiso de pago de cuotas del permiso de pago de inscripción (permiso 66).
-- No asigna el permiso a ningún rol; debe asignarse desde la gestión de roles.
-- Se puede volver a ejecutar: si el permiso ya existe y coincide, no se modifica.
SET NOCOUNT ON;
SET XACT_ABORT ON;
BEGIN TRY
    BEGIN TRANSACTION;
    IF OBJECT_ID(N'dbo.Patentes',N'U') IS NULL OR OBJECT_ID(N'dbo.DigitoVerificador_83KI',N'U') IS NULL
        THROW 52960,N'La migración de pagos de cuotas requiere Patentes y DigitoVerificador_83KI.',1;
    IF EXISTS(SELECT 1 FROM dbo.Patentes WHERE CodigoPatente=76 AND Nombre COLLATE Latin1_General_100_BIN2<>N'Registrar pago de cuotas')
       OR EXISTS(SELECT 1 FROM dbo.Patentes WHERE Nombre=N'Registrar pago de cuotas' AND CodigoPatente<>76)
        THROW 52961,N'El permiso 76 o su nombre ya están en uso. Revisar antes de aplicar.',1;
    IF NOT EXISTS(SELECT 1 FROM dbo.Patentes WHERE CodigoPatente=76)
    BEGIN
        SET IDENTITY_INSERT dbo.Patentes ON;
        INSERT dbo.Patentes(CodigoPatente,Nombre,DVH)
        VALUES(76,N'Registrar pago de cuotas',LOWER(CONVERT(VARCHAR(64),HASHBYTES('SHA2_256',
            CONVERT(NVARCHAR(MAX),N'CodigoPatente=76|Nombre=Registrar pago de cuotas')),2)));
        SET IDENTITY_INSERT dbo.Patentes OFF;
        DECLARE @dvvPatentes NVARCHAR(MAX)=COALESCE((SELECT CONVERT(NVARCHAR(MAX),DVH) AS [text()]
            FROM dbo.Patentes ORDER BY DVH FOR XML PATH(''),TYPE).value('.','nvarchar(max)'),N'');
        UPDATE dbo.DigitoVerificador_83KI
        SET DVV=LOWER(CONVERT(VARCHAR(64),HASHBYTES('SHA2_256',@dvvPatentes),2)),FechaActualizacion=GETDATE()
        WHERE NombreTabla=N'Patentes';
        IF @@ROWCOUNT<>1 THROW 52962,N'Falta el registro de integridad de Patentes.',1;
    END;
    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT>0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;

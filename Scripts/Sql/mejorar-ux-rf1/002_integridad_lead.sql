USE [InstitutoVisionEnAccion]
GO

IF OBJECT_ID(N'dbo.[Lead]', N'U') IS NOT NULL
   AND OBJECT_ID(N'dbo.DigitoVerificador_83KI', N'U') IS NOT NULL
BEGIN
    UPDATE dbo.[Lead]
    SET DVH = LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', CONCAT(
        N'Apellido=', LTRIM(RTRIM(ISNULL(CONVERT(NVARCHAR(MAX), Apellido), N'∅'))), N'|',
        N'DNI=', LTRIM(RTRIM(ISNULL(CONVERT(NVARCHAR(MAX), DNI), N'∅'))), N'|',
        N'Email=', LTRIM(RTRIM(ISNULL(CONVERT(NVARCHAR(MAX), Email), N'∅'))), N'|',
        N'Estado=', LTRIM(RTRIM(ISNULL(CONVERT(NVARCHAR(MAX), Estado), N'∅'))), N'|',
        N'FechaAlta=', CONVERT(NVARCHAR(25), FechaAlta, 126), N'|',
        N'IdLead=', CONVERT(NVARCHAR(128), IdLead), N'|',
        N'Nombre=', LTRIM(RTRIM(ISNULL(CONVERT(NVARCHAR(MAX), Nombre), N'∅'))), N'|',
        N'Telefono=', LTRIM(RTRIM(ISNULL(CONVERT(NVARCHAR(MAX), Telefono), N'∅')))
    )), 2));

    IF NOT EXISTS (SELECT 1 FROM dbo.DigitoVerificador_83KI WHERE NombreTabla = N'Lead')
        INSERT INTO dbo.DigitoVerificador_83KI (NombreTabla, DVV, FechaActualizacion) VALUES (N'Lead', '', GETDATE());

    UPDATE dbo.DigitoVerificador_83KI
    SET DVV = LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', ISNULL(CAST((SELECT CAST(N'' AS NVARCHAR(MAX)) + DVH FROM dbo.[Lead] ORDER BY DVH FOR XML PATH(N'')) AS NVARCHAR(MAX)), N'')), 2)), FechaActualizacion = GETDATE()
    WHERE NombreTabla = N'Lead';
END
GO

IF OBJECT_ID(N'dbo.ConsultaLead', N'U') IS NOT NULL
   AND OBJECT_ID(N'dbo.DigitoVerificador_83KI', N'U') IS NOT NULL
BEGIN
    UPDATE dbo.ConsultaLead
    SET DVH = LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', CONCAT(
        N'Codigo=', LTRIM(RTRIM(ISNULL(CONVERT(NVARCHAR(MAX), Codigo), N'∅'))), N'|',
        N'Estado=', LTRIM(RTRIM(ISNULL(CONVERT(NVARCHAR(MAX), Estado), N'∅'))), N'|',
        N'FechaConsulta=', CONVERT(NVARCHAR(25), FechaConsulta, 126), N'|',
        N'IdComision=', CONVERT(NVARCHAR(128), IdComision), N'|',
        N'IdConsultaLead=', CONVERT(NVARCHAR(128), IdConsultaLead), N'|',
        N'IdLead=', CONVERT(NVARCHAR(128), IdLead), N'|',
        N'MedioContacto=', LTRIM(RTRIM(ISNULL(CONVERT(NVARCHAR(MAX), MedioContacto), N'∅'))), N'|',
        N'Motivo=', LTRIM(RTRIM(ISNULL(CONVERT(NVARCHAR(MAX), Motivo), N'∅'))), N'|',
        N'Observaciones=', LTRIM(RTRIM(ISNULL(CONVERT(NVARCHAR(MAX), Observaciones), N'∅')))
    )), 2));

    IF NOT EXISTS (SELECT 1 FROM dbo.DigitoVerificador_83KI WHERE NombreTabla = N'ConsultaLead')
        INSERT INTO dbo.DigitoVerificador_83KI (NombreTabla, DVV, FechaActualizacion) VALUES (N'ConsultaLead', '', GETDATE());

    UPDATE dbo.DigitoVerificador_83KI
    SET DVV = LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', ISNULL(CAST((SELECT CAST(N'' AS NVARCHAR(MAX)) + DVH FROM dbo.ConsultaLead ORDER BY DVH FOR XML PATH(N'')) AS NVARCHAR(MAX)), N'')), 2)), FechaActualizacion = GETDATE()
    WHERE NombreTabla = N'ConsultaLead';
END
GO

-- Migración aditiva. Detener las operaciones de escritura de la aplicación y crear una copia de seguridad antes de aplicarla manualmente.
SET NOCOUNT ON;
SET XACT_ABORT ON;
BEGIN TRY
 BEGIN TRANSACTION;
 IF OBJECT_ID(N'dbo.ResultadoEvaluacion',N'U') IS NULL OR OBJECT_ID(N'dbo.DigitoVerificador_83KI',N'U') IS NULL
     THROW 52910,N'CUN09 requires CUN08 and the financing/integrity schema.',1;
 IF EXISTS(SELECT 1 FROM dbo.Patentes WHERE CodigoPatente=72 AND Nombre COLLATE Latin1_General_100_BIN2<>N'Generar certificado de finalización')
     THROW 52911,N'Permission 72 is already in use.',1;
 IF OBJECT_ID(N'dbo.CertificadoFinalizacion',N'U') IS NULL
 BEGIN
   CREATE TABLE dbo.CertificadoFinalizacion(
     IdCertificado INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_CertificadoFinalizacion PRIMARY KEY,
     IdInscripcion INT NOT NULL CONSTRAINT FK_Certificado_Inscripcion REFERENCES dbo.SolicitudInscripcion(IdSolicitudInscripcion),
     Nombre NVARCHAR(200) NOT NULL, Apellido NVARCHAR(200) NOT NULL, Curso NVARCHAR(500) NOT NULL,
     Promedio DECIMAL(28,16) NOT NULL, Firmante NVARCHAR(200) NOT NULL, Cargo NVARCHAR(200) NOT NULL,
     Folio NVARCHAR(200) NOT NULL, Fecha DATE NOT NULL, Modalidad NVARCHAR(30) NOT NULL,
     Email NVARCHAR(254) NOT NULL, Mencion NVARCHAR(200) NOT NULL, Observaciones NVARCHAR(500) NOT NULL,
     DVH VARCHAR(64) NOT NULL,
     CONSTRAINT UQ_Certificado_Inscripcion UNIQUE(IdInscripcion),
     CONSTRAINT UQ_Certificado_Folio UNIQUE(Folio),
     CONSTRAINT CK_Certificado_Promedio CHECK(Promedio>=7 AND Promedio<=10),
     CONSTRAINT CK_Certificado_Modalidad CHECK(Modalidad IN(N'Solo digital',N'Digital e impresa')),
     CONSTRAINT CK_Certificado_Requeridos CHECK(LEN(LTRIM(RTRIM(Folio)))>0 AND LEN(LTRIM(RTRIM(Firmante)))>0 AND LEN(LTRIM(RTRIM(Cargo)))>0)
   );
 END;
 IF COL_LENGTH(N'dbo.CertificadoFinalizacion',N'Promedio') IS NULL OR
    NOT EXISTS(SELECT 1 FROM sys.key_constraints WHERE parent_object_id=OBJECT_ID(N'dbo.CertificadoFinalizacion') AND name=N'UQ_Certificado_Inscripcion') OR
    NOT EXISTS(SELECT 1 FROM sys.key_constraints WHERE parent_object_id=OBJECT_ID(N'dbo.CertificadoFinalizacion') AND name=N'UQ_Certificado_Folio')
     THROW 52912,N'Incompatible certificate schema. Review manually.',1;
 IF NOT EXISTS(SELECT 1 FROM dbo.Patentes WHERE CodigoPatente=72)
 BEGIN
   SET IDENTITY_INSERT dbo.Patentes ON;
   INSERT dbo.Patentes(CodigoPatente,Nombre,DVH) VALUES(72,N'Generar certificado de finalización',
     LOWER(CONVERT(VARCHAR(64),HASHBYTES('SHA2_256',CONVERT(NVARCHAR(MAX),N'CodigoPatente=72|Nombre=Generar certificado de finalización')),2)));
   SET IDENTITY_INSERT dbo.Patentes OFF;
   DECLARE @hashes NVARCHAR(MAX)=COALESCE((SELECT CONVERT(NVARCHAR(MAX),DVH) AS [text()] FROM dbo.Patentes ORDER BY DVH FOR XML PATH(''),TYPE).value('.','nvarchar(max)'),N'');
   UPDATE dbo.DigitoVerificador_83KI SET DVV=LOWER(CONVERT(VARCHAR(64),HASHBYTES('SHA2_256',@hashes),2)),FechaActualizacion=GETDATE() WHERE NombreTabla=N'Patentes';
   IF @@ROWCOUNT<>1 THROW 52913,N'Permission integrity registration missing.',1;
 END;
 IF NOT EXISTS(SELECT 1 FROM dbo.DigitoVerificador_83KI WHERE NombreTabla=N'CertificadoFinalizacion')
 BEGIN
   IF EXISTS(SELECT 1 FROM dbo.CertificadoFinalizacion) THROW 52914,N'Existing certificates require integrity review, not a backfill.',1;
   INSERT dbo.DigitoVerificador_83KI(NombreTabla,DVV,FechaActualizacion) VALUES(N'CertificadoFinalizacion',
      LOWER(CONVERT(VARCHAR(64),HASHBYTES('SHA2_256',CONVERT(NVARCHAR(MAX),N'')),2)),GETDATE());
 END;
 -- Se permite el mantenimiento del DVH; los datos de negocio al momento de emisión son inmutables incluso para el SQL de la aplicación.
 EXEC(N'CREATE OR ALTER TRIGGER dbo.TR_Certificado_Inmutable ON dbo.CertificadoFinalizacion AFTER UPDATE,DELETE AS
 BEGIN
   SET NOCOUNT ON;
   IF NOT EXISTS(SELECT 1 FROM inserted) OR UPDATE(IdInscripcion) OR UPDATE(Nombre) OR UPDATE(Apellido) OR UPDATE(Curso)
      OR UPDATE(Promedio) OR UPDATE(Firmante) OR UPDATE(Cargo) OR UPDATE(Folio) OR UPDATE(Fecha) OR UPDATE(Modalidad)
      OR UPDATE(Email) OR UPDATE(Mencion) OR UPDATE(Observaciones)
       THROW 52915,N''Los certificados emitidos son inmutables.'',1;
 END');
 COMMIT TRANSACTION;
END TRY
BEGIN CATCH
 IF @@TRANCOUNT>0 ROLLBACK TRANSACTION;
 THROW;
END CATCH;

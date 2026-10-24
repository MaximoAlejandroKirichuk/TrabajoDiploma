-- Migración aditiva de CUN08. Crear una copia de seguridad de la base de datos y detener primero las operaciones de escritura de la aplicación.
-- No se reescriben las asignaciones de permisos, las inscripciones, los pagos ni los hashes de resultados existentes.
SET NOCOUNT ON;
SET XACT_ABORT ON;
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
BEGIN TRY
    BEGIN TRANSACTION;
    IF OBJECT_ID(N'dbo.Comision',N'U') IS NULL OR OBJECT_ID(N'dbo.SolicitudInscripcion',N'U') IS NULL
        OR OBJECT_ID(N'dbo.Profesor',N'U') IS NULL OR OBJECT_ID(N'dbo.Usuarios',N'U') IS NULL
        OR OBJECT_ID(N'dbo.Patentes',N'U') IS NULL OR OBJECT_ID(N'dbo.DigitoVerificador_83KI',N'U') IS NULL
        OR COL_LENGTH(N'dbo.Cuota',N'BalanceAdeudado') IS NULL OR COL_LENGTH(N'dbo.Cuota',N'TipoConcepto') IS NULL
        THROW 52810,N'CUN08 prerequisites missing. Apply the base schema and financing installer first.',1;
    IF EXISTS(SELECT 1 FROM dbo.Patentes WHERE CodigoPatente=71 AND Nombre COLLATE Latin1_General_100_BIN2<>N'Registrar resultados de evaluación')
        THROW 52811,N'Patent 71 is already used by another capability.',1;

    IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.SolicitudInscripcion') AND name=N'UX_CUN08_InscripcionComision')
        CREATE UNIQUE INDEX UX_CUN08_InscripcionComision ON dbo.SolicitudInscripcion(IdSolicitudInscripcion,IdComision);
    IF NOT EXISTS(SELECT 1 FROM sys.indexes i WHERE i.object_id=OBJECT_ID(N'dbo.SolicitudInscripcion') AND i.name=N'UX_CUN08_InscripcionComision'
        AND i.is_unique=1 AND i.is_disabled=0 AND i.has_filter=0
        AND (SELECT COUNT(*) FROM sys.index_columns ic WHERE ic.object_id=i.object_id AND ic.index_id=i.index_id AND ic.key_ordinal>0)=2
        AND EXISTS(SELECT 1 FROM sys.index_columns ic JOIN sys.columns c ON c.object_id=ic.object_id AND c.column_id=ic.column_id
            WHERE ic.object_id=i.object_id AND ic.index_id=i.index_id AND ic.key_ordinal=1 AND c.name=N'IdSolicitudInscripcion')
        AND EXISTS(SELECT 1 FROM sys.index_columns ic JOIN sys.columns c ON c.object_id=ic.object_id AND c.column_id=ic.column_id
            WHERE ic.object_id=i.object_id AND ic.index_id=i.index_id AND ic.key_ordinal=2 AND c.name=N'IdComision'))
        THROW 52812,N'Incompatible CUN08 enrollment/commission index.',1;
    IF OBJECT_ID(N'dbo.ResultadoEvaluacion',N'U') IS NULL
    BEGIN
        CREATE TABLE dbo.ResultadoEvaluacion(
            IdResultadoEvaluacion INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_ResultadoEvaluacion PRIMARY KEY,
            IdComision INT NOT NULL,
            IdInscripcion INT NOT NULL,
            FechaEvaluacion DATE NOT NULL,
            Calificacion DECIMAL(4,2) NOT NULL,
            Observaciones NVARCHAR(500) NOT NULL CONSTRAINT DF_ResultadoEvaluacion_Observaciones DEFAULT(N''),
            CondicionFinal NVARCHAR(20) NOT NULL,
            DVH VARCHAR(64) NOT NULL CONSTRAINT DF_ResultadoEvaluacion_DVH DEFAULT(''),
            CONSTRAINT FK_ResultadoEvaluacion_Comision FOREIGN KEY(IdComision) REFERENCES dbo.Comision(IdComision),
            CONSTRAINT FK_ResultadoEvaluacion_Inscripcion FOREIGN KEY(IdInscripcion,IdComision)
                REFERENCES dbo.SolicitudInscripcion(IdSolicitudInscripcion,IdComision),
            CONSTRAINT UQ_ResultadoEvaluacion_InscripcionFecha UNIQUE(IdInscripcion,FechaEvaluacion),
            CONSTRAINT CK_ResultadoEvaluacion_Calificacion CHECK(Calificacion BETWEEN 1 AND 10),
            CONSTRAINT CK_ResultadoEvaluacion_Condicion CHECK(
                (Calificacion>=7 AND CondicionFinal COLLATE Latin1_General_100_BIN2=N'Aprobado') OR
                (Calificacion<7 AND CondicionFinal COLLATE Latin1_General_100_BIN2=N'Desaprobado'))
        );
    END;
    -- Rechazar los esquemas preexistentes incompatibles en lugar de aceptarlos silenciosamente o reescribirlos.
    IF (SELECT COUNT(*) FROM sys.columns WHERE object_id=OBJECT_ID(N'dbo.ResultadoEvaluacion'))<>8
        OR EXISTS(SELECT 1 FROM (VALUES
            (N'IdResultadoEvaluacion',N'int',4,0,0),(N'IdComision',N'int',4,0,0),(N'IdInscripcion',N'int',4,0,0),
            (N'FechaEvaluacion',N'date',3,0,0),(N'Calificacion',N'decimal',5,4,2),
            (N'Observaciones',N'nvarchar',1000,0,0),(N'CondicionFinal',N'nvarchar',40,0,0),(N'DVH',N'varchar',64,0,0)
        ) e(n,t,l,p,s) LEFT JOIN sys.columns c ON c.object_id=OBJECT_ID(N'dbo.ResultadoEvaluacion') AND c.name=e.n
            WHERE c.column_id IS NULL OR TYPE_NAME(c.user_type_id)<>e.t OR c.max_length<>e.l OR c.is_nullable=1
            OR (e.p>0 AND (c.precision<>e.p OR c.scale<>e.s)))
        OR COLUMNPROPERTY(OBJECT_ID(N'dbo.ResultadoEvaluacion'),N'IdResultadoEvaluacion','IsIdentity')<>1
        OR NOT EXISTS(SELECT 1 FROM sys.key_constraints WHERE parent_object_id=OBJECT_ID(N'dbo.ResultadoEvaluacion') AND name=N'UQ_ResultadoEvaluacion_InscripcionFecha')
        OR (SELECT COUNT(*) FROM sys.foreign_keys WHERE parent_object_id=OBJECT_ID(N'dbo.ResultadoEvaluacion')
            AND name IN(N'FK_ResultadoEvaluacion_Comision',N'FK_ResultadoEvaluacion_Inscripcion') AND is_disabled=0 AND is_not_trusted=0)<>2
        OR (SELECT COUNT(*) FROM sys.check_constraints WHERE parent_object_id=OBJECT_ID(N'dbo.ResultadoEvaluacion')
            AND name IN(N'CK_ResultadoEvaluacion_Calificacion',N'CK_ResultadoEvaluacion_Condicion') AND is_disabled=0 AND is_not_trusted=0)<>2
        THROW 52812,N'Incompatible ResultadoEvaluacion schema. Review it before migration.',1;

    IF NOT EXISTS(SELECT 1 FROM dbo.Patentes WHERE CodigoPatente=71)
    BEGIN
        SET IDENTITY_INSERT dbo.Patentes ON;
        INSERT dbo.Patentes(CodigoPatente,Nombre,DVH)
        VALUES(71,N'Registrar resultados de evaluación',LOWER(CONVERT(VARCHAR(64),HASHBYTES('SHA2_256',
            CONVERT(NVARCHAR(MAX),N'CodigoPatente=71|Nombre=Registrar resultados de evaluación')),2)));
        SET IDENTITY_INSERT dbo.Patentes OFF;
        DECLARE @patentes NVARCHAR(MAX)=COALESCE((SELECT CONVERT(NVARCHAR(MAX),DVH) AS [text()]
            FROM dbo.Patentes ORDER BY DVH FOR XML PATH(''),TYPE).value('.','nvarchar(max)'),N'');
        UPDATE dbo.DigitoVerificador_83KI SET DVV=LOWER(CONVERT(VARCHAR(64),HASHBYTES('SHA2_256',@patentes),2)),FechaActualizacion=GETDATE()
            WHERE NombreTabla=N'Patentes';
        IF @@ROWCOUNT<>1 THROW 52813,N'Patent integrity registration missing.',1;
    END;
    IF NOT EXISTS(SELECT 1 FROM dbo.DigitoVerificador_83KI WHERE NombreTabla=N'ResultadoEvaluacion')
    BEGIN
        IF EXISTS(SELECT 1 FROM dbo.ResultadoEvaluacion) THROW 52814,N'Existing results require an integrity audit, not a backfill.',1;
        INSERT dbo.DigitoVerificador_83KI(NombreTabla,DVV,FechaActualizacion)
        VALUES(N'ResultadoEvaluacion',LOWER(CONVERT(VARCHAR(64),HASHBYTES('SHA2_256',CONVERT(NVARCHAR(MAX),N'')),2)),GETDATE());
    END;
    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT>0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;

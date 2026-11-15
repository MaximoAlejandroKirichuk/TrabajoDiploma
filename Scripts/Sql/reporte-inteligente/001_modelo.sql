-- Ejecutar sobre la base de la aplicación, con la aplicación cerrada.
SET XACT_ABORT ON;
IF OBJECT_ID(N'dbo.ResultadoEvaluacion',N'U') IS NULL OR COL_LENGTH(N'dbo.Comision',N'FechaFin') IS NULL
    THROW 53010,N'Apply the existing CUN03/CUN06/CUN07/CUN08/CUN09 and financing migrations first.',1;
IF COL_LENGTH(N'dbo.Curso',N'CupoMinimoReferencia') IS NULL
    ALTER TABLE dbo.Curso ADD CupoMinimoReferencia int NULL;
GO
IF NOT EXISTS(SELECT 1 FROM sys.check_constraints WHERE name=N'CK_Curso_CupoReferencia')
    ALTER TABLE dbo.Curso ADD CONSTRAINT CK_Curso_CupoReferencia CHECK(CupoMinimoReferencia IS NULL OR CupoMinimoReferencia>0);
IF OBJECT_ID(N'dbo.RecorridoCurso',N'U') IS NULL
BEGIN
    CREATE TABLE dbo.RecorridoCurso(
        IdRecorridoCurso int IDENTITY(1,1) NOT NULL CONSTRAINT PK_RecorridoCurso PRIMARY KEY,
        IdCursoPrevio int NOT NULL CONSTRAINT FK_Recorrido_Previo REFERENCES dbo.Curso(IdCurso),
        IdCursoRecomendado int NOT NULL CONSTRAINT FK_Recorrido_Recomendado REFERENCES dbo.Curso(IdCurso),
        Orden int NOT NULL, DVH varchar(64) NOT NULL CONSTRAINT DF_Recorrido_DVH DEFAULT(''),
        CONSTRAINT UQ_Recorrido_Par UNIQUE(IdCursoPrevio,IdCursoRecomendado),
        CONSTRAINT CK_Recorrido_Distinto CHECK(IdCursoPrevio<>IdCursoRecomendado),
        CONSTRAINT CK_Recorrido_Orden CHECK(Orden>0));
END;
GO
-- Hashes canónicos UTF-16 compatibles con IntegridadTablaConfig_83KI e IntegridadDAL_83KI.
-- El procedimiento admite únicamente las tablas previstas, no identificadores SQL arbitrarios.
CREATE OR ALTER PROCEDURE dbo.sp_Inteligente_ActualizarIntegridad @Tabla sysname,@SoloVacios bit=0
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @expr nvarchar(max),@sql nvarchar(max),@all nvarchar(max),@dvv varchar(64);
    SET @expr=CASE @Tabla
    WHEN N'Curso' THEN N'N''CargaHoraria=''+CONVERT(nvarchar(20),CargaHoraria)+N''|CupoMinimoReferencia=''+COALESCE(CONVERT(nvarchar(20),CupoMinimoReferencia),N''∅'')+N''|Descripcion=''+COALESCE(LTRIM(RTRIM(Descripcion)),N''∅'')+N''|EstadoActivo=''+CONVERT(nvarchar(1),EstadoActivo)+N''|IdCurso=''+CONVERT(nvarchar(20),IdCurso)+N''|Nombre=''+LTRIM(RTRIM(Nombre))'
    WHEN N'RecorridoCurso' THEN N'N''IdCursoPrevio=''+CONVERT(nvarchar(20),IdCursoPrevio)+N''|IdCursoRecomendado=''+CONVERT(nvarchar(20),IdCursoRecomendado)+N''|IdRecorridoCurso=''+CONVERT(nvarchar(20),IdRecorridoCurso)+N''|Orden=''+CONVERT(nvarchar(20),Orden)'
    WHEN N'Patentes' THEN N'N''CodigoPatente=''+CONVERT(nvarchar(20),CodigoPatente)+N''|Nombre=''+LTRIM(RTRIM(Nombre))'
    WHEN N'RolPatente' THEN N'N''CodigoPatente=''+CONVERT(nvarchar(20),CodigoPatente)+N''|CodigoRol=''+CONVERT(nvarchar(20),CodigoRol)'
    END;
    IF @expr IS NULL AND @Tabla IN(N'Profesor',N'DisponibilidadProfesor',N'Comision',N'Alumno',N'SolicitudInscripcion',N'Cuota',N'PagoInscripcion',N'ResultadoEvaluacion',N'Lead',N'ConsultaLead')
    BEGIN
        SELECT @expr=STUFF((SELECT N'+N''|'+c.name+N'=''+COALESCE('+CASE
            WHEN TYPE_NAME(c.user_type_id) IN(N'date',N'datetime',N'datetime2',N'smalldatetime') THEN N'CONVERT(nvarchar(23),CONVERT(datetime,'+QUOTENAME(c.name)+N'),126)'
            WHEN TYPE_NAME(c.user_type_id)=N'time' THEN N'CONVERT(nvarchar(8),'+QUOTENAME(c.name)+N')'
            ELSE N'LTRIM(RTRIM(CONVERT(nvarchar(max),'+QUOTENAME(c.name)+N')))'
            END+N',N''∅'')'
            FROM sys.columns c WHERE c.object_id=OBJECT_ID(N'dbo.'+@Tabla) AND c.name<>N'DVH'
            ORDER BY c.name COLLATE Latin1_General_100_BIN2 FOR XML PATH(''),TYPE).value('.','nvarchar(max)'),1,1,N'');
        SET @expr=STUFF(@expr,3,1,N'');
        -- El estilo 126 puede omitir .000; se explicita el formato con milisegundos fijos.
        SELECT @expr=REPLACE(@expr,N'CONVERT(nvarchar(23),CONVERT(datetime,'+QUOTENAME(c.name)+N'),126)',
            N'REPLACE(CONVERT(nvarchar(23),CONVERT(datetime,'+QUOTENAME(c.name)+N'),121),N'' '',N''T'')')
        FROM sys.columns c WHERE c.object_id=OBJECT_ID(N'dbo.'+@Tabla) AND TYPE_NAME(c.user_type_id) IN(N'date',N'datetime',N'datetime2',N'smalldatetime');
    END;
    IF @expr IS NULL THROW 53011,N'Unsupported integrity table.',1;
    SET @sql=N'UPDATE dbo.'+QUOTENAME(@Tabla)+N' WITH(TABLOCKX) SET DVH=LOWER(CONVERT(varchar(64),HASHBYTES(''SHA2_256'',CONVERT(nvarchar(max),'+@expr+N')),2))'+CASE WHEN @SoloVacios=1 THEN N' WHERE DVH=''''' ELSE N'' END+N';
        SELECT @all=COALESCE((SELECT DVH AS [text()] FROM dbo.'+QUOTENAME(@Tabla)+N' ORDER BY DVH COLLATE Latin1_General_100_BIN2 FOR XML PATH(''''),TYPE).value(''.'',''nvarchar(max)''),N'''');';
    EXEC sys.sp_executesql @sql,N'@all nvarchar(max) OUTPUT',@all OUTPUT;
    SET @dvv=LOWER(CONVERT(varchar(64),HASHBYTES('SHA2_256',@all),2));
    UPDATE dbo.DigitoVerificador_83KI SET DVV=@dvv,FechaActualizacion=GETDATE() WHERE NombreTabla=@Tabla;
    IF @@ROWCOUNT=0 INSERT dbo.DigitoVerificador_83KI(NombreTabla,DVV,FechaActualizacion) VALUES(@Tabla,@dvv,GETDATE());
END;
GO
BEGIN TRANSACTION;
IF EXISTS(SELECT 1 FROM dbo.Patentes WHERE (CodigoPatente=77 AND Nombre<>N'Ver reporte inteligente') OR (CodigoPatente=78 AND Nombre<>N'Gestionar recorridos'))
    THROW 53012,N'Permission codes 77/78 are already occupied. Review before installing.',1;
SET IDENTITY_INSERT dbo.Patentes ON;
INSERT dbo.Patentes(CodigoPatente,Nombre,DVH)
SELECT v.Id,v.Nombre,'' FROM (VALUES(77,N'Ver reporte inteligente'),(78,N'Gestionar recorridos'))v(Id,Nombre)
WHERE NOT EXISTS(SELECT 1 FROM dbo.Patentes p WHERE p.CodigoPatente=v.Id);
SET IDENTITY_INSERT dbo.Patentes OFF;
INSERT dbo.RolPatente(CodigoRol,CodigoPatente,DVH)
SELECT r.CodigoRol,p.CodigoPatente,'' FROM dbo.Roles r CROSS JOIN dbo.Patentes p
WHERE r.Nombre COLLATE Latin1_General_100_CI_AI LIKE N'%Coordinador%Acad%'
AND p.CodigoPatente IN(77,78) AND NOT EXISTS(SELECT 1 FROM dbo.RolPatente rp WHERE rp.CodigoRol=r.CodigoRol AND rp.CodigoPatente=p.CodigoPatente);
IF NOT EXISTS(SELECT 1 FROM dbo.Roles WHERE Nombre COLLATE Latin1_General_100_CI_AI LIKE N'%Coordinador%Acad%')
    PRINT N'No academic coordinator role found. Assign permissions 77/78 through role management before opening the screens.';
EXEC dbo.sp_Inteligente_ActualizarIntegridad N'Curso';
EXEC dbo.sp_Inteligente_ActualizarIntegridad N'RecorridoCurso';
EXEC dbo.sp_Inteligente_ActualizarIntegridad N'Patentes';
EXEC dbo.sp_Inteligente_ActualizarIntegridad N'RolPatente';
COMMIT;
GO

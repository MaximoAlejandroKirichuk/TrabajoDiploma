/* Auditoría histórica de disponibilidad de profesores. Ejecución repetible. */
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

/* Migra el nombre anterior de la tabla sin perder identificadores ni historial. */
IF OBJECT_ID(N'dbo.DisponibilidadProfesor_C', N'U') IS NULL
   AND OBJECT_ID(N'dbo.DisponibilidadProfesor_Historial', N'U') IS NOT NULL
    EXEC sys.sp_rename N'dbo.DisponibilidadProfesor_Historial', N'DisponibilidadProfesor_C';
GO
IF OBJECT_ID(N'dbo.DisponibilidadProfesor_C', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.DisponibilidadProfesor_C
    (
        IdVersion BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_DisponibilidadProfesor_C PRIMARY KEY,
        IdDisponibilidadProfesor INT NOT NULL,
        IdProfesor INT NOT NULL,
        DiaSemana NVARCHAR(20) NOT NULL,
        HoraInicio TIME NOT NULL,
        HoraFin TIME NOT NULL,
        EstadoActivo BIT NOT NULL,
        DVH VARCHAR(64) NOT NULL,
        Act BIT NOT NULL CONSTRAINT DF_DisponibilidadProfesor_C_Act DEFAULT (1),
        FechaRegistro DATETIME2(0) NOT NULL CONSTRAINT DF_DisponibilidadProfesor_C_Fecha DEFAULT (SYSDATETIME()),
         CONSTRAINT FK_DisponibilidadProfesor_C_Profesor FOREIGN KEY (IdProfesor) REFERENCES dbo.Profesor(IdProfesor),
         CONSTRAINT FK_DisponibilidadProfesor_C_Disponibilidad FOREIGN KEY (IdDisponibilidadProfesor) REFERENCES dbo.DisponibilidadProfesor(IdDisponibilidadProfesor),
        CONSTRAINT CK_DisponibilidadProfesor_C_Horario CHECK (HoraInicio < HoraFin)
    );
END;
GO
/* Las versiones anteriores no registraban operación ni usuario; no se inventa su origen. */
IF COL_LENGTH(N'dbo.DisponibilidadProfesor_C', N'TipoOperacion') IS NULL
    ALTER TABLE dbo.DisponibilidadProfesor_C ADD TipoOperacion NVARCHAR(24) NOT NULL
        CONSTRAINT DF_DisponibilidadProfesor_C_Operacion DEFAULT (N'SIN_CLASIFICAR') WITH VALUES;
IF COL_LENGTH(N'dbo.DisponibilidadProfesor_C', N'Usuario') IS NULL
    ALTER TABLE dbo.DisponibilidadProfesor_C ADD Usuario NVARCHAR(100) NULL;
GO
/* Actualiza tablas creadas por versiones anteriores de este script. */
IF COL_LENGTH(N'dbo.DisponibilidadProfesor_C', N'DVH') IS NULL
    ALTER TABLE dbo.DisponibilidadProfesor_C ADD DVH VARCHAR(64) NOT NULL CONSTRAINT DF_DisponibilidadProfesor_C_DVH DEFAULT ('');
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_DisponibilidadProfesor_C_Profesor' AND parent_object_id = OBJECT_ID(N'dbo.DisponibilidadProfesor_C'))
    ALTER TABLE dbo.DisponibilidadProfesor_C WITH CHECK ADD CONSTRAINT FK_DisponibilidadProfesor_C_Profesor FOREIGN KEY (IdProfesor) REFERENCES dbo.Profesor(IdProfesor);
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_DisponibilidadProfesor_C_Disponibilidad' AND parent_object_id = OBJECT_ID(N'dbo.DisponibilidadProfesor_C'))
    ALTER TABLE dbo.DisponibilidadProfesor_C WITH CHECK ADD CONSTRAINT FK_DisponibilidadProfesor_C_Disponibilidad FOREIGN KEY (IdDisponibilidadProfesor) REFERENCES dbo.DisponibilidadProfesor(IdDisponibilidadProfesor);
IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = N'CK_DisponibilidadProfesor_C_Horario' AND parent_object_id = OBJECT_ID(N'dbo.DisponibilidadProfesor_C'))
    ALTER TABLE dbo.DisponibilidadProfesor_C WITH CHECK ADD CONSTRAINT CK_DisponibilidadProfesor_C_Horario CHECK (HoraInicio < HoraFin);
/* La activación desmarca la versión anterior antes de seleccionar su reemplazo. */
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_DisponibilidadProfesor_C_Activa' AND object_id = OBJECT_ID(N'dbo.DisponibilidadProfesor_C'))
    CREATE UNIQUE INDEX UX_DisponibilidadProfesor_C_Activa ON dbo.DisponibilidadProfesor_C(IdDisponibilidadProfesor) WHERE Act = 1;
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_DisponibilidadProfesor_C_Filtro' AND object_id = OBJECT_ID(N'dbo.DisponibilidadProfesor_C'))
    CREATE INDEX IX_DisponibilidadProfesor_C_Filtro ON dbo.DisponibilidadProfesor_C(IdProfesor, IdDisponibilidadProfesor, FechaRegistro);
GO

/* Conserva los campos de la tabla principal, incluido el valor de integridad almacenado. */
DECLARE @FechaMigracionDisponibilidad DATETIME2(0) = SYSDATETIME();
INSERT dbo.DisponibilidadProfesor_C
    (IdDisponibilidadProfesor, IdProfesor, DiaSemana, HoraInicio, HoraFin, EstadoActivo, DVH, Act, FechaRegistro, TipoOperacion, Usuario)
SELECT d.IdDisponibilidadProfesor, d.IdProfesor, d.DiaSemana, d.HoraInicio, d.HoraFin, d.EstadoActivo, d.DVH, 1, @FechaMigracionDisponibilidad, N'INICIALIZACION', NULL
FROM dbo.DisponibilidadProfesor d
WHERE NOT EXISTS (SELECT 1 FROM dbo.DisponibilidadProfesor_C h WHERE h.IdDisponibilidadProfesor = d.IdDisponibilidadProfesor AND h.Act = 1);
GO

CREATE OR ALTER TRIGGER dbo.TR_DisponibilidadProfesor_Auditoria
ON dbo.DisponibilidadProfesor
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Profesores TABLE (IdProfesor INT PRIMARY KEY);
    INSERT @Profesores SELECT IdProfesor FROM deleted UNION SELECT IdProfesor FROM inserted;
    /* Bloquea los profesores en orden de clave para coordinar ambas clases de escritura. */
    DECLARE @LockId INT;
    DECLARE lock_cursor CURSOR LOCAL FAST_FORWARD FOR SELECT IdProfesor FROM @Profesores ORDER BY IdProfesor;
    OPEN lock_cursor;
    FETCH NEXT FROM lock_cursor INTO @LockId;
    WHILE @@FETCH_STATUS = 0
    BEGIN
        SELECT @LockId = IdProfesor FROM dbo.Profesor WITH (UPDLOCK, HOLDLOCK) WHERE IdProfesor = @LockId;
        FETCH NEXT FROM lock_cursor INTO @LockId;
    END;
    CLOSE lock_cursor;
    DEALLOCATE lock_cursor;

    DECLARE @Afectadas TABLE (IdDisponibilidadProfesor INT PRIMARY KEY);
    INSERT @Afectadas
    SELECT i.IdDisponibilidadProfesor
    FROM inserted i LEFT JOIN deleted d ON d.IdDisponibilidadProfesor = i.IdDisponibilidadProfesor
    WHERE d.IdDisponibilidadProfesor IS NULL
       OR i.IdProfesor <> d.IdProfesor OR i.DiaSemana <> d.DiaSemana
       OR i.HoraInicio <> d.HoraInicio OR i.HoraFin <> d.HoraFin
       OR i.EstadoActivo <> d.EstadoActivo;

    /* Los cambios exclusivos de DVH y las actualizaciones sin diferencias no generan versiones. */
    IF NOT EXISTS (SELECT 1 FROM @Afectadas) RETURN;

    DECLARE @RestaurarVersion BIGINT = TRY_CONVERT(BIGINT, SESSION_CONTEXT(N'DisponibilidadProfesorVersionRestaurada'));
    IF @RestaurarVersion IS NULL
    BEGIN
        UPDATE h SET Act = 0
        FROM dbo.DisponibilidadProfesor_C h JOIN @Afectadas x ON x.IdDisponibilidadProfesor = h.IdDisponibilidadProfesor
        WHERE h.Act = 1;

        INSERT dbo.DisponibilidadProfesor_C
            (IdDisponibilidadProfesor, IdProfesor, DiaSemana, HoraInicio, HoraFin, EstadoActivo, DVH, Act, FechaRegistro, TipoOperacion, Usuario)
        SELECT i.IdDisponibilidadProfesor, i.IdProfesor, i.DiaSemana, i.HoraInicio, i.HoraFin, i.EstadoActivo,
               LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', CONCAT(
                   N'DiaSemana=', LTRIM(RTRIM(i.DiaSemana)), N'|', N'EstadoActivo=', CONVERT(NVARCHAR(1), i.EstadoActivo), N'|',
                   N'HoraFin=', CONVERT(NVARCHAR(8), i.HoraFin), N'|', N'HoraInicio=', CONVERT(NVARCHAR(8), i.HoraInicio), N'|',
                   N'IdDisponibilidadProfesor=', CONVERT(NVARCHAR(128), i.IdDisponibilidadProfesor), N'|',
                     N'IdProfesor=', CONVERT(NVARCHAR(128), i.IdProfesor))), 2)), 1, SYSDATETIME(),
               CASE WHEN d.IdDisponibilidadProfesor IS NULL THEN N'INSERT'
                    WHEN d.EstadoActivo = 1 AND i.EstadoActivo = 0 THEN N'BAJA_LOGICA'
                    WHEN d.EstadoActivo = 0 AND i.EstadoActivo = 1 THEN N'REACTIVACION'
                    ELSE N'UPDATE' END,
               NULLIF(CONVERT(NVARCHAR(100), SESSION_CONTEXT(N'DisponibilidadProfesorUsuario')), N'')
        FROM inserted i JOIN @Afectadas x ON x.IdDisponibilidadProfesor = i.IdDisponibilidadProfesor
        LEFT JOIN deleted d ON d.IdDisponibilidadProfesor = i.IdDisponibilidadProfesor;
    END;

    /* Valida el estado resultante completo, incluidas todas las filas modificadas. */
    DECLARE @Codigo NVARCHAR(30), @Hora TIME, @Mensaje NVARCHAR(2048);
    SELECT TOP (1) @Codigo = co.Codigo, @Hora = co.HoraInicio
    FROM @Afectadas x
    JOIN dbo.Comision co ON EXISTS (SELECT 1 FROM @Profesores p WHERE p.IdProfesor = co.IdProfesor)
    WHERE co.Estado IN (N'preapertura', N'confirmada') AND co.FechaFin >= CONVERT(date, GETDATE())
      AND NOT EXISTS
      (SELECT 1 FROM dbo.DisponibilidadProfesor a
       WHERE a.IdProfesor = co.IdProfesor AND a.EstadoActivo = 1 AND a.DiaSemana = co.DiaSemana
           AND a.HoraInicio <= co.HoraInicio AND a.HoraFin >= co.HoraFin)
    ORDER BY co.Codigo;
    IF @Codigo IS NOT NULL
    BEGIN
        SET @Mensaje = CONCAT(N'Errores.DisponibilidadProfesorCompromisoComision: ', @Codigo, N' (', CONVERT(NVARCHAR(5), @Hora, 108), N').');
        THROW 52801, @Mensaje, 1;
    END;

END;
GO

/* La activación histórica sincroniza una sola vez los datos de la disponibilidad vigente. */
CREATE OR ALTER TRIGGER dbo.TR_DisponibilidadProfesor_C_Activar
ON dbo.DisponibilidadProfesor_C
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT UPDATE(Act) RETURN;
    DECLARE @Seleccionada TABLE (IdVersion BIGINT PRIMARY KEY, IdDisponibilidadProfesor INT, IdProfesor INT,
        DiaSemana NVARCHAR(20), HoraInicio TIME, HoraFin TIME, EstadoActivo BIT, DVH VARCHAR(64));
    INSERT @Seleccionada
    SELECT i.IdVersion, i.IdDisponibilidadProfesor, i.IdProfesor, i.DiaSemana, i.HoraInicio, i.HoraFin, i.EstadoActivo, i.DVH
    FROM inserted i JOIN deleted d ON d.IdVersion=i.IdVersion
    WHERE d.Act=0 AND i.Act=1;
    IF NOT EXISTS (SELECT 1 FROM @Seleccionada) RETURN;
    IF (SELECT COUNT(*) FROM @Seleccionada) <> 1
        THROW 52802, N'Errores.DisponibilidadProfesorActivacionIndividual', 1;
    DECLARE @Profesor INT = (SELECT IdProfesor FROM @Seleccionada);
    SELECT @Profesor = IdProfesor FROM dbo.Profesor WITH (UPDLOCK, HOLDLOCK) WHERE IdProfesor = @Profesor;
    IF EXISTS (SELECT 1 FROM @Seleccionada s JOIN dbo.DisponibilidadProfesor d
               ON d.IdProfesor=s.IdProfesor AND d.IdDisponibilidadProfesor<>s.IdDisponibilidadProfesor
               AND d.EstadoActivo=1 AND s.EstadoActivo=1 AND d.DiaSemana=s.DiaSemana
               AND d.HoraInicio=s.HoraInicio AND d.HoraFin=s.HoraFin)
        THROW 52803, N'Errores.DisponibilidadProfesorDuplicada', 1;
    DECLARE @VersionSeleccionada BIGINT = (SELECT IdVersion FROM @Seleccionada);
    EXEC sys.sp_set_session_context @key=N'DisponibilidadProfesorVersionRestaurada', @value=@VersionSeleccionada;
    BEGIN TRY
        UPDATE d SET IdProfesor=s.IdProfesor, DiaSemana=s.DiaSemana, HoraInicio=s.HoraInicio,
            HoraFin=s.HoraFin, EstadoActivo=s.EstadoActivo, DVH=s.DVH
        FROM dbo.DisponibilidadProfesor d JOIN @Seleccionada s ON s.IdDisponibilidadProfesor=d.IdDisponibilidadProfesor;
        EXEC sys.sp_set_session_context @key=N'DisponibilidadProfesorVersionRestaurada', @value=NULL;
    END TRY
    BEGIN CATCH
        EXEC sys.sp_set_session_context @key=N'DisponibilidadProfesorVersionRestaurada', @value=NULL;
        THROW;
    END CATCH;
END;
GO

CREATE OR ALTER TRIGGER dbo.TR_Comision_DisponibilidadProfesor
ON dbo.Comision
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Profesores TABLE (IdProfesor INT PRIMARY KEY);
    INSERT @Profesores SELECT IdProfesor FROM deleted UNION SELECT IdProfesor FROM inserted;
    DECLARE @LockId INT;
    DECLARE lock_cursor CURSOR LOCAL FAST_FORWARD FOR SELECT IdProfesor FROM @Profesores ORDER BY IdProfesor;
    OPEN lock_cursor;
    FETCH NEXT FROM lock_cursor INTO @LockId;
    WHILE @@FETCH_STATUS = 0
    BEGIN
        SELECT @LockId = IdProfesor FROM dbo.Profesor WITH (UPDLOCK, HOLDLOCK) WHERE IdProfesor = @LockId;
        FETCH NEXT FROM lock_cursor INTO @LockId;
    END;
    CLOSE lock_cursor;
    DEALLOCATE lock_cursor;

    DECLARE @Codigo NVARCHAR(30), @Hora TIME, @Mensaje NVARCHAR(2048);
    SELECT TOP (1) @Codigo = co.Codigo, @Hora = co.HoraInicio
    FROM inserted co
    WHERE co.Estado IN (N'preapertura', N'confirmada') AND co.FechaFin >= CONVERT(date, GETDATE())
      AND NOT EXISTS (SELECT 1 FROM dbo.DisponibilidadProfesor a WHERE a.IdProfesor = co.IdProfesor AND a.EstadoActivo = 1
        AND a.DiaSemana = co.DiaSemana AND a.HoraInicio <= co.HoraInicio AND a.HoraFin >= co.HoraFin)
    ORDER BY co.Codigo;
    IF @Codigo IS NOT NULL
    BEGIN
        SET @Mensaje = CONCAT(N'Errores.DisponibilidadProfesorCompromisoComision: ', @Codigo, N' (', CONVERT(NVARCHAR(5), @Hora, 108), N').');
        THROW 52801, @Mensaje, 1;
    END;
END;
GO

IF OBJECT_ID(N'dbo.sp_DisponibilidadProfesor_ListarHistorial', N'P') IS NULL
    EXEC(N'CREATE PROCEDURE dbo.sp_DisponibilidadProfesor_ListarHistorial AS RETURN 0;');
GO
ALTER PROCEDURE dbo.sp_DisponibilidadProfesor_ListarHistorial
    @IdProfesor INT, @IdDisponibilidadProfesor INT = NULL, @FechaDesde DATE = NULL, @FechaHasta DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT IdVersion, IdDisponibilidadProfesor, IdProfesor, DiaSemana, HoraInicio, HoraFin, EstadoActivo, DVH, Act, FechaRegistro, TipoOperacion, Usuario
    FROM dbo.DisponibilidadProfesor_C
    WHERE IdProfesor = @IdProfesor AND (@IdDisponibilidadProfesor IS NULL OR IdDisponibilidadProfesor = @IdDisponibilidadProfesor)
      AND (@FechaDesde IS NULL OR FechaRegistro >= @FechaDesde)
      AND (@FechaHasta IS NULL OR FechaRegistro < DATEADD(day, 1, @FechaHasta))
    ORDER BY IdDisponibilidadProfesor, FechaRegistro DESC, IdVersion DESC;
END;
GO

IF OBJECT_ID(N'dbo.sp_DisponibilidadProfesor_ActivarVersion', N'P') IS NULL
    EXEC(N'CREATE PROCEDURE dbo.sp_DisponibilidadProfesor_ActivarVersion AS RETURN 0;');
GO
ALTER PROCEDURE dbo.sp_DisponibilidadProfesor_ActivarVersion @IdVersion BIGINT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @InicioTran INT = @@TRANCOUNT, @IdDisponibilidadProfesor INT, @IdProfesor INT,
            @DiaSemana NVARCHAR(20), @HoraInicio TIME, @HoraFin TIME, @EstadoActivo BIT, @DVH VARCHAR(64);
    IF @InicioTran = 0 BEGIN TRANSACTION;
    ELSE SAVE TRANSACTION ActivarDisponibilidad;
    BEGIN TRY
        SELECT @IdDisponibilidadProfesor = IdDisponibilidadProfesor, @IdProfesor = IdProfesor,
               @DiaSemana = DiaSemana, @HoraInicio = HoraInicio, @HoraFin = HoraFin,
               @EstadoActivo = EstadoActivo, @DVH = DVH
        FROM dbo.DisponibilidadProfesor_C WITH (UPDLOCK, HOLDLOCK) WHERE IdVersion = @IdVersion;
        IF @IdDisponibilidadProfesor IS NULL THROW 52802, N'Errores.DisponibilidadProfesorVersionNoEncontrada', 1;

        /* El trigger histórico sincroniza la fila actual sin crear otra versión. */
        UPDATE dbo.DisponibilidadProfesor_C SET Act = 0
        WHERE IdDisponibilidadProfesor = @IdDisponibilidadProfesor AND Act = 1 AND IdVersion <> @IdVersion;
        UPDATE dbo.DisponibilidadProfesor_C SET Act = 1 WHERE IdVersion = @IdVersion AND Act = 0;

        IF @InicioTran = 0 COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        EXEC sys.sp_set_session_context @key=N'DisponibilidadProfesorVersionRestaurada', @value=NULL;
        IF XACT_STATE() = 1
        BEGIN
            IF @InicioTran = 0 ROLLBACK TRANSACTION;
            ELSE ROLLBACK TRANSACTION ActivarDisponibilidad;
        END
        ELSE IF XACT_STATE() = -1 AND @InicioTran = 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;
GO

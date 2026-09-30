-- A03 Serializacion - Prepara datos para la DEMO
-- 1) Carga disponibilidad a TODOS los profesores activos: lunes a sabado, 08:00 a 22:00.
--    (si el profesor ya tiene ese mismo bloque activo ese dia, no lo duplica)
-- 2) Opcional: habilita a todos los profesores activos en todos los cursos activos (@HabilitarCursos = 1).
-- 3) Recalcula DVH de las filas nuevas y el DVV de las tablas tocadas, con la misma
--    canonicalizacion que IntegridadDAL_83KI (Col=Val|Col=Val, UTF-16, SHA-256), para que
--    la verificacion de integridad al iniciar la app no marque errores.
--
-- Es idempotente: se puede ejecutar varias veces.
USE [InstitutoVisionEnAccion];
GO
SET NOCOUNT ON;
SET XACT_ABORT ON;

-------------------------------------------------------------------------
-- Parametros de la demo
-------------------------------------------------------------------------
DECLARE @HoraDesde TIME(0) = '08:00';
DECLARE @HoraHasta TIME(0) = '22:00';
DECLARE @HabilitarCursos BIT = 1;   -- 1 = cada profesor activo queda habilitado en cada curso activo

DECLARE @Dias TABLE (DiaSemana NVARCHAR(20) NOT NULL PRIMARY KEY);
INSERT INTO @Dias (DiaSemana) VALUES
(N'Monday'), (N'Tuesday'), (N'Wednesday'), (N'Thursday'), (N'Friday'), (N'Saturday');

DECLARE @NuevasDisponibilidades TABLE (IdDisponibilidadProfesor INT NOT NULL);
DECLARE @CursoProfesorTocados TABLE (IdCurso INT NOT NULL, IdProfesor INT NOT NULL);

BEGIN TRANSACTION;
BEGIN TRY
    ---------------------------------------------------------------------
    -- 1) Disponibilidad
    ---------------------------------------------------------------------
    INSERT INTO dbo.DisponibilidadProfesor (IdProfesor, DiaSemana, HoraInicio, HoraFin, EstadoActivo, DVH)
    OUTPUT inserted.IdDisponibilidadProfesor INTO @NuevasDisponibilidades (IdDisponibilidadProfesor)
    SELECT p.IdProfesor, d.DiaSemana, @HoraDesde, @HoraHasta, 1, ''
    FROM dbo.Profesor p
    CROSS JOIN @Dias d
    WHERE p.EstadoActivo = 1
      AND NOT EXISTS
      (
          SELECT 1
          FROM dbo.DisponibilidadProfesor dp
          WHERE dp.IdProfesor = p.IdProfesor
            AND dp.DiaSemana = d.DiaSemana
            AND dp.EstadoActivo = 1
            AND dp.HoraInicio <= @HoraDesde
            AND dp.HoraFin >= @HoraHasta
      );

    UPDATE dp
    SET DVH = LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', CONCAT(
        N'DiaSemana=', LTRIM(RTRIM(CONVERT(NVARCHAR(MAX), dp.DiaSemana))), N'|',
        N'EstadoActivo=', CASE WHEN dp.EstadoActivo = 1 THEN N'1' ELSE N'0' END, N'|',
        N'HoraFin=', CONVERT(NVARCHAR(8), dp.HoraFin, 108), N'|',
        N'HoraInicio=', CONVERT(NVARCHAR(8), dp.HoraInicio, 108), N'|',
        N'IdDisponibilidadProfesor=', CONVERT(NVARCHAR(128), dp.IdDisponibilidadProfesor), N'|',
        N'IdProfesor=', CONVERT(NVARCHAR(128), dp.IdProfesor)
    )), 2))
    FROM dbo.DisponibilidadProfesor dp
    INNER JOIN @NuevasDisponibilidades n ON n.IdDisponibilidadProfesor = dp.IdDisponibilidadProfesor;

    ---------------------------------------------------------------------
    -- 2) Curso-Profesor (opcional)
    ---------------------------------------------------------------------
    IF @HabilitarCursos = 1
    BEGIN
        -- Relaciones existentes pero deshabilitadas -> se habilitan
        UPDATE cp
        SET EstadoActivo = 1
        OUTPUT inserted.IdCurso, inserted.IdProfesor INTO @CursoProfesorTocados (IdCurso, IdProfesor)
        FROM dbo.CursoProfesor cp
        INNER JOIN dbo.Curso c ON c.IdCurso = cp.IdCurso AND c.EstadoActivo = 1
        INNER JOIN dbo.Profesor p ON p.IdProfesor = cp.IdProfesor AND p.EstadoActivo = 1
        WHERE cp.EstadoActivo = 0;

        -- Relaciones que no existen -> se crean habilitadas
        INSERT INTO dbo.CursoProfesor (IdCurso, IdProfesor, EstadoActivo, DVH)
        OUTPUT inserted.IdCurso, inserted.IdProfesor INTO @CursoProfesorTocados (IdCurso, IdProfesor)
        SELECT c.IdCurso, p.IdProfesor, 1, ''
        FROM dbo.Curso c
        CROSS JOIN dbo.Profesor p
        WHERE c.EstadoActivo = 1
          AND p.EstadoActivo = 1
          AND NOT EXISTS (SELECT 1 FROM dbo.CursoProfesor cp WHERE cp.IdCurso = c.IdCurso AND cp.IdProfesor = p.IdProfesor);

        UPDATE cp
        SET DVH = LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', CONCAT(
            N'EstadoActivo=', CASE WHEN cp.EstadoActivo = 1 THEN N'1' ELSE N'0' END, N'|',
            N'IdCurso=', CONVERT(NVARCHAR(128), cp.IdCurso), N'|',
            N'IdProfesor=', CONVERT(NVARCHAR(128), cp.IdProfesor)
        )), 2))
        FROM dbo.CursoProfesor cp
        INNER JOIN @CursoProfesorTocados t ON t.IdCurso = cp.IdCurso AND t.IdProfesor = cp.IdProfesor;
    END;

    ---------------------------------------------------------------------
    -- 3) DVV de las tablas tocadas
    ---------------------------------------------------------------------
    IF NOT EXISTS (SELECT 1 FROM dbo.DigitoVerificador_83KI WHERE NombreTabla = N'DisponibilidadProfesor')
        INSERT INTO dbo.DigitoVerificador_83KI (NombreTabla, DVV, FechaActualizacion) VALUES (N'DisponibilidadProfesor', '', GETDATE());

    UPDATE dbo.DigitoVerificador_83KI
    SET DVV = LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', ISNULL(CAST((SELECT CAST(N'' AS NVARCHAR(MAX)) + DVH FROM dbo.DisponibilidadProfesor ORDER BY DVH FOR XML PATH(N'')) AS NVARCHAR(MAX)), N'')), 2)),
        FechaActualizacion = GETDATE()
    WHERE NombreTabla = N'DisponibilidadProfesor';

    IF @HabilitarCursos = 1
    BEGIN
        IF NOT EXISTS (SELECT 1 FROM dbo.DigitoVerificador_83KI WHERE NombreTabla = N'CursoProfesor')
            INSERT INTO dbo.DigitoVerificador_83KI (NombreTabla, DVV, FechaActualizacion) VALUES (N'CursoProfesor', '', GETDATE());

        UPDATE dbo.DigitoVerificador_83KI
        SET DVV = LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', ISNULL(CAST((SELECT CAST(N'' AS NVARCHAR(MAX)) + DVH FROM dbo.CursoProfesor ORDER BY DVH FOR XML PATH(N'')) AS NVARCHAR(MAX)), N'')), 2)),
            FechaActualizacion = GETDATE()
        WHERE NombreTabla = N'CursoProfesor';
    END;

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;

-------------------------------------------------------------------------
-- Resumen
-------------------------------------------------------------------------
SELECT (SELECT COUNT(*) FROM @NuevasDisponibilidades) AS DisponibilidadesNuevas,
       (SELECT COUNT(*) FROM @CursoProfesorTocados)   AS CursoProfesorHabilitados;

SELECT p.IdProfesor, p.Apellido + N', ' + p.Nombre AS Profesor, dp.DiaSemana,
       CONVERT(CHAR(5), dp.HoraInicio, 108) AS Desde, CONVERT(CHAR(5), dp.HoraFin, 108) AS Hasta
FROM dbo.DisponibilidadProfesor dp
INNER JOIN dbo.Profesor p ON p.IdProfesor = dp.IdProfesor
WHERE dp.EstadoActivo = 1 AND p.EstadoActivo = 1
ORDER BY p.IdProfesor,
         CASE dp.DiaSemana WHEN N'Monday' THEN 1 WHEN N'Tuesday' THEN 2 WHEN N'Wednesday' THEN 3
                           WHEN N'Thursday' THEN 4 WHEN N'Friday' THEN 5 WHEN N'Saturday' THEN 6 ELSE 7 END,
         dp.HoraInicio;
GO

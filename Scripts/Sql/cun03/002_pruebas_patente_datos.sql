-- =============================================================================
-- CUN03 - Script de datos de prueba para verificacion manual (002_pruebas)
-- =============================================================================
-- Requisito previo: ejecutar primero Scripts/Sql/cun03/001_migracion.sql
-- o crear la base con Scripts/Sql/EsquemaCompleto.sql.
--
-- Este script es SOLO PARA PRUEBAS. Crea:
--   * Familia/rol/usuario AUTORIZADO para el permiso CUN03 (patente 64).
--   * Familia/rol/usuario NO AUTORIZADO para el permiso CUN03.
--   * Planes de pago, curso, profesor, comisiones y personas de prueba.
--   * Datos para los flujos: Lead, Alumno existente, DNI nuevo, sin cupo,
--     inscripcion duplicada.
--   * SELECTs de verificacion de SolicitudInscripcion, Cuota, Bitacora y
--     estado DVH/DVV.
--
-- CONVENCION IMPORTANTE: DiaSemana se guarda en INGLES (DayOfWeek: Monday,
-- Tuesday, ...) porque los DAL mapean con Enum.Parse(typeof(DayOfWeek), ...).
-- La UI traduce para mostrar via Idiomas ("Dominio.DiaSemana.Monday" = "Lunes").
-- NO insertar dias en espanol o el listado de comisiones lanza
-- ArgumentException al entrar a la pantalla.
-- =============================================================================

SET NOCOUNT ON;
GO

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

PRINT 'CUN03 test data script started.';
GO

-- =============================================================================
-- Constantes de prueba
-- =============================================================================
DECLARE @CodigoPatenteCun03 INT = 64;
DECLARE @TestPasswordHash VARCHAR(64) = LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', N'Password123!'), 2));
GO

-- =============================================================================
-- SECCION 1: Usuario AUTORIZADO para CUN03
-- La patente 64 ('Registrar solicitud de inscripcion') ya existe gracias a
-- 001_migracion.sql. Aqui se crea un rol/familia/usuario de prueba que la tenga.
-- =============================================================================
DECLARE @CodigoPatenteCun03 INT = 64;
DECLARE @TestPasswordHash VARCHAR(64) = LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', N'Password123!'), 2));

IF NOT EXISTS (SELECT 1 FROM dbo.Familias WHERE Nombre = N'PreInscripcionCUN03')
    INSERT INTO dbo.Familias (Nombre, DVH) VALUES (N'PreInscripcionCUN03', '');

DECLARE @CodigoFamiliaCun03 INT = (SELECT CodigoFamilia FROM dbo.Familias WHERE Nombre = N'PreInscripcionCUN03');

IF NOT EXISTS (SELECT 1 FROM dbo.FamiliaPatente WHERE CodigoFamilia = @CodigoFamiliaCun03 AND CodigoPatente = @CodigoPatenteCun03)
    INSERT INTO dbo.FamiliaPatente (CodigoFamilia, CodigoPatente, DVH) VALUES (@CodigoFamiliaCun03, @CodigoPatenteCun03, '');

IF NOT EXISTS (SELECT 1 FROM dbo.Roles WHERE Nombre = N'RolTestCUN03')
    INSERT INTO dbo.Roles (Nombre, DVH) VALUES (N'RolTestCUN03', '');

DECLARE @CodigoRolCun03 INT = (SELECT CodigoRol FROM dbo.Roles WHERE Nombre = N'RolTestCUN03');

IF NOT EXISTS (SELECT 1 FROM dbo.RolFamilia WHERE CodigoRol = @CodigoRolCun03 AND CodigoFamilia = @CodigoFamiliaCun03)
    INSERT INTO dbo.RolFamilia (CodigoRol, CodigoFamilia, DVH) VALUES (@CodigoRolCun03, @CodigoFamiliaCun03, '');

IF NOT EXISTS (SELECT 1 FROM dbo.Usuarios WHERE DNI = N'99900001')
    INSERT INTO dbo.Usuarios (DNI, Nombre, Apellido, Email, Bloqueado, Contrasena, Username, Activo, CodigoRol, IntentosRealizados, FechaUltimoIntento, IdiomaId, DVH)
    VALUES (N'99900001', N'Test', N'CUN03 Autorizado', N'test.cun03.autorizado@example.com', 0, @TestPasswordHash, N'99900001TestCUN03', 1, @CodigoRolCun03, 0, NULL, N'es-AR', '');

PRINT 'Authorized test user created/verified: DNI 99900001, RolTestCUN03.';
GO

-- =============================================================================
-- SECCION 2: Usuario NO AUTORIZADO para CUN03
-- Mismo esquema de familia/rol pero SIN asignar la patente 64.
-- =============================================================================
DECLARE @CodigoPatenteCun03 INT = 64;
DECLARE @TestPasswordHash VARCHAR(64) = LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', N'Password123!'), 2));

IF NOT EXISTS (SELECT 1 FROM dbo.Familias WHERE Nombre = N'SinPreInscripcionCUN03')
    INSERT INTO dbo.Familias (Nombre, DVH) VALUES (N'SinPreInscripcionCUN03', '');

DECLARE @CodigoFamiliaSinCun03 INT = (SELECT CodigoFamilia FROM dbo.Familias WHERE Nombre = N'SinPreInscripcionCUN03');

-- NOTA: no se inserta fila en FamiliaPatente para la patente 64.

IF NOT EXISTS (SELECT 1 FROM dbo.Roles WHERE Nombre = N'RolTestSinCUN03')
    INSERT INTO dbo.Roles (Nombre, DVH) VALUES (N'RolTestSinCUN03', '');

DECLARE @CodigoRolSinCun03 INT = (SELECT CodigoRol FROM dbo.Roles WHERE Nombre = N'RolTestSinCUN03');

IF NOT EXISTS (SELECT 1 FROM dbo.RolFamilia WHERE CodigoRol = @CodigoRolSinCun03 AND CodigoFamilia = @CodigoFamiliaSinCun03)
    INSERT INTO dbo.RolFamilia (CodigoRol, CodigoFamilia, DVH) VALUES (@CodigoRolSinCun03, @CodigoFamiliaSinCun03, '');

IF NOT EXISTS (SELECT 1 FROM dbo.Usuarios WHERE DNI = N'99900002')
    INSERT INTO dbo.Usuarios (DNI, Nombre, Apellido, Email, Bloqueado, Contrasena, Username, Activo, CodigoRol, IntentosRealizados, FechaUltimoIntento, IdiomaId, DVH)
    VALUES (N'99900002', N'Test', N'CUN03 NO Autorizado', N'test.cun03.noautorizado@example.com', 0, @TestPasswordHash, N'99900002TestCUN03No', 1, @CodigoRolSinCun03, 0, NULL, N'es-AR', '');

PRINT 'Unauthorized test user created/verified: DNI 99900002, RolTestSinCUN03.';
GO

-- =============================================================================
-- SECCION 3: Datos de negocio
-- =============================================================================
DECLARE @TestPasswordHash VARCHAR(64) = LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', N'Password123!'), 2));

-- Planes de pago de prueba (2+ planes activos para poder probar seleccion)
DECLARE @IdPlanContado INT = (SELECT IdPlanDePago FROM dbo.PlanesDePago WHERE Nombre = N'CUN03 Contado');
IF @IdPlanContado IS NULL
BEGIN
    INSERT INTO dbo.PlanesDePago (Nombre, RecargoPorcentaje, EstadoActivo) VALUES (N'CUN03 Contado', 0.0000, 1);
    SET @IdPlanContado = SCOPE_IDENTITY();
END

DECLARE @IdPlan3Cuotas INT = (SELECT IdPlanDePago FROM dbo.PlanesDePago WHERE Nombre = N'CUN03 3 cuotas');
IF @IdPlan3Cuotas IS NULL
BEGIN
    INSERT INTO dbo.PlanesDePago (Nombre, RecargoPorcentaje, EstadoActivo) VALUES (N'CUN03 3 cuotas', 5.0000, 1);
    SET @IdPlan3Cuotas = SCOPE_IDENTITY();
END

-- Curso de prueba
DECLARE @IdCurso INT = (SELECT IdCurso FROM dbo.Curso WHERE Nombre = N'Curso CUN03 Test');
IF @IdCurso IS NULL
BEGIN
    INSERT INTO dbo.Curso (Nombre, Descripcion, CargaHoraria, EstadoActivo, DVH)
    VALUES (N'Curso CUN03 Test', N'Curso creado unicamente para pruebas manuales de CUN03.', 60, 1, '');
    SET @IdCurso = SCOPE_IDENTITY();
END

-- Profesor de prueba
DECLARE @IdProfesor INT = (SELECT IdProfesor FROM dbo.Profesor WHERE DNI = N'99000001');
IF @IdProfesor IS NULL
BEGIN
    INSERT INTO dbo.Profesor (DNI, Nombre, Apellido, Email, EstadoActivo, DVH)
    VALUES (N'99000001', N'Profesor', N'Test CUN03', N'profesor.test.cun03@example.com', 1, '');
    SET @IdProfesor = SCOPE_IDENTITY();
END

-- Relacion curso-profesor
IF NOT EXISTS (SELECT 1 FROM dbo.CursoProfesor WHERE IdCurso = @IdCurso AND IdProfesor = @IdProfesor)
    INSERT INTO dbo.CursoProfesor (IdCurso, IdProfesor, EstadoActivo, DVH) VALUES (@IdCurso, @IdProfesor, 1, '');

-- Disponibilidad del profesor (tres dias sin solapamiento para tres comisiones)
IF NOT EXISTS (SELECT 1 FROM dbo.DisponibilidadProfesor WHERE IdProfesor = @IdProfesor AND DiaSemana = N'Monday' AND HoraInicio = CAST(N'18:00' AS TIME) AND HoraFin = CAST(N'20:00' AS TIME))
    INSERT INTO dbo.DisponibilidadProfesor (IdProfesor, DiaSemana, HoraInicio, HoraFin, EstadoActivo, DVH)
    VALUES (@IdProfesor, N'Monday', CAST(N'18:00' AS TIME), CAST(N'20:00' AS TIME), 1, '');

IF NOT EXISTS (SELECT 1 FROM dbo.DisponibilidadProfesor WHERE IdProfesor = @IdProfesor AND DiaSemana = N'Tuesday' AND HoraInicio = CAST(N'18:00' AS TIME) AND HoraFin = CAST(N'20:00' AS TIME))
    INSERT INTO dbo.DisponibilidadProfesor (IdProfesor, DiaSemana, HoraInicio, HoraFin, EstadoActivo, DVH)
    VALUES (@IdProfesor, N'Tuesday', CAST(N'18:00' AS TIME), CAST(N'20:00' AS TIME), 1, '');

IF NOT EXISTS (SELECT 1 FROM dbo.DisponibilidadProfesor WHERE IdProfesor = @IdProfesor AND DiaSemana = N'Wednesday' AND HoraInicio = CAST(N'18:00' AS TIME) AND HoraFin = CAST(N'20:00' AS TIME))
    INSERT INTO dbo.DisponibilidadProfesor (IdProfesor, DiaSemana, HoraInicio, HoraFin, EstadoActivo, DVH)
    VALUES (@IdProfesor, N'Wednesday', CAST(N'18:00' AS TIME), CAST(N'20:00' AS TIME), 1, '');

DECLARE @FechaLimite DATE = DATEADD(day, 30, CONVERT(date, GETDATE()));
DECLARE @FechaInicio DATE = CONVERT(date, GETDATE());
DECLARE @FechaFin DATE = DATEADD(month, 3, CONVERT(date, GETDATE()));

-- Comision ELEGIBLE: cupo amplio, matricula definida, plan contado
DECLARE @IdComisionElegible INT = (SELECT IdComision FROM dbo.Comision WHERE Codigo LIKE N'COM-%' AND IdCurso = @IdCurso AND DiaSemana = N'Monday' AND CAST(HoraInicio AS TIME) = CAST(N'18:00' AS TIME));
IF @IdComisionElegible IS NULL
BEGIN
    INSERT INTO dbo.Comision (Codigo, IdCurso, IdProfesor, DiaSemana, HoraInicio, HoraFin, CupoMinimo, CupoMaximo, FechaLimitePago, FechaInicio, FechaFin, ArancelBase, MontoMatricula, IdPlanDePago, RecargoPlanSnapshot, Estado, DVH)
    VALUES (N'', @IdCurso, @IdProfesor, N'Monday', CAST(N'18:00' AS TIME), CAST(N'20:00' AS TIME), 5, 10, @FechaLimite, @FechaInicio, @FechaFin, 50000.00, 15000.00, @IdPlanContado, 0.0000, N'preapertura', '');
    SET @IdComisionElegible = SCOPE_IDENTITY();
    UPDATE dbo.Comision SET Codigo = CONCAT(N'COM-', RIGHT(CONCAT(N'000000', @IdComisionElegible), 6)) WHERE IdComision = @IdComisionElegible;
END

-- Comision para flujo LEAD: cupo amplio, matricula distinta, plan 3 cuotas
DECLARE @IdComisionLead INT = (SELECT IdComision FROM dbo.Comision WHERE Codigo LIKE N'COM-%' AND IdCurso = @IdCurso AND DiaSemana = N'Tuesday' AND CAST(HoraInicio AS TIME) = CAST(N'18:00' AS TIME));
IF @IdComisionLead IS NULL
BEGIN
    INSERT INTO dbo.Comision (Codigo, IdCurso, IdProfesor, DiaSemana, HoraInicio, HoraFin, CupoMinimo, CupoMaximo, FechaLimitePago, FechaInicio, FechaFin, ArancelBase, MontoMatricula, IdPlanDePago, RecargoPlanSnapshot, Estado, DVH)
    VALUES (N'', @IdCurso, @IdProfesor, N'Tuesday', CAST(N'18:00' AS TIME), CAST(N'20:00' AS TIME), 5, 10, @FechaLimite, @FechaInicio, @FechaFin, 60000.00, 20000.00, @IdPlan3Cuotas, 5.0000, N'preapertura', '');
    SET @IdComisionLead = SCOPE_IDENTITY();
    UPDATE dbo.Comision SET Codigo = CONCAT(N'COM-', RIGHT(CONCAT(N'000000', @IdComisionLead), 6)) WHERE IdComision = @IdComisionLead;
END

-- Comision SIN VACANTES: cupo 1, ocupada por un alumno existente
DECLARE @IdComisionSinCupo INT = (SELECT IdComision FROM dbo.Comision WHERE Codigo LIKE N'COM-%' AND IdCurso = @IdCurso AND DiaSemana = N'Wednesday' AND CAST(HoraInicio AS TIME) = CAST(N'18:00' AS TIME));
IF @IdComisionSinCupo IS NULL
BEGIN
    INSERT INTO dbo.Comision (Codigo, IdCurso, IdProfesor, DiaSemana, HoraInicio, HoraFin, CupoMinimo, CupoMaximo, FechaLimitePago, FechaInicio, FechaFin, ArancelBase, MontoMatricula, IdPlanDePago, RecargoPlanSnapshot, Estado, DVH)
    VALUES (N'', @IdCurso, @IdProfesor, N'Wednesday', CAST(N'18:00' AS TIME), CAST(N'20:00' AS TIME), 1, 1, @FechaLimite, @FechaInicio, @FechaFin, 40000.00, 12000.00, @IdPlanContado, 0.0000, N'preapertura', '');
    SET @IdComisionSinCupo = SCOPE_IDENTITY();
    UPDATE dbo.Comision SET Codigo = CONCAT(N'COM-', RIGHT(CONCAT(N'000000', @IdComisionSinCupo), 6)) WHERE IdComision = @IdComisionSinCupo;
END

-- Lead de prueba con consulta asociada a la comision lead (flujo auto-sugerencia)
DECLARE @IdLead INT = (SELECT IdLead FROM dbo.[Lead] WHERE DNI = N'99000010');
IF @IdLead IS NULL
BEGIN
    INSERT INTO dbo.[Lead] (DNI, Nombre, Apellido, Email, Telefono, FechaAlta, Estado, DVH)
    VALUES (N'99000010', N'Lead', N'Test CUN03', N'lead.test.cun03@example.com', N'54911111111', GETDATE(), N'activo', '');
    SET @IdLead = SCOPE_IDENTITY();
END

IF NOT EXISTS (SELECT 1 FROM dbo.ConsultaLead WHERE IdLead = @IdLead AND IdComision = @IdComisionLead)
BEGIN
    INSERT INTO dbo.ConsultaLead (Codigo, IdLead, IdComision, FechaConsulta, MedioContacto, Motivo, Observaciones, Estado, DVH)
    VALUES (N'', @IdLead, @IdComisionLead, GETDATE(), N'Email', N'Interes en comision de prueba CUN03', N'Consulta generada por script de pruebas.', N'registrada', '');
    DECLARE @IdConsultaLead INT = SCOPE_IDENTITY();
    UPDATE dbo.ConsultaLead SET Codigo = CONCAT(N'CON-', FORMAT(GETDATE(), 'yyyyMMdd'), N'-', RIGHT(CONCAT(N'0000', @IdConsultaLead), 4)) WHERE IdConsultaLead = @IdConsultaLead;
END

-- Alumno existente de prueba (flujo reutilizacion + seleccion manual de comision)
DECLARE @IdAlumnoExistente INT = (SELECT IdAlumno FROM dbo.Alumno WHERE DNI = N'99000020');
IF @IdAlumnoExistente IS NULL
BEGIN
    INSERT INTO dbo.Alumno (DNI, Nombre, Apellido, Email, Telefono, FechaAlta, Estado, DVH)
    VALUES (N'99000020', N'Alumno', N'Existente CUN03', N'alumno.existente.cun03@example.com', N'54912222222', GETDATE(), N'activo', '');
    SET @IdAlumnoExistente = SCOPE_IDENTITY();
END

-- Alumno "ocupador" para dejar la comision sin cupo lleno
DECLARE @IdAlumnoOcupador INT = (SELECT IdAlumno FROM dbo.Alumno WHERE DNI = N'99000040');
IF @IdAlumnoOcupador IS NULL
BEGIN
    INSERT INTO dbo.Alumno (DNI, Nombre, Apellido, Email, Telefono, FechaAlta, Estado, DVH)
    VALUES (N'99000040', N'Ocupador', N'Cupo CUN03', N'ocupador.cupo.cun03@example.com', N'54913333333', GETDATE(), N'activo', '');
    SET @IdAlumnoOcupador = SCOPE_IDENTITY();
END

-- Ocupar la unica vacante de la comision sin cupo
IF NOT EXISTS (SELECT 1 FROM dbo.SolicitudInscripcion WHERE IdAlumno = @IdAlumnoOcupador AND IdComision = @IdComisionSinCupo AND Estado IN (N'pendiente', N'confirmada', N'activa'))
BEGIN
    INSERT INTO dbo.SolicitudInscripcion (Codigo, IdAlumno, IdLeadOrigen, IdComision, IdPlanDePago, RecargoPlanSnapshot, FechaSolicitud, Estado, Observaciones, DVH)
    VALUES (N'', @IdAlumnoOcupador, NULL, @IdComisionSinCupo, @IdPlanContado, 0.0000, GETDATE(), N'pendiente', N'Ocupacion de cupo para prueba sin vacantes.', '');
    DECLARE @IdSolicitudOcupador INT = SCOPE_IDENTITY();
    UPDATE dbo.SolicitudInscripcion SET Codigo = CONCAT(N'SI-', RIGHT(CONCAT(N'000000', @IdSolicitudOcupador), 6)) WHERE IdSolicitudInscripcion = @IdSolicitudOcupador;

    INSERT INTO dbo.Cuota (IdSolicitudInscripcion, NumeroCuota, MontoOriginal, BalanceAdeudado, FechaVencimiento, Estado, DVH)
    VALUES (@IdSolicitudOcupador, 1, 12000.00, 12000.00, DATEADD(day, 7, CONVERT(date, GETDATE())), N'pendiente', '');
END

-- Solicitud previa para el alumno existente en la comision elegible (escenario duplicado)
IF NOT EXISTS (SELECT 1 FROM dbo.SolicitudInscripcion WHERE IdAlumno = @IdAlumnoExistente AND IdComision = @IdComisionElegible AND Estado IN (N'pendiente', N'confirmada', N'activa'))
BEGIN
    INSERT INTO dbo.SolicitudInscripcion (Codigo, IdAlumno, IdLeadOrigen, IdComision, IdPlanDePago, RecargoPlanSnapshot, FechaSolicitud, Estado, Observaciones, DVH)
    VALUES (N'', @IdAlumnoExistente, NULL, @IdComisionElegible, @IdPlanContado, 0.0000, GETDATE(), N'pendiente', N'Solicitud previa para prueba de duplicado.', '');
    DECLARE @IdSolicitudDup INT = SCOPE_IDENTITY();
    UPDATE dbo.SolicitudInscripcion SET Codigo = CONCAT(N'SI-', RIGHT(CONCAT(N'000000', @IdSolicitudDup), 6)) WHERE IdSolicitudInscripcion = @IdSolicitudDup;

    INSERT INTO dbo.Cuota (IdSolicitudInscripcion, NumeroCuota, MontoOriginal, BalanceAdeudado, FechaVencimiento, Estado, DVH)
    VALUES (@IdSolicitudDup, 1, 15000.00, 15000.00, DATEADD(day, 7, CONVERT(date, GETDATE())), N'pendiente', '');
END

PRINT 'Business test data created/verified.';
GO

-- =============================================================================
-- SECCION 4: Recalculo de DVH/DVV para tablas de permisos
-- (Las tablas de negocio se recalculan desde la aplicacion al operar.)
-- =============================================================================
-- Recalculo de DVH para tablas de permisos usando el mismo patron que
-- Scripts/Sql/EsquemaCompleto.sql (caracter U+2205 para valores nulos).
UPDATE dbo.Patentes
SET DVH = LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', CONCAT(
    N'CodigoPatente=', CONVERT(NVARCHAR(128), CodigoPatente), N'|',
    N'Nombre=', LTRIM(RTRIM(ISNULL(CONVERT(NVARCHAR(MAX), Nombre), N'∅')))
)), 2));
GO

UPDATE dbo.Familias
SET DVH = LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', CONCAT(
    N'CodigoFamilia=', CONVERT(NVARCHAR(128), CodigoFamilia), N'|',
    N'Nombre=', LTRIM(RTRIM(ISNULL(CONVERT(NVARCHAR(MAX), Nombre), N'∅')))
)), 2));
GO

UPDATE dbo.Roles
SET DVH = LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', CONCAT(
    N'CodigoRol=', CONVERT(NVARCHAR(128), CodigoRol), N'|',
    N'Nombre=', LTRIM(RTRIM(ISNULL(CONVERT(NVARCHAR(MAX), Nombre), N'∅')))
)), 2));
GO

UPDATE dbo.FamiliaPatente
SET DVH = LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', CONCAT(
    N'CodigoFamilia=', CONVERT(NVARCHAR(128), CodigoFamilia), N'|',
    N'CodigoPatente=', CONVERT(NVARCHAR(128), CodigoPatente)
)), 2));
GO

UPDATE dbo.RolFamilia
SET DVH = LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', CONCAT(
    N'CodigoFamilia=', CONVERT(NVARCHAR(128), CodigoFamilia), N'|',
    N'CodigoRol=', CONVERT(NVARCHAR(128), CodigoRol)
)), 2));
GO

UPDATE dbo.RolPatente
SET DVH = LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', CONCAT(
    N'CodigoPatente=', CONVERT(NVARCHAR(128), CodigoPatente), N'|',
    N'CodigoRol=', CONVERT(NVARCHAR(128), CodigoRol)
)), 2));
GO

-- NOTA: Usuarios.DVH no se recalcula aqui porque depende de la formula exacta
-- del sistema. Los usuarios de prueba se crean con DVH vacio; utilizar la
-- opcion "Recalcular hashes" de la aplicacion antes de loguearse, o loguearse
-- con un usuario existente que ya tenga la patente 64 (por ejemplo Admin).

UPDATE dbo.DigitoVerificador_83KI SET DVV = LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', ISNULL(CAST((SELECT CAST(N'' AS NVARCHAR(MAX)) + DVH FROM dbo.Patentes ORDER BY DVH FOR XML PATH(N'')) AS NVARCHAR(MAX)), N'')), 2)), FechaActualizacion = GETDATE() WHERE NombreTabla = N'Patentes';
UPDATE dbo.DigitoVerificador_83KI SET DVV = LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', ISNULL(CAST((SELECT CAST(N'' AS NVARCHAR(MAX)) + DVH FROM dbo.Familias ORDER BY DVH FOR XML PATH(N'')) AS NVARCHAR(MAX)), N'')), 2)), FechaActualizacion = GETDATE() WHERE NombreTabla = N'Familias';
UPDATE dbo.DigitoVerificador_83KI SET DVV = LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', ISNULL(CAST((SELECT CAST(N'' AS NVARCHAR(MAX)) + DVH FROM dbo.Roles ORDER BY DVH FOR XML PATH(N'')) AS NVARCHAR(MAX)), N'')), 2)), FechaActualizacion = GETDATE() WHERE NombreTabla = N'Roles';
UPDATE dbo.DigitoVerificador_83KI SET DVV = LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', ISNULL(CAST((SELECT CAST(N'' AS NVARCHAR(MAX)) + DVH FROM dbo.FamiliaPatente ORDER BY DVH FOR XML PATH(N'')) AS NVARCHAR(MAX)), N'')), 2)), FechaActualizacion = GETDATE() WHERE NombreTabla = N'FamiliaPatente';
UPDATE dbo.DigitoVerificador_83KI SET DVV = LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', ISNULL(CAST((SELECT CAST(N'' AS NVARCHAR(MAX)) + DVH FROM dbo.RolFamilia ORDER BY DVH FOR XML PATH(N'')) AS NVARCHAR(MAX)), N'')), 2)), FechaActualizacion = GETDATE() WHERE NombreTabla = N'RolFamilia';
UPDATE dbo.DigitoVerificador_83KI SET DVV = LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', ISNULL(CAST((SELECT CAST(N'' AS NVARCHAR(MAX)) + DVH FROM dbo.RolPatente ORDER BY DVH FOR XML PATH(N'')) AS NVARCHAR(MAX)), N'')), 2)), FechaActualizacion = GETDATE() WHERE NombreTabla = N'RolPatente';
GO

PRINT 'Permission hashes recalculated.';
GO

-- =============================================================================
-- SECCION 5: SELECTs de verificacion
-- Ejecutar estos SELECTs manualmente despues de correr el script y despues de
-- las pruebas de UI para confirmar estado.
-- =============================================================================
PRINT 'Running verification SELECTs...';
GO

-- 1) Comisiones elegibles segun el SP de CUN03
SELECT N'Comisiones elegibles' AS Verificacion, c.IdComision, c.Codigo, cu.Nombre AS Curso, c.DiaSemana, c.MontoMatricula,
       c.CupoMaximo - ISNULL(s.Ocupadas, 0) AS VacantesDisponibles
FROM dbo.Comision c
INNER JOIN dbo.Curso cu ON cu.IdCurso = c.IdCurso
OUTER APPLY (SELECT COUNT(1) AS Ocupadas FROM dbo.SolicitudInscripcion si WHERE si.IdComision = c.IdComision AND si.Estado IN (N'pendiente', N'confirmada', N'activa')) s
WHERE c.Estado <> N'eliminada'
  AND c.FechaLimitePago >= CONVERT(date, GETDATE())
  AND c.MontoMatricula > 0
  AND c.CupoMaximo - ISNULL(s.Ocupadas, 0) > 0
  AND EXISTS (SELECT 1 FROM dbo.PlanesDePago pp WHERE pp.IdPlanDePago = c.IdPlanDePago AND pp.EstadoActivo = 1)
  AND c.IdCurso = (SELECT IdCurso FROM dbo.Curso WHERE Nombre = N'Curso CUN03 Test')
ORDER BY c.DiaSemana;
GO

-- 2) Lead y consulta de prueba
SELECT N'Lead + consulta' AS Verificacion, l.IdLead, l.DNI, l.Nombre, l.Apellido, cl.IdConsultaLead, cl.Codigo, cl.IdComision, c.Codigo AS CodigoComision
FROM dbo.[Lead] l
INNER JOIN dbo.ConsultaLead cl ON cl.IdLead = l.IdLead
INNER JOIN dbo.Comision c ON c.IdComision = cl.IdComision
WHERE l.DNI = N'99000010';
GO

-- 3) Alumno existente de prueba
SELECT N'Alumno existente' AS Verificacion, IdAlumno, DNI, Nombre, Apellido, Email, Telefono
FROM dbo.Alumno
WHERE DNI = N'99000020';
GO

-- 4) Solicitudes de inscripcion generadas por el script
SELECT N'Solicitudes de inscripcion' AS Verificacion, si.IdSolicitudInscripcion, si.Codigo, a.DNI, a.Nombre, a.Apellido,
       c.Codigo AS CodigoComision, pp.Nombre AS PlanDePago, si.Estado, si.FechaSolicitud
FROM dbo.SolicitudInscripcion si
INNER JOIN dbo.Alumno a ON a.IdAlumno = si.IdAlumno
INNER JOIN dbo.Comision c ON c.IdComision = si.IdComision
INNER JOIN dbo.PlanesDePago pp ON pp.IdPlanDePago = si.IdPlanDePago
WHERE a.DNI IN (N'99000020', N'99000040')
ORDER BY si.FechaSolicitud DESC;
GO

-- 5) Cuotas generadas (numero 1, monto = matricula, balance, vencimiento +7 dias, estado Pendiente)
SELECT N'Cuota 1' AS Verificacion, cu.IdCuota, si.Codigo AS Solicitud, cu.NumeroCuota, cu.MontoOriginal, cu.BalanceAdeudado,
       cu.FechaVencimiento, cu.Estado, DATEDIFF(day, CONVERT(date, si.FechaSolicitud), cu.FechaVencimiento) AS DiasDesdeSolicitud
FROM dbo.Cuota cu
INNER JOIN dbo.SolicitudInscripcion si ON si.IdSolicitudInscripcion = cu.IdSolicitudInscripcion
INNER JOIN dbo.Alumno a ON a.IdAlumno = si.IdAlumno
WHERE a.DNI IN (N'99000020', N'99000040')
ORDER BY cu.FechaVencimiento;
GO

-- 6) Bitacora: eventos recientes del modulo PreInscripcion o Usuarios para los usuarios de prueba
SELECT N'Bitacora' AS Verificacion, Id, Username, Fecha, Modulo, Descripcion, Criticidad
FROM dbo.BitacoraEventos
WHERE Username IN (N'99900001TestCUN03', N'99900002TestCUN03No')
   OR Modulo = N'PreInscripcion'
ORDER BY Fecha DESC;
GO

-- 7) Estado DVH/DVV de tablas relevantes
SELECT N'DVH/DVV' AS Verificacion, NombreTabla, DVV, FechaActualizacion
FROM dbo.DigitoVerificador_83KI
WHERE NombreTabla IN (N'Patentes', N'Familias', N'FamiliaPatente', N'Roles', N'RolFamilia', N'RolPatente', N'Usuarios',
                      N'Alumno', N'SolicitudInscripcion', N'Cuota', N'Lead', N'ConsultaLead', N'Comision', N'PlanesDePago')
ORDER BY NombreTabla;
GO

PRINT 'CUN03 test data script completed.';
GO

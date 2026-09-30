-- A03 Serializacion - Chequeo previo a la DEMO (solo lectura).
-- Ejecutar antes de presentar: cada bloque indica si esta OK o que corregir.
USE [InstitutoVisionEnAccion];
GO
SET NOCOUNT ON;

-- 1) Permisos 68/69 creados y asignados al Arol 1
SELECT N'1) Permisos' AS Chequeo, p.CodigoPatente, p.Nombre,
       CASE WHEN rp.CodigoRol IS NULL THEN N'FALTA asignar al rol 1' ELSE N'OK' END AS Estado
FROM dbo.Patentes p
LEFT JOIN dbo.RolPatente rp ON rp.CodigoPatente = p.CodigoPatente AND rp.CodigoRol = 1
WHERE p.CodigoPatente IN (68, 69);

IF (SELECT COUNT(*) FROM dbo.Patentes WHERE CodigoPatente IN (68, 69)) < 2
    SELECT N'1) Permisos' AS Chequeo, N'FALTAN patentes 68/69: ejecutar 001_permisos.sql' AS Estado;

-- 2) Dias guardados en ingles (si aparece 'Lunes' etc. hay datos de prueba viejos)
SELECT N'2) Dias en comisiones' AS Chequeo, DiaSemana, COUNT(*) AS Cantidad,
       CASE WHEN DiaSemana IN (N'Monday', N'Tuesday', N'Wednesday', N'Thursday', N'Friday', N'Saturday', N'Sunday')
            THEN N'OK' ELSE N'Valor no estandar (la grilla lo tolera, pero conviene corregirlo)' END AS Estado
FROM dbo.Comision
GROUP BY DiaSemana;

-- 3) Planes de pago activos
SELECT N'3) Planes de pago activos' AS Chequeo, IdPlanDePago, Nombre, RecargoPorcentaje
FROM dbo.PlanesDePago WHERE EstadoActivo = 1;

-- 4) Profesores habilitados con disponibilidad (base para registrar comisiones)
SELECT N'4) Curso-Profesor con disponibilidad' AS Chequeo,
       COUNT(DISTINCT CONCAT(cp.IdCurso, N'-', cp.IdProfesor)) AS ParesHabilitados,
       COUNT(DISTINCT CONCAT(dp.IdProfesor, N'-', dp.DiaSemana)) AS ProfesorDiaConDisponibilidad
FROM dbo.CursoProfesor cp
INNER JOIN dbo.Curso c ON c.IdCurso = cp.IdCurso AND c.EstadoActivo = 1
INNER JOIN dbo.Profesor p ON p.IdProfesor = cp.IdProfesor AND p.EstadoActivo = 1
INNER JOIN dbo.DisponibilidadProfesor dp ON dp.IdProfesor = p.IdProfesor AND dp.EstadoActivo = 1
WHERE cp.EstadoActivo = 1;

-- 5) Ultimas comisiones registradas (para mostrar lo guardado desde el XML)
SELECT TOP (10) N'5) Ultimas comisiones' AS Chequeo, c.IdComision, c.Codigo, cu.Nombre AS Curso,
       p.Apellido + N', ' + p.Nombre AS Profesor, c.DiaSemana,
       CONVERT(CHAR(5), c.HoraInicio, 108) AS Desde, CONVERT(CHAR(5), c.HoraFin, 108) AS Hasta,
       c.FechaLimitePago, c.Estado
FROM dbo.Comision c
INNER JOIN dbo.Curso cu ON cu.IdCurso = c.IdCurso
INNER JOIN dbo.Profesor p ON p.IdProfesor = c.IdProfesor
ORDER BY c.IdComision DESC;
GO

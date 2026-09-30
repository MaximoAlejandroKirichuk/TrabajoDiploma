-- A03 Serializacion - Genera un XML de comisiones VALIDO para probar "Deserializar" + "Guardar seleccionadas en BD".
-- Cada comision generada:
--   * usa un curso-profesor habilitado (curso, profesor y relacion activos),
--   * cae dentro de una disponibilidad activa del profesor,
--   * NO se superpone con ninguna comision existente (no eliminada) del profesor,
--   * NO se superpone con las otras comisiones del mismo XML (una sola por profesor y dia),
--   * tiene fecha limite de pago futura y un plan de pago activo.
--
-- USO: ejecutar en SSMS, hacer clic en el resultado XML, "Guardar como" -> Comisiones_OK.xml
-- Solo lee datos: no inserta ni modifica nada.
USE [InstitutoVisionEnAccion];
GO
SET NOCOUNT ON;

DECLARE @CantidadComisiones INT = 5;
DECLARE @Hoy DATE = CONVERT(DATE, GETDATE());
DECLARE @IdPlan INT, @Recargo DECIMAL(9,4);

SELECT TOP (1) @IdPlan = IdPlanDePago, @Recargo = RecargoPorcentaje
FROM dbo.PlanesDePago
WHERE EstadoActivo = 1
ORDER BY IdPlanDePago;

IF @IdPlan IS NULL
BEGIN
    RAISERROR(N'No hay planes de pago activos.', 16, 1);
    RETURN;
END;

IF OBJECT_ID(N'tempdb..#Libres') IS NOT NULL DROP TABLE #Libres;

;WITH Numeros AS
(
    SELECT TOP (24) ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) - 1 AS n
    FROM sys.all_objects
),
Ventanas AS
(
    SELECT cp.IdCurso,
           c.Nombre AS Curso,
           p.IdProfesor,
           p.Apellido + N', ' + p.Nombre AS Profesor,
           dp.DiaSemana,
           dp.HoraInicio AS VentanaInicio,
           DATEDIFF(MINUTE, dp.HoraInicio, dp.HoraFin) AS Largo
    FROM dbo.CursoProfesor cp
    INNER JOIN dbo.Curso c ON c.IdCurso = cp.IdCurso AND c.EstadoActivo = 1
    INNER JOIN dbo.Profesor p ON p.IdProfesor = cp.IdProfesor AND p.EstadoActivo = 1
    INNER JOIN dbo.DisponibilidadProfesor dp ON dp.IdProfesor = p.IdProfesor AND dp.EstadoActivo = 1
    WHERE cp.EstadoActivo = 1
      AND dp.DiaSemana IN (N'Monday', N'Tuesday', N'Wednesday', N'Thursday', N'Friday', N'Saturday', N'Sunday')
),
Candidatos AS
(
    -- Bloques de hasta 2 horas, arrancando cada 1 hora dentro de la ventana de disponibilidad.
    SELECT v.IdCurso, v.Curso, v.IdProfesor, v.Profesor, v.DiaSemana,
           DATEADD(MINUTE, n.n * 60, v.VentanaInicio) AS HoraInicio,
           DATEADD(MINUTE, n.n * 60 + CASE WHEN v.Largo >= 120 THEN 120 ELSE v.Largo END, v.VentanaInicio) AS HoraFin
    FROM Ventanas v
    INNER JOIN Numeros n ON n.n * 60 + CASE WHEN v.Largo >= 120 THEN 120 ELSE v.Largo END <= v.Largo
),
SinSuperposicion AS
(
    SELECT ca.*,
           ROW_NUMBER() OVER (PARTITION BY ca.IdProfesor, ca.DiaSemana ORDER BY ca.HoraInicio, ca.IdCurso) AS Orden
    FROM Candidatos ca
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM dbo.Comision co
        WHERE co.IdProfesor = ca.IdProfesor
          AND co.DiaSemana = ca.DiaSemana
          AND co.Estado <> N'eliminada'
          AND co.HoraInicio < ca.HoraFin
          AND co.HoraFin > ca.HoraInicio
    )
)
SELECT TOP (@CantidadComisiones) *
INTO #Libres
FROM SinSuperposicion
WHERE Orden = 1   -- una sola comision por profesor y dia: no se pisan entre si
ORDER BY IdProfesor, DiaSemana;

IF NOT EXISTS (SELECT 1 FROM #Libres)
BEGIN
    RAISERROR(N'No hay horarios libres: cargue una disponibilidad para un profesor habilitado en un curso (Gestion de profesores / Gestion curso-profesor).', 16, 1);
    RETURN;
END;

SELECT
    N'XML-OK-' + RIGHT(N'0' + CONVERT(NVARCHAR(10), ROW_NUMBER() OVER (ORDER BY l.IdProfesor, l.DiaSemana)), 2) AS Codigo,
    l.IdCurso,
    l.Curso,
    l.IdProfesor,
    l.Profesor,
    l.DiaSemana,
    5 AS CupoMinimo,
    15 AS CupoMaximo,
    CONVERT(CHAR(10), DATEADD(DAY, 30, @Hoy), 23) AS FechaLimitePago,
    CONVERT(CHAR(10), DATEADD(DAY, 35, @Hoy), 23) AS FechaInicio,
    CONVERT(CHAR(10), DATEADD(DAY, 125, @Hoy), 23) AS FechaFin,
    CAST(50000.00 AS DECIMAL(18,2)) AS ArancelBase,
    CAST(50000.00 AS DECIMAL(18,2)) AS MontoMatricula,
    @IdPlan AS IdPlanDePago,
    @Recargo AS RecargoPlanSnapshot,
    N'preapertura' AS Estado,
    CONVERT(CHAR(5), l.HoraInicio, 108) AS HoraInicio,
    CONVERT(CHAR(5), l.HoraFin, 108) AS HoraFin
FROM #Libres l
ORDER BY l.IdProfesor, l.DiaSemana
FOR XML PATH(N'Comision'), ROOT(N'Comisiones');

DROP TABLE #Libres;
GO

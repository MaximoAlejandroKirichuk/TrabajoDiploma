-- Datos de DEMO para CUN07 (pago de cuota) -> CUN08 (resultados) -> CUN09 (certificado).
-- NO borra ni modifica datos existentes: solo AGREGA un set nuevo.
-- Cada ejecucion crea un set nuevo y limpio: DEMO-CUN09-1, DEMO-CUN09-2, ...
-- (ensayar con un set y, antes de la clase, volver a ejecutar para tener otro sin notas ni certificados).
--
-- Crea:
--   * Comision DEMO-CUN09-<n>: curso del profesor Daniel (DNI 28188200), confirmada,
--     inicio hace 75 dias, fin hace 2 dias (ya finalizada => aparece en CUN09).
--   * 6 alumnos inscriptos (activa), sin evaluaciones ni certificados:
--       01 Valentina Gomez  -> todo pagado            (aprueba => se emite certificado)
--       02 Tomas Perez      -> todo pagado            (nota 6 => retencion academica)
--       03 Martina Lopez    -> todo pagado            (aprueba => prueba de folio duplicado)
--       04 Lucas Diaz       -> cuota 2 VENCIDA        (CUN07 la paga => aparece en CUN08)
--       05 Sofia Romero     -> cuota 2 VENCIDA        (no paga => excluida de CUN08)
--       06 Julian Castro    -> cuotas futuras impagas (aprueba => retencion arancelaria)
--
-- Los DVH quedan vacios: DESPUES de ejecutar este script correr
--   UI\bin\Debug\UI.exe /recalcular-integridad
-- para recalcular DVH/DVV con el mismo algoritmo de la aplicacion.
SET NOCOUNT ON;
SET XACT_ABORT ON;

DECLARE @Hoy DATE = CONVERT(date, GETDATE());
DECLARE @IdProfesor INT = (SELECT IdProfesor FROM dbo.Profesor WHERE DNI = N'28188200' AND EstadoActivo = 1);
IF @IdProfesor IS NULL THROW 53200, N'No existe el profesor activo DNI 28188200 (Daniel).', 1;

-- curso: el de una comision previa de Daniel; si no hay, cualquier curso activo
DECLARE @IdCurso INT = (SELECT TOP (1) IdCurso FROM dbo.Comision WHERE IdProfesor = @IdProfesor ORDER BY IdComision);
IF @IdCurso IS NULL SET @IdCurso = (SELECT TOP (1) IdCurso FROM dbo.Curso ORDER BY IdCurso);
IF @IdCurso IS NULL THROW 53201, N'No hay cursos cargados.', 1;

-- plan de 3 cuotas activo (si no hay, el primer plan activo)
DECLARE @IdPlan INT, @NombrePlan NVARCHAR(200), @Recargo DECIMAL(9,4);
SELECT TOP (1) @IdPlan = IdPlanDePago, @NombrePlan = Nombre, @Recargo = RecargoPorcentaje
FROM dbo.PlanesDePago WHERE EstadoActivo = 1
ORDER BY CASE WHEN CantidadCuotas = 3 THEN 0 ELSE 1 END, IdPlanDePago;
IF @IdPlan IS NULL THROW 53202, N'No hay planes de pago activos.', 1;

DECLARE @Set INT = 1 + (SELECT COUNT(*) FROM dbo.Comision WHERE Codigo LIKE N'DEMO-CUN09-%');
DECLARE @Codigo NVARCHAR(30) = N'DEMO-CUN09-' + CONVERT(NVARCHAR(10), @Set);
DECLARE @PrefijoDni VARCHAR(10) = '4170' + RIGHT('00' + CONVERT(VARCHAR(10), @Set), 2);

IF EXISTS (SELECT 1 FROM dbo.Alumno WHERE DNI LIKE @PrefijoDni + '__')
    THROW 53203, N'Ya existen alumnos con el prefijo de DNI de este set. Revisar datos previos.', 1;

DECLARE @Inicio DATE = DATEADD(DAY, -75, @Hoy);
DECLARE @Fin DATE = DATEADD(DAY, -2, @Hoy);
DECLARE @Arancel DECIMAL(18,2) = 90000.00, @Matricula DECIMAL(18,2) = 20000.00;
DECLARE @Total DECIMAL(18,2) = ROUND(@Arancel * (1 + @Recargo / 100), 2);
DECLARE @Cuota DECIMAL(18,2) = ROUND(@Total / 3, 2);
DECLARE @UltimaCuota DECIMAL(18,2) = @Total - 2 * @Cuota;

-- vencimientos: matricula antes del inicio, cuota 2 vencida, cuotas 3 y 4 futuras
DECLARE @VenceMatricula DATE = DATEADD(DAY, -7, @Inicio);
DECLARE @Vence2 DATE = DATEADD(DAY, -20, @Hoy);
DECLARE @Vence3 DATE = DATEADD(DAY, 12, @Hoy);
DECLARE @Vence4 DATE = DATEADD(DAY, 43, @Hoy);

DECLARE @Alumnos TABLE (Nro INT PRIMARY KEY, Nombre NVARCHAR(80), Apellido NVARCHAR(80), PagaHasta INT);
-- PagaHasta = ultima cuota pagada (1 = solo matricula, 2 = hasta cuota 2, 4 = todo)
INSERT @Alumnos VALUES
    (1, N'Valentina', N'Gómez',  4),
    (2, N'Tomás',     N'Pérez',  4),
    (3, N'Martina',   N'López',  4),
    (4, N'Lucas',     N'Díaz',   1),
    (5, N'Sofía',     N'Romero', 1),
    (6, N'Julián',    N'Castro', 2);

BEGIN TRANSACTION;

INSERT INTO dbo.Comision
    (Codigo, IdCurso, IdProfesor, DiaSemana, HoraInicio, HoraFin, CupoMinimo, CupoMaximo,
     FechaLimitePago, FechaInicio, FechaFin, ArancelBase, MontoMatricula, IdPlanDePago,
     RecargoPlanSnapshot, Estado, DVH)
VALUES
    (@Codigo, @IdCurso, @IdProfesor, N'Jueves', '18:00', '20:00', 1, 20,
     DATEADD(DAY, -7, @Inicio), @Inicio, @Fin, @Arancel, @Matricula, @IdPlan,
     @Recargo, N'confirmada', '');
DECLARE @IdComision INT = SCOPE_IDENTITY();

DECLARE @Nro INT = 1, @Nombre NVARCHAR(80), @Apellido NVARCHAR(80), @PagaHasta INT,
        @Dni VARCHAR(20), @IdAlumno INT, @IdSolicitud INT, @IdCuota INT, @n INT,
        @Monto DECIMAL(18,2), @Vence DATE, @Pagada BIT;

WHILE @Nro <= 6
BEGIN
    SELECT @Nombre = Nombre, @Apellido = Apellido, @PagaHasta = PagaHasta FROM @Alumnos WHERE Nro = @Nro;
    SET @Dni = @PrefijoDni + RIGHT('0' + CONVERT(VARCHAR(2), @Nro), 2);

    INSERT INTO dbo.Alumno (DNI, Nombre, Apellido, Email, Telefono, Estado, DVH)
    VALUES (@Dni, @Nombre, @Apellido,
            LOWER(@Nombre + N'.demo' + CONVERT(NVARCHAR(10), @Set) + N'@ejemplo.com'),
            N'11' + RIGHT(@Dni, 8), N'activo', '');
    SET @IdAlumno = SCOPE_IDENTITY();

    INSERT INTO dbo.SolicitudInscripcion
        (Codigo, IdAlumno, IdLeadOrigen, IdComision, IdPlanDePago, RecargoPlanSnapshot,
         FechaSolicitud, Estado, Observaciones, DVH,
         ArancelCursoSnapshot, CantidadCuotasSnapshot, TotalCursoSnapshot,
         FechaPrimerVencimientoSnapshot, NombrePlanSnapshot)
    VALUES
        (N'SI-DEMO' + CONVERT(NVARCHAR(10), @Set) + N'-' + RIGHT(N'0' + CONVERT(NVARCHAR(2), @Nro), 2),
         @IdAlumno, NULL, @IdComision, @IdPlan, @Recargo,
         DATEADD(DAY, -14, CONVERT(datetime, @Inicio)), N'activa', N'Datos de demo CUN07-CUN09', '',
         @Arancel, 3, @Total, @Vence2, @NombrePlan);
    SET @IdSolicitud = SCOPE_IDENTITY();

    SET @n = 1;
    WHILE @n <= 4
    BEGIN
        SET @Monto = CASE @n WHEN 1 THEN @Matricula WHEN 4 THEN @UltimaCuota ELSE @Cuota END;
        SET @Vence = CASE @n WHEN 1 THEN @VenceMatricula WHEN 2 THEN @Vence2 WHEN 3 THEN @Vence3 ELSE @Vence4 END;
        SET @Pagada = CASE WHEN @n <= @PagaHasta THEN 1 ELSE 0 END;

        INSERT INTO dbo.Cuota
            (IdSolicitudInscripcion, NumeroCuota, MontoOriginal, BalanceAdeudado, FechaVencimiento,
             Estado, DVH, TipoConcepto, Periodo)
        VALUES
            (@IdSolicitud, @n, @Monto, CASE WHEN @Pagada = 1 THEN 0 ELSE @Monto END, @Vence,
             CASE WHEN @Pagada = 1 THEN N'pagada' ELSE N'pendiente' END, '',
             CASE WHEN @n = 1 THEN N'matricula' ELSE N'curso' END,
             CASE WHEN @n = 1 THEN NULL ELSE DATEFROMPARTS(YEAR(@Vence), MONTH(@Vence), 1) END);
        SET @IdCuota = SCOPE_IDENTITY();

        IF @Pagada = 1
            INSERT INTO dbo.PagoInscripcion
                (IdAlumno, IdSolicitudInscripcion, IdCuota, MetodoPago, MontoPagado, NumeroReferencia,
                 FechaPago, FechaAcreditacion, DVH)
            VALUES
                (@IdAlumno, @IdSolicitud, @IdCuota, N'transferencia', @Monto,
                 N'DEMO' + CONVERT(NVARCHAR(10), @Set) + N'-' + @Dni + N'-C' + CONVERT(NVARCHAR(2), @n),
                 CASE WHEN @Vence < @Hoy THEN DATEADD(DAY, -2, CONVERT(datetime, @Vence)) ELSE DATEADD(DAY, -3, CONVERT(datetime, @Hoy)) END,
                 CASE WHEN @Vence < @Hoy THEN DATEADD(DAY, -2, CONVERT(datetime, @Vence)) ELSE DATEADD(DAY, -3, CONVERT(datetime, @Hoy)) END,
                 '');
        SET @n += 1;
    END;
    SET @Nro += 1;
END;

COMMIT TRANSACTION;

PRINT N'Set creado: ' + @Codigo + N' (IdComision ' + CONVERT(NVARCHAR(10), @IdComision) + N').';
PRINT N'Ahora ejecutar: UI\bin\Debug\UI.exe /recalcular-integridad';

SELECT c.Codigo, cu.Nombre AS Curso, c.FechaInicio, c.FechaFin, c.Estado
FROM dbo.Comision c JOIN dbo.Curso cu ON cu.IdCurso = c.IdCurso WHERE c.IdComision = @IdComision;

SELECT a.DNI, a.Nombre + N' ' + a.Apellido AS Alumno,
       SUM(q.BalanceAdeudado) AS SaldoTotal,
       SUM(CASE WHEN q.BalanceAdeudado > 0 AND q.FechaVencimiento < @Hoy THEN 1 ELSE 0 END) AS CuotasVencidas
FROM dbo.SolicitudInscripcion si
JOIN dbo.Alumno a ON a.IdAlumno = si.IdAlumno
JOIN dbo.Cuota q ON q.IdSolicitudInscripcion = si.IdSolicitudInscripcion
WHERE si.IdComision = @IdComision
GROUP BY a.DNI, a.Nombre, a.Apellido
ORDER BY a.DNI;

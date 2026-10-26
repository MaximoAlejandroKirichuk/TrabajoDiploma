-- RF2 - Informe de morosidad por alumno y comisión: DATOS DE PRUEBA.
-- Usar SOLO en una base de prueba o con copia de seguridad previa.
--
-- Re-ejecutable: primero borra los datos de prueba anteriores y después los vuelve a crear con fechas relativas a HOY.
-- Limpieza limitada a las tres comisiones, nueve alumnos y nueve inscripciones exactos de este fixture.
-- Si detecta relaciones ajenas a esos identificadores, aborta en vez de borrar datos compartidos.
-- Reutiliza el primer Profesor y el plan de pago con más cuotas (no los modifica).
-- Al final recalcula DVH (solo de las filas de prueba) y DVV (de las tablas tocadas) con la misma
-- canonicalización que IntegridadDAL_83KI, para que el login no detecte inconsistencias.
--
-- Casos (inscripción = código; los días son relativos a hoy):
--   C01 RF2MOR-A activa     sin deuda (todo pagado)                                  -> NO aparece
--   C02 RF2MOR-A activa     solo deuda futura (una cuota vence HOY y otra en 25 días) -> NO aparece
--   C03 RF2MOR-A pendiente  matrícula impaga vencida hace 5 días + 2 cuotas futuras   -> aparece, 5 días   (1-30)
--   C04 RF2MOR-A activa     1 cuota vencida hace 30 días + 1 futura                   -> aparece, 30 días  (1-30)
--   C05 RF2MOR-A activa     1 cuota vencida hace 31 días                              -> aparece, 31 días  (31-60)
--   C06 RF2MOR-B activa     1 cuota vencida hace 61 días                              -> aparece, 61 días  (>60)
--   C07 RF2MOR-B activa     3 cuotas vencidas (90, 60 y 10 días) + 1 futura           -> aparece, 90 días  (>60)
--   C08 RF2MOR-B cancelada  inscripción inactiva con deuda vencida hace 40 días       -> NO aparece
--   C09 RF2MOR-C pendiente  comisión cancelada por falta de quórum, matrícula vencida -> NO aparece
--                           (tampoco aparece RF2MOR-C en el combo de comisiones)
SET NOCOUNT ON;
SET XACT_ABORT ON;
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_PADDING ON;
SET ANSI_WARNINGS ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET ARITHABORT ON;
SET NUMERIC_ROUNDABORT OFF;

IF OBJECT_ID(N'dbo.Cuota', N'U') IS NULL OR COL_LENGTH(N'dbo.Cuota', N'TipoConcepto') IS NULL
   OR COL_LENGTH(N'dbo.SolicitudInscripcion', N'NombrePlanSnapshot') IS NULL OR COL_LENGTH(N'dbo.Comision', N'MontoMatricula') IS NULL
BEGIN
    RAISERROR(N'Falta el esquema de inscripciones y financiación de cuotas (CUN03/CUN05/CUN07). Aplique esas migraciones primero.', 16, 1);
    RETURN;
END;

DECLARE @IdProfesor INT = (SELECT TOP (1) IdProfesor FROM dbo.Profesor ORDER BY IdProfesor);
DECLARE @IdPlan INT, @NombrePlan NVARCHAR(200);
SELECT TOP (1) @IdPlan = IdPlanDePago, @NombrePlan = Nombre FROM dbo.PlanesDePago ORDER BY CantidadCuotas DESC, IdPlanDePago;
IF @IdProfesor IS NULL OR @IdPlan IS NULL
BEGIN
    RAISERROR(N'Faltan datos base: debe existir al menos un Profesor y un Plan de pago.', 16, 1);
    RETURN;
END;

DECLARE @Hoy DATE = CONVERT(date, GETDATE());
DECLARE @Nulo NVARCHAR(1) = NCHAR(8709); -- '∅', mismo marcador de NULL que IntegridadDAL_83KI
DECLARE @PrimerPeriodo DATE = DATEFROMPARTS(YEAR(DATEADD(DAY, -95, @Hoy)), MONTH(DATEADD(DAY, -95, @Hoy)), 1);

BEGIN TRANSACTION;
BEGIN TRY
    -------------------------------------------------------------------------
    -- 1) Limpieza de los datos de prueba anteriores
    -------------------------------------------------------------------------
    DECLARE @ComisionesPrueba TABLE (IdComision INT PRIMARY KEY);
    DECLARE @AlumnosPrueba TABLE (IdAlumno INT PRIMARY KEY);
    DECLARE @InscripcionesPrueba TABLE (IdSolicitudInscripcion INT PRIMARY KEY);
    DECLARE @CursosFixture TABLE (IdCurso INT PRIMARY KEY);

    -- Los identificadores reservados solo acreditan que una fila pertenece al fixture
    -- si coinciden todos los campos identificatorios y las relaciones. Primero recopilar
    -- por claves reservadas para que ningún cambio en un marcador vuelva invisible una fila existente.
    IF EXISTS (SELECT 1 FROM dbo.Curso WHERE Nombre = N'PRUEBA RF2 Morosidad'
               AND (Descripcion <> N'Curso de prueba del informe de morosidad (RF2)' OR CargaHoraria <> 40 OR EstadoActivo <> 1))
        THROW 52974, N'El nombre reservado del curso RF2MOR pertenece a una fila alterada; limpieza cancelada.', 1;
    INSERT @CursosFixture SELECT IdCurso FROM dbo.Curso WHERE Nombre = N'PRUEBA RF2 Morosidad';

    INSERT @ComisionesPrueba
    SELECT IdComision FROM dbo.Comision WHERE Codigo IN (N'RF2MOR-A', N'RF2MOR-B', N'RF2MOR-C');
    IF EXISTS (SELECT 1 FROM dbo.Comision c
               WHERE c.Codigo IN (N'RF2MOR-A', N'RF2MOR-B', N'RF2MOR-C')
                 AND NOT EXISTS (SELECT 1 FROM @CursosFixture f WHERE f.IdCurso = c.IdCurso))
        THROW 52975, N'Un código reservado RF2MOR no está asociado al curso fixture; limpieza cancelada.', 1;

    INSERT @AlumnosPrueba
    SELECT IdAlumno FROM dbo.Alumno
    WHERE DNI IN ('99075001','99075002','99075003','99075004','99075005','99075006','99075007','99075008','99075009');
    IF EXISTS (SELECT 1 FROM dbo.Alumno a
               WHERE a.DNI IN ('99075001','99075002','99075003','99075004','99075005','99075006','99075007','99075008','99075009')
                 AND NOT EXISTS (SELECT 1 FROM (VALUES
                     ('99075001',N'Sin deuda',N'PruebaRF2 C01',N'rf2.c01@example.com',N'1100000001'),
                     ('99075002',N'Deuda solo futura',N'PruebaRF2 C02',N'rf2.c02@example.com',N'1100000002'),
                     ('99075003',N'Matricula impaga',N'PruebaRF2 C03',N'rf2.c03@example.com',N'1100000003'),
                     ('99075004',N'Atraso 30 dias',N'PruebaRF2 C04',N'rf2.c04@example.com',N'1100000004'),
                     ('99075005',N'Atraso 31 dias',N'PruebaRF2 C05',N'rf2.c05@example.com',N'1100000005'),
                     ('99075006',N'Atraso 61 dias',N'PruebaRF2 C06',N'rf2.c06@example.com',N'1100000006'),
                     ('99075007',N'Varias vencidas',N'PruebaRF2 C07',N'rf2.c07@example.com',N'1100000007'),
                     ('99075008',N'Inactiva con deuda',N'PruebaRF2 C08',N'rf2.c08@example.com',N'1100000008'),
                     ('99075009',N'Comision cancelada',N'PruebaRF2 C09',N'rf2.c09@example.com',N'1100000009')
                 ) expected(DNI, Nombre, Apellido, Email, Telefono)
                 WHERE a.DNI = expected.DNI AND a.Nombre = expected.Nombre AND a.Apellido = expected.Apellido
                   AND a.Email = expected.Email AND a.Telefono = expected.Telefono))
        THROW 52976, N'Un DNI reservado RF2MOR pertenece a un alumno alterado; limpieza cancelada.', 1;

    INSERT @InscripcionesPrueba
    SELECT IdSolicitudInscripcion FROM dbo.SolicitudInscripcion
    WHERE Codigo IN (N'RF2MOR-C01',N'RF2MOR-C02',N'RF2MOR-C03',N'RF2MOR-C04',N'RF2MOR-C05',N'RF2MOR-C06',N'RF2MOR-C07',N'RF2MOR-C08',N'RF2MOR-C09');
    IF EXISTS (SELECT 1 FROM dbo.SolicitudInscripcion si
               WHERE si.Codigo IN (N'RF2MOR-C01',N'RF2MOR-C02',N'RF2MOR-C03',N'RF2MOR-C04',N'RF2MOR-C05',N'RF2MOR-C06',N'RF2MOR-C07',N'RF2MOR-C08',N'RF2MOR-C09')
                 AND NOT EXISTS (SELECT 1 FROM (VALUES
                     (N'RF2MOR-C01','99075001',N'RF2MOR-A'),(N'RF2MOR-C02','99075002',N'RF2MOR-A'),
                     (N'RF2MOR-C03','99075003',N'RF2MOR-A'),(N'RF2MOR-C04','99075004',N'RF2MOR-A'),
                     (N'RF2MOR-C05','99075005',N'RF2MOR-A'),(N'RF2MOR-C06','99075006',N'RF2MOR-B'),
                     (N'RF2MOR-C07','99075007',N'RF2MOR-B'),(N'RF2MOR-C08','99075008',N'RF2MOR-B'),
                     (N'RF2MOR-C09','99075009',N'RF2MOR-C')
                 ) expected(Codigo,DNI,Comision)
                 INNER JOIN dbo.Alumno a ON a.DNI = expected.DNI
                 INNER JOIN dbo.Comision c ON c.Codigo = expected.Comision
                 WHERE si.Codigo = expected.Codigo AND si.IdAlumno = a.IdAlumno AND si.IdComision = c.IdComision))
        THROW 52977, N'Un código de inscripción RF2MOR tiene alumno o comisión inesperados; limpieza cancelada.', 1;

    -- Any registration touching a reserved fixture person/commission/course must
    -- itself be one of the reserved registration codes with the expected links.
    IF EXISTS (SELECT 1 FROM dbo.SolicitudInscripcion si
               WHERE (si.IdAlumno IN (SELECT IdAlumno FROM @AlumnosPrueba)
                   OR si.IdComision IN (SELECT IdComision FROM @ComisionesPrueba)
                   OR si.IdComision IN (SELECT c.IdComision FROM dbo.Comision c INNER JOIN @CursosFixture f ON f.IdCurso = c.IdCurso))
                 AND si.IdSolicitudInscripcion NOT IN (SELECT IdSolicitudInscripcion FROM @InscripcionesPrueba))
        THROW 52978, N'Hay inscripciones inesperadas asociadas a identificadores RF2MOR; limpieza cancelada.', 1;


    IF OBJECT_ID(N'dbo.CertificadoFinalizacion', N'U') IS NOT NULL
       AND EXISTS (SELECT 1 FROM dbo.CertificadoFinalizacion WHERE IdInscripcion IN (SELECT IdSolicitudInscripcion FROM @InscripcionesPrueba))
        THROW 52960, N'Hay certificados emitidos sobre inscripciones de prueba RF2MOR (son inmutables). Revise manualmente antes de re-ejecutar.', 1;

    -- Este fixture nunca crea pagos, facturas, evaluaciones ni becas.
    -- Sus vínculos con inscripciones/alumnos no demuestran que le pertenezcan: conservarlos y
    -- abortar antes de cualquier limpieza si existe alguno.
    IF OBJECT_ID(N'dbo.PagoInscripcion', N'U') IS NOT NULL
       AND EXISTS (SELECT 1 FROM dbo.PagoInscripcion WHERE IdSolicitudInscripcion IN (SELECT IdSolicitudInscripcion FROM @InscripcionesPrueba))
        THROW 52968, N'Hay pagos asociados a inscripciones RF2MOR que no pertenecen al fixture; limpieza cancelada.', 1;
    IF OBJECT_ID(N'dbo.FacturaCUN07', N'U') IS NOT NULL
       AND EXISTS (SELECT 1 FROM dbo.FacturaCUN07 f INNER JOIN dbo.PagoInscripcion p ON p.IdPagoInscripcion = f.IdPagoInscripcion
                   WHERE p.IdSolicitudInscripcion IN (SELECT IdSolicitudInscripcion FROM @InscripcionesPrueba))
        THROW 52969, N'Hay facturas asociadas a inscripciones RF2MOR que no pertenecen al fixture; limpieza cancelada.', 1;
    IF OBJECT_ID(N'dbo.ResultadoEvaluacion', N'U') IS NOT NULL
       AND EXISTS (SELECT 1 FROM dbo.ResultadoEvaluacion WHERE IdInscripcion IN (SELECT IdSolicitudInscripcion FROM @InscripcionesPrueba))
        THROW 52970, N'Hay evaluaciones asociadas a inscripciones RF2MOR que no pertenecen al fixture; limpieza cancelada.', 1;
    IF OBJECT_ID(N'dbo.Beca', N'U') IS NOT NULL
       AND EXISTS (SELECT 1 FROM dbo.Beca WHERE IdSolicitudInscripcion IN (SELECT IdSolicitudInscripcion FROM @InscripcionesPrueba))
        THROW 52971, N'Hay becas asociadas a inscripciones RF2MOR que no pertenecen al fixture; limpieza cancelada.', 1;
    -- Las cuotas son los únicos registros dependientes que inserta este fixture. Exigir que cada
    -- registro vinculado coincida con los valores exactos de caso/número/monto/saldo/fecha.
    IF EXISTS (
        SELECT 1 FROM dbo.Cuota cu
        INNER JOIN dbo.SolicitudInscripcion si ON si.IdSolicitudInscripcion = cu.IdSolicitudInscripcion
        WHERE si.IdSolicitudInscripcion IN (SELECT IdSolicitudInscripcion FROM @InscripcionesPrueba)
          AND NOT EXISTS (
              SELECT 1 FROM (VALUES
                  (N'C01',1,15000.00,0.00,-90),(N'C01',2,20000.00,0.00,-60),(N'C01',3,20000.00,0.00,-30),(N'C01',4,20000.00,0.00,-1),
                  (N'C02',1,15000.00,0.00,-90),(N'C02',2,20000.00,20000.00,0),(N'C02',3,20000.00,20000.00,25),
                  (N'C03',1,15000.00,15000.00,-5),(N'C03',2,20000.00,20000.00,25),(N'C03',3,20000.00,20000.00,55),
                  (N'C04',1,15000.00,0.00,-90),(N'C04',2,20000.00,20000.00,-30),(N'C04',3,20000.00,20000.00,1),
                  (N'C05',1,15000.00,0.00,-90),(N'C05',2,20000.00,20000.00,-31),
                  (N'C06',1,15000.00,0.00,-90),(N'C06',2,25000.00,25000.00,-61),
                  (N'C07',1,15000.00,0.00,-95),(N'C07',2,10000.00,10000.00,-90),(N'C07',3,10000.00,10000.00,-60),(N'C07',4,10000.00,10000.00,-10),(N'C07',5,10000.00,10000.00,20),
                  (N'C08',1,15000.00,0.00,-90),(N'C08',2,20000.00,20000.00,-40),
                  (N'C09',1,15000.00,15000.00,-30),(N'C09',2,20000.00,20000.00,30)
              ) expected(Caso, Numero, Monto, Saldo, DiasVto)
              WHERE si.Codigo = N'RF2MOR-' + expected.Caso
                AND cu.NumeroCuota = expected.Numero
                AND cu.TipoConcepto = CASE WHEN expected.Numero = 1 THEN N'matricula' ELSE N'curso' END
                AND cu.MontoOriginal = expected.Monto AND cu.BalanceAdeudado = expected.Saldo
                AND CONVERT(date, cu.FechaVencimiento) = DATEADD(DAY, expected.DiasVto, @Hoy)
          )
    )
        THROW 52972, N'Hay cuotas RF2MOR inesperadas o modificadas; limpieza cancelada.', 1;
    IF EXISTS (
        SELECT si.Codigo, cu.NumeroCuota
        FROM dbo.Cuota cu INNER JOIN dbo.SolicitudInscripcion si ON si.IdSolicitudInscripcion = cu.IdSolicitudInscripcion
        WHERE si.IdSolicitudInscripcion IN (SELECT IdSolicitudInscripcion FROM @InscripcionesPrueba)
        GROUP BY si.Codigo, cu.NumeroCuota HAVING COUNT(*) > 1
    )
        THROW 52973, N'Hay cuotas duplicadas en inscripciones RF2MOR; limpieza cancelada.', 1;
    -- Este fixture no inserta consultas de leads ni actas de comisión, por lo que no se puede
    -- demostrar que esos registros le pertenezcan. Abortar en lugar de eliminarlos por la FK de comisión.
    IF OBJECT_ID(N'dbo.ConsultaLead', N'U') IS NOT NULL
       AND EXISTS (SELECT 1 FROM dbo.ConsultaLead WHERE IdComision IN (SELECT IdComision FROM @ComisionesPrueba))
        THROW 52964, N'Hay consultas de lead asociadas a comisiones RF2MOR; no se puede probar que sean del fixture. Limpieza cancelada.', 1;
    IF OBJECT_ID(N'dbo.AltaOficialComision', N'U') IS NOT NULL
       AND EXISTS (SELECT 1 FROM dbo.AltaOficialComision WHERE IdComision IN (SELECT IdComision FROM @ComisionesPrueba))
        THROW 52965, N'Hay altas oficiales asociadas a comisiones RF2MOR; no se puede probar que sean del fixture. Limpieza cancelada.', 1;
    IF OBJECT_ID(N'dbo.ActaCierreComision', N'U') IS NOT NULL
       AND EXISTS (SELECT 1 FROM dbo.ActaCierreComision WHERE IdComision IN (SELECT IdComision FROM @ComisionesPrueba))
        THROW 52966, N'Hay actas de cierre asociadas a comisiones RF2MOR; no se puede probar que sean del fixture. Limpieza cancelada.', 1;
    IF EXISTS (SELECT 1 FROM dbo.Comision c INNER JOIN @CursosFixture f ON f.IdCurso = c.IdCurso
                 WHERE c.IdComision NOT IN (SELECT IdComision FROM @ComisionesPrueba))
        THROW 52963, N'El curso del fixture contiene comisiones ajenas; limpieza cancelada.', 1;
    -- Este fixture no inserta asociaciones CursoProfesor. Conservar todos los vínculos existentes;
    -- además, un vínculo impide eliminar el curso, así que abortar antes de cualquier acción destructiva.
    IF OBJECT_ID(N'dbo.CursoProfesor', N'U') IS NOT NULL
       AND EXISTS (SELECT 1 FROM dbo.CursoProfesor WHERE IdCurso IN (SELECT IdCurso FROM @CursosFixture))
        THROW 52967, N'El curso RF2MOR tiene asociaciones CursoProfesor que no pertenecen al fixture; limpieza cancelada.', 1;

    -- Ya se completaron todas las comprobaciones de colisiones, relaciones y registros dependientes.
    DELETE cu FROM dbo.Cuota cu
    INNER JOIN dbo.SolicitudInscripcion si ON si.IdSolicitudInscripcion = cu.IdSolicitudInscripcion
    WHERE si.IdSolicitudInscripcion IN (SELECT IdSolicitudInscripcion FROM @InscripcionesPrueba);
    DELETE FROM dbo.SolicitudInscripcion WHERE IdSolicitudInscripcion IN (SELECT IdSolicitudInscripcion FROM @InscripcionesPrueba);
    DELETE FROM dbo.Alumno WHERE IdAlumno IN (SELECT IdAlumno FROM @AlumnosPrueba);
    DELETE FROM dbo.Comision WHERE IdComision IN (SELECT IdComision FROM @ComisionesPrueba);
    DELETE FROM dbo.Curso WHERE IdCurso IN (SELECT IdCurso FROM @CursosFixture);

    -------------------------------------------------------------------------
    -- 2) Definición de los casos
    -------------------------------------------------------------------------
    DECLARE @Comisiones TABLE (Codigo NVARCHAR(30), Estado NVARCHAR(30), DiaSemana NVARCHAR(20), HoraInicio TIME(0), HoraFin TIME(0),
                               DiasLimitePago INT, DiasInicio INT, DiasFin INT);
    INSERT @Comisiones VALUES
        (N'RF2MOR-A', N'confirmada',             N'Monday',    '18:00', '20:00', -100, -95, 120),
        (N'RF2MOR-B', N'confirmada',             N'Wednesday', '18:00', '20:00', -100, -95, 120),
        (N'RF2MOR-C', N'cancelada_falta_quorum', N'Friday',    '18:00', '20:00',  -40, -30,  90);

    DECLARE @Casos TABLE (Caso NVARCHAR(3), Comision NVARCHAR(30), EstadoInscripcion NVARCHAR(30), Nombre NVARCHAR(80));
    INSERT @Casos VALUES
        (N'C01', N'RF2MOR-A', N'activa',    N'Sin deuda'),
        (N'C02', N'RF2MOR-A', N'activa',    N'Deuda solo futura'),
        (N'C03', N'RF2MOR-A', N'pendiente', N'Matricula impaga'),
        (N'C04', N'RF2MOR-A', N'activa',    N'Atraso 30 dias'),
        (N'C05', N'RF2MOR-A', N'activa',    N'Atraso 31 dias'),
        (N'C06', N'RF2MOR-B', N'activa',    N'Atraso 61 dias'),
        (N'C07', N'RF2MOR-B', N'activa',    N'Varias vencidas'),
        (N'C08', N'RF2MOR-B', N'cancelada', N'Inactiva con deuda'),
        (N'C09', N'RF2MOR-C', N'pendiente', N'Comision cancelada');

    -- Numero 1 = matrícula. Saldo 0 = pagada. DiasVto negativo = ya venció.
    DECLARE @Cuotas TABLE (Caso NVARCHAR(3), Numero INT, Monto DECIMAL(18,2), Saldo DECIMAL(18,2), DiasVto INT);
    INSERT @Cuotas VALUES
        (N'C01', 1, 15000, 0, -90), (N'C01', 2, 20000, 0, -60), (N'C01', 3, 20000, 0, -30), (N'C01', 4, 20000, 0, -1),
        (N'C02', 1, 15000, 0, -90), (N'C02', 2, 20000, 20000, 0), (N'C02', 3, 20000, 20000, 25),
        (N'C03', 1, 15000, 15000, -5), (N'C03', 2, 20000, 20000, 25), (N'C03', 3, 20000, 20000, 55),
        (N'C04', 1, 15000, 0, -90), (N'C04', 2, 20000, 20000, -30), (N'C04', 3, 20000, 20000, 1),
        (N'C05', 1, 15000, 0, -90), (N'C05', 2, 20000, 20000, -31),
        (N'C06', 1, 15000, 0, -90), (N'C06', 2, 25000, 25000, -61),
        (N'C07', 1, 15000, 0, -95), (N'C07', 2, 10000, 10000, -90), (N'C07', 3, 10000, 10000, -60), (N'C07', 4, 10000, 10000, -10), (N'C07', 5, 10000, 10000, 20),
        (N'C08', 1, 15000, 0, -90), (N'C08', 2, 20000, 20000, -40),
        (N'C09', 1, 15000, 15000, -30), (N'C09', 2, 20000, 20000, 30);

    -------------------------------------------------------------------------
    -- 3) Inserción
    -------------------------------------------------------------------------
    INSERT dbo.Curso (Nombre, Descripcion, CargaHoraria, EstadoActivo, DVH)
    VALUES (N'PRUEBA RF2 Morosidad', N'Curso de prueba del informe de morosidad (RF2)', 40, 1, N'');
    DECLARE @IdCurso INT = SCOPE_IDENTITY();

    INSERT dbo.Comision (Codigo, IdCurso, IdProfesor, DiaSemana, HoraInicio, HoraFin, CupoMinimo, CupoMaximo,
                         FechaLimitePago, FechaInicio, FechaFin, ArancelBase, MontoMatricula, IdPlanDePago, RecargoPlanSnapshot, Estado, DVH)
    SELECT Codigo, @IdCurso, @IdProfesor, DiaSemana, HoraInicio, HoraFin, 1, 20,
           DATEADD(DAY, DiasLimitePago, @Hoy), DATEADD(DAY, DiasInicio, @Hoy), DATEADD(DAY, DiasFin, @Hoy),
           120000.00, 15000.00, @IdPlan, 0, Estado, N''
    FROM @Comisiones;

    INSERT dbo.Alumno (DNI, Nombre, Apellido, Email, Telefono, FechaAlta, Estado, DVH)
    SELECT '990750' + RIGHT(Caso, 2), Nombre, N'PruebaRF2 ' + Caso, N'rf2.' + LOWER(Caso) + N'@example.com',
           N'11000000' + RIGHT(Caso, 2), CAST(DATEADD(DAY, -120, @Hoy) AS DATETIME), N'activo', N''
    FROM @Casos;

    INSERT dbo.SolicitudInscripcion (Codigo, IdAlumno, IdLeadOrigen, IdComision, IdPlanDePago, RecargoPlanSnapshot, FechaSolicitud,
                                     Estado, Observaciones, DVH, ArancelCursoSnapshot, CantidadCuotasSnapshot, TotalCursoSnapshot,
                                     FechaPrimerVencimientoSnapshot, NombrePlanSnapshot)
    SELECT N'RF2MOR-' + ca.Caso, a.IdAlumno, NULL, c.IdComision, @IdPlan, 0, CAST(DATEADD(DAY, -100, @Hoy) AS DATETIME),
           ca.EstadoInscripcion, N'Dato de prueba RF2 morosidad', N'', 120000.00,
           (SELECT COUNT(*) FROM @Cuotas q WHERE q.Caso = ca.Caso AND q.Numero > 1),
           (SELECT ISNULL(SUM(q.Monto), 0) FROM @Cuotas q WHERE q.Caso = ca.Caso AND q.Numero > 1),
           (SELECT DATEADD(DAY, MIN(q.DiasVto), @Hoy) FROM @Cuotas q WHERE q.Caso = ca.Caso AND q.Numero = 2),
           @NombrePlan
    FROM @Casos ca
    INNER JOIN dbo.Alumno a ON a.DNI = '990750' + RIGHT(ca.Caso, 2)
    INNER JOIN dbo.Comision c ON c.Codigo = ca.Comision;

    INSERT dbo.Cuota (IdSolicitudInscripcion, NumeroCuota, TipoConcepto, Periodo, MontoOriginal, BalanceAdeudado, FechaVencimiento, Estado, DVH)
    SELECT si.IdSolicitudInscripcion, q.Numero,
           CASE WHEN q.Numero = 1 THEN N'matricula' ELSE N'curso' END,
           CASE WHEN q.Numero = 1 THEN NULL ELSE DATEADD(MONTH, q.Numero - 2, @PrimerPeriodo) END,
           q.Monto, q.Saldo, DATEADD(DAY, q.DiasVto, @Hoy),
           CASE WHEN q.Saldo = 0 THEN N'pagada' ELSE N'pendiente' END, N''
    FROM @Cuotas q
    INNER JOIN dbo.SolicitudInscripcion si ON si.Codigo = N'RF2MOR-' + q.Caso;

    -------------------------------------------------------------------------
    -- 4) DVH de las filas de prueba (orden alfabético de columnas de IntegridadTablaConfig_83KI;
    --    fechas yyyy-MM-ddTHH:mm:ss.fff, TIME hh:mm:ss, NULL = '∅')
    -------------------------------------------------------------------------
    UPDATE dbo.Curso SET DVH = LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', CONCAT(
        N'CargaHoraria=', CONVERT(NVARCHAR(20), CargaHoraria),
        N'|Descripcion=', ISNULL(LTRIM(RTRIM(Descripcion)), @Nulo),
        N'|EstadoActivo=', CONVERT(NVARCHAR(1), EstadoActivo),
        N'|IdCurso=', CONVERT(NVARCHAR(20), IdCurso),
        N'|Nombre=', ISNULL(LTRIM(RTRIM(Nombre)), @Nulo))), 2))
    WHERE IdCurso = @IdCurso;

    UPDATE dbo.Comision SET DVH = LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', CONCAT(
        N'ArancelBase=', CONVERT(NVARCHAR(40), ArancelBase),
        N'|Codigo=', ISNULL(LTRIM(RTRIM(Codigo)), @Nulo),
        N'|CupoMaximo=', CONVERT(NVARCHAR(20), CupoMaximo),
        N'|CupoMinimo=', CONVERT(NVARCHAR(20), CupoMinimo),
        N'|DiaSemana=', ISNULL(LTRIM(RTRIM(DiaSemana)), @Nulo),
        N'|Estado=', ISNULL(LTRIM(RTRIM(Estado)), @Nulo),
        N'|FechaFin=', ISNULL(REPLACE(CONVERT(NVARCHAR(23), CAST(FechaFin AS DATETIME), 121), N' ', N'T'), @Nulo),
        N'|FechaInicio=', ISNULL(REPLACE(CONVERT(NVARCHAR(23), CAST(FechaInicio AS DATETIME), 121), N' ', N'T'), @Nulo),
        N'|FechaLimitePago=', ISNULL(REPLACE(CONVERT(NVARCHAR(23), CAST(FechaLimitePago AS DATETIME), 121), N' ', N'T'), @Nulo),
        N'|HoraFin=', ISNULL(CONVERT(NVARCHAR(8), HoraFin, 108), @Nulo),
        N'|HoraInicio=', ISNULL(CONVERT(NVARCHAR(8), HoraInicio, 108), @Nulo),
        N'|IdComision=', CONVERT(NVARCHAR(20), IdComision),
        N'|IdCurso=', CONVERT(NVARCHAR(20), IdCurso),
        N'|IdPlanDePago=', ISNULL(CONVERT(NVARCHAR(20), IdPlanDePago), @Nulo),
        N'|IdProfesor=', CONVERT(NVARCHAR(20), IdProfesor),
        N'|MontoMatricula=', ISNULL(CONVERT(NVARCHAR(40), MontoMatricula), @Nulo),
        N'|RecargoPlanSnapshot=', CONVERT(NVARCHAR(40), RecargoPlanSnapshot))), 2))
    WHERE Codigo IN (N'RF2MOR-A', N'RF2MOR-B', N'RF2MOR-C');

    UPDATE dbo.Alumno SET DVH = LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', CONCAT(
        N'Apellido=', ISNULL(LTRIM(RTRIM(Apellido)), @Nulo),
        N'|DNI=', ISNULL(LTRIM(RTRIM(DNI)), @Nulo),
        N'|Email=', ISNULL(LTRIM(RTRIM(Email)), @Nulo),
        N'|Estado=', ISNULL(LTRIM(RTRIM(Estado)), @Nulo),
        N'|FechaAlta=', ISNULL(REPLACE(CONVERT(NVARCHAR(23), FechaAlta, 121), N' ', N'T'), @Nulo),
        N'|IdAlumno=', CONVERT(NVARCHAR(20), IdAlumno),
        N'|Nombre=', ISNULL(LTRIM(RTRIM(Nombre)), @Nulo),
        N'|Telefono=', ISNULL(LTRIM(RTRIM(Telefono)), @Nulo))), 2))
    WHERE IdAlumno IN (SELECT IdAlumno FROM @AlumnosPrueba);

    UPDATE dbo.SolicitudInscripcion SET DVH = LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', CONCAT(
        N'ArancelCursoSnapshot=', ISNULL(CONVERT(NVARCHAR(40), ArancelCursoSnapshot), @Nulo),
        N'|CantidadCuotasSnapshot=', ISNULL(CONVERT(NVARCHAR(20), CantidadCuotasSnapshot), @Nulo),
        N'|Codigo=', ISNULL(LTRIM(RTRIM(Codigo)), @Nulo),
        N'|Estado=', ISNULL(LTRIM(RTRIM(Estado)), @Nulo),
        N'|FechaPrimerVencimientoSnapshot=', ISNULL(REPLACE(CONVERT(NVARCHAR(23), CAST(FechaPrimerVencimientoSnapshot AS DATETIME), 121), N' ', N'T'), @Nulo),
        N'|FechaSolicitud=', ISNULL(REPLACE(CONVERT(NVARCHAR(23), FechaSolicitud, 121), N' ', N'T'), @Nulo),
        N'|IdAlumno=', CONVERT(NVARCHAR(20), IdAlumno),
        N'|IdComision=', CONVERT(NVARCHAR(20), IdComision),
        N'|IdLeadOrigen=', ISNULL(CONVERT(NVARCHAR(20), IdLeadOrigen), @Nulo),
        N'|IdPlanDePago=', CONVERT(NVARCHAR(20), IdPlanDePago),
        N'|IdSolicitudInscripcion=', CONVERT(NVARCHAR(20), IdSolicitudInscripcion),
        N'|NombrePlanSnapshot=', ISNULL(LTRIM(RTRIM(NombrePlanSnapshot)), @Nulo),
        N'|Observaciones=', ISNULL(LTRIM(RTRIM(Observaciones)), @Nulo),
        N'|RecargoPlanSnapshot=', CONVERT(NVARCHAR(40), RecargoPlanSnapshot),
        N'|TotalCursoSnapshot=', ISNULL(CONVERT(NVARCHAR(40), TotalCursoSnapshot), @Nulo))), 2))
    WHERE Codigo IN (N'RF2MOR-C01',N'RF2MOR-C02',N'RF2MOR-C03',N'RF2MOR-C04',N'RF2MOR-C05',N'RF2MOR-C06',N'RF2MOR-C07',N'RF2MOR-C08',N'RF2MOR-C09');

    UPDATE cu SET DVH = LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', CONCAT(
        N'BalanceAdeudado=', CONVERT(NVARCHAR(40), cu.BalanceAdeudado),
        N'|Estado=', ISNULL(LTRIM(RTRIM(cu.Estado)), @Nulo),
        N'|FechaVencimiento=', ISNULL(REPLACE(CONVERT(NVARCHAR(23), CAST(cu.FechaVencimiento AS DATETIME), 121), N' ', N'T'), @Nulo),
        N'|IdCuota=', CONVERT(NVARCHAR(20), cu.IdCuota),
        N'|IdSolicitudInscripcion=', CONVERT(NVARCHAR(20), cu.IdSolicitudInscripcion),
        N'|MontoOriginal=', CONVERT(NVARCHAR(40), cu.MontoOriginal),
        N'|NumeroCuota=', CONVERT(NVARCHAR(20), cu.NumeroCuota),
        N'|Periodo=', ISNULL(REPLACE(CONVERT(NVARCHAR(23), CAST(cu.Periodo AS DATETIME), 121), N' ', N'T'), @Nulo),
        N'|TipoConcepto=', ISNULL(LTRIM(RTRIM(cu.TipoConcepto)), @Nulo))), 2))
    FROM dbo.Cuota cu
    INNER JOIN dbo.SolicitudInscripcion si ON si.IdSolicitudInscripcion = cu.IdSolicitudInscripcion
    WHERE si.Codigo IN (N'RF2MOR-C01',N'RF2MOR-C02',N'RF2MOR-C03',N'RF2MOR-C04',N'RF2MOR-C05',N'RF2MOR-C06',N'RF2MOR-C07',N'RF2MOR-C08',N'RF2MOR-C09');

    -------------------------------------------------------------------------
    -- 5) DVV de cada tabla tocada (alta o baja de filas), desde los DVH guardados
    -------------------------------------------------------------------------
    DECLARE @Tablas TABLE (Nombre SYSNAME);
    INSERT @Tablas VALUES (N'Curso'), (N'Comision'), (N'Alumno'), (N'SolicitudInscripcion'), (N'Cuota');
    DECLARE @Tabla SYSNAME, @Sql NVARCHAR(MAX);
    DECLARE curTablas CURSOR LOCAL FAST_FORWARD FOR
        SELECT Nombre FROM @Tablas WHERE OBJECT_ID(N'dbo.' + Nombre, N'U') IS NOT NULL;
    OPEN curTablas;
    FETCH NEXT FROM curTablas INTO @Tabla;
    WHILE @@FETCH_STATUS = 0
    BEGIN
        -- @Tabla sale de la lista fija de arriba, no de datos del usuario
        SET @Sql = N'DECLARE @h NVARCHAR(MAX) = COALESCE((SELECT CONVERT(NVARCHAR(MAX), DVH) AS [text()] FROM dbo.' + QUOTENAME(@Tabla) +
                   N' ORDER BY DVH FOR XML PATH(''''), TYPE).value(''.'', ''nvarchar(max)''), N'''');
                     UPDATE dbo.DigitoVerificador_83KI SET DVV = LOWER(CONVERT(VARCHAR(64), HASHBYTES(''SHA2_256'', @h), 2)), FechaActualizacion = GETDATE()
                     WHERE NombreTabla = @tabla;';
        EXEC sp_executesql @Sql, N'@tabla SYSNAME', @tabla = @Tabla;
        FETCH NEXT FROM curTablas INTO @Tabla;
    END;
    CLOSE curTablas;
    DEALLOCATE curTablas;

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;

PRINT N'Datos de prueba RF2 (morosidad) creados. Hoy = ' + CONVERT(NVARCHAR(10), @Hoy, 103);
PRINT N'Esperado en el informe (filtro Curso = PRUEBA RF2 Morosidad), en este orden:';
PRINT N'  RF2MOR-C07  3 cuotas  90 días  deuda vencida 30.000,00  deuda total 40.000,00  (>60)';
PRINT N'  RF2MOR-C06  1 cuota   61 días  deuda vencida 25.000,00  deuda total 25.000,00  (>60)';
PRINT N'  RF2MOR-C05  1 cuota   31 días  deuda vencida 20.000,00  deuda total 20.000,00  (31-60)';
PRINT N'  RF2MOR-C04  1 cuota   30 días  deuda vencida 20.000,00  deuda total 40.000,00  (1-30)';
PRINT N'  RF2MOR-C03  1 cuota    5 días  deuda vencida 15.000,00  deuda total 55.000,00  (1-30, matrícula)';
PRINT N'No deben aparecer: C01, C02, C08, C09. Torta: 1-30 = 2, 31-60 = 1, >60 = 2.';

-- Control rápido de lo creado
SELECT si.Codigo, si.Estado AS EstadoInscripcion, c.Codigo AS Comision, c.Estado AS EstadoComision,
       cu.NumeroCuota, cu.TipoConcepto, cu.BalanceAdeudado, cu.FechaVencimiento, DATEDIFF(DAY, cu.FechaVencimiento, @Hoy) AS DiasDesdeVencimiento
FROM dbo.SolicitudInscripcion si
INNER JOIN dbo.Comision c ON c.IdComision = si.IdComision
INNER JOIN dbo.Cuota cu ON cu.IdSolicitudInscripcion = si.IdSolicitudInscripcion
WHERE si.Codigo IN (N'RF2MOR-C01',N'RF2MOR-C02',N'RF2MOR-C03',N'RF2MOR-C04',N'RF2MOR-C05',N'RF2MOR-C06',N'RF2MOR-C07',N'RF2MOR-C08',N'RF2MOR-C09')
ORDER BY si.Codigo, cu.NumeroCuota;

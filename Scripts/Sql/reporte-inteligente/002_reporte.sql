CREATE OR ALTER PROCEDURE dbo.sp_Reporte_Inteligente
    @Desde date=NULL,@Hasta date=NULL,@IdCurso int=NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF @Desde>@Hasta THROW 53013,N'Inteligente.FechasInvalidas',1;
    DECLARE @Hoy date=CONVERT(date,GETDATE());
    SELECT @Hoy FechaCenso;

    -- Cada evento histórico usa su fecha; los rótulos no deben sugerir una reconstrucción de estados pasados.
    SELECT c.IdCurso,c.Nombre Curso,
        (SELECT COUNT(*) FROM dbo.SolicitudInscripcion si JOIN dbo.Comision co ON co.IdComision=si.IdComision
         WHERE co.IdCurso=c.IdCurso AND si.Estado IN(N'pendiente',N'confirmada',N'activa')
         AND (@Desde IS NULL OR si.FechaSolicitud>=@Desde) AND (@Hasta IS NULL OR CONVERT(date,si.FechaSolicitud)<=@Hasta)) Registradas,
        (SELECT COUNT(*) FROM dbo.SolicitudInscripcion si JOIN dbo.Comision co ON co.IdComision=si.IdComision
         WHERE co.IdCurso=c.IdCurso AND si.Estado IN(N'confirmada',N'activa')
         AND (@Desde IS NULL OR si.FechaSolicitud>=@Desde) AND (@Hasta IS NULL OR CONVERT(date,si.FechaSolicitud)<=@Hasta)) Concretadas,
        (SELECT COUNT(*) FROM dbo.SolicitudInscripcion si JOIN dbo.Comision co ON co.IdComision=si.IdComision
         WHERE co.IdCurso=c.IdCurso AND si.Estado=N'cancelada'
         AND (@Desde IS NULL OR si.FechaSolicitud>=@Desde) AND (@Hasta IS NULL OR CONVERT(date,si.FechaSolicitud)<=@Hasta)) Canceladas,
        (SELECT COUNT(*) FROM dbo.SolicitudInscripcion si JOIN dbo.Comision co ON co.IdComision=si.IdComision
         WHERE co.IdCurso=c.IdCurso AND si.Estado=N'anulada'
         AND (@Desde IS NULL OR si.FechaSolicitud>=@Desde) AND (@Hasta IS NULL OR CONVERT(date,si.FechaSolicitud)<=@Hasta)) Anuladas,
        (SELECT COUNT(DISTINCT l.IdLead) FROM dbo.Lead l JOIN dbo.ConsultaLead q ON q.IdLead=l.IdLead
         JOIN dbo.Comision co ON co.IdComision=q.IdComision WHERE co.IdCurso=c.IdCurso
         AND (@Desde IS NULL OR l.FechaAlta>=@Desde) AND (@Hasta IS NULL OR CONVERT(date,l.FechaAlta)<=@Hasta)) LeadsUnicos,
        (SELECT COUNT(*) FROM dbo.ConsultaLead q JOIN dbo.Comision co ON co.IdComision=q.IdComision
         WHERE co.IdCurso=c.IdCurso AND (@Desde IS NULL OR q.FechaConsulta>=@Desde) AND (@Hasta IS NULL OR CONVERT(date,q.FechaConsulta)<=@Hasta)) Consultas
    FROM dbo.Curso c WHERE (@IdCurso IS NULL OR c.IdCurso=@IdCurso) ORDER BY c.Nombre;

    -- Definición común de plazas ocupadas y matrículas regularizadas según CUN06.
    SELECT co.IdComision,co.IdCurso,c.Nombre Curso,co.Codigo Comision,co.Estado,co.CupoMinimo,co.CupoMaximo,co.FechaInicio,
        CONVERT(int,(SELECT COUNT(*) FROM dbo.SolicitudInscripcion si WHERE si.IdComision=co.IdComision AND si.Estado IN(N'pendiente',N'confirmada',N'activa'))) Ocupadas,
        CONVERT(int,(SELECT COUNT(*) FROM dbo.SolicitudInscripcion si JOIN dbo.Cuota cu ON cu.IdSolicitudInscripcion=si.IdSolicitudInscripcion
            AND cu.TipoConcepto=N'matricula' AND cu.Estado=N'pagada' WHERE si.IdComision=co.IdComision AND si.Estado=N'activa')) Regularizadas,
        CAST(CASE WHEN co.Estado<>N'eliminada' AND co.FechaLimitePago>=@Hoy AND co.MontoMatricula>0
            AND EXISTS(SELECT 1 FROM dbo.PlanesDePago pp WHERE pp.IdPlanDePago=co.IdPlanDePago AND pp.EstadoActivo=1)
            AND co.CupoMaximo>(SELECT COUNT(*) FROM dbo.SolicitudInscripcion si WHERE si.IdComision=co.IdComision AND si.Estado IN(N'pendiente',N'confirmada',N'activa'))
            THEN 1 ELSE 0 END AS bit) Disponible
    INTO #Comisiones FROM dbo.Comision co JOIN dbo.Curso c ON c.IdCurso=co.IdCurso WHERE (@IdCurso IS NULL OR c.IdCurso=@IdCurso);

    SELECT IdCurso,Curso,Comision,Estado,CupoMinimo,CupoMaximo,Ocupadas,Regularizadas,Disponible
    FROM #Comisiones WHERE (@Desde IS NULL OR FechaInicio>=@Desde) AND (@Hasta IS NULL OR FechaInicio<=@Hasta)
    ORDER BY Curso,Comision;

    -- La oferta actual muestra todas las comisiones disponibles, no una comisión histórica arbitraria.
    SELECT x.IdCurso,x.Curso,x.Comision,x.Estado,x.CupoMinimo,x.CupoMaximo,x.Ocupadas,x.Regularizadas,x.Disponible
    FROM #Comisiones x JOIN dbo.Curso c ON c.IdCurso=x.IdCurso WHERE x.Disponible=1 AND c.EstadoActivo=1
    UNION ALL
    SELECT c.IdCurso,c.Nombre,CAST(NULL AS nvarchar(30)),CAST(NULL AS nvarchar(30)),c.CupoMinimoReferencia,0,0,0,CAST(0 AS bit)
    FROM dbo.Curso c WHERE c.EstadoActivo=1 AND (@IdCurso IS NULL OR c.IdCurso=@IdCurso)
        AND EXISTS(SELECT 1 FROM dbo.RecorridoCurso r JOIN dbo.Curso p ON p.IdCurso=r.IdCursoPrevio AND p.EstadoActivo=1 WHERE r.IdCursoRecomendado=c.IdCurso)
        AND NOT EXISTS(SELECT 1 FROM #Comisiones x WHERE x.IdCurso=c.IdCurso AND x.Disponible=1)
    ORDER BY Curso,Comision;

    ;WITH Aprobados AS(
        SELECT si.IdAlumno,co.IdCurso
        FROM dbo.SolicitudInscripcion si JOIN dbo.Comision co ON co.IdComision=si.IdComision
        JOIN dbo.ResultadoEvaluacion re ON re.IdInscripcion=si.IdSolicitudInscripcion AND re.IdComision=si.IdComision
        WHERE co.FechaFin<=@Hoy AND co.Estado NOT IN(N'eliminada',N'cancelada_falta_quorum') AND si.Estado IN(N'pendiente',N'confirmada',N'activa')
        GROUP BY si.IdAlumno,co.IdCurso,si.IdSolicitudInscripcion HAVING COUNT(*)=2 AND MIN(re.Calificacion)>=7
    ), Candidatos AS(
        SELECT a.IdAlumno,r.IdCursoPrevio,r.IdCursoRecomendado,
            ROW_NUMBER() OVER(PARTITION BY a.IdAlumno,r.IdCursoRecomendado ORDER BY r.Orden,r.IdCursoPrevio) posicion
        FROM Aprobados a JOIN dbo.RecorridoCurso r ON r.IdCursoPrevio=a.IdCurso
        JOIN dbo.Curso p ON p.IdCurso=r.IdCursoPrevio AND p.EstadoActivo=1
        JOIN dbo.Curso s ON s.IdCurso=r.IdCursoRecomendado AND s.EstadoActivo=1
        WHERE (@IdCurso IS NULL OR r.IdCursoRecomendado=@IdCurso)
        AND NOT EXISTS(SELECT 1 FROM dbo.SolicitudInscripcion si JOIN dbo.Comision co ON co.IdComision=si.IdComision
            WHERE si.IdAlumno=a.IdAlumno AND co.IdCurso=r.IdCursoRecomendado AND si.Estado IN(N'pendiente',N'confirmada',N'activa'))
    )
    SELECT c.IdAlumno,c.IdCursoRecomendado,a.Apellido+N', '+a.Nombre Alumno,a.Email,a.Telefono,p.Nombre CursoAprobado,s.Nombre CursoRecomendado,
        COALESCE(d.DeudaVencida,0) DeudaVencida,COALESCE(d.SaldoFuturo,0) SaldoFuturo,COALESCE(DATEDIFF(day,d.MasAntigua,@Hoy),0) DiasAtraso
    FROM Candidatos c JOIN dbo.Alumno a ON a.IdAlumno=c.IdAlumno JOIN dbo.Curso p ON p.IdCurso=c.IdCursoPrevio
    JOIN dbo.Curso s ON s.IdCurso=c.IdCursoRecomendado
    OUTER APPLY(SELECT SUM(CASE WHEN cu.FechaVencimiento<@Hoy THEN cu.BalanceAdeudado ELSE 0 END) DeudaVencida,
        SUM(CASE WHEN cu.FechaVencimiento>=@Hoy THEN cu.BalanceAdeudado ELSE 0 END) SaldoFuturo,
        MIN(CASE WHEN cu.FechaVencimiento<@Hoy THEN cu.FechaVencimiento END) MasAntigua
        FROM dbo.Cuota cu JOIN dbo.SolicitudInscripcion si ON si.IdSolicitudInscripcion=cu.IdSolicitudInscripcion
        JOIN dbo.Comision co ON co.IdComision=si.IdComision WHERE si.IdAlumno=c.IdAlumno
        AND si.Estado IN(N'pendiente',N'confirmada',N'activa') AND co.Estado NOT IN(N'cancelada_falta_quorum',N'eliminada')
        AND cu.Estado<>N'reintegro_pendiente' AND cu.BalanceAdeudado>0) d
    WHERE c.posicion=1 ORDER BY s.Nombre,a.Apellido,a.Nombre;

    -- El período arancelario usa el vencimiento de la cuota. La puntualidad requiere un pago registrado.
    SELECT c.IdCurso,c.Nombre Curso,
        SUM(CASE WHEN cu.FechaVencimiento<@Hoy AND cu.BalanceAdeudado>0 THEN cu.BalanceAdeudado ELSE 0 END) DeudaVencida,
        SUM(CASE WHEN cu.BalanceAdeudado>0 THEN cu.BalanceAdeudado ELSE 0 END) DeudaTotal,
        SUM(CASE WHEN cu.FechaVencimiento<@Hoy AND cu.BalanceAdeudado>0 THEN 1 ELSE 0 END) CuotasVencidas,
        MAX(CASE WHEN cu.FechaVencimiento<@Hoy AND cu.BalanceAdeudado>0 THEN DATEDIFF(day,cu.FechaVencimiento,@Hoy) ELSE 0 END) DiasAtrasoMaximo,
        SUM(CASE WHEN cu.Estado=N'pagada' AND pago.FechaPago IS NOT NULL THEN 1 ELSE 0 END) Pagadas,
        SUM(CASE WHEN cu.Estado=N'pagada' AND CONVERT(date,pago.FechaPago)<=cu.FechaVencimiento THEN 1 ELSE 0 END) PagadasEnTermino
    FROM dbo.Curso c JOIN dbo.Comision co ON co.IdCurso=c.IdCurso
    JOIN dbo.SolicitudInscripcion si ON si.IdComision=co.IdComision
    JOIN dbo.Cuota cu ON cu.IdSolicitudInscripcion=si.IdSolicitudInscripcion
    OUTER APPLY(SELECT MAX(p.FechaPago) FechaPago FROM dbo.PagoInscripcion p WHERE p.IdCuota=cu.IdCuota) pago
    WHERE (@IdCurso IS NULL OR c.IdCurso=@IdCurso) AND si.Estado IN(N'pendiente',N'confirmada',N'activa')
        AND co.Estado NOT IN(N'eliminada',N'cancelada_falta_quorum') AND cu.Estado<>N'reintegro_pendiente'
        AND (@Desde IS NULL OR cu.FechaVencimiento>=@Desde) AND (@Hasta IS NULL OR cu.FechaVencimiento<=@Hasta)
    GROUP BY c.IdCurso,c.Nombre ORDER BY c.Nombre;
END;
GO

USE [InstitutoVisionEnAccion]
GO

IF OBJECT_ID(N'dbo.Comision', N'U') IS NULL
BEGIN
    RAISERROR(N'dbo.Comision no existe en esta base: use EsquemaCompleto.sql para una base nueva.', 16, 1);
    RETURN;
END
GO

IF OBJECT_ID(N'dbo.Curso', N'U') IS NULL
BEGIN
    RAISERROR(N'dbo.Curso no existe en esta base: use EsquemaCompleto.sql para una base nueva.', 16, 1);
    RETURN;
END
GO

IF OBJECT_ID(N'dbo.Profesor', N'U') IS NULL
BEGIN
    RAISERROR(N'dbo.Profesor no existe en esta base: use EsquemaCompleto.sql para una base nueva.', 16, 1);
    RETURN;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_CUN02_ListarComisionesPreapertura
AS
BEGIN
    SET NOCOUNT ON;
    SELECT c.IdComision, c.Codigo, c.IdCurso, cu.Nombre AS Curso, c.IdProfesor,
           CONCAT(p.Apellido, N', ', p.Nombre) AS Profesor, c.DiaSemana, c.HoraInicio, c.HoraFin,
           c.CupoMinimo, c.CupoMaximo, c.FechaLimitePago, c.FechaInicio, c.FechaFin,
           c.ArancelBase, c.Estado
    FROM dbo.Comision c
    INNER JOIN dbo.Curso cu ON cu.IdCurso = c.IdCurso
    INNER JOIN dbo.Profesor p ON p.IdProfesor = c.IdProfesor
    WHERE c.Estado = N'preapertura'
      AND c.FechaLimitePago >= CONVERT(date, GETDATE())
    ORDER BY c.FechaInicio DESC, c.Codigo;
END
GO

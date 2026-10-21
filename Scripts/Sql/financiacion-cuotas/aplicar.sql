:On Error exit
:setvar RepoRoot "."

-- Run in SSMS with Query > SQLCMD Mode enabled.
-- Confirm the selected database is the intended application database.
-- In SSMS set RepoRoot above to the absolute path of your clone; in sqlcmd run from its root.
SET XACT_ABORT ON;
BEGIN TRANSACTION;
GO

:r $(RepoRoot)\Scripts\Sql\financiacion-cuotas\001_modelo.sql
:r $(RepoRoot)\Scripts\Sql\cun03\001_migracion.sql
:r $(RepoRoot)\Scripts\Sql\cun05\001_migracion.sql
:r $(RepoRoot)\Scripts\Sql\cun06\001_migracion.sql
:r $(RepoRoot)\Scripts\Sql\reporte-ocupacion-comisiones\001_migracion.sql
:r $(RepoRoot)\Scripts\Sql\cun07\001_migracion.sql
:r $(RepoRoot)\Scripts\Sql\financiacion-cuotas\002_completar_inscripciones.sql

COMMIT TRANSACTION;
GO

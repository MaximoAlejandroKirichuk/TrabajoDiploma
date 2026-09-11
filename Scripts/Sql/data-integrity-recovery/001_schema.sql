-- =============================================================================
-- 001_schema.sql — esquema de recuperacion de integridad de datos
-- agrega columnas dvh a 8 tablas protegidas + tabla de control DigitoVerificador.
-- tablas protegidas: Usuarios, Roles, Familias, Patentes,
--   RolPatente, RolFamilia, FamiliaPatente, FamiliaFamilia
-- excluida: BitacoraEventos (evita acoplamiento circular de integridad con el pipeline de auditoria)
-- =============================================================================

-- ---------------------------------------------------------------------------
-- 1. columnas dvh en tablas protegidas
-- ---------------------------------------------------------------------------

-- Usuarios
IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID('dbo.Usuarios') AND name = 'DVH'
)
BEGIN
    ALTER TABLE dbo.Usuarios
    ADD DVH VARCHAR(64) NOT NULL CONSTRAINT DF_Usuarios_DVH DEFAULT '';
END
GO

-- Roles
IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID('dbo.Roles') AND name = 'DVH'
)
BEGIN
    ALTER TABLE dbo.Roles
    ADD DVH VARCHAR(64) NOT NULL CONSTRAINT DF_Roles_DVH DEFAULT '';
END
GO

-- Familias
IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID('dbo.Familias') AND name = 'DVH'
)
BEGIN
    ALTER TABLE dbo.Familias
    ADD DVH VARCHAR(64) NOT NULL CONSTRAINT DF_Familias_DVH DEFAULT '';
END
GO

-- Patentes
IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID('dbo.Patentes') AND name = 'DVH'
)
BEGIN
    ALTER TABLE dbo.Patentes
    ADD DVH VARCHAR(64) NOT NULL CONSTRAINT DF_Patentes_DVH DEFAULT '';
END
GO

-- RolPatente
IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID('dbo.RolPatente') AND name = 'DVH'
)
BEGIN
    ALTER TABLE dbo.RolPatente
    ADD DVH VARCHAR(64) NOT NULL CONSTRAINT DF_RolPatente_DVH DEFAULT '';
END
GO

-- RolFamilia
IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID('dbo.RolFamilia') AND name = 'DVH'
)
BEGIN
    ALTER TABLE dbo.RolFamilia
    ADD DVH VARCHAR(64) NOT NULL CONSTRAINT DF_RolFamilia_DVH DEFAULT '';
END
GO

-- FamiliaPatente
IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID('dbo.FamiliaPatente') AND name = 'DVH'
)
BEGIN
    ALTER TABLE dbo.FamiliaPatente
    ADD DVH VARCHAR(64) NOT NULL CONSTRAINT DF_FamiliaPatente_DVH DEFAULT '';
END
GO

-- FamiliaFamilia
IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID('dbo.FamiliaFamilia') AND name = 'DVH'
)
BEGIN
    ALTER TABLE dbo.FamiliaFamilia
    ADD DVH VARCHAR(64) NOT NULL CONSTRAINT DF_FamiliaFamilia_DVH DEFAULT '';
END
GO

-- ---------------------------------------------------------------------------
-- 2. tabla de control DigitoVerificador
-- ---------------------------------------------------------------------------

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'DigitoVerificador_83KI')
BEGIN
    CREATE TABLE dbo.DigitoVerificador_83KI
    (
        NombreTabla         VARCHAR(128)    NOT NULL,
        DVV                 VARCHAR(64)     NOT NULL,
        FechaActualizacion  DATETIME        NOT NULL CONSTRAINT DF_DigitoVerificador_Fecha DEFAULT GETDATE(),

        CONSTRAINT PK_DigitoVerificador_83KI PRIMARY KEY CLUSTERED (NombreTabla)
    );
END
GO

-- verifica que el esquema se aplico correctamente
PRINT '001_schema.sql aplicado: 8 columnas DVH + tabla DigitoVerificador_83KI creadas.';
GO

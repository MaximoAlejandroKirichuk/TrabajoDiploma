# Genera Instalador\Database\Deploy_InstitutoVisionEnAccion.sql a partir de los scripts
# que ya existen en Scripts\Sql (no se reescribe ninguno: se reutilizan tal cual).
#
# El orden de las migraciones es el mismo que usa la aplicacion en
# DAL\BootstrapBaseDatosDAL_83KI.cs (metodo InstalarBaseDatos). Si se agrega una migracion
# alla, hay que agregarla tambien aca y volver a ejecutar este script.
#
# Uso (desde la raiz del repositorio):
#   powershell -ExecutionPolicy Bypass -File Instalador\GenerarDeploy.ps1

$ErrorActionPreference = 'Stop'

$raiz        = Split-Path -Parent $PSScriptRoot
$carpetaSql  = Join-Path $raiz 'Scripts\Sql'
$salida      = Join-Path $PSScriptRoot 'Database\Deploy_InstitutoVisionEnAccion.sql'
$nombreBase  = 'InstitutoVisionEnAccion'
$variable    = '$(BaseDatos)'

$migraciones = @(
    'financiacion-cuotas\001_modelo.sql',
    'cun03\001_migracion.sql',
    'cun05\001_migracion.sql',
    'cun06\001_migracion.sql',
    'reporte-ocupacion-comisiones\001_migracion.sql',
    'cun07\001_migracion.sql',
    'financiacion-cuotas\002_completar_inscripciones.sql',
    'cun08\001_migracion.sql',
    'cun09\001_migracion.sql',
    'cun08\006_evaluaciones.sql',
    'reporte-morosidad\001_migracion.sql',
    'pagos-cuotas\001_permiso.sql',
    'disponibilidad-profesor\001_auditoria.sql',
    'reporte-inteligente\001_modelo.sql',
    'reporte-inteligente\002_reporte.sql'
)

function Leer-Lineas([string]$ruta) {
    if (-not (Test-Path $ruta)) { throw "No se encontro el script: $ruta" }
    # ReadAllLines detecta y descarta el BOM si lo hay
    return [System.IO.File]::ReadAllLines($ruta, [System.Text.Encoding]::UTF8)
}

# --- EsquemaCompleto.sql se divide en tres partes usando sus sentencias USE ---
#   (se descarta el bloque CREATE DATABASE / ALTER DATABASE de SSMS: la base se crea con las
#    opciones por defecto de la instancia, porque varias de esas opciones exigen SQL Server 2019)
#   inicial  : tablas base + datos semilla + digitos verificadores (generado por SSMS, NO re-ejecutable)
#   resto    : secciones con guardas IF ... IS NULL (re-ejecutables)
$esquema = Leer-Lineas (Join-Path $carpetaSql 'EsquemaCompleto.sql')
$usos = @()
for ($i = 0; $i -lt $esquema.Length; $i++) {
    if ($esquema[$i] -match '^\s*USE\s+\[') { $usos += $i }
}
if ($usos.Count -ne 4) {
    throw "EsquemaCompleto.sql cambio de estructura: se esperaban 4 sentencias USE y hay $($usos.Count). Revisar GenerarDeploy.ps1."
}

$inicial  = $esquema[($usos[1] + 1)..($usos[2] - 1)]
$resto    = $esquema[($usos[3] + 1)..($esquema.Length - 1)]

$sb = New-Object System.Text.StringBuilder
function Agregar([string[]]$lineas) { foreach ($l in $lineas) { [void]$sb.AppendLine($l) } }
function Titulo([string]$texto) {
    $linea = '-- ' + ('=' * 76)
    Agregar @('GO', '', $linea, "-- $texto", $linea)
}

Agregar @(
    '/* =============================================================================',
    '   Deploy_InstitutoVisionEnAccion.sql  -  ARCHIVO GENERADO, NO EDITAR A MANO',
    '   Se genera con Instalador\GenerarDeploy.ps1 a partir de Scripts\Sql.',
    '',
    '   Script unico e idempotente: sirve para instalacion nueva y para actualizacion.',
    '   - Si la base no existe, la crea.',
    '   - Decide UNA sola vez si es instalacion nueva (no existe dbo.Usuarios).',
    '   - Tablas base, datos semilla y digitos verificadores: SOLO en instalacion nueva.',
    '   - Secciones con guardas y migraciones: siempre (no pisan datos existentes).',
    '   - Todo dentro de una transaccion con XACT_ABORT: ante un error no queda nada a medias.',
    '   - Al terminar bien imprime DEPLOY_OK.',
    '',
    '   Requisitos: SQL Server 2016 SP1 o superior (CREATE OR ALTER, SESSION_CONTEXT).',
    '   Ejecucion manual:',
    '     sqlcmd -S "<instancia>" -E -b -I -l 30 -f 65001 -v BaseDatos="InstitutoVisionEnAccion" -i Deploy_InstitutoVisionEnAccion.sql',
    '   (-b corta ante el primer error; -I activa QUOTED_IDENTIFIER; -f 65001 lee UTF-8)',
    '   ============================================================================= */',
    ':on error exit',
    'SET NOCOUNT ON;',
    'SET ANSI_NULLS ON;',
    'SET QUOTED_IDENTIFIER ON;',
    'SET ANSI_PADDING ON;',
    'SET ANSI_WARNINGS ON;',
    'SET ARITHABORT ON;',
    'SET CONCAT_NULL_YIELDS_NULL ON;',
    'SET NUMERIC_ROUNDABORT OFF;',
    'GO',
    'USE [master]',
    'GO',
    "IF DB_ID(N'$variable') IS NULL",
    'BEGIN',
    "    CREATE DATABASE [$variable];",
    "    PRINT 'Base de datos creada: $variable';",
    'END',
    'GO',
    "USE [$variable]",
    'GO',
    '-- Decision unica: es instalacion nueva si todavia no existe la tabla testigo dbo.Usuarios.',
    "DECLARE @nueva BIT = CASE WHEN OBJECT_ID(N'dbo.Usuarios', N'U') IS NULL THEN 1 ELSE 0 END;",
    "EXEC sys.sp_set_session_context @key = N'EsInstalacionNueva', @value = @nueva;",
    "IF @nueva = 1 PRINT 'Modo: INSTALACION NUEVA (esquema + datos semilla)';",
    "ELSE PRINT 'Modo: ACTUALIZACION (se conservan los datos existentes)';",
    'GO',
    'SET XACT_ABORT ON;',
    'GO',
    'BEGIN TRANSACTION;',
    'GO',
    '-- En una actualizacion, SET NOEXEC ON hace que los lotes siguientes se compilen pero NO se ejecuten.',
    "IF CAST(SESSION_CONTEXT(N'EsInstalacionNueva') AS BIT) = 0 SET NOEXEC ON;"
)

Titulo 'Tablas base, datos semilla y digitos verificadores (solo instalacion nueva) - origen: EsquemaCompleto.sql'
Agregar $inicial
Agregar @('GO', 'SET NOEXEC OFF;')

Titulo 'Esquema con guardas (siempre) - origen: EsquemaCompleto.sql'
Agregar $resto

foreach ($m in $migraciones) {
    Titulo "Migracion (siempre) - origen: Scripts\Sql\$m"
    Agregar (Leer-Lineas (Join-Path $carpetaSql $m))
}

Titulo 'Cierre'
Agregar @(
    'IF @@TRANCOUNT <> 1',
    'BEGIN',
    '    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;',
    "    THROW 50000, N'El deploy termino con una cantidad inesperada de transacciones abiertas. No se aplico ningun cambio.', 1;",
    'END',
    'COMMIT TRANSACTION;',
    'GO',
    "IF CAST(SESSION_CONTEXT(N'EsInstalacionNueva') AS BIT) = 1 PRINT 'Resultado: instalacion nueva completada.';",
    "ELSE PRINT 'Resultado: actualizacion completada.';",
    "PRINT 'DEPLOY_OK';",
    'GO'
)

$texto = $sb.ToString()
if ($texto.Contains($nombreBase + ']') -or $texto -match "USE\s+\[?$nombreBase") {
    throw 'Quedo el nombre de la base escrito a mano en el script generado.'
}

$carpetaSalida = Split-Path -Parent $salida
if (-not (Test-Path $carpetaSalida)) { New-Item -ItemType Directory -Path $carpetaSalida | Out-Null }
[System.IO.File]::WriteAllText($salida, $texto, (New-Object System.Text.UTF8Encoding($true)))
Write-Host "Generado: $salida ($([math]::Round((Get-Item $salida).Length / 1KB)) KB)"

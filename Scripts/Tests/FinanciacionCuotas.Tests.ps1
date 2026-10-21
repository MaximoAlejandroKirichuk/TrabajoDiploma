param([string]$Server = '(localdb)\MSSQLLocalDB', [string]$BuildPath = 'C:\Users\User\AppData\Local\Temp\opencode\financiacion-build')
$ErrorActionPreference = 'Stop'
$root = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
$temporary = 'C:\Users\User\AppData\Local\Temp\opencode'
if (-not (Test-Path -LiteralPath $temporary)) { throw 'Temporary directory is unavailable.' }
$name = 'FinanceTests_' + [Guid]::NewGuid().ToString('N')
$master = New-Object System.Data.SqlClient.SqlConnection("Server=$Server;Database=master;Integrated Security=True;Connect Timeout=30")
$connection = New-Object System.Data.SqlClient.SqlConnection("Server=$Server;Database=$name;Integrated Security=True;Connect Timeout=30")
function Execute([string]$sql) {
    $command = $connection.CreateCommand()
    $command.CommandText = $sql
    $command.CommandTimeout = 120
    try { [void]$command.ExecuteNonQuery() } finally { $command.Dispose() }
}
function ExecuteFile([string]$relative, [bool]$baseSchema = $false) {
    $text = Get-Content -Raw -Encoding UTF8 -LiteralPath (Join-Path $root $relative)
    foreach ($batch in [regex]::Split($text, '(?im)^\s*GO\s*\r?$')) {
        if ([string]::IsNullOrWhiteSpace($batch)) { continue }
        # Bootstrap the existing schema only inside the disposable database. Never run its database/USE batches.
        if ($baseSchema -and ($batch -match '(?im)^\s*USE\s|InstitutoVisionEnAccion')) { continue }
        if ($batch -match '(?im)^\s*(USE\s|CREATE\s+DATABASE|ALTER\s+DATABASE)') { throw "Unexpected database switch in $relative" }
        try { Execute $batch } catch { throw "${relative}: $($_.Exception.Message)" }
    }
}
function ApplyInstaller {
    $start = New-Object System.Diagnostics.ProcessStartInfo
    $start.FileName = @(Get-Command sqlcmd -CommandType Application)[0].Source
    $start.WorkingDirectory = $root
    $start.Arguments = '-S "' + $Server + '" -d "' + $name + '" -E -b -i "Scripts/Sql/financiacion-cuotas/aplicar.sql"'
    $start.UseShellExecute = $false
    $start.RedirectStandardOutput = $true
    $start.RedirectStandardError = $true
    $process = New-Object System.Diagnostics.Process
    $process.StartInfo = $start
    try {
        [void]$process.Start()
        $output = $process.StandardOutput.ReadToEndAsync()
        $errors = $process.StandardError.ReadToEndAsync()
        if (-not $process.WaitForExit(120000)) { $process.Kill(); throw 'Installer timed out.' }
        if ($process.ExitCode -ne 0) { throw ($output.Result + $errors.Result) }
    } finally { $process.Dispose() }
    'PASS: the actual SQLCMD installer runs using portable repository paths.'
}
function VerifyIntegrity {
    foreach ($assembly in @('BE.dll','Service.dll','DAL.dll')) { Add-Type -Path (Join-Path $BuildPath $assembly) }
    $integrity = New-Object DAL.IntegridadDAL_83KI (New-Object Service.Encriptador_83KI)
    foreach ($table in @('Cuota','SolicitudInscripcion')) {
        $command = $connection.CreateCommand()
        $command.CommandText = "SELECT * FROM dbo.$table"
        $adapter = New-Object System.Data.SqlClient.SqlDataAdapter($command)
        $data = New-Object System.Data.DataTable
        [void]$adapter.Fill($data)
        $hashes = New-Object 'System.Collections.Generic.List[string]'
        foreach ($row in $data.Rows) {
            $values = New-Object 'System.Collections.Generic.Dictionary[string,object]'
            foreach ($column in $data.Columns) {
                $value = $row[$column.ColumnName]
                if ($value -is [DBNull]) { $value = $null }
                $values.Add($column.ColumnName,$value)
            }
            $hash = $integrity.CalcularDVH($table,$values)
            if ($hash -ne $row.DVH) { throw "SQL/.NET integrity mismatch in $table" }
            $hashes.Add($hash)
        }
        $hashes.Sort([StringComparer]::Ordinal)
        $expected = (New-Object Service.Encriptador_83KI).CalcularHashIntegridad([string]::Concat($hashes.ToArray()))
        $command.CommandText = "SELECT DVV FROM dbo.DigitoVerificador_83KI WHERE NombreTabla=N'$table'"
        if ($command.ExecuteScalar() -ne $expected) { throw "Incorrect DVV for $table" }
    }
    'PASS: SQL backfill hashes match the application DVH/DVV implementation.'
}
function VerifyLookupAndBusinessRules {
    $command = $connection.CreateCommand()
    $command.CommandText = "EXEC dbo.sp_CUN07_BuscarCuotasPendientes N'99776611',N'FIN-LEG-COM'"
    $adapter = New-Object System.Data.SqlClient.SqlDataAdapter($command)
    $data = New-Object System.Data.DataTable
    [void]$adapter.Fill($data)
    if ($data.Rows.Count -ne 6) { throw 'CUN07 did not return all six course installments.' }
    $flags = [System.Reflection.BindingFlags]'NonPublic,Static'
    $mapper = [DAL.PagoCuotaDAL_83KI].GetMethod('MapearBusqueda',$flags)
    $result = $mapper.Invoke($null,[object[]]@($data.Rows[0]))
    if (-not $result.MatriculaPagada -or $result.Solicitud.TotalCursoSnapshot -ne 132000) { throw 'Incorrect financial lookup mapping.' }
    $mapper = [DAL.PagoCuotaDAL_83KI].GetMethod('MapearCuota',$flags)
    foreach ($row in $data.Rows) {
        $cuota = $mapper.Invoke($null,[object[]]@($row))
        if ($cuota.EsMatricula -or $null -eq $cuota.Periodo) { throw 'Incorrect installment concept/period mapping.' }
        $result.Cuotas.Add($cuota)
        $result.CuotasPendientes.Add($cuota)
    }
    if ($result.Cuotas[0].NumeroMensualidad -ne 1) { throw 'The course installment number is not independent of matrícula.' }
    $references = @('BE.dll','Service.dll','BLL.dll') | ForEach-Object { Join-Path $BuildPath $_ }
    Add-Type -Path (Join-Path $BuildPath 'BLL.dll')
    Add-Type -Path (Join-Path $PSScriptRoot 'FinanciacionCuotas.BllTests.cs') -ReferencedAssemblies ($references + @('System.Core.dll'))
    [FinanciacionCuotasBllTests]::Run($result)
}
function VerifyEnrollmentLookup {
    $command = $connection.CreateCommand()
    $command.CommandText = "EXEC dbo.sp_CUN05_BuscarCuotasPendientesPorDni N'99885511'"
    $adapter = New-Object System.Data.SqlClient.SqlDataAdapter($command)
    $data = New-Object System.Data.DataTable
    [void]$adapter.Fill($data)
    $mapper = [DAL.PagoInscripcionDAL_83KI].GetMethod('MapearMatriculas',[System.Reflection.BindingFlags]'NonPublic,Static')
    $arguments = New-Object 'object[]' 1
    $arguments[0] = $data.PSObject.BaseObject
    $result = $mapper.Invoke($null,$arguments)
    if ($result.MatriculasPendientes.Count -ne 2) { throw 'The DAL lost an enrollment fee.' }
    foreach ($fee in $result.MatriculasPendientes) {
        if ($fee.Cuota.IdSolicitudInscripcion -ne $fee.Solicitud.IdSolicitudInscripcion -or [string]::IsNullOrEmpty($fee.CodigoComision)) {
            throw 'An enrollment fee lost its own enrollment/commission context.'
        }
    }
    [FinanciacionCuotasBllTests]::RunMatriculas($result)
    Execute @'
DECLARE @alumno INT=(SELECT IdAlumno FROM dbo.Alumno WHERE DNI=N'99885511'),@solicitud INT,@cuota INT,@today DATETIME=CONVERT(date,GETDATE());
SELECT @solicitud=si.IdSolicitudInscripcion FROM dbo.SolicitudInscripcion si
JOIN dbo.Comision c ON c.IdComision=si.IdComision WHERE si.IdAlumno=@alumno AND c.Codigo=N'REVIEW-COM-B';
SELECT @cuota=IdCuota FROM dbo.Cuota WHERE IdSolicitudInscripcion=@solicitud AND TipoConcepto=N'matricula';
EXEC dbo.sp_CUN05_RegistrarPagoInscripcion @alumno,@solicitud,@cuota,N'transferencia',20000,N'REVIEW-SECOND-FEE',@today;
IF NOT EXISTS(SELECT 1 FROM dbo.PagoInscripcion WHERE NumeroReferencia=N'REVIEW-SECOND-FEE' AND IdSolicitudInscripcion=@solicitud AND IdCuota=@cuota)
    THROW 52927,N'The second enrollment fee was not paid.',1;
IF NOT EXISTS(SELECT 1 FROM dbo.SolicitudInscripcion si JOIN dbo.Comision c ON c.IdComision=si.IdComision
    WHERE si.IdAlumno=@alumno AND c.Codigo=N'REVIEW-COM-A' AND si.Estado=N'pendiente')
    THROW 52928,N'Paying the second enrollment activated the first.',1;
'@
    'PASS: the second enrollment fee can be paid in SQL without activating the first enrollment.'
}
try {
    $master.Open()
    $command = $master.CreateCommand()
    $command.CommandText = "CREATE DATABASE [$name] ON (NAME=N'$name',FILENAME=N'$temporary\$name.mdf') LOG ON (NAME=N'${name}_log',FILENAME=N'$temporary\${name}_log.ldf')"
    [void]$command.ExecuteNonQuery()
    $connection.Open()
    ExecuteFile 'Scripts/Sql/EsquemaCompleto.sql' $true
    ApplyInstaller
    ExecuteFile 'Scripts/Sql/financiacion-cuotas/000_fixture_legacy.sql'
    ExecuteFile 'Scripts/Sql/financiacion-cuotas/002_completar_inscripciones.sql'
    Execute @'
IF NOT EXISTS(SELECT 1 FROM dbo.SolicitudInscripcion WHERE Codigo=N'FIN-LEG-SOL' AND Estado=N'activa' AND CantidadCuotasSnapshot=6 AND TotalCursoSnapshot=132000)
    THROW 52918,N'Legacy financing or academic state migration failed.',1;
IF (SELECT COUNT(*) FROM dbo.Cuota cu JOIN dbo.SolicitudInscripcion si ON si.IdSolicitudInscripcion=cu.IdSolicitudInscripcion WHERE si.Codigo=N'FIN-LEG-SOL')<>7
    THROW 52919,N'Legacy enrollment did not receive six installments.',1;
IF NOT EXISTS(SELECT 1 FROM dbo.PagoInscripcion WHERE NumeroReferencia=N'LEGACY-PAYMENT' AND MontoPagado=20000 AND NumeroReciboInstitucional=N'LEGACY-RECEIPT' AND DVH=N'legacy-payment-hash')
    THROW 52920,N'An existing payment was rewritten.',1;
'@
    VerifyIntegrity
    VerifyLookupAndBusinessRules
    ExecuteFile 'Scripts/Sql/financiacion-cuotas/003_pruebas.sql'
    ExecuteFile 'Scripts/Sql/financiacion-cuotas/004_pruebas_matriculas.sql'
    VerifyEnrollmentLookup
    ExecuteFile 'Scripts/Sql/financiacion-cuotas/001_modelo.sql'
    ExecuteFile 'Scripts/Sql/financiacion-cuotas/002_completar_inscripciones.sql'
    foreach ($language in @('es-AR','en-US')) {
        $json = Get-Content -Raw -Encoding UTF8 -LiteralPath (Join-Path $root "Service/Idiomas/$language.json") | ConvertFrom-Json
        if (-not $json) { throw "Invalid language file: $language" }
    }
    'PASS: financing SQL regression suite and idempotent schema/backfill.'
} finally {
    $connection.Dispose()
    [System.Data.SqlClient.SqlConnection]::ClearAllPools()
    if ($master.State -eq 'Open') {
        $command = $master.CreateCommand()
        $command.CommandText = "IF DB_ID(N'$name') IS NOT NULL BEGIN ALTER DATABASE [$name] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [$name]; END"
        [void]$command.ExecuteNonQuery()
    }
    $master.Dispose()
}

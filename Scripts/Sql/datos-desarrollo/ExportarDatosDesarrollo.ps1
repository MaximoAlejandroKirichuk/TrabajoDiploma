# Exporta datos de la base de desarrollo como INSERT idempotentes (solo inserta lo que falta).
param([string]$Instancia, [string]$Base, [string]$Salida)
Add-Type -AssemblyName System.Data
$cs = "Data Source=$Instancia;Initial Catalog=$Base;Integrated Security=True;"
$cn = New-Object System.Data.SqlClient.SqlConnection $cs; $cn.Open()
function Consulta($sql) { $c=$cn.CreateCommand(); $c.CommandText=$sql; $d=New-Object System.Data.DataTable; $d.Load($c.ExecuteReader()); return ,$d }
function Lit($v, $tipo) {
  if ($v -is [System.DBNull]) { return 'NULL' }
  switch -Regex ($tipo) {
    '^(int|bigint|smallint|tinyint)$' { return [string]$v }
    '^bit$' { if ($v) {return '1'} else {return '0'} }
    '^(decimal|numeric|money|float|real)$' { return ([System.Convert]::ToString($v,[Globalization.CultureInfo]::InvariantCulture)) }
    '^(nvarchar|nchar|ntext)$' { return "N'" + ([string]$v).Replace("'","''") + "'" }
    '^(varchar|char|text)$' { return "'" + ([string]$v).Replace("'","''") + "'" }
    '^time$' { return "CAST(N'" + ([TimeSpan]$v).ToString('hh\:mm\:ss\.fffffff') + "' AS Time)" }
    '^date$' { return "CAST(N'" + ([DateTime]$v).ToString('yyyy-MM-dd') + "' AS Date)" }
    '^(datetime|smalldatetime)$' { return "CAST(N'" + ([DateTime]$v).ToString('yyyy-MM-ddTHH:mm:ss.fff') + "' AS DateTime)" }
    '^datetime2$' { return "CAST(N'" + ([DateTime]$v).ToString('yyyy-MM-ddTHH:mm:ss.fffffff') + "' AS DateTime2)" }
    '^uniqueidentifier$' { return "N'" + $v + "'" }
    '^(varbinary|binary)$' { return '0x' + (($v | ForEach-Object { $_.ToString('X2') }) -join '') }
    default { throw "Tipo no soportado: $tipo" }
  }
}
$tablas = @(
  @{T='Patentes'}, @{T='Roles'}, @{T='Familias'}, @{T='FamiliaPatente'}, @{T='FamiliaFamilia'}, @{T='RolFamilia'}, @{T='RolPatente'},
  @{T='Usuarios'; W="Username IN (N'28188299Fernanda', N'28188200Daniel', N'28188300Daiana')"},
  @{T='Profesor'}, @{T='Curso'}, @{T='CursoProfesor'}, @{T='RecorridoCurso'}, @{T='PlanesDePago'},
  @{T='DisponibilidadProfesor'}
)
$o = New-Object System.Text.StringBuilder
[void]$o.AppendLine("-- Datos de la base de desarrollo: seguridad (roles/familias), usuarios de demostracion y maestros.")
[void]$o.AppendLine("-- Idempotente: solo inserta las filas que faltan (por clave primaria); no modifica las existentes.")
[void]$o.AppendLine("-- Generado con Scripts\Sql\datos-desarrollo\ExportarDatosDesarrollo.ps1")
[void]$o.AppendLine("SET NOCOUNT ON;"); [void]$o.AppendLine("GO")
foreach ($t in $tablas) {
  $n = $t.T
  $cols = Consulta "SELECT c.name, ty.name AS tipo, c.is_identity FROM sys.columns c JOIN sys.types ty ON ty.user_type_id=c.user_type_id WHERE c.object_id=OBJECT_ID(N'dbo.$n') AND c.is_computed=0 ORDER BY c.column_id"
  $pk = @((Consulta "SELECT col.name FROM sys.indexes i JOIN sys.index_columns ic ON ic.object_id=i.object_id AND ic.index_id=i.index_id JOIN sys.columns col ON col.object_id=ic.object_id AND col.column_id=ic.column_id WHERE i.object_id=OBJECT_ID(N'dbo.$n') AND i.is_primary_key=1 ORDER BY ic.key_ordinal").Rows | ForEach-Object { $_.name })
  $ident = @($cols.Rows | Where-Object { $_.is_identity }).Count -gt 0
  $sql = "SELECT * FROM dbo.[$n]"; if ($t.W) { $sql += " WHERE " + $t.W }
  $datos = Consulta $sql
  [void]$o.AppendLine("-- $n ($($datos.Rows.Count) filas)")
  if ($datos.Rows.Count -eq 0) { continue }
  if ($ident) { [void]$o.AppendLine("SET IDENTITY_INSERT dbo.[$n] ON;") }
  $lista = ($cols.Rows | ForEach-Object { '[' + $_.name + ']' }) -join ', '
  foreach ($r in $datos.Rows) {
    $cond = ($pk | ForEach-Object { $tipo = ($cols.Rows | Where-Object { $_.name -eq $pk[0] -or $_.name -eq $_ } | Select-Object -First 1); $p=$_; $ty=($cols.Rows | Where-Object { $_.name -eq $p }).tipo; "[$p] = " + (Lit $r[$p] $ty) }) -join ' AND '
    $vals = ($cols.Rows | ForEach-Object { Lit $r[$_.name] $_.tipo }) -join ', '
    [void]$o.AppendLine("IF NOT EXISTS (SELECT 1 FROM dbo.[$n] WHERE $cond) INSERT INTO dbo.[$n] ($lista) VALUES ($vals);")
  }
  if ($ident) { [void]$o.AppendLine("SET IDENTITY_INSERT dbo.[$n] OFF;") }
  [void]$o.AppendLine("GO")
}
[void]$o.AppendLine("-- Digitos verificadores verticales de las tablas cargadas")
foreach ($n in 'Patentes','Roles','Familias','FamiliaPatente','FamiliaFamilia','RolFamilia','RolPatente','Usuarios','Profesor','Curso','CursoProfesor','RecorridoCurso','DisponibilidadProfesor') {
  [void]$o.AppendLine("UPDATE dbo.DigitoVerificador_83KI SET DVV = LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', ISNULL(CAST((SELECT CAST(N'' AS NVARCHAR(MAX)) + DVH FROM dbo.[$n] ORDER BY DVH FOR XML PATH(N'')) AS NVARCHAR(MAX)), N'')), 2)), FechaActualizacion = GETDATE() WHERE NombreTabla = N'$n';")
}
[void]$o.AppendLine("GO")
[System.IO.File]::WriteAllText($Salida, $o.ToString(), (New-Object System.Text.UTF8Encoding($true)))
$cn.Close()
"Generado: $Salida"

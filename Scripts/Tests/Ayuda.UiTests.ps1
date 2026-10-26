param([string]$Configuration = 'Debug')
$ErrorActionPreference = 'Stop'
$root = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
$vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'
$msbuild = & $vswhere -latest -products * -requires Microsoft.Component.MSBuild -find 'MSBuild\**\Bin\MSBuild.exe'
if (-not $msbuild) { throw 'Visual Studio MSBuild was not found.' }
& $msbuild (Join-Path $root 'Solucion.sln') /t:Build "/p:Configuration=$Configuration" /verbosity:minimal /nologo
if ($LASTEXITCODE -ne 0) { throw 'Solution build failed.' }
$output = Join-Path $root "UI\bin\$Configuration"
if (-not (Test-Path -LiteralPath $output)) { throw 'Build output directory is missing.' }
$compiler = Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319\csc.exe'
$executable = Join-Path $output 'Ayuda.UiTests.exe'
$references = @('UI.exe', 'Service.dll', 'BE.dll') | ForEach-Object { '/reference:' + (Join-Path $output $_) }
& $compiler /nologo /codepage:65001 /target:exe "/out:$executable" $references /reference:System.Windows.Forms.dll /reference:System.Drawing.dll (Join-Path $PSScriptRoot 'Ayuda.UiTests.cs')
if ($LASTEXITCODE -ne 0) { throw 'Help test compilation failed.' }
& $executable
if ($LASTEXITCODE -ne 0) { throw 'Help UI tests failed.' }

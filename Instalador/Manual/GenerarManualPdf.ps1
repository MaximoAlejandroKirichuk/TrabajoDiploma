# Genera Manual_Instalador.pdf a partir de Manual_Instalador.html usando Microsoft Edge (o Chrome).
# Uso (desde la raiz del repositorio):
#   powershell -ExecutionPolicy Bypass -File Instalador\Manual\GenerarManualPdf.ps1

$ErrorActionPreference = 'Stop'

$html = Join-Path $PSScriptRoot 'Manual_Instalador.html'
$pdf  = Join-Path $PSScriptRoot 'Manual_Instalador.pdf'

$navegadores = @(
    "${env:ProgramFiles(x86)}\Microsoft\Edge\Application\msedge.exe",
    "$env:ProgramFiles\Microsoft\Edge\Application\msedge.exe",
    "$env:ProgramFiles\Google\Chrome\Application\chrome.exe"
)
$navegador = $navegadores | Where-Object { Test-Path $_ } | Select-Object -First 1
if (-not $navegador) { throw 'No se encontro Microsoft Edge ni Google Chrome para generar el PDF.' }

if (Test-Path $pdf) { Remove-Item $pdf -Confirm:$false }

$perfil = Join-Path $env:TEMP 'manual_instalador_perfil'
$url = (New-Object System.Uri($html)).AbsoluteUri
$argumentos = @('--headless', '--disable-gpu', '--no-pdf-header-footer', "--user-data-dir=`"$perfil`"",
                "--print-to-pdf=`"$pdf`"", "`"$url`"")
Start-Process -FilePath $navegador -ArgumentList $argumentos -Wait

if (-not (Test-Path $pdf)) { throw 'El navegador no genero el PDF.' }
Write-Host "Generado: $pdf ($([math]::Round((Get-Item $pdf).Length / 1KB)) KB)"

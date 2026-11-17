# Deja la instancia y la base elegidas como valores por defecto de la aplicacion.
#
# La aplicacion guarda la conexion en Properties.Settings (por usuario de Windows):
#   DatabaseServerInstance y DatabaseName. Si un usuario todavia no guardo nada, .NET toma
#   los valores de la seccion <userSettings> de UI.exe.config. Este script escribe esa seccion,
#   sin tocar el resto del archivo y respetando UTF-8.
#
# Uso manual (como administrador):
#   powershell -ExecutionPolicy Bypass -File ConfigurarConexion.ps1 -RutaConfig "C:\...\UI.exe.config" -Instancia "." -BaseDatos "InstitutoVisionEnAccion"
param(
    [Parameter(Mandatory = $true)][string]$RutaConfig,
    [Parameter(Mandatory = $true)][string]$Instancia,
    [Parameter(Mandatory = $true)][string]$BaseDatos
)

$ErrorActionPreference = 'Stop'

try {
    $tipoGrupo   = 'System.Configuration.UserSettingsGroup, System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'
    $tipoSeccion = 'System.Configuration.ClientSettingsSection, System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'
    $nombreSeccion = 'UI.Properties.Settings'

    $xml = New-Object System.Xml.XmlDocument
    $xml.PreserveWhitespace = $true
    $xml.Load($RutaConfig)

    $raiz = $xml.DocumentElement
    if ($raiz.Name -ne 'configuration') { throw "El archivo no es un .config valido: $RutaConfig" }

    # <configSections> tiene que ser el PRIMER hijo de <configuration>
    $secciones = $raiz.SelectSingleNode('configSections')
    if ($secciones -eq $null) {
        $secciones = $xml.CreateElement('configSections')
        [void]$raiz.InsertBefore($secciones, $raiz.FirstChild)
    }

    $grupo = $secciones.SelectSingleNode("sectionGroup[@name='userSettings']")
    if ($grupo -eq $null) {
        $grupo = $xml.CreateElement('sectionGroup')
        $grupo.SetAttribute('name', 'userSettings')
        $grupo.SetAttribute('type', $tipoGrupo)
        [void]$secciones.AppendChild($grupo)
    }

    if ($grupo.SelectSingleNode("section[@name='$nombreSeccion']") -eq $null) {
        $seccion = $xml.CreateElement('section')
        $seccion.SetAttribute('name', $nombreSeccion)
        $seccion.SetAttribute('type', $tipoSeccion)
        $seccion.SetAttribute('allowExeDefinition', 'MachineToLocalUser')
        $seccion.SetAttribute('requirePermission', 'false')
        [void]$grupo.AppendChild($seccion)
    }

    $userSettings = $raiz.SelectSingleNode('userSettings')
    if ($userSettings -eq $null) {
        $userSettings = $xml.CreateElement('userSettings')
        [void]$raiz.AppendChild($userSettings)
    }

    $valores = $userSettings.SelectSingleNode($nombreSeccion)
    if ($valores -eq $null) {
        $valores = $xml.CreateElement($nombreSeccion)
        [void]$userSettings.AppendChild($valores)
    }

    # Se escribe con saltos de linea entre los <setting>: .NET no lee bien esta seccion
    # cuando los elementos quedan pegados en una sola linea.
    $n = [Environment]::NewLine
    $valores.InnerXml =
        $n + '            <setting name="DatabaseServerInstance" serializeAs="String">' +
        $n + '                <value>' + [System.Security.SecurityElement]::Escape($Instancia) + '</value>' +
        $n + '            </setting>' +
        $n + '            <setting name="DatabaseName" serializeAs="String">' +
        $n + '                <value>' + [System.Security.SecurityElement]::Escape($BaseDatos) + '</value>' +
        $n + '            </setting>' +
        $n + '        '

    $opciones = New-Object System.Xml.XmlWriterSettings
    $opciones.Encoding = New-Object System.Text.UTF8Encoding($false)
    $escritor = [System.Xml.XmlWriter]::Create($RutaConfig, $opciones)
    try { $xml.Save($escritor) } finally { $escritor.Close() }

    Write-Output "CONFIG_OK instancia=$Instancia base=$BaseDatos"
    exit 0
}
catch {
    Write-Output "CONFIG_ERROR: $($_.Exception.Message)"
    exit 1
}

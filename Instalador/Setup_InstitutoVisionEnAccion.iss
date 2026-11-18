; =============================================================================
;  Setup_InstitutoVisionEnAccion.iss  -  Instalador unico e idempotente (Inno Setup 6)
;
;  MODOS (el mismo setup sirve para los tres casos)
;   - Instalacion limpia: copia la aplicacion, pide la instancia de SQL Server, crea la
;     base con el esquema y los datos semilla.
;   - Reinstalacion / actualizacion: si ya se instalo con este setup (clave de Registro
;     HKLM\Software\InstitutoVisionEnAccion), reutiliza la carpeta y la instancia anteriores.
;   - Actualizacion desde el instalador viejo (Solucion_83KI.msi): ofrece desinstalarlo y
;     pide la instancia donde esta la base.
;   En la reinstalacion se puede elegir entre ACTUALIZAR la base conservando los datos
;   (opcion por defecto) o RESTAURARLA desde una copia de seguridad (.bak). En los dos casos
;   primero se hace un backup de la base actual.
;   En los tres casos el script SQL decide solo si la base es nueva o existente:
;   si existe, primero se hace un backup y despues se actualiza el esquema CONSERVANDO los datos.
;
;  RUTAS
;   - Aplicacion:        {app}  (por defecto C:\Program Files\InstitutoVisionEnAccion)
;   - Scripts de base:   {app}\Database  (Deploy, Backup_Previo, ConfigurarConexion)
;   - Backups y logs:    C:\ProgramData\InstitutoVisionEnAccion\Backups y \Logs
;     (el .bak lo escribe el SERVICIO de SQL Server: dentro de Program Files falla con
;      "Operating system error 5". Estas carpetas NO se borran al desinstalar.)
;
;  REQUISITOS EN LA PC DESTINO
;   - Windows de 64 bits, .NET Framework 4.7.2 o superior.
;   - SQL Server 2016 SP1 o superior (o LocalDB) con autenticacion integrada de Windows.
;   - sqlcmd en el PATH. Sin sqlcmd la aplicacion se instala igual, pero la base hay que
;     crearla/actualizarla a mano (o desde la pantalla de configuracion de la aplicacion).
;
;  COMO COMPILAR
;   1) powershell -ExecutionPolicy Bypass -File Instalador\GenerarDeploy.ps1
;   2) dotnet build "Solucion.sln" -c Release
;   3) "%LOCALAPPDATA%\Programs\Inno Setup 6\ISCC.exe" /Q Instalador\Setup_InstitutoVisionEnAccion.iss
;      (si Inno Setup se instalo para todos los usuarios: C:\Program Files (x86)\Inno Setup 6\ISCC.exe)
;   El resultado queda en Instalador\Salida\Setup_InstitutoVisionEnAccion_v2.exe
;
;  INSTALACION DESATENDIDA (opcional, para pruebas o despliegues)
;   Setup_InstitutoVisionEnAccion_v2.exe /VERYSILENT /SUPPRESSMSGBOXES /Instancia="(localdb)\MSSQLLocalDB" /Base=MiBase
;   /Instancia y /Base reemplazan a lo que se elige en la pagina "Base de datos".
;   /Restaurar="C:\ruta\copia.bak" (solo reinstalacion) restaura la base desde ese backup.
;   En modo silencioso no se ofrece desinstalar el instalador viejo.
; =============================================================================

#define NombreVisible  "Instituto Visión en Acción"
#define NombreInterno  "InstitutoVisionEnAccion"
#define VersionApp     "2.0.0"
#define ExeApp         "UI.exe"
#define CarpetaRelease "..\UI\bin\Release"
#define BaseDefecto    "InstitutoVisionEnAccion"

[Setup]
AppId={{0AC127FC-8EE7-43AE-8EC5-A62A660D0CCA}
AppName={#NombreVisible}
AppVersion={#VersionApp}
AppPublisher={#NombreVisible}
DefaultDirName={autopf}\{#NombreInterno}
DefaultGroupName={#NombreVisible}
OutputDir=Salida
OutputBaseFilename=Setup_{#NombreInterno}_v2
PrivilegesRequired=admin
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
UsePreviousAppDir=yes
DisableDirPage=auto
DisableProgramGroupPage=yes
CloseApplications=yes
WizardStyle=modern
Compression=lzma2
SolidCompression=yes
UninstallDisplayIcon={app}\{#ExeApp}
UninstallDisplayName={#NombreVisible}

[Languages]
Name: "es"; MessagesFile: "compiler:Languages\Spanish.isl"

[Tasks]
Name: "escritorio"; Description: "Crear un acceso directo en el escritorio"; GroupDescription: "Accesos directos:"

[Dirs]
; El servicio de SQL Server necesita poder escribir el .bak en Backups.
Name: "{commonappdata}\{#NombreInterno}\Backups"; Permissions: service-modify; Flags: uninsneveruninstall
Name: "{commonappdata}\{#NombreInterno}\Logs"; Flags: uninsneveruninstall

[Files]
; Aplicacion (incluye Idiomas\ y Scripts\Sql\ que usa la pantalla de configuracion de la app)
Source: "{#CarpetaRelease}\*"; DestDir: "{app}"; Excludes: "*.pdb,*.xml,*.vshost.*,*UiTests*"; Flags: ignoreversion recursesubdirs createallsubdirs
; Scripts de base de datos: quedan siempre en {app}\Database para poder ejecutarlos a mano
Source: "Database\Deploy_{#NombreInterno}.sql"; DestDir: "{app}\Database"; Flags: ignoreversion
Source: "Database\Backup_Previo.sql"; DestDir: "{app}\Database"; Flags: ignoreversion
Source: "Database\Restaurar_Base.sql"; DestDir: "{app}\Database"; Flags: ignoreversion
Source: "Database\ConfigurarConexion.ps1"; DestDir: "{app}\Database"; Flags: ignoreversion

[Icons]
Name: "{group}\{#NombreVisible}"; Filename: "{app}\{#ExeApp}"
Name: "{group}\Desinstalar {#NombreVisible}"; Filename: "{uninstallexe}"
Name: "{autodesktop}\{#NombreVisible}"; Filename: "{app}\{#ExeApp}"; Tasks: escritorio

[Registry]
Root: HKLM; Subkey: "Software\{#NombreInterno}"; ValueType: string; ValueName: "InstallPath"; ValueData: "{app}"; Flags: uninsdeletekey
Root: HKLM; Subkey: "Software\{#NombreInterno}"; ValueType: string; ValueName: "Version"; ValueData: "{#VersionApp}"
Root: HKLM; Subkey: "Software\{#NombreInterno}"; ValueType: string; ValueName: "SqlInstance"; ValueData: "{code:ObtenerInstancia}"
Root: HKLM; Subkey: "Software\{#NombreInterno}"; ValueType: string; ValueName: "SqlDatabase"; ValueData: "{code:ObtenerBase}"
Root: HKLM; Subkey: "Software\{#NombreInterno}"; ValueType: string; ValueName: "InstallId"; ValueData: "{code:ObtenerIdInstalacion}"

[Run]
; 1) Backup previo. Si la base no existe imprime BACKUP_SIN_BASE y no hace nada.
Filename: "{cmd}"; \
  Parameters: "/C ""sqlcmd -S ""{code:ObtenerInstancia}"" -E -b -l 30 -v BaseDatos=""{code:ObtenerBase}"" CarpetaBackup=""{code:ObtenerCarpetaBackups}"" ArchivoBackup=""{code:ObtenerArchivoBackup}"" -i ""{app}\Database\Backup_Previo.sql"" > ""{code:ObtenerLogBackup}"" 2>&1"""; \
  StatusMsg: "Haciendo una copia de seguridad de la base de datos..."; \
  Flags: runhidden waituntilterminated; Check: HaySqlCmd; AfterInstall: VerificarBackup

; 2) Restore (solo si se eligio en la reinstalacion). Corre despues del backup de la base actual.
Filename: "{cmd}"; \
  Parameters: "/C ""sqlcmd -S ""{code:ObtenerInstancia}"" -E -b -l 30 -v BaseDatos=""{code:ObtenerBase}"" ArchivoBak=""{code:ObtenerArchivoRestore}"" -i ""{app}\Database\Restaurar_Base.sql"" > ""{code:ObtenerLogRestore}"" 2>&1"""; \
  StatusMsg: "Restaurando la base de datos desde la copia de seguridad..."; \
  Flags: runhidden waituntilterminated; Check: DebeRestaurar; AfterInstall: VerificarRestore

; 3) Deploy: crea la base o actualiza el esquema. Solo corre si el backup termino bien
;    (y, si se pidio restaurar, si la restauracion termino bien).
;    -b corta ante el primer error, -I activa QUOTED_IDENTIFIER, -f 65001 lee el script en UTF-8,
;    -l 30 da tiempo a que arranque LocalDB.
Filename: "{cmd}"; \
  Parameters: "/C ""sqlcmd -S ""{code:ObtenerInstancia}"" -E -b -I -l 30 -f 65001 -v BaseDatos=""{code:ObtenerBase}"" -i ""{app}\Database\Deploy_{#NombreInterno}.sql"" > ""{code:ObtenerLogDeploy}"" 2>&1"""; \
  StatusMsg: "Creando o actualizando la base de datos..."; \
  Flags: runhidden waituntilterminated; Check: DebeEjecutarDeploy; AfterInstall: VerificarDeploy

; 4) Abrir la aplicacion al terminar
Filename: "{app}\{#ExeApp}"; Description: "Abrir {#NombreVisible}"; Flags: postinstall nowait skipifsilent; Check: PuedeAbrirAplicacion

[UninstallDelete]
; Se borra toda la carpeta de la aplicacion. NUNCA se toca la base ni ProgramData (backups y logs).
Type: filesandordirs; Name: "{app}"

[Code]
const
  ClaveRegistro = 'Software\{#NombreInterno}';
  ClaveInstancias = 'SOFTWARE\Microsoft\Microsoft SQL Server\Instance Names\SQL';
  ClaveDesinstalar = 'Software\Microsoft\Windows\CurrentVersion\Uninstall';
  NombreInstaladorViejo = 'Solucion_83KI';
  NetRelease472 = 461808;
  VersionSqlMinima = 13;  // SQL Server 2016
  TextoMinimo = 'Se requiere SQL Server 2016 SP1 o superior.';

var
  SqlCmdEncontrado: Boolean;
  EsReinstalacion: Boolean;
  EsActualizacionV1: Boolean;
  InstanciaAnterior: String;
  UsarInstanciaGuardada: Boolean;   // False si la instancia guardada no responde: se pide de nuevo
  BaseAnterior: String;
  CarpetaAnterior: String;
  BackupOk: Boolean;
  DeployOk: Boolean;
  IntegridadOk: Boolean;
  MarcaTiempo: String;
  ParamInstancia: String;
  ParamBase: String;
  ParamRestaurar: String;
  RestoreOk: Boolean;
  PaginaRestore: TWizardPage;
  RadioActualizar: TNewRadioButton;
  RadioRestaurar: TNewRadioButton;
  ComboBackup: TNewComboBox;
  BotonExaminar: TNewButton;
  PaginaBD: TWizardPage;
  PaginaInfo: TOutputMsgWizardPage;
  ComboInstancia: TNewComboBox;
  EditBase: TNewEdit;
  BotonProbar: TNewButton;
  EtiquetaResultado: TNewStaticText;

{ ---------- valores que usan [Run] y [Registry] ---------- }

function ObtenerInstancia(Param: String): String;
begin
  if UsarInstanciaGuardada then
    Result := InstanciaAnterior
  else
    Result := Trim(ComboInstancia.Text);
end;

function ObtenerBase(Param: String): String;
begin
  if UsarInstanciaGuardada then
    Result := BaseAnterior
  else
    Result := Trim(EditBase.Text);
end;

{ Cambia en cada instalacion: la aplicacion lo usa para tomar la conexion recien instalada. }
function ObtenerIdInstalacion(Param: String): String;
begin
  Result := MarcaTiempo;
end;

function ObtenerCarpetaBackups(Param: String): String;
begin
  Result := ExpandConstant('{commonappdata}\{#NombreInterno}\Backups');
end;

function ObtenerArchivoBackup(Param: String): String;
begin
  Result := ObtenerBase('') + '_' + MarcaTiempo + '.bak';
end;

function ObtenerLogBackup(Param: String): String;
begin
  Result := ExpandConstant('{commonappdata}\{#NombreInterno}\Logs\Backup.log');
end;

function ObtenerLogDeploy(Param: String): String;
begin
  Result := ExpandConstant('{commonappdata}\{#NombreInterno}\Logs\Deploy.log');
end;

function ObtenerLogRestore(Param: String): String;
begin
  Result := ExpandConstant('{commonappdata}\{#NombreInterno}\Logs\Restore.log');
end;

function QuiereRestaurar: Boolean;
begin
  Result := EsReinstalacion and SqlCmdEncontrado and RadioRestaurar.Checked;
end;

{ Si se escribio solo el nombre y el archivo esta en la carpeta de Backups, se completa la ruta.
  Un nombre sin carpeta que no esta ahi se busca en la carpeta de backups de la instancia. }
function ObtenerArchivoRestore(Param: String): String;
begin
  Result := Trim(ComboBackup.Text);
  if (Pos('\', Result) = 0) and FileExists(ObtenerCarpetaBackups('') + '\' + Result) then
    Result := ObtenerCarpetaBackups('') + '\' + Result;
end;

function HaySqlCmd: Boolean;
begin
  Result := SqlCmdEncontrado;
end;

function DebeEjecutarDeploy: Boolean;
begin
  Result := SqlCmdEncontrado and BackupOk and (not QuiereRestaurar or RestoreOk);
end;

function DebeRestaurar: Boolean;
begin
  Result := SqlCmdEncontrado and BackupOk and QuiereRestaurar;
end;

function PuedeAbrirAplicacion: Boolean;
begin
  Result := not SqlCmdEncontrado or (DeployOk and IntegridadOk);
end;

function GetCustomSetupExitCode: Integer;
begin
  Result := 0;
  if SqlCmdEncontrado and not (DeployOk and IntegridadOk) then
    Result := 10;
end;

{ ---------- utilidades ---------- }

function LeerArchivo(Ruta: String): String;
var
  Contenido: AnsiString;
begin
  Result := '';
  if LoadStringFromFile(Ruta, Contenido) then
    Result := String(Contenido);
end;

function UltimasLineas(Texto: String): String;
begin
  Result := Trim(Texto);
  if Length(Result) > 1200 then
    Result := '[...]' + #13#10 + Copy(Result, Length(Result) - 1200, 1201);
  if Result = '' then
    Result := '(el log esta vacio)';
end;

function NombreBaseValido(Nombre: String): Boolean;
var
  I: Integer;
  C: Char;
begin
  Result := Nombre <> '';
  for I := 1 to Length(Nombre) do
  begin
    C := Nombre[I];
    if not (((C >= 'A') and (C <= 'Z')) or ((C >= 'a') and (C <= 'z')) or
            ((C >= '0') and (C <= '9')) or (C = '_')) then
      Result := False;
  end;
end;

function ComandoManualDeploy: String;
begin
  Result := 'sqlcmd -S "' + ObtenerInstancia('') + '" -E -b -I -l 30 -f 65001 -v BaseDatos="' +
            ObtenerBase('') + '" -i "' + ExpandConstant('{app}\Database\Deploy_{#NombreInterno}.sql') + '"';
end;

{ Prueba la conexion y devuelve la version del servidor en Salida. }
function ProbarConexionConTiempo(Instancia: String; Segundos: Integer; var Salida: String): Boolean;
var
  Codigo: Integer;
  ArchivoTmp: String;
begin
  ArchivoTmp := ExpandConstant('{tmp}\conexion.log');
  DeleteFile(ArchivoTmp);
  Result := Exec(ExpandConstant('{cmd}'),
    '/C "sqlcmd -S "' + Instancia + '" -E -b -l ' + IntToStr(Segundos) + ' -h -1 -W -Q "SET NOCOUNT ON; SELECT CAST(SERVERPROPERTY(''ProductVersion'') AS VARCHAR(40)) + '' - '' + CAST(SERVERPROPERTY(''Edition'') AS VARCHAR(80))" > "' + ArchivoTmp + '" 2>&1"',
    '', SW_HIDE, ewWaitUntilTerminated, Codigo) and (Codigo = 0);
  Salida := Trim(LeerArchivo(ArchivoTmp));
end;

function ProbarConexion(Instancia: String; var Salida: String): Boolean;
begin
  Result := ProbarConexionConTiempo(Instancia, 30, Salida);
end;

{ La salida correcta empieza con la version, por ejemplo "15.0.4153.1 - Express Edition". }
function VersionSqlSuficiente(Salida: String): Boolean;
var
  P: Integer;
begin
  P := Pos('.', Salida);
  Result := (P > 1) and (StrToIntDef(Copy(Salida, 1, P - 1), 0) >= VersionSqlMinima);
end;

procedure AgregarInstancias(Raiz: Integer);
var
  Nombres: TArrayOfString;
  I: Integer;
  Texto: String;
begin
  if RegGetValueNames(Raiz, ClaveInstancias, Nombres) then
    for I := 0 to GetArrayLength(Nombres) - 1 do
    begin
      if CompareText(Nombres[I], 'MSSQLSERVER') = 0 then
        Texto := '.'
      else
        Texto := '.\' + Nombres[I];
      if ComboInstancia.Items.IndexOf(Texto) < 0 then
      begin
        if Texto = '.' then
          ComboInstancia.Items.Insert(0, Texto)   // la instancia predeterminada va primero
        else
          ComboInstancia.Items.Add(Texto);
      end;
    end;
end;

{ Busca el instalador viejo (MSI de Visual Studio) y devuelve su codigo de producto. }
function BuscarInstaladorViejoEn(Raiz: Integer; var Codigo: String): Boolean;
var
  Claves: TArrayOfString;
  I: Integer;
  Nombre: String;
begin
  Result := False;
  if RegGetSubkeyNames(Raiz, ClaveDesinstalar, Claves) then
    for I := 0 to GetArrayLength(Claves) - 1 do
      if RegQueryStringValue(Raiz, ClaveDesinstalar + '\' + Claves[I], 'DisplayName', Nombre) then
        if (CompareText(Nombre, NombreInstaladorViejo) = 0) and (Copy(Claves[I], 1, 1) = '{') then
        begin
          Codigo := Claves[I];
          Result := True;
          Exit;
        end;
end;

function BuscarInstaladorViejo(var Codigo: String): Boolean;
begin
  Result := BuscarInstaladorViejoEn(HKLM32, Codigo) or BuscarInstaladorViejoEn(HKLM64, Codigo) or
            BuscarInstaladorViejoEn(HKCU, Codigo);
end;

{ ---------- eventos del asistente ---------- }

function InitializeSetup: Boolean;
var
  SalidaProbe: String;
  Release: Cardinal;
  Codigo: Integer;
  ProductoAnterior: String;
  VersionAnterior: String;
begin
  Result := True;
  BackupOk := False;
  DeployOk := False;
  IntegridadOk := False;
  EsActualizacionV1 := BuscarInstaladorViejo(ProductoAnterior);
  RestoreOk := False;
  MarcaTiempo := GetDateTimeString('yyyymmdd_hhnnss', #0, #0);

  // a) .NET Framework 4.7.2
  if not RegQueryDWordValue(HKLM, 'SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full', 'Release', Release) then
    Release := 0;
  if Release < NetRelease472 then
    if MsgBox('No se detecto .NET Framework 4.7.2 o superior. La aplicacion no va a funcionar sin eso.' + #13#10 + #13#10 +
              '¿Desea continuar igualmente?', mbConfirmation, MB_YESNO) = IDNO then
    begin
      Result := False;
      Exit;
    end;

  // b) sqlcmd en el PATH
  SqlCmdEncontrado := Exec(ExpandConstant('{cmd}'), '/C where sqlcmd', '', SW_HIDE, ewWaitUntilTerminated, Codigo) and (Codigo = 0);

  // parametros opcionales de linea de comandos: /Instancia="..." /Base=...
  ParamInstancia := Trim(ExpandConstant('{param:Instancia|}'));
  ParamBase := Trim(ExpandConstant('{param:Base|}'));
  ParamRestaurar := Trim(ExpandConstant('{param:Restaurar|}'));
  if not NombreBaseValido(ParamBase) then
    ParamBase := '';

  // c) modo: reinstalacion si este setup ya dejo su clave de Registro
  EsReinstalacion := RegQueryStringValue(HKLM, ClaveRegistro, 'InstallPath', CarpetaAnterior) and (CarpetaAnterior <> '');
  if EsReinstalacion then
  begin
    if not RegQueryStringValue(HKLM, ClaveRegistro, 'SqlInstance', InstanciaAnterior) or (Trim(InstanciaAnterior) = '') then
      InstanciaAnterior := '.';
    if not RegQueryStringValue(HKLM, ClaveRegistro, 'SqlDatabase', BaseAnterior) or not NombreBaseValido(BaseAnterior) then
      BaseAnterior := '{#BaseDefecto}';
    if RegQueryStringValue(HKLM, ClaveRegistro, 'Version', VersionAnterior) and
      (VersionAnterior = '1.0.8') then
    begin
      if not RegQueryStringValue(HKCU64, ClaveRegistro, 'V1SqlInstance', InstanciaAnterior) then
        InstanciaAnterior := '';
      if not RegQueryStringValue(HKCU64, ClaveRegistro, 'V1SqlDatabase', BaseAnterior) then
        BaseAnterior := '';
    end;
    if ParamInstancia <> '' then
      InstanciaAnterior := ParamInstancia;
    if ParamBase <> '' then
      BaseAnterior := ParamBase;
  end;

  // La instancia guardada solo se reutiliza si responde (o si se indico /Instancia).
  // Si no responde (por ejemplo '.' en una PC que solo tiene LocalDB), se vuelve a pedir en el asistente.
  UsarInstanciaGuardada := EsReinstalacion and (Trim(InstanciaAnterior) <> '') and NombreBaseValido(BaseAnterior);
  if EsReinstalacion and SqlCmdEncontrado and (ParamInstancia = '') then
    if not (ProbarConexionConTiempo(InstanciaAnterior, 5, SalidaProbe) and VersionSqlSuficiente(SalidaProbe)) then
      UsarInstanciaGuardada := False;
end;

procedure ProbarClick(Sender: TObject);
var
  Salida: String;
begin
  if Trim(ComboInstancia.Text) = '' then
  begin
    EtiquetaResultado.Caption := 'Escriba o elija una instancia.';
    Exit;
  end;
  if not SqlCmdEncontrado then
  begin
    EtiquetaResultado.Caption := 'No se encontro sqlcmd en el PATH: no se puede probar la conexion.';
    Exit;
  end;
  WizardForm.NextButton.Enabled := False;
  BotonProbar.Enabled := False;
  EtiquetaResultado.Caption := 'Probando la conexion...';
  try
    if ProbarConexion(Trim(ComboInstancia.Text), Salida) then
    begin
      if VersionSqlSuficiente(Salida) then
        EtiquetaResultado.Caption := 'Conexion correcta. Servidor: ' + Salida
      else
        EtiquetaResultado.Caption := 'Conexion correcta, pero la version es antigua: ' + Salida + #13#10 + TextoMinimo;
    end
    else
      EtiquetaResultado.Caption := 'No se pudo conectar:' + #13#10 + UltimasLineas(Salida);
  finally
    WizardForm.NextButton.Enabled := True;
    BotonProbar.Enabled := True;
  end;
end;

procedure OpcionRestoreClick(Sender: TObject);
begin
  ComboBackup.Enabled := RadioRestaurar.Checked;
  BotonExaminar.Enabled := RadioRestaurar.Checked;
end;

procedure ExaminarClick(Sender: TObject);
var
  Archivo: String;
begin
  Archivo := '';
  if GetOpenFileName('Elegir copia de seguridad', Archivo, ObtenerCarpetaBackups(''),
       'Copias de seguridad (*.bak)|*.bak|Todos los archivos (*.*)|*.*', 'bak') then
    ComboBackup.Text := Archivo;
end;

{ Carga en el combo los .bak de la carpeta de Backups, del mas nuevo al mas viejo
  (el nombre termina en la fecha y hora, asi que alcanza con ordenar por nombre). }
procedure CargarBackups;
var
  Encontrado: TFindRec;
  Lista: TStringList;
  I: Integer;
begin
  Lista := TStringList.Create;
  try
    if FindFirst(ObtenerCarpetaBackups('') + '\*.bak', Encontrado) then
    begin
      try
        repeat
          Lista.Add(Encontrado.Name);
        until not FindNext(Encontrado);
      finally
        FindClose(Encontrado);
      end;
    end;
    Lista.Sort;
    for I := Lista.Count - 1 downto 0 do
      ComboBackup.Items.Add(Lista[I]);
  finally
    Lista.Free;
  end;
  if ComboBackup.Items.Count > 0 then
    ComboBackup.ItemIndex := 0;
end;

procedure CrearPaginaRestore;
var
  Etiqueta: TNewStaticText;
begin
  PaginaRestore := CreateCustomPage(PaginaInfo.ID, 'Base de datos existente',
    'Elija que hacer con la base de datos. En los dos casos primero se hace un backup de la base actual.');

  RadioActualizar := TNewRadioButton.Create(PaginaRestore);
  RadioActualizar.Parent := PaginaRestore.Surface;
  RadioActualizar.Caption := 'Actualizar la base conservando los datos (recomendado)';
  RadioActualizar.Top := 0;
  RadioActualizar.Width := PaginaRestore.SurfaceWidth;
  RadioActualizar.Checked := True;
  RadioActualizar.OnClick := @OpcionRestoreClick;

  RadioRestaurar := TNewRadioButton.Create(PaginaRestore);
  RadioRestaurar.Parent := PaginaRestore.Surface;
  RadioRestaurar.Caption := 'Restaurar la base desde una copia de seguridad (reemplaza los datos actuales)';
  RadioRestaurar.Top := RadioActualizar.Top + RadioActualizar.Height + ScaleY(10);
  RadioRestaurar.Width := PaginaRestore.SurfaceWidth;
  RadioRestaurar.OnClick := @OpcionRestoreClick;

  Etiqueta := TNewStaticText.Create(PaginaRestore);
  Etiqueta.Parent := PaginaRestore.Surface;
  Etiqueta.Caption := 'Copia de seguridad (.bak) a restaurar:';
  Etiqueta.Top := RadioRestaurar.Top + RadioRestaurar.Height + ScaleY(14);
  Etiqueta.Left := ScaleX(18);

  ComboBackup := TNewComboBox.Create(PaginaRestore);
  ComboBackup.Parent := PaginaRestore.Surface;
  ComboBackup.Style := csDropDown;
  ComboBackup.Top := Etiqueta.Top + Etiqueta.Height + ScaleY(6);
  ComboBackup.Left := ScaleX(18);
  ComboBackup.Width := PaginaRestore.SurfaceWidth - ScaleX(18) - ScaleX(100);

  BotonExaminar := TNewButton.Create(PaginaRestore);
  BotonExaminar.Parent := PaginaRestore.Surface;
  BotonExaminar.Caption := 'Examinar...';
  BotonExaminar.Top := ComboBackup.Top - ScaleY(1);
  BotonExaminar.Left := ComboBackup.Left + ComboBackup.Width + ScaleX(8);
  BotonExaminar.Width := ScaleX(90);
  BotonExaminar.Height := ScaleY(23);
  BotonExaminar.OnClick := @ExaminarClick;

  Etiqueta := TNewStaticText.Create(PaginaRestore);
  Etiqueta.Parent := PaginaRestore.Surface;
  Etiqueta.AutoSize := False;
  Etiqueta.WordWrap := True;
  Etiqueta.Top := ComboBackup.Top + ComboBackup.Height + ScaleY(14);
  Etiqueta.Left := ScaleX(18);
  Etiqueta.Width := PaginaRestore.SurfaceWidth - ScaleX(18);
  Etiqueta.Height := ScaleY(60);
  Etiqueta.Caption := 'La lista muestra las copias de ' + ObtenerCarpetaBackups('') + ', de la mas nueva a la mas vieja. ' +
    'Despues de restaurar, el instalador actualiza el esquema de la base restaurada.';

  CargarBackups;
  if ParamRestaurar <> '' then
  begin
    RadioRestaurar.Checked := True;
    ComboBackup.Text := ParamRestaurar;
  end;
  OpcionRestoreClick(nil);
end;

procedure InitializeWizard;
var
  Etiqueta: TNewStaticText;
  I: Integer;
  SalidaProbe: String;
begin
  // Pagina informativa de la reinstalacion (ShouldSkipPage la oculta en instalacion limpia)
   PaginaInfo := CreateOutputMsgPage(wpWelcome, 'Actualizacion / reinstalacion v2.0.0',
    'Ya hay una version instalada con este instalador.',
    'Se va a actualizar la aplicacion y la base de datos CONSERVANDO los datos:' + #13#10 + #13#10 +
    '  1. Se hace una copia de seguridad (backup) de la base.' + #13#10 +
    '  2. Se actualiza el esquema (tablas, procedimientos, permisos nuevos).' + #13#10 +
    '  3. No se borra ni se vuelve a cargar ningun dato existente.' + #13#10 + #13#10 +
    'En la pantalla siguiente tambien puede elegir RESTAURAR la base desde una copia anterior.' + #13#10 + #13#10 +
    'Carpeta anterior: ' + CarpetaAnterior + #13#10 +
    'Instancia de SQL Server: ' + InstanciaAnterior + #13#10 +
    'Base de datos: ' + BaseAnterior);

  if EsActualizacionV1 and not EsReinstalacion then
    PaginaInfo.MsgLabel.Caption :=
      'Se detecto la version v1 (Solucion_83KI). Se instalara la version 2.0.0.' + #13#10 + #13#10 +
      'En la pagina Base de datos indique la MISMA instancia y el MISMO nombre de base que utiliza v1.' + #13#10 +
      'El esquema original de v1 usa GestionUsuarios; confirme el nombre en su configuracion actual.' + #13#10 + #13#10 +
      'Se realizara un backup antes de actualizar el esquema. Los datos existentes se conservan.' + #13#10 +
      'Si elige otro nombre, se creara otra base y no se migraran los datos de v1.';

  // Pagina de la reinstalacion: actualizar o restaurar
  CrearPaginaRestore;

  // Pagina "Base de datos"
  PaginaBD := CreateCustomPage(wpSelectDir, 'Base de datos',
    'Elija la instancia de SQL Server donde esta (o donde se va a crear) la base.');

  Etiqueta := TNewStaticText.Create(PaginaBD);
  Etiqueta.Parent := PaginaBD.Surface;
  Etiqueta.Caption := 'Instancia de SQL Server (autenticacion integrada de Windows):';
  Etiqueta.Top := 0;

  ComboInstancia := TNewComboBox.Create(PaginaBD);
  ComboInstancia.Parent := PaginaBD.Surface;
  ComboInstancia.Style := csDropDown;
  ComboInstancia.Top := Etiqueta.Top + Etiqueta.Height + ScaleY(6);
  ComboInstancia.Width := ScaleX(280);
  AgregarInstancias(HKLM64);
  AgregarInstancias(HKLM32);
  ComboInstancia.Items.Add('(localdb)\MSSQLLocalDB');
  ComboInstancia.ItemIndex := 0;
  // Se propone la primera instancia que realmente responde (una puede estar instalada pero detenida).
  if SqlCmdEncontrado and (ParamInstancia = '') then
    for I := 0 to ComboInstancia.Items.Count - 1 do
      if ProbarConexionConTiempo(ComboInstancia.Items[I], 4, SalidaProbe) and VersionSqlSuficiente(SalidaProbe) then
      begin
        ComboInstancia.ItemIndex := I;
        Break;
      end;

  BotonProbar := TNewButton.Create(PaginaBD);
  BotonProbar.Parent := PaginaBD.Surface;
  BotonProbar.Caption := 'Probar conexion';
  BotonProbar.Top := ComboInstancia.Top - ScaleY(1);
  BotonProbar.Left := ComboInstancia.Left + ComboInstancia.Width + ScaleX(10);
  BotonProbar.Width := ScaleX(120);
  BotonProbar.Height := ScaleY(23);
  BotonProbar.OnClick := @ProbarClick;

  Etiqueta := TNewStaticText.Create(PaginaBD);
  Etiqueta.Parent := PaginaBD.Surface;
  Etiqueta.Caption := 'Nombre de la base de datos (si ya existe, se actualiza y se conservan sus datos):';
  Etiqueta.Top := ComboInstancia.Top + ComboInstancia.Height + ScaleY(16);

  EditBase := TNewEdit.Create(PaginaBD);
  EditBase.Parent := PaginaBD.Surface;
  EditBase.Top := Etiqueta.Top + Etiqueta.Height + ScaleY(6);
  EditBase.Width := ScaleX(280);
  EditBase.Text := '{#BaseDefecto}';
  if EsReinstalacion and (BaseAnterior <> '') then
    EditBase.Text := BaseAnterior;

  EtiquetaResultado := TNewStaticText.Create(PaginaBD);
  EtiquetaResultado.Parent := PaginaBD.Surface;
  EtiquetaResultado.AutoSize := False;
  EtiquetaResultado.WordWrap := True;
  EtiquetaResultado.Top := EditBase.Top + EditBase.Height + ScaleY(16);
  EtiquetaResultado.Width := PaginaBD.SurfaceWidth;
  EtiquetaResultado.Height := ScaleY(90);
  EtiquetaResultado.Caption := TextoMinimo;

  if ParamInstancia <> '' then
    ComboInstancia.Text := ParamInstancia;
  if ParamBase <> '' then
    EditBase.Text := ParamBase;
end;

function ShouldSkipPage(PageID: Integer): Boolean;
begin
  Result := False;
  if PageID = PaginaBD.ID then
    Result := UsarInstanciaGuardada    // en la reinstalacion se reutiliza la instancia guardada si responde
  else if PageID = PaginaRestore.ID then
    Result := not (EsReinstalacion and SqlCmdEncontrado)
  else if PageID = PaginaInfo.ID then
    Result := not (EsReinstalacion or EsActualizacionV1);
end;

function NextButtonClick(CurPageID: Integer): Boolean;
var
  Salida: String;
begin
  Result := True;

  if CurPageID = PaginaRestore.ID then
  begin
    if RadioRestaurar.Checked then
    begin
      if Trim(ComboBackup.Text) = '' then
      begin
        MsgBox('Elija o escriba la copia de seguridad (.bak) que quiere restaurar.', mbError, MB_OK);
        Result := False;
      end
      else if (Pos('\', ObtenerArchivoRestore('')) > 0) and not FileExists(ObtenerArchivoRestore('')) then
      begin
        MsgBox('No se encontro el archivo:' + #13#10 + ObtenerArchivoRestore(''), mbError, MB_OK);
        Result := False;
      end
      else
        Result := MsgBox('Se va a REEMPLAZAR la base "' + ObtenerBase('') + '" con el contenido de:' + #13#10 +
          ObtenerArchivoRestore('') + #13#10 + #13#10 +
          'Antes se hace un backup de la base actual, asi que se puede volver atras.' + #13#10 + #13#10 +
          '¿Desea continuar?', mbConfirmation, MB_YESNO) = IDYES;
    end;
    Exit;
  end;

  if CurPageID <> PaginaBD.ID then
    Exit;

  if Trim(ComboInstancia.Text) = '' then
  begin
    MsgBox('Indique la instancia de SQL Server.', mbError, MB_OK);
    Result := False;
    Exit;
  end;
  if not NombreBaseValido(Trim(EditBase.Text)) then
  begin
    MsgBox('El nombre de la base solo puede tener letras, numeros y guion bajo.', mbError, MB_OK);
    Result := False;
    Exit;
  end;

  if not SqlCmdEncontrado then
  begin
    Result := MsgBox('No se encontro sqlcmd en el PATH.' + #13#10 +
      'La aplicacion se va a instalar, pero la base de datos NO se va a crear ni actualizar.' + #13#10 + #13#10 +
      '¿Desea continuar igualmente?', mbConfirmation, MB_YESNO) = IDYES;
    Exit;
  end;

  WizardForm.NextButton.Enabled := False;
  try
    if not ProbarConexion(Trim(ComboInstancia.Text), Salida) then
      Result := MsgBox('No se pudo conectar a "' + Trim(ComboInstancia.Text) + '":' + #13#10 + #13#10 +
        UltimasLineas(Salida) + #13#10 + #13#10 + '¿Desea continuar igualmente?', mbConfirmation, MB_YESNO) = IDYES
    else if not VersionSqlSuficiente(Salida) then
      Result := MsgBox('La version del servidor es antigua: ' + Salida + #13#10 + TextoMinimo + #13#10 + #13#10 +
        '¿Desea continuar igualmente?', mbConfirmation, MB_YESNO) = IDYES;
  finally
    WizardForm.NextButton.Enabled := True;
  end;
end;

function UpdateReadyMemo(Space, NewLine, MemoUserInfoInfo, MemoDirInfo, MemoTypeInfo,
  MemoComponentsInfo, MemoGroupInfo, MemoTasksInfo: String): String;
var
  Modo: String;
begin
  if QuiereRestaurar then
    Modo := 'Reinstalacion: RESTAURAR la base desde ' + ObtenerArchivoRestore('')
  else if EsReinstalacion then
    Modo := 'Reinstalacion / actualizacion v2.0.0 (se conservan los datos)'
  else if EsActualizacionV1 then
    Modo := 'Actualizacion v1 a v2.0.0 (confirme la base de v1)'
  else
    Modo := 'Instalacion (si la base ya existe, se actualiza conservando los datos)';
  if not SqlCmdEncontrado then
    Modo := Modo + ' - SIN sqlcmd: no se toca la base de datos';

  Result := 'Modo:' + NewLine + Space + Modo + NewLine + NewLine +
            MemoDirInfo + NewLine + NewLine +
            'Base de datos:' + NewLine +
            Space + 'Instancia: ' + ObtenerInstancia('') + NewLine +
            Space + 'Base: ' + ObtenerBase('') + NewLine + NewLine +
            'Backup previo (solo si la base ya existe):' + NewLine +
            Space + ObtenerCarpetaBackups('') + '\' + ObtenerArchivoBackup('') + NewLine + NewLine +
            'Logs:' + NewLine + Space + ExpandConstant('{commonappdata}\{#NombreInterno}\Logs');
  if MemoTasksInfo <> '' then
    Result := Result + NewLine + NewLine + MemoTasksInfo;
end;

{ ---------- verificaciones de [Run] ---------- }

procedure VerificarBackup;
var
  Log: String;
begin
  Log := LeerArchivo(ObtenerLogBackup(''));
  BackupOk := (Pos('BACKUP_SIN_BASE', Log) > 0) or (Pos('BACKUP_OK', Log) > 0);
  // BACKUP_OK tambien coincide con BACKUP_OK_CARPETA_SQL (backup en la carpeta de la instancia)
  if not BackupOk then
    MsgBox('No se pudo hacer la copia de seguridad de la base, asi que la base NO se actualizo.' + #13#10 +
      'La aplicacion queda instalada y no se borro nada.' + #13#10 + #13#10 +
      UltimasLineas(Log) + #13#10 + #13#10 +
      'Log: ' + ObtenerLogBackup('') + #13#10 + #13#10 +
      'Para actualizar la base a mano (despues de hacer un backup):' + #13#10 + ComandoManualDeploy,
      mbError, MB_OK);
end;

procedure VerificarRestore;
var
  Log: String;
begin
  Log := LeerArchivo(ObtenerLogRestore(''));
  RestoreOk := Pos('RESTORE_OK', Log) > 0;
  if not RestoreOk then
    MsgBox('No se pudo restaurar la base desde la copia de seguridad. La base NO se actualizo.' + #13#10 +
      'La aplicacion queda instalada. El backup de la base que habia antes esta en:' + #13#10 +
      ObtenerCarpetaBackups('') + '\' + ObtenerArchivoBackup('') + #13#10 + #13#10 +
      UltimasLineas(Log) + #13#10 + #13#10 +
      'Log: ' + ObtenerLogRestore(''),
      mbError, MB_OK);
end;

procedure VerificarDeploy;
var
  Log: String;
  Codigo: Integer;
begin
  Log := LeerArchivo(ObtenerLogDeploy(''));
  DeployOk := Pos('DEPLOY_OK', Log) > 0;
  if not DeployOk then
    MsgBox('La base de datos no se pudo crear o actualizar. No se aplico ningun cambio en la base' + #13#10 +
      '(todo el script corre en una transaccion). La aplicacion queda instalada igual.' + #13#10 + #13#10 +
      UltimasLineas(Log) + #13#10 + #13#10 +
      'Log: ' + ObtenerLogDeploy('') + #13#10 +
      'Backup: ' + ObtenerCarpetaBackups('') + #13#10 + #13#10 +
      'Para volver a ejecutarlo a mano:' + #13#10 + ComandoManualDeploy,
      mbError, MB_OK)
  else
  begin
    IntegridadOk := Exec(ExpandConstant('{app}\{#ExeApp}'), '/recalcular-integridad',
      '', SW_HIDE, ewWaitUntilTerminated, Codigo) and (Codigo = 0);
    if not IntegridadOk then
      MsgBox('El esquema se actualizo, pero fallo el recalculo de integridad (codigo ' +
        IntToStr(Codigo) + '). Ejecute nuevamente este instalador para reintentar.', mbError, MB_OK);
  end;
end;

{ ---------- pasos de la instalacion ---------- }

procedure DesinstalarInstaladorViejo;
var
  CodigoProducto: String;
  Codigo: Integer;
begin
  if WizardSilent then
    Exit;   // en modo silencioso no se desinstala nada sin preguntar
  if not BuscarInstaladorViejo(CodigoProducto) then
    Exit;
  if MsgBox('Se encontro la version anterior instalada con el instalador viejo (' + NombreInstaladorViejo + ').' + #13#10 +
            '¿Desea desinstalarla ahora? La base de datos NO se toca.', mbConfirmation, MB_YESNO) = IDNO then
    Exit;
  if not Exec('msiexec.exe', '/x ' + CodigoProducto + ' /passive /norestart', '', SW_SHOW, ewWaitUntilTerminated, Codigo) or (Codigo <> 0) then
    MsgBox('No se pudo desinstalar la version anterior (codigo ' + IntToStr(Codigo) + ').' + #13#10 +
           'Puede quitarla despues desde "Agregar o quitar programas". La instalacion continua.', mbInformation, MB_OK);
end;

{ Deja la instancia y la base como valores por defecto en UI.exe.config.
  Se hace siempre, porque el .config se reemplaza en cada instalacion. }
procedure ConfigurarConexion;
var
  Codigo: Integer;
  Log, RutaConfig: String;
begin
  Log := ExpandConstant('{commonappdata}\{#NombreInterno}\Logs\Config.log');
  RutaConfig := ExpandConstant('{app}\{#ExeApp}.config');
  DeleteFile(Log);
  if not Exec(ExpandConstant('{cmd}'),
       '/C "powershell.exe -NoProfile -ExecutionPolicy Bypass -File "' + ExpandConstant('{app}\Database\ConfigurarConexion.ps1') +
       '" -RutaConfig "' + RutaConfig + '" -Instancia "' + ObtenerInstancia('') + '" -BaseDatos "' + ObtenerBase('') +
       '" > "' + Log + '" 2>&1"',
       '', SW_HIDE, ewWaitUntilTerminated, Codigo) or (Codigo <> 0) or (Pos('CONFIG_OK', LeerArchivo(Log)) = 0) then
    MsgBox('No se pudo guardar la conexion en ' + RutaConfig + '.' + #13#10 + #13#10 +
      UltimasLineas(LeerArchivo(Log)) + #13#10 + #13#10 +
      'No es grave: al abrir la aplicacion se muestra la pantalla de configuracion y ahi puede elegir' + #13#10 +
      'la instancia "' + ObtenerInstancia('') + '" y la base "' + ObtenerBase('') + '".',
      mbInformation, MB_OK);
end;

procedure CurStepChanged(CurStep: TSetupStep);
begin
  if CurStep = ssInstall then
    DesinstalarInstaladorViejo
  else if CurStep = ssPostInstall then
    ConfigurarConexion;
end;

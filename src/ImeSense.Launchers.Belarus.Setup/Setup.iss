#define MyAppName "Belarus Launcher"
#define MyAppVersion "2.0"
#define MyAppPublisher "ImeSense"
#define MyAppURL "https://github.com/imesense/belarus-launcher"
#define MyAppExeName "SBLauncher.exe"

[Setup]
AppId={{6CFDCEE0-6B00-4CD5-946C-78E081AD619B}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={sd}\Games\S.T.A.L.K.E.R. Belarus
DisableProgramGroupPage=yes
;PrivilegesRequired=lowest
PrivilegesRequiredOverridesAllowed=dialog
OutputBaseFilename=SBLauncherInstaller
OutputDir=..\..\bin
Compression=lzma
SolidCompression=yes
WizardStyle=modern
SetupIconFile=logo.ico
LicenseFile=license.txt
DisableDirPage=no

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"
Name: "russian"; MessagesFile: "compiler:Languages\Russian.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "..\..\bin\Release\win-x64\publish\{#MyAppExeName}"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\..\bin\Release\win-x64\publish\SBLauncherUpdater.exe"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{autoprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent shellexec

[UninstallDelete]
Type: FilesAndOrDirs; Name: {app}

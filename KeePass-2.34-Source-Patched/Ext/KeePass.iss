; KeePass Password Safe Installation Script
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!
; Thanks to Hilbrand Edskes for installer improvements.

#define MyAppNameShort "KeePass"
#define MyAppNameShortEx "KeePass 2"
#define MyAppName "KeePass Password Safe"
#define MyAppNameEx "KeePass Password Safe 2"
#define MyAppPublisher "Dominik Reichl"

#define KeeVersionStr "2.34"
#define KeeVersionStrWithMinor "2.34"
#define KeeVersionStrWithMinorPath "2.34"
#define KeeVersionWin "2.34.0.0"
#define KeeVersionWinShort "2.34"

#define MyAppURL "http://keepass.info/"
#define MyAppExeName "KeePass.exe"
#define MyAppUrlName "KeePass.url"
#define MyAppHelpName "KeePass.chm"
#define KeeDevPeriod "2003-2016"
#define MyAppId "KeePassPasswordSafe2"

[Setup]
AppName={#MyAppName}
AppVersion={#KeeVersionWinShort}
AppVerName={#MyAppName} {#KeeVersionStrWithMinor}
AppId={#MyAppId}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
AppCopyright=Copyright (c) {#KeeDevPeriod} {#MyAppPublisher}
MinVersion=5.0
DefaultDirName={pf}\{#MyAppNameEx}
DefaultGroupName={#MyAppNameEx}
AllowNoIcons=yes
LicenseFile=..\Docs\License.txt
OutputDir=..\Build\KeePass_Distrib
OutputBaseFilename={#MyAppNameShort}-{#KeeVersionStrWithMinorPath}-Setup
Compression=lzma2/ultra
SolidCompression=yes
InternalCompressLevel=ultra
UninstallDisplayIcon={app}\{#MyAppExeName}
AppMutex=KeePassAppMutex,Global\KeePassAppMutexEx
SetupMutex=KeePassSetupMutex2
VersionInfoVersion={#KeeVersionWin}
VersionInfoCompany={#MyAppPublisher}
VersionInfoDescription={#MyAppName} {#KeeVersionStr} Setup
VersionInfoCopyright=Copyright (c) {#KeeDevPeriod} {#MyAppPublisher}
WizardImageFile=compiler:WizModernImage-IS.bmp
WizardSmallImageFile=compiler:WizModernSmallImage-IS.bmp
DisableDirPage=auto
AlwaysShowDirOnReadyPage=yes
DisableProgramGroupPage=yes
AlwaysShowGroupOnReadyPage=no

[Languages]
Name: english; MessagesFile: compiler:Default.isl
Name: brazilianportuguese; MessagesFile: compiler:Languages\BrazilianPortuguese.isl
Name: catalan; MessagesFile: compiler:Languages\Catalan.isl
Name: czech; MessagesFile: compiler:Languages\Czech.isl
Name: danish; MessagesFile: compiler:Languages\Danish.isl
Name: dutch; MessagesFile: compiler:Languages\Dutch.isl
Name: finnish; MessagesFile: compiler:Languages\Finnish.isl
Name: french; MessagesFile: compiler:Languages\French.isl
Name: german; MessagesFile: compiler:Languages\German.isl
Name: hungarian; MessagesFile: compiler:Languages\Hungarian.isl
Name: italian; MessagesFile: compiler:Languages\Italian.isl
Name: norwegian; MessagesFile: compiler:Languages\Norwegian.isl
Name: polish; MessagesFile: compiler:Languages\Polish.isl
Name: portuguese; MessagesFile: compiler:Languages\Portuguese.isl
Name: russian; MessagesFile: compiler:Languages\Russian.isl
; Name: slovak; MessagesFile: compiler:Languages\Slovak.isl
Name: slovenian; MessagesFile: compiler:Languages\Slovenian.isl
Name: spanish; MessagesFile: compiler:Languages\Spanish.isl

[Tasks]
Name: FileAssoc; Description: {cm:AssocFileExtension,{#MyAppNameShort},.kdbx}
Name: DesktopIcon; Description: {cm:CreateDesktopIcon}; GroupDescription: {cm:AdditionalIcons}; Flags: unchecked
Name: QuickLaunchIcon; Description: {cm:CreateQuickLaunchIcon}; GroupDescription: {cm:AdditionalIcons}; Flags: unchecked

[Components]
Name: Core; Description: Core KeePass Application Files; Flags: fixed; Types: full compact custom
Name: UserDoc; Description: Help Manual; Types: full custom
Name: KeePassLibC; Description: Native Support Library (KeePass 1.x); Types: full custom
; Name: NativeLib; Description: Native Crypto Library (Fast Key Transformations); Types: full custom
Name: XSL; Description: XSL Stylesheets for KDBX XML Files; Types: full custom
Name: NGen; Description: Optimize KeePass Performance; Types: full custom; ExtraDiskSpaceRequired: 1048576
Name: PreLoad; Description: Optimize KeePass On-Demand Start-Up Performance; Types: full custom; ExtraDiskSpaceRequired: 2048
; Name: FileAssoc; Description: {cm:AssocFileExtension,{#MyAppNameShort},.kdbx}; Types: full custom

[Dirs]
Name: "{app}\Plugins"; Flags: uninsalwaysuninstall

[Files]
Source: ..\Build\KeePass_Distrib\KeePass.exe; DestDir: {app}; Flags: ignoreversion; Components: Core
Source: ..\Build\KeePass_Distrib\KeePass.XmlSerializers.dll; DestDir: {app}; Flags: ignoreversion; Components: Core
Source: ..\Build\KeePass_Distrib\KeePass.exe.config; DestDir: {app}; Flags: ignoreversion; Components: Core
Source: ..\Build\KeePass_Distrib\KeePass.config.xml; DestDir: {app}; Flags: onlyifdoesntexist; Components: Core
Source: ..\Build\KeePass_Distrib\License.txt; DestDir: {app}; Flags: ignoreversion; Components: Core
Source: ..\Build\KeePass_Distrib\ShInstUtil.exe; DestDir: {app}; Flags: ignoreversion; Components: Core
Source: ..\Build\KeePass_Distrib\KeePass.chm; DestDir: {app}; Flags: ignoreversion; Components: UserDoc
Source: ..\Build\KeePass_Distrib\KeePassLibC32.dll; DestDir: {app}; Flags: ignoreversion; Components: KeePassLibC
Source: ..\Build\KeePass_Distrib\KeePassLibC64.dll; DestDir: {app}; Flags: ignoreversion; Components: KeePassLibC
; Source: ..\Build\KeePass_Distrib\KeePassNtv32.dll; DestDir: {app}; Flags: ignoreversion; Components: NativeLib
; Source: ..\Build\KeePass_Distrib\KeePassNtv64.dll; DestDir: {app}; Flags: ignoreversion; Components: NativeLib
Source: ..\Build\KeePass_Distrib\XSL\KDBX_DetailsFull.xsl; DestDir: {app}\XSL; Components: XSL
Source: ..\Build\KeePass_Distrib\XSL\KDBX_DetailsLite.xsl; DestDir: {app}\XSL; Components: XSL
Source: ..\Build\KeePass_Distrib\XSL\KDBX_PasswordsOnly.xsl; DestDir: {app}\XSL; Components: XSL
Source: ..\Build\KeePass_Distrib\XSL\KDBX_Styles.css; DestDir: {app}\XSL; Components: XSL
Source: ..\Build\KeePass_Distrib\XSL\KDBX_Tabular.xsl; DestDir: {app}\XSL; Components: XSL
Source: ..\Build\KeePass_Distrib\XSL\TableHeader.gif; DestDir: {app}\XSL; Components: XSL

[Registry]
; Always unregister .kdbx association at uninstall
Root: HKCR; Subkey: .kdbx; Flags: uninsdeletekey; Tasks: not FileAssoc
Root: HKCR; Subkey: kdbxfile; Flags: uninsdeletekey; Tasks: not FileAssoc
; Register .kdbx association at install, and unregister at uninstall
Root: HKCR; Subkey: .kdbx; ValueType: string; ValueData: kdbxfile; Flags: uninsdeletekey; Tasks: FileAssoc
Root: HKCR; Subkey: kdbxfile; ValueType: string; ValueData: KeePass Database; Flags: uninsdeletekey; Tasks: FileAssoc
Root: HKCR; Subkey: kdbxfile; ValueType: string; ValueName: AlwaysShowExt; Flags: uninsdeletekey; Tasks: FileAssoc
Root: HKCR; Subkey: kdbxfile\DefaultIcon; ValueType: string; ValueData: """{app}\{#MyAppExeName}"",0"; Flags: uninsdeletekey; Tasks: FileAssoc
Root: HKCR; Subkey: kdbxfile\shell\open; ValueType: string; ValueData: &Open with {#MyAppName}; Flags: uninsdeletekey; Tasks: FileAssoc
Root: HKCR; Subkey: kdbxfile\shell\open\command; ValueType: string; ValueData: """{app}\{#MyAppExeName}"" ""%1"""; Flags: uninsdeletekey; Tasks: FileAssoc

; [INI]
; Filename: {app}\{#MyAppUrlName}; Section: InternetShortcut; Key: URL; String: {#MyAppURL}

[Icons]
; Name: {group}\{#MyAppName}; Filename: {app}\{#MyAppExeName}
; Name: {group}\{cm:ProgramOnTheWeb,{#MyAppName}}; Filename: {app}\{#MyAppUrlName}
; Name: {group}\Help; Filename: {app}\{#MyAppHelpName}; Components: UserDoc
; Name: {group}\{cm:UninstallProgram,{#MyAppName}}; Filename: {uninstallexe}
Name: {commonprograms}\{#MyAppNameShortEx}; Filename: {app}\{#MyAppExeName}
Name: {userdesktop}\{#MyAppNameShortEx}; Filename: {app}\{#MyAppExeName}; Tasks: DesktopIcon
Name: {userappdata}\Microsoft\Internet Explorer\Quick Launch\{#MyAppNameShortEx}; Filename: {app}\{#MyAppExeName}; Tasks: QuickLaunchIcon

[Run]
; Filename: {app}\KeePass.exe; Parameters: -RegisterFileExt; Components: FileAssoc
Filename: {app}\ShInstUtil.exe; Parameters: net_check; WorkingDir: {app}; Flags: skipifdoesntexist skipifsilent
Filename: {app}\ShInstUtil.exe; Parameters: preload_register; WorkingDir: {app}; StatusMsg: "Optimizing KeePass on-demand start-up performance..."; Flags: skipifdoesntexist; Components: PreLoad
Filename: {app}\ShInstUtil.exe; Parameters: ngen_install; WorkingDir: {app}; StatusMsg: "Optimizing KeePass performance..."; Flags: skipifdoesntexist; Components: NGen
Filename: {app}\{#MyAppExeName}; Description: {cm:LaunchProgram,{#MyAppNameShort}}; Flags: postinstall nowait skipifsilent

[UninstallRun]
; Filename: {app}\KeePass.exe; Parameters: -UnregisterFileExt
Filename: {app}\ShInstUtil.exe; Parameters: preload_unregister; WorkingDir: {app}; Flags: skipifdoesntexist; RunOnceId: "PreLoad"; Components: PreLoad
Filename: {app}\ShInstUtil.exe; Parameters: ngen_uninstall; WorkingDir: {app}; Flags: skipifdoesntexist; RunOnceId: "NGen"; Components: NGen

; Delete old files when upgrading
[InstallDelete]
Name: {app}\{#MyAppUrlName}; Type: files
Name: {group}\{#MyAppName}.lnk; Type: files
Name: {group}\{cm:ProgramOnTheWeb,{#MyAppName}}.lnk; Type: files
Name: {group}\Help.lnk; Type: files
Name: {group}\{cm:UninstallProgram,{#MyAppName}}.lnk; Type: files
Name: {group}; Type: dirifempty
Name: {userdesktop}\{#MyAppName}.lnk; Type: files
Name: {userappdata}\Microsoft\Internet Explorer\Quick Launch\{#MyAppName}.lnk; Type: files

; [UninstallDelete]
; Type: files; Name: {app}\{#MyAppUrlName}

TrlUtil.exe src_from_xml DefaultText.xml
MOVE /Y KPRes.Generated.cs ..\KeePass\Resources\KPRes.Generated.cs
MOVE /Y KLRes.Generated.cs ..\KeePassLib\Resources\KLRes.Generated.cs

TrlUtil.exe src_from_xml DefaultTextSD.xml
MOVE /Y KSRes.Generated.cs ..\KeePassLib\Resources\KSRes.Generated.cs

PAUSE
CLS
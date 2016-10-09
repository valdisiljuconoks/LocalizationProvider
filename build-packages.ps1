cd .\.nuget

.\nuget.exe pack ..\DbLocalizationProvider.Abstractions\DbLocalizationProvider.Abstractions.csproj -Properties Configuration=Release
.\nuget.exe pack ..\DbLocalizationProvider\DbLocalizationProvider.csproj -Properties Configuration=Release
.\nuget.exe pack ..\DbLocalizationProvider.AdminUI\DbLocalizationProvider.AdminUI.csproj -Properties Configuration=Release
.\nuget.exe pack ..\DbLocalizationProvider.EPiServer\DbLocalizationProvider.EPiServer.csproj -Properties Configuration=Release
.\nuget.exe pack ..\DbLocalizationProvider.AdminUI.EPiServer\DbLocalizationProvider.AdminUI.EPiServer.csproj -Properties Configuration=Release
.\nuget.exe pack ..\DbLocalizationProvider.MigrationTool\DbLocalizationProvider.MigrationTool.csproj -Properties Configuration=Release -tool
cd ..\
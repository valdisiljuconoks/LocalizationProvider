cd C:\Users\valdis\Documents\GitHubVisualStudio\DbLocalizationProvider\.nuget

.\nuget.exe pack ..\DbLocalizationProvider\DbLocalizationProvider.csproj -Properties Configuration=Release
.\nuget.exe pack ..\DbLocalizationProvider.AdminUI\DbLocalizationProvider.AdminUI.csproj -Properties Configuration=Release
.\nuget.exe pack ..\DbLocalizationProvider.MigrationTool\DbLocalizationProvider.MigrationTool.csproj -Properties Configuration=Release -tool
cd ..\
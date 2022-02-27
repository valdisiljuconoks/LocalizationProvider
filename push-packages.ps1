cd .\.nuget

.\nuget.exe push LocalizationProvider.Abstractions.6.5.2.nupkg -source https://api.nuget.org/v3/index.json
.\nuget.exe push LocalizationProvider.6.5.2.nupkg -source https://api.nuget.org/v3/index.json
.\nuget.exe push LocalizationProvider.AdminUI.Models.6.5.2.nupkg -source https://api.nuget.org/v3/index.json
.\nuget.exe push LocalizationProvider.Csv.6.5.2.nupkg -source https://api.nuget.org/v3/index.json
.\nuget.exe push LocalizationProvider.Storage.PostgreSql.6.5.2.nupkg -source https://api.nuget.org/v3/index.json
.\nuget.exe push LocalizationProvider.Storage.SqlServer.6.5.2.nupkg -source https://api.nuget.org/v3/index.json

cd ..\

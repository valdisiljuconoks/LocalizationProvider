cd .\.nuget

.\nuget.exe push LocalizationProvider.Abstractions.8.0.0.nupkg -source https://api.nuget.org/v3/index.json
.\nuget.exe push LocalizationProvider.8.0.0.nupkg -source https://api.nuget.org/v3/index.json
.\nuget.exe push LocalizationProvider.AdminUI.Models.8.0.0.nupkg -source https://api.nuget.org/v3/index.json
.\nuget.exe push LocalizationProvider.Csv.8.0.0.nupkg -source https://api.nuget.org/v3/index.json
.\nuget.exe push LocalizationProvider.Storage.AzureTables.8.0.0.nupkg -source https://api.nuget.org/v3/index.json
.\nuget.exe push LocalizationProvider.Storage.PostgreSql.8.0.0.nupkg -source https://api.nuget.org/v3/index.json
.\nuget.exe push LocalizationProvider.Storage.SqlServer.8.0.0.nupkg -source https://api.nuget.org/v3/index.json
.\nuget.exe push LocalizationProvider.Xliff.8.0.0.nupkg -source https://api.nuget.org/v3/index.json

cd ..\

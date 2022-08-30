cd .\.nuget

.\nuget.exe push LocalizationProvider.Abstractions.7.3.1.nupkg -source https://api.nuget.org/v3/index.json
.\nuget.exe push LocalizationProvider.7.3.1.nupkg -source https://api.nuget.org/v3/index.json
.\nuget.exe push LocalizationProvider.AdminUI.Models.7.3.1.nupkg -source https://api.nuget.org/v3/index.json
.\nuget.exe push LocalizationProvider.Csv.7.3.1.nupkg -source https://api.nuget.org/v3/index.json
.\nuget.exe push LocalizationProvider.Storage.AzureTables.7.3.1.nupkg -source https://api.nuget.org/v3/index.json
.\nuget.exe push LocalizationProvider.Storage.PostgreSql.7.3.1.nupkg -source https://api.nuget.org/v3/index.json
.\nuget.exe push LocalizationProvider.Storage.SqlServer.7.3.1.nupkg -source https://api.nuget.org/v3/index.json
.\nuget.exe push LocalizationProvider.Xliff.7.3.1.nupkg -source https://api.nuget.org/v3/index.json

cd ..\

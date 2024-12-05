cd .\.nuget

nuget push LocalizationProvider.Abstractions.9.0.0.nupkg -source https://api.nuget.org/v3/index.json
nuget push LocalizationProvider.9.0.0.nupkg -source https://api.nuget.org/v3/index.json
nuget push LocalizationProvider.AdminUI.Models.9.0.0.nupkg -source https://api.nuget.org/v3/index.json
nuget push LocalizationProvider.Csv.9.0.0.nupkg -source https://api.nuget.org/v3/index.json
nuget push LocalizationProvider.Storage.AzureTables.9.0.0.nupkg -source https://api.nuget.org/v3/index.json
nuget push LocalizationProvider.Storage.PostgreSql.9.0.0.nupkg -source https://api.nuget.org/v3/index.json
nuget push LocalizationProvider.Storage.SqlServer.9.0.0.nupkg -source https://api.nuget.org/v3/index.json
nuget push LocalizationProvider.Xliff.9.0.0.nupkg -source https://api.nuget.org/v3/index.json
nuget push LocalizationProvider.Translator.Azure.9.0.0.nupkg -source https://api.nuget.org/v3/index.json

cd ..\

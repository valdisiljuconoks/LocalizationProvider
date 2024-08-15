cd .\.nuget

nuget push LocalizationProvider.Abstractions.8.1.4.nupkg -source https://api.nuget.org/v3/index.json
nuget push LocalizationProvider.8.1.4.nupkg -source https://api.nuget.org/v3/index.json
nuget push LocalizationProvider.AdminUI.Models.8.1.4.nupkg -source https://api.nuget.org/v3/index.json
nuget push LocalizationProvider.Csv.8.1.4.nupkg -source https://api.nuget.org/v3/index.json
nuget push LocalizationProvider.Storage.AzureTables.8.1.4.nupkg -source https://api.nuget.org/v3/index.json
nuget push LocalizationProvider.Storage.PostgreSql.8.1.4.nupkg -source https://api.nuget.org/v3/index.json
nuget push LocalizationProvider.Storage.SqlServer.8.1.4.nupkg -source https://api.nuget.org/v3/index.json
nuget push LocalizationProvider.Xliff.8.1.4.nupkg -source https://api.nuget.org/v3/index.json
nuget push LocalizationProvider.Translator.Azure.8.1.4.nupkg -source https://api.nuget.org/v3/index.json

cd ..\

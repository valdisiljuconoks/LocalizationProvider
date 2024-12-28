# nuget setapikey .... -Source https://www.nuget.org/api/v2/package
# nuget setapikey .... -Source https://www.nuget.org/api/v2/symbolpackage

cd .\.nuget

nuget push LocalizationProvider.AspNetCore.8.2.4.nupkg -source https://api.nuget.org/v3/index.json
nuget push LocalizationProvider.AdminUI.AspNetCore.Csv.8.2.4.nupkg -source https://api.nuget.org/v3/index.json
nuget push LocalizationProvider.AdminUI.AspNetCore.Xliff.8.2.4.nupkg -source https://api.nuget.org/v3/index.json
nuget push LocalizationProvider.AdminUI.AspNetCore.8.2.4.nupkg -source https://api.nuget.org/v3/index.json

cd ..\

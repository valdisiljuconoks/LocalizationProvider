cd .\.nuget

nuget push .\LocalizationProvider.Abstractions.5.3.0.nupkg -source https://api.nuget.org/v3/index.json
nuget push .\LocalizationProvider.5.3.0.nupkg -source https://api.nuget.org/v3/index.json

cd ..\

cd .\.nuget

# episerver libraries

cd .\..\src\DbLocalizationProvider.EPiServer\
dotnet build -c Release
dotnet pack -c Release
copy .\bin\Release\DbLocalizationProvider.EPiServer.*.nupkg .\..\..\.nuget\
copy .\bin\Release\DbLocalizationProvider.EPiServer.*.snupkg .\..\..\.nuget\
cd .\..\..\.nuget\

cd .\..\src\DbLocalizationProvider.AdminUI.EPiServer\
dotnet build -c Release
dotnet pack -c Release
copy .\bin\Release\DbLocalizationProvider.AdminUI.EPiServer.*.nupkg .\..\..\.nuget\
copy .\bin\Release\DbLocalizationProvider.AdminUI.EPiServer.*.snupkg .\..\..\.nuget\
cd .\..\..\.nuget\

cd ..
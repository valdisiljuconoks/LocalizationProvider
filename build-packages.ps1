cd .\.nuget

# common libraries
cd .\..\src\DbLocalizationProvider.Abstractions\
dotnet pack -c Release
copy .\bin\Release\*.nupkg .\..\..\.nuget\
cd .\..\..\.nuget\

cd .\..\src\DbLocalizationProvider\
dotnet pack -c Release
copy .\bin\Release\*.nupkg .\..\..\.nuget\
cd .\..\..\.nuget\

cd ..\

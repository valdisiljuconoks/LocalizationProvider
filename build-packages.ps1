cd .\.nuget

# common libraries
cd .\..\src\DbLocalizationProvider.Abstractions\
dotnet pack -c Release
dotnet pack --include-symbols -p:SymbolPackageFormat=snupkg
copy .\bin\Release\*.nupkg .\..\..\.nuget\
copy .\bin\Release\*.snupkg .\..\..\.nuget\
cd .\..\..\.nuget\

cd .\..\src\DbLocalizationProvider\
dotnet pack -c Release
dotnet pack --include-symbols -p:SymbolPackageFormat=snupkg
copy .\bin\Release\*.nupkg .\..\..\.nuget\
copy .\bin\Release\*.snupkg .\..\..\.nuget\
cd .\..\..\.nuget\

# storage
cd .\..\src\DbLocalizationProvider.Storage.SqlServer\
dotnet pack -c Release
dotnet pack --include-symbols -p:SymbolPackageFormat=snupkg
copy .\bin\Release\*.nupkg .\..\..\.nuget\
copy .\bin\Release\*.snupkg .\..\..\.nuget\
cd .\..\..\.nuget\

cd .\..\src\DbLocalizationProvider.Storage.PostreSql\
dotnet pack -c Release
dotnet pack --include-symbols -p:SymbolPackageFormat=snupkg
copy .\bin\Release\*.nupkg .\..\..\.nuget\
copy .\bin\Release\*.snupkg .\..\..\.nuget\
cd .\..\..\.nuget\

cd ..\

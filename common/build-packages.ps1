cd .\.nuget

# common libraries
cd .\..\src\DbLocalizationProvider.Abstractions\
dotnet build -c Release
dotnet pack -c Release
dotnet pack --include-symbols -p:SymbolPackageFormat=snupkg
copy .\bin\Release\*.nupkg .\..\..\.nuget\
copy .\bin\Release\*.snupkg .\..\..\.nuget\
cd .\..\..\.nuget\

cd .\..\src\DbLocalizationProvider\
dotnet build -c Release
dotnet pack -c Release
dotnet pack --include-symbols -p:SymbolPackageFormat=snupkg
copy .\bin\Release\*.nupkg .\..\..\.nuget\
copy .\bin\Release\*.snupkg .\..\..\.nuget\
cd .\..\..\.nuget\

cd .\..\src\DbLocalizationProvider.AdminUI.Models\
dotnet build -c Release
dotnet pack -c Release
dotnet pack --include-symbols -p:SymbolPackageFormat=snupkg
copy .\bin\Release\*.nupkg .\..\..\.nuget\
copy .\bin\Release\*.snupkg .\..\..\.nuget\
cd .\..\..\.nuget\

cd .\..\src\DbLocalizationProvider.Csv\
dotnet build -c Release
dotnet pack -c Release
dotnet pack --include-symbols -p:SymbolPackageFormat=snupkg
copy .\bin\Release\*.nupkg .\..\..\.nuget\
copy .\bin\Release\*.snupkg .\..\..\.nuget\
cd .\..\..\.nuget\

cd .\..\src\DbLocalizationProvider.Xliff\
dotnet build -c Release
dotnet pack -c Release
dotnet pack --include-symbols -p:SymbolPackageFormat=snupkg
copy .\bin\Release\*.nupkg .\..\..\.nuget\
copy .\bin\Release\*.snupkg .\..\..\.nuget\
cd .\..\..\.nuget\

# storage
cd .\..\src\DbLocalizationProvider.Storage.SqlServer\
dotnet build -c Release
dotnet pack -c Release
dotnet pack --include-symbols -p:SymbolPackageFormat=snupkg
copy .\bin\Release\*.nupkg .\..\..\.nuget\
copy .\bin\Release\*.snupkg .\..\..\.nuget\
cd .\..\..\.nuget\

cd .\..\src\DbLocalizationProvider.Storage.PostgreSql\
dotnet build -c Release
dotnet pack -c Release
dotnet pack --include-symbols -p:SymbolPackageFormat=snupkg
copy .\bin\Release\*.nupkg .\..\..\.nuget\
copy .\bin\Release\*.snupkg .\..\..\.nuget\
cd .\..\..\.nuget\

cd .\..\src\DbLocalizationProvider.Storage.MongoDb\
dotnet build -c Release
dotnet pack -c Release
dotnet pack --include-symbols -p:SymbolPackageFormat=snupkg
copy .\bin\Release\*.nupkg .\..\..\.nuget\
copy .\bin\Release\*.snupkg .\..\..\.nuget\
cd .\..\..\.nuget\

cd .\..\src\DbLocalizationProvider.Storage.AzureTables\
dotnet build -c Release
dotnet pack -c Release
dotnet pack --include-symbols -p:SymbolPackageFormat=snupkg
copy .\bin\Release\*.nupkg .\..\..\.nuget\
copy .\bin\Release\*.snupkg .\..\..\.nuget\
cd .\..\..\.nuget\

cd .\..\src\DbLocalizationProvider.Translator.Azure\
dotnet build -c Release
dotnet pack -c Release
dotnet pack --include-symbols -p:SymbolPackageFormat=snupkg
copy .\bin\Release\*.nupkg .\..\..\.nuget\
copy .\bin\Release\*.snupkg .\..\..\.nuget\
cd .\..\..\.nuget\

cd ..\

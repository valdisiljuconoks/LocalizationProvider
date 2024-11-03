cd .\.nuget

cd .\..\src\DbLocalizationProvider.AspNetCore\
dotnet build -c Release
dotnet pack -c Release
dotnet pack --include-symbols -p:SymbolPackageFormat=snupkg
copy .\bin\Release\LocalizationProvider.AspNetCore.*.nupkg .\..\..\.nuget\
copy .\bin\Release\LocalizationProvider.AspNetCore.*.snupkg .\..\..\.nuget\
cd .\..\..\.nuget\

cd .\..\src\DbLocalizationProvider.AdminUI.AspNetCore\
dotnet build -c Release
dotnet pack -c Release
dotnet pack --include-symbols -p:SymbolPackageFormat=snupkg
copy .\bin\Release\LocalizationProvider.AdminUI.AspNetCore.*.nupkg .\..\..\.nuget\
copy .\bin\Release\LocalizationProvider.AdminUI.AspNetCore.*.snupkg .\..\..\.nuget\
cd .\..\..\.nuget\

cd .\..\src\DbLocalizationProvider.AdminUI.AspNetCore.Csv\
dotnet build -c Release
dotnet pack -c Release
dotnet pack --include-symbols -p:SymbolPackageFormat=snupkg
copy .\bin\Release\LocalizationProvider.AdminUI.AspNetCore.Csv.*.nupkg .\..\..\.nuget\
copy .\bin\Release\LocalizationProvider.AdminUI.AspNetCore.Csv.*.snupkg .\..\..\.nuget\
cd .\..\..\.nuget\

cd .\..\src\DbLocalizationProvider.AdminUI.AspNetCore.Xliff\
dotnet build -c Release
dotnet pack -c Release
dotnet pack --include-symbols -p:SymbolPackageFormat=snupkg
copy .\bin\Release\LocalizationProvider.AdminUI.AspNetCore.Xliff.*.nupkg .\..\..\.nuget\
copy .\bin\Release\LocalizationProvider.AdminUI.AspNetCore.Xliff.*.snupkg .\..\..\.nuget\
cd .\..\..\.nuget\

cd ..\

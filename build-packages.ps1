Add-Type -assembly "system.io.compression.filesystem"

cd .\.nuget

# common libraries (netfx)
cd .\..\src\DbLocalizationProvider.Abstractions\
dotnet pack -c Release
copy .\bin\Release\*.nupkg .\..\..\.nuget\
cd .\..\..\.nuget\

cd .\..\src\DbLocalizationProvider\
dotnet pack -c Release
copy .\bin\Release\*.nupkg .\..\..\.nuget\
cd .\..\..\.nuget\

#.\nuget.exe pack ..\src\DbLocalizationProvider.Abstractions\DbLocalizationProvider.Abstractions.csproj -Properties Configuration=Release -IncludeReferencedProjects
#.\nuget.exe pack ..\src\DbLocalizationProvider\DbLocalizationProvider.csproj -Properties Configuration=Release -IncludeReferencedProjects

C:\Program Files\dotnet\sdk\NuGetFallbackFolder\microsoft.build.runtime\15.3.409\contentFiles\any\net46\msbuild.exe ..\src\DbLocalizationProvider.AspNet\DbLocalizationProvider.AspNet.csproj -t:Pack -p:Configuration=Release -p:TargetFramework=net461

# .\nuget.exe pack ..\src\DbLocalizationProvider.AspNet\DbLocalizationProvider.AspNet.csproj -Properties Configuration=Release -IncludeReferencedProjects
.\nuget.exe pack ..\src\DbLocalizationProvider.AdminUI\DbLocalizationProvider.AdminUI.csproj -Properties Configuration=Release -IncludeReferencedProjects
.\nuget.exe pack ..\src\DbLocalizationProvider.MigrationTool\DbLocalizationProvider.MigrationTool.csproj -Properties Configuration=Release -tool
.\nuget.exe pack ..\src\DbLocalizationProvider.Xliff\DbLocalizationProvider.Xliff.csproj -Properties Configuration=Release -IncludeReferencedProjects


# episerver libraries

.\nuget.exe pack ..\src\DbLocalizationProvider.EPiServer\DbLocalizationProvider.EPiServer.csproj -Properties Configuration=Release -IncludeReferencedProjects

$moduleName = "DbLocalizationProvider.AdminUI.EPiServer"
$source = $PSScriptRoot + "\src\" + $moduleName + "\modules\_protected\" + $moduleName
$destination = $PSScriptRoot + "\src\" + $moduleName + "\" + $moduleName + ".zip"

If(Test-path $destination) {Remove-item $destination}
[io.compression.zipfile]::CreateFromDirectory($Source, $destination)

.\nuget.exe pack ..\src\DbLocalizationProvider.AdminUI.EPiServer\DbLocalizationProvider.AdminUI.EPiServer.csproj -Properties Configuration=Release -IncludeReferencedProjects
.\nuget.exe pack ..\src\DbLocalizationProvider.EPiServer.JsResourceHandler\DbLocalizationProvider.EPiServer.JsResourceHandler.csproj -Properties Configuration=Release -IncludeReferencedProjects
.\nuget.exe pack ..\src\DbLocalizationProvider.AdminUI.EPiServer.Xliff\DbLocalizationProvider.AdminUI.EPiServer.Xliff.csproj -Properties Configuration=Release -IncludeReferencedProjects
cd ..\

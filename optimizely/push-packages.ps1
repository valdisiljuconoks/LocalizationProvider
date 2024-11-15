cd .\.nuget

nuget push .\DbLocalizationProvider.EPiServer.8.0.0.nupkg -source https://nuget.episerver.com/feed/packages.svc
nuget push .\DbLocalizationProvider.EPiServer.JsResourceHandler.8.0.0.nupkg -source https://nuget.episerver.com/feed/packages.svc
nuget push .\DbLocalizationProvider.AdminUI.EPiServer.8.0.0.nupkg -source https://nuget.episerver.com/feed/packages.svc/
nuget push .\DbLocalizationProvider.AdminUI.EPiServer.Xliff.8.0.0.nupkg -source https://nuget.episerver.com/feed/packages.svc/
nuget push .\DbLocalizationProvider.MigrationTool.8.0.0.nupkg -source https://nuget.episerver.com/feed/packages.svc/

cd ..\

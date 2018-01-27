# Getting Started (Asp.Net Mvc)

Localization Provider consists from few components:

* `LocalizationProvider.Abstractions` - abstractions package giving you possibility to annotate resources or models without referencing core `LocalizationProvider` pacakge.
* `LocalizationProvider` - this is core package, model and resource synchronization process and other core features.
* `LocalizationProvider.AdminUI` - administrator user interface for editors and administrators to overview resources, make translations, import / export and do other management stuff.


### Installing Package

Installation nowadays can't be more simpler as just adding NuGet package(s).

```
PM> Install-Package LocalizationProvider
```

Execute this line to install administration user interface:

```
PM> Install-Package LocalizationProvider.AdminUI
```

## Setup & Configuration

### Setup Provider
To get started you need to add `DbLocalizationProvider` via `IAppBuilder` interface (usally in your `Startup.cs` class):

```csharp
public class Startup
{
    public void Configuration(IAppBuilder appBuilder)
    {
        appBuilder.UseDbLocalizationProvider(c =>
                                             {
                                                 c.ConnectionName = "MyConnectionString";
                                             });
    }
}
```

For list of available startup configuration options [go here](http://blog.tech-fellow.net/2016/04/21/db-localization-provider-part-2-configuration-and-extensions/#configuringdblocalizationprovider).

### Setup AdminUI
You will need to map administration UI to some Url. Could be done like this:

```csharp
public class Startup
{
    public void Configuration(IAppBuilder appBuilder)
    {
        appBuilder.Map("/localization-admin", app => app.UseDbLocalizationProviderAdminUI());
    }
}
```

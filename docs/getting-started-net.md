## What is LocalzationProvider project?

LocalizationProvider project is Asp.Net Mvc web application localization provider on steriods.

Giving you main following features:
* Database driven localization provider for Asp.Net Mvc applications projects
* Easy resource registrations via code
* Supports hierarchical resource organization (with help of child classes)
* Administration UI for editors to change or add new translations for required languages

## Getting Started

Localization Provider consists from few components:

* `LocalizationProvider.Abstractions` - abstractions package giving you possibility to annotate resources or models without referencing core `LocalizationProvider` pacakge.
* `LocalizationProvider` - this is core package, model and resource synchronization process and other core features.
* `LocalizationProvider.AdminUI` - administrator user interface for editors and administrators to overview resources, make translations, import / export and do other management stuff.


### Installing Provider

Installation nowadays can't be more simpler as just adding NuGet package(s).

```
PM> Install-Package LocalizationProvider
```

Execute this line to install administration user interface:

```
PM> Install-Package LocalizationProvider.AdminUI
```

## Setup & Configuration

To get started you need to add `DbLocalizationProvider` via `IAppBuilder` interface (usally in your `Startup.cs` class):

```
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


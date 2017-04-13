# Getting Started (EPiServer)

## What is LocalzationProvider for EPiServer?

LocalizationProvider project is an attempt to improve EPiServer built-in localization provider originally based on collection of Xml files.

Aside from Asp.Net Mvc features EPiServer integration components gives following additional features:
* Database driven localization provider for EPiServer projects
* Administration UI for editors to change or add new translations for required languages
* Possibility to add new localized translations while adding new language to the EPiServer site without involving the developer
* Easy migration tool from built-in default Xml files to your EPiServer database

## Getting Started

Localization Provider for EPiServer consists from few components:

* `DbLocalizationProvider.EPiServer` - core package for EPiServer integration giving you all necessary extension methods for working with resources or models in EPiServer environment.
* `DbLocalizationProvider.AdminUI.EPiServer` - administrator user interface for editors and administrators to overview resources, manage translations, import / export and do other tasks.
* `LocalizationProvider.MigrationTool` - tool to help migrate from Xml language files (default EPiServer localization approach) to database driven provider.
* `LocalizationProvider.JsResourceHandler` - this package brings localized resources down to client-side as Json object easily accessible in javascipt.


### Installing Provider

Installation nowadays can't be more simpler as just adding NuGet package(s).

```
PM> Install-Package DbLocalizationProvider.EPiServer
```

Execute this line to install administration user interface:

```
PM> Install-Package DbLocalizationProvider.AdminUI.EPiServer
```

### Config File Changes


**NB!** Currently `DbLocalizationProvider.EPiServer` package has naïve `web.config` transformation file assuming that `<episerver.framework>` section is not extracted into separate file (this was usual case for older versions of AlloyTech sample sites).

New localization provider needs to be "registered" and added to the list of the localization providers configured for EPiServer Framework. Section may look like this:

```xml
<episerver.framework>
  ..
  <localization>
    <providers>
      <add name="db"
           type="DbLocalizationProvider.EPiServer.DatabaseLocalizationProvider, DbLocalizationProvider.EPiServer" />
      <add name="languageFiles" virtualPath="~/lang"
           type="EPiServer.Framework.Localization.XmlResources.FileXmlLocalizationProvider, EPiServer.Framework" />
    </providers>
  </localization>
  ..
</episerver.framework>
```

**NB!** If you do have extracted `<episerver.framework>` section into separate file (usually `episerver.framework.config`), please clean-up web.config after NuGet package installation and move content of the `<localization>` section to that separate physical file.


## Setup & Configuration

To get started you need to configure localization provider configuration context. Best place - in EPiServer initialization module:

```
using DbLocalizationProvider;

[InitializableModule]
public class InitializationModule1 : IInitializableModule
{
    public void Initialize(InitializationEngine context)
    {
        ConfigurationContext.Setup(ctx => ... );
    }

    public void Uninitialize(InitializationEngine context) { }
}
```

For list of available startup configuration options [go here](http://blog.tech-fellow.net/2016/04/21/db-localization-provider-part-2-configuration-and-extensions/#configuringdblocalizationprovider).


## What is LocalzationProvider project?

LocalizationProvider project is an attempt to improve EPiServer built-in localization provider originally based on collection of Xml files.

Giving you following features:
* Database driven localization provider for EPiServer
* Administration UI for editors to change or add new translations for required languages
* Possibility to add new localized messages while adding new language to the EPiServer site without involving the developer
* Easy migration tool from built-in default Xml files to your EPiServer database

Localization Provider consists from few components:

* `DbLocalizationProvider` - this is core package and gives you EPiServer localization provider, `DataAnnotation` attributes, model and resource synchronization process and other core features.
* `DbLocalizationProvider.AdminUI` - administrator user interface for editors and administrators to overview resources, make translations, import / export and do other management stuff.
* `DbLocalizationProvider.MigrationTool` - tool gives you possibility to generate JSON format data out of Xml language files later to import these resources into this new DbLocalizationProvider.


### Installing Provider

Installation nowadays can't be more simpler as just adding NuGet package(s).

```
PM> Install-Package DbLocalizationProvider
PM> Install-Package DbLocalizationProvider.AdminUI
PM> Install-Package DbLocalizationProvider.MigrationTool
```

**NB!** Currently `DbLocalizationProvider` NuGet package has naïve `web.config` transformation file assuming that `<episerver.framework>` section is not extracted into separate file (this was usual case for older versions of AlloyTech sample sites).

New localization provider needs to be "registered" and added to the list of the localization providers configured for EPiServer Framework. Section may look like this:

```xml
<episerver.framework>
  ..
  <localization>
    <providers>
      <add name="db"
           type="DbLocalizationProvider.DatabaseLocalizationProvider, DbLocalizationProvider" />
      <add name="languageFiles" virtualPath="~/lang"
           type="EPiServer.Framework.Localization.XmlResources.FileXmlLocalizationProvider, EPiServer.Framework" />
    </providers>
  </localization>
  ..
</episerver.framework>
```

**NB!** If you do have extracted `<episerver.framework>` section into separate file (usually `episerver.framework.config`), please clean-up web.config after NuGet package installation and move content of the `<localization>` section to that separate physical file.

## DbLocalizationProvider Features

All resources in DbLocalizationProvider system are divided into 2 groups:

* **Resources** - localized resources are just list of key / value pairs. You may have a key for the resource and value is its translation in specific language. Resources are designed as just POCO objects.
* **Models** - models are usually view models that may have `DataAnnotation` attributes attached to them (like, `Display`, `Required`, etc).


### Localized Resources

Localized resource is straight forward way to define list of properties that are localizable. Localized resource is simple POCO class that defines list of properties:

```csharp
namespace MySampleProject {

    [LocalizedResource]
    public class MyResources
    {
        public static string SampleResource => "This is default value";
    }
}
```


Now, you may use one of the ways to output this resource to the end-users:

```
@using DbLocalizationProvider

<div>
    @Html.Translate(() => MyResources.SampleResource)
</div>
```


### Localized Models

Another more interesting way and usage is to define localizable view models.

Localizable view model means that `DbLocalizationProvider` library will search for `[LocalizedModel]` attributes and will discover all models and further discovery of the resources there.

Now you may define following view model:

```csharp
namespace MySampleProject {

    [LocalizedModel]
    public class MyViewModel
    {
        [Display(Name = "This is default value")]
        public string SampleProperty { get; set; }
    }
}
```

### Model Data Annotations

Usually view models are decorated with various `DataAnnotation` attributes to get model validation into the Asp.Net Mvc request processing pipeline. Which is very fine and `DbLocalizationProvider` aware of these attributes once scanning for localized models.

So if you add bit more attributes to initial view model:

```csharp
namespace MySampleProject {

    [LocalizedModel]
    public class MyViewModel
    {
        [Display(Name = "This is default value")]
        [Required]
        [StringLength(5)]
        public string SampleProperty { get; set; }
    }
}
```

Following resources will be discovered:

```
MySampleProject.MyViewModel.SampleProperty
MySampleProject.MyViewModel.SampleProperty-Required
MySampleProject.MyViewModel.SampleProperty-StringLength
```

Which gives you a possibility to translation precise error messages shown when particular property has invalid value for this model.


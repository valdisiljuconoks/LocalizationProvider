[![Build status](https://ci.appveyor.com/api/projects/status/vlbh4yux2ube1gsr?svg=true)](https://ci.appveyor.com/project/ValdisIljuconoks/localizationprovider)


## What is LocalzationProvider project?

LocalizationProvider project is an attempt to improve EPiServer built-in localization provider originally based on collection of Xml files.

Giving you following features:
* Database driven localization provider for EPiServer
* Administration UI for editors to change or add new translations for required languages
* Possibility to add new localized messages while adding new language to the EPiServer site without involving the developer
* Easy migration tool from built-in default Xml files to your EPiServer database

Localization Provider consists from few components:

* `LocalizationProvider` - this is core package and gives you EPiServer localization provider, `DataAnnotation` attributes, model and resource synchronization process and other core features.
* `LocalizationProvider.AdminUI` - administrator user interface for editors and administrators to overview resources, make translations, import / export and do other management stuff.

### Installing Provider

Installation nowadays can't be more simpler as just adding NuGet package(s).

```
PM> Install-Package LocalizationProvider
PM> Install-Package LocalizationProvider.AdminUI
```

## Getting Started

To get started bare minimum is - you need to add `DbLocalizationProvider` via `IAppBuilder` interface in your `Startup.cs` class:

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

## Breaking Changes in 2.0
### [Ignore] Attribute
You need to change reference from `EPiServer.DataAnnotations.IgnoreAttribute` to `DbLocalizationProvider.Sync.IgnoreAttribute`.

## EPiServer Integration
[Read more on wiki](https://github.com/valdisiljuconoks/LocalizationProvider/wiki/EPiServer-Integration)

## DbLocalizationProvider Features
Here goes the list of new features available in version 2.0. List is not sorted in any order.

Features:
* [Localized Resource](https://github.com/valdisiljuconoks/LocalizationProvider#localized-resources)
* [Localized Models](https://github.com/valdisiljuconoks/LocalizationProvider#localized-models)
* [Localized Model Metadata](https://github.com/valdisiljuconoks/LocalizationProvider#localized-model-data-annotations)
* [Translate System.Enum](https://github.com/valdisiljuconoks/LocalizationProvider#translate-systemenum)
* [Translation with Placeholders](https://github.com/valdisiljuconoks/LocalizationProvider#templates-with-placeholders)
* [Customer Resource Keys](https://github.com/valdisiljuconoks/LocalizationProvider#custom-resource-keys)
* [Support for Nullable Properties](https://github.com/valdisiljuconoks/LocalizationProvider#support-for-nullable-properties)
* [Support for [Display(Description = "...")]](https://github.com/valdisiljuconoks/LocalizationProvider#displaydescription--)
* [Mark Required Fields in Form](https://github.com/valdisiljuconoks/LocalizationProvider#mark-required-fields)
* [Manually Register Resources](https://github.com/valdisiljuconoks/LocalizationProvider#manually-register-resources)


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

### Localized Model Data Annotations

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

So you can easily continue using Html helper extensions like:


```
<div>
    @Html.LabelFor(m => m.SampleProperty)
    @Html.EditorFor(m => m.SampleProperty)
</div>
```

### Translate System.Enum
It's quite often that you do have a enumeration in the domain to ensure that your entity might have value only from predefined list of values - like `Document.Status` or `PurchaseOrder.Shipment.Status`. These values are usually defined as `System.Enum`. And it's also quite often case when you need to render a list of these available values on the page or anywhere else for the user of the application to choose from. Now in 2.0 enumeration translation is easily available.

```
[LocalizedResource]
public enum DocumentStatus
{
    None,
    New,
    Pending.
    Active,
    Closed
}

[LocalizedModel]
public class Document
{
    ...

    public DocumentStatus Status { get; }
}
```

Now you can use following snippet to give end-user localized dropdown list of available document statuses:

```
@using DbLocalizationProvider
@model Document

@{
    var statuses = Enum.GetValues(typeof(DocumentStatus))
                       .Cast<DocumentStatus>()
                       .Select(s => new SelectListItem
                                        {
                                            Value = s.ToString(),
                                            Text = s.Translate()
                                        });
}

@Html.DropDownListFor(m => m.Status, statuses)
```

Or if you just need to output current status of the document to the end-user:

```
@using DbLocalizationProvider
@model Document

@Model.Status.Translate()
```

### Templates with Placeholders
Index based `string.Format` style arguments for localized message is very nice and flexible approach. However - it's readable most probably only by developer or somebody who understands why first element starts with `{0}` and not `{1}`.
When you need to give access to the resources to editors or anybody else with even enough technical background, you might receive questions back - "What is `{0}` and what will be placed in `{4}`?" Pretty tricky question if you need to open source code and look for passed in format arguments.
Now in v2.0 you can pass in anonymous object with named properties and use those in your localized resource.

For example, greeting message for end-users (I really hate these kind of greetings.. so simple to invest in vocative case):

```
[LocalizedResource]
public class StartPageResources
{
    public static string GreetingMessage => "Hi {Firstname} {Lastname}, where would you like to click today?";
}


@Html.Translate(() => StartPageResources.GreetingMessage, new { Firstname = "John", Lastname = "Doe" })
```

Or you may have a view model as basis for some translated message, so you can pass in directly that model:

```
public class Document
{
    public string Nr { get; }
    public string Author { get; }
}

[LocalizedResource]
public class DocumentResources
{
    public static string SharedTo => "{Author}, somebody shared your '{Nr}' document!";
}
...

@model Document

@Html.Translate(() => StartPageResources.GreetingMessage, Model)
```

Now it should be much easier for the editors to understand what value goes in which placeholders.


### Custom Resource Keys

Back in time [Linus blogged](http://world.episerver.com/blogs/Linus-Ekstrom/Dates/2013/12/New-standardized-format-for-content-type-localizations/) about new standardized way to localize Cms content types and properties. As originally EPiServer is based on Xml and XPath resource key conventions - there was no support in DbLocalizationProvider to make it happen and supply these specially generated resource keys via strongly typed resources.

Now in v2.0 you can do that by specifying `ResourceKey` directly on property of the `PageType`:


```
...
using DbLocalizationProvider;

[ContentType(GUID = "....")]
[LocalizedModel(KeyPrefix = "/contenttypes/startpage/")]
[ResourceKey("name", Value = "This is StartPage!")]
public class StartPage : PageData
{
    [ResourceKey("properties/headertitle/caption", Value = "Title of the page")]
    public virtual string HeaderTitle { get; set; }
}
```

This class should register 2 resources with following keys (that will be picked up by EPiServer automatically):

* `/contenttypes/startpage/name`
* `/contenttypes/startpage/properties/headertitle/caption`

You may also need to specify descriptions or any other resources for that Cms content type property, you can have multiple `ResourceKey` attributes:

```
...
using DbLocalizationProvider;

[ContentType(GUID = "....")]
[LocalizedModel(KeyPrefix = "/contenttypes/startpage/")]
[ResourceKey("name", Value = "This is StartPage!")]
[ResourceKey("description", Value = "This is StartPage!")]
public class StartPage : PageData
{
    [ResourceKey("properties/headertitle/caption", Value = "Title of the page")]
    [ResourceKey("properties/headertitle/help", Value = "Enter some meaningful title of the page")]
    public virtual string HeaderTitle { get; set; }
}
```

### Support for Nullable properties
This is pretty small addition, but previously in 1.x version nullable properties where not supported.
Now following property will be discovered and added to localized resources:

```
[LocalizedModel]
public class Document
{
    public DateTime? DeletedWhen { get; }
}
```

Following data types are treated as simple and thus - added to list of resources to synchronize:

* `typeof(Enum)`,
* `typeof(string)`,
* `typeof(char)`,
* `typeof(Guid)`,
* `typeof(bool)`,
* `typeof(byte)`,
* `typeof(short)`,
* `typeof(int)`,
* `typeof(long)`,
* `typeof(float)`,
* `typeof(double)`,
* `typeof(decimal)`,
* `typeof(sbyte)`,
* `typeof(ushort)`,
* `typeof(uint)`,
* `typeof(ulong)`,
* `typeof(DateTime)`,
* `typeof(DateTimeOffset)`,
* `typeof(TimeSpan)`

And their `Nullable<>` counterpart.


### [Display(Description = "...")]
Also small addition to `ModelMetadataProvider` infrastructure available for Asp.Net Mvc pipeline. Now you can also localize description for the property via `DataAnnotations` attributes:

```
namespace MyProject
{
    public class MyViewModel
    {
        [Display(Name = "Login name", Description = "Login name for the user is email.")]
        public string Username { get; set; }
    }
}
```

Will generate following resource `MyProject.MyViewModel.Username-Description` *only* if `Description` property of `Display` attribute will not be `string.Empty`. You can localize it via AdminUI and set new value if needed.

When you need to use this value in your display or editor templates you can access it via `ViewData`:


```
<div>
    ...
    <span class="field-description">@ViewData.ModelMetadata.Description</span>
    ...
</div>
```

### Mark Required Fields

*NB!* This is experimental feature, so feedback is welcome.
Got a request from one of our projects to indicate all required fields in the system with some sort of prefix (e.g., asterix `"*"` or anything like that). We were considering to create some HtmlHelper extensions for this and revisit its usage across the pages. However, using new DbLocalizationProvider all calls for model metadata is going through `ModelMetadataProvider` infrastructure and there is single point of responsibility for providing value for code snippet like this `@Html.LabelFor(...)`.

So I decided to add this experimental feature to the localization provider to give single configuration option for the developers to enable this requirement. Might not be directly related to responsibility of localization provider, that's why it's still experimental and not sure whether a good idea to add it here. Anyhow, here is the way how to achieve this:


```
[LocalizedResources]
public class Common
{
    public static string RequiredIndicator => " *";
}

[InitializableModule]
[ModuleDependency(typeof(InitializationModule))]
public class SetupLocalization : IInitializableModule
{
    public void Initialize(InitializationEngine context)
    {
        ConfigurationContext.Setup(ctx =>
        {
            ctx.ModelMetadataProviders.MarkRequiredFields = true;
            ctx.ModelMetadataProviders.RequiredFieldResource =
                () => Common.RequiredIndicator;
        });
    }

    public void Uninitialize(InitializationEngine context) { }
}

public class MyViewModel
{
    [Required]
    public string Username { get; set; }
}
```

Now running in this context, if you type in:

```
@model MyViewModel

@Html.Label(m => m.Username)
```

With no modifications to default resource translations, it should output:

```
<label for="...">Username *</label>
```


### Manually Register Resources

If it's required - you can register resources manually (you are taking care about generated proper resource `key`).
Registration (culture is taken from `ConfigurationContext.Current.DefaultResourceCulture`):

```
var synchronizer = new ResourceSynchronizer();
synchronizer.RegisterManually(new[] { new ManualResource("This.Is.Sample.Key", translation) });
````

Retrieval:

```
LocalizationProvider.Current.GetString("This.Is.Sample.Key");
```

# More Info

* [Part 1: Resources and Models](http://blog.tech-fellow.net/2016/03/16/db-localization-provider-part-1-resources-and-models/)
* [Part 2: Configuration and Extensions](http://blog.tech-fellow.net/2016/04/21/db-localization-provider-part-2-configuration-and-extensions/)
* Part 3: Import and Export
* Part 4: Resource Refactoring and Migrations

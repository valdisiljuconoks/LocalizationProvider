# Resource Types

## Why Different Types?

You may ask why need to differentiate these types as they seems to be the same - subject for the localization? 
Main reason why these types need to be different is for the scanning and discovery process. 
Localized resources are just a bunch of strings that you target for localization (key value pair list).
However - models are more complex subject for localization as additional metadata needs to be taken into account.

## Localized Resources

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

Following resource key will be generated:

```
MySampleProject.MyResources.SampleResource
```

Now, you are able to [start working](working-with-resources-net.md) with resources.


### Nested Localized Resources

Localized resources are not limited to "single level" container properties. It means that you can design your resource hierarchy as you wish and what makes more sense for your project and editors.

So for instance you may have following structure for your resources:

```csharp
namespace MyProject
{
    [LocalizedResource]
    public class MyPageResources
    {
        public static string ThisIsErrorMessage => "This is default value from code";
        public static HeaderResources Header { get; }

        public class HeaderResources
        {
            public string HelloMessage => "Well, hello there!";
        }
    }
}

```

This approach avoids to "pollute" global namespace with types that have really narrow usages - only to group related properties under related parent resource. Scanning this kind of structure, following keys will be discovered:

```
MyProject.MyPageResources.ThisIsErrorMessage
MyProject.MyPageResources.Header.HelloMessage
```

Scanning process makes sure that resource keys for nested resources "follows" usage context.
**NB!** Note that there is no `static` keyword added to `HelloMessage` property in type `HeaderResources`.

For example, how you might use nested resource is following:

```csharp
@Html.Translate(() => MyPageResources.Header.HelloMessage)
```

Last part of the `LambdaExpression` (e.g. `.HelloMessage`) is accessible ("compilable") **only if** property `HelloMessage` for type `HeaderResources` is marked as "instance property" (no `static` property).

If you define `static` property accessor for `HelloMessage` like this:

```csharp
namespace MyProject
{
    [LocalizedResource]
    public class MyPageResources
    {
        public static HeaderResources Header { get; }

        public class HeaderResources
        {
            public static string HelloMessage => "Well, hello there!";
        }
    }
}
```

Then usage ("path" to property) is different:

```csharp
@Html.Translate(() => MyPageResources.HeaderResources.HelloMessage)
```

Which makes it difficult to find correct path to corresponding resources.
So library tries to follow usage of the resource from code perspective and makes it easier for editor and developer to understand the context and usage of the localized resource.



## Resource Keys
Usually developers don't need to know resource key, but anyway - by default resource key is equal to type FQN (Fully Qualified Name) = `{namespace}.{type-name}.{property-name}`.


### Custom Resource Keys

Back in time [Linus blogged](http://world.episerver.com/blogs/Linus-Ekstrom/Dates/2013/12/New-standardized-format-for-content-type-localizations/) about new standardized way to localize Cms content types and properties. As originally EPiServer is based on Xml and XPath resource key conventions - there was no support in DbLocalizationProvider to make it happen and supply these specially generated resource keys via strongly typed resources.

Now you can do that by specifying `[ResourceKey]` directly on property of the `PageType`:


```csharp
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

Note, that you can also specify `[LocalizedModel(KeyPrefix = "...")]` as shortcut for skipping beginning of all resource keys associated with each property. This class will register two resources keys:

* `/contenttypes/startpage/name`
* `/contenttypes/startpage/properties/headertitle/caption`

You may also need to specify descriptions or any other resources for that Cms content type property, you can have multiple `[ResourceKey]` attributes:

```csharp
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

## Class Property or Field?

Should I implement my localized resources as class' properties or as class fields?

Properties:

```csharp
namespace MySampleProject {

    [LocalizedResource]
    public class MyResources
    {
        public static string SampleResource => "This is default value";

        public static string AnotherSample { get; set; };
    }
}
```

Field:

```csharp
namespace MySampleProject {

    [LocalizedResource]
    public class MyResources
    {
        public static string SampleResource = "This is default value";
    }
}
```

Both notations are supported - so you can define resources as class' fields or properties. It's up to you!

## Localized View Models
Another more interesting way and usage is to define localizable view models.
Localizable view model means that `DbLocalizationProvider` library will search for `[LocalizedModel]` attributes and will discover all models and further discovery of the resources from this model.

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

Now, you are able to [start working](working-with-resources-net.md) with localized models.


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

You can also specify default translation for error message within attribute - `[Required(ErrorMessage = "Sample property is required!")]`.

Both `[Display(Name = "...")]` and `[DisplayName]` attributes are supported.

So you can easily continue using Html helper extensions like:


```
<div>
    @Html.LabelFor(m => m.SampleProperty)
    @Html.EditorFor(m => m.SampleProperty)
</div>
```

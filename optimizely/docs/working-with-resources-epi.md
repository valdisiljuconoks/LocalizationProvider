# Working with Resources (Optimizely)

Following resource used in samples:

```csharp
namespace MySampleProject {

    [LocalizedResource]
    public class MyResources
    {
        public static string SampleResource => "This is default value";
    }
}
```

## Translating in Markup (.cshtml)

Now, you may use one of the ways to output this resource to the end-users:

```
@using DbLocalizationProvider

<div>
    @Html.Translate(() => MySampleProject.MyResources.SampleResource)
</div>
```

Retrieve translation by specific culture ("Norsk" in this case):

```
@using DbLocalizationProvider

<div>
    @Html.TranslateByCulture(() => MySampleProject.MyResources.SampleResource,
                             CultureInfo.GetCultureInfo("no"))
</div>
```


## Translating in C#

It's also possible to retrieve translation in C# if needed (for example when localizing messages in services).

```csharp
using DbLocalizationProvider;
using EPiServer.Framework.Localization;

...

private Injected<LocalizationService> LocalizationService { get; set; }

...

var t = LocalizationService.Service.GetString(() => MySampleProject.MyResources.SampleResource);
```

Retrieve translation by specific culture ("Norsk" in this case):

```csharp
using DbLocalizationProvider;
using EPiServer.Framework.Localization;

...

private Injected<LocalizationService> LocalizationService { get; set; }

...

var t2 = LocalizationService.Service.GetStringByCulture(
    () => MySampleProject.MyResources.SampleResource,
    CultureInfo.GetCultureInfo("no"));
```

## System.Enum as Optimizely Content Property

If you want to have content type property of `System.Enum` and at the same time have it localized, then you should use `LocalizedEnumSelectionFactory<T>` as selection factory for `[SelectOne]` or `[SelectMany]` attribute:

```csharp
[ContentType(...
[LocalizedModel(OnlyIncluded = true)]
public class StartPage : PageData
{
    ...

    [SelectOne(SelectionFactoryType = typeof(LocalizedEnumSelectionFactory<SomeValuesEnum>))]
    public virtual SomeValuesEnum SomeValue { get; set; }
}
```

Or you can use short-cut attribute (of course `[LocalizedSelectMany]` is also supported):

```csharp
[ContentType(...
[LocalizedModel(OnlyIncluded = true)]
public class StartPage : PageData
{
    ...

    [LocalizedSelectOneEnum(typeof(SomeValuesEnum))]
    public virtual SomeValuesEnum SomeValue { get; set; }
}
```

Enum type itself should have expected attributes when working with [basic Enum translations](https://github.com/valdisiljuconoks/LocalizationProvider/blob/master/docs/translate-enum-net.md):

```csharp
[LocalizedResource]
public enum SomeValuesEnum
{
    [Display(Name = "NOONE!")]
    None = 0,

    [Display(Name = "1st value")]
    FirstValue = 1,

    [Display(Name = "This is second")]
    SecondValue = 2,

    [Display(Name = "And here comes last (3rd)")]
    ThirdOne = 3
}
```

`[LocalizedEnum]` attribute will make sure to tell Optimizely to setup things accordingly and assign specific localized selection factory type to provide correctly translated enum values.

## Translating Optimizely Categories

If you need to translate categories in Optimizely you need to decorate category definition with `[LocalizedCategory]`.

For example:

```csharp
[LocalizedCategory]
public class SampleCategory : Category
{
    public SampleCategory()
    {
        Name = "This is sample cat. from code";
    }
}
```

`Name` value will be assigned to invariant culture by default.

Resource key assigned to the localized category is `/categories/category[@name=\"{CategoryName}\"]/description`.

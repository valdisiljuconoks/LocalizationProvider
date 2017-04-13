# Working with Resources (EPiServer)

By default translations in `CultureInfo.CurrentUICulture` will be returned.

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

var t = LocalizationService.Current.GetString(() => MySampleProject.MyResources.SampleResource);
```

Retrieve translation by specific culture ("Norsk" in this case):

```csharp
using DbLocalizationProvider;
using EPiServer.Framework.Localization;

...

var t2 = LocalizationService.Current.GetStringByCulture(
    () => MySampleProject.MyResources.SampleResource,
    CultureInfo.GetCultureInfo("no"));
```


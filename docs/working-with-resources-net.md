# Working with Resources

## Registering Resources

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

## Registering for Other Languages


If you need to register resources for other languages as well (not only for default one), it's possible using following attribute:

```csharp
namespace DbLocalizationProvider.Demo
{
    [LocalizedResource]
    public class CommonResources
    {
        [TranslationForCulture("Navn", "no")]
        [TranslationForCulture("Namn", "sv")]
        public static string UserName => "Name";
    }
}
```

**NB!** If there will be duplicate resource translations for the same language - an exception will be thrown.
Which also means - that theoretically an exception might be thrown if your default language is let's say "en" and you have additional translations via attribute also set to "en". So be careful.


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


### Accessing [Display(Description = "...")]
Also small addition to `ModelMetadataProvider` infrastructure available for Asp.Net Mvc pipeline. Now you can also localize description for the property via `DataAnnotations` attributes:

```csharp
namespace MyProject
{
    public class MyViewModel
    {
        [Display(Name = "Login name", Description = "Login name for the user is email.")]
        public string Username { get; set; }
    }
}
```

Will generate following resource `MyProject.MyViewModel.Username-Description` *only* if `Description` property of `Display` attribute will not be `string.Empty()`. You can localize it via AdminUI and set new value if needed.

When you need to use this value in your display or editor templates you can access it via `ViewData`:


```
<div>
    ...
    <span class="field-description">@ViewData.ModelMetadata.Description</span>
    ...
</div>
```


## Translating in C#

It's also possible to retrieve translation in C# if needed (for example when localizing messages in services).

```csharp
var t = LocalizationProvider.Current.GetString(() => MySampleProject.MyResources.SampleResource);
```

Retrieve translation by specific culture ("Norsk" in this case):

```csharp
using DbLocalizationProvider;

...

var t2 = LocalizationProvider.Current.GetStringByCulture(
    () => MySampleProject.MyResources.SampleResource,
    CultureInfo.GetCultureInfo("no"));
```


## Translations with Placeholders

Index based `string.Format()` style arguments for localized message is very nice and flexible approach. However - it's readable most probably only by developer or somebody who understands why first element starts with `{0}` and not `{1}`.
When you need to give access to the resources to editors or anybody else with even enough technical background, you might receive questions back - "What is `{0}` and what will be placed in `{4}`?" Pretty tricky question if you need to open source code and look for passed in format arguments.
Now you can pass in anonymous object with named properties and use those in your localization.

For example, greeting message for end-users:

```csharp
[LocalizedResource]
public class StartPageResources
{
    public static string GreetingMessage => "Hi {Firstname} {Lastname}, where would you like to click today?";
}


@Html.Translate(() => StartPageResources.GreetingMessage, new { Firstname = "John", Lastname = "Doe" })
```

Or you may have a view model as basis for some translated message, so you can pass in directly that model:

```csharp
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

**NB!** If you misspell placeholder - it will be left untranslated (substitution will not be done).
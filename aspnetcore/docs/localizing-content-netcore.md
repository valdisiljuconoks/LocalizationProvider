## App Content Localization
Localizing application content via `IStringLocalizer<T>` is similar as that would be done for regular Asp.Net applications.

You have to define resource container type:

```csharp
[LocalizedResource]
public class SampleResources
{
    public string PageHeader => "This is page header";
}
```

Then you can demand `IStringLocalizer<T>` is any place you need that one (f.ex. in controller):

```csharp
public class HomeController : Controller
{
    private readonly IStringLocalizer<SampleResources> _localizer;

    public HomeController(IStringLocalizer<SampleResources> localizer)
    {
        _localizer = localizer;
    }

    public IActionResult Index()
    {
        var smth = _localizer.GetString(r => r.PageHeader);
        return View();
    }
}
```

As you can see - you are able to use nice strongly-typed access to the resource type: `_localizer.GetString(r => r.PageHeader);`.

Even if you demanded strongly-typed localizer with specified container type `T`, it's possible to use also general/shared static resources:

```csharp
[LocalizedResource]
public class SampleResources
{
    public static string SomeCommonText => "Hello World!";
    public string PageHeader => "This is page header";
}

public class HomeController : Controller
{
    private readonly IStringLocalizer<SampleResources> _localizer;

    public HomeController(IStringLocalizer<SampleResources> localizer)
    {
        _localizer = localizer;
    }

    public IActionResult Index()
    {
        var smth = _localizer.GetString(() => SampleResources.SomeCommonText);
        return View();
    }
}
```

## View Localization

Regarding the views, story here is exactly the same - all built-in approach is supported:

```
@model UserViewModel
@inject IViewLocalizer Localizer
@inject IHtmlLocalizer<SampleResources> HtmlLocalizer

@Localizer.GetString(() => SampleResources.SomeCommonText)
@HtmlLocalizer.GetString(r => r.PageHeader)
```

You can also depend directly on `LocalizationProvider`.

```
@inject LocalizationProvider Provider

..

@Provider.GetString(() => ...)
```


### Data Annotations
Supported. Sample:

```csharp
[LocalizedModel]
public class UserViewModel
{
    [Display(Name = "User name:")]
    [Required(ErrorMessage = "Name of the user is required!")]
    public string UserName { get; set; }

    [Display(Name = "Password:")]
    [Required(ErrorMessage = "Password is kinda required :)")]
    public string Password { get; set; }
}
```

View.cshtml:

```html
@model UserViewModel

<form asp-controller="Home" asp-action="Index" method="post">
    <div>
        <label asp-for="UserName"></label>
        <input asp-for="UserName"/>
        <span asp-validation-for="UserName"></span>
    </div>
    <div>
        <label asp-for="Password"></label>
        <input asp-for="Password" type="password"/>
        <span asp-validation-for="Password"></span>
    </div>
    ...
</form>
```

### Property [DisplayName] Description

Sometimes you need to get description of the property (like for the cases when you need to provide some descriptive label or help text for the property). This is now supported with following code:

```csharp
[LocalizedModel]
public class UserViewModel
{
    [Display(Name = "User name:",
             Description = "This is help text for the user name property")]
    public string UserName { get; set; }
}
```

View.cshtml:

```
@Html.DescriptionFor(m => m.UserName)
```

### HtmlHelper

You can also use `HtmlHelper` extension methods.

For view models:

```
@Html.TranslateFor(m => m.UserName)
```

Just translating a resource:

```
@Html.Translate(() => SampleResource.SomeStaticProperty)
```

## Localization in Libraries

You can either rely on `IStringLocalizer` implementation that's coming from `Microsoft.Extensions.Localization` namespace and demand that one in your injections:

```csharp
using Microsoft.Extensions.Localization;

public class MyService
{
    public MyService(IStringLocalizer localizer)
    {
       ...
    }
}
```

Or you can also depend on `LocalizationProvider` class defined in `DbLocalizationProvider` namespace:


```csharp
using DbLocalizationProvider;

public class MyService
{
    public MyService(LocalizationProvider provider)
    {
       ...
    }
}
```

Both of these types provide similar functionality in terms how to retrieve localized content.

## Changing Culture
Sometimes you need to get translation for other language and not primary UI one.
This is possible either via built-in method:

```csharp
@inject IHtmlLocalizer<SampleResources> Localizer

Localizer.WithCulture(new CultureInfo("no"))
         .GetString(() => SampleResources.SomeCommonText)
```

Or via additional extension method:

```csharp
@inject IHtmlLocalizer<SampleResources> Localizer

Localizer.GetStringByCulture(() => SampleResources.SomeCommonText, new Culture("no"))
```

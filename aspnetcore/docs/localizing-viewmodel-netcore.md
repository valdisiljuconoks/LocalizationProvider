# View Model Localization

Sometimes it's also beneficial to provide proper error messages for the user while validating model on the client-side.
Localization provider can help here as well.

## Preparing ViewModel
To add support for viewmodel localization - there isn't really anything special. Just add `[LocalizedModel]` attribute to your viewmodel.

```csharp
[LocalizedModel]
public class UserViewModel
{
    [Display(Name = "User name:", Description = "This is description of UserName field")]
    [Required(ErrorMessage = "Name of the user is required!")]
    public string UserName { get; set; }

    [Display(Name = "Password:")]
    [Required(ErrorMessage = "Password is kinda required :)")]
    [StringLength(15, MinimumLength = 5, ErrorMessage = "Please use longer password than 5 symbols!!")]
    public string Password { get; set; }
}
```

Markup for the page is the same standard way using [tag helpers](https://docs.microsoft.com/en-us/aspnet/core/mvc/views/tag-helpers/built-in/).

```cshtml
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
    <button type="submit">Submit</button>
</form>
```

Error message for invalid property will be taken from localization provider in [currently set](localizing-content-netcore.md#changing-culture) language.

### Where Is Translation?
When you need to change translation for validation message, you should look for `{ViewModel-Type}.{Property}-{DataAnnotationAttribute}` resource key.
For example, username's `Required` validation error message is located under resource with key `DbLocalizationProvider.Core.AspNetSample.Models.UserViewModel.UserName-Required`.

## Gotchas
### My ViewModel's Property Is Not Localized!
After some inspection of Asp.Net Core built-in validation attribute adapters it's obvious that custom `IStringLocalizer` passed to `IValidationAttributeAdapterProvider.GetAttributeAdapter()` will be used only when data annotation attribute will have `ErrorMessage` property set.

For example, if you will have following model:

```csharp
[LocalizedModel]
public class UserViewModel
{
    [Required]
    public string UserName { get; set; }
}
```

Error message for the `UserName` property will be not translated by LocalizedProvider.

You must set error message for the `Required` attribute:

```csharp
[LocalizedModel]
public class UserViewModel
{
    [Required(ErrorMessage = "...")]
    public string UserName { get; set; }
}
```

# Mark Required Fields

*NB!* This is experimental feature, so feedback is welcome.
Got a request from one of our projects to indicate all required fields in the system with some sort of prefix (e.g., asterix `"*"` or anything like that). We were considering to create some HtmlHelper extensions for this and revisit its usage across the pages. However, using new DbLocalizationProvider all calls for model metadata is going through `ModelMetadataProvider` infrastructure and there is single point of responsibility for providing value for code snippet like this `@Html.LabelFor(...)`.

So I decided to add this experimental feature to the localization provider to give single configuration option for the developers to enable this requirement. Might not be directly related to responsibility of localization provider, that's why it's still experimental and not sure whether a good idea to add it here. Anyhow, here is the way how to achieve this:


```csharp
[LocalizedResources]
public class Common
{
    public static string RequiredIndicator => "&nbsp;*";
}

public class Startup
{
    public void Configuration(IAppBuilder appBuilder)
    {
        appBuilder.UseDbLocalizationProvider(c =>
        {
            ctx.ModelMetadataProviders.MarkRequiredFields = true;
            ctx.ModelMetadataProviders.RequiredFieldResource =
                () => Common.RequiredIndicator;
        });
    }
}

public class MyViewModel
{
    [Required]
    public string Username { get; set; }
}
```

Now running the application in this case, if you type in:

```
@model MyViewModel

@Html.Label(m => m.Username)
```

With no modifications to default resource translations, it should output:

```
<label for="...">Username&nbsp;*</label>
```

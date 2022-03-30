# Working with Languages

## Fallback Languages

LocalizationProvider gives you option to configure fallback languages for translation lookup.
It means that provider will try to get translation in requested language. And if it does not exist in that language, fallback language list is used to decide which language to try next until either succeeds or fails with no translation found.

To configure fallback languages use code below:

```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDbLocalizationProvider(_ =>
        {
            ...
            // try "sv" -> "no" -> "en"
            _.FallbackCultures
                .Try(new CultureInfo("sv"))
                .Then(new CultureInfo("no"))
                .Then(new CultureInfo("en"));

            _.EnableInvariantCultureFallback = true;
        });
    }
}
```

This means that following logic will be used during translation lookup:

1) Developer requests translation in Swedish culture (`"sv"`) using `ILocalizationProvider.GetString(() => ...)` method.
2) If translation does not exist -> provider is looking for translation in Norwegian language (`"no"` - second language in the fallback list).
3) If translation is found - one is returned; if not - provider continues process and is looking for translation in English (`"en"`).
4) If there is no translation in English -> depending on `ConfigurationContext.EnableInvariantCultureFallback` setting -> translation in InvariantCulture may be returned.

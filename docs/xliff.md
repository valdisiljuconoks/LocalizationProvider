# XLIFF Support

If you are working with some translating agencies, then you know their things are all in XLIFF.

Fortunately DbLocalizationProvider supports this format as well for both export and import.

All you need to do is install the package.

```powershell
dotnet add package LocalizationProvider.Xliff
```

And add this to configuration setup code:

```csharp
services
    .AddDbLocalizationProviderAdminUI(ctx =>
    {
        ...
    })
    .AddCsvSupport()
    .AddXliffSupport();
```

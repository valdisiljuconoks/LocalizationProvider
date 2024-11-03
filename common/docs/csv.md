# CSV Support

If you have JSON or XLIFF and want to get translations as raw as possible - you can work (export/import) in CSV format.

All you need to do is install the package.

```powershell
dotnet add package LocalizationProvider.Csv
```

And add this to configuration setup code:

```csharp
services
    .AddDbLocalizationProviderAdminUI(ctx =>
    {
        ...
    })
    .AddCsvSupport()
    .AddCsvSupport();
```

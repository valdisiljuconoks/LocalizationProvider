# Getting Started (ASP.NET Core)

## Install Package

```
> dotnet add package LocalizationProvider.AspNetCore
```

## Configuring Services

In your `Startup.cs` class you need to add stuff related to Mvc localization (to get required services into DI container - service collection).

And then `services.AddDbLocalizationProvider()`. You can pass in configuration settings class (parameter name `cfg`) and setup provider's behavior.

```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // add your authorization provider (asp.net identity, identity server, which ever..)
    
        services
            .AddControllersWithViews()
            .AddMvcLocalization();
    
        services.AddRouting();
    
        services.AddDbLocalizationProvider(cfg =>
        {
            cfg.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            ...
        });
    }
}
```

Following configuration options are available:

| Option | Description |
|------|------|
| `AssemblyScanningFilter` | You can specify what assemblies needs to be scanned. By default some of the system ones are excluded (no need to waste time) |
| `CacheManager` | Gets or sets cache manager used to store resources and translations (`InMemory` by default) |
| `CustomAttributes` | Gets or sets list of custom attributes that should be discovered and registered during startup scanning. |
| `DefaultResourceCulture` | Gets or sets the default resource culture to register translations for newly discovered resources. |
| `DiagnosticsEnabled` | Gets or sets value enabling or disabling diagnostics for localization provider (e.g. missing keys will be written to log file). |
| `DiscoverAndRegisterResources` | Gets or sets the flag to control localized models discovery and registration during app startup. |
| `EnableInvariantCultureFallback` | Gets or sets flag to enable or disable invariant culture fallback (to use resource values discovered & registered from code). |
| `EnableLegacyMode` | Gets or sets the callback function to check if legacy mode is enabled. Legacy mode is special treatment of old XML file based resources (like resource key `/component/properties/property1`). If legacy mode is enabled and XML based resource is requested, additional lookup will be performed to resolve translation for these resources. |
| `EnableLocalization` | Gets or sets the callback function for enabling or disabling localization. If this returns `false` - requested resource key will be returned as translation. |
| `Export` | Gets or sets settings used for export of the resources. |
| `FallbackCultures` | Using this list you can configure language fallback settings. |
| `ForeignResources` | Gets or sets collection of foreign resources. Foreign resource descriptors are used to include classes without `[LocalizedResource]` or `[LocalizedModel]` attributes. |
| `Import` | Gets or sets settings to be used during resource import. |
| `Logger` | Gets or sets abstract logger which can bridge log entries down to underlying host/runtime logging infrastructure. |
| `ModelMetadataProviders` | Settings for model metadata providers. |
| `PopulateCacheOnStartup` | Gets or sets a value indicating whether cache should be populated during startup (default = `true`). |
| `ResourceKeyNameFilter` | If you are looking for a way to allow other characters in resource key name - this is the property to set. |
| `ResourceLookupFilter` | Callback function to check whether resource lookup should be performed (based on resource key). Use this with precaution or avoid usage at all. |
| `ScanAllAssemblies` | Forces type scanner to load all referenced assemblies. When enabled, scanner is not relying on current `AppDomain.GetAssemblies` but checks referenced assemblies recursively (default `false`). |
| `TypeFactory` | Returns type factory used internally for creating new services or handlers for commands. |
| `TypeScanners` | Gets list of all known type scanners. |

Following `ImportSettings` configuration options are available:

| Option | Description |
|------|------|
| `Providers` | Gets or sets list of known import handlers. Handlers are based on file format (handles specific file extension). |

Following `ExportSettings` configuration options are available:

| Option | Description |
|------|------|
| `Providers` | Gets or sets list of known export handlers. Handlers are based on file format/extension. |

Following `ModelMetadataProviders` configuration options are available:

| Option | Description |
|------|------|
| `MarkRequiredFields` | Set `true` to add translation returned from `RequiredFieldResource` for required fields. |
| `ReplaceProviders` | Gets or sets a value to replace ModelMetadataProvider to use new db localization system. |
| `RequiredFieldResource` | If `MarkRequiredFields` is set to `true`, return of this method will be used to indicate required fields (added at the end of label). |
| `SetupCallback` | If callback action is supplied it's invoked instead of default  model medata data providers setup. This is required in cases when model metadata provider infrastructure is different between runtimes. |
| `UseCachedProviders` | Gets or sets a value to use cached version of ModelMetadataProvider. |

### Post Configuration

It is also possible to perform post configuration (after you have called `AddDbLocalizationProvider()`) of the localization provider.
This is useful when you are unit testing your web app, after Startup code is executed and you want to make sure that some post configuration settings are applied for your unit tests to execute correctly.

**NB!** Please note, that it is *not* possible to configure any types, scanners or anything else that is added to the DI container during `AddDbLocalizationProvider()` call. This limitation is due to fact that types are added to DI container and post configuration is called afterwards (when `IServiceProvider` is already built).

To post configure localization provider you have to follow standard .NET Options pattern:

```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        ...

        services.AddDbLocalizationProvider(cfg =>
        {
            ...
        });

        // post configuring provider
        services.Configure<ConfigurationContext>(ctx =>
        {
            ctx.EnableInvariantCultureFallback = false;
        });
    }
}
```

### Adding Services to the App
After then you will need to make sure that you start using the provider:

```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        ...
    }

    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
        ...

        app.UseDbLocalizationProvider();
    }
}
```

Using localization provider will make sure that resources are discovered and registered in the database (if this process will not be disabled via `AddDbLocalizationProvider()` method by setting `ConfigurationContext.DiscoverAndRegisterResources` to `false`).

### Configure Fallback Languages
LocalizationProvider gives you option to configure fallback languages for the library.
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
            _.FallbackCultures
                .Try(new CultureInfo("sv"))
                .Then(new CultureInfo("no"))
                .Then(new CultureInfo("en"));
        });
    }
}
```

This means that following logic will be used during translation lookup:

1) Developer requests translation in Swedish culture (`"sv"`) using `ILocalizationProvider.GetString(() => ...)` method.
2) If translation does not exist -> provider is looking for translation in Norwegian language (`"no"` - second language in the fallback list).
3) If translation is found - one is returned; if not - provider continues process and is looking for translation in English (`"en"`).
4) If there is no translation in English -> depending on `ConfigurationContext.EnableInvariantCultureFallback` setting -> translation in InvariantCulture may be returned.

## Working with [LocalizedResource] & [LocalizedModel] Attributes

For more information on how localized resources and localized models are working - please read [docs in main package repo](https://github.com/valdisiljuconoks/LocalizationProvider/blob/master/docs/resource-types.md).

## Adding Additional Cultures

Localization is all about translations into multiple languages. So it's often required to add more supported languages to the application. LocalizationProvider uses `RequestLocalizationOptions` to understand what languages application is supporting. You can configure this setting using `ConfigureServices` startup method.

```csharp
public void ConfigureServices(IServiceCollection services)
{
    ...
    // just adding English and Latvian support
    services.Configure<RequestLocalizationOptions>(opts =>
    {
        var supportedCultures = new List<CultureInfo>
                                {
                                    new CultureInfo("en"),
                                    new CultureInfo("lv")
                                };

        opts.DefaultRequestCulture = new RequestCulture("en");
        opts.SupportedCultures = supportedCultures;
        opts.SupportedUICultures = supportedCultures;
    });
}
```

## Add AdminUI

For adding AdminUI to your application - refer to instructions [here](getting-started-adminui.md).

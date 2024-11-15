# Getting Started (Optimizely)

## What is LocalzationProvider for Optimizely?

LocalizationProvider project is an attempt to improve Optimizely built-in localization provider originally based on collection of Xml files.

Aside from features for .NET apps Optimizely integration gives following additional features:
* Database driven localization provider for Optimizely projects
* Administration UI for editors to change or add new translations for required languages
* Possibility to add new localized translations while adding new language to the Optimizely site without involving the developer
* Easy migration tool from built-in default Xml files to your Optimizely database

## Getting Started

Localization Provider for Optimizely consists from few components:

* `DbLocalizationProvider.EPiServer` - core package for Optimizely integration giving you all necessary extension methods for working with resources or models in Optimizely environment.
* `DbLocalizationProvider.AdminUI.EPiServer` - administrator user interface for editors and administrators to overview resources, manage translations, import / export and do other tasks.
* `LocalizationProvider.MigrationTool` - tool to help migrate from Xml language files (default Optimizely localization approach) to database driven provider.


### Installing Localization Provider

Installation nowadays can't be more simpler as just adding NuGet package(s). Add Optimizely integration package trogether with AdminUI:

```powershell
> dotnet add package DbLocalizationProvider.EPiServer
> dotnet add package DbLocalizationProvider.AdminUI.Optimizely
```

And also you might need to install SQL Server storage implementation (if you are using default Optimizely database to store resources):

```powershell
> dotnet add package LocalizationProvider.Storage.SqlServer
```

## Setup & Configuration

### Adding Provider to the Application
After installing packages you need to configure few things.

Adding it to the service collection:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services
        .AddDbLocalizationProvider(ctx =>
        {
            // now you can set different options via configuration context (`ctx`)
        })
        .AddOptimizely();  // add Optimizely integration
}
```

Use provider in the application:

```csharp
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    app.UseDbLocalizationProvider();
}
```

### Adding Provider AdminUI to the Application

If you need AdminUI this requires additional setup.
Adding it to the service collection:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services
        .AddDbLocalizationProviderAdminUI(ctx =>
        {
            // now you can set different options via configuration context (`ctx`)
        })
        .AddOptimizelyAdminUI()  // add Optimizely integration (adds menu items and stuff)
        .AddCsvSupport()         // you can also add additional export formats if needed
        .AddXliffSupport();
}
```

Use localization provider in the application (make sure you call also `UseStaticFiles()`):

```csharp
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    app.UseStaticFiles();

    app.UseAuthentication();
    app.UseAuthorization();

    app.UseDbLocalizationProvider();
    app.UseDbLocalizationProviderAdminUI();

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapRazorPages();
        endpoints.MapDbLocalizationAdminUI();
    });
}
```

### Configure to use SQL Server
In order to get localization provider to work properly - you might need to point to Optimizely database (where resources will be stored).

This is done by configuring provider to use SQL Server storage implementation:

```csharp
private readonly IConfiguration _configuration;

public Startup(IConfiguration configuration)
{
    _configuration = configuration;
}

public void ConfigureServices(IServiceCollection services)
{
    services
        .AddDbLocalizationProvider(ctx =>
        {
            ctx.UseSqlServer(_configuration.GetConnectionString("EPiServerDB"));
        })
        .AddOptimizely();
}
```

## Other Configuration Options

For list of available startup configuration options [visit this page](https://github.com/valdisiljuconoks/localization-provider-core/blob/master/docs/getting-started-netcore.md#configure-services).

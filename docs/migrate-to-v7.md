![](v7-1.png)

# Many Thanks!

As you can see v7.0 came quite heavily and took a long time before it saw daylight. It wouldn't be possible without my supporters and QA volunteers. Many thanks to everyone who was involved in this release and for making sure that we hammer out as many bugs as we could discover. I know that there is more to discover around the codebase well hidden in the dark corners of the inherited legacy heritage.

In this blog post I wanted to highlight some of the most obvious and visible changes to the library. As we jump to the next major version - this was good timing for me to finally materialize some of the pending break changes I was holding back for a very long time.

# General Changes (Some Quite Hard Breaking Ones)

There are some general changes that are worth mentioning:

* Everything is re-targeted to **.NET 5.0**.
* ASP.NET (old .NET Framework Runtime) package has been **dropped**. It's still around on the NuGet feed but is not actively developed anymore.
* **No more static properties** - all stack has been refactored to use DI (finally!).
* New types showed up `ICommandExecutor` and `IQueryExecutor`. You can now access resources in a programmatic way if you need to. Just ask your favorite DI container for these dependencies.

## Namespace Changes

The following types have changed their living space:

* `[LocalizedModelAttribute]` moved to `DbLocalizationProvider.Abstractions` namespace
* `[LocalizedResourceAttribute]` moved to `DbLocalizationProvider.Abstractions` namespace
* `[ResourceKeyAttribute]` moved to `DbLocalizationProvider.Abstractions` namespace
* Moved `Translate(this Enum target, ...)` to `ILocalizationProvider.Translate(this Enum target, ...)`

## List of Packages

Below you can find the list of packages that now library or maybe we should call it platform ;) contains:

| Package | Description |
|---------|-------------|
| `LocalizationProvider.Abstractions` | Contains all the high-level abstractions required for the libraries. |
| `LocalizationProvider` | Main core package containing resource scanners, generic CQRS stuff, <br/>localization service, etc. |
| `LocalizationProvider.AdminUI.Models` | Data model for AdminUI service API endpoints. <br/>Can be used to extend and create new AdminUI integrations. |
| `LocalizationProvider.AspNetCore` | Main core package to integrate into ASP.NET Core (.NET 5.0) runtime. |
| `LocalizationProvider.Storage.AzureTables` | If you need to store resources and translations in Azure Table storage. |
| `LocalizationProvider.Storage.PostgreSql` | Storage implementation for some fancy and geeky Linux fans. |
| `LocalizationProvider.Storage.SqlServer` | Standard implementation of the MSSQL storage. |
| `LocalizationProvider.AdminUI.AspNetCore` | AdminUI integration package for ASP.NET Core (.NET 5.0). |
| `LocalizationProvider.Csv` | Export support for CSV format. |
| `LocalizationProvider.AdminUI.AspNetCore.Csv` | AdminUI integaration for CSV format exporter. |
| `LocalizationProvider.Xliff` | Export support for XLIFF format. |
| `LocalizationProvider.AdminUI.AspNetCore.Xliff` | AdminUI integaration for XLIFF format exporter. |
| `DbLocalizationProvider.EPiServer` | If you are running on Optimizely, you can use this package to integrate <br/>DbLocalizationProvider into Optimizely runtime. |
| `DbLocalizationProvider.AdminUI.EPiServer` | Integrate AdminUI into Optimizely runtime. |



## Configuration

* Removed static methods such as:

```
ConfigurationContext.Setup()
ConfigurationContext.Current
```

* Renamed fallback cultures property:

```
ConfigurationContext.FallbackCultures
```

to

```
ConfigurationContext.FallbackLanguages
```


## Data Model

* `LocalizationResource.Translations` from `ICollection<LocalizationResourceTranslation>` to `LocalizationResourceTranslationCollection`
* introduced `IResourceRepository` - for easier storage implementations
* moved `DiscoveredResource` from `DbLocalizationProvider.Sync` to `DbLocalizationProvider.Abstractions`
* removed `GetAllTranslations` query


## Storage

### Implementing Custom Storage Provider

`IResourceRepository` repository interface to be implemented by any storage implementation. So now if you are considering adding new storage implementation - this is the only interface you might need to provide an implementation for (with the rest of the schema sync stuff if any).

### New Implementation - Azure Tables
For those who are running on low-end budgets and SQL Server is just overkill, there is a new storage implementation - Microsoft Azure Storage tables.

Just install the package:

```powershell
> dotnet add package LocalizationProvider.Storage.AzureTables
```

And then (in your `Startup.cs`):

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services
        .AddDbLocalizationProvider(ctx =>
        {
            // ..

             ctx.UseAzureTables("...");
       });
}
```

# AdminUI for .NET Runtime

## Configuration Changes

In favor of no static stuff `UiConfigurationContext.Current` property has been removed.

## Restricting Access

Following properties have been removed in favor of lambda to configure access policy:

* `UiConfigurationContext.AuthorizedAdminRoles` - list of roles with administrator access;
* `UiConfigurationContext.AuthorizedEditorRoles` - list of roles with editor access;

Now you can just define access policy:

```csharp
services.AddDbLocalizationProviderAdminUI(_ =>
{
    _.AccessPolicyOptions = builder =>
        builder.AddRequirements(new RolesAuthorizationRequirement(new [] { "test" }));
});
```

If none is set - by default only users with `Administrators` role have access to AdminUI.

## Some New Features

AdminUI along the way got some shiny blings.

* Sticky header - if you have reaaally long list of resources - now the table header will stick to the top of the page for you to easier understand which language are you going to edit.

* List of available languages - now you can select which languages you would like to see in AdminUI. This might be handy if you have many languages available on the site but you want to work only with a subset of those.

* Added support for CSV and XLIFF export formats:

```csharp
public void ConfigureServices(IServiceCollection services)
{
services
    .AddDbLocalizationProviderAdminUI(_ =>
    {
        // setup adminui
    })
    .AddCsvSupport()
    .AddXliffSupport();
}
```

# Optimizely Runtime

## Configuration

You have to call `.AddOptimizely()` and `.AddOptimizelyAdminUI()` methods (along with additional configuration and setup) to add provider and administrative user interface to your Optimizely application.

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services
        .AddDbLocalizationProvider(ctx =>
        {
            // ..
       })
       .AddOptimizely();
}
```

```csharp
public void ConfigureServices(IServiceCollection services)
{
services
    .AddDbLocalizationProviderAdminUI(_ =>
    {
        // setup adminui
    })
    .AddOptimizelyAdminUI();
}
```

More info - [getting-started-epi.md](https://github.com/valdisiljuconoks/localization-provider-epi/blob/release/docs/getting-started-epi.md)

## Available Languages

Now a list of available languages is fetched from the Optimizely language repository. Sequence and title of the language set by editors are also respected and languages in AdminUI are sorted according to the sequence.

## Current Language (ex. CultureInfo.CurrentUICulture)

For a long time localization provider was relying on `CultureInfo.CurrentUICulture` property to determine in which language resource translation should be fetched. While this was working OK in pure .NET applications, Optimizely has way more advanced contexts and edge-cases on how language could be selected and which one is the current one.

Now library respects current language (preview in edit mode with a different language selected is now properly supported).

# Azure Functions Runtime

No big changes here - integration with Azure Function runtime works as it should be.

```
[assembly: FunctionsStartup(typeof(Startup))]

namespace funcapp
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddDbLocalizationProvider(...);
        }
    }
}
```

# Roadmap for the v7.1

* Retarget to .NET 6.0 (once Optimizely platform will start supporting it)
* Implement "Import" functionality in AdminUI
* Smaller editorial features to make life easier for editors
* Implement "New Resource" feature to manually create resources
* "Delete All" command in AdminUI to TNTify all resources


<br/>
<p>Happy coding! Keep up a good work!</p>

Stay safe!

[*eof*]
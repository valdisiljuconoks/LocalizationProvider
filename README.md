[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=valdisiljuconoks_LocalizationProvider&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=valdisiljuconoks_LocalizationProvider)

# Supporting LocalizationProvider

If you find this library useful, cup of coffee would be awesome! You can support further development of the library via [Paypal](https://paypal.me/valdisiljuconoks).

# Localization Provider v8.0 IS OUT!

I'm pleased to announce that Localization Provider v8.0 is finally out. Again - took a bit longer than expected :)

What's new?

* .NET8 set as default target
* Added provider model for translations - starting with [Azure AI](common/docs/translators.md) for automatic translations
* `ConfigurationContext` now supports config configuration as well (you can change some settings after you have added and configured default settings for localization provider). This is very useful in unit test scenarios when you need to adjust some settings for specific test.
* Various bug fixes
* Some performance improvements (resource key comparison, [pagination in Admin UI](aspnetcore/docs/getting-started-adminui.md))
* Security improvements (by default upgrading insecure connections)
* Dependencies upgrade

More info in this [blog post](https://tech-fellow.eu/2024/02/28/localization-provider-v8-released/).

# What is the LocalizationProvider project?

LocalizationProvider project is ASP.NET Mvc web application localization provider on steroids.

Giving you the main following features:
* Database-driven localization provider for Asp.Net Mvc applications projects
* Easy resource registrations via code
* Supports hierarchical resource organization (with help of child classes)
* Administration UI for editors to change or add new translations for required languages

## Getting Started (.NET)

### Bare Minimum to Start With
Below are code fragments that are essential to get started with a localization provider.

Install required packages:

```
> dotnet add package LocalizationProvider.AspNetCore
> dotnet add package LocalizationProvider.AdminUI.AspNetCore
> dotnet add package LocalizationProvider.Storage.SqlServer
```

Following service configuration (usually in `Startup.cs`) is required to get the localization provider working:

```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // add your authorization provider (asp.net identity, identity server, whichever..)

        services
            .AddControllersWithViews()
            .AddMvcLocalization();

        services.AddRazorPages();
        services.AddRouting();

        services.AddDbLocalizationProvider(_ =>
        {
            _.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            ...
        });

        services.AddDbLocalizationProviderAdminUI(_ =>
        {
            ...
        });
    }

    ...
}
```

And following setup of the application is required as a minimum (also usually located in `Startup.cs`):

```csharp
public class Startup
{
    ...

    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseDbLocalizationProvider();
        app.UseDbLocalizationProviderAdminUI();
        app.UseDbLocalizationClientsideProvider(); //assuming that you like also Javascript

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapRazorPages();
            endpoints.MapDbLocalizationAdminUI();
            endpoints.MapDbLocalizationClientsideProvider();
        });
    }
}
```


Also, you can refer to [sample app in GitHub](https://github.com/valdisiljuconoks/localizationprovider/tree/master/aspnetcore/tests/DbLocalizationProvider.Core.AspNetSample) for some more hints if needed.

### More Detailed Help

* [Getting Started](aspnetcore/docs/getting-started-netcore.md)
* [Getting Started with AdminUI](aspnetcore/docs/getting-started-adminui.md)
* [Localizing App Content](aspnetcore/docs/localizing-content-netcore.md)
* [Localizing View Model (with DataAnnotations attributes)](aspnetcore/docs/localizing-viewmodel-netcore.md)
* [Localizing Client-side](aspnetcore/docs/client-side-provider-netcore.md)


## Getting Started - Optimizely CMS/Commerce

* [Working with Resources (Optimizely)](optimizely/docs/working-with-resources-epi.md)
* [Optimizely Frontend Localization](optimizely/docs/jsresourcehandler-epi.md)
* [Optimizely Xml File Migration](optimizely/docs/xml-migration-epi.md)


## Supported DbLocalizationProvider Storage Providers

* [SQL Server](common/docs/sqlserver.md)
* PostgreSql
* MongoDb
* Azure Tables

## Working with DbLocalizationProvider Stuff

* [Localized Resource Types](common/docs/resource-types.md)
* [Synchronization Process](common/docs/sync-net.md)
* [MSSQL Storage Configuration](common/docs/mssql.md)
* [Working with Resources](common/docs/working-with-resources-net.md)
* [Working with Languages](common/docs/working-with-languages-net.md)
* [Translating System.Enum Types](common/docs/translate-enum-net.md)
* [Mark Required Fields](common/docs/required-fields.md)
* [Foreign Resources](common/docs/foreign-resources.md)
* [Hidden Resources](common/docs/hidden-resources.md)
* [Reference Other Resource](common/docs/ref-resources.md)
* [Cache Event Notifications](common/docs/cache-events.md)
* [XLIFF Support](common/docs/xliff.md)
* [CSV Support](common/docs/csv.md)
* [Migrations & Refactorings](common/docs/migr.md)

## Integrating with Optimizely
* For more information about Optimizely integration - read [here](https://github.com/valdisiljuconoks/localization-provider-epi/blob/master/README.md)

# Other Versions

## Localization Provider v7.x Released

Please read more in [this blog post](https://tech-fellow.eu/2022/01/23/dblocalizationprovider-for-optimizely/)!


# More Info

* [Part 1: Resources and Models](https://tech-fellow.eu/2016/03/16/db-localization-provider-part-1-resources-and-models/)
* [Part 2: Configuration and Extensions](https://tech-fellow.eu/2016/04/22/db-localization-provider-part-2-configuration-and-extensions/)
* [Part 3: Import and Export](https://tech-fellow.eu/2017/02/23/localization-provider-import-and-export-merge/)
* [Part 4: Resource Refactoring and Migrations](https://tech-fellow.eu/2017/10/10/localizationprovider-tree-view-export-and-migrations/)

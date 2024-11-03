# Getting Started with AdminUI for Asp.Net Core

## Install Package

```powershell
> dotnet add package LocalizationProvider.AdminUI.AspNetCore
```

## Configure Provider

For Minimal API syntax use following set of configuration as your starting point to get AdminUI up & running.

```csharp
using DbLocalizationProvider.AdminUI.AspNetCore;
using DbLocalizationProvider.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services
    .AddControllersWithViews()
    .AddMvcLocalization();

services.AddRazorPages();

services
    .AddMemoryCache()
    .AddAuthorization();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseDbLocalizationProvider();
app.UseDbLocalizationProviderAdminUI();

app.MapRazorPages();
app.MapControllers();

app.Run();
```

## Configure Services (.NET)
In order to add AdminUI module to your Asp.Net Core Mvc application you have to first add services to dependency container (service collection) via `services.AddDbLocalizationProviderAdminUI()` method:

```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services
            .AddControllersWithViews()
            .AddMvcLocalization();

        services.AddRazorPages();
        services.AddRouting();

        ...

        services.AddDbLocalizationProvider(cfg =>
        {
            // configure provider
            // for example cfg.UseSqlServer(...);
            cfg...
        });

        services.AddDbLocalizationProviderAdminUI(c =>
        {
            ...
            c.ShowInvariantCulture = true;
        });
    }

    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
        app.UseRouting();
        app.UseStaticFiles();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseDbLocalizationProvider();
        app.UseDbLocalizationProviderAdminUI();

        ...

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            endpoints.MapRazorPages();
            ...
        });
    }
}
```

You can also configure AdminUI according to your requirements by using passed in UI configuration context (`UiConfigurationContext`).

Following configuration options are available:

| Option | Description |
|------|------|
| `MaxResourceKeyDisplayLength` | Maximnum number of symbols to show for resource key. Default 80.  |
| `MaxResourceKeyPopupTitleLength` | Maximnum number of symbols to show for resource edit window title. Default 80. |
| `DefaultView` | Which view to show when accessing AdminUI. Default is `Table`. |
| `ShowInvariantCulture` | Do you want to see invariant culture column? |
| `ShowHiddenResources` | Do you want to see hidden resources (decorated with `[Hidden]` attribute)? |
| `CustomCssPath` | Make your AdminUI look familiar using external CSS file. |
| `RootUrl` | Mapping url of AdminUI (by which URL you will be able to access the user interface). |
| `HideDeleteButton` | Should `Delete` button be visible? |
| `AccessPolicyOptions` | How are you going to secure access to AdminUI? |
| `UseAvailableLanguageListFromStorage` | Flag whether list of available languages should be taked from the underlying storage. |
| `EnableDbSearch` | Set this this `true` to enable server-side search (should be used if AdminUI performance gives you some headaches). |
| `PageSize` | If `EnableDbSearch` is set to `true` this controls how many resources will be returned. If you don't see your resources, try to be more specific in search or increase page size. Default is `50` items. |

### Post Configuration

It is also possible to perform post configuration (after you have called `AddDbLocalizationProviderAdminUI()`) of the localization provider Admin UI.
This is useful when you are unit testing your web app, after Startup code is executed and you want to make sure that some post configuration settings are applied for your unit tests to execute correctly.

**NB!** Please note, that it is *not* possible to configure `RootUrl` and `AccessPolicyOptions`, as these options affects settings regsitered during DI build process.

To post configure localization provider Admin UI you have to follow standard .NET Options pattern:

```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        ...

        services.AddDbLocalizationProviderAdminUI(c =>
        {
            ...
            c.ShowInvariantCulture = true;
        });

        services.Configure<UiConfigurationContext>(ctx =>
        {
            ctx.DefaultView = ResourceListView.Table;
        });
    }
}
```

## Accessing AdminUI
By default administration UI is mapped on `/localization-admin` path. You can customize path via `app.AddDbLocalizationProviderAdminUI();`. For example to map to `/loc-admin-ui`, you have to:

```
services.AddDbLocalizationProviderAdminUI(_ =>
{
    _.RootUrl = "/loc-admin-ui";
});
```

## Securing Admin UI
AdminUI by default is secured with user roles access policy requirement.

```csharp
public class CheckAdministratorsRoleRequirement
    : AuthorizationHandler<CheckAdministratorsRoleRequirement>, IAuthorizationRequirement
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        CheckAdministratorsRoleRequirement requirement)
    {
        if (context.User.IsInRole("Administrators"))
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail();
        }

        return Task.CompletedTask;
    }
}
```
If you want to customize access policy - you can configure it via `Configure` method on startup:

```csharp
services
    .AddDbLocalizationProviderAdminUI(_ =>
    {
        _.AccessPolicyOptions = builder => builder.AddRequirements(...);
    });
```

# Known Issues

## Admin UI Returns 404

If accessing AdminUI (by default `/localization-admin`) you get 404, check if you have added Razor page mapping.

```csharp
services.AddRazorPages();

app.MapRazorPages();
```

## Static Files are Missing

Ref: [#213](https://github.com/valdisiljuconoks/localization-provider-opti/issues/213).

Solution is to add static web assets configuration.

```csharp
.ConfigureWebHostDefaults(webBuilder =>
{
    webBuilder.UseStaticWebAssets();
});
```

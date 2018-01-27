# Getting Started (Asp.Net Core)

## Install Package

```
PM> Install-Package LocalizationProvider.AspNetCore
```

## Configure Services

In your `Startup.cs` class you need to stuff related to Mvc localization (to get required services into DI container - service collection).

And then `services.AddDbLocalizationProvider()`. You can pass in configuration settings class and setup provider's behavior.

```csharp
public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddLocalization();

        services.AddMvc()
                .AddViewLocalization()
                .AddDataAnnotationsLocalization();

        services.AddDbLocalizationProvider(cfg =>
        {
            cfg...
        });
    }
}
```

After then you will need to make sure that you start using the provider:

```csharp
public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

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

Using localization provider will make sure that resources are discovered and registered in the database (if this process will not be disabled via `AddDbLocalizationProvider()` method).

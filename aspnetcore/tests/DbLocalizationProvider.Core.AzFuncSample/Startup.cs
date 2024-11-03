using System;
using System.IO;
using System.Reflection;
using DbLocalizationProvider.AspNetCore;
using DbLocalizationProvider.Storage.SqlServer;
using funcapp;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

[assembly: FunctionsStartup(typeof(Startup))]

namespace funcapp;

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        builder.Services.AddSingleton<MyService>();

        var actual_root = Environment.GetEnvironmentVariable("AzureWebJobsScriptRoot") // local_root
                          ?? (Environment.GetEnvironmentVariable("HOME") == null
                              ? Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
                              : $"{Environment.GetEnvironmentVariable("HOME")}/site/wwwroot"); // azure_root

        var b = new ConfigurationBuilder()
            .SetBasePath(actual_root)
            .AddJsonFile("settings.json", true)
            .AddJsonFile("local.settings.json", true)
            .Build();

        //builder.Services.AddMemoryCache();
        builder.Services.AddLogging(b =>
        {
            b.SetMinimumLevel(LogLevel.Trace);
            b.AddConsole();
        });

        builder.Services.AddDbLocalizationProvider(ctx =>
        {
            ctx.EnableInvariantCultureFallback = true;
            ctx.DiscoverAndRegisterResources = false;
            ctx.DiagnosticsEnabled = true;

            ctx.UseSqlServer(b.GetConnectionString("DefaultConnection"));
        });

        builder.Services.BuildServiceProvider().UseDbLocalizationProvider();
    }
}

public class MyService { }

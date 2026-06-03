using DbLocalizationProvider.AdminUI.AspNetCore;
using DbLocalizationProvider.AdminUI.AspNetCore.Routing;
using DbLocalizationProvider.AdminUI.EPiServer;
using DbLocalizationProvider.AspNetCore;
using DbLocalizationProvider.AspNetCore.ClientsideProvider.Routing;
using DbLocalizationProvider.EPiServer;
using DbLocalizationProvider.Storage.SqlServer;
using DbLocalizationProvider.Translator.Azure;
using EPiServer.Authorization;
using EPiServer.Cms.UI.AspNetIdentity;
using EPiServer.Data;
using EPiServer.DependencyInjection;
using EPiServer.Scheduler;
using EPiServer.Web.Routing;
using Optimizely.Alloy.Cms13.Extensions;

namespace Optimizely.Alloy.Cms13;

public class Startup(IWebHostEnvironment webHostingEnvironment, IConfiguration config)
{
    public void ConfigureServices(IServiceCollection services)
    {
        if (webHostingEnvironment.IsDevelopment())
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", Path.Combine(webHostingEnvironment.ContentRootPath, "App_Data"));

            services.Configure<SchedulerOptions>(options => options.Enabled = false);
        }

        services.Configure<DataAccessOptions>(o => o.UpdateDatabaseCompatibilityLevel = true);

        services.AddMvc();

        services
            .AddCmsAspNetIdentity<ApplicationUser>()
            .AddCms()
            .AddAlloy()
            .AddAdminUserRegistration()
            .AddEmbeddedLocalization<Startup>();

        // Required by Wangkanai.Detection
        services.AddDetection();

        services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromSeconds(10);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });

        services
            .AddDbLocalizationProvider(ctx =>
            {
                ctx.UseSqlServer(config.GetConnectionString("EPiServerDB")!);

                var section = config.GetSection("AzureCognitiveServices");
                if (section.Exists())
                {
                    var accessKey  = section["AccessKey"];
                    var region = section["Region"];

                    if (!string.IsNullOrWhiteSpace(accessKey) && !string.IsNullOrWhiteSpace(region))
                    {
                        ctx.UseAzureCognitiveServices(accessKey, region);
                    }
                }
            })
            .AddOptimizely()            // add Optimizely integration
            .AddDbLocalizationProviderAdminUI(ctx =>
            {
                // now you can set different options via configuration context (`ctx`)

                ctx.AccessPolicyOptions = builder => builder.RequireRole(Roles.WebAdmins);
            })
            .AddOptimizelyAdminUI()     // add Optimizely integration (adds menu items and stuff)
            .AddCsvSupport()            // you can also add additional export formats if needed
            .AddXliffSupport();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        // Required by Wangkanai.Detection
        app.UseDetection();
        app.UseSession();

        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseCors();

        app.UseDbLocalizationProvider();
        app.UseDbLocalizationProviderAdminUI();
        app.UseDbLocalizationClientsideProvider();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapContent();
            endpoints.MapRazorPages();
            endpoints.MapDbLocalizationAdminUI();
            endpoints.MapDbLocalizationClientsideProvider();
        });
    }
}

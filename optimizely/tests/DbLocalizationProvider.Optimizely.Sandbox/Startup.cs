using DbLocalizationProvider.Optimizely.Sandbox.Extensions;
using EPiServer.Authorization;
using EPiServer.Cms.UI.AspNetIdentity;
using EPiServer.Data;
using EPiServer.DependencyInjection;
using EPiServer.Framework.Localization;
using EPiServer.Scheduler;
using EPiServer.Web;
using EPiServer.Web.Routing;
using Microsoft.Extensions.Options;
using Optimizely.Graph.DependencyInjection;

namespace DbLocalizationProvider.Optimizely.Sandbox;

public class Startup(IWebHostEnvironment webHostingEnvironment)
{
    public void ConfigureServices(IServiceCollection services)
    {
        if (webHostingEnvironment.IsDevelopment())
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", Path.Combine(webHostingEnvironment.ContentRootPath, "App_Data"));

            services.Configure<SchedulerOptions>(options => options.Enabled = false);
        }

        services.Configure<DataAccessOptions>(o => o.UpdateDatabaseCompatibilityLevel = true);

        services
            .AddCmsAspNetIdentity<ApplicationUser>()
            .AddCms()
            .AddAlloy()
            .AddAdminUserRegistration()
            .AddEmbeddedLocalization<Startup>()
            .Configure<LocalizationOptions>(o =>
            {
                o.FallbackBehavior = FallbackBehaviors.FallbackCulture;
            })
            .Configure<UIOptions>(uiOptions =>
            {
                uiOptions.UIShowGlobalizationUserInterface = true;
            })
            .Configure<RequestLocalizationOptions>(opts =>
            {
                //opts.SupportedCultures = supportedCultures;
                //opts.SupportedUICultures = supportedCultures;
                opts.ApplyCurrentCultureToResponseHeaders = true;
            });

        //services.AddContentGraph();
        services.AddContentManager();

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
                        //ctx.FallbackLanguages.Try(supportedCultures);
                        ctx.EnableInvariantCultureFallback = true;
                        ctx.EnableLegacyMode = () => true;

                        ctx.UseSqlServer(connectionstring);

                        ctx.FlexibleRefactoringMode = true;
                    })
                    .AddOptimizely();

        // post setup configuration
        services.Configure<ConfigurationContext>(ctx => ctx.DiagnosticsEnabled = true);

        services
            .AddDbLocalizationProviderAdminUI(ctx =>
            {
                ctx.RootUrl = "/localization-admin-ui";

                ctx.AccessPolicyOptions = builder => builder.RequireRole(Roles.CmsAdmins);

                ctx.ShowInvariantCulture = true;
                ctx.ShowHiddenResources = false;
                ctx.DefaultView = ResourceListView.Tree;
                ctx.CustomCssPath = "/css/custom-adminui.css";
                ctx.HideDeleteButton = false;
                ctx.EnableDbSearch = true;
                ctx.PageSize = 5;
            })
            .AddOptimizelyAdminUI()
            .AddCsvSupport()
            .AddXliffSupport();

        services.Configure<UiConfigurationContext>(ctx => ctx.DefaultView = ResourceListView.Table);
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

        var cache = app.ApplicationServices.GetRequiredService<IOptions<ConfigurationContext>>().Value.CacheManager;
        var cache2 = app.ApplicationServices.GetRequiredService<ICacheManager>();

        cache.OnInsert += e => CacheManagerOnOnInsert(e, app.ApplicationServices.GetRequiredService<ILogger<Startup>>());
        cache.OnRemove += e => CacheManagerOnOnRemove(e, app.ApplicationServices.GetRequiredService<ILogger<Startup>>());

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapContent();

            endpoints.MapRazorPages();

            endpoints.MapDbLocalizationClientsideProvider();
        });
    }

    private void CacheManagerOnOnInsert(CacheEventArgs e, ILogger<Startup> logger)
    {
        logger.LogInformation($"CACHE ADDED: {e.ResourceKey}");
    }

    private void CacheManagerOnOnRemove(CacheEventArgs e, ILogger<Startup> logger)
    {
        logger.LogInformation($"CACHE REMOVED: {e.ResourceKey}");
    }
}

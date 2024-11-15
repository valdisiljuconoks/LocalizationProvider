using System;
using System.Collections.Generic;
using System.Globalization;
using AlloySampleSite.Extensions;
using AlloySampleSite.Infrastructure;
using EPiServer.Cms.UI.AspNetIdentity;
using EPiServer.Data;
using EPiServer.Web.Routing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using AlloySampleSite.Resources;
using DbLocalizationProvider;
using DbLocalizationProvider.AdminUI.AspNetCore;
using DbLocalizationProvider.AdminUI.EPiServer;
using DbLocalizationProvider.AspNetCore;
using DbLocalizationProvider.AspNetCore.ClientsideProvider.Routing;
using DbLocalizationProvider.Cache;
using DbLocalizationProvider.EPiServer;
using DbLocalizationProvider.Storage.SqlServer;
using EPiServer.Authorization;
using EPiServer.Framework.Localization;
using EPiServer.Logging.Compatibility;
using EPiServer.Web;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;

namespace AlloySampleSite
{
    public class Startup
    {
        private readonly IWebHostEnvironment _webHostingEnvironment;
        private readonly IConfiguration _configuration;

        public Startup(IWebHostEnvironment webHostingEnvironment, IConfiguration configuration)
        {
            _webHostingEnvironment = webHostingEnvironment;
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var dbPath = Path.Combine(_webHostingEnvironment.ContentRootPath, "App_Data\\Alloy.mdf");
            var connectionstring = _configuration.GetConnectionString("EPiServerDB") ?? $"Data Source=(LocalDb)\\MSSQLLocalDB;AttachDbFilename={dbPath};Initial Catalog=alloy_mvc_netcore;Integrated Security=True;Connect Timeout=30;MultipleActiveResultSets=True";


            var logger = new LoggerConfiguration()
                         .ReadFrom.Configuration(_configuration)
                         .Enrich.FromLogContext()
                         .CreateLogger();

            services.AddLogging(builder =>
            {
                builder.ClearProviders();
                builder.AddSimpleConsole();
                builder.AddSerilog(logger);
            });

            services.Configure<DataAccessOptions>(o =>
            {
                o.SetConnectionString(connectionstring);
            });

            services.AddCmsAspNetIdentity<ApplicationUser>(o =>
            {
                if (string.IsNullOrEmpty(o.ConnectionStringOptions?.ConnectionString))
                {
                    o.ConnectionStringOptions = new ConnectionStringOptions()
                    {
                        ConnectionString = connectionstring
                    };
                }
            });

            services.AddOpenIDConnect<ApplicationUser>(true);
            services.AddOpenIDConnectUI();

            services.AddMvc();

            // add Episerver stuff
            var supportedCultures = new List<CultureInfo> { new("lv-LV"), new("sv"), new("no"), new("en") };

            services.AddCms()
                    .AddAlloy()
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

            services.AddLocalization(options =>
            {
                options.ResourcesPath = Path.Combine(_webHostingEnvironment.ContentRootPath, "lang");
            });

            services
                    .AddDbLocalizationProvider(ctx =>
                    {
                        //ctx.FallbackLanguages.Try(supportedCultures);
                        ctx.EnableInvariantCultureFallback = true;
                        ctx.EnableLegacyMode = () => true;

                        ctx.ManualResourceProviders.Add<SomeManualResourceProvider>();
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

        private void CacheManagerOnOnInsert(CacheEventArgs e, ILogger<Startup> logger)
        {
            logger.LogInformation($"CACHE ADDED: {e.ResourceKey}");
        }

        private void CacheManagerOnOnRemove(CacheEventArgs e, ILogger<Startup> logger)
        {
            logger.LogInformation($"CACHE REMOVED: {e.ResourceKey}");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMiddleware<AdministratorRegistrationPageMiddleware>();
            }

            //var options = app.ApplicationServices.GetRequiredService<IOptions<RequestLocalizationOptions>>();
            //app.UseRequestLocalization(options.Value);

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
                endpoints.MapControllerRoute("Register", "/Register", new { controller = "Register", action = "Index" });
                endpoints.MapControllerRoute("TestView", "/TestView", new { controller = "Test", action = "TestView" });

                endpoints.MapContent();
                endpoints.MapRazorPages();

                endpoints.MapDbLocalizationClientsideProvider();
            });
        }
    }
}

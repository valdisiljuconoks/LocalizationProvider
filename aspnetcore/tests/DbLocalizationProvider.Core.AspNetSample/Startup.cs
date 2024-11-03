using System.Collections.Generic;
using System.Globalization;
using DbLocalizationProvider.AdminUI.AspNetCore;
using DbLocalizationProvider.AspNetCore;
using DbLocalizationProvider.AspNetCore.ClientsideProvider.Routing;
using DbLocalizationProvider.Core.AspNet.ForeignAssembly;
using DbLocalizationProvider.Core.AspNetSample.Data;
using DbLocalizationProvider.Core.AspNetSample.Resources;
using DbLocalizationProvider.Logging;
using DbLocalizationProvider.Storage.SqlServer;
using DbLocalizationProvider.Translator.Azure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DbLocalizationProvider.Core.AspNetSample;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContext<ApplicationDbContext>(
            options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

        services
            .AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        // mvc stuff
        services
            .AddControllersWithViews( /*opt => opt.EnableEndpointRouting = false*/)
            .AddMvcLocalization();

        services.AddRazorPages();
        services.AddRouting();
        services.AddHealthChecks();

        services.AddLogging(c =>
        {
            c.AddConsole();
            c.AddDebug();
        });

        var supportedCultures = new List<CultureInfo>
        {
            new("lv-LV"),
            new("sv"),
            new("no"),
            new("en"),
            new("fr"),
            new("bg-BG"),
            new("de-DE"),
            new("fi-FI"),
            new("hu-HU"),
            new("nl-NL"),
            new("pl"),
            new("sk"),
            new("uk"),
            new("et-EE"),
            new("lt-LT"),
            new("es-CR"),
            new("en-ZA"),
            new("en-JM"),
            new("es-DO"),
            new("es-VE"),
            new("es-CO"),
            new("en-BZ"),
            new("en-TT"),
            new("es-EC"),
            new("es-UY"),
            new("es-SV"),
            new("es-SV"),
            new("es-PR"),
            new("se-FI"),
            new("hr-BA"),
            new("mi-NZ"),
            new("ns-ZA"),
            new("mt-MT"),
            new("en-IE"),
            new("de-LI"),
            new("fr-LU"),
            new("es-PA"),
            new("fr-MC"),
            new("ar-TN"),
            new("ar-DZ"),
            new("az-Latn-AZ"),
            new("eu-ES"),
            new("fo-FO"),
            new("hi-IN"),
            new("fa-IR"),
            new("ur-PK"),
            new("he-IL"),
            new("el-GR"),
            new("da-DK"),
            new("cs-CZ"),
            new("zh-TW")
        };

        services.Configure<RequestLocalizationOptions>(opts =>
        {
            opts.DefaultRequestCulture = new RequestCulture("en");
            opts.SupportedCultures = supportedCultures;
            opts.SupportedUICultures = supportedCultures;
        });

        services.AddDbLocalizationProvider(x =>
        {
            x.EnableInvariantCultureFallback = true;
            x.DefaultResourceCulture = CultureInfo.InvariantCulture;
            x.CustomAttributes.Add(typeof(WeirdCustomAttribute));
            x.ScanAllAssemblies = true;
            x.FallbackLanguages.Try(supportedCultures);
            x.ForeignResources.Add<SomeForeignViewModel>();
            //.Try(new CultureInfo("sv"))
            //.Then(new CultureInfo("no"))
            //.Then(new CultureInfo("en"));

            x.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            //_.UseAzureTables("UseDevelopmentStorage=true");

            x.ManualResourceProviders.Add<SomeManualResources>();
            x.ModelMetadataProviders.ReplaceProviders = true;
            x.FlexibleRefactoringMode = true;

            // read these settings from somewhere
            //x.UseAzureCognitiveServices("ADD YOUR KEY...", "ADD YOUR REGION...");
        });

        services.AddDbLocalizationProviderAdminUI(x =>
            {
                x.RootUrl = "/localization-admin";
                //_.AccessPolicyOptions = builder => builder.AddRequirements(new RolesAuthorizationRequirement(new [] { "test" }));
                x.ShowInvariantCulture = true;
                x.ShowHiddenResources = false;
                x.DefaultView = ResourceListView.Tree;
                x.CustomCssPath = "/css/custom-adminui.css";
                x.HideDeleteButton = false;
                x.EnableDbSearch = true;
                x.PageSize = 50;
            })
            .AddCsvSupport()
            .AddXliffSupport();

        services.Configure<UiConfigurationContext>(ctx => ctx.DefaultView = ResourceListView.Table);
        //.VerifyDbLocalizationProviderAdminUISetup();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        var options = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
        app.UseRequestLocalization(options.Value);

        app.UseRouting();
        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseDbLocalizationProvider();
        app.UseDbLocalizationProviderAdminUI();
        app.UseDbLocalizationClientsideProvider();

        // app.UseMvc(routes =>
        // {
        //     routes.MapDbLocalizationAdminUI();
        //     routes.MapDbLocalizationClientsideProvider();
        //
        //     routes.MapRoute(
        //         name: "default",
        //         template: "{controller=Home}/{action=Index}/{id?}");
        // });

        using (var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
        {
            scope.ServiceProvider.GetService<ApplicationDbContext>().Database.Migrate();
        }

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            endpoints.MapRazorPages();

            endpoints.MapDbLocalizationClientsideProvider();

            endpoints.MapHealthChecks("healthz");
        });


        // app.UseMvc(routes =>
        //                // Enables HTML5 history mode for Vue app
        //                // Ref: https://weblog.west-wind.com/posts/2017/aug/07/handling-html5-client-route-fallbacks-in-aspnet-core
        //                routes.MapRoute(
        //                    name: "catch-all",
        //                    template: "{*url}",
        //                    defaults: new { controller = "Home", action = "Index" }));
    }
}

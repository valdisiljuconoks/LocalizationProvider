using System.Collections.Generic;
using System.Globalization;
using DbLocalizationProvider.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackifyMiddleware;

namespace DbLocalizationProvider.Core.AspNetSample
{
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
            services.AddLocalization(_ => _.ResourcesPath = "Resources");

            services.AddMvc()
                .AddViewLocalization(_ => _.ResourcesPath = "Resources")
                .AddDataAnnotationsLocalization();

            services.AddDbLocalizationProvider(_ =>
            {
                _.ConnectionName = Configuration.GetConnectionString("DefaultConnection");
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            var rlOptions = new RequestLocalizationOptions
            {
                SupportedCultures = new List<CultureInfo>
                {
                    new CultureInfo("en")
                },
                SupportedUICultures = new List<CultureInfo>
                {
                    new CultureInfo("en")
                },
                DefaultRequestCulture = new RequestCulture("en")
            };

            app.UseRequestLocalization(rlOptions);

            if(env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseMiddleware<RequestTracerMiddleware>();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    "default",
                    "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseDbLocalizationProvider();
        }
    }
}

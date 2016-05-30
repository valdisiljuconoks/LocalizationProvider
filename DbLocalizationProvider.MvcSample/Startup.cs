using System.Globalization;
using DbLocalizationProvider.AdminUI;
using DbLocalizationProvider.MvcSample;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Startup))]

namespace DbLocalizationProvider.MvcSample
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en");

            app.UseDbLocalizationProvider(ctx => { ctx.ConnectionName = "MyConnectionString"; });

            app.UseDbLocalizationProviderAdminUI();
        }
    }
}

using DbLocalizationProvider.MvcSample;
using DbLocalizationProvider.Sync;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Startup))]

namespace DbLocalizationProvider.MvcSample
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseDbLocalizationProvider();
        }
    }
}

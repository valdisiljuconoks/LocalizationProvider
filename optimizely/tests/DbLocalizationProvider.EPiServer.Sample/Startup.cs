using DbLocalizationProvider.EPiServer.Sample;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Startup))]

namespace DbLocalizationProvider.EPiServer.Sample
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
        }
    }
}

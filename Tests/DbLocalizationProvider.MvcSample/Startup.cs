using System.Globalization;
using DbLocalizationProvider.AdminUI;
using DbLocalizationProvider.MvcSample;
using DbLocalizationProvider.MvcSample.Resources;
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

            app.UseDbLocalizationProvider(ctx =>
                                          {
                                              ctx.ConnectionName = "MyConnectionString";
                                              ctx.EnableInvariantCultureFallback = true;
                                              ctx.DefaultResourceCulture = new CultureInfo("en");
                                              ctx.ModelMetadataProviders.MarkRequiredFields = true;
                                              ctx.ModelMetadataProviders.RequiredFieldResource = () => HomePageResources.RequiredFieldIndicator;
                                              ctx.CustomAttributes = new[]
                                              {
                                                  new CustomAttributeDescriptor(typeof(HelpTextAttribute), false),
                                                  new CustomAttributeDescriptor(typeof(FancyHelpTextAttribute), false),
                                                  new CustomAttributeDescriptor(typeof(TableHeaderTitleAttribute))
                                              };

                                              ctx.ForeignResources.Add(new ForeignResourceDescriptor(typeof(ForeignResources)));
                                          });

            app.Map("/localization-admin", b => b.UseDbLocalizationProviderAdminUI());
        }
    }
}

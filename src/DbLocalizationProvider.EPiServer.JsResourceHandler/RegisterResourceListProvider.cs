using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;

namespace DbLocalizationProvider.EPiServer.JsResourceHandler
{
    [InitializableModule]
    public class RegisterResourceListProvider : IConfigurableModule
    {
        public void Initialize(InitializationEngine context) { }

        public void Uninitialize(InitializationEngine context) { }

        public void ConfigureContainer(ServiceConfigurationContext context)
        {
            context.Services.RemoveAll<IResourceListProvider>();
            context.Services.AddTransient<IResourceListProvider, DbLocalizationResourceListProvider>();
        }
    }
}

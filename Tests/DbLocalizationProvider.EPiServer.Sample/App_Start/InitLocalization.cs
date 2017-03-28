using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using WebModule = EPiServer.Web.InitializationModule;

namespace DbLocalizationProvider.EPiServer.Sample
{
    [InitializableModule]
    [ModuleDependency(typeof(WebModule))]
    public class InitLocalization : IInitializableModule
    {
        public void Initialize(InitializationEngine context)
        {
            ConfigurationContext.Setup(cfg =>
            {
                cfg.DiagnosticsEnabled = true;
                cfg.ModelMetadataProviders.EnableLegacyMode = () => true;
                cfg.CustomAttributes = new[]
                {
                    new CustomAttributeDescriptor(typeof(HelpTextAttribute), false)
                };
            });
        }

        public void Uninitialize(InitializationEngine context) { }
    }
}

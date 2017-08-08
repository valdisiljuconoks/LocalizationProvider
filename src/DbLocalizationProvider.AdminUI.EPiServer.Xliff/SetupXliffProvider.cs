using DbLocalizationProvider.EPiServer;
using DbLocalizationProvider.Xliff;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using InitializationModule = EPiServer.Web.InitializationModule;

namespace DbLocalizationProvider.AdminUI.EPiServer.Xliff
{
    [InitializableModule]
    [ModuleDependency(typeof(InitializationModule))]
    [ModuleDependency(typeof(DbLocalizationProviderInitializationModule))]
    public class SetupXliffProvider : IInitializableModule
    {
        public void Initialize(InitializationEngine context)
        {
            ConfigurationContext.Setup(ctx =>
                                       {
                                           ctx.Export.Providers.Add(new Exporter());
                                           ctx.Import.Providers.Add(new Importer());
                                       });
        }

        public void Uninitialize(InitializationEngine context) { }
    }
}

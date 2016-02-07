using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using InitializationModule = EPiServer.Web.InitializationModule;

namespace DbLocalizationProvider.Sync
{
    [InitializableModule]
    [ModuleDependency(typeof (InitializationModule))]
    public class DiscoverLocalizedModels : IInitializableModule
    {
        public void Initialize(InitializationEngine context)
        {
            if (ConfigurationContext.Current.DiscoverAndRegisterLocalizedModels())
            {
                var types = TypeDiscoveryHelper.GetTypesOfInterface<ILocalizedModel>();
            }
        }

        public void Uninitialize(InitializationEngine context)
        {
            //Add uninitialization logic
        }
    }
}

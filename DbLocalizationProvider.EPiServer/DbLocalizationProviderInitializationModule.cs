using System;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using StructureMap;
using InitializationModule = EPiServer.Web.InitializationModule;

namespace DbLocalizationProvider.EPiServer
{
    [InitializableModule]
    [ModuleDependency(typeof(InitializationModule))]
    public class DbLocalizationProviderInitializationModule : IConfigurableModule
    {
        private IContainer _container;
        private bool _eventHandlerAttached;

        public void Initialize(InitializationEngine context)
        {
            if(_eventHandlerAttached)
            {
                return;
            }

            context.InitComplete += DiscoverAndRegister;
            _eventHandlerAttached = true;
        }

        public void Uninitialize(InitializationEngine context)
        {
            context.InitComplete -= DiscoverAndRegister;
        }

        public void ConfigureContainer(ServiceConfigurationContext context)
        {
            // we need to capture container in order to replace ModelMetaDataProvider if needed
            _container = context.Container;
        }

        private void DiscoverAndRegister(object sender, EventArgs eventArgs)
        {
            ConfigurationContext.Setup(ctx =>
                                       {
                                           ctx.ConnectionName = "EPiServer";
                                           ctx.CacheManager = new EPiServerCacheManager();
                                       });

            var synchronizer = new EPiServerResourceSync();
            synchronizer.DiscoverAndRegister();
        }
    }
}

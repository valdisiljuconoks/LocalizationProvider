using System;
using System.Web.Mvc;
using DbLocalizationProvider.Commands;
using DbLocalizationProvider.DataAnnotations;
using DbLocalizationProvider.EPiServer.Queries;
using DbLocalizationProvider.Queries;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using StructureMap;
using StructureMap.Pipeline;
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

                                           ctx.TypeFactory.ForQuery<AvailableLanguages.Query>().SetHandler<EPiServerAvailableLanguages.Handler>();
                                           ctx.TypeFactory.ForQuery<GetTranslation.Query>().SetHandler<EPiServerGetTranslation.Handler>();
                                           ctx.TypeFactory.ForQuery<GetAllResources.Query>().SetHandler<GetAllResources.Handler>();
                                           ctx.TypeFactory.ForQuery<GetAllTranslations.Query>().SetHandler<GetAllTranslations.Handler>();

                                           ctx.TypeFactory.ForCommand<CreateNewResource.Command>().SetHandler<CreateNewResource.Handler>();
                                           ctx.TypeFactory.ForCommand<DeleteResource.Command>().SetHandler<DeleteResource.Handler>();
                                           ctx.TypeFactory.ForCommand<CreateOrUpdateTranslation.Command>().SetHandler<CreateOrUpdateTranslation.Handler>();
                                           ctx.TypeFactory.ForCommand<ClearCache.Command>().SetHandler<ClearCache.Handler>();
                                       });

            var synchronizer = new EPiServerResourceSync();
            synchronizer.DiscoverAndRegister();

            if(!ConfigurationContext.Current.ModelMetadataProviders.ReplaceProviders)
                return;

            var currentProvider = _container.TryGetInstance<ModelMetadataProvider>();

            if(currentProvider == null)
            {
                // set current provider
                if(ConfigurationContext.Current.ModelMetadataProviders.UseCachedProviders)
                {
                    _container.Configure(ctx => ctx.For<ModelMetadataProvider>().Use<CachedLocalizedMetadataProvider>());
                }
                else
                {
                    _container.Configure(ctx => ctx.For<ModelMetadataProvider>().Use<LocalizedMetadataProvider>());
                }
            }
            else
            {
                // decorate existing provider
                if(ConfigurationContext.Current.ModelMetadataProviders.UseCachedProviders)
                {
                    _container.Configure(ctx => ctx.For<ModelMetadataProvider>(Lifecycles.Singleton)
                                                   .Use(() => new CompositeModelMetadataProvider<CachedLocalizedMetadataProvider>(currentProvider)));
                }
                else
                {
                    _container.Configure(ctx => ctx.For<ModelMetadataProvider>(Lifecycles.Singleton)
                                                   .Use(() => new CompositeModelMetadataProvider<LocalizedMetadataProvider>(currentProvider)));
                }
            }

            for (var i = 0; i < ModelValidatorProviders.Providers.Count; i++)
            {
                var provider = ModelValidatorProviders.Providers[i];
                if(!(provider is DataAnnotationsModelValidatorProvider))
                {
                    continue;
                }

                ModelValidatorProviders.Providers.RemoveAt(i);
                ModelValidatorProviders.Providers.Insert(i, new LocalizedModelValidatorProvider());
                break;
            }
        }
    }
}

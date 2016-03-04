using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using DbLocalizationProvider.DataAnnotations;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.Globalization;
using EPiServer.ServiceLocation;
using StructureMap;
using InitializationModule = EPiServer.Web.InitializationModule;

namespace DbLocalizationProvider.Sync
{
    [InitializableModule]
    [ModuleDependency(typeof (InitializationModule))]
    public class DbLocalizationProviderInitializationModule : IConfigurableModule
    {
        private IContainer _container;
        private bool _eventHandlerAttached;

        public void Initialize(InitializationEngine context)
        {
            if (_eventHandlerAttached)
            {
                return;
            }

            context.InitComplete += DiscoverAndRegister;
            _eventHandlerAttached = true;
        }

        public void ConfigureContainer(ServiceConfigurationContext context)
        {
            // we need to capture container in order to replace ModelMetaDataProvider if needed
            _container = context.Container;
        }

        public void Uninitialize(InitializationEngine context)
        {
            context.InitComplete -= DiscoverAndRegister;
        }

        private void DiscoverAndRegister(object sender, EventArgs eventArgs)
        {
            if (!ConfigurationContext.Current.DiscoverAndRegisterResources)
            {
                return;
            }

            using (var db = new LanguageEntities("EPiServerDB"))
            {
                ResetSyncStatus(db);

                // TODO: look for a way to unify these scanning methods and instead while traveling around the AppDomain collect all necessary types at once
                RegisterDiscoveredResources(db);
                RegisterDiscoveredModels(db);

                if (ConfigurationContext.Current.PopulateCacheOnStartup)
                {
                    PopulateCache();
                }
            }

            if (ConfigurationContext.Current.ReplaceModelMetadataProviders)
            {
                if (ConfigurationContext.Current.UseCachedModelMetadataProviders)
                {
                    _container.Configure(ctx => ctx.For<ModelMetadataProvider>().Use<CachedLocalizedMetadataProvider>());
                }
                else
                {
                    _container.Configure(ctx => ctx.For<ModelMetadataProvider>().Use<LocalizedMetadataProvider>());
                }

                ModelValidatorProviders.Providers.Clear();
                ModelValidatorProviders.Providers.Add(new LocalizedModelValidatorProvider());
            }
        }

        private void PopulateCache()
        {
            var repo = new CachedLocalizationResourceRepository(new LocalizationResourceRepository());
            repo.PopulateCache();
        }

        private void ResetSyncStatus(DbContext db)
        {
            var existingResources = db.Set<LocalizationResource>();
            foreach (var resource in existingResources)
            {
                resource.FromCode = false;
            }

            db.SaveChanges();
        }

        private void RegisterDiscoveredModels(LanguageEntities db)
        {
            var types = TypeDiscoveryHelper.GetTypesWithAttribute<LocalizedModelAttribute>();
            var properties = types.SelectMany(type => TypeDiscoveryHelper.GetAllProperties(type, contextAwareScanning: false));

            foreach (var property in properties)
            {
                RegisterIfNotExist(db, property.Item2, property.Item3);
                db.SaveChanges();
            }
        }

        private void RegisterDiscoveredResources(LanguageEntities db)
        {
            var types = TypeDiscoveryHelper.GetTypesWithAttribute<LocalizedResourceAttribute>();
            var properties = types.SelectMany(type => TypeDiscoveryHelper.GetAllProperties(type));

            foreach (var property in properties)
            {
                RegisterIfNotExist(db, property.Item2, property.Item3);
            }

            db.SaveChanges();
        }

        private void RegisterIfNotExist(LanguageEntities db, string resourceKey, string resourceValue)
        {
            var existingResource = db.LocalizationResources.FirstOrDefault(r => r.ResourceKey == resourceKey);

            if (existingResource != null)
            {
                existingResource.FromCode = true;
                return;
            }

            // create new resource
            var resource = new LocalizationResource(resourceKey)
                           {
                               ModificationDate = DateTime.UtcNow,
                               Author = "type-scanner",
                               FromCode = true
                           };

            var translation = new LocalizationResourceTranslation
                              {
                                  Language = ContentLanguage.PreferredCulture != null ? ContentLanguage.PreferredCulture.Name : "en",
                                  Value = resourceValue
                              };

            resource.Translations.Add(translation);
            db.LocalizationResources.Add(resource);
        }
    }
}

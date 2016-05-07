using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
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
            if(!ConfigurationContext.Current.DiscoverAndRegisterResources)
            {
                return;
            }

            var discoveredTypes = TypeDiscoveryHelper.GetTypes(t => t.GetCustomAttribute<LocalizedResourceAttribute>() != null,
                                                               t => t.GetCustomAttribute<LocalizedModelAttribute>() != null);

            using (var db = new LanguageEntities("EPiServerDB"))
            {
                ResetSyncStatus(db);
                RegisterDiscoveredResources(db, discoveredTypes[0]);
                RegisterDiscoveredModels(db, discoveredTypes[1]);
            }

            if(ConfigurationContext.Current.PopulateCacheOnStartup)
            {
                PopulateCache();
            }

            if(ConfigurationContext.Current.ReplaceModelMetadataProviders)
            {
                var currentProvider = _container.GetInstance<ModelMetadataProvider>();
                if(ConfigurationContext.Current.UseCachedModelMetadataProviders)
                {
                    _container.Configure(ctx => ctx.For<ModelMetadataProvider>()
                                                   .Use<CompositeModelMetadataProvider<CachedLocalizedMetadataProvider>>()
                                                   .Ctor<ModelMetadataProvider>().Is(currentProvider));
                }
                else
                {
                    _container.Configure(ctx => ctx.For<ModelMetadataProvider>()
                                                   .Use<CompositeModelMetadataProvider<LocalizedMetadataProvider>>()
                                                   .Ctor<ModelMetadataProvider>().Is(currentProvider));
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

        private void RegisterDiscoveredModels(LanguageEntities db, IEnumerable<Type> types)
        {
            var properties = types.SelectMany(type => TypeDiscoveryHelper.GetAllProperties(type, contextAwareScanning: false));

            foreach (var property in properties)
            {
                RegisterIfNotExist(db, property.Key, property.Translation);
                db.SaveChanges();
            }
        }

        private void RegisterDiscoveredResources(LanguageEntities db, IEnumerable<Type> types)
        {
            var properties = types.SelectMany(type => TypeDiscoveryHelper.GetAllProperties(type));

            foreach (var property in properties)
            {
                RegisterIfNotExist(db, property.Key, property.Translation);
            }

            db.SaveChanges();
        }

        private void RegisterIfNotExist(LanguageEntities db, string resourceKey, string resourceValue)
        {
            var existingResource = db.LocalizationResources.Include(r => r.Translations).FirstOrDefault(r => r.ResourceKey == resourceKey);
            var defaultTranslationCulture = DetermineDefaultCulture();

            if(existingResource != null)
            {
                existingResource.FromCode = true;

                // if resource is not modified - we can sync default value from code
                if(existingResource.IsModified.HasValue && !existingResource.IsModified.Value)
                {
                    var defaultTranslation = existingResource.Translations.FirstOrDefault(t => t.Language == defaultTranslationCulture);
                    if(defaultTranslation != null)
                    {
                        defaultTranslation.Value = resourceValue;
                    }
                }

                existingResource.ModificationDate = DateTime.UtcNow;
            }
            else
            {
                // create new resource
                var resource = new LocalizationResource(resourceKey)
                               {
                                   ModificationDate = DateTime.UtcNow,
                                   Author = "type-scanner",
                                   FromCode = true,
                                   IsModified = false
                               };

                var translation = new LocalizationResourceTranslation
                                  {
                                      Language = defaultTranslationCulture,
                                      Value = resourceValue
                                  };

                resource.Translations.Add(translation);
                db.LocalizationResources.Add(resource);
            }
        }

        private static string DetermineDefaultCulture()
        {
            return ConfigurationContext.Current.DefaultResourceCulture != null
                       ? ConfigurationContext.Current.DefaultResourceCulture.Name
                       : (ContentLanguage.PreferredCulture != null ? ContentLanguage.PreferredCulture.Name : "en");
        }
    }
}

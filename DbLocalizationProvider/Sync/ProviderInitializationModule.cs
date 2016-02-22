using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.Globalization;
using InitializationModule = EPiServer.Web.InitializationModule;

namespace DbLocalizationProvider.Sync
{
    [InitializableModule]
    [ModuleDependency(typeof (InitializationModule))]
    public class ProviderInitializationModule : IInitializableModule
    {
        private bool _eventHandlerAttached;

        public void Initialize(InitializationEngine context)
        {
            if (_eventHandlerAttached)
            {
                return;
            }

            context.InitComplete += ContextOnInitComplete;
            _eventHandlerAttached = true;
        }

        public void Uninitialize(InitializationEngine context)
        {
            context.InitComplete -= ContextOnInitComplete;
        }

        private void ContextOnInitComplete(object sender, EventArgs eventArgs)
        {
            if (!ConfigurationContext.Current.DiscoverAndRegisterResources)
            {
                return;
            }

            using (var db = new LanguageEntities("EPiServerDB"))
            {
                ResetSyncStatus(db);
                RegisterDiscoveredResources(db);
                RegisterDiscoveredModels(db);
            }

            if (ConfigurationContext.Current.ReplaceModelMetadataProviders)
            {
                ModelMetadataProviders.Current = new LocalizedMetadataProvider();
                ModelValidatorProviders.Providers.Clear();
                ModelValidatorProviders.Providers.Add(new LocalizedModelValidatorProvider());
            }
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
            }

            db.SaveChanges();
        }

        private void RegisterDiscoveredResources(LanguageEntities db)
        {
            var types = TypeDiscoveryHelper.GetTypesWithAttribute<LocalizedResourceAttribute>();
            var properties = types.SelectMany(type => TypeDiscoveryHelper.GetAllProperties(type));

            foreach (var property in properties)
            {
                var info = property.Item1;
                var resourceKey = property.Item2;
                var resourceValue = resourceKey;

                if (TypeDiscoveryHelper.IsStaticStringProperty(info))
                {
                    try
                    {
                        resourceValue = info.GetGetMethod().Invoke(null, null) as string;
                    }
                    catch
                    {
                        // if we fail to retrieve value for the resource - just use its FQN
                    }
                }

                RegisterIfNotExist(db, resourceKey, resourceValue);
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

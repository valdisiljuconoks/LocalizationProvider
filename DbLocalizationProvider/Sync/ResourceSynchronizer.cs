using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using DbLocalizationProvider.Cache;
using DbLocalizationProvider.Commands;
using DbLocalizationProvider.Queries;

namespace DbLocalizationProvider.Sync
{
    public class ResourceSynchronizer
    {
        protected virtual string DetermineDefaultCulture()
        {
            return ConfigurationContext.Current.DefaultResourceCulture != null
                       ? ConfigurationContext.Current.DefaultResourceCulture.Name
                       : "en";
        }

        public void DiscoverAndRegister()
        {
            if(!ConfigurationContext.Current.DiscoverAndRegisterResources)
            {
                return;
            }

            var discoveredTypes = TypeDiscoveryHelper.GetTypes(t => t.GetCustomAttribute<LocalizedResourceAttribute>() != null,
                                                               t => t.GetCustomAttribute<LocalizedModelAttribute>() != null);

            using (var db = new LanguageEntities())
            {
                ResetSyncStatus(db);
                RegisterDiscoveredResources(db, discoveredTypes[0]);
                RegisterDiscoveredModels(db, discoveredTypes[1]);
            }

            if(ConfigurationContext.Current.PopulateCacheOnStartup)
            {
                PopulateCache();
            }
        }

        private void PopulateCache()
        {
            var c = new ClearCache.Command();
            c.Execute();

            var allResources = new GetAllResources.Query().Execute();

            foreach (var resource in allResources)
            {
                var key = CacheKeyHelper.BuildKey(resource.ResourceKey);
                ConfigurationContext.Current.CacheManager.Insert(key, resource);
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
    }
}

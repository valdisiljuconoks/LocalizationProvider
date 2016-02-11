using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web;
using EPiServer;
using EPiServer.Framework.Localization;

namespace DbLocalizationProvider
{
    public class LocalizationResourceRepository
    {
        private const string CacheKeyPrefix = "DbLocalizationProviderCache";

        public string GetTranslation(string key, CultureInfo language)
        {
            var cacheKey = BuildCacheKey(key);
            var cachedResource = CacheManager.Get(cacheKey);

            var localizationResource = cachedResource as LocalizationResource;
            if (localizationResource != null)
            {
                return localizationResource.Translations.FirstOrDefault(t => t.Language == language.Name)?.Value;
            }

            using (var db = GetDatabaseContext())
            {
                var resource = db.LocalizationResources
                                 .Include(r => r.Translations)
                                 .FirstOrDefault(r => r.ResourceKey == key);

                if (resource == null)
                {
                    return null;
                }

                CacheManager.Insert(cacheKey, resource);

                var localization = resource.Translations.FirstOrDefault(t => t.Language == language.Name);
                if (localization != null)
                {
                    return localization.Value;
                }
            }

            return null;
        }

        public IEnumerable<CultureInfo> GetAvailableLanguages()
        {
            var cacheKey = BuildCacheKey("AvailableLanguages");
            var cachedLanguages = CacheManager.Get(cacheKey) as IEnumerable<CultureInfo>;
            if (cachedLanguages != null)
            {
                return cachedLanguages;
            }

            using (var db = GetDatabaseContext())
            {
                var availableLanguages = db.LocalizationResourceTranslations
                                           .Select(t => t.Language)
                                           .Distinct()
                                           .ToList()
                                           .Select(l => new CultureInfo(l)).ToList();

                CacheManager.Insert(cacheKey, availableLanguages);

                return availableLanguages;
            }
        }

        public IEnumerable<LocalizationResource> GetAllResources()
        {
            using (var db = GetDatabaseContext())
            {
                return db.LocalizationResources.Include(r => r.Translations).ToList();
            }
        }

        public IEnumerable<ResourceItem> GetAllTranslations(string key, CultureInfo language)
        {
            var allResources = GetAllResources().Where(r =>
                                                       r.ResourceKey.StartsWith(key)
                                                       && r.Translations.Any(t => t.Language == language.Name)).ToList();

            if (!allResources.Any())
            {
                return Enumerable.Empty<ResourceItem>();
            }

            return allResources.Select(r => new ResourceItem(r.ResourceKey,
                                                             r.Translations.First(t => t.Language == language.Name).Value,
                                                             language));
        }

        private static string BuildCacheKey(string key)
        {
            return $"{CacheKeyPrefix}_{key}";
        }

        internal LanguageEntities GetDatabaseContext()
        {
            return new LanguageEntities("EPiServerDB");
        }

        public void CreateOrUpdateTranslation(string key, CultureInfo language, string newValue)
        {
            using (var db = GetDatabaseContext())
            {
                var resource = db.LocalizationResources.Include(r => r.Translations).FirstOrDefault(r => r.ResourceKey == key);

                if (resource == null)
                {
                    // TODO: return some status response obj
                    return;
                }

                var translation = resource.Translations.FirstOrDefault(t => t.Language == language.Name);

                if (translation != null)
                {
                    // update existing translation
                    translation.Value = newValue;
                }
                else
                {
                    var newTranslation = new LocalizationResourceTranslation
                                         {
                                             Value = newValue,
                                             Language = language.Name,
                                             ResourceId = resource.Id
                                         };

                    db.LocalizationResourceTranslations.Add(newTranslation);
                }

                db.SaveChanges();
                CacheManager.Remove(BuildCacheKey(key));
            }
        }

        public void ClearCache()
        {
            if (HttpContext.Current == null)
            {
                return;
            }

            if (HttpContext.Current.Cache == null)
            {
                return;
            }

            var itemsToRemove = new List<string>();
            var enumerator = HttpContext.Current.Cache.GetEnumerator();

            while (enumerator.MoveNext())
            {
                if (enumerator.Key.ToString().ToLower().StartsWith(CacheKeyPrefix.ToLower()))
                {
                    itemsToRemove.Add(enumerator.Key.ToString());
                }
            }

            foreach (var itemToRemove in itemsToRemove)
            {
                CacheManager.Remove(itemToRemove);
            }
        }

        public void CreateResource(string key, string username)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            using (var db = GetDatabaseContext())
            {
                var existingResource = db.LocalizationResources.FirstOrDefault(r => r.ResourceKey == key);

                if (existingResource != null)
                {
                    throw new InvalidOperationException($"Resource with key `{key}` already exists");
                }

                db.LocalizationResources.Add(new LocalizationResource(key)
                                             {
                                                 ModificationDate = DateTime.UtcNow,
                                                 Author = username
                                             });
                db.SaveChanges();
            }
        }

        public void DeleteResource(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            using (var db = GetDatabaseContext())
            {
                var existingResource = db.LocalizationResources.FirstOrDefault(r => r.ResourceKey == key);

                if (existingResource != null)
                {
                    db.LocalizationResources.Remove(existingResource);
                    db.SaveChanges();
                }
            }

            CacheManager.Remove(BuildCacheKey(key));
        }
    }
}

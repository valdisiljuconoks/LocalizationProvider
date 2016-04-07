using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using EPiServer;
using EPiServer.Framework.Localization;

namespace DbLocalizationProvider
{
    public class CachedLocalizationResourceRepository : ILocalizationResourceRepository
    {
        private const string CacheKeyPrefix = "DbLocalizationProviderCache";
        private const string DbLocalizationProviderNoMatchList = CacheKeyPrefix + "_NoMatchList";
        private readonly LocalizationResourceRepository _repository;

        public CachedLocalizationResourceRepository(LocalizationResourceRepository repository)
        {
            if(repository == null)
            {
                throw new ArgumentNullException(nameof(repository));
            }

            _repository = repository;
        }

        public string GetTranslation(string key, CultureInfo language)
        {
            var cacheKey = BuildCacheKey(key);
            var localizationResource = GetFromCache(cacheKey);
            if (localizationResource != null)
            {

                return localizationResource.Translations.FirstOrDefault(t => t.Language == language.Name)?.Value;
            }

            var resource = _repository.GetResource(key);
            if (resource == null)
            {
                RegisterNotFoundResource(cacheKey);
                return null;
            }

            CacheManager.Insert(cacheKey, resource);
            var localization = resource.Translations.FirstOrDefault(t => t.Language == language.Name);

            return localization?.Value;
        }

        
        public IEnumerable<CultureInfo> GetAvailableLanguages()
        {
            var cacheKey = BuildCacheKey("AvailableLanguages");
            var cachedLanguages = CacheManager.Get(cacheKey) as IEnumerable<CultureInfo>;
            if (cachedLanguages != null)
            {
                return cachedLanguages;
            }

            var languages = _repository.GetAvailableLanguages();
            CacheManager.Insert(cacheKey, languages);

            return languages;
        }

        public IEnumerable<LocalizationResource> GetAllResources()
        {
            return _repository.GetAllResources();
        }

        public IEnumerable<ResourceItem> GetAllTranslations(string key, CultureInfo language)
        {
            return _repository.GetAllTranslations(key, language);
        }

        public void CreateResource(string key, string username)
        {
            _repository.CreateResource(key, username);
            RemoveFromNotFoundList(BuildCacheKey(key));
        }

        public void DeleteResource(string key)
        {
            _repository.DeleteResource(key);
            CacheManager.Remove(BuildCacheKey(key));
        }

        public void CreateOrUpdateTranslation(string key, CultureInfo language, string newValue)
        {
            _repository.CreateOrUpdateTranslation(key, language, newValue);
            CacheManager.Remove(BuildCacheKey(key));
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

        internal void PopulateCache()
        {
            var allResources = GetAllResources();

            ClearCache();

            foreach (var resource in allResources)
            {
                var key = BuildCacheKey(resource.ResourceKey);
                CacheManager.Insert(key, resource);
            }
        }

        internal LanguageEntities GetDatabaseContext()
        {
            return _repository.GetDatabaseContext();
        }

        private static string BuildCacheKey(string key)
        {
            return $"{CacheKeyPrefix}_{key}";
        }

        private static LocalizationResource GetFromCache(string cacheKey)
        {
            var noMatchList = CacheManager.Get(DbLocalizationProviderNoMatchList) as ConcurrentDictionary<string, byte>;
            if(noMatchList != null)
            {
                if(noMatchList.ContainsKey(cacheKey))
                {
                    return null;
                }
            }

            var cachedResource = CacheManager.Get(cacheKey);
            return cachedResource as LocalizationResource;
        }

        private void RegisterNotFoundResource(string cacheKey)
        {
            var noMatchList = CacheManager.Get(DbLocalizationProviderNoMatchList) as ConcurrentDictionary<string, byte>;
            if(noMatchList == null)
            {
                noMatchList = new ConcurrentDictionary<string, byte>();
                CacheManager.Insert(DbLocalizationProviderNoMatchList, noMatchList);
            }

            noMatchList.TryAdd(cacheKey, default(byte));
        }

        private void RemoveFromNotFoundList(string cacheKey)
        {
            var noMatchList = CacheManager.Get(DbLocalizationProviderNoMatchList) as ConcurrentDictionary<string, byte>;
            byte oldValue;
            noMatchList?.TryRemove(cacheKey, out oldValue);
        }
    }
}

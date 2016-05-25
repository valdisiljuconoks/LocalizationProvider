using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using DbLocalizationProvider.Cache;

namespace DbLocalizationProvider
{
    public class CachedLocalizationResourceRepository : ILocalizationResourceRepository
    {
        private const string CacheKeyPrefix = "DbLocalizationProviderCache";
        private readonly ICacheManager _cacheManager;
        private readonly LocalizationResourceRepository _repository;

        public CachedLocalizationResourceRepository(LocalizationResourceRepository repository) : this(repository, new HttpCacheManager()) { }

        public CachedLocalizationResourceRepository(LocalizationResourceRepository repository, ICacheManager cacheManager)
        {
            if(repository == null)
                throw new ArgumentNullException(nameof(repository));

            if(cacheManager == null)
                throw new ArgumentNullException(nameof(cacheManager));

            _repository = repository;
            _cacheManager = cacheManager;
        }

        public string GetTranslation(string key, CultureInfo language)
        {
            var cacheKey = BuildCacheKey(key);
            var localizationResource = GetFromCache(cacheKey);
            if(localizationResource != null)
            {
                // if value for the cache key is null - this is non-existing resource (no hit)
                return localizationResource.Translations?.FirstOrDefault(t => t.Language == language.Name)?.Value;
            }

            var resource = _repository.GetResource(key);
            LocalizationResourceTranslation localization = null;
            if(resource == null)
            {
                // create empty null resource 0 to indicate non-existing resource
                resource = LocalizationResource.CreateNonExisting(key);
            }
            else
            {
                localization = resource.Translations.FirstOrDefault(t => t.Language == language.Name);
            }

            _cacheManager.Insert(cacheKey, resource);
            return localization?.Value;
        }

        public IEnumerable<CultureInfo> GetAvailableLanguages()
        {
            var cacheKey = BuildCacheKey("AvailableLanguages");
            var cachedLanguages = _cacheManager.Get(cacheKey) as IEnumerable<CultureInfo>;
            if(cachedLanguages != null)
            {
                return cachedLanguages;
            }

            var languages = _repository.GetAvailableLanguages();
            _cacheManager.Insert(cacheKey, languages);

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

        public void CreateResource(string key, string username, bool fromCode = true)
        {
            _repository.CreateResource(key, username, fromCode: fromCode);
        }

        public void DeleteResource(string key)
        {
            _repository.DeleteResource(key);
            _cacheManager.Remove(BuildCacheKey(key));
        }

        public void CreateOrUpdateTranslation(string key, CultureInfo language, string newValue)
        {
            _repository.CreateOrUpdateTranslation(key, language, newValue);
            _cacheManager.Remove(BuildCacheKey(key));
        }

        public void ClearCache()
        {
            if(HttpContext.Current == null)
            {
                return;
            }

            if(HttpContext.Current.Cache == null)
            {
                return;
            }

            var itemsToRemove = new List<string>();
            var enumerator = HttpContext.Current.Cache.GetEnumerator();

            while (enumerator.MoveNext())
            {
                if(enumerator.Key.ToString().ToLower().StartsWith(CacheKeyPrefix.ToLower()))
                {
                    itemsToRemove.Add(enumerator.Key.ToString());
                }
            }

            foreach (var itemToRemove in itemsToRemove)
            {
                _cacheManager.Remove(itemToRemove);
            }
        }

        internal void PopulateCache()
        {
            var allResources = GetAllResources();

            ClearCache();

            foreach (var resource in allResources)
            {
                var key = BuildCacheKey(resource.ResourceKey);
                _cacheManager.Insert(key, resource);
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

        private LocalizationResource GetFromCache(string cacheKey)
        {
            var cachedResource = _cacheManager.Get(cacheKey);
            return cachedResource as LocalizationResource;
        }
    }
}

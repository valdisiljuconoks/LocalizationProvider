using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Cache;
using Microsoft.Extensions.Options;

namespace DbLocalizationProvider.Queries;

public class ResourceService : IResourceService
{
    private readonly IOptions<ConfigurationContext> _configurationContext;
    private readonly IResourceRepository _resourceRepository;

    public ResourceService(IOptions<ConfigurationContext> configurationContext, IResourceRepository resourceRepository)
    {
        _configurationContext = configurationContext;
        _resourceRepository = resourceRepository;
    }

    public CultureInfo GetCurrentCulture() =>
        CultureInfo.CurrentUICulture;

    public ICollection<AvailableLanguage> GetAvailableLanguages(bool includeInvariant = false)
    {
        var cacheKey = CacheKeyHelper.BuildKey($"DbLocalizationProviderCache:AvailableLanguages:{includeInvariant}");
        if (_configurationContext.Value.CacheManager.Get(cacheKey) is ICollection<AvailableLanguage> languages)
        {
            return languages;
        }
        
        languages = _resourceRepository
            .GetAvailableLanguages(includeInvariant)
            .Select((l, ix) => new AvailableLanguage(l.EnglishName, ix, l))
            .ToArray();
        
        _configurationContext.Value.CacheManager.Insert(cacheKey, languages, false);

        return languages;
    }

    public IDictionary<string, LocalizationResource> GetAllResources()
    {
        return _resourceRepository
            .GetAll()
            .ToDictionary(r => r.ResourceKey, r => r, StringComparer.Ordinal);
    }

    public LocalizationResource? GetResource(string key)
    {
        ArgumentException.ThrowIfNullOrEmpty(key);

        return _resourceRepository.GetByKey(key);
    }

    public IEnumerable<LocalizationResource> GetResources() =>
        _resourceRepository.GetAll();

    public string GetTranslation(string key, CultureInfo culture, bool fallbackToInvariant = false)
    {
        if (!_configurationContext.Value.EnableLocalization())
        {
            return key;
        }
        
        if (_configurationContext.Value._baseCacheManager.AreKnownKeysStored() &&
            !_configurationContext.Value._baseCacheManager.IsKeyKnown(key))
        {
            // we are here because of a couple of reasons:
            //  * someone is asking for non-existing resource (known keys are synced and key does not exist)
            //  * someone has programmatically created resource and query is made on different cluster node (cache is still cold for this resource)
            //
            // if this resource is not yet found in cache
            // we can try to lookup resource once more in database and if not found - then we can short-break the circuit

            // GetCachedResourceOrReadFromStorage(query);
        }

        throw new NotImplementedException();
    }
}

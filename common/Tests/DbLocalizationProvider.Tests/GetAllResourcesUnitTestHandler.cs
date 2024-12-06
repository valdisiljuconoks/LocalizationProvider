using System.Collections.Generic;
using System.Linq;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Queries;

namespace DbLocalizationProvider.Tests;

public class GetAllResourcesUnitTestHandler : IQueryHandler<GetAllResources.Query, Dictionary<string, LocalizationResource>>
{
    private readonly List<LocalizationResource> _resources;

    public GetAllResourcesUnitTestHandler(IEnumerable<LocalizationResource> resources)
    {
        _resources = resources.ToList();
    }

    public GetAllResourcesUnitTestHandler(IEnumerable<DiscoveredResource> discoveredResources)
    {
        _resources = new List<LocalizationResource>();
        foreach (var discoveredResource in discoveredResources)
        {
            var translations = new LocalizationResourceTranslationCollection(true);

            foreach (var translation in discoveredResource.Translations)
            {
                translations.Add(new LocalizationResourceTranslation
                {
                    Language = translation.Culture, Value = translation.Translation
                });
            }

            _resources.Add(new LocalizationResource(discoveredResource.Key, true)
            {
                ResourceKey = discoveredResource.Key, Translations = translations
            });
        }
    }

    public Dictionary<string, LocalizationResource> Execute(GetAllResources.Query query)
    {
        return _resources.ToDictionary(r => r.ResourceKey, r => r);
    }
}

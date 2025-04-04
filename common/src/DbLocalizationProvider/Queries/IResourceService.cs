using System.Collections.Generic;
using System.Globalization;
using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Queries;

public interface IResourceService
{
    public CultureInfo GetCurrentCulture();
    public ICollection<AvailableLanguage> GetAvailableLanguages(bool includeInvariant = false);
    public IDictionary<string, LocalizationResource> GetAllResources();
    public LocalizationResource? GetResource(string key);
    public IEnumerable<LocalizationResource> GetResources();
    public string GetTranslation(string key, CultureInfo culture, bool fallbackToInvariant = false);
}

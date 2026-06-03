using System.Collections.Generic;
using System.Linq;
using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider;

/// <summary>
/// No one reads this
/// </summary>
public static class ICollectionOfDiscoveredTranslationExtensions
{
    /// <summary>
    /// Returns invariant translation for invariant culture
    /// </summary>
    /// <param name="translations">List of discovered translations</param>
    /// <returns>Invariant translation for invariant culture</returns>
    public static DiscoveredTranslation? InvariantTranslation(this ICollection<DiscoveredTranslation>? translations)
    {
        return translations?.FirstOrDefault(t => t.Culture == string.Empty);
    }
}

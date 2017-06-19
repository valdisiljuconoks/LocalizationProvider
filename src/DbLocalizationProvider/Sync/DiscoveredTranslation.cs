using System.Collections.Generic;
using System.Linq;
using DbLocalizationProvider.Queries;

namespace DbLocalizationProvider.Sync
{
    public static class ListOfDiscoveredTranslationExtensions
    {
        public static string DefaultTranslation(this List<DiscoveredTranslation> target)
        {
            return target.FirstOrDefault(t => !string.IsNullOrEmpty(t.Culture))?.Translation;
        }
    }

    public class DiscoveredTranslation
    {
        public DiscoveredTranslation(string translation, string culture)
        {
            Translation = translation;
            Culture = culture;
        }

        public string Translation { get; }

        public string Culture { get; }

        public static List<DiscoveredTranslation> FromSingle(string translation)
        {
            var defaultCulture = new DetermineDefaultCulture.Query().Execute();

            return new List<DiscoveredTranslation>
                   {
                       // invariant translation
                       new DiscoveredTranslation(translation, ConfigurationContext.CultureForTranslationsFromCode),

                       // default translation for default culture
                       new DiscoveredTranslation(translation, defaultCulture)
                   };
        }
    }
}

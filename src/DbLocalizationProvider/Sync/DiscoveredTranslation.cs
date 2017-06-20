using System.Collections.Generic;
using System.Diagnostics;
using DbLocalizationProvider.Queries;

namespace DbLocalizationProvider.Sync
{
    [DebuggerDisplay("Translation: {Translation} / Culture: {Culture}")]
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

using System.Collections.Generic;
using System.Linq;

namespace DbLocalizationProvider.Sync {
    public static class ListOfDiscoveredTranslationExtensions
    {
        public static string DefaultTranslation(this List<DiscoveredTranslation> target)
        {
            return target.FirstOrDefault(t => !string.IsNullOrEmpty(t.Culture))?.Translation;
        }
    }
}
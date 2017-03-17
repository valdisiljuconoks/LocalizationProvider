using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace DbLocalizationProvider
{
    public static class TranslationsExtensions
    {
        public static string ByLanguage(this ICollection<LocalizationResourceTranslation> translations, CultureInfo language)
        {
            return ByLanguage(translations, language.Name);
        }

        public static bool ExistsLanguage(this ICollection<LocalizationResourceTranslation> translations, string language)
        {
            if(string.IsNullOrEmpty(language))
                throw new ArgumentNullException(nameof(language));

            return translations?.FirstOrDefault(t => t.Language == language) != null;
        }

        public static string ByLanguage(this ICollection<LocalizationResourceTranslation> translations, string language)
        {
            if(translations == null)
                return string.Empty;

            if(string.IsNullOrEmpty(language))
                throw new ArgumentNullException(nameof(language));

            var translation = translations.FirstOrDefault(t => t.Language == language);
            return translation != null ? translation.Value : string.Empty;
        }
    }
}

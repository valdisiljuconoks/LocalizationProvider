// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace DbLocalizationProvider
{
    public static class TranslationsExtensions
    {
        public static LocalizationResourceTranslation FindByLanguage(this ICollection<LocalizationResourceTranslation> translations, CultureInfo language)
        {
            return FindByLanguage(translations, language.Name);
        }

        public static LocalizationResourceTranslation InvariantTranslation(this ICollection<LocalizationResourceTranslation> translations)
        {
            return FindByLanguage(translations, CultureInfo.InvariantCulture);
        }

        public static LocalizationResourceTranslation FindByLanguage(this ICollection<LocalizationResourceTranslation> translations, string language)
        {
            return translations?.FirstOrDefault(t => t.Language == language);
        }

        public static string ByLanguage(this ICollection<LocalizationResourceTranslation> translations, CultureInfo language)
        {
            return ByLanguage(translations, language.Name);
        }

        public static string ByLanguage(this ICollection<LocalizationResourceTranslation> translations, CultureInfo language, bool invariantCultureFallback)
        {
            return ByLanguage(translations, language.Name, invariantCultureFallback);
        }

        public static string ByLanguage(this ICollection<LocalizationResourceTranslation> translations, string language)
        {
            return ByLanguage(translations, language, ConfigurationContext.Current.EnableInvariantCultureFallback);
        }

        public static string ByLanguage(this ICollection<LocalizationResourceTranslation> translations, string language, bool invariantCultureFallback)
        {
            if (translations == null) return string.Empty;
            if (language == null) throw new ArgumentNullException(nameof(language));

            var translation = translations.FindByLanguage(language);

            return translation != null ? translation.Value :
                   invariantCultureFallback ? translations.FindByLanguage(string.Empty)?.Value : string.Empty;
        }

        public static bool ExistsLanguage(this ICollection<LocalizationResourceTranslation> translations, string language)
        {
            if (language == null) throw new ArgumentNullException(nameof(language));

            return translations?.FirstOrDefault(t => t.Language == language) != null;
        }
    }
}

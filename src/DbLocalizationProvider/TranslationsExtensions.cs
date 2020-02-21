// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace DbLocalizationProvider
{
    /// <summary>
    /// Some static class
    /// </summary>
    public static class TranslationsExtensions
    {
        /// <summary>
        /// Finds translation the by language.
        /// </summary>
        /// <param name="translations">The translations.</param>
        /// <param name="language">The language.</param>
        /// <returns>Translation class</returns>
        public static LocalizationResourceTranslation FindByLanguage(this ICollection<LocalizationResourceTranslation> translations, CultureInfo language)
        {
            return FindByLanguage(translations, language.Name);
        }

        /// <summary>
        /// Find translation in invariant culture.
        /// </summary>
        /// <param name="translations">The translations.</param>
        /// <returns>Translation class</returns>
        public static LocalizationResourceTranslation InvariantTranslation(this ICollection<LocalizationResourceTranslation> translations)
        {
            return FindByLanguage(translations, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Finds translation by language.
        /// </summary>
        /// <param name="translations">The translations.</param>
        /// <param name="language">The language.</param>
        /// <returns>Translation class</returns>
        public static LocalizationResourceTranslation FindByLanguage(this ICollection<LocalizationResourceTranslation> translations, string language)
        {
            return translations?.FirstOrDefault(t => t.Language == language);
        }

        /// <summary>
        /// Finds translation by language.
        /// </summary>
        /// <param name="translations">The translations.</param>
        /// <param name="language">The language.</param>
        /// <returns>Translation class</returns>
        public static string ByLanguage(this ICollection<LocalizationResourceTranslation> translations, CultureInfo language)
        {
            return ByLanguage(translations, language.Name);
        }

        /// <summary>
        /// Finds translation by language.
        /// </summary>
        /// <param name="translations">The translations.</param>
        /// <param name="language">The language.</param>
        /// <param name="invariantCultureFallback">if set to <c>true</c> invariant culture fallback is used.</param>
        /// <returns>Translation class</returns>
        public static string ByLanguage(this ICollection<LocalizationResourceTranslation> translations, CultureInfo language, bool invariantCultureFallback)
        {
            return ByLanguage(translations, language.Name, invariantCultureFallback);
        }

        /// <summary>
        /// Finds translation by language.
        /// </summary>
        /// <param name="translations">The translations.</param>
        /// <param name="language">The language.</param>
        /// <returns>Translation class</returns>
        public static string ByLanguage(this ICollection<LocalizationResourceTranslation> translations, string language)
        {
            return ByLanguage(translations, language, ConfigurationContext.Current.EnableInvariantCultureFallback);
        }

        /// <summary>
        /// Finds translation by language.
        /// </summary>
        /// <param name="translations">The translations.</param>
        /// <param name="language">The language.</param>
        /// <param name="invariantCultureFallback">if set to <c>true</c> [invariant culture fallback].</param>
        /// <returns>Translation class</returns>
        /// <exception cref="ArgumentNullException">language</exception>
        public static string ByLanguage(this ICollection<LocalizationResourceTranslation> translations, string language, bool invariantCultureFallback)
        {
            if (translations == null) return string.Empty;
            if (language == null) throw new ArgumentNullException(nameof(language));

            var translation = translations.FindByLanguage(language);

            return translation != null ? translation.Value :
                   invariantCultureFallback ? translations.FindByLanguage(string.Empty)?.Value : string.Empty;
        }

        /// <summary>
        /// Checks whether translation exists in given language.
        /// </summary>
        /// <param name="translations">The translations.</param>
        /// <param name="language">The language.</param>
        /// <returns><c>true</c> is translation exists; otherwise <c>false</c></returns>
        /// <exception cref="ArgumentNullException">language</exception>
        public static bool ExistsLanguage(this ICollection<LocalizationResourceTranslation> translations, string language)
        {
            if (language == null) throw new ArgumentNullException(nameof(language));

            return translations?.FirstOrDefault(t => t.Language == language) != null;
        }
    }
}

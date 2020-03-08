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

        /// <summary>
        /// Get translation in given language or in any of fallback languages
        /// </summary>
        /// <param name="translations">target</param>
        /// <param name="language">Language in which to get translation first</param>
        /// <param name="fallbackLanguages">If translation does not exist in language supplied by parameter <paramref name="language"/> then this list of fallback languages is used to find translation</param>
        /// <returns>Translation in requested language or uin any fallback languages; <c>null</c> otherwise if translation is not found</returns>
        public static string GetValueWithFallback(this ICollection<LocalizationResourceTranslation> translations,
            CultureInfo language,
            IReadOnlyCollection<CultureInfo> fallbackLanguages)
        {
            return GetValueWithFallback(translations, language.Name, fallbackLanguages);
        }

        /// <summary>
        /// Get translation in given language or in any of fallback languages
        /// </summary>
        /// <param name="translations">target</param>
        /// <param name="language">Language in which to get translation first</param>
        /// <param name="fallbackLanguages">If translation does not exist in language supplied by parameter <paramref name="language"/> then this list of fallback languages is used to find translation</param>
        /// <returns>Translation in requested language or uin any fallback languages; <c>null</c> otherwise if translation is not found</returns>
        public static string GetValueWithFallback(this ICollection<LocalizationResourceTranslation> translations,
            string language,
            IReadOnlyCollection<CultureInfo> fallbackLanguages)
        {
            if (translations == null) return null;
            if (language == null) throw new ArgumentNullException(nameof(language));
            if (fallbackLanguages == null) throw new ArgumentNullException(nameof(fallbackLanguages));

            var inRequestedLanguage = FindByLanguage(translations, language);
            if (inRequestedLanguage != null) return inRequestedLanguage.Value;

            // find if requested language is not "inside" fallback languages
            var culture = new CultureInfo(language);
            var searchableLanguages = fallbackLanguages.ToList();

            if (fallbackLanguages.Contains(culture))
            {
                // requested language is inside fallback languages, so we need to "continue" from there
                var restOfFallbackLanguages = fallbackLanguages.SkipWhile(c => !Equals(c, culture)).ToList();

                // check if we are not at the end of the list
                if (restOfFallbackLanguages.Any())
                {
                    // if there are still elements - we have to skip 1 (as this is requested language)
                    searchableLanguages = restOfFallbackLanguages.Skip(1).ToList();
                }
            }

            foreach (var fallbackLanguage in searchableLanguages)
            {
                var translationInFallback = FindByLanguage(translations, fallbackLanguage);
                if (translationInFallback != null) return translationInFallback.Value;
            }

            return null;
        }
    }
}

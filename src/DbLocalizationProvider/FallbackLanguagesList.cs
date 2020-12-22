using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using DbLocalizationProvider.Internal;

namespace DbLocalizationProvider
{
    /// <summary>
    /// List of fallback languages.
    /// </summary>
    public class FallbackLanguagesList : IReadOnlyCollection<CultureInfo>
    {
        private readonly List<CultureInfo> _fallbackLanguages = new List<CultureInfo>();

        /// <inheritdoc />
        public IEnumerator<CultureInfo> GetEnumerator()
        {
            return _fallbackLanguages.GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <inheritdoc />
        public int Count => _fallbackLanguages.Count;

        /// <summary>
        /// Registers fallback language.
        /// </summary>
        /// <param name="fallbackLanguage">The fallback language.</param>
        /// <returns>The same list to support chaining</returns>
        /// <exception cref="ArgumentNullException">fallbackLanguage</exception>
        public FallbackLanguagesList Try(CultureInfo fallbackLanguage)
        {
            if (fallbackLanguage == null)
            {
                throw new ArgumentNullException(nameof(fallbackLanguage));
            }

            _fallbackLanguages.Add(fallbackLanguage);

            return this;
        }

        /// <summary>
        /// Add new language to the list of fallback languages.
        /// </summary>
        /// <param name="language">Language to add.</param>
        /// <returns>The same list so you can do fluent stuff.</returns>
        public FallbackLanguagesList Add(CultureInfo language)
        {
            _fallbackLanguages.Add(language);

            return this;
        }

        /// <summary>
        /// Registers fallback languages.
        /// </summary>
        /// <param name="fallbackLanguages">The fallback languages.</param>
        /// <returns>The same list of registered fallback languages to support API chaining (that fluent thingy).</returns>
        /// <exception cref="ArgumentNullException">fallbackLanguages</exception>
        public FallbackLanguagesList Try(IList<CultureInfo> fallbackLanguages)
        {
            if (fallbackLanguages == null)
            {
                throw new ArgumentNullException(nameof(fallbackLanguages));
            }

            fallbackLanguages.ForEach(_fallbackLanguages.Add);

            return this;
        }

        /// <summary>
        /// Registered specified fallback language.
        /// </summary>
        /// <param name="fallbackLanguage">The fallback language.</param>
        /// <returns>The same list of registered fallback languages to support API chaining (that fluent thingy).</returns>
        public FallbackLanguagesList Then(CultureInfo fallbackLanguage)
        {
            return Try(fallbackLanguage);
        }
    }

    /// <summary>
    /// Extensions for language fallback list.
    /// </summary>
    public static class FallbackLanguagesListExtensions
    {
        /// <summary>
        /// Extension method to use to configure fallback languages. Use this method when you want to specify which languages to use when <paramref name="notFoundCulture"/> language was not found.
        /// </summary>
        /// <param name="list">List of fallback languages.</param>
        /// <param name="notFoundCulture">Configure fallback languages for this language.</param>
        /// <returns>The same list of registered fallback languages to support API chaining (that fluent thingy).</returns>
        public static FallbackLanguagesList When(this FallbackLanguagesList list, CultureInfo notFoundCulture)
        {
            if (notFoundCulture == null)
            {
                throw new ArgumentNullException(nameof(notFoundCulture));
            }

            if (ConfigurationContext.Current.FallbackList.ContainsKey(notFoundCulture.Name))
            {
                throw new ArgumentException($"Fallback languages already have setting for `{notFoundCulture.Name}` language");
            }

            var newList = new FallbackLanguagesList();
            ConfigurationContext.Current.FallbackList.Add(notFoundCulture.Name, newList);

            return newList;
        }

        /// <summary>
        /// Get list of fallback languages configured for <paramref name="language"/>.
        /// </summary>
        /// <param name="language">Language to get fallback languages for.</param>
        /// <returns>The list of registered fallback languages for given <paramref name="language"/>.</returns>
        public static FallbackLanguagesList GetFallbackLanguageList(this CultureInfo language)
        {
            if (language == null)
            {
                throw new ArgumentNullException(nameof(language));
            }

            return language.Name.GetFallbackLanguageList();
        }

        /// <summary>
        /// Get list of fallback languages configured for <paramref name="language"/>.
        /// </summary>
        /// <param name="language">Language to get fallback languages for.</param>
        /// <returns>The list of registered fallback languages for given <paramref name="language"/>.</returns>
        public static FallbackLanguagesList GetFallbackLanguageList(this string language)
        {
            return GetFallbackLanguageList(language, ConfigurationContext.Current.FallbackList);
        }

        /// <summary>
        /// Get list of fallback languages configured for <paramref name="language"/>.
        /// </summary>
        /// <param name="language">Language to get fallback languages for.</param>
        /// <param name="fallbackList">List of fallback languages.</param>
        /// <returns>The list of registered fallback languages for given <paramref name="language"/>.</returns>
        public static FallbackLanguagesList GetFallbackLanguageList(
            this string language,
            Dictionary<string, FallbackLanguagesList> fallbackList)
        {
            if (language == null)
            {
                throw new ArgumentNullException(nameof(language));
            }

            return !fallbackList.ContainsKey(language)
                ? fallbackList["default"]
                : fallbackList[language];
        }
    }
}

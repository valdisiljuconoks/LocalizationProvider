using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using DbLocalizationProvider.Internal;

namespace DbLocalizationProvider
{
    public class FallbackLanguagesList : IReadOnlyCollection<CultureInfo>
    {
        private readonly List<CultureInfo> _fallbackLanguages = new List<CultureInfo>();

        /// <inheritdoc />
        public IEnumerator<CultureInfo> GetEnumerator()
        {
            return _fallbackLanguages.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

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

        public FallbackLanguagesList Add(CultureInfo language)
        {
            _fallbackLanguages.Add(language);

            return this;
        }

        /// <summary>
        /// Registers fallback languages.
        /// </summary>
        /// <param name="fallbackLanguages">The fallback languages.</param>
        /// <returns>The same list of registered fallback languages to support API chaining</returns>
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
        /// <param name="list">The list of fallback languages registered so far.</param>
        /// <param name="fallbackLanguage">The fallback language.</param>
        /// <returns>The same list of registered fallback languages to support API chaining</returns>
        public FallbackLanguagesList Then(CultureInfo fallbackLanguage)
        {
            return Try(fallbackLanguage);
        }
    }

    public static class FallbackLanguagesListExtensions
    {
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

        public static FallbackLanguagesList GetFallbackLanguageList(this CultureInfo language)
        {
            if (language == null)
            {
                throw new ArgumentNullException(nameof(language));
            }

            return language.Name.GetFallbackLanguageList();
        }

        public static FallbackLanguagesList GetFallbackLanguageList(this string language)
        {
            return GetFallbackLanguageList(language, ConfigurationContext.Current.FallbackList);
        }

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

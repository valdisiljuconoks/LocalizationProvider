// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using System.Globalization;
using DbLocalizationProvider.Internal;

namespace DbLocalizationProvider
{
    /// <summary>
    /// WHAT?
    /// </summary>
    public static class ReadOnlyListOfCultureInfoExtensions
    {
        /// <summary>
        /// Registers fallback language.
        /// </summary>
        /// <param name="list">The list of fallback languages registered so far.</param>
        /// <param name="fallbackLanguage">The fallback language.</param>
        /// <returns>The same list to support chaining</returns>
        /// <exception cref="ArgumentNullException">fallbackLanguage</exception>
        public static IReadOnlyCollection<CultureInfo> Try(this IReadOnlyCollection<CultureInfo> list, CultureInfo fallbackLanguage)
        {
            if (fallbackLanguage == null) throw new ArgumentNullException(nameof(fallbackLanguage));

            ConfigurationContext.Current.FallbackCulturesList.Add(fallbackLanguage);

            return list;
        }

        /// <summary>
        /// Registers fallback languages.
        /// </summary>
        /// <param name="list">The list of fallback languages registered so far.</param>
        /// <param name="fallbackLanguages">The fallback languages.</param>
        /// <returns>The same list of registered fallback languages to support API chaining</returns>
        /// <exception cref="ArgumentNullException">fallbackLanguages</exception>
        public static IReadOnlyCollection<CultureInfo> Try(this IReadOnlyCollection<CultureInfo> list, IList<CultureInfo> fallbackLanguages)
        {
            if (fallbackLanguages == null) throw new ArgumentNullException(nameof(fallbackLanguages));

            fallbackLanguages.ForEach(ConfigurationContext.Current.FallbackCulturesList.Add);

            return list;
        }

        /// <summary>
        /// Registered specified fallback language.
        /// </summary>
        /// <param name="list">The list of fallback languages registered so far.</param>
        /// <param name="fallbackLanguage">The fallback language.</param>
        /// <returns>The same list of registered fallback languages to support API chaining</returns>
        public static IReadOnlyCollection<CultureInfo> Then(this IReadOnlyCollection<CultureInfo> list, CultureInfo fallbackLanguage)
        {
            return list.Try(fallbackLanguage);
        }
    }
}

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
    public static class ListOfCultureInfoExtensions
    {
        /// <summary>
        /// Registers fallback language.
        /// </summary>
        /// <param name="list">The list of fallback languages regsitered so far.</param>
        /// <param name="fallbackLanguage">The fallback language.</param>
        /// <returns>The same list to support chaining</returns>
        /// <exception cref="ArgumentNullException">fallbackLanguage</exception>
        public static List<CultureInfo> Try(this List<CultureInfo> list, CultureInfo fallbackLanguage)
        {
            if (fallbackLanguage == null) throw new ArgumentNullException(nameof(fallbackLanguage));

            list.Add(fallbackLanguage);

            return list;
        }

        /// <summary>
        /// Registers fallback languages.
        /// </summary>
        /// <param name="list">The list of fallback languages registered so far.</param>
        /// <param name="fallbackLanguages">The fallback languages.</param>
        /// <returns>The same list of registered fallback languages to support API chaining</returns>
        /// <exception cref="ArgumentNullException">fallbackLanguages</exception>
        public static List<CultureInfo> Try(this List<CultureInfo> list, IList<CultureInfo> fallbackLanguages)
        {
            if (fallbackLanguages == null) throw new ArgumentNullException(nameof(fallbackLanguages));

            fallbackLanguages.ForEach(list.Add);

            return list;
        }

        /// <summary>
        /// Registered specified fallback language.
        /// </summary>
        /// <param name="list">The list of fallback languages registered so far.</param>
        /// <param name="fallbackLanguage">The fallback language.</param>
        /// <returns>The same list of registered fallback languages to support API chaining</returns>
        public static List<CultureInfo> Then(this List<CultureInfo> list, CultureInfo fallbackLanguage)
        {
            return list.Try(fallbackLanguage);
        }
    }
}

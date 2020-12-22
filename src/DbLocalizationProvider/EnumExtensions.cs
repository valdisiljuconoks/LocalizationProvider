// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Globalization;

namespace DbLocalizationProvider
{
    /// <summary>
    /// Summary for static extension classes? Who reads this anyways??
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Translates the specified enum with some formatting arguments (if needed).
        /// </summary>
        /// <param name="target">The enum to translate.</param>
        /// <param name="formatArguments">The format arguments.</param>
        /// <returns>Translated enum values</returns>
        public static string Translate(this Enum target, params object[] formatArguments)
        {
            return TranslateByCulture(target, CultureInfo.CurrentUICulture, formatArguments);
        }

        /// <summary>
        /// Translates the specified enum with some formatting arguments (if needed).
        /// </summary>
        /// <param name="target">The enum to translate.</param>
        /// <param name="culture">The culture.</param>
        /// <param name="formatArguments">The format arguments.</param>
        /// <returns>Translated enum values</returns>
        /// <exception cref="ArgumentNullException">
        /// target
        /// or
        /// culture
        /// </exception>
        public static string TranslateByCulture(this Enum target, CultureInfo culture, params object[] formatArguments)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (culture == null)
            {
                throw new ArgumentNullException(nameof(culture));
            }

            var resourceKey = ResourceKeyBuilder.BuildResourceKey(target.GetType(), target.ToString());

            return LocalizationProvider.Current.GetStringByCulture(resourceKey, culture, formatArguments);
        }
    }

    namespace DbLocalizationProvider.AspNetCore { }
}

// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Sync.Collectors
{
    /// <summary>
    ///     Helper for working with translations
    /// </summary>
    public static class TranslationsHelper
    {
        /// <summary>
        ///     Gets all translations.
        /// </summary>
        /// <param name="mi">The member info type to get resources from.</param>
        /// <param name="resourceKey">The resource key.</param>
        /// <param name="defaultTranslation">The default translation.</param>
        /// <returns>List of discovered resources</returns>
        /// <exception cref="DuplicateResourceTranslationsException">
        ///     Duplicate translations for the same culture for following
        ///     resource: `{resourceKey}`
        /// </exception>
        public static ICollection<DiscoveredTranslation> GetAllTranslations(MemberInfo mi, string resourceKey, string defaultTranslation)
        {
            var translations = DiscoveredTranslation.FromSingle(defaultTranslation);
            var additionalTranslations = mi.GetCustomAttributes<TranslationForCultureAttribute>().ToList();

            if (!additionalTranslations.Any()) return translations;

            if (additionalTranslations.GroupBy(t => t.Culture).Any(g => g.Count() > 1))
            {
                throw new DuplicateResourceTranslationsException(
                    $"Duplicate translations for the same culture for following resource: `{resourceKey}`");
            }

            additionalTranslations.ForEach(t =>
            {
                var existingTranslation = translations.FirstOrDefault(_ => _.Culture == t.Culture);
                if (existingTranslation != null) existingTranslation.Translation = t.Translation;
                else translations.Add(new DiscoveredTranslation(t.Translation, t.Culture));
            });

            return translations;
        }
    }
}

// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Internal;

namespace DbLocalizationProvider.Sync.Collectors
{
    public static class TranslationsHelper
    {
        public static ICollection<DiscoveredTranslation> GetAllTranslations(MemberInfo mi, string resourceKey, string defaultTranslation)
        {
            var translations = DiscoveredTranslation.FromSingle(defaultTranslation);
            var additionalTranslations = mi.GetCustomAttributes<TranslationForCultureAttribute>();
            if (additionalTranslations != null && additionalTranslations.Any())
            {
                if (additionalTranslations.GroupBy(t => t.Culture).Any(g => g.Count() > 1))  throw new DuplicateResourceTranslationsException($"Duplicate translations for the same culture for following resource: `{resourceKey}`");

                additionalTranslations.ForEach(t =>
                                               {
                                                   var existingTranslation = translations.FirstOrDefault(_ => _.Culture == t.Culture);
                                                   if (existingTranslation != null)  existingTranslation.Translation = t.Translation;
                                                   else  translations.Add(new DiscoveredTranslation(t.Translation, t.Culture));
                                               });
            }

            return translations;
        }
    }
}

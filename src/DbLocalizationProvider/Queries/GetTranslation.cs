// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Collections.Generic;
using System.Globalization;
using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Queries
{
    public class GetTranslation
    {
        public class Query : IQuery<string>
        {
            public Query(string key, CultureInfo language, bool useFallback)
            {
                Key = key;
                Language = language;
                UseFallback = useFallback;
            }

            public string Key { get; }

            public CultureInfo Language { get; }

            public bool UseFallback { get; }
        }

        public abstract class GetTranslationHandlerBase
        {
            protected virtual LocalizationResourceTranslation GetTranslationFromAvailableList(
                ICollection<LocalizationResourceTranslation> translations,
                CultureInfo language,
                bool queryUseFallback)
            {
                var foundTranslation = translations?.FindByLanguage(language);
                if (foundTranslation == null && queryUseFallback) return translations.InvariantTranslation();

                return foundTranslation;
            }

            protected virtual LocalizationResourceTranslation GetTranslationWithFallback(
                ICollection<LocalizationResourceTranslation> translations,
                CultureInfo language,
                List<CultureInfo> fallbackLanguages,
                bool queryUseFallback)
            {
                // explicitly turning invariant culture fallback off (for now)
                var foundTranslation = GetTranslationFromAvailableList(translations, language, false);

                if (foundTranslation == null)
                {
                    // do the fallback of the languages
                    foreach (var objFallbackCulture in fallbackLanguages)
                    {
                        var f2 = GetTranslationFromAvailableList(translations, objFallbackCulture, false);
                        if (f2 != null)
                        {
                            return f2;
                        }
                    }
                }

                if (foundTranslation == null && queryUseFallback)
                {
                    // explicitly return invariant culture translation now (as rest of the languages have no translation)
                    return translations.InvariantTranslation();
                }

                return foundTranslation;
            }
        }
    }
}

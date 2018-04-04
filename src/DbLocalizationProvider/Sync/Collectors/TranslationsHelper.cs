// Copyright (c) 2018 Valdis Iljuconoks.
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.

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
            if(additionalTranslations != null && additionalTranslations.Any())
            {
                if(additionalTranslations.GroupBy(t => t.Culture).Any(g => g.Count() > 1))
                    throw new DuplicateResourceTranslationsException($"Duplicate translations for the same culture for following resource: `{resourceKey}`");

                additionalTranslations.ForEach(t =>
                                               {
                                                   var existingTranslation = translations.FirstOrDefault(_ => _.Culture == t.Culture);
                                                   if(existingTranslation != null)
                                                       existingTranslation.Translation = t.Translation;
                                                   else
                                                       translations.Add(new DiscoveredTranslation(t.Translation, t.Culture));
                                               });
            }

            return translations;
        }
    }
}

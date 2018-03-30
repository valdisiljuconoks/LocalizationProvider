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

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Internal;

namespace DbLocalizationProvider.Sync
{
    internal class LocalizedEnumTypeScanner : IResourceTypeScanner
    {
        public bool ShouldScan(Type target)
        {
            return target.BaseType == typeof(Enum) && target.GetCustomAttribute<LocalizedResourceAttribute>() != null;
        }

        public string GetResourceKeyPrefix(Type target, string keyPrefix = null)
        {
            var resourceAttribute = target.GetCustomAttribute<LocalizedResourceAttribute>();

            return !string.IsNullOrEmpty(resourceAttribute?.KeyPrefix)
                       ? resourceAttribute.KeyPrefix
                       : (string.IsNullOrEmpty(keyPrefix) ? target.FullName : keyPrefix);
        }

        public ICollection<DiscoveredResource> GetClassLevelResources(Type target, string resourceKeyPrefix)
        {
            return Enumerable.Empty<DiscoveredResource>().ToList();
        }

        public ICollection<DiscoveredResource> GetResources(Type target, string resourceKeyPrefix)
        {
            var enumType = Enum.GetUnderlyingType(target);
            var isHidden = target.GetCustomAttribute<HiddenAttribute>() != null;

            string GetEnumTranslation(MemberInfo mi)
            {
                var result = mi.Name;
                var displayAttribute = mi.GetCustomAttribute<DisplayAttribute>();
                if(displayAttribute != null)
                    result = displayAttribute.Name;

                return result;
            }

            return target.GetMembers(BindingFlags.Public | BindingFlags.Static)
                         .Select(mi =>
                         {
                             var isResourceHidden = isHidden || mi.GetCustomAttribute<HiddenAttribute>() != null;
                             var resourceKey = ResourceKeyBuilder.BuildResourceKey(target, mi.Name);
                             var translations = DiscoveredTranslation.FromSingle(GetEnumTranslation(mi));
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

                             return new DiscoveredResource(mi,
                                                           resourceKey,
                                                           translations,
                                                           mi.Name,
                                                           target,
                                                           enumType,
                                                           enumType.IsSimpleType(),
                                                           isResourceHidden);
                         }).ToList();
        }
    }
}

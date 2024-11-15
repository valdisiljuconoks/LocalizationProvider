// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using System.Linq;

namespace DbLocalizationProvider.MigrationTool
{
    internal class ResourceListMerger
    {
        public ICollection<LocalizationResource> Merge(ICollection<LocalizationResource> list1, ICollection<LocalizationResource> list2)
        {
            return Merge(list1, list2, false);
        }

        public ICollection<LocalizationResource> Merge(ICollection<LocalizationResource> list1, ICollection<LocalizationResource> list2, bool ignoreDuplicateKeys)
        {
            if (list1 == null)
            {
                throw new ArgumentNullException(nameof(list1));
            }

            if (list2 == null)
            {
                throw new ArgumentNullException(nameof(list2));
            }

            if (!list1.Any())
            {
                return list2;
            }

            if (!list2.Any())
            {
                return list1;
            }

            var result = new List<LocalizationResource>(list1);

            foreach (var resourceItem in list2)
            {
                if (result.Any(r => string.Equals(r.ResourceKey, resourceItem.ResourceKey, StringComparison.InvariantCultureIgnoreCase)))
                {
                    // resource exists in target list - need to merge translations
                    var matchedResource = result.First(r => string.Equals(r.ResourceKey, resourceItem.ResourceKey, StringComparison.InvariantCultureIgnoreCase));
                    foreach (var resourceTranslation in resourceItem.Translations)
                    {
                        if (matchedResource.Translations.Any(t => t.Language == resourceTranslation.Language))
                        {
                            if (ignoreDuplicateKeys)
                            {
                                Console.WriteLine($"{matchedResource.ResourceKey} Additional resource with same key found. Ignored");
                            }
                            else
                            {
                                // there is already a translation in this culture - we can't resolve this conflict. blowing up
                                throw new NotSupportedException($"There are duplicate translations for resource '{matchedResource.ResourceKey}' in culture '{resourceTranslation.Language}'. Use -d to ignore duplicates");
                            }
                        }
                        else
                        {
                            matchedResource.Translations.Add(resourceTranslation);
                        }
                    }
                }
                else
                {
                    // add new resource to the list
                    result.Add(resourceItem);
                }
            }

            return result;
        }
    }
}

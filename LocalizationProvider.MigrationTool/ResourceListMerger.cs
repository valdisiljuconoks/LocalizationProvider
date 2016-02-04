using System;
using System.Collections.Generic;
using System.Linq;
using DbLocalizationProvider;

namespace TechFellow.LocalizationProvider.MigrationTool
{
    internal class ResourceListMerger
    {
        public ICollection<LocalizationResource> Merge(ICollection<LocalizationResource> list1, ICollection<LocalizationResource> list2)
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
                if (result.Any(r => r.ResourceKey == resourceItem.ResourceKey))
                {
                    // resource exists in target list - need to merge translations
                    var matchedResource = result.First(r => r.ResourceKey == resourceItem.ResourceKey);
                    foreach (var resourceTranslation in resourceItem.Translations)
                    {
                        if (matchedResource.Translations.Any(t => t.Language == resourceTranslation.Language))
                        {
                            // there is already a translation in this culture - we can't resolve this conflict. blowing up
                            throw new NotSupportedException($"There are duplicate translations for resource '{matchedResource.ResourceKey}' in culture '{resourceTranslation.Language}'");
                        }

                        matchedResource.Translations.Add(resourceTranslation);
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

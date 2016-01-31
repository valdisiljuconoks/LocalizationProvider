using System.Collections.Generic;
using System.Linq;

namespace DbLocalizationProvider.Import
{
    public class ResourceImporter
    {
        public object Import(IEnumerable<LocalizationResource> newResources, bool importOnlyNewContent)
        {
            using (var db = new LanguageEntities("EPiServerDB"))
            {
                var existingResources = db.Set<LocalizationResource>();
                db.LocalizationResources.RemoveRange(existingResources);
                db.SaveChanges();

                foreach (var localizationResource in newResources)
                {
                    db.LocalizationResources.Add(localizationResource);
                    db.LocalizationResourceTranslations.AddRange(localizationResource.Translations);
                }

                db.SaveChanges();
            }

            return null;
        }
    }
}
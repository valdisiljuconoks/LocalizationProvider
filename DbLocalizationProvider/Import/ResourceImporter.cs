using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace DbLocalizationProvider.Import
{
    public class ResourceImporter
    {
        public object Import(IEnumerable<LocalizationResource> newResources, bool importOnlyNewContent)
        {
            var count = 0;

            using (var db = new LanguageEntities("EPiServerDB"))
            {
                // if we are overwriting old content - we need to get rid of it first

                if (!importOnlyNewContent)
                {
                    var existingResources = db.Set<LocalizationResource>();
                    db.LocalizationResources.RemoveRange(existingResources);
                    db.SaveChanges();
                }

                foreach (var localizationResource in newResources)
                {
                    if (importOnlyNewContent)
                    {
                        // look for existing resource
                        var existingResource = db.LocalizationResources
                                                 .Include(r => r.Translations)
                                                 .FirstOrDefault(r => r.ResourceKey == localizationResource.ResourceKey);

                        if (existingResource == null)
                        {
                            // resource with this key does not exist - so we can just add it
                            AddNewResource(db, localizationResource);
                            count++;
                        }
                        else
                        {
                            // there is a resource with this key - looking for missing translations
                            foreach (var translation in localizationResource.Translations)
                            {
                                var existingTranslation = existingResource.Translations.FirstOrDefault(t => t.Language == translation.Language);

                                if (existingTranslation == null)
                                {
                                    // there is no translation in that language - adding one
                                    // but before adding that - we need to fix its reference to resource (exported file might have different id)
                                    translation.ResourceId = existingResource.Id;
                                    db.LocalizationResourceTranslations.Add(translation);
                                }
                                else if (string.IsNullOrEmpty(existingTranslation.Value))
                                {
                                    // we can check - if content of the translation is empty - for us - it's the same as translation would not exist
                                    existingTranslation.Value = translation.Value;
                                }
                            }
                        }
                    }
                    else
                    {
                        // don't care about state in DB
                        // if we are importing all resources once again - all will be gone anyway
                        AddNewResource(db, localizationResource);
                        count++;
                    }
                }

                db.SaveChanges();
            }

            return $"Import successful. Imported {count} resources";
        }

        private static void AddNewResource(LanguageEntities db, LocalizationResource localizationResource)
        {
            db.LocalizationResources.Add(localizationResource);
            db.LocalizationResourceTranslations.AddRange(localizationResource.Translations);
        }
    }
}

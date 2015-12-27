using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using EPiServer.Framework.Localization;

namespace TechFellow.DbLocalizationProvider
{
    public class DatabaseLocalizationProvider : LocalizationProvider
    {
        public override IEnumerable<CultureInfo> AvailableLanguages
        {
            get
            {
                using (var db = new LanguageEntities("EPiServerDB"))
                {
                    return db.LocalizationResourceTranslations
                             .Select(t => t.Language)
                             .Distinct()
                             .ToList()
                             .Select(l => new CultureInfo(l));
                }
            }
        }

        public override string GetString(string originalKey, string[] normalizedKey, CultureInfo culture)
        {
            // TODO: add localization disabled fallback
            using (var db = new LanguageEntities("EPiServerDB"))
            {
                var resource = db.LocalizationResources
                                 .Include(r => r.Translations)
                                 .FirstOrDefault(r => r.ResourceKey == originalKey);

                if (resource == null)
                {
                    return null;
                }

                var localization = resource.Translations.FirstOrDefault(t => t.Language == culture.Name);
                if (localization != null)
                {
                    return localization.Value;
                }
            }

            return null;
        }

        public override IEnumerable<ResourceItem> GetAllStrings(string originalKey, string[] normalizedKey, CultureInfo culture)
        {
            using (var db = new LanguageEntities("EPiServerDB"))
            {
                var resources = db.LocalizationResources
                                  .Include(l => l.Translations)
                                  .Where(r => r.ResourceKey.StartsWith(originalKey) && r.Translations.Any(t => t.Language == culture.Name))
                                  .ToList();

                if (resources.Any())
                {
                    return resources.Select(r => new ResourceItem(r.ResourceKey,
                                                                  r.Translations.First(t => t.Language == culture.Name).Value,
                                                                  culture)).ToList();
                }

                return Enumerable.Empty<ResourceItem>();
            }
        }
    }
}

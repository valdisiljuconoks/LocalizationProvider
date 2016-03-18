using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using EPiServer.Framework.Localization;

namespace DbLocalizationProvider
{
    public class LocalizationResourceRepository : ILocalizationResourceRepository
    {
        public string GetTranslation(string key, CultureInfo language)
        {
            var resource = GetResource(key);

            var localization = resource?.Translations.FirstOrDefault(t => t.Language == language.Name);
            return localization?.Value;
        }

        public IEnumerable<CultureInfo> GetAvailableLanguages()
        {
            using (var db = GetDatabaseContext())
            {
                var availableLanguages = db.LocalizationResourceTranslations
                                           .Select(t => t.Language)
                                           .Distinct()
                                           .ToList()
                                           .Select(l => new CultureInfo(l)).ToList();

                return availableLanguages;
            }
        }

        public IEnumerable<LocalizationResource> GetAllResources()
        {
            using (var db = GetDatabaseContext())
            {
                return db.LocalizationResources.Include(r => r.Translations).ToList();
            }
        }

        public IEnumerable<ResourceItem> GetAllTranslations(string key, CultureInfo language)
        {
            var allResources = GetAllResources().Where(r =>
                                                       r.ResourceKey.StartsWith(key)
                                                       && r.Translations.Any(t => t.Language == language.Name)).ToList();

            if (!allResources.Any())
            {
                return Enumerable.Empty<ResourceItem>();
            }

            return allResources.Select(r => new ResourceItem(r.ResourceKey,
                                                             r.Translations.First(t => t.Language == language.Name).Value,
                                                             language));
        }

        public void CreateOrUpdateTranslation(string key, CultureInfo language, string newValue)
        {
            using (var db = GetDatabaseContext())
            {
                var resource = db.LocalizationResources.Include(r => r.Translations).FirstOrDefault(r => r.ResourceKey == key);

                if (resource == null)
                {
                    // TODO: return some status response obj
                    return;
                }

                var translation = resource.Translations.FirstOrDefault(t => t.Language == language.Name);

                if (translation != null)
                {
                    // update existing translation
                    translation.Value = newValue;
                }
                else
                {
                    var newTranslation = new LocalizationResourceTranslation
                                         {
                                             Value = newValue,
                                             Language = language.Name,
                                             ResourceId = resource.Id
                                         };

                    db.LocalizationResourceTranslations.Add(newTranslation);
                }

                db.SaveChanges();
            }
        }

        public void CreateResource(string key, string username)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            using (var db = GetDatabaseContext())
            {
                var existingResource = db.LocalizationResources.FirstOrDefault(r => r.ResourceKey == key);

                if (existingResource != null)
                {
                    throw new InvalidOperationException($"Resource with key `{key}` already exists");
                }

                db.LocalizationResources.Add(new LocalizationResource(key)
                                             {
                                                 ModificationDate = DateTime.UtcNow,
                                                 Author = username
                                             });
                db.SaveChanges();
            }
        }

        public void DeleteResource(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            using (var db = GetDatabaseContext())
            {
                var existingResource = db.LocalizationResources.FirstOrDefault(r => r.ResourceKey == key);

                if(existingResource == null)
                {
                    return;
                }

                if(existingResource.FromCode)
                {
                    throw new InvalidOperationException("Cannot delete resource that is synced with code");
                }

                db.LocalizationResources.Remove(existingResource);
                db.SaveChanges();
            }
        }

        internal LocalizationResource GetResource(string key)
        {
            using (var db = GetDatabaseContext())
            {
                var resource = db.LocalizationResources
                                 .Include(r => r.Translations)
                                 .FirstOrDefault(r => r.ResourceKey == key);

                return resource;
            }
        }

        internal LanguageEntities GetDatabaseContext()
        {
            return new LanguageEntities("EPiServerDB");
        }
    }
}

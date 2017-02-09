using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using DbLocalizationProvider.Cache;

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

        public class Handler : IQueryHandler<Query, string>
        {
            public string Execute(Query query)
            {
                if(ConfigurationContext.Current.EnableLocalization())
                    return query.Key;

                return GetTranslation(query);
            }

            private string GetTranslation(Query query)
            {
                var key = query.Key;
                var language = query.Language;
                var cacheKey = CacheKeyHelper.BuildKey(key);
                var localizationResource = ConfigurationContext.Current.CacheManager.Get(cacheKey) as LocalizationResource;

                if(localizationResource != null)
                    return GetTranslationFromAvailableList(localizationResource.Translations, language, query.UseFallback)?.Value;

                var resource = GetResourceFromDb(key);
                LocalizationResourceTranslation localization = null;

                // create empty null resource - to indicate non-existing one
                if (resource == null)
                    resource = LocalizationResource.CreateNonExisting(key);
                else
                    localization = GetTranslationFromAvailableList(resource.Translations, language, query.UseFallback);

                ConfigurationContext.Current.CacheManager.Insert(cacheKey, resource);
                return localization?.Value;
            }

            private LocalizationResourceTranslation GetTranslationFromAvailableList(ICollection<LocalizationResourceTranslation> translations,
                                                                                    CultureInfo language,
                                                                                    bool queryUseFallback)
            {
                var foundTranslation = translations?.FirstOrDefault(t => t.Language == language.Name);

                if(foundTranslation == null && queryUseFallback)
                {
                    return translations?.FirstOrDefault(t => t.Language == ConfigurationContext.CultureForTranslationsFromCode);
                }

                return foundTranslation;
            }

            private static LocalizationResource GetResourceFromDb(string key)
            {
                using (var db = new LanguageEntities())
                {
                    var resource = db.LocalizationResources
                                     .Include(r => r.Translations)
                                     .FirstOrDefault(r => r.ResourceKey == key);

                    return resource;
                }
            }
        }
    }
}

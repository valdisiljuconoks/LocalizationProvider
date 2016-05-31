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
            public Query(string key, CultureInfo language)
            {
                Key = key;
                Language = language;
            }

            public string Key { get; set; }

            public CultureInfo Language { get; set; }
        }

        public class Handler : IQueryHandler<Query, string>
        {
            public string Execute(Query query)
            {
                var result = GetTranslation(query.Key, query.Language);

                if(result == null)
                {
                    return null;
                }

                return ConfigurationContext.Current.EnableLocalization() ? result : query.Key;
            }

            private string GetTranslation(string key, CultureInfo language)
            {
                var cacheKey = CacheKeyHelper.BuildKey(key);
                var localizationResource = ConfigurationContext.Current.CacheManager.Get(cacheKey) as LocalizationResource;
                if(localizationResource != null)
                {
                    // if value for the cache key is null - this is non-existing resource (no hit)
                    return localizationResource.Translations?.FirstOrDefault(t => t.Language == language.Name)?.Value;
                }

                var resource = GetResourceFromDb(key);
                LocalizationResourceTranslation localization = null;
                if(resource == null)
                {
                    // create empty null resource - to indicate non-existing one
                    resource = LocalizationResource.CreateNonExisting(key);
                }
                else
                {
                    localization = resource.Translations.FirstOrDefault(t => t.Language == language.Name);
                }

                ConfigurationContext.Current.CacheManager.Insert(cacheKey, resource);
                return localization?.Value;
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

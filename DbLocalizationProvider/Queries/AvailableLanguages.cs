using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DbLocalizationProvider.Cache;

namespace DbLocalizationProvider.Queries
{
    public class AvailableLanguages
    {
        public class Query : IQuery<IEnumerable<CultureInfo>> { }

        public class Handler : IQueryHandler<Query, IEnumerable<CultureInfo>>
        {
            public IEnumerable<CultureInfo> Execute(Query query)
            {
                var cacheKey = CacheKeyHelper.BuildKey("AvailableLanguages");
                var cachedLanguages = ConfigurationContext.Current.CacheManager.Get(cacheKey) as IEnumerable<CultureInfo>;

                if(cachedLanguages != null)
                    return cachedLanguages;

                var languages = GetAvailableLanguages();
                ConfigurationContext.Current.CacheManager.Insert(cacheKey, languages);

                return languages;
            }

            private IEnumerable<CultureInfo> GetAvailableLanguages()
            {
                using (var db = new LanguageEntities())
                {
                    var availableLanguages = db.LocalizationResourceTranslations
                                               .Select(t => t.Language)
                                               .Distinct()
                                               .Where(l => l != ConfigurationContext.CultureForTranslationsFromCode)
                                               .ToList()
                                               .Select(l => new CultureInfo(l)).ToList();

                    return availableLanguages;
                }
            }
        }
    }
}

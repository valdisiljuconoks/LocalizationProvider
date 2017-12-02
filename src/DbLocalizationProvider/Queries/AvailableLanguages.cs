using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DbLocalizationProvider.Cache;

namespace DbLocalizationProvider.Queries
{
    public class AvailableLanguages
    {
        public class Query : IQuery<IEnumerable<CultureInfo>>
        {
            public bool IncludeInvariant { get; set; }
        }

        public class Handler : IQueryHandler<Query, IEnumerable<CultureInfo>>
        {
            public IEnumerable<CultureInfo> Execute(Query query)
            {
                var cacheKey = CacheKeyHelper.BuildKey($"AvailableLanguages_{query.IncludeInvariant}");

                if(ConfigurationContext.Current.CacheManager.Get(cacheKey) is IEnumerable<CultureInfo> cachedLanguages)
                    return cachedLanguages;

                var languages = GetAvailableLanguages(query.IncludeInvariant);
                ConfigurationContext.Current.CacheManager.Insert(cacheKey, languages);

                return languages;
            }

            private IEnumerable<CultureInfo> GetAvailableLanguages(bool includeInvariant)
            {
                using(var db = new LanguageEntities())
                {
                    var availableLanguages = db.LocalizationResourceTranslations
                        .Select(t => t.Language)
                        .Distinct()
                        .Where(l => includeInvariant || l != CultureInfo.InvariantCulture.Name)
                        .ToList()
                        .Select(l => new CultureInfo(l)).ToList();

                    return availableLanguages;
                }
            }
        }
    }
}

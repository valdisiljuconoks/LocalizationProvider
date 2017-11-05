using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DbLocalizationProvider.Cache;
using DbLocalizationProvider.Queries;

namespace DbLocalizationProvider.AspNet.Queries
{
    public class AvailableLanguagesHandler : IQueryHandler<AvailableLanguages.Query, IEnumerable<CultureInfo>>
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

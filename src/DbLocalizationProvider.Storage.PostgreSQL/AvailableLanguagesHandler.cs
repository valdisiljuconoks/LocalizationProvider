// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Collections.Generic;
using System.Globalization;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Cache;
using DbLocalizationProvider.Queries;

namespace DbLocalizationProvider.Storage.PostgreSql
{
    /// <summary>
    /// Reads all available languages form database (in which translations are added).
    /// </summary>
    public class AvailableLanguagesHandler : IQueryHandler<AvailableLanguages.Query, IEnumerable<CultureInfo>>
    {
        /// <summary>
        /// Place where query handling happens
        /// </summary>
        /// <param name="query">This is the query instance</param>
        /// <returns>
        /// You have to return something from the query execution. Of course you can return <c>null</c> as well if you
        /// will.
        /// </returns>
        public IEnumerable<CultureInfo> Execute(AvailableLanguages.Query query)
        {
            var cacheKey = CacheKeyHelper.BuildKey($"AvailableLanguages_{query.IncludeInvariant}");
            if (ConfigurationContext.Current.CacheManager.Get(cacheKey) is IEnumerable<CultureInfo> cachedLanguages)
            {
                return cachedLanguages;
            }

            var languages = GetAvailableLanguages(query.IncludeInvariant);
            ConfigurationContext.Current.CacheManager.Insert(cacheKey, languages, false);

            return languages;
        }

        private IEnumerable<CultureInfo> GetAvailableLanguages(bool includeInvariant)
        {
            var repo = new ResourceRepository();

            return repo.GetAvailableLanguages(includeInvariant);
        }
    }
}

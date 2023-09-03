// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Cache;

namespace DbLocalizationProvider.Queries
{
    /// <summary>
    /// When you need to get al available/supported languages
    /// </summary>
    public class AvailableLanguages
    {
        /// <summary>
        /// Reads all available languages form database (in which translations are added).
        /// </summary>
        public class Handler : IQueryHandler<AvailableLanguages.Query, IEnumerable<AvailableLanguage>>
        {
            private readonly ConfigurationContext _configurationContext;
            private readonly IResourceRepository _repository;

            /// <summary>
            /// Creates new instance of the handler.
            /// </summary>
            /// <param name="configurationContext">Configuration settings.</param>
            /// <param name="repository">Resource repository (usually provided by storage implementation).</param>
            public Handler(ConfigurationContext configurationContext, IResourceRepository repository)
            {
                _configurationContext = configurationContext;
                _repository = repository;
            }

            /// <summary>
            /// Place where query handling happens
            /// </summary>
            /// <param name="query">This is the query instance</param>
            /// <returns>
            /// You have to return something from the query execution. Of course you can return <c>null</c> as well if you
            /// will.
            /// </returns>
            public async Task<IEnumerable<AvailableLanguage>> Execute(Query query)
            {
                var cacheKey = CacheKeyHelper.BuildKey($"AvailableLanguages_{query.IncludeInvariant}");
                if (_configurationContext.CacheManager.Get(cacheKey) is IEnumerable<AvailableLanguage> cachedLanguages)
                {
                    return cachedLanguages;
                }

                var languages = await GetAvailableLanguages(query.IncludeInvariant);
                _configurationContext.CacheManager.Insert(cacheKey, languages, false);

                return languages;
            }

            private async Task<IEnumerable<AvailableLanguage>> GetAvailableLanguages(bool includeInvariant)
            {
                var allLanguages = await _repository.GetAvailableLanguagesAsync(includeInvariant);
                return allLanguages
                    .Select((l, ix) => new AvailableLanguage(l.EnglishName, ix, l));
            }
        }

        /// <summary>
        /// Query definition of the all available/supported languages
        /// </summary>
        public class Query : IQuery<IEnumerable<AvailableLanguage>>
        {
            /// <summary>
            /// To control whether you would like to include all invariant translations as well
            /// </summary>
            public bool IncludeInvariant { get; set; }
        }
    }
}

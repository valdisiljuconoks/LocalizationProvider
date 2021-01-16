// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Collections.Generic;
using System.Globalization;
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
        public class Handler : IQueryHandler<AvailableLanguages.Query, IEnumerable<CultureInfo>>
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
            public IEnumerable<CultureInfo> Execute(AvailableLanguages.Query query)
            {
                var cacheKey = CacheKeyHelper.BuildKey($"AvailableLanguages_{query.IncludeInvariant}");
                if (_configurationContext.CacheManager.Get(cacheKey) is IEnumerable<CultureInfo> cachedLanguages)
                {
                    return cachedLanguages;
                }

                var languages = GetAvailableLanguages(query.IncludeInvariant);
                _configurationContext.CacheManager.Insert(cacheKey, languages, false);

                return languages;
            }

            private IEnumerable<CultureInfo> GetAvailableLanguages(bool includeInvariant)
            {
                return _repository.GetAvailableLanguages(includeInvariant);
            }
        }

        /// <summary>
        /// Query definition of the all available/supported languages
        /// </summary>
        public class Query : IQuery<IEnumerable<CultureInfo>>
        {
            /// <summary>
            /// To control whether you would like to include all invariant translations as well
            /// </summary>
            public bool IncludeInvariant { get; set; }
        }
    }
}

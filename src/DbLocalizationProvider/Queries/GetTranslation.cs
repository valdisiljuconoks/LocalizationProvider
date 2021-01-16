// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Globalization;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Cache;

namespace DbLocalizationProvider.Queries
{
    /// <summary>
    /// Gets translation for given resource
    /// </summary>
    public class GetTranslation
    {
        /// <summary>
        /// Gets translation handler
        /// </summary>
        public class Handler : IQueryHandler<GetTranslation.Query, string>
        {
            private readonly ConfigurationContext _configurationContext;
            private readonly IQueryExecutor _queryExecutor;

            /// <summary>
            /// Creates new instance of the class.
            /// </summary>
            /// <param name="configurationContext">Configuration settings.</param>
            /// <param name="queryExecutor">The executor of the queries.</param>
            public Handler(ConfigurationContext configurationContext, IQueryExecutor queryExecutor)
            {
                _configurationContext = configurationContext;
                _queryExecutor = queryExecutor;
            }

            /// <summary>
            /// Place where query handling happens
            /// </summary>
            /// <param name="query">This is the query instance</param>
            /// <returns>
            /// You have to return something from the query execution. Of course you can return <c>null</c> as well if you
            /// will.
            /// </returns>
            public string Execute(GetTranslation.Query query)
            {
                if (!_configurationContext.EnableLocalization())
                {
                    return query.Key;
                }

                var key = query.Key;

                // we can check whether we know this resource at all
                // if not - we can break circuit here
                if (!_configurationContext.BaseCacheManager.IsKeyKnown(key))
                {
                    return null;
                }

                var cacheKey = CacheKeyHelper.BuildKey(key);
                if (_configurationContext.DiagnosticsEnabled)
                {
                    _configurationContext.Logger?.Debug($"Executing query for resource key `{query.Key}` (lang: `{query.Language.Name})..");
                }

                var localizationResource = _configurationContext.CacheManager.Get(cacheKey) as LocalizationResource;

                if (localizationResource == null)
                {
                    if (_configurationContext.DiagnosticsEnabled)
                    {
                        _configurationContext.Logger?.Info(
                            $"MISSING: Resource Key (culture: {query.Language.Name}): {query.Key}. Probably class is not decorated with either [LocalizedModel] or [LocalizedResource] attribute.");
                    }

                    // resource is not found in the cache, let's check database
                    localizationResource = GetResourceFromDb(key) ?? LocalizationResource.CreateNonExisting(key);
                    _configurationContext.CacheManager.Insert(cacheKey, localizationResource, true);
                }

                return localizationResource.Translations.GetValueWithFallback(
                    query.Language,
                    _configurationContext.FallbackList.GetFallbackLanguages(query.Language));
            }

            /// <summary>
            /// Gets the resource from database.
            /// </summary>
            /// <param name="key">The key.</param>
            /// <returns></returns>
            protected virtual LocalizationResource GetResourceFromDb(string key)
            {
                var q = new GetResource.Query(key);

                return _queryExecutor.Execute(q);
            }
        }

        /// <summary>
        /// Query definition to get translation for given resource
        /// </summary>
        public class Query : IQuery<string>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Query" /> class.
            /// </summary>
            /// <param name="key">The resource key.</param>
            /// <param name="language">The language to get translation in.</param>
            public Query(string key, CultureInfo language)
            {
                Key = key;
                Language = language;
            }

            /// <summary>
            /// Gets the key.
            /// </summary>
            public string Key { get; }

            /// <summary>
            /// Gets the language.
            /// </summary>
            public CultureInfo Language { get; }
        }
    }
}

// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Globalization;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Cache;
using DbLocalizationProvider.Logging;
using Microsoft.Extensions.Options;

namespace DbLocalizationProvider.Queries;

/// <summary>
/// Gets translation for given resource
/// </summary>
public class GetTranslation
{
    /// <summary>
    /// Gets translation handler
    /// </summary>
    public class Handler : IQueryHandler<Query, string?>
    {
        private readonly IOptions<ConfigurationContext> _configurationContext;
        private readonly ILogger _logger;
        private readonly IQueryExecutor _queryExecutor;

        /// <summary>
        /// Creates new instance of the class.
        /// </summary>
        /// <param name="configurationContext">Configuration settings.</param>
        /// <param name="queryExecutor">The executor of the queries.</param>
        /// <param name="logger">When you need to write down your thoughts.</param>
        public Handler(IOptions<ConfigurationContext> configurationContext, IQueryExecutor queryExecutor, ILogger logger)
        {
            _configurationContext = configurationContext ?? throw new ArgumentNullException(nameof(configurationContext));
            _queryExecutor = queryExecutor ?? throw new ArgumentNullException(nameof(queryExecutor));
            _logger = logger;
        }

        /// <summary>
        /// Place where query handling happens
        /// </summary>
        /// <param name="query">This is the query instance</param>
        /// <returns>
        /// You have to return something from the query execution. Of course, you can return <c>null</c> as well if you
        /// will.
        /// </returns>
        public string? Execute(Query query)
        {
            if (!_configurationContext.Value.EnableLocalization())
            {
                return query.Key;
            }

            var key = query.Key;

            if (_configurationContext.Value._baseCacheManager.AreKnownKeysStored()
                && !_configurationContext.Value._baseCacheManager.IsKeyKnown(key))
            {
                // we are here because of a couple of reasons:
                //  * someone is asking for non-existing resource (known keys are synced and key does not exist)
                //  * someone has programmatically created resource and query is made on different cluster node (cache is still cold for this resource)
                //
                // if this resource is not yet found in cache
                // we can try to lookup resource once more in database and if not found - then we can short-break the circuit

                GetCachedTranslationsOrReadFromStorage(query);
            }

            if (_configurationContext.Value.DiagnosticsEnabled)
            {
                _logger.Debug($"Executing query for resource key `{key}` (lang: `{query.Language.Name}`)..");
            }

            var translations = GetCachedTranslationsOrReadFromStorage(query);

            if (translations.Count == 0)
            {
                return null;
            }

            return query.FallbackToInvariant
                ? translations.ByLanguage(query.Language.Name, true)
                : translations.GetValueWithFallback(
                    query.Language,
                    _configurationContext.Value._fallbackCollection.GetFallbackLanguages(query.Language));
        }

        /// <summary>
        /// Gets the resource from database.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        protected virtual LocalizationResource? GetResourceFromDb(string key)
        {
            var q = new GetResource.Query(key);

            return _queryExecutor.Execute(q);
        }

        private CachedTranslations GetCachedTranslationsOrReadFromStorage(Query query)
        {
            var key = query.Key;
            var cacheKey = CacheKeyHelper.BuildKey(key);

            var cached = _configurationContext.Value.CacheManager.Get(cacheKey);
            if (cached is CachedTranslations translations)
            {
                return translations;
            }

            if (cached is LocalizationResource legacy)
            {
                // legacy in-memory entries (written by primers before this type existed) - upgrade in place
                translations = CachedTranslations.From(legacy);
                _configurationContext.Value.CacheManager.Insert(cacheKey, translations, true);
                return translations;
            }

            if (_configurationContext.Value.DiagnosticsEnabled)
            {
                _logger.Info(
                    $"MISSING: Resource Key (culture: {query.Language.Name}): {key}. Probably class is not decorated with either [LocalizedModel] or [LocalizedResource] attribute.");
            }

            var fromDb = GetResourceFromDb(key);
            translations = fromDb != null ? CachedTranslations.From(fromDb) : CachedTranslations.NonExisting;
            _configurationContext.Value.CacheManager.Insert(cacheKey, translations, true);

            return translations;
        }
    }

    /// <summary>
    /// Query definition to get translation for given resource
    /// </summary>
    public class Query(string key, CultureInfo? language) : IQuery<string?>
    {
        /// <summary>
        /// Gets the key.
        /// </summary>
        public string Key { get; } = key;

        /// <summary>
        /// Gets the language.
        /// </summary>
        public CultureInfo? Language { get; } = language;

        /// <summary>
        /// You can explicitly set fallback for this query if needed (configured global value will not be affected).
        /// </summary>
        public bool FallbackToInvariant { get; set; }
    }
}

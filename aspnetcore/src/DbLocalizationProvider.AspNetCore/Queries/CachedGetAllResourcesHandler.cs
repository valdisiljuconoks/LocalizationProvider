// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Cache;
using DbLocalizationProvider.Queries;
using Microsoft.Extensions.Options;

namespace DbLocalizationProvider.AspNetCore.Queries;

/// <summary>
/// Cached implementation of <see cref="GetAllResources.Query" />.
/// </summary>
public class CachedGetAllResourcesHandler : IQueryHandler<GetAllResources.Query, Dictionary<string, LocalizationResource>>
{
    private readonly IOptions<ConfigurationContext> _configurationContext;
    private readonly IQueryHandler<GetAllResources.Query, Dictionary<string, LocalizationResource>> _inner;
    private readonly IQueryExecutor _queryExecutor;

    /// <summary>
    /// Creates new instance of this class.
    /// </summary>
    /// <param name="inner">Inner query.</param>
    /// <param name="queryExecutor">The executor of the queries.</param>
    /// <param name="configurationContext">Configuration settings.</param>
    public CachedGetAllResourcesHandler(
        IQueryHandler<GetAllResources.Query, Dictionary<string, LocalizationResource>> inner,
        IQueryExecutor queryExecutor,
        IOptions<ConfigurationContext> configurationContext)
    {
        _inner = inner ?? throw new ArgumentNullException(nameof(inner));
        _queryExecutor = queryExecutor ?? throw new ArgumentNullException(nameof(queryExecutor));
        _configurationContext = configurationContext ?? throw new ArgumentNullException(nameof(configurationContext));
    }

    /// <summary>
    /// Executes the handler
    /// </summary>
    /// <param name="query">Query to execute.</param>
    /// <returns>Result from the query.</returns>
    public Dictionary<string, LocalizationResource> Execute(GetAllResources.Query query)
    {
        if (query.ForceReadFromDb)
        {
            return _inner.Execute(query);
        }

        // try to get the cached dictionary directly (O(1) lookup)
        var cached = _configurationContext.Value._baseCacheManager.GetAllResourcesDictionary();
        if (cached != null)
        {
            return cached;
        }

        // cache miss: load from inner handler (which reads from DB)
        var result = _inner.Execute(query);

        // populate individual resource entries for GetTranslation lookups
        foreach (var kv in result)
        {
            var cacheKey = CacheKeyHelper.BuildKey(kv.Key);
            _configurationContext.Value.CacheManager.Insert(cacheKey, kv.Value, true);
        }

        // store the dictionary last (after individual inserts that invalidate any stale dictionary)
        _configurationContext.Value._baseCacheManager.InsertAllResourcesDictionary(result);

        return result;
    }
}

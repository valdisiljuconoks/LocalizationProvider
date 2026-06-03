// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider;

/// <summary>
/// Helper utility to get along with queries.
/// </summary>
public class QueryExecutor : IQueryExecutor
{
    private readonly TypeFactory _factory;

    /// <summary>
    /// Creates new instance.
    /// </summary>
    /// <param name="factory">Factory of the types.</param>
    public QueryExecutor(TypeFactory factory)
    {
        _factory = factory;
    }

    /// <summary>
    /// Execute given query.
    /// </summary>
    /// <typeparam name="TResult">Return type from the <paramref name="query" />.</typeparam>
    /// <param name="query">Query descriptor.</param>
    /// <returns>Result from the query execution.</returns>
    public TResult? Execute<TResult>(IQuery<TResult?> query)
    {
        if (query == null)
        {
            throw new ArgumentNullException(nameof(query));
        }

        var handler = _factory.GetQueryHandler(query);

        return handler == null ? default : handler.Execute(query);
    }
}

// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider
{
    /// <summary>
    /// Helper utility to get along with queries.
    /// </summary>
    public class QueryExecutor : IQueryExecutor
    {
        private readonly ConfigurationContext _context;

        /// <summary>
        /// Creates new instance.
        /// </summary>
        /// <param name="context">Configuration settings.</param>
        public QueryExecutor(ConfigurationContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Execute given query.
        /// </summary>
        /// <typeparam name="TResult">Return type from the <paramref name="query"/>.</typeparam>
        /// <param name="query">Query descriptor.</param>
        /// <returns>Result from the query execution.</returns>
        public TResult Execute<TResult>(IQuery<TResult> query)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var handler = _context.TypeFactory.GetQueryHandler(query);

            return handler.Execute(query);
        }
    }
}

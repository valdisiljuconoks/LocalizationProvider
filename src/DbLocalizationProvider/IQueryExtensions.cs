// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider
{
    /// <summary>
    /// A-HA
    /// </summary>
    public static class IQueryExtensions
    {
        /// <summary>
        /// Executes the specified query.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">query</exception>
        public static TResult Execute<TResult>(this IQuery<TResult> query)
        {
            if (query == null) throw new ArgumentNullException(nameof(query));
            var handler = ConfigurationContext.Current.TypeFactory.GetQueryHandler(query);

            return handler.Execute(query);
        }
    }
}

// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using Azure.Data.Tables;

namespace DbLocalizationProvider.Storage.AzureTables
{
    /// <summary>
    /// Extensions for Azure Tables
    /// </summary>
    public static class CloudTableExtensions
    {
        /// <summary>
        /// New Azure Storage API allows now only segmented query execution. Meaning that it will return limited results if row count exceeds limits
        /// (1000 if recall correctly).
        /// So this method helps to make it easy to query larger set.
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="table">Target table</param>
        /// <param name="filter">Filter expression as string</param>
        /// <param name="columns">Column selector</param>
        /// <param name="ct">If things gets too long - this is a way to rage quit.</param>
        /// <param name="onProgress">If you are interested - we can tell you when you segment is fetched.</param>
        /// <returns>Results obviously.</returns>
        public static IList<T> ExecuteQuery<T>(
            this TableClient table,
            string filter,
            IEnumerable<string> columns = null,
            CancellationToken ct = default,
            Action<IList<T>> onProgress = null)
            where T : class, ITableEntity, new()
        {
            var items = new List<T>();
            var pages = table.Query<T>(filter, select: columns, cancellationToken: ct);

            foreach (var page in pages.AsPages())
            {
                items.AddRange(page.Values);
                onProgress?.Invoke(items);
            }

            return items;
        }

        /// <summary>
        /// New Azure Storage API allows now only segmented query execution. Meaning that it will return limited results if row count exceeds limits
        /// (1000 if recall correctly).
        /// So this method helps to make it easy to query larger set.
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="table">Target table</param>
        /// <param name="filter">Filter expression</param>
        /// <param name="columns">Column selector</param>
        /// <param name="ct">If things gets too long - this is a way to rage quit.</param>
        /// <param name="onProgress">If you are interested - we can tell you when you segment is fetched.</param>
        /// <returns>Results obviously.</returns>
        public static IList<T> ExecuteQuery<T>(
            this TableClient table,
            Expression<Func<T, bool>> filter,
            IEnumerable<string> columns = null,
            CancellationToken ct = default,
            Action<IList<T>> onProgress = null)
            where T : class, ITableEntity, new()
        {
            var items = new List<T>();
            var pages = table.Query(filter, select: columns, cancellationToken: ct);

            foreach (var page in pages.AsPages())
            {
                items.AddRange(page.Values);
                onProgress?.Invoke(items);
            }

            return items;
        }
    }
}

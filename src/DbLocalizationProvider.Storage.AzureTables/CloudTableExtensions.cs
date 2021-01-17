using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;

namespace DbLocalizationProvider.Storage.AzureTables
{
    /// <summary>
    /// Extensions for Azure Tables
    /// </summary>
    public static class CloudTableExtensions
    {
        /// <summary>
        /// New Azure Storage API allows now only segmented query execution. Meaning that it will return limited results if row count exceeds limits.
        /// So this method helps to make it easy to query larger set.
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="table">Target table</param>
        /// <param name="query">Specs for what are you looking for</param>
        /// <param name="ct">If things gets too long - this is a way to rage quit.</param>
        /// <param name="onProgress">If you are interested - we can tell you when you segment is fetched.</param>
        /// <returns>Results obviously.</returns>
        public static async Task<IList<T>> ExecuteQueryAsync<T>(this CloudTable table, TableQuery<T> query, CancellationToken ct = new CancellationToken(), Action<IList<T>> onProgress = null)
            where T : ITableEntity, new()
        {
            var runningQuery = new TableQuery<T>
                               {
                                   FilterString = query.FilterString,
                                   SelectColumns = query.SelectColumns
                               };

            var items = new List<T>();
            TableContinuationToken token = null;

            do
            {
                runningQuery.TakeCount = query.TakeCount - items.Count;

                var seg = await table.ExecuteQuerySegmentedAsync(runningQuery, token, null, null, ct);
                token = seg.ContinuationToken;
                items.AddRange(seg);
                onProgress?.Invoke(items);
            }
            while(token != null && !ct.IsCancellationRequested && (query.TakeCount == null || items.Count < query.TakeCount.Value));

            return items;
        }
    }
}

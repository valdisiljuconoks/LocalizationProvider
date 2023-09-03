// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Threading.Tasks;
using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider
{
    /// <summary>
    /// </summary>
    public interface IQueryExecutor
    {
        /// <summary>
        /// </summary>
        /// <param name="query"></param>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        Task<TResult> Execute<TResult>(IQuery<TResult> query);
    }
}

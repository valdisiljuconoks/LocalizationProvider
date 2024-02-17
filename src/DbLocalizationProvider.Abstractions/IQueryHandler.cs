// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

namespace DbLocalizationProvider.Abstractions;

/// <summary>
/// And this is handler of the <see cref="IQuery{TResult}" />
/// </summary>
/// <typeparam name="TQuery">Instance of the query which is being executed</typeparam>
/// <typeparam name="TResult">Return type of the query. Might be collection of something also.</typeparam>
public interface IQueryHandler<in TQuery, out TResult> where TQuery : IQuery<TResult>
{
    /// <summary>
    /// Place where query handling happens
    /// </summary>
    /// <param name="query">This is the query instance</param>
    /// <returns>
    /// You have to return something from the query execution. Of course you can return <c>null</c> as well if you
    /// will.
    /// </returns>
    TResult Execute(TQuery query);
}

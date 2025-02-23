// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Queries.Internal;

internal abstract class QueryHandlerWrapper<TResult>
{
    public abstract TResult Execute(IQuery<TResult> message);
}

internal class QueryHandlerWrapper<TQuery, TResult>(IQueryHandler<TQuery, TResult> inner) : QueryHandlerWrapper<TResult>
    where TQuery : IQuery<TResult>
{
    public override TResult Execute(IQuery<TResult> message)
    {
        return inner.Execute((TQuery)message);
    }
}

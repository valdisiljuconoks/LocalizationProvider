// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Threading.Tasks;
using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Queries.Internal
{
    internal abstract class QueryHandlerWrapper<TResult>
    {
        public abstract Task<TResult> Execute(IQuery<TResult> message);
    }

    internal class QueryHandlerWrapper<TQuery, TResult> : QueryHandlerWrapper<TResult> where TQuery : IQuery<TResult>
    {
        private readonly IQueryHandler<TQuery, TResult> _inner;

        public QueryHandlerWrapper(IQueryHandler<TQuery, TResult> inner)
        {
            _inner = inner;
        }

        public override Task<TResult> Execute(IQuery<TResult> message)
        {
            return _inner.Execute((TQuery)message);
        }
    }
}

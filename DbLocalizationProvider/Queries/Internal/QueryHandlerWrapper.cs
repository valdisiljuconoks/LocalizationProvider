namespace DbLocalizationProvider.Queries.Internal
{
    

    internal abstract class QueryHandlerWrapper<TResult>
    {
        public abstract TResult Execute(IQuery<TResult> message);
    }

    internal class QueryHandlerWrapper<TQuery, TResult> : QueryHandlerWrapper<TResult> where TQuery : IQuery<TResult>
    {
        private readonly IQueryHandler<TQuery, TResult> _inner;

        public QueryHandlerWrapper(IQueryHandler<TQuery, TResult> inner)
        {
            _inner = inner;
        }

        public override TResult Execute(IQuery<TResult> message)
        {
            return _inner.Execute((TQuery) message);
        }
    }
}

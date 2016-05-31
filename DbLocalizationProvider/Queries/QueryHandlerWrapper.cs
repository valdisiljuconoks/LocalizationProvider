namespace DbLocalizationProvider.Queries
{
    internal abstract class QueryHandlerWrapper<TResult>
    {
        public abstract TResult Execute(IQuery<TResult> message);
    }

    internal class QueryHandlerWrapper<TCommand, TResult> : QueryHandlerWrapper<TResult> where TCommand : IQuery<TResult>
    {
        private readonly IQueryHandler<TCommand, TResult> _inner;

        public QueryHandlerWrapper(IQueryHandler<TCommand, TResult> inner)
        {
            _inner = inner;
        }

        public override TResult Execute(IQuery<TResult> message)
        {
            return _inner.Execute((TCommand) message);
        }
    }
}

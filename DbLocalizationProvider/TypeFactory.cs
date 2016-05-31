using System;
using System.Collections.Concurrent;
using DbLocalizationProvider.Queries;

namespace DbLocalizationProvider
{
    public class TypeFactory
    {
        private readonly ConcurrentDictionary<Type, Type> _mappings = new ConcurrentDictionary<Type, Type>();
        private readonly ConcurrentDictionary<Type, Type> _wrapperHandlerCache = new ConcurrentDictionary<Type, Type>();

        public SetHandlerExpression<TQuery> ForQuery<TQuery>()
        {
            return new SetHandlerExpression<TQuery>(_mappings);
        }

        internal QueryHandlerWrapper<TResult> GetQueryHandler<TResult>(IQuery<TResult> query)
        {
            return GetHandler<QueryHandlerWrapper<TResult>, TResult>(query, typeof(QueryHandlerWrapper<,>));
        }

        private TWrapper GetHandler<TWrapper, TResponse>(object request, Type wrapperType)
        {
            var requestType = request.GetType();
            var genericWrapperType = _wrapperHandlerCache.GetOrAdd(requestType, wrapperType, (query, wrapper) => wrapper.MakeGenericType(query, typeof(TResponse)));
            var handler = GetHandler(requestType);

            return (TWrapper) Activator.CreateInstance(genericWrapperType, handler);
        }

        private object GetHandler(Type queryType)
        {
            return Activator.CreateInstance(_mappings[queryType]);
        }
    }

    public class SetHandlerExpression<TQuery>
    {
        private readonly ConcurrentDictionary<Type, Type> _mappings;

        public SetHandlerExpression(ConcurrentDictionary<Type, Type> mappings)
        {
            _mappings = mappings;
        }

        public void SetHandler<THandler>()
        {
            _mappings.GetOrAdd(typeof(TQuery), typeof(THandler));
        }
    }
}

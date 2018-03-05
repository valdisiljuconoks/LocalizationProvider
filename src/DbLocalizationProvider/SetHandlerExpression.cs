using System;
using System.Collections.Concurrent;

namespace DbLocalizationProvider
{
    public class SetHandlerExpression<T>
    {
        private readonly ConcurrentDictionary<Type, Type> _decoratorMmappings;
        private readonly ConcurrentDictionary<Type, Func<object>> _mappings;

        public SetHandlerExpression(ConcurrentDictionary<Type, Func<object>> mappings, ConcurrentDictionary<Type, Type> decoratorMmappings)
        {
            _mappings = mappings;
            _decoratorMmappings = decoratorMmappings;
        }

        public void SetHandler<THandler>()
        {
            _mappings.GetOrAdd(typeof(T), () => TypeFactory.ActivatorFactory(typeof(THandler)));
        }

        public void SetHandler<THandler>(Func<THandler> instanceFactory)
        {
            _mappings.GetOrAdd(typeof(T), () => instanceFactory.Invoke());
        }

        public void DecorateWith<TDecorator>()
        {
            _decoratorMmappings.GetOrAdd(typeof(T), typeof(TDecorator));
        }
    }
}

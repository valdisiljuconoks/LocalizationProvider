using System;
using System.Collections.Concurrent;

namespace DbLocalizationProvider
{
    public class SetHandlerExpression<T>
    {
        private readonly ConcurrentDictionary<Type, Type> _decoratorMappings;
        private readonly ConcurrentDictionary<Type, Func<object>> _mappings;

        public SetHandlerExpression(ConcurrentDictionary<Type, Func<object>> mappings, ConcurrentDictionary<Type, Type> decoratorMappings)
        {
            _mappings = mappings;
            _decoratorMappings = decoratorMappings;
        }

        public void SetHandler<THandler>()
        {
            _mappings.AddOrUpdate(typeof(T), () => TypeFactory.ActivatorFactory(typeof(THandler)), (_, __) => () => TypeFactory.ActivatorFactory(typeof(THandler)));
        }

        public void SetHandler<THandler>(Func<THandler> instanceFactory)
        {
            _mappings.AddOrUpdate(typeof(T), () => instanceFactory.Invoke(), (_, __) => () => instanceFactory.Invoke());
        }

        public void DecorateWith<TDecorator>()
        {
            _decoratorMappings.GetOrAdd(typeof(T), typeof(TDecorator));
        }
    }
}

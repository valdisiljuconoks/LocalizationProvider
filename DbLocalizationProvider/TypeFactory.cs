using System;
using System.Collections.Concurrent;
using DbLocalizationProvider.Commands.Internal;
using DbLocalizationProvider.Internal;
using DbLocalizationProvider.Queries.Internal;

namespace DbLocalizationProvider
{
    /// <summary>
    ///     Inspiration came from https://github.com/jbogard/MediatR
    /// </summary>
    public class TypeFactory
    {
        private readonly ConcurrentDictionary<Type, Type> _decoratorMmappings = new ConcurrentDictionary<Type, Type>();
        private readonly ConcurrentDictionary<Type, Type> _mappings = new ConcurrentDictionary<Type, Type>();

        private readonly ConcurrentDictionary<Type, Type> _wrapperHandlerCache = new ConcurrentDictionary<Type, Type>();

        public SetHandlerExpression<TQuery> ForQuery<TQuery>()
        {
            return new SetHandlerExpression<TQuery>(_mappings, _decoratorMmappings);
        }

        public SetHandlerExpression<TCommand> ForCommand<TCommand>() where TCommand : ICommand
        {
            return new SetHandlerExpression<TCommand>(_mappings, _decoratorMmappings);
        }

        internal QueryHandlerWrapper<TResult> GetQueryHandler<TResult>(IQuery<TResult> query)
        {
            return GetQueryHandler<QueryHandlerWrapper<TResult>, TResult>(query, typeof(QueryHandlerWrapper<,>));
        }

        internal CommandHandlerWrapper GetCommandHandler<TCommand>(TCommand command)
        {
            return GetCommandHandler<CommandHandlerWrapper, TCommand>(command, typeof(CommandHandlerWrapper<>));
        }

        internal TWrapper GetCommandHandler<TWrapper, TCommand>(TCommand request, Type wrapperType)
        {
            var commandType = request.GetType();
            var genericWrapperType = _wrapperHandlerCache.GetOrAdd(commandType, wrapperType, (command, wrapper) => wrapper.MakeGenericType(command));
            var handler = GetHandler(commandType);

            return (TWrapper) Activator.CreateInstance(genericWrapperType, handler);
        }

        private TWrapper GetQueryHandler<TWrapper, TResponse>(object request, Type wrapperType)
        {
            var requestType = request.GetType();
            var genericWrapperType = _wrapperHandlerCache.GetOrAdd(requestType, wrapperType, (query, wrapper) => wrapper.MakeGenericType(query, typeof(TResponse)));
            var handler = GetHandler(requestType);

            return (TWrapper) Activator.CreateInstance(genericWrapperType, handler);
        }

        private object GetHandler(Type queryType)
        {
            var instance = Activator.CreateInstance(_mappings[queryType]);
            return !_decoratorMmappings.ContainsKey(queryType)
                       ? instance
                       : Activator.CreateInstance(_decoratorMmappings[queryType], instance);
        }
    }

    public class SetHandlerExpression<T>
    {
        private readonly ConcurrentDictionary<Type, Type> _decoratorMmappings;
        private readonly ConcurrentDictionary<Type, Type> _mappings;

        public SetHandlerExpression(ConcurrentDictionary<Type, Type> mappings, ConcurrentDictionary<Type, Type> decoratorMmappings)
        {
            _mappings = mappings;
            _decoratorMmappings = decoratorMmappings;
        }

        public void SetHandler<THandler>()
        {
            _mappings.GetOrAdd(typeof(T), typeof(THandler));
        }

        public void DecorateWith<TDecorator>()
        {
            _decoratorMmappings.GetOrAdd(typeof(T), typeof(TDecorator));
        }
    }
}

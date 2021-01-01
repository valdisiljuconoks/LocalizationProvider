// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Concurrent;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Commands.Internal;
using DbLocalizationProvider.Internal;
using DbLocalizationProvider.Queries.Internal;

namespace DbLocalizationProvider
{
    public delegate object ServiceFactory(Type serviceType);

    /// <summary>
    /// Inspiration came from https://github.com/jbogard/MediatR
    /// </summary>
    public class TypeFactory
    {
        private readonly ServiceFactory _serviceFactory;
        private readonly ConcurrentDictionary<Type, Type> _decoratorMappings = new ConcurrentDictionary<Type, Type>();
        private readonly ConcurrentDictionary<Type, ServiceFactory> _mappings = new ConcurrentDictionary<Type, ServiceFactory>();
        private readonly ConcurrentDictionary<Type, Type> _wrapperHandlerCache = new ConcurrentDictionary<Type, Type>();

        /// <summary>
        /// Creates new instance of the class.
        /// </summary>
        /// <param name="serviceFactory">Factory delegate for the services.</param>
        public TypeFactory(ServiceFactory serviceFactory)
        {
            _serviceFactory = serviceFactory;
        }

        /// <summary>
        ///Used for <see cref="System.Activator"/> based factory.
        /// </summary>
        /// <param name="serviceType">Type of the service to create.</param>
        /// <returns>Service instance; otherwise throws various exceptions.</returns>
        internal static ServiceFactory ActivatorFactory(Type serviceType) =>
            type => Activator.CreateInstance(serviceType);

        /// <summary>
        /// Start registration of the handler for query with this method.
        /// </summary>
        /// <typeparam name="TQuery">The type of the query.</typeparam>
        /// <returns></returns>
        public SetHandlerExpression<TQuery> ForQuery<TQuery>()
        {
            return new SetHandlerExpression<TQuery>(_mappings, _decoratorMappings, _serviceFactory, this);
        }

        /// <summary>
        /// Start registration of the handler for command with this method.
        /// </summary>
        /// <typeparam name="TCommand">The type of the command.</typeparam>
        /// <returns></returns>
        public SetHandlerExpression<TCommand> ForCommand<TCommand>() where TCommand : ICommand
        {
            return new SetHandlerExpression<TCommand>(_mappings, _decoratorMappings, _serviceFactory, this);
        }

        internal QueryHandlerWrapper<TResult> GetQueryHandler<TResult>(
            IQuery<TResult> query,
            ConfigurationContext configurationContext)
        {
            return GetQueryHandler<QueryHandlerWrapper<TResult>, TResult>(
                query,
                typeof(QueryHandlerWrapper<,>),
                configurationContext);
        }

        internal CommandHandlerWrapper GetCommandHandler<TCommand>(TCommand command, ConfigurationContext configurationContext)
            where TCommand : ICommand
        {
            return GetCommandHandler<CommandHandlerWrapper, TCommand>(
                command,
                typeof(CommandHandlerWrapper<>),
                configurationContext);
        }

        internal TWrapper GetCommandHandler<TWrapper, TCommand>(
            TCommand request,
            Type wrapperType,
            ConfigurationContext configurationContext) where TCommand : ICommand
        {
            var commandType = request.GetType();
            var genericWrapperType = _wrapperHandlerCache.GetOrAdd(
                commandType,
                wrapperType,
                (command, wrapper) => wrapper.MakeGenericType(command));
            var handler = GetHandler(commandType, configurationContext);

            return (TWrapper)Activator.CreateInstance(genericWrapperType, handler);
        }

        internal TWrapper GetQueryHandler<TWrapper, TResponse>(
            object request,
            Type wrapperType,
            ConfigurationContext configurationContext)
        {
            var requestType = request.GetType();
            var genericWrapperType = _wrapperHandlerCache.GetOrAdd(
                requestType,
                wrapperType,
                (query, wrapper) => wrapper.MakeGenericType(query, typeof(TResponse)));

            var handler = GetHandler(requestType, configurationContext);

            return (TWrapper)Activator.CreateInstance(genericWrapperType, handler);
        }

        internal object GetHandler(Type queryType, ConfigurationContext configurationContext)
        {
            var found = _mappings.TryGetValue(queryType, out var factory);
            if (!found)
            {
                throw new HandlerNotFoundException(
                    $"Failed to find handler for `{queryType}`. Make sure that you have invoked required registration methods (like .UseSqlServer(), .etc..) on ConfigurationContext object in your app startup.");
            }

            var instance = factory.Invoke(queryType);

            return !_decoratorMappings.ContainsKey(queryType)
                ? instance
                : Activator.CreateInstance(_decoratorMappings[queryType], instance);
        }

    }
}

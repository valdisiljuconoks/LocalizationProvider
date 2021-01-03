// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Commands.Internal;
using DbLocalizationProvider.Internal;
using DbLocalizationProvider.Queries.Internal;

namespace DbLocalizationProvider
{
    /// <summary>
    /// Callback for creating new instances of the handlers. Usually this delegate is replaced by <c>DependencyContainer.GetService</c> method.
    /// </summary>
    /// <param name="serviceType">Type of the service to create.</param>
    /// <returns>Service instance if successful; otherwise <c>null</c>.</returns>
    public delegate object ServiceFactory(Type serviceType);

    /// <summary>
    /// Inspiration came from https://github.com/jbogard/MediatR
    /// </summary>
    public class TypeFactory
    {
        private ServiceFactory _serviceFactory;
        private readonly ConfigurationContext _configurationContext;
        private readonly ConcurrentDictionary<Type, Type> _decoratorMappings = new ConcurrentDictionary<Type, Type>();
        private readonly ConcurrentDictionary<Type, (Type, ServiceFactory)> _mappings = new ConcurrentDictionary<Type, (Type, ServiceFactory)>();
        private readonly ConcurrentDictionary<Type, Type> _wrapperHandlerCache = new ConcurrentDictionary<Type, Type>();

        /// <summary>
        /// Creates new instance of the class.
        /// </summary>
        /// <param name="serviceFactory">Factory delegate for the services.</param>
        /// <param name="configurationContext"></param>
        public TypeFactory(ConfigurationContext configurationContext, ServiceFactory serviceFactory = null)
        {
            _serviceFactory = serviceFactory ?? ActivatorFactory;
            _configurationContext = configurationContext;
        }

        /// <summary>
        /// Used for <see cref="System.Activator" /> based factory.
        /// </summary>
        /// <param name="serviceType">Type of the service to create.</param>
        /// <returns>Service instance; otherwise throws various exceptions.</returns>
        internal object ActivatorFactory(Type serviceType)
        {
            var constructorInfo = serviceType.GetConstructor(new[] { typeof(ConfigurationContext) });

            return constructorInfo != null
                ? Activator.CreateInstance(serviceType, _configurationContext)
                : Activator.CreateInstance(serviceType);
        }

        internal void SetServiceFactory(ServiceFactory factory)
        {
            _serviceFactory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        internal ServiceFactory ServiceFactory => _serviceFactory;

        /// <summary>
        /// Start registration of the handler for query with this method.
        /// </summary>
        /// <typeparam name="TQuery">The type of the query.</typeparam>
        /// <returns></returns>
        public SetHandlerExpression<TQuery> ForQuery<TQuery>()
        {
            return new SetHandlerExpression<TQuery>(_mappings, _decoratorMappings, this);
        }

        /// <summary>
        /// Start registration of the handler for command with this method.
        /// </summary>
        /// <typeparam name="TCommand">The type of the command.</typeparam>
        /// <returns></returns>
        public SetHandlerExpression<TCommand> ForCommand<TCommand>() where TCommand : ICommand
        {
            return new SetHandlerExpression<TCommand>(_mappings, _decoratorMappings, this);
        }

        internal QueryHandlerWrapper<TResult> GetQueryHandler<TResult>(IQuery<TResult> query)
        {
            return GetQueryHandler<QueryHandlerWrapper<TResult>, TResult>(
                query,
                typeof(QueryHandlerWrapper<,>));
        }

        internal CommandHandlerWrapper GetCommandHandler<TCommand>(TCommand command) where TCommand : ICommand
        {
            return GetCommandHandler<CommandHandlerWrapper, TCommand>(
                command,
                typeof(CommandHandlerWrapper<>));
        }

        internal TWrapper GetCommandHandler<TWrapper, TCommand>(TCommand request, Type wrapperType) where TCommand : ICommand
        {
            var commandType = request.GetType();
            var genericWrapperType = _wrapperHandlerCache.GetOrAdd(
                commandType,
                wrapperType,
                (command, wrapper) => wrapper.MakeGenericType(command));

            var handler = GetHandler(commandType);

            return (TWrapper)Activator.CreateInstance(genericWrapperType, handler);
        }

        internal TWrapper GetQueryHandler<TWrapper, TResponse>(object request, Type wrapperType)
        {
            var requestType = request.GetType();
            var genericWrapperType = _wrapperHandlerCache.GetOrAdd(
                requestType,
                wrapperType,
                (query, wrapper) => wrapper.MakeGenericType(query, typeof(TResponse)));

            var handler = GetHandler(requestType);

            return (TWrapper)Activator.CreateInstance(genericWrapperType, handler);
        }

        internal object GetHandler(Type queryType)
        {
            var found = _mappings.TryGetValue(queryType, out var factory);
            if (!found)
            {
                throw new HandlerNotFoundException(
                    $"Failed to find handler for `{queryType}`. Make sure that you have invoked required registration methods (like context.UseSqlServer(), .etc..) on ConfigurationContext object in your app startup.");
            }

            var instance = factory.Item2(queryType);

            if(!_decoratorMappings.ContainsKey(queryType))
            {
                return instance;
            }

            var decoratorType = _decoratorMappings[queryType];
            var constructors = decoratorType
                .GetConstructors(BindingFlags.Public | BindingFlags.Instance)
                .OrderByDescending(c => c.GetParameters().Length)
                .First();

            // build parameter map
            var parameterList = new List<object>();
            var parameters = constructors.GetParameters();
            foreach (var parameterInfo in parameters)
            {
                if (IsAssignableToGenericType(parameterInfo.ParameterType, typeof(IQueryHandler<,>)))
                {
                    continue;
                }

                if (parameterInfo.ParameterType.IsAssignableFrom(typeof(ConfigurationContext)))
                {
                    parameterList.Add(_configurationContext);
                    continue;
                }

                var parameterInstance = _serviceFactory(parameterInfo.ParameterType);
                parameterList.Add(parameterInstance);
            }

            // add inner instance also to the list of the parameters for constructor
            parameterList.Insert(0, instance);

            return Activator.CreateInstance(decoratorType, parameterList.ToArray(), new object[] { });
        }

        internal IEnumerable<Type> GetAllHandlers()
        {
            return _mappings.Select(m => m.Value.Item1);
        }

        private static bool IsAssignableToGenericType(Type givenType, Type genericType)
        {
            var interfaceTypes = givenType.GetInterfaces();

            foreach (var it in interfaceTypes)
            {
                if (it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
                {
                    return true;
                }
            }

            if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
            {
                return true;
            }

            var baseType = givenType.BaseType;

            return baseType != null && IsAssignableToGenericType(baseType, genericType);
        }
    }
}

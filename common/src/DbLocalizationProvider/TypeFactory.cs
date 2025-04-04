// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Commands;
using DbLocalizationProvider.Commands.Internal;
using DbLocalizationProvider.Internal;
using DbLocalizationProvider.Queries;
using DbLocalizationProvider.Queries.Internal;
using Microsoft.Extensions.Options;

namespace DbLocalizationProvider;

/// <summary>
/// Callback for creating new instances of the handlers. Usually this delegate is replaced by <c>DependencyContainer.GetService</c> method.
/// </summary>
/// <param name="serviceType">Type of the service to create.</param>
/// <returns>Service instance if successful; otherwise <c>null</c>.</returns>
public delegate object? ServiceFactory(Type serviceType);

/// <summary>
/// Inspiration came from https://github.com/jbogard/MediatR
/// </summary>
public class TypeFactory
{
    private readonly IOptions<ConfigurationContext> _configurationContext;
    private readonly ConcurrentDictionary<Type, Type> _decoratorMappings = new();
    private readonly ConcurrentDictionary<Type, (Type, ServiceFactory)> _mappings = new();
    private readonly ConcurrentDictionary<Type, Type> _transientMappings = new();
    private readonly ConcurrentDictionary<Type, Type> _wrapperHandlerCache = new();

    /// <summary>
    /// Creates new instance of the class.
    /// </summary>
    /// <param name="serviceFactory">Factory delegate for the services.</param>
    /// <param name="configurationContext"></param>
    public TypeFactory(IOptions<ConfigurationContext> configurationContext, ServiceFactory? serviceFactory = null)
    {
        ServiceFactory = serviceFactory ?? ActivatorFactory;
        _configurationContext = configurationContext;

        // set default mappings (later anyone can override of course if needed)
        ForCommand<CreateNewResource.Command>().SetHandler<CreateNewResource.Handler>();
        ForCommand<CreateNewResources.Command>().SetHandler<CreateNewResources.Handler>();
        ForCommand<CreateOrUpdateTranslation.Command>().SetHandler<CreateOrUpdateTranslation.Handler>();
        ForCommand<DeleteAllResources.Command>().SetHandler<DeleteAllResources.Handler>();
        ForCommand<DeleteResource.Command>().SetHandler<DeleteResource.Handler>();
        ForCommand<RemoveTranslation.Command>().SetHandler<RemoveTranslation.Handler>();

        ForQuery<AvailableLanguages.Query>().SetHandler<AvailableLanguages.Handler>();
        ForQuery<GetCurrentUICulture.Query>().SetHandler<GetCurrentUICulture.Handler>();
        ForQuery<GetAllResources.Query>().SetHandler<GetAllResources.Handler>();
        ForQuery<GetResource.Query>().SetHandler<GetResource.Handler>();
        ForQuery<GetTranslation.Query>().SetHandler<GetTranslation.Handler>();
    }

    internal ServiceFactory ServiceFactory { get; private set; }

    /// <summary>
    /// Used for <see cref="System.Activator" /> based factory.
    /// </summary>
    /// <param name="serviceType">Type of the service to create.</param>
    /// <returns>Service instance; otherwise throws various exceptions.</returns>
    internal object? ActivatorFactory(Type serviceType)
    {
        var constructorInfo = serviceType.GetConstructor([typeof(IOptions<ConfigurationContext>)]);

        return constructorInfo != null
            ? Activator.CreateInstance(serviceType, _configurationContext)
            : Activator.CreateInstance(serviceType);
    }

    internal void SetServiceFactory(ServiceFactory factory)
    {
        ServiceFactory = factory ?? throw new ArgumentNullException(nameof(factory));
    }

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

    /// <summary>
    /// Adds a transient service of the type specified in <typeparamref name="TService" /> with an
    /// implementation type specified in <typeparamref name="TImplementation" />.
    /// </summary>
    /// <typeparam name="TService">The type of the service to add.</typeparam>
    /// <typeparam name="TImplementation">The type of the implementation to use.</typeparam>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public TypeFactory AddTransient<TService, TImplementation>()
        where TService : class
        where TImplementation : class, TService
    {
        _transientMappings.TryAdd(typeof(TService), typeof(TImplementation));

        return this;
    }

    /// <summary>
    /// Returns type of the handler for command or query.
    /// </summary>
    /// <typeparam name="T">Type of the command or query.</typeparam>
    /// <returns>Type of the handler; otherwise of course <c>null</c> if not found.</returns>
    public Type? GetHandlerType<T>()
    {
        if (!_mappings.ContainsKey(typeof(T)))
        {
            return null;
        }

        return _mappings.TryGetValue(typeof(T), out var info) ? info.Item1 : null;
    }

    internal QueryHandlerWrapper<TResult>? GetQueryHandler<TResult>(IQuery<TResult> query)
    {
        return GetQueryHandler<QueryHandlerWrapper<TResult>, TResult>(
            query,
            typeof(QueryHandlerWrapper<,>));
    }

    internal CommandHandlerWrapper? GetCommandHandler<TCommand>(TCommand command) where TCommand : ICommand
    {
        return GetCommandHandler<CommandHandlerWrapper, TCommand>(
            command,
            typeof(CommandHandlerWrapper<>));
    }

    internal TWrapper? GetCommandHandler<TWrapper, TCommand>(TCommand request, Type wrapperType)
        where TCommand : ICommand where TWrapper : class
    {
        var commandType = request.GetType();
        var genericWrapperType = _wrapperHandlerCache.GetOrAdd(
            commandType,
            wrapperType,
            (command, wrapper) => wrapper.MakeGenericType(command));

        var handler = GetHandler(commandType);

        return Activator.CreateInstance(genericWrapperType, handler) as TWrapper;
    }

    internal TWrapper? GetQueryHandler<TWrapper, TResponse>(object request, Type wrapperType) where TWrapper : class
    {
        var requestType = request.GetType();
        var genericWrapperType = _wrapperHandlerCache.GetOrAdd(
            requestType,
            wrapperType,
            (query, wrapper) => wrapper.MakeGenericType(query, typeof(TResponse)));

        var handler = GetHandler(requestType);

        return Activator.CreateInstance(genericWrapperType, handler) as TWrapper;
    }

    internal object? GetHandler(Type queryType)
    {
        var found = _mappings.TryGetValue(queryType, out var factory);
        if (!found)
        {
            throw new HandlerNotFoundException(
                $"Failed to find handler for `{queryType}`. Make sure that you have invoked required registration methods (like context.UseSqlServer(), .etc..) on ConfigurationContext object in your app startup.");
        }

        var instance = factory.Item2(queryType);

        if (!_decoratorMappings.TryGetValue(queryType, out var decoratorType))
        {
            return instance;
        }

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

            if (parameterInfo.ParameterType.IsAssignableFrom(typeof(IOptions<ConfigurationContext>)))
            {
                parameterList.Add(_configurationContext);
                continue;
            }

            var parameterInstance = ServiceFactory(parameterInfo.ParameterType);
            if (parameterInstance != null)
            {
                parameterList.Add(parameterInstance);
            }
        }

        // add inner instance also to the list of the parameters for constructor
        parameterList.Insert(0, instance!);

        return Activator.CreateInstance(decoratorType, parameterList.ToArray(), []);
    }

    internal IEnumerable<Type> GetAllHandlers()
    {
        return _mappings.Select(m => m.Value.Item1);
    }

    internal IDictionary<Type, Type> GetAllTransientServiceMappings()
    {
        return _transientMappings;
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

// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Concurrent;

namespace DbLocalizationProvider
{
    /// <summary>
    /// Helper for more fluent APIs
    /// </summary>
    /// <typeparam name="T">Type of the handler</typeparam>
    public class SetHandlerExpression<T>
    {
        private readonly ConcurrentDictionary<Type, Type> _decoratorMappings;
        private readonly ConcurrentDictionary<Type, Func<ConfigurationContext, object>> _mappings;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetHandlerExpression{T}" /> class.
        /// </summary>
        /// <param name="mappings">The mappings.</param>
        /// <param name="decoratorMappings">The decorator mappings.</param>
        public SetHandlerExpression(
            ConcurrentDictionary<Type, Func<ConfigurationContext, object>> mappings,
            ConcurrentDictionary<Type, Type> decoratorMappings)
        {
            _mappings = mappings;
            _decoratorMappings = decoratorMappings;
        }

        /// <summary>
        /// Sets the handler for specified command or query.
        /// </summary>
        /// <typeparam name="THandler">The type of the handler.</typeparam>
        public void SetHandler<THandler>()
        {
            _mappings.AddOrUpdate(typeof(T),
                                  ctx => TypeFactory.ActivatorFactory(typeof(THandler), ctx),
                                  (_, __) => ctx => TypeFactory.ActivatorFactory(typeof(THandler), ctx));
        }

        /// <summary>
        /// Sets the handler for the specified command or query.
        /// </summary>
        /// <typeparam name="THandler">The type of the handler.</typeparam>
        /// <param name="instanceFactory">The instance factory.</param>
        public void SetHandler<THandler>(Func<THandler> instanceFactory)
        {
            _mappings.AddOrUpdate(typeof(T), ctx => instanceFactory.Invoke(), (_, __) => ctx => instanceFactory.Invoke());
        }

        /// <summary>
        /// Decorates (adds interceptor) command or query.
        /// </summary>
        /// <typeparam name="TDecorator">The type of the decorator.</typeparam>
        public void DecorateWith<TDecorator>()
        {
            _decoratorMappings.GetOrAdd(typeof(T), typeof(TDecorator));
        }
    }
}

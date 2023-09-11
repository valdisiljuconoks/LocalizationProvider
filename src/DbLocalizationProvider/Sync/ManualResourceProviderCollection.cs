// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;

namespace DbLocalizationProvider.Sync
{
    /// <summary>
    /// Collection of manual resource providers.
    /// </summary>
    public class ManualResourceProviderCollection
    {
        /// <summary>
        /// Collection of manual resource providers.
        /// </summary>
        public ICollection<Type> Providers { get; } = new List<Type>();

        /// <summary>
        /// Creates new instance of manual resource provider collection.
        /// </summary>
        /// <typeparam name="T">Type of the manual resource.</typeparam>
        /// <returns>The same collection for easier chaining.</returns>
        public ManualResourceProviderCollection Add<T>() where T : IManualResourceProvider
        {
            Providers.Add(typeof(T));

            return this;
        }
    }
}

// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Cache
{
    /// <summary>
    /// When executed - configured <see cref="ConfigurationContext.CacheManager" /> should clear out all cached items from
    /// underlying storage.
    /// </summary>
    public class ClearCache
    {
        /// <summary>
        /// Actual command type of the <see cref="ClearCache" /> definition.
        /// </summary>
        public class Command : ICommand { }
    }
}

// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Queries
{
    /// <summary>
    /// Gets specific resource by given key.
    /// </summary>
    public class GetResource
    {
        /// <summary>
        /// Query definition for getting resource by given key.
        /// </summary>
        public class Query : IQuery<LocalizationResource>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Query"/> class.
            /// </summary>
            /// <param name="resourceKey">The resource key.</param>
            /// <exception cref="ArgumentNullException">resourceKey</exception>
            public Query(string resourceKey)
            {
                ResourceKey = resourceKey ?? throw new ArgumentNullException(nameof(resourceKey));
            }

            /// <summary>
            /// Gets the resource key.
            /// </summary>
            public string ResourceKey { get; }
        }
    }
}

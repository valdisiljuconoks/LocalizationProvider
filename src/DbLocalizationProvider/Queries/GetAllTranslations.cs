// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Collections.Generic;
using System.Globalization;
using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Queries
{
    /// <summary>
    /// Get all translations for resource
    /// </summary>
    public class GetAllTranslations
    {
        /// <summary>
        /// Query definition for getting all translations for given resource.
        /// </summary>
        public class Query : IQuery<IEnumerable<ResourceItem>>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Query"/> class.
            /// </summary>
            /// <param name="key">The key.</param>
            /// <param name="language">The language.</param>
            public Query(string key, CultureInfo language)
            {
                Key = key;
                Language = language;
            }

            /// <summary>
            /// Gets or sets the resource key.
            /// </summary>
            public string Key { get; set; }

            /// <summary>
            /// Gets or sets the language for the query.
            /// </summary>
            public CultureInfo Language { get; set; }
        }
    }
}

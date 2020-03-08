// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Globalization;
using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Queries
{
    /// <summary>
    /// Gets translation for given resource
    /// </summary>
    public class GetTranslation
    {
        /// <summary>
        /// Query definition to get translation for given resource
        /// </summary>
        public class Query : IQuery<string>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Query"/> class.
            /// </summary>
            /// <param name="key">The resource key.</param>
            /// <param name="language">The language to get translation in.</param>
            public Query(string key, CultureInfo language)
            {
                Key = key;
                Language = language;
            }

            /// <summary>
            /// Gets the key.
            /// </summary>
            public string Key { get; }

            /// <summary>
            /// Gets the language.
            /// </summary>
            public CultureInfo Language { get; }
        }
    }
}

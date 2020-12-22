// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Collections.Generic;
using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Queries
{
    /// <summary>
    /// When you need everything in one go
    /// </summary>
    public class GetAllResources
    {
        /// <summary>
        /// Query definition for getting all resources in one go
        /// </summary>
        public class Query : IQuery<IEnumerable<LocalizationResource>>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Query" /> class.
            /// </summary>
            /// <param name="forceReadFromDb">if set to <c>true</c> read from database is forced (skipping cache).</param>
            public Query(bool forceReadFromDb = false)
            {
                ForceReadFromDb = forceReadFromDb;
            }

            /// <summary>
            /// Gets a value indicating whether read from database should be forced.
            /// </summary>
            public bool ForceReadFromDb { get; }
        }
    }
}

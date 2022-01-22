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
        /// Retrieves single resource
        /// </summary>
        public class Handler : IQueryHandler<Query, LocalizationResource>
        {
            private readonly IResourceRepository _repository;

            /// <summary>
            /// Creates new instance of the class.
            /// </summary>
            /// <param name="repository">Resource repository</param>
            public Handler(IResourceRepository repository)
            {
                _repository = repository;
            }

            /// <summary>
            /// Place where query handling happens
            /// </summary>
            /// <param name="query">This is the query instance</param>
            /// <returns>
            /// You have to return something from the query execution. Of course you can return <c>null</c> as well if you
            /// will.
            /// </returns>
            public LocalizationResource Execute(Query query)
            {
                return _repository.GetByKey(query.ResourceKey);
            }
        }

        /// <summary>
        /// Query definition for getting resource by given key.
        /// </summary>
        public class Query : IQuery<LocalizationResource>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Query" /> class.
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

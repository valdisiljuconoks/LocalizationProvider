// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Collections.Generic;
using System.Threading.Tasks;
using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Queries
{
    /// <summary>
    /// When you need everything in one go
    /// </summary>
    public class GetAllResources
    {
        /// <summary>
        /// Reads all resources from underlying storage
        /// </summary>
        public class Handler : IQueryHandler<Query, IEnumerable<LocalizationResource>>
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
            public async Task<IEnumerable<LocalizationResource>> Execute(Query query)
            {
                return await _repository.GetAllAsync();
            }
        }


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

using System.Collections.Generic;
using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Queries
{
    /// <summary>
    /// When you need to search and pagination
    /// </summary>
    public class GetSearchResources
    {
        /// <summary>
        /// Query definition for getting all resources in one go
        /// </summary>
        public class Query : IQuery<QueryResult<IEnumerable<LocalizationResource>>>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="GetAllResources.Query"/> class.
            /// </summary>
            /// <param name="query"></param>
            /// <param name="page"></param>
            /// <param name="pageSize"></param>
            /// <param name="forceReadFromDb">if set to <c>true</c> read from database is forced (skipping cache).</param>
            public Query(string query, int? page, int? pageSize, bool forceReadFromDb = false)
            {
                ForceReadFromDb = forceReadFromDb;
                QueryString = query;
                Page = page;
                PageSize = pageSize;
            }

            /// <summary>
            /// Gets a value indicating whether read from database should be forced.
            /// </summary>
            public bool ForceReadFromDb { get; }

            /// <summary>
            /// Query string for filter
            /// </summary>
            public string QueryString { get; }

            /// <summary>
            /// Page
            /// </summary>
            public int? Page { get; }

            /// <summary>
            /// Page size
            /// </summary>
            public int? PageSize { get; }
        }
    }
}

using System.Collections.Generic;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Queries;

namespace DbLocalizationProvider.Storage.SqlServer
{
    /// <summary>
    /// Reads resources from underlying storage with pagination
    /// </summary>
    public class GetSearchResourcesHandler : IQueryHandler<GetSearchResources.Query, QueryResult<IEnumerable<LocalizationResource>>>
    {
        /// <summary>
        /// Place where query handling happens
        /// </summary>
        /// <param name="query">This is the query instance</param>
        /// <returns>
        /// You have to return something from the query execution. Of course you can return <c>null</c> as well if you
        /// will.
        /// </returns>
        public QueryResult<IEnumerable<LocalizationResource>> Execute(GetSearchResources.Query query)
        {
            var repository = new ResourceRepository();

            var searchResult = repository.Search(query.QueryString, query.Page, query.PageSize, out var rowCount);

            return new QueryResult<IEnumerable<LocalizationResource>>(searchResult, rowCount);
        }
    }
}

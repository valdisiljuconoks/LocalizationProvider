using System.Globalization;
using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Queries;

/// <summary>
/// Represents a query to get the current UI culture.
/// </summary>
public class GetCurrentUICulture
{
    /// <summary>
    /// Represents the query to get the current UI culture.
    /// </summary>
    public class Query : IQuery<CultureInfo> { }

    /// <summary>
    /// Handles the execution of the <see cref="Query"/> to get the current UI culture.
    /// </summary>
    public class Handler : IQueryHandler<Query, CultureInfo>
    {
        /// <summary>
        /// Executes the specified query to get the current UI culture.
        /// </summary>
        /// <param name="query">The query to execute.</param>
        /// <returns>The current UI culture.</returns>
        public CultureInfo Execute(Query query)
        {
            return CultureInfo.CurrentUICulture;
        }
    }
}

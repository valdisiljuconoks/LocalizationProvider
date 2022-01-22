using System.Globalization;
using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Queries
{
    public class GetCurrentUICulture
    {
        public class Query : IQuery<CultureInfo> { }

        public class Handler : IQueryHandler<Query, CultureInfo>
        {
            public CultureInfo Execute(Query query) => CultureInfo.CurrentUICulture;
        }
    }
}

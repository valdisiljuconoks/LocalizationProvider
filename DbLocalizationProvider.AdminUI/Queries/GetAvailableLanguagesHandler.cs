using System.Collections.Generic;
using System.Globalization;
using DbLocalizationProvider.Queries;

namespace DbLocalizationProvider.AdminUI.Queries
{
    public partial class GetAvailableLanguages
    {
        public class Handler : IQueryHandler<Query, IEnumerable<CultureInfo>>
        {
            public IEnumerable<CultureInfo> Execute(Query query)
            {
                return ConfigurationContext.Current.Repository.GetAvailableLanguages();
            }
        }
    }
}

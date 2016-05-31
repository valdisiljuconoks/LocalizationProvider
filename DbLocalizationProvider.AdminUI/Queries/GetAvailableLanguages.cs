using System.Collections.Generic;
using System.Globalization;
using DbLocalizationProvider.Queries;

namespace DbLocalizationProvider.AdminUI.Queries
{
    public partial class GetAvailableLanguages
    {
        public class Query : IQuery<IEnumerable<CultureInfo>> { }
    }
}

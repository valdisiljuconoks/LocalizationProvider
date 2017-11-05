using System.Collections.Generic;
using System.Globalization;

namespace DbLocalizationProvider.Queries
{
    public class AvailableLanguages
    {
        public class Query : IQuery<IEnumerable<CultureInfo>>
        {
            public bool IncludeInvariant { get; set; }
        }
    }
}

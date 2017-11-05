using System.Collections.Generic;
using System.Globalization;

namespace DbLocalizationProvider.Queries
{
    public class GetAllTranslations
    {
        public class Query : IQuery<IEnumerable<ResourceItem>>
        {
            public Query(string key, CultureInfo language)
            {
                Key = key;
                Language = language;
            }

            public string Key { get; set; }

            public CultureInfo Language { get; set; }
        }
    }
}

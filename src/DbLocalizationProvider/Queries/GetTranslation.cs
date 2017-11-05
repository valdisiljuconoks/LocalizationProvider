using System.Globalization;

namespace DbLocalizationProvider.Queries
{
    public class GetTranslation
    {
        public class Query : IQuery<string>
        {
            public Query(string key, CultureInfo language, bool useFallback)
            {
                Key = key;
                Language = language;
                UseFallback = useFallback;
            }

            public string Key { get; }

            public CultureInfo Language { get; }

            public bool UseFallback { get; }
        }
    }
}

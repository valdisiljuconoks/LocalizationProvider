using System.Globalization;

namespace DbLocalizationProvider.Queries
{
    public class GetTranslation
    {
        public class Query : IQuery<string>
        {
            public Query(string key, CultureInfo language)
            {
                Key = key;
                Language = language;
            }

            public string Key { get; set; }

            public CultureInfo Language { get; set; }
        }

        public class Handler : IQueryHandler<Query, string>
        {
            public string Execute(Query query)
            {
                var result = ConfigurationContext.Current.Repository.GetTranslation(query.Key, query.Language);

                if(result == null)
                {
                    return null;
                }

                return ConfigurationContext.Current.EnableLocalization() ? result : query.Key;
            }
        }
    }
}

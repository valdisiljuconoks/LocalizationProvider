using System.Collections.Generic;
using System.Globalization;
using System.Linq;

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

        public class Handler : IQueryHandler<Query, IEnumerable<ResourceItem>>
        {
            public IEnumerable<ResourceItem> Execute(Query query)
            {
                var q = new GetAllResources.Query();
                var allResources = q.Execute().Where(r =>
                                                         r.ResourceKey.StartsWith(query.Key) &&
                                                         r.Translations.Any(t => t.Language == query.Language.Name)).ToList();

                if(!allResources.Any())
                {
                    return Enumerable.Empty<ResourceItem>();
                }

                return allResources.Select(r => new ResourceItem(r.ResourceKey,
                                                                 r.Translations.First(t => t.Language == query.Language.Name).Value,
                                                                 query.Language)).ToList();
            }
        }
    }
}

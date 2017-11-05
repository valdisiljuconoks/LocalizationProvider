using System.Collections.Generic;

namespace DbLocalizationProvider.Queries
{
    public class GetAllResources
    {
        public class Query : IQuery<IEnumerable<LocalizationResource>> { }
    }
}

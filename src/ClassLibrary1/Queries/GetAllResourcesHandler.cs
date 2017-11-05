using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using DbLocalizationProvider.Queries;

namespace DbLocalizationProvider.AspNet.Queries
{
    public class GetAllResourcesHandler : IQueryHandler<GetAllResources.Query, IEnumerable<LocalizationResource>>
    {
        public IEnumerable<LocalizationResource> Execute(GetAllResources.Query query)
        {
            using(var db = new LanguageEntities())
            {
                return db.LocalizationResources.Include(r => r.Translations).ToList();
            }
        }
    }
}

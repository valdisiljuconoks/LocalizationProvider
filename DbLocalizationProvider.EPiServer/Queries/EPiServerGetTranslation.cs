using DbLocalizationProvider.Queries;
using EPiServer.Framework.Localization;
using EPiServer.ServiceLocation;

namespace DbLocalizationProvider.EPiServer.Queries
{
    public class EPiServerGetTranslation
    {
        public class Handler : IQueryHandler<GetTranslation.Query, string>
        {
            public string Execute(GetTranslation.Query query)
            {
                var service = ServiceLocator.Current.GetInstance<LocalizationService>();
                return service.GetStringByCulture(query.Key, query.Language);
            }
        }
    }
}

using DbLocalizationProvider.Queries;
using EPiServer.Framework.Localization;
using EPiServer.Logging;
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

        public class HandlerWithLogging : IQueryHandler<GetTranslation.Query, string>
        {
            private readonly GetTranslation.Handler _inner;
            private readonly ILogger _logger;

            public HandlerWithLogging(GetTranslation.Handler inner)
            {
                _inner = inner;
                _logger = LogManager.GetLogger(typeof(Handler));
            }

            public string Execute(GetTranslation.Query query)
            {
                var result = _inner.Execute(query);
                if(result == null)
                    _logger.Warning($"MISSING resource key (culture: {query.Language.Name}): {query.Key}");

                return result;
            }
        }
    }
}

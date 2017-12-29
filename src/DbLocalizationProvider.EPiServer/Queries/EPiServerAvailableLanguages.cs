using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Queries;
using EPiServer.DataAbstraction;
using EPiServer.Security;
using EPiServer.ServiceLocation;

namespace DbLocalizationProvider.EPiServer.Queries
{
    public class EPiServerAvailableLanguages
    {
        public class Handler : IQueryHandler<AvailableLanguages.Query, IEnumerable<CultureInfo>>
        {
            private readonly ILanguageBranchRepository _languageBranchRepository;

            public Handler()
            {
                _languageBranchRepository = ServiceLocator.Current.GetInstance<ILanguageBranchRepository>();
            }

            public IEnumerable<CultureInfo> Execute(AvailableLanguages.Query query)
            {
                var currentLanguages = _languageBranchRepository.ListEnabled()
                                                                .Where(l => l.QueryEditAccessRights(PrincipalInfo.CurrentPrincipal))
                                                                .Select(l => new CultureInfo(l.LanguageID));

                if(query.IncludeInvariant)
                    currentLanguages = new[] { CultureInfo.InvariantCulture }.Concat(currentLanguages);

                return currentLanguages;
            }
        }
    }
}

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
                return _languageBranchRepository.ListEnabled().Where(l => l.QueryEditAccessRights(PrincipalInfo.CurrentPrincipal))
                                                .Select(l => new CultureInfo(l.LanguageID));
            }
        }
    }
}

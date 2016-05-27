using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using EPiServer.DataAbstraction;
using EPiServer.Security;

namespace DbLocalizationProvider.AdminUI.EPiServer
{
    public class LanguageBranchProvider : IAvailableLanguagesProvider
    {
        private readonly LanguageBranchRepository _languageBranchRepository;

        public LanguageBranchProvider(LanguageBranchRepository languageBranchRepository)
        {
            _languageBranchRepository = languageBranchRepository;
        }

        public IEnumerable<CultureInfo> GetAll()
        {
            return _languageBranchRepository.ListEnabled()
                                            .Where(l => l.QueryEditAccessRights(PrincipalInfo.CurrentPrincipal))
                                            .Select(l => new CultureInfo(l.LanguageID));
        }
    }
}

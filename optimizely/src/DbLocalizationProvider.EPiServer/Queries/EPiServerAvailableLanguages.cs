// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Queries;
using EPiServer.DataAbstraction;
using EPiServer.Security;

namespace DbLocalizationProvider.EPiServer.Queries;

public class EPiServerAvailableLanguages
{
    public class Handler : IQueryHandler<AvailableLanguages.Query, IEnumerable<AvailableLanguage>>
    {
        private readonly ILanguageBranchRepository _languageBranchRepository;

        public Handler(ILanguageBranchRepository languageBranchRepository)
        {
            _languageBranchRepository = languageBranchRepository;
        }

        public IEnumerable<AvailableLanguage> Execute(AvailableLanguages.Query query)
        {
            var currentLanguages = _languageBranchRepository.ListEnabled()
                                                            .Where(l => l.QueryEditAccessRights(PrincipalInfo.CurrentPrincipal))
                                                            .Select(l => new AvailableLanguage(l.Name, l.SortIndex, l.Culture))
                                                            .ToList();

            if (query.IncludeInvariant)
            {
                currentLanguages.Insert(0, AvailableLanguage.Invariant);
            }

            return currentLanguages;
        }
    }
}

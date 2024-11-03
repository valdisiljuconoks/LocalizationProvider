// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Queries;

namespace DbLocalizationProvider.AdminUI.AspNetCore.Queries;

public class AvailableLanguagesHandler : IQueryHandler<AvailableLanguages.Query, IEnumerable<AvailableLanguage>>
{
    private readonly IEnumerable<AvailableLanguage> _supportedLanguages;

    public AvailableLanguagesHandler()
    {
        _supportedLanguages = new List<AvailableLanguage>();
    }

    public AvailableLanguagesHandler(IList<CultureInfo> supportedLanguages)
    {
        if (supportedLanguages == null)
        {
            throw new ArgumentNullException(nameof(supportedLanguages));
        }

        _supportedLanguages = supportedLanguages.Select((l, ix) => new AvailableLanguage(l.EnglishName, ix, l));
    }

    public IEnumerable<AvailableLanguage> Execute(AvailableLanguages.Query query)
    {
        return _supportedLanguages;
    }
}

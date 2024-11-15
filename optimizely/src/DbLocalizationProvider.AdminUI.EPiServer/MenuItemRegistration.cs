// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Collections.Generic;
using DbLocalizationProvider.AdminUI.AspNetCore.Security;
using DbLocalizationProvider.EPiServer;
using EPiServer.Framework.Localization;
using EPiServer.Shell;
using EPiServer.Shell.Navigation;

namespace DbLocalizationProvider.AdminUI.EPiServer;

[MenuProvider]
public class MenuProvider : IMenuProvider
{
    private readonly LocalizationService _localizationService;

    public MenuProvider(LocalizationService localizationService)
    {
        _localizationService = localizationService;
    }

    public IEnumerable<MenuItem> GetMenuItems()
    {
        var url = Paths.ToResource(GetType(), "uihostpage");

        var link = new UrlMenuItem(_localizationService.GetString(() => EPiServerResources.MenuTitle),
            MenuPaths.Global + "/cms/uihostpage",
            url)
        {
            SortIndex = 100, AuthorizationPolicy = AccessPolicy.Name
        };

        return new List<MenuItem> { link };
    }
}

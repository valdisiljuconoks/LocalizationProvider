// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Collections.Generic;
using DbLocalizationProvider.AdminUI.AspNetCore;
using DbLocalizationProvider.AdminUI.AspNetCore.Security;
using DbLocalizationProvider.EPiServer;
using EPiServer.Framework.Localization;
using EPiServer.Shell.Navigation;
using Microsoft.Extensions.Options;

namespace DbLocalizationProvider.AdminUI.EPiServer;

[MenuProvider]
public class MenuProvider(
    LocalizationService localizationService,
    IOptions<UiConfigurationContext> uiConfig) : IMenuProvider
{
    public IEnumerable<MenuItem> GetMenuItems()
    {
        var link = new UrlMenuItem(
            localizationService.GetString(() => EPiServerResources.MenuTitle),
            MenuPaths.Global + "/cms/uihostpage",
            uiConfig.Value.RootUrl + "-host")
        {
            SortIndex = 100,
            AuthorizationPolicy = AccessPolicy.Name
        };

        return [link];
    }
}

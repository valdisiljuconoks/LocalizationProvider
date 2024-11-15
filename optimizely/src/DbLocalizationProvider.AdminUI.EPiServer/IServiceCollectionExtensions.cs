// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Linq;
using DbLocalizationProvider.AdminUI.AspNetCore;
using EPiServer.Authorization;
using EPiServer.Cms.Shell;
using EPiServer.Shell.Modules;
using Microsoft.Extensions.DependencyInjection;

namespace DbLocalizationProvider.AdminUI.EPiServer;

/// <summary>
/// Home of the extensions method.
/// </summary>
public static class IServiceCollectionExtensions
{
    /// <summary>
    /// Adds localization provider AdminUI menu item in Optimizely administrative interface.
    /// </summary>
    /// <param name="builder">Localization provider builder.</param>
    /// <returns>Localization provider builder so you can chain further.</returns>
    public static IDbLocalizationProviderAdminUIBuilder AddOptimizelyAdminUI(this IDbLocalizationProviderAdminUIBuilder builder)
    {
        builder.Services.AddCmsUI();
        builder.Services.Configure<ProtectedModuleOptions>(
            pm =>
            {
                if (!pm.Items.Any(i => i.Name.Equals("DbLocalizationProvider.AdminUI.EPiServer", StringComparison.OrdinalIgnoreCase)))
                {
                    pm.Items.Add(new ModuleDetails { Name = "DbLocalizationProvider.AdminUI.EPiServer" });
                }
            });

        return builder;
    }
}

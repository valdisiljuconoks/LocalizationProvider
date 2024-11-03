// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using DbLocalizationProvider.Sync;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DbLocalizationProvider.AspNetCore;

/// <summary>
/// Extension to initialize and setup provider.
/// </summary>
public static class IServiceProviderExtensions
{
    /// <summary>
    /// Synchronizes resources with underlying storage
    /// </summary>
    /// <param name="serviceFactory">Factory of the services (this will be required to get access to previously registered services)</param>
    /// <returns>ASP.NET Core application builder to enable fluent API call chains</returns>
    public static void UseDbLocalizationProvider(this IServiceProvider serviceFactory)
    {
        if (serviceFactory == null)
        {
            throw new ArgumentNullException(nameof(serviceFactory));
        }

        var context = serviceFactory.GetRequiredService<IOptions<ConfigurationContext>>();

        // resolve inner cache (if set)
        if (context.Value._baseCacheManager._implementationFactory != null)
        {
            context.Value._baseCacheManager.SetInnerManager(context.Value._baseCacheManager._implementationFactory(serviceFactory));
        }

        var usageConfigurator = serviceFactory.GetService<IUsageConfigurator>();

        if (usageConfigurator != null)
        {
            usageConfigurator.Configure(context, serviceFactory);
        }

        // if we need to sync - then it's good time to do it now
        var sync = serviceFactory.GetRequiredService<Synchronizer>();
        sync.SyncResources(context.Value.DiscoverAndRegisterResources);

        if (!context.Value.DiscoverAndRegisterResources)
        {
            context.Value.Logger?.Info($"{nameof(context.Value.DiscoverAndRegisterResources)}=false. Resource synchronization skipped.");
        }

        foreach (var provider in serviceFactory.GetServices<IManualResourceProvider>())
        {
            sync.RegisterManually(provider.GetResources());
        }

        context.Value.Logger?.Info("DbLocalizationProvider initialized.");
    }
}

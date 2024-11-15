// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using DbLocalizationProvider.AspNetCore;
using EPiServer.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DbLocalizationProvider.EPiServer;

/// <inheritdoc />
public class OptimizelyUsageConfigurator : IUsageConfigurator
{
    /// <inheritdoc />
    public void Configure(IOptions<ConfigurationContext> context, IServiceProvider serviceProvider)
    {
        // here (after container creation) we can "finalize" some of the service setup procedures
        context.Value.Logger = new LoggerAdapter();

        // respect configuration whether we should sync and register resources
        // skip if application currently is in read-only mode
        var dbMode = serviceProvider.GetRequiredService<IDatabaseMode>().DatabaseMode;
        if (context.Value.DiscoverAndRegisterResources)
        {
            context.Value.DiscoverAndRegisterResources = dbMode != DatabaseMode.ReadOnly;
        }
    }
}

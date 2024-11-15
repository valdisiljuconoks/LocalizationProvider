// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Globalization;
using System.Linq;
using DbLocalizationProvider.AspNetCore;
using DbLocalizationProvider.EPiServer.Categories;
using DbLocalizationProvider.EPiServer.Queries;
using DbLocalizationProvider.Queries;
using DbLocalizationProvider.Sync;
using EPiServer.Framework.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DbLocalizationProvider.EPiServer;

/// <summary>
/// You have to have this placeholder class to define extension methods
/// </summary>
public static class IServiceCollectionExtensions
{
    /// <summary>
    /// Adds Optimizely support for DbLocalizationProvider.
    /// </summary>
    /// <param name="builder">Provider builder interface (to capture context and service collection).</param>
    /// <param name="setup">If required, modify configurations using the <see cref="DbLocalizationConfigurationContext"/></param>
    /// <returns>Service collection to support fluent API.</returns>
    public static IDbLocalizationProviderBuilder AddOptimizely(
        this IDbLocalizationProviderBuilder builder,
        Action<DbLocalizationConfigurationContext>? setup = null)
    {
        var configContext = new DbLocalizationConfigurationContext();

        setup?.Invoke(configContext);

        builder.Context._baseCacheManager.SetInnerManager(configContext.InnerCache);

        builder.Services.AddTransient<IResourceTypeScanner, LocalizedCategoryScanner>();

        // overriding handlers and registering those in DI container (so handler factory can later find them and create instance).
        builder.Context.TypeFactory.ForQuery<AvailableLanguages.Query>().SetHandler<EPiServerAvailableLanguages.Handler>();
        builder.Services.AddTransient<EPiServerAvailableLanguages.Handler>();

        builder.Context.TypeFactory.ForQuery<DetermineDefaultCulture.Query>().SetHandler<EPiServerDetermineDefaultCulture.Handler>();
        builder.Services.AddTransient<EPiServerDetermineDefaultCulture.Handler>();

        builder.Context.TypeFactory.ForQuery<GetCurrentUICulture.Query>().SetHandler<EPiServerGetCurrentUICulture.Handler>();
        builder.Services.AddTransient<EPiServerGetCurrentUICulture.Handler>();


        // if fallback list is empty - meaning that user has not configured anything
        // we can jump in and initialize config from Episerver settings
        if (!builder.Context.FallbackLanguages.Any()
            && LocalizationService.Current.FallbackBehavior.HasFlag(FallbackBehaviors.FallbackCulture)
            && !Equals(LocalizationService.Current.FallbackCulture, CultureInfo.InvariantCulture))
        {
            // read language fallback from the configuration file
            builder.Context.FallbackLanguages.Try(LocalizationService.Current.FallbackCulture);
        }

        builder.Services.AddLocalizationProvider<DatabaseLocalizationProvider>();

        builder.Services.Replace(
            new ServiceDescriptor(
                typeof(IUsageConfigurator),
                typeof(OptimizelyUsageConfigurator),
                ServiceLifetime.Singleton));

        return builder;
    }
}

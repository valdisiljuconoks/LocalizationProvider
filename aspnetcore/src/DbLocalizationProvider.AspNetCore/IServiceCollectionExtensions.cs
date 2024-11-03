// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Globalization;
using System.Linq;
using DbLocalizationProvider.AspNetCore.Cache;
using DbLocalizationProvider.AspNetCore.DataAnnotations;
using DbLocalizationProvider.AspNetCore.Queries;
using DbLocalizationProvider.Cache;
using DbLocalizationProvider.Internal;
using DbLocalizationProvider.Queries;
using DbLocalizationProvider.Refactoring;
using DbLocalizationProvider.Sync;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ILogger = DbLocalizationProvider.Logging.ILogger;

namespace DbLocalizationProvider.AspNetCore;

/// <summary>
/// Extension for adding localization provider services to the service collection.
/// </summary>
public static class IServiceCollectionExtensions
{
    /// <summary>
    /// Adds the database localization provider.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="setup">The setup callback.</param>
    /// <returns></returns>
    public static IDbLocalizationProviderBuilder AddDbLocalizationProvider(
        this IServiceCollection services,
        Action<ConfigurationContext>? setup = null)
    {
        var ctx = new ConfigurationContext(services);
        var factory = ctx.TypeFactory;

        // setup default implementations
        factory.ForQuery<GetAllResources.Query>().DecorateWith<CachedGetAllResourcesHandler>();
        factory.ForQuery<DetermineDefaultCulture.Query>().SetHandler<DetermineDefaultCulture.Handler>();
        factory.ForCommand<ClearCache.Command>().SetHandler<ClearCacheHandler>();

        // set to default in-memory provider
        // only if we have IMemoryCache service registered
        var memCacheDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IMemoryCache));
        if (memCacheDescriptor is { ServiceType: not null })
        {
            ctx._baseCacheManager.SetInnerManager(sp => new InMemoryCache((IMemoryCache)sp.GetRequiredService(memCacheDescriptor.ServiceType)));
        }

        // run custom configuration setup (if any)
        setup?.Invoke(ctx);

        services.AddSingleton(ctx.CacheManager);

        services.AddSingleton<ScanState>();
        services.AddSingleton<ResourceKeyBuilder>();
        services.AddSingleton<OldResourceKeyBuilder>();
        services.AddSingleton<ExpressionHelper>();
        services.AddSingleton<QueryExecutor>();
        services.AddSingleton<IQueryExecutor>(sp => sp.GetRequiredService<QueryExecutor>());
        services.AddSingleton<CommandExecutor>();
        services.AddSingleton<ICommandExecutor>(sp => sp.GetRequiredService<CommandExecutor>());
        services.AddSingleton<DiscoveredTranslationBuilder>();
        
        services
            .AddOptions<ConfigurationContext>()
            .Configure(x =>
            {
                x.CopyFrom(ctx);
            });


        services.AddSingleton(sp =>
        {
            factory.SetServiceFactory(sp.GetService);
            return factory;
        });

        // add all registered handlers to DI (in order to use service factory callback from DI lib)
        foreach (var handler in factory.GetAllHandlers())
        {
            services.AddTransient(handler);
        }

        // add all registered handlers to DI (in order to use service factory callback from DI lib)
        foreach (var (service, implementation) in factory.GetAllTransientServiceMappings())
        {
            services.AddTransient(service, implementation);
        }

        services.AddSingleton<ILogger>(p => new LoggerAdapter(p.GetService<ILogger<LoggerAdapter>>()));

        services.AddSingleton<TypeDiscoveryHelper>();
        services.AddTransient<IResourceTypeScanner, LocalizedModelTypeScanner>();
        services.AddTransient<IResourceTypeScanner, LocalizedResourceTypeScanner>();
        services.AddTransient<IResourceTypeScanner, LocalizedEnumTypeScanner>();
        services.AddTransient<IResourceTypeScanner, LocalizedForeignResourceTypeScanner>();

        services.AddSingleton<LocalizationProvider>();
        services.AddSingleton<ILocalizationProvider>(sp => sp.GetRequiredService<LocalizationProvider>());

        services.AddTransient<ISynchronizer, Synchronizer>();
        services.AddTransient<Synchronizer>();

        services.AddSingleton<DbStringLocalizerFactory>();
        services.AddSingleton<IStringLocalizerFactory>(p => p.GetRequiredService<DbStringLocalizerFactory>());
        services.AddSingleton<DbHtmlLocalizerFactory>();
        services.AddSingleton<IHtmlLocalizerFactory>(p => p.GetRequiredService<DbHtmlLocalizerFactory>());
        services.AddTransient<IViewLocalizer, DbViewLocalizer>();
        services.AddTransient(typeof(IHtmlLocalizer<>), typeof(DbHtmlLocalizer<>));

        // we need to check whether invariant fallback is correctly configured
        if (ctx.EnableInvariantCultureFallback && !ctx.FallbackLanguages.Contains(CultureInfo.InvariantCulture))
        {
            ctx.FallbackLanguages.Then(CultureInfo.InvariantCulture);
        }

        // add manual resource providers
        foreach (var providerType in ctx.ManualResourceProviders.Providers)
        {
            services.TryAddEnumerable(ServiceDescriptor.Transient(typeof(IManualResourceProvider), providerType));
        }

        // setup model metadata providers
        if (ctx.ModelMetadataProviders.ReplaceProviders)
        {
            services.TryAddEnumerable(
                ServiceDescriptor.Transient<IConfigureOptions<MvcOptions>, ConfigureModelMetadataDetailsProviders>());
            services.AddSingleton<IValidationAttributeAdapterProvider, LocalizedAttributeAdapterProvider>();
            services.TryAddEnumerable(
                ServiceDescriptor
                    .Transient<IConfigureOptions<MvcDataAnnotationsLocalizationOptions>, DataAnnotationLocalizerProviderSetup>());
        }

        services.AddHttpContextAccessor();

        return new DbLocalizationProviderBuilder(services, ctx);
    }
}

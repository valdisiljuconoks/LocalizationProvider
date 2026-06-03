using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Toolchains.InProcess.NoEmit;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Internal;
using DbLocalizationProvider.Logging;
using DbLocalizationProvider.Queries;
using DbLocalizationProvider.Sync;
using Microsoft.Extensions.Options;

namespace DbLocalizationProvider.Benchmarks;

[Config(typeof(InProcessConfig))]
public class HotPathBenchmarks
{
    // Key produced by ExpressionHelper for `() => SampleResources.Welcome` is the
    // declaring type's full name joined with the property name.
    private const string KnownKey = "DbLocalizationProvider.Benchmarks.SampleResources.Welcome";

    private LocalizationProvider _provider = null!;
    private CultureInfo _english = null!;
    private CultureInfo _frenchBE = null!;
    private Expression<Func<object>> _expression = null!;

    [GlobalSetup]
    public void Setup()
    {
        var ctx = new ConfigurationContext();
        var wrapper = new OptionsWrapper<ConfigurationContext>(ctx);
        var keyBuilder = new ResourceKeyBuilder(new ScanState(), wrapper);
        var expressionHelper = new ExpressionHelper(keyBuilder);
        var logger = new NullLogger();

        // Fallback chain: fr-BE -> fr -> en
        ctx.FallbackLanguages
            .When(CultureInfo.GetCultureInfo("fr-BE"))
            .Try(CultureInfo.GetCultureInfo("fr"))
            .Then(CultureInfo.GetCultureInfo("en"));

        var resource = new LocalizationResource(KnownKey, false);
        resource.Translations.Add(new LocalizationResourceTranslation { Language = "en", Value = "Welcome" });
        resource.Translations.Add(new LocalizationResourceTranslation { Language = "fr", Value = "Bienvenue" });

        var store = new Dictionary<string, LocalizationResource>(StringComparer.OrdinalIgnoreCase)
        {
            [KnownKey] = resource
        };

        IQueryExecutor queryExecutor = new QueryExecutor(ctx.TypeFactory);

        // Plug in a tiny service factory so default handlers that need IOptions/IQueryExecutor/ILogger
        // can be constructed (we are not running through Microsoft.Extensions.DependencyInjection here).
        object? ResolveService(Type serviceType)
        {
            if (serviceType == typeof(IOptions<ConfigurationContext>)) return wrapper;
            if (serviceType == typeof(IQueryExecutor)) return queryExecutor;
            if (serviceType == typeof(ILogger)) return logger;

            var ctor = serviceType.GetConstructors()[0];
            var parameters = ctor.GetParameters();
            if (parameters.Length == 0)
            {
                return Activator.CreateInstance(serviceType);
            }

            var args = new object?[parameters.Length];
            for (var i = 0; i < parameters.Length; i++)
            {
                args[i] = ResolveService(parameters[i].ParameterType);
            }
            return ctor.Invoke(args);
        }

        ctx.TypeFactory.SetServiceFactory(ResolveService);

        // Replace storage with an in-memory dictionary so the cache-miss code path
        // stays in-process; the benchmarks measure the cache-hit path.
        ctx.TypeFactory.ForQuery<GetResource.Query>()
            .SetHandler(() => new InMemoryResourceHandler(store));

        _provider = new LocalizationProvider(keyBuilder, expressionHelper, wrapper, queryExecutor, new ScanState());
        _english = CultureInfo.GetCultureInfo("en");
        _frenchBE = CultureInfo.GetCultureInfo("fr-BE");
        _expression = () => SampleResources.Welcome;

        // Warm the cache so subsequent calls measure the hit path.
        _provider.GetString(KnownKey, _english);
        _provider.GetString(KnownKey, _frenchBE);
    }

    [Benchmark(Description = "string-key, exact culture (cache hit)")]
    public string? GetString_StringKey_CacheHit()
    {
        return _provider.GetString(KnownKey, _english);
    }

    [Benchmark(Description = "string-key, fr-BE -> fr fallback walk (cache hit)")]
    public string? GetString_StringKey_FallbackWalk()
    {
        return _provider.GetString(KnownKey, _frenchBE);
    }

    [Benchmark(Description = "expression-key, exact culture (cache hit)")]
    public string? GetString_Expression_CacheHit()
    {
        return _provider.GetStringByCulture(_expression, _english, Array.Empty<object>());
    }

    [Benchmark(Description = "expression-key, exact culture, no format args overload")]
    public string? GetString_Expression_CacheHit_NoArgs()
    {
        return _provider.GetStringByCulture(_expression, _english);
    }

    private sealed class InMemoryResourceHandler(Dictionary<string, LocalizationResource> store)
        : IQueryHandler<GetResource.Query, LocalizationResource?>
    {
        public LocalizationResource? Execute(GetResource.Query query)
        {
            return store.GetValueOrDefault(query.ResourceKey);
        }
    }

    private sealed class InProcessConfig : ManualConfig
    {
        public InProcessConfig()
        {
            AddJob(Job.Default.WithToolchain(InProcessNoEmitToolchain.Instance));
            AddDiagnoser(MemoryDiagnoser.Default);
        }
    }
}

[LocalizedResource]
public static class SampleResources
{
    public static string Welcome => "Welcome";
}

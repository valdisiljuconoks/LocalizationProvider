using System;
using System.Collections.Generic;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Cache;

namespace DbLocalizationProvider.Benchmarks;

/// <summary>
/// Measures retained heap of the per-key translation cache after seeding N resources × M cultures.
/// Not a normal BenchmarkDotNet benchmark - allocation/GC tooling reports allocations during a
/// micro-benchmark, but we care about long-lived footprint here. Invoke from <see cref="Program"/>
/// with the <c>--heap</c> flag.
/// </summary>
internal static class HeapRetentionMeasurement
{
    public static void Run(int resourceCount = 10_000, int cultureCount = 8)
    {
        var languages = new[] { "en", "fr", "de", "es", "it", "lv", "ru", "fi" };
        if (cultureCount > languages.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(cultureCount));
        }

        Console.WriteLine($"Seeding {resourceCount} resources × {cultureCount} cultures and measuring retained heap.");
        Console.WriteLine();

        MeasureLegacyShape(resourceCount, cultureCount, languages);
        MeasureCompactShape(resourceCount, cultureCount, languages);
    }

    private static void MeasureLegacyShape(int resourceCount, int cultureCount, string[] languages)
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        var before = GC.GetTotalMemory(true);

        var store = new Dictionary<string, LocalizationResource>(resourceCount, StringComparer.OrdinalIgnoreCase);
        for (var i = 0; i < resourceCount; i++)
        {
            var key = "App.Resources.Resource" + i;
            var resource = new LocalizationResource(key, false);
            for (var c = 0; c < cultureCount; c++)
            {
                resource.Translations.Add(new LocalizationResourceTranslation
                {
                    Language = languages[c],
                    Value = "Translation " + i + " " + c
                });
            }
            store[key] = resource;
        }

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        var after = GC.GetTotalMemory(true);

        Console.WriteLine($"Legacy LocalizationResource graph: {(after - before) / 1024.0 / 1024.0:F2} MB " +
                          $"({(after - before) / resourceCount} bytes/resource)");
        GC.KeepAlive(store);
    }

    private static void MeasureCompactShape(int resourceCount, int cultureCount, string[] languages)
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        var before = GC.GetTotalMemory(true);

        var store = new Dictionary<string, CachedTranslations>(resourceCount, StringComparer.OrdinalIgnoreCase);
        for (var i = 0; i < resourceCount; i++)
        {
            var key = "App.Resources.Resource" + i;
            var resource = new LocalizationResource(key, false);
            for (var c = 0; c < cultureCount; c++)
            {
                resource.Translations.Add(new LocalizationResourceTranslation
                {
                    Language = languages[c],
                    Value = "Translation " + i + " " + c
                });
            }
            store[key] = CachedTranslations.From(resource);
        }

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        var after = GC.GetTotalMemory(true);

        Console.WriteLine($"Compact CachedTranslations:        {(after - before) / 1024.0 / 1024.0:F2} MB " +
                          $"({(after - before) / resourceCount} bytes/resource)");
        GC.KeepAlive(store);
    }
}

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Logging;
using DbLocalizationProvider.Sync;
using Xunit;

namespace DbLocalizationProvider.Storage.PostgreSql.Tests.ResourceSynchronizedTests;

public class Tests
{
    private readonly Synchronizer _sut;

    public Tests()
    {
        var ctx = new ConfigurationContext();
        _sut = new Synchronizer(
            new TypeDiscoveryHelper(Enumerable.Empty<IResourceTypeScanner>(), ctx),
            new QueryExecutor(ctx.TypeFactory),
            new CommandExecutor(ctx.TypeFactory),
            new ResourceRepository(ctx),
            new NullLogger(),
            ctx);
    }

    [Fact]
    public void Merge_AllDifferentResources_ShouldKeepAll()
    {
        var db = new List<LocalizationResource>
        {
            new("key-from-db", false)
            {
                Translations = new LocalizationResourceTranslationCollection(false)
                {
                    new() { Language = "en", Value = "English from DB" }
                }
            }
        };

        var resources = new List<DiscoveredResource>
        {
            new(null,
                "discovered-resource",
                new List<DiscoveredTranslation> { new("English discovered resource", "en") },
                "",
                null,
                null,
                false)
        };

        var models = new List<DiscoveredResource>
        {
            new(null,
                "discovered-model",
                new List<DiscoveredTranslation> { new("English discovered model", "en") },
                "",
                null,
                null,
                false)
        };

        var result = _sut.MergeLists(db, resources, models);

        Assert.NotEmpty(result);
        Assert.Equal(3, result.Count());
    }

    [Fact]
    public void Merge_DatabaseContainsDiscoveredResource_NotModified_ShouldOverwrite_IncludingInvariant()
    {
        var db = new List<LocalizationResource>
        {
            new("resource-key-1", false)
            {
                Translations = new LocalizationResourceTranslationCollection(false)
                {
                    new() { Language = string.Empty, Value = "Resource-1 INVARIANT from DB" },
                    new() { Language = "en", Value = "Resource-1 English from DB" }
                }
            },
            new("resource-key-2", false)
            {
                Translations = new LocalizationResourceTranslationCollection(false)
                {
                    new() { Language = string.Empty, Value = "Resource-2 INVARIANT from DB" },
                    new() { Language = "en", Value = "Resource-2 English from DB" }
                }
            }
        };

        var resources = new List<DiscoveredResource>
        {
            new(null,
                "resource-key-1",
                new List<DiscoveredTranslation>
                {
                    new("Resource-1 INVARIANT from Discovery", string.Empty),
                    new("Resource-1 English from Discovery", "en")
                },
                "",
                null,
                null,
                false),
            new(null,
                "discovered-resource",
                new List<DiscoveredTranslation> { new("English discovered resource", "en") },
                "",
                null,
                null,
                false)
        };

        var models = new List<DiscoveredResource>
        {
            new(null,
                "discovered-model",
                new List<DiscoveredTranslation> { new("English discovered model", "en") },
                "",
                null,
                null,
                false),
            new(null,
                "resource-key-2",
                new List<DiscoveredTranslation>
                {
                    new("Resource-2 INVARIANT from Discovery", string.Empty),
                    new("Resource-2 English from Discovery", "en")
                },
                "",
                null,
                null,
                false)
        };

        var result = _sut.MergeLists(db, resources, models);

        Assert.NotEmpty(result);
        Assert.Equal(4, result.Count());
        Assert.Equal("Resource-1 INVARIANT from Discovery",
                     result.First(r => r.ResourceKey == "resource-key-1").Translations.ByLanguage(CultureInfo.InvariantCulture));
        Assert.Equal("Resource-1 English from Discovery",
                     result.First(r => r.ResourceKey == "resource-key-1").Translations.ByLanguage("en"));
        Assert.Equal("Resource-2 INVARIANT from Discovery",
                     result.First(r => r.ResourceKey == "resource-key-2").Translations.ByLanguage(CultureInfo.InvariantCulture));
        Assert.Equal("Resource-2 English from Discovery",
                     result.First(r => r.ResourceKey == "resource-key-2").Translations.ByLanguage("en"));
    }

    [Fact]
    public void Merge_DatabaseContainsDiscoveredResource_Modified_ShouldNotOverwrite_ShouldOverwriteInvariant()
    {
        var db = new List<LocalizationResource>
        {
            new("resource-key-1", false)
            {
                IsModified = true,
                Translations =
                    new LocalizationResourceTranslationCollection(false)
                    {
                        new() { Language = string.Empty, Value = "Resource-1 INVARIANT from DB" },
                        new() { Language = "en", Value = "Resource-1 English from DB" }
                    }
            },
            new("resource-key-2", false)
            {
                IsModified = true,
                Translations = new LocalizationResourceTranslationCollection(false)
                {
                    new() { Language = string.Empty, Value = "Resource-2 INVARIANT from DB" },
                    new() { Language = "en", Value = "Resource-2 English from DB" }
                }
            }
        };

        var resources = new List<DiscoveredResource>
        {
            new(null,
                "resource-key-1",
                new List<DiscoveredTranslation>
                {
                    new("Resource-1 INVARIANT from Discovery", string.Empty),
                    new("Resource-1 English from Discovery", "en")
                },
                "",
                null,
                null,
                false),
            new(null,
                "discovered-resource",
                new List<DiscoveredTranslation> { new("English discovered resource", "en") },
                "",
                null,
                null,
                false)
        };

        var models = new List<DiscoveredResource>
        {
            new(null,
                "discovered-model",
                new List<DiscoveredTranslation> { new("English discovered model", "en") },
                "",
                null,
                null,
                false),
            new(null,
                "resource-key-2",
                new List<DiscoveredTranslation>
                {
                    new("Resource-2 INVARIANT from Discovery", string.Empty),
                    new("Resource-2 English from Discovery", "en")
                },
                "",
                null,
                null,
                false)
        };

        var result = _sut.MergeLists(db, resources, models);

        Assert.NotEmpty(result);
        Assert.Equal(4, result.Count());
        Assert.Equal("Resource-1 INVARIANT from Discovery",
                     result.First(r => r.ResourceKey == "resource-key-1").Translations.ByLanguage(CultureInfo.InvariantCulture));
        Assert.Equal("Resource-1 English from DB",
                     result.First(r => r.ResourceKey == "resource-key-1").Translations.ByLanguage("en"));
        Assert.Equal("Resource-2 INVARIANT from Discovery",
                     result.First(r => r.ResourceKey == "resource-key-2").Translations.ByLanguage(CultureInfo.InvariantCulture));
        Assert.Equal("Resource-2 English from DB",
                     result.First(r => r.ResourceKey == "resource-key-2").Translations.ByLanguage("en"));
    }
}

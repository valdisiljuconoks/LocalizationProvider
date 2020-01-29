using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DbLocalizationProvider.Sync;
using Xunit;

namespace DbLocalizationProvider.Storage.SqlServer.Tests.ResourceSynchronizedTests
{
    public class Tests
    {
        [Fact]
        public void MergeEmptyLists()
        {
            var sut = new ResourceSynchronizer();

            var result = sut.MergeLists(Enumerable.Empty<LocalizationResource>(), null, null);

            Assert.Empty(result);
        }

        [Fact]
        public void Merge_AllDifferentResources_ShouldKeepAll()
        {
            var sut = new ResourceSynchronizer();
            var db = new List<LocalizationResource>
            {
                new LocalizationResource("key-from-db")
                {
                    Translations = new List<LocalizationResourceTranslation>()
                    {
                        new LocalizationResourceTranslation
                        {
                            Language = "en", Value = "English from DB"
                        }
                    }
                }
            };

            var resources = new List<DiscoveredResource>
            {
                new DiscoveredResource(null, "discovered-resource", new List<DiscoveredTranslation> { new DiscoveredTranslation("English discovered resource", "en") }, "", null, null, false, false)
            };

            var models = new List<DiscoveredResource>
            {
                new DiscoveredResource(null, "discovered-model", new List<DiscoveredTranslation> { new DiscoveredTranslation("English discovered model", "en") }, "", null, null, false, false)
            };

            var result = sut.MergeLists(db, resources, models);

            Assert.NotEmpty(result);
            Assert.Equal(3, result.Count());
        }

        [Fact]
        public void Merge_DatabaseContainsDiscoveredResource_NotModified_ShouldOverwrite_IncludingInvariant()
        {
            var sut = new ResourceSynchronizer();
            var db = new List<LocalizationResource>
            {
                new LocalizationResource("resource-key-1")
                {
                    Translations = new List<LocalizationResourceTranslation>
                    {
                        new LocalizationResourceTranslation
                        {
                            Language = string.Empty, Value = "Resource-1 INVARIANT from DB"
                        },
                        new LocalizationResourceTranslation
                        {
                            Language = "en", Value = "Resource-1 English from DB"
                        }
                    }
                },
                new LocalizationResource("resource-key-2")
                {
                    Translations = new List<LocalizationResourceTranslation>
                    {
                        new LocalizationResourceTranslation
                        {
                            Language = string.Empty, Value = "Resource-2 INVARIANT from DB"
                        },
                        new LocalizationResourceTranslation
                        {
                            Language = "en", Value = "Resource-2 English from DB"
                        }
                    }
                }
            };

            var resources = new List<DiscoveredResource>
            {
                new DiscoveredResource(null, "resource-key-1", new List<DiscoveredTranslation> { new DiscoveredTranslation("Resource-1 INVARIANT from Discovery", string.Empty), new DiscoveredTranslation("Resource-1 English from Discovery", "en") }, "", null, null, false, false),
                new DiscoveredResource(null, "discovered-resource", new List<DiscoveredTranslation> { new DiscoveredTranslation("English discovered resource", "en") }, "", null, null, false, false)
            };

            var models = new List<DiscoveredResource>
            {
                new DiscoveredResource(null, "discovered-model", new List<DiscoveredTranslation> { new DiscoveredTranslation("English discovered model", "en") }, "", null, null, false, false),
                new DiscoveredResource(null, "resource-key-2", new List<DiscoveredTranslation> { new DiscoveredTranslation("Resource-2 INVARIANT from Discovery", string.Empty), new DiscoveredTranslation("Resource-2 English from Discovery", "en") }, "", null, null, false, false)
            };

            var result = sut.MergeLists(db, resources, models);

            Assert.NotEmpty(result);
            Assert.Equal(4, result.Count());
            Assert.Equal("Resource-1 INVARIANT from Discovery", result.First(r => r.ResourceKey == "resource-key-1").Translations.ByLanguage(CultureInfo.InvariantCulture));
            Assert.Equal("Resource-1 English from Discovery", result.First(r => r.ResourceKey == "resource-key-1").Translations.ByLanguage("en"));
            Assert.Equal("Resource-2 INVARIANT from Discovery", result.First(r => r.ResourceKey == "resource-key-2").Translations.ByLanguage(CultureInfo.InvariantCulture));
            Assert.Equal("Resource-2 English from Discovery", result.First(r => r.ResourceKey == "resource-key-2").Translations.ByLanguage("en"));
        }

        [Fact]
        public void Merge_DatabaseContainsDiscoveredResource_Modified_ShouldNotOverwrite_ShouldOverwriteInvariant()
        {
            var sut = new ResourceSynchronizer();
            var db = new List<LocalizationResource>
            {
                new LocalizationResource("resource-key-1")
                {
                    IsModified = true,
                    Translations = new List<LocalizationResourceTranslation>
                    {
                        new LocalizationResourceTranslation
                        {
                            Language = string.Empty, Value = "Resource-1 INVARIANT from DB"
                        },
                        new LocalizationResourceTranslation
                        {
                            Language = "en", Value = "Resource-1 English from DB"
                        }
                    }
                },
                new LocalizationResource("resource-key-2")
                {
                    IsModified = true,
                    Translations = new List<LocalizationResourceTranslation>
                    {
                        new LocalizationResourceTranslation
                        {
                            Language = string.Empty, Value = "Resource-2 INVARIANT from DB"
                        },
                        new LocalizationResourceTranslation
                        {
                            Language = "en", Value = "Resource-2 English from DB"
                        }
                    }
                }
            };

            var resources = new List<DiscoveredResource>
            {
                new DiscoveredResource(null, "resource-key-1", new List<DiscoveredTranslation> { new DiscoveredTranslation("Resource-1 INVARIANT from Discovery", string.Empty), new DiscoveredTranslation("Resource-1 English from Discovery", "en") }, "", null, null, false, false),
                new DiscoveredResource(null, "discovered-resource", new List<DiscoveredTranslation> { new DiscoveredTranslation("English discovered resource", "en") }, "", null, null, false, false)
            };

            var models = new List<DiscoveredResource>
            {
                new DiscoveredResource(null, "discovered-model", new List<DiscoveredTranslation> { new DiscoveredTranslation("English discovered model", "en") }, "", null, null, false, false),
                new DiscoveredResource(null, "resource-key-2", new List<DiscoveredTranslation> { new DiscoveredTranslation("Resource-2 INVARIANT from Discovery", string.Empty), new DiscoveredTranslation("Resource-2 English from Discovery", "en") }, "", null, null, false, false)
            };

            var result = sut.MergeLists(db, resources, models);

            Assert.NotEmpty(result);
            Assert.Equal(4, result.Count());
            Assert.Equal("Resource-1 INVARIANT from Discovery", result.First(r => r.ResourceKey == "resource-key-1").Translations.ByLanguage(CultureInfo.InvariantCulture));
            Assert.Equal("Resource-1 English from DB", result.First(r => r.ResourceKey == "resource-key-1").Translations.ByLanguage("en"));
            Assert.Equal("Resource-2 INVARIANT from Discovery", result.First(r => r.ResourceKey == "resource-key-2").Translations.ByLanguage(CultureInfo.InvariantCulture));
            Assert.Equal("Resource-2 English from DB", result.First(r => r.ResourceKey == "resource-key-2").Translations.ByLanguage("en"));
        }
    }
}

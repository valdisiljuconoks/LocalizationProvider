using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Internal;
using DbLocalizationProvider.Json;
using DbLocalizationProvider.Queries;
using DbLocalizationProvider.Sync;
using Microsoft.Extensions.Options;
using Xunit;

namespace DbLocalizationProvider.Tests.JsonConverterTests;

public class JsonConverterTests
{
    [Fact]
    public void VariousResources_WithMixedKeyNames()
    {
        var resources = new List<LocalizationResource>
        {
            new("/another/mixmatch/key", false)
            {
                Translations = new LocalizationResourceTranslationCollection(false)
                {
                    new() { Language = "en", Value = "this is another english" }
                }
            },
            new("This.Is.Resource.Key", false)
            {
                Translations = new LocalizationResourceTranslationCollection(false)
                {
                    new() { Language = "en", Value = "this is english" },
                    new() { Language = "no", Value = "this is norsk" }
                }
            },
            new("This.Is.AnotherResource.Key", false)
            {
                Translations = new LocalizationResourceTranslationCollection(false)
                {
                    new() { Language = "en", Value = "this is another english" },
                    new() { Language = "no", Value = "this is another norsk" }
                }
            },
            new("This.Totally.Another.Resource.Key", false)
            {
                Translations = new LocalizationResourceTranslationCollection(false)
                {
                    new() { Language = "en", Value = "this totally another english" },
                    new() { Language = "no", Value = "this totally another norsk" }
                }
            },
            new("This.Is.Back.Resource.Key", false)
            {
                Translations = new LocalizationResourceTranslationCollection(false)
                {
                    new() { Language = "en", Value = "this is back english" },
                    new() { Language = "no", Value = "this is back norsk" }
                }
            },
            new("This.Resource.Is.The.Last.Resource", false)
            {
                Translations = new LocalizationResourceTranslationCollection(false)
                {
                    new() { Language = "en", Value = "last" }, new() { Language = "no", Value = "siste" }
                }
            }
        };

        var sut = new JsonConverter(new QueryExecutor(new ConfigurationContext().TypeFactory), new ScanState());

        var resourcesAsJson = sut.Convert(resources, "en", CultureInfo.InvariantCulture, false);

        Assert.Equal("this is english", resourcesAsJson["This"]["Is"]["Resource"]["Key"]);

        Assert.Single(resourcesAsJson);
    }

    [Fact]
    public void VariousResourcesWithNorskTranslation_RequestedEnglishWithoutFallback_NoResults()
    {
        var resources = new List<LocalizationResource>
        {
            new("This.Is.Resource.Key", false)
            {
                Translations = new LocalizationResourceTranslationCollection(false)
                {
                    new() { Language = "no", Value = "this is norsk" }
                }
            },
            new("This.Is.AnotherResource.Key", false)
            {
                Translations = new LocalizationResourceTranslationCollection(false)
                {
                    new() { Language = "no", Value = "this is norsk" }
                }
            }
        };
        var sut = new JsonConverter(new QueryExecutor(new ConfigurationContext().TypeFactory), new ScanState());

        var resourcesAsJson = sut.Convert(resources, "en", CultureInfo.InvariantCulture, false);

        Assert.Empty(resourcesAsJson);
    }

    [Fact]
    public void VariousResources_WithSharedRootKeyName()
    {
        var sut = new JsonConverter(new QueryExecutor(new ConfigurationContext().TypeFactory), new ScanState());

        var resources = new List<LocalizationResource>
        {
            new("This.Is.Resource.Key", false)
            {
                Translations = new LocalizationResourceTranslationCollection(false)
                {
                    new() { Language = "en", Value = "this is english" },
                    new() { Language = "no", Value = "this is norsk" }
                }
            },
            new("This.Is.Resource.AnotherKey", false)
            {
                Translations = new LocalizationResourceTranslationCollection(false)
                {
                    new() { Language = "en", Value = "this is another english" },
                    new() { Language = "no", Value = "this is another norsk" }
                }
            },
            new("This.Is.YetAnotherResource.AnotherKey", false)
            {
                Translations = new LocalizationResourceTranslationCollection(false)
                {
                    new() { Language = "en", Value = "this is another english 2" },
                    new() { Language = "no", Value = "this is another norsk 2" }
                }
            }
        };

        var resourcesAsJson = sut.Convert(resources, "en", CultureInfo.InvariantCulture, false);

        Assert.Equal("this is english", resourcesAsJson["This"]["Is"]["Resource"]["Key"]);
    }

    [Fact]
    public void ResourcesWithMixedKeys_MixedOrder()
    {
        var sut = new JsonConverter(new QueryExecutor(new ConfigurationContext().TypeFactory), new ScanState());

        var resources = new List<LocalizationResource>
        {
            new("This.Is.Resource.Key", false)
            {
                Translations = new LocalizationResourceTranslationCollection(false)
                {
                    new() { Language = "en", Value = "this is english" },
                    new() { Language = "no", Value = "this is norsk" }
                }
            },
            new("This.Is.Resource.AnotherKey", false)
            {
                Translations = new LocalizationResourceTranslationCollection(false)
                {
                    new() { Language = "en", Value = "this is another english" },
                    new() { Language = "no", Value = "this is another norsk" }
                }
            },
            new("This.Is.YetAnotherResource.AnotherKey", false)
            {
                Translations = new LocalizationResourceTranslationCollection(false)
                {
                    new() { Language = "en", Value = "this is another english 2" },
                    new() { Language = "no", Value = "this is another norsk 2" }
                }
            }
        };

        var resourcesAsJson = sut.Convert(resources, "en", CultureInfo.InvariantCulture, false);

        Assert.Equal("this is english", resourcesAsJson["This"]["Is"]["Resource"]["Key"]);
    }

    [Fact]
    public void ResourceWithMultipleTranslations_ReturnRequestedTranslation()
    {
        var sut = new JsonConverter(new QueryExecutor(new ConfigurationContext().TypeFactory), new ScanState());

        var resources = new List<LocalizationResource>
        {
            new("This.Is.Resource.Key", false)
            {
                Translations = new LocalizationResourceTranslationCollection(false)
                {
                    new() { Language = "en", Value = "this is english" },
                    new() { Language = "no", Value = "this is norsk" }
                }
            }
        };

        var resourcesAsJson = sut.Convert(resources, "no", CultureInfo.InvariantCulture, false);

        Assert.Equal("this is norsk", resourcesAsJson["This"]["Is"]["Resource"]["Key"]);
    }

    [Fact]
    public void Resource_SerializeWithCamelCase()
    {
        var sut = new JsonConverter(new QueryExecutor(new ConfigurationContext().TypeFactory), new ScanState());

        var resources = new List<LocalizationResource>
        {
            new("This.Is.TheResource.Key", false)
            {
                Translations = new LocalizationResourceTranslationCollection(false)
                {
                    new() { Language = "en", Value = "this is english" },
                    new() { Language = "no", Value = "this is norsk" }
                }
            }
        };

        var resourcesAsJson = sut.Convert(resources, "en", CultureInfo.InvariantCulture, true);

        Assert.Equal("this is english", resourcesAsJson["this"]["is"]["theResource"]["key"]);
    }

    [Fact]
    public void ConvertToNonExistingLanguage_NoFallback_ShouldNotReturnNull()
    {
        var ctx = new ConfigurationContext();
        var wrapper = new OptionsWrapper<ConfigurationContext>(ctx);
        var keyBuilder = new ResourceKeyBuilder(new ScanState(), wrapper);
        ctx.TypeFactory
            .ForQuery<GetAllResources.Query>()
            .SetHandler(() => new GetAllResourcesUnitTestHandler(Enumerable.Empty<LocalizationResource>()));

        var sut = new LocalizationProvider(keyBuilder,
                                           new ExpressionHelper(keyBuilder),
                                           new OptionsWrapper<ConfigurationContext>(new ConfigurationContext()),
                                           new QueryExecutor(ctx.TypeFactory),
                                           new ScanState());

        var result = sut.Translate<SomeResourceClass>(new CultureInfo("fr"));

        Assert.NotNull(result);
    }

    [Fact]
    public void WithSpecificLanguageFallback_SomeOfTranslationsNotExist_ProperFallbackLanguageShouldBeUsed()
    {
        var resources = new List<LocalizationResource>
        {
            new("DbLocalizationProvider.Tests.JsonConverterTests.SomeResourceClass.PropertyInAllLanguages", false)
            {
                Translations = new LocalizationResourceTranslationCollection(false)
                {
                    new() { Language = "fr", Value = "FR" },
                    new() { Language = "en-GB", Value = "EN-GB" },
                    new() { Language = "en", Value = "EN" },
                    new() { Language = "", Value = "INVARIANT" }
                }
            },
            new("DbLocalizationProvider.Tests.JsonConverterTests.SomeResourceClass.PropertyOnlyInEnglish", false)
            {
                Translations = new LocalizationResourceTranslationCollection(false)
                {
                    new() { Language = "en", Value = "EN" }, new() { Language = "", Value = "INVARIANT" }
                }
            },
            new("DbLocalizationProvider.Tests.JsonConverterTests.SomeResourceClass.PropertyOnlyInInvariant", false)
            {
                Translations = new LocalizationResourceTranslationCollection(false)
                {
                    new() { Language = "", Value = "INVARIANT" }
                }
            }
        };

        var ctx = new ConfigurationContext();
        var wrapper = new OptionsWrapper<ConfigurationContext>(ctx);
        var keyBuilder = new ResourceKeyBuilder(new ScanState(), wrapper);
        ctx.TypeFactory.ForQuery<GetAllResources.Query>().SetHandler(() => new GetAllResourcesUnitTestHandler(resources));
        ctx.FallbackLanguages.Try(
            new List<CultureInfo>
            {
                new("fr"),
                new("en-GB"),
                new("en"),
                CultureInfo.InvariantCulture
            });

        var sut = new LocalizationProvider(keyBuilder,
                                           new ExpressionHelper(keyBuilder),
                                           new OptionsWrapper<ConfigurationContext>(ctx),
                                           new QueryExecutor(ctx.TypeFactory),
                                           new ScanState());

        var result = sut.Translate<SomeResourceClass>(new CultureInfo("fr-FR"));

        Assert.NotNull(result);
        Assert.Equal("FR", result.PropertyInAllLanguages);
        Assert.Equal("EN", result.PropertyOnlyInEnglish);
        Assert.Equal("INVARIANT", result.PropertyOnlyInInvariant);
    }

    [Fact]
    public void RequestTranslationForLanguageInsideFallbackList_NoTranslation_NextFallbackLanguageShouldBeUsed()
    {
        var resources = new List<LocalizationResource>
        {
            new("DbLocalizationProvider.Tests.JsonConverterTests.SomeResourceClass.PropertyInFrenchAndEnglish", false)
            {
                Translations = new LocalizationResourceTranslationCollection(false)
                {
                    new() { Language = "fr", Value = "FR" },
                    new() { Language = "en", Value = "EN" },
                    new() { Language = "", Value = "INVARIANT" }
                }
            },
            new("DbLocalizationProvider.Tests.JsonConverterTests.SomeResourceClass.PropertyOnlyInInvariant", false)
            {
                Translations = new LocalizationResourceTranslationCollection(false)
                {
                    new() { Language = "", Value = "INVARIANT" }
                }
            }
        };

        var ctx = new ConfigurationContext();
        var wrapper = new OptionsWrapper<ConfigurationContext>(ctx);
        var keyBuilder = new ResourceKeyBuilder(new ScanState(), wrapper);
        ctx.TypeFactory.ForQuery<GetAllResources.Query>().SetHandler(() => new GetAllResourcesUnitTestHandler(resources));
        ctx.FallbackLanguages.Try(
            new List<CultureInfo>
            {
                new("fr"),
                new("en-GB"),
                new("en"),
                CultureInfo.InvariantCulture
            });

        var sut = new LocalizationProvider(keyBuilder,
                                           new ExpressionHelper(keyBuilder),
                                           new OptionsWrapper<ConfigurationContext>(ctx),
                                           new QueryExecutor(ctx.TypeFactory), 
                                           new ScanState());

        var result = sut.Translate<SomeResourceClass>(new CultureInfo("en-GB"));

        Assert.NotNull(result);
        Assert.Equal("EN", result.PropertyInFrenchAndEnglish);

        // request for last language in the list - invariant should be returned
        result = sut.Translate<SomeResourceClass>(new CultureInfo("en"));
        Assert.Equal("INVARIANT", result.PropertyOnlyInInvariant);
    }
}

[LocalizedResource]
public class SomeResourceClass
{
    public string PropertyInAllLanguages { get; set; } = "In all languages";
    public string PropertyOnlyInEnglish { get; set; } = "Only in English";
    public string PropertyOnlyInInvariant { get; set; } = "Only in Invariant";
    public string PropertyInFrenchAndEnglish { get; set; } = "Only in Invariant";
}

public class GetAllResourcesUnitTestHandler : IQueryHandler<GetAllResources.Query, IEnumerable<LocalizationResource>>
{
    private readonly List<LocalizationResource> _resources;

    public GetAllResourcesUnitTestHandler(IEnumerable<LocalizationResource> resources)
    {
        _resources = resources.ToList();
    }
    
    public GetAllResourcesUnitTestHandler(IEnumerable<DiscoveredResource> discoveredResources)
    {
        _resources = new List<LocalizationResource>();
        foreach (var discoveredResource in discoveredResources)
        {
            var translations = new LocalizationResourceTranslationCollection(true);

            foreach (var translation in discoveredResource.Translations)
            {
                translations.Add(new LocalizationResourceTranslation()
                {
                    Language = translation.Culture,
                    Value = translation.Translation
                });
            }

            _resources.Add(new LocalizationResource(discoveredResource.Key, true)
            {
                ResourceKey = discoveredResource.Key,
                Translations = translations
            });
        }
    }

    public IEnumerable<LocalizationResource> Execute(GetAllResources.Query query)
    {
        return _resources;
    }
}

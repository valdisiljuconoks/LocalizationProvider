using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Internal;
using DbLocalizationProvider.Json;
using DbLocalizationProvider.Queries;
using DbLocalizationProvider.Sync;
using Xunit;

namespace DbLocalizationProvider.Tests.JsonConverterTests
{
    public class JsonConverterTests
    {
        [Fact]
        public void VariousResources_WithMixedKeyNames()
        {
            var resources = new List<LocalizationResource>
            {
                new LocalizationResource("/another/mixmatch/key", false)
                {
                    Translations = new LocalizationResourceTranslationCollection(false)
                    {
                        new LocalizationResourceTranslation
                        {
                            Language = "en",
                            Value = "this is another english"
                        }
                    }
                },
                new LocalizationResource("This.Is.Resource.Key", false)
                {
                    Translations = new LocalizationResourceTranslationCollection(false)
                    {
                        new LocalizationResourceTranslation
                        {
                            Language = "en",
                            Value = "this is english"
                        },
                        new LocalizationResourceTranslation
                        {
                            Language = "no",
                            Value = "this is norsk"
                        }
                    }
                },
                new LocalizationResource("This.Is.AnotherResource.Key", false)
                {
                    Translations = new LocalizationResourceTranslationCollection(false)
                                   {
                                       new LocalizationResourceTranslation { Language = "en", Value = "this is another english" },
                                       new LocalizationResourceTranslation { Language = "no", Value = "this is another norsk" }
                                   }
                },
                new LocalizationResource("This.Totally.Another.Resource.Key", false)
                {
                    Translations = new LocalizationResourceTranslationCollection(false)
                                   {
                                       new LocalizationResourceTranslation { Language = "en", Value = "this totally another english" },
                                       new LocalizationResourceTranslation { Language = "no", Value = "this totally another norsk" }
                                   }
                },
                new LocalizationResource("This.Is.Back.Resource.Key", false)
                {
                    Translations = new LocalizationResourceTranslationCollection(false)
                                   {
                                       new LocalizationResourceTranslation { Language = "en", Value = "this is back english" },
                                       new LocalizationResourceTranslation { Language = "no", Value = "this is back norsk" }
                                   }
                },
                new LocalizationResource("This.Resource.Is.The.Last.Resource", false)
                {
                    Translations = new LocalizationResourceTranslationCollection(false)
                                   {
                                       new LocalizationResourceTranslation { Language = "en", Value = "last" },
                                       new LocalizationResourceTranslation { Language = "no", Value = "siste" }
                                   }
                }
            };

            var sut = new JsonConverter(new QueryExecutor(new ConfigurationContext()));

            var resourcesAsJson = sut.Convert(resources, "en", CultureInfo.InvariantCulture, false);

            Assert.Equal("this is english", resourcesAsJson["This"]["Is"]["Resource"]["Key"]);

            Assert.Single(resourcesAsJson);
        }

        [Fact]
        public void VariousResourcesWithNorskTranslation_RequestedEnglishWithoutFallback_NoResults()
        {
            var resources = new List<LocalizationResource>
            {
                new LocalizationResource("This.Is.Resource.Key", false)
                {
                    Translations = new LocalizationResourceTranslationCollection(false)
                    {
                        new LocalizationResourceTranslation
                        {
                            Language = "no",
                            Value = "this is norsk"
                        }
                    }
                },
                new LocalizationResource("This.Is.AnotherResource.Key", false)
                {
                    Translations = new LocalizationResourceTranslationCollection(false)
                    {
                        new LocalizationResourceTranslation
                        {
                            Language = "no",
                            Value = "this is norsk"
                        }
                    }
                }
            };
            var sut = new Json.JsonConverter(new QueryExecutor(new ConfigurationContext()));

            var resourcesAsJson = sut.Convert(resources, "en", CultureInfo.InvariantCulture, false);

            Assert.Empty(resourcesAsJson);
        }

        [Fact]
        public void VariousResources_WithSharedRootKeyName()
        {
            var sut = new Json.JsonConverter(new QueryExecutor(new ConfigurationContext()));

            var resources = new List<LocalizationResource>
            {
                new LocalizationResource("This.Is.Resource.Key", false)
                {
                    Translations = new LocalizationResourceTranslationCollection(false)
                    {
                        new LocalizationResourceTranslation
                        {
                            Language = "en",
                            Value = "this is english"
                        },
                        new LocalizationResourceTranslation
                        {
                            Language = "no",
                            Value = "this is norsk"
                        }
                    }
                },
                new LocalizationResource("This.Is.Resource.AnotherKey", false)
                {
                    Translations = new LocalizationResourceTranslationCollection(false)
                    {
                        new LocalizationResourceTranslation
                        {
                            Language = "en",
                            Value = "this is another english"
                        },
                        new LocalizationResourceTranslation
                        {
                            Language = "no",
                            Value = "this is another norsk"
                        }
                    }
                },
                new LocalizationResource("This.Is.YetAnotherResource.AnotherKey", false)
                {
                    Translations = new LocalizationResourceTranslationCollection(false)
                    {
                        new LocalizationResourceTranslation
                        {
                            Language = "en",
                            Value = "this is another english 2"
                        },
                        new LocalizationResourceTranslation
                        {
                            Language = "no",
                            Value = "this is another norsk 2"
                        }
                    }
                }
            };

            var resourcesAsJson = sut.Convert(resources, "en", CultureInfo.InvariantCulture, false);

            Assert.Equal("this is english", resourcesAsJson["This"]["Is"]["Resource"]["Key"]);
        }

        [Fact]
        public void ResourcesWithMixedKeys_MixedOrder()
        {
            var sut = new JsonConverter(new QueryExecutor(new ConfigurationContext()));

            var resources = new List<LocalizationResource>
            {
                new LocalizationResource("This.Is.Resource.Key", false)
                {
                    Translations = new LocalizationResourceTranslationCollection(false)
                    {
                        new LocalizationResourceTranslation
                        {
                            Language = "en",
                            Value = "this is english"
                        },
                        new LocalizationResourceTranslation
                        {
                            Language = "no",
                            Value = "this is norsk"
                        }
                    }
                },
                new LocalizationResource("This.Is.Resource.AnotherKey", false)
                {
                    Translations = new LocalizationResourceTranslationCollection(false)
                    {
                        new LocalizationResourceTranslation
                        {
                            Language = "en",
                            Value = "this is another english"
                        },
                        new LocalizationResourceTranslation
                        {
                            Language = "no",
                            Value = "this is another norsk"
                        }
                    }
                },
                new LocalizationResource("This.Is.YetAnotherResource.AnotherKey", false)
                {
                    Translations = new LocalizationResourceTranslationCollection(false)
                    {
                        new LocalizationResourceTranslation
                        {
                            Language = "en",
                            Value = "this is another english 2"
                        },
                        new LocalizationResourceTranslation
                        {
                            Language = "no",
                            Value = "this is another norsk 2"
                        }
                    }
                }
            };

            var resourcesAsJson = sut.Convert(resources, "en", CultureInfo.InvariantCulture, false);

            Assert.Equal("this is english", resourcesAsJson["This"]["Is"]["Resource"]["Key"]);
        }

        [Fact]
        public void ResourceWithMultipleTranslations_ReturnRequestedTranslation()
        {
            var sut = new Json.JsonConverter(new QueryExecutor(new ConfigurationContext()));

            var resources = new List<LocalizationResource>
            {
                new LocalizationResource("This.Is.Resource.Key", false)
                {
                    Translations = new LocalizationResourceTranslationCollection(false)
                    {
                        new LocalizationResourceTranslation
                        {
                            Language = "en",
                            Value = "this is english"
                        },
                        new LocalizationResourceTranslation
                        {
                            Language = "no",
                            Value = "this is norsk"
                        }
                    }
                }
            };

            var resourcesAsJson = sut.Convert(resources, "no", CultureInfo.InvariantCulture, false);

            Assert.Equal("this is norsk", resourcesAsJson["This"]["Is"]["Resource"]["Key"]);
        }

        [Fact]
        public void Resource_SerializeWithCamelCase()
        {
            var sut = new Json.JsonConverter(new QueryExecutor(new ConfigurationContext()));

            var resources = new List<LocalizationResource>
            {
                new LocalizationResource("This.Is.TheResource.Key", false)
                {
                    Translations = new LocalizationResourceTranslationCollection(false)
                    {
                        new LocalizationResourceTranslation
                        {
                            Language = "en",
                            Value = "this is english"
                        },
                        new LocalizationResourceTranslation
                        {
                            Language = "no",
                            Value = "this is norsk"
                        }
                    }
                }
            };

            var resourcesAsJson = sut.Convert(resources, "en", CultureInfo.InvariantCulture, true);

            Assert.Equal("this is english", resourcesAsJson["this"]["is"]["theResource"]["key"]);
        }

        [Fact]
        public void ConvertToNonExistingLanguage_NoFallback_ShouldNotReturnNull()
        {
            var keyBuilder = new ResourceKeyBuilder(new ScanState());
            var ctx = new ConfigurationContext();
            ctx.TypeFactory
                .ForQuery<GetAllResources.Query>()
                .SetHandler(() => new GetAllResourcesUnitTestHandler(Enumerable.Empty<LocalizationResource>()));

            var sut = new LocalizationProvider(keyBuilder,
                                               new ExpressionHelper(keyBuilder),
                                               new FallbackLanguagesCollection(),
                                               new QueryExecutor(ctx));

            var result = sut.Translate<SomeResourceClass>(new CultureInfo("fr"));

            Assert.NotNull(result);
        }

        [Fact]
        public void WithSpecificLanguageFallback_SomeOfTranslationsNotExist_ProperFallbackLanguageShouldBeUsed()
        {
            var resources = new List<LocalizationResource>
            {
                new LocalizationResource("DbLocalizationProvider.Tests.JsonConverterTests.SomeResourceClass.PropertyInAllLanguages", false)
                {
                    Translations = new LocalizationResourceTranslationCollection(false)
                    {
                        new LocalizationResourceTranslation
                        {
                            Language = "fr", Value = "FR"
                        },
                        new LocalizationResourceTranslation
                        {
                            Language = "en-GB", Value = "EN-GB"
                        },
                        new LocalizationResourceTranslation
                        {
                            Language = "en", Value = "EN"
                        },
                        new LocalizationResourceTranslation
                        {
                            Language = "", Value = "INVARIANT"
                        }
                    }
                },
                new LocalizationResource("DbLocalizationProvider.Tests.JsonConverterTests.SomeResourceClass.PropertyOnlyInEnglish", false)
                {
                    Translations = new LocalizationResourceTranslationCollection(false)
                    {
                        new LocalizationResourceTranslation
                        {
                            Language = "en", Value = "EN"
                        },
                        new LocalizationResourceTranslation
                        {
                            Language = "", Value = "INVARIANT"
                        }
                    }
                },
                new LocalizationResource("DbLocalizationProvider.Tests.JsonConverterTests.SomeResourceClass.PropertyOnlyInInvariant", false)
                {
                    Translations = new LocalizationResourceTranslationCollection(false)
                    {
                        new LocalizationResourceTranslation
                        {
                            Language = "", Value = "INVARIANT"
                        }
                    }
                }
            };

            var keyBuilder = new ResourceKeyBuilder(new ScanState());
            var ctx = new ConfigurationContext();
            ctx.TypeFactory.ForQuery<GetAllResources.Query>().SetHandler(() => new GetAllResourcesUnitTestHandler(resources));
            ctx.FallbackLanguages.Try(
                new List<CultureInfo>
                {
                    new CultureInfo("fr"),
                    new CultureInfo("en-GB"),
                    new CultureInfo("en"),
                    CultureInfo.InvariantCulture
                });

            var sut = new LocalizationProvider(keyBuilder,
                                               new ExpressionHelper(keyBuilder),
                                               ctx.FallbackList,
                                               new QueryExecutor(ctx));

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
                new LocalizationResource("DbLocalizationProvider.Tests.JsonConverterTests.SomeResourceClass.PropertyInFrenchAndEnglish", false)
                {
                    Translations = new LocalizationResourceTranslationCollection(false)
                    {
                        new LocalizationResourceTranslation
                        {
                            Language = "fr", Value = "FR"
                        },
                        new LocalizationResourceTranslation
                        {
                            Language = "en", Value = "EN"
                        },
                        new LocalizationResourceTranslation
                        {
                            Language = "", Value = "INVARIANT"
                        }
                    }
                },
                new LocalizationResource("DbLocalizationProvider.Tests.JsonConverterTests.SomeResourceClass.PropertyOnlyInInvariant", false)
                {
                    Translations = new LocalizationResourceTranslationCollection(false)
                    {
                        new LocalizationResourceTranslation { Language = "", Value = "INVARIANT" }
                    }
                }
            };

            var keyBuilder = new ResourceKeyBuilder(new ScanState());
            var ctx = new ConfigurationContext();
            ctx.TypeFactory.ForQuery<GetAllResources.Query>().SetHandler(() => new GetAllResourcesUnitTestHandler(resources));
            ctx.FallbackLanguages.Try(
                new List<CultureInfo>
                {
                    new CultureInfo("fr"),
                    new CultureInfo("en-GB"),
                    new CultureInfo("en"),
                    CultureInfo.InvariantCulture
                });

            var sut = new LocalizationProvider(keyBuilder,
                                               new ExpressionHelper(keyBuilder),
                                               ctx.FallbackList,
                                               new QueryExecutor(ctx));

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
        private readonly IEnumerable<LocalizationResource> _resources;

        public GetAllResourcesUnitTestHandler(IEnumerable<LocalizationResource> resources)
        {
            _resources = resources;
        }

        public IEnumerable<LocalizationResource> Execute(GetAllResources.Query query)
        {
            return _resources;
        }
    }
}

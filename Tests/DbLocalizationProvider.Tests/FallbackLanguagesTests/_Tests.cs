using System.Collections.Generic;
using System.Globalization;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Internal;
using DbLocalizationProvider.Queries;
using DbLocalizationProvider.Sync;
using Xunit;

namespace DbLocalizationProvider.Tests.FallbackLanguagesTests
{
    public class FallbackLanguagesTests
    {
        private readonly LocalizationProvider _sut;

        public FallbackLanguagesTests()
        {
            var keyBuilder = new ResourceKeyBuilder(new ScanState());
            var ctx = new ConfigurationContext();

            // try "sv" -> "no" -> "en"
            ctx.FallbackLanguages
                .Try(new CultureInfo("sv"))
                .Then(new CultureInfo("no"))
                .Then(new CultureInfo("en"));

            // for rare cases - configure language specific fallback
            ctx.FallbackLanguages
                .When(new CultureInfo("fr-BE"))
                .Try(new CultureInfo("fr"))
                .Then(new CultureInfo("en"));

            ctx.TypeFactory.ForQuery<GetTranslation.Query>().SetHandler(() => new FallbackLanguagesTestTranslationHandler(ctx.FallbackList));

            IQueryExecutor queryExecutor = new QueryExecutor(ctx);

            _sut = new LocalizationProvider(keyBuilder, new ExpressionHelper(keyBuilder), ctx.FallbackList, queryExecutor);
        }

        [Fact]
        public void FallbackTranslationTests()
        {
            Assert.Equal("Some Swedish translation", _sut.GetString("Resource.With.Swedish.Translation", new CultureInfo("sv")));
            Assert.Equal("Some English translation", _sut.GetString("Resource.With.English.Translation", new CultureInfo("sv")));
            Assert.Equal("Some Norwegian translation", _sut.GetString("Resource.With.Norwegian.Translation", new CultureInfo("sv")));
            Assert.Equal("Some Norwegian translation", _sut.GetString("Resource.With.Norwegian.And.English.Translation", new CultureInfo("sv")));
        }

        [Fact]
        public void Language_ShouldFollowLanguageBranchSpecs()
        {
            Assert.Equal("Some French translation", _sut.GetString("Resource.With.FrenchFallback.Translation", new CultureInfo("fr-BE")));
            Assert.Equal("Some English translation", _sut.GetString("Resource.InFrench.With.EnglishFallback.Translation", new CultureInfo("fr-BE")));

        }
    }

    public class FallbackLanguagesTestTranslationHandler : IQueryHandler<GetTranslation.Query, string>
    {
        private readonly FallbackLanguagesCollection _fallbackCollection;
        private readonly Dictionary<string, LocalizationResource> _resources = new Dictionary<string, LocalizationResource>
        {
            {
                "Resource.With.Swedish.Translation",
                new LocalizationResource("Resource.With.Swedish.Translation", false)
                {
                    Translations = new LocalizationResourceTranslationCollection(false)
                    {
                        new LocalizationResourceTranslation
                        {
                            Language = "sv", Value = "Some Swedish translation"
                        }
                    }
                }
            },
            {
                "Resource.With.English.Translation",
                new LocalizationResource("Resource.With.English.Translation", false)
                {
                    Translations = new LocalizationResourceTranslationCollection(false)
                    {
                        new LocalizationResourceTranslation
                        {
                            Language = "en", Value = "Some English translation"
                        }
                    }
                }
            },
            {
                "Resource.With.Norwegian.Translation",
                new LocalizationResource("Resource.With.Norwegian.Translation", false)
                {
                    Translations = new LocalizationResourceTranslationCollection(false)
                    {
                        new LocalizationResourceTranslation
                        {
                            Language = "no", Value = "Some Norwegian translation"
                        }
                    }
                }
            },
            {
                "Resource.With.Norwegian.And.English.Translation",
                new LocalizationResource("Resource.With.Norwegian.And.English.Translation", false)
                {
                    Translations = new LocalizationResourceTranslationCollection(false)
                    {
                        new LocalizationResourceTranslation
                        {
                            Language = "no", Value = "Some Norwegian translation"
                        },
                        new LocalizationResourceTranslation
                        {
                            Language = "en", Value = "Some English translation"
                        }
                    }
                }
            },
            {
                "Resource.With.FrenchFallback.Translation",
                new LocalizationResource("Resource.With.FrenchFallback.Translation", false)
                {
                    Translations = new LocalizationResourceTranslationCollection(false)
                    {
                        new LocalizationResourceTranslation
                        {
                            Language = "fr", Value = "Some French translation"
                        }
                    }
                }
            },
            {
                "Resource.InFrench.With.EnglishFallback.Translation",
                new LocalizationResource("Resource.InFrench.With.EnglishFallback.Translation", false)
                {
                    Translations = new LocalizationResourceTranslationCollection(false)
                    {
                        new LocalizationResourceTranslation
                        {
                            Language = "en", Value = "Some English translation"
                        }
                    }
                }
            }
        };

        public FallbackLanguagesTestTranslationHandler(FallbackLanguagesCollection fallbackCollection)
        {
            _fallbackCollection = fallbackCollection;
        }

        public string Execute(GetTranslation.Query query)
        {
            return _resources[query.Key].Translations.GetValueWithFallback(
                query.Language,
                _fallbackCollection.GetFallbackLanguages(query.Language));
        }
    }
}

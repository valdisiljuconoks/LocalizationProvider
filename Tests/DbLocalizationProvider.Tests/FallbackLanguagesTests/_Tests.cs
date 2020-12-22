using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Internal;
using DbLocalizationProvider.Queries;
using DbLocalizationProvider.Sync;
using Xunit;

namespace DbLocalizationProvider.Tests.FallbackLanguagesTests
{
    public class _Tests
    {
        [Fact]
        public void FallbackTranslationTests()
        {
            ConfigurationContext.Setup(_ =>
            {
                // try "sv" -> "no" -> "en"
                _.FallbackCultures
                 .Try(new CultureInfo("sv"))
                 .Then(new CultureInfo("no"))
                 .Then(new CultureInfo("en"));

                // for rare cases - configure language specific fallback
                _.FallbackCultures
                 .When(new CultureInfo("fr-BE"))
                 .Try(new CultureInfo("fr"))
                 .Then(new CultureInfo("en"));

                _.TypeFactory.ForQuery<GetTranslation.Query>().SetHandler(() => new FallbacksTestsTranslationHandler(_.FallbackList));
            });

            var keyBuilder = new ResourceKeyBuilder(new ScanState());
            var sut = new LocalizationProvider(keyBuilder, new ExpressionHelper(keyBuilder));

            Assert.Equal("Some Swedish translation", sut.GetString("Resource.With.Swedish.Translation", new CultureInfo("sv")));

            Assert.Equal("Some English translation", sut.GetString("Resource.With.English.Translation", new CultureInfo("sv")));

            Assert.Equal("Some Norwegian translation", sut.GetString("Resource.With.Norwegian.Translation", new CultureInfo("sv")));

            Assert.Equal("Some Norwegian translation", sut.GetString("Resource.With.Norwegian.And.English.Translation", new CultureInfo("sv")));
        }

        [Fact]
        public void Language_ShouldFollowLanguageBranchSpecs()
        {
            ConfigurationContext.Setup(_ =>
            {
                // try "sv" -> "no" -> "en"
                _.FallbackCultures
                 .Try(new CultureInfo("sv"))
                 .Then(new CultureInfo("no"))
                 .Then(new CultureInfo("en"));

                // for rare cases - configure language specific fallback
                _.FallbackCultures
                 .When(new CultureInfo("fr-BE"))
                 .Try(new CultureInfo("fr"))
                 .Then(new CultureInfo("en"));

                _.TypeFactory.ForQuery<GetTranslation.Query>().SetHandler(() => new FallbacksTestsTranslationHandler(_.FallbackList));
            });

            var keyBuilder = new ResourceKeyBuilder(new ScanState());
            var sut = new LocalizationProvider(keyBuilder, new ExpressionHelper(keyBuilder));

            Assert.Equal("Some French translation", sut.GetString("Resource.With.FrenchFallback.Translation", new CultureInfo("fr-BE")));
            Assert.Equal("Some English translation", sut.GetString("Resource.InFrench.With.EnglishFallback.Translation", new CultureInfo("fr-BE")));

        }
    }

    public class FallbacksTestsTranslationHandler : IQueryHandler<GetTranslation.Query, string>
    {
        private readonly Dictionary<string, FallbackLanguagesList> _objFallbackCultures;
        private readonly Dictionary<string, LocalizationResource> _resources = new Dictionary<string, LocalizationResource>
        {
            {
                "Resource.With.Swedish.Translation",
                new LocalizationResource("Resource.With.Swedish.Translation")
                {
                    Translations = new List<LocalizationResourceTranslation>
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
                new LocalizationResource("Resource.With.English.Translation")
                {
                    Translations = new List<LocalizationResourceTranslation>
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
                new LocalizationResource("Resource.With.Norwegian.Translation")
                {
                    Translations = new List<LocalizationResourceTranslation>
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
                new LocalizationResource("Resource.With.Norwegian.And.English.Translation")
                {
                    Translations = new List<LocalizationResourceTranslation>
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
                new LocalizationResource("Resource.With.FrenchFallback.Translation")
                {
                    Translations = new List<LocalizationResourceTranslation>
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
                new LocalizationResource("Resource.InFrench.With.EnglishFallback.Translation")
                {
                    Translations = new List<LocalizationResourceTranslation>
                    {
                        new LocalizationResourceTranslation
                        {
                            Language = "en", Value = "Some English translation"
                        }
                    }
                }
            }
        };

        public FallbacksTestsTranslationHandler(Dictionary<string, FallbackLanguagesList> objFallbackCultures)
        {
            _objFallbackCultures = objFallbackCultures;
        }

        public string Execute(GetTranslation.Query query)
        {
            return _resources[query.Key].Translations.GetValueWithFallback(
                query.Language,
                query.Language.GetFallbackLanguageList());

            //return _objFallbackCultures != null && _objFallbackCultures.Any()
            //           ?
            //           : base.GetTranslationFromAvailableList(_resources[query.Key].Translations, query.Language, query.UseFallback)?.Value;
        }
    }
}

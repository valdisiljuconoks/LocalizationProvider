using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Queries;
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
                                           _.FallbackCultures.Try(new CultureInfo("sv"))
                                            .Then(new CultureInfo("no"))
                                            .Then(new CultureInfo("en"));

                                           _.TypeFactory.ForQuery<GetTranslation.Query>().SetHandler(() => new FallbacksTestsTranslationHandler(_.FallbackCultures));
                                       });

            var sut = new LocalizationProvider();

            Assert.Equal("Some Swedish translation", sut.GetString("Resource.With.Swedish.Translation", new CultureInfo("sv")));

            Assert.Equal("Some English translation", sut.GetString("Resource.With.English.Translation", new CultureInfo("sv")));

            Assert.Equal("Some Norwegian translation", sut.GetString("Resource.With.Norwegian.Translation", new CultureInfo("sv")));

            Assert.Equal("Some Norwegian translation", sut.GetString("Resource.With.Norwegian.And.English.Translation", new CultureInfo("sv")));
        }
    }

    public class FallbacksTestsTranslationHandler : IQueryHandler<GetTranslation.Query, string>
    {
        private readonly IReadOnlyCollection<CultureInfo> _objFallbackCultures;
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
            }
        };

        public FallbacksTestsTranslationHandler(IReadOnlyCollection<CultureInfo> objFallbackCultures)
        {
            _objFallbackCultures = objFallbackCultures;
        }

        public string Execute(GetTranslation.Query query)
        {
            return _resources[query.Key].Translations.GetValueWithFallback(query.Language, _objFallbackCultures);
            //return _objFallbackCultures != null && _objFallbackCultures.Any()
            //           ? 
            //           : base.GetTranslationFromAvailableList(_resources[query.Key].Translations, query.Language, query.UseFallback)?.Value;
        }
    }
}

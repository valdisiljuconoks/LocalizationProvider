using System.Collections.Generic;
using System.Globalization;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Internal;
using DbLocalizationProvider.Queries;
using DbLocalizationProvider.Sync;
using Xunit;

namespace DbLocalizationProvider.Tests.FallbackLanguagesTests;

public class FallbackLanguagesTests
{
    private readonly LocalizationProvider _sut;

    public FallbackLanguagesTests()
    {
        var ctx = new ConfigurationContext();
        var keyBuilder = new ResourceKeyBuilder(new ScanState(), ctx);

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

        ctx.TypeFactory.ForQuery<GetTranslation.Query>()
            .SetHandler(() => new FallbackLanguagesTestTranslationHandler(ctx.FallbackList));

        IQueryExecutor queryExecutor = new QueryExecutor(ctx.TypeFactory);

        _sut = new LocalizationProvider(keyBuilder, new ExpressionHelper(keyBuilder), ctx.FallbackList, queryExecutor);
    }

    [Fact]
    public void FallbackTranslationTests()
    {
        Assert.Equal("Some Swedish translation", _sut.GetString("Resource.With.Swedish.Translation", new CultureInfo("sv")));
        Assert.Equal("Some English translation", _sut.GetString("Resource.With.English.Translation", new CultureInfo("sv")));
        Assert.Equal("Some Norwegian translation", _sut.GetString("Resource.With.Norwegian.Translation", new CultureInfo("sv")));
        Assert.Equal("Some Norwegian translation",
                     _sut.GetString("Resource.With.Norwegian.And.English.Translation", new CultureInfo("sv")));
    }

    [Fact]
    public void Language_ShouldFollowLanguageBranchSpecs()
    {
        Assert.Equal("Some French translation",
                     _sut.GetString("Resource.With.FrenchFallback.Translation", new CultureInfo("fr-BE")));
        Assert.Equal("Some English translation",
                     _sut.GetString("Resource.InFrench.With.EnglishFallback.Translation", new CultureInfo("fr-BE")));
    }

    [Fact]
    public void GetStringByNorwegianRegion_ShouldReturnInNorwegian()
    {
        Assert.Equal("Some Latvian translation", _sut.GetString("Resource.With.Latvian.Translation", new CultureInfo("lv")));
        Assert.Equal("Some Latvian translation", _sut.GetString("Resource.With.Latvian.Translation", new CultureInfo("lv-LV")));
    }
}

public class FallbackLanguagesTestTranslationHandler : IQueryHandler<GetTranslation.Query, string>
{
    private readonly FallbackLanguagesCollection _fallbackCollection;

    private readonly Dictionary<string, LocalizationResource> _resources = new()
    {
        {
            "Resource.With.Swedish.Translation",
            new LocalizationResource("Resource.With.Swedish.Translation", false)
            {
                Translations = new LocalizationResourceTranslationCollection(false)
                {
                    new() { Language = "sv", Value = "Some Swedish translation" }
                }
            }
        },
        {
            "Resource.With.English.Translation",
            new LocalizationResource("Resource.With.English.Translation", false)
            {
                Translations = new LocalizationResourceTranslationCollection(false)
                {
                    new() { Language = "en", Value = "Some English translation" }
                }
            }
        },
        {
            "Resource.With.Norwegian.Translation",
            new LocalizationResource("Resource.With.Norwegian.Translation", false)
            {
                Translations = new LocalizationResourceTranslationCollection(false)
                {
                    new() { Language = "no", Value = "Some Norwegian translation" }
                }
            }
        },
        {
            "Resource.With.Latvian.Translation",
            new LocalizationResource("Resource.With.Latvian.Translation", false)
            {
                Translations = new LocalizationResourceTranslationCollection(false)
                {
                    new() { Language = "lv", Value = "Some Latvian translation" }
                }
            }
        },
        {
            "Resource.With.Norwegian.And.English.Translation",
            new LocalizationResource("Resource.With.Norwegian.And.English.Translation", false)
            {
                Translations = new LocalizationResourceTranslationCollection(false)
                {
                    new() { Language = "no", Value = "Some Norwegian translation" },
                    new() { Language = "en", Value = "Some English translation" }
                }
            }
        },
        {
            "Resource.With.FrenchFallback.Translation",
            new LocalizationResource("Resource.With.FrenchFallback.Translation", false)
            {
                Translations = new LocalizationResourceTranslationCollection(false)
                {
                    new() { Language = "fr", Value = "Some French translation" }
                }
            }
        },
        {
            "Resource.InFrench.With.EnglishFallback.Translation",
            new LocalizationResource("Resource.InFrench.With.EnglishFallback.Translation", false)
            {
                Translations = new LocalizationResourceTranslationCollection(false)
                {
                    new() { Language = "en", Value = "Some English translation" }
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
        return _resources[query.Key]
            .Translations.GetValueWithFallback(
                query.Language,
                _fallbackCollection.GetFallbackLanguages(query.Language));
    }
}

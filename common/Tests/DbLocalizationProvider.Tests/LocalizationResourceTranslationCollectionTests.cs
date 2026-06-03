using System.Globalization;
using DbLocalizationProvider.Abstractions;
using Xunit;

namespace DbLocalizationProvider.Tests;

public class LocalizationResourceTranslationCollectionTests
{
    private static LocalizationResourceTranslationCollection Build(bool invariantFallback = false)
    {
        return new LocalizationResourceTranslationCollection(invariantFallback)
        {
            new() { Language = "it-IT", Value = "Ciao" },
            new() { Language = "de-AT", Value = "Servus" },
            new() { Language = "de", Value = "Hallo" },
            new() { Language = string.Empty, Value = "Hello" }
        };
    }

    [Theory]
    [InlineData("it-IT")]
    [InlineData("it-it")]
    [InlineData("IT-IT")]
    [InlineData("It-It")]
    public void FindByLanguage_MixedCase_FindsStoredTranslation(string requested)
    {
        var translations = Build();

        var found = translations.FindByLanguage(requested);

        Assert.NotNull(found);
        Assert.Equal("Ciao", found!.Value);
    }

    [Fact]
    public void FindByLanguage_CultureInfo_NormalizesToName()
    {
        var translations = Build();

        var found = translations.FindByLanguage(CultureInfo.GetCultureInfo("it-IT"));

        Assert.NotNull(found);
        Assert.Equal("Ciao", found!.Value);
    }

    [Theory]
    [InlineData("it-IT", true)]
    [InlineData("it-it", true)]
    [InlineData("fr-FR", false)]
    public void ExistsLanguage_IsCaseInsensitive(string requested, bool expected)
    {
        var translations = Build();

        Assert.Equal(expected, translations.ExistsLanguage(requested));
    }

    [Fact]
    public void ByLanguage_InvariantFallback_StillReturnsInvariantForUnknownLanguage()
    {
        var translations = Build(invariantFallback: true);

        var value = translations.ByLanguage("fr-FR", invariantCultureFallback: true);

        Assert.Equal("Hello", value);
    }

    [Fact]
    public void ByLanguage_MixedCase_ReturnsRequestedLanguageNotInvariant()
    {
        var translations = Build(invariantFallback: true);

        var value = translations.ByLanguage("it-it", invariantCultureFallback: true);

        Assert.Equal("Ciao", value);
    }

    [Fact]
    public void GetValueWithFallback_FindsParentCulture_WhenChildMissingAndCaseDiffers()
    {
        var translations = new LocalizationResourceTranslationCollection(false)
        {
            new() { Language = "de", Value = "Hallo" },
            new() { Language = string.Empty, Value = "Hello" }
        };

        var value = translations.GetValueWithFallback("DE-AT", []);

        Assert.Equal("Hallo", value);
    }
}

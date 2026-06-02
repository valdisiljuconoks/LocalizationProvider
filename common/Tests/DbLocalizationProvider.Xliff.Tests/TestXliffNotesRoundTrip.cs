using System.Collections.Generic;
using System.Linq;
using DbLocalizationProvider.Abstractions;
using Xunit;

namespace DbLocalizationProvider.Xliff.Tests;

public class TestXliffNotesRoundTrip
{
    [Fact]
    public void Export_Then_Import_PreservesNotes()
    {
        var first = new LocalizationResource("My.Resource.Key", false) { Notes = "translator note for the key" };
        first.Translations.Add(new LocalizationResourceTranslation { Language = "en", Value = "this is english text" });
        first.Translations.Add(new LocalizationResourceTranslation { Language = "no", Value = "det er tekst i norsk" });

        var second = new LocalizationResource("My.Resource.AnotherKey", false);
        second.Translations.Add(new LocalizationResourceTranslation { Language = "en", Value = "another english text" });
        second.Translations.Add(new LocalizationResourceTranslation { Language = "no", Value = "annen norsk tekst" });

        var resources = new Dictionary<string, LocalizationResource>
        {
            { first.ResourceKey, first }, { second.ResourceKey, second }
        };

        var exported = new XliffResourceExporter().Export(
            resources,
            new Dictionary<string, string[]> { { "sourceLang", ["en"] }, { "targetLang", ["no"] } });

        var parsed = new FormatParser().Parse(exported.SerializedData);

        var roundTripped = parsed.Resources.First(r => r.ResourceKey == "My.Resource.Key");
        Assert.Equal("translator note for the key", roundTripped.Notes);
        Assert.Equal("det er tekst i norsk", roundTripped.Translations.First().Value);

        var withoutNotes = parsed.Resources.First(r => r.ResourceKey == "My.Resource.AnotherKey");
        Assert.True(string.IsNullOrEmpty(withoutNotes.Notes));
    }
}

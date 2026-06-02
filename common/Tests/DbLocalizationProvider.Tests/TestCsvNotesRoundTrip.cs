// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Collections.Generic;
using System.Linq;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Csv;
using Xunit;

namespace DbLocalizationProvider.Tests;

public class TestCsvNotesRoundTrip
{
    [Fact]
    public void Export_Then_Import_PreservesNotes()
    {
        var withNotes = new LocalizationResource("My.Resource.Key", false) { Notes = "documents {0} placeholder" };
        withNotes.Translations.Add(new LocalizationResourceTranslation { Language = "en", Value = "english" });
        withNotes.Translations.Add(new LocalizationResourceTranslation { Language = "no", Value = "norsk" });

        var withoutNotes = new LocalizationResource("My.Resource.Other", false);
        withoutNotes.Translations.Add(new LocalizationResourceTranslation { Language = "en", Value = "other" });

        var resources = new Dictionary<string, LocalizationResource>
        {
            { withNotes.ResourceKey, withNotes }, { withoutNotes.ResourceKey, withoutNotes }
        };

        var csv = new CsvResourceExporter().Export(resources, null);
        var parsed = new CsvResourceFormatParser().Parse(csv.SerializedData);

        // "Notes" must not be detected as a language column
        Assert.DoesNotContain(parsed.DetectedLanguages, l => l.Name == "Notes");

        var roundTripped = parsed.Resources.Single(r => r.ResourceKey == "My.Resource.Key");
        Assert.Equal("documents {0} placeholder", roundTripped.Notes);
        Assert.Equal("english", roundTripped.Translations.ByLanguage("en", false));

        var other = parsed.Resources.Single(r => r.ResourceKey == "My.Resource.Other");
        Assert.True(string.IsNullOrEmpty(other.Notes));
    }
}

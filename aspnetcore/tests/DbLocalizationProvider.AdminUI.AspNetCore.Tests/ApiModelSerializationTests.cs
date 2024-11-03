using System.Collections.Generic;
using System.Globalization;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.AdminUI.Models;
using Newtonsoft.Json;
using Xunit;

namespace DbLocalizationProvider.AdminUI.AspNetCore.Tests;

public class ApiModelSerializationTests
{
    [Fact]
    public void SerializeSimpleResourceWith2Languages()
    {
        var model = new LocalizationResourceApiModel(
            new List<LocalizationResource>
            {
                new("the-key1", false)
                {
                    Translations = new LocalizationResourceTranslationCollection(false)
                    {
                        new() { Language = "", Value = "Invariant" },
                        new() { Language = "en", Value = "English" },
                        new() { Language = "no", Value = "Norsk" }
                    }
                }
            },
            new List<AvailableLanguage> { new("English", 1, new CultureInfo("en")), new("Norsk", 2, new CultureInfo("no")) },
            new List<AvailableLanguage> { new("English", 1, new CultureInfo("en")), new("Norsk", 2, new CultureInfo("no")) },
            120,
            80,
            new UiOptions());

        var result = JsonConvert.SerializeObject(model);
    }

    [Fact]
    public void SerializeSimpleResourceWith2Languages_MissingTranslationFor1OfThem()
    {
        var model = new LocalizationResourceApiModel(
            new List<LocalizationResource>
            {
                new("the-key1", false)
                {
                    Translations = new LocalizationResourceTranslationCollection(false)
                    {
                        new() { Language = "", Value = "Invariant" }, new() { Language = "en", Value = "English" }
                    }
                }
            },
            new List<AvailableLanguage> { new("English", 1, new CultureInfo("en")), new("Norsk", 2, new CultureInfo("no")) },
            new List<AvailableLanguage> { new("English", 1, new CultureInfo("en")), new("Norsk", 2, new CultureInfo("no")) },
            120,
            80,
            new UiOptions());

        var result = JsonConvert.SerializeObject(model);
    }
}

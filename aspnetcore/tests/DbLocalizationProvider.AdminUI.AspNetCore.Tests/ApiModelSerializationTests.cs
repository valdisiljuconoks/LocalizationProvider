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

    [Fact]
    public void BatchTranslatePreviewRequestModel_RoundTripsWithCamelCaseKeys()
    {
        var model = new BatchTranslatePreviewRequestModel
        {
            Keys = ["a", "b"],
            SourceLanguage = "",
            TargetLanguage = "de",
            OnlyEmpty = true
        };

        var json = JsonConvert.SerializeObject(model);
        Assert.Contains("\"keys\"", json);
        Assert.Contains("\"targetLanguage\":\"de\"", json);
        Assert.Contains("\"onlyEmpty\":true", json);

        var back = JsonConvert.DeserializeObject<BatchTranslatePreviewRequestModel>(json);
        Assert.Equal(2, back.Keys.Length);
        Assert.Equal("de", back.TargetLanguage);
        Assert.True(back.OnlyEmpty);
    }

    [Fact]
    public void BatchTranslatePreviewModel_SerializesItemsWithCamelCaseKeys()
    {
        var model = new BatchTranslatePreviewModel
        {
            Language = "de",
            Results =
            [
                new BatchTranslateItem { Key = "a", SourceText = "Hello", Translation = "Hallo", Success = true },
                new BatchTranslateItem { Key = "b", SourceText = "Bye", Success = false, Error = "boom" }
            ]
        };

        var json = JsonConvert.SerializeObject(model);
        Assert.Contains("\"language\":\"de\"", json);
        Assert.Contains("\"sourceText\":\"Hello\"", json);
        Assert.Contains("\"translation\":\"Hallo\"", json);
        Assert.Contains("\"success\":false", json);
        Assert.Contains("\"error\":\"boom\"", json);
    }

    [Fact]
    public void BatchTranslateApplyRequestModel_RoundTripsItems()
    {
        var model = new BatchTranslateApplyRequestModel
        {
            TargetLanguage = "de",
            Items = [new BatchTranslateApplyItem { Key = "a", Translation = "Hallo" }]
        };

        var json = JsonConvert.SerializeObject(model);
        Assert.Contains("\"targetLanguage\":\"de\"", json);
        Assert.Contains("\"items\"", json);

        var back = JsonConvert.DeserializeObject<BatchTranslateApplyRequestModel>(json);
        Assert.Single(back.Items);
        Assert.Equal("a", back.Items[0].Key);
        Assert.Equal("Hallo", back.Items[0].Translation);
    }
}

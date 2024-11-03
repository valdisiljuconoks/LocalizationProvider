using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.AdminUI.Models;
using Newtonsoft.Json.Linq;
using Xunit;

namespace DbLocalizationProvider.AdminUI.AspNetCore.Tests.TreeModelGeneration;

public class ModelTreeViewModelTests
{
    private const string _segmentPropertyName = "segmentKey";

    [Fact]
    public void GenerateSampleTreeModel_MultipleResources_NoSharedKeyRoots()
    {
        var resources = new List<LocalizationResource>
        {
            new("This.Is.Resource.Key", false)
            {
                Translations = new LocalizationResourceTranslationCollection(false)
                {
                    new() // invariant
                    {
                        Language = "", Value = "Invariant"
                    },
                    new() { Language = "en", Value = "English" },
                    new() { Language = "lv", Value = "Latvian" }
                }
            },
            new("Another.Resource.Key", false)
            {
                Translations = new LocalizationResourceTranslationCollection(false)
                {
                    new() // invariant
                    {
                        Language = "", Value = "Invariant 2"
                    },
                    new() { Language = "en", Value = "English 2" },
                    new() { Language = "lv", Value = "Latvian 2" }
                }
            }
        };

        var languages =
            new AvailableLanguage[] { new("English", 1, new CultureInfo("en")), new("Latvian", 2, new CultureInfo("lv")) };

        var sut = new LocalizationResourceApiTreeModel(resources, languages, languages, 100, 100, new UiOptions());

        var result = sut.ConvertToApiModel(resources);
        var first = result.First();
        var second = result[1];

        Assert.NotNull(first);
        Assert.Equal("Another", second[_segmentPropertyName]);
        Assert.Equal("Invariant", result[0]["_children"][0]["_children"][0]["_children"][0]["translation"]);
        Assert.Equal("Invariant 2", result[1]["_children"][0]["_children"][0]["translation"]);
    }

    [Fact]
    public void GenerateSampleTreeModel_MultipleResources_SharedKeyRoots()
    {
        var resources = new List<LocalizationResource>
        {
            new("This.Is.Resource.Key", false)
            {
                Translations = new LocalizationResourceTranslationCollection(false)
                {
                    new() // invariant
                    {
                        Language = "", Value = "Invariant"
                    },
                    new() { Language = "en", Value = "English" },
                    new() { Language = "lv", Value = "Latvian" }
                }
            },
            new("This.Is.Resource.AnotherKey", false)
            {
                Translations = new LocalizationResourceTranslationCollection(false)
                {
                    new() // invariant
                    {
                        Language = "", Value = "Invariant 2"
                    },
                    new() { Language = "en", Value = "English 2" },
                    new() { Language = "lv", Value = "Latvian 2" }
                }
            }
        };

        var languages =
            new AvailableLanguage[] { new("English", 1, new CultureInfo("en")), new("Latvian", 2, new CultureInfo("lv")) };

        var sut = new LocalizationResourceApiTreeModel(resources, languages, languages, 100, 100, new UiOptions());

        var result = sut.ConvertToApiModel(resources);
        var first = result.First();

        Assert.NotNull(first);
        Assert.Equal("Invariant", result[0]["_children"][0]["_children"][0]["_children"][0]["translation"]);
        Assert.Equal("Invariant 2", result[0]["_children"][0]["_children"][0]["_children"][1]["translation"]);
    }

    [Fact]
    public void GenerateSampleTreeModel_SingleResource()
    {
        var resources = new List<LocalizationResource>
        {
            new("This.Is.Resource.Key", false)
            {
                Translations = new LocalizationResourceTranslationCollection(false)
                {
                    new() // invariant
                    {
                        Language = "", Value = "Invariant"
                    },
                    new() { Language = "en", Value = "English" },
                    new() { Language = "lv", Value = "Latvian" }
                }
            }
        };

        var languages =
            new AvailableLanguage[] { new("English", 1, new CultureInfo("en")), new("Latvian", 2, new CultureInfo("lv")) };

        var sut = new LocalizationResourceApiTreeModel(resources, languages, languages, 100, 100, new UiOptions());

        var result = sut.ConvertToApiModel(resources);
        var first = result.Single();

        Assert.NotNull(first);
        Assert.Equal("This", first[_segmentPropertyName]);
        Assert.Equal("Invariant", result[0]["_children"][0]["_children"][0]["_children"][0]["translation"]);
    }

    [Fact]
    public void GenerateSampleTreeModel_SingleResource_MissingTranslation()
    {
        var resources = new List<LocalizationResource>
        {
            new("This.Is.Resource.Key", false)
            {
                Translations = new LocalizationResourceTranslationCollection(false)
                {
                    new() // invariant
                    {
                        Language = "", Value = "Invariant"
                    },
                    new() { Language = "en", Value = "English" },
                    new() { Language = "lv", Value = "Latvian" }
                }
            }
        };

        var languages = new AvailableLanguage[]
        {
            new("English", 1, new CultureInfo("en")),
            new("Latvian", 2, new CultureInfo("lv")),
            new("Norsk", 2, new CultureInfo("no"))
        };

        var sut = new LocalizationResourceApiTreeModel(resources, languages, languages, 100, 100, new UiOptions());

        var result = sut.ConvertToApiModel(resources);
        var first = result.Single();

        Assert.NotNull(first);
        Assert.Equal("This", first[_segmentPropertyName]);
        Assert.Equal("Invariant", result[0]["_children"][0]["_children"][0]["_children"][0]["translation"]);
        Assert.Null(((JValue)result[0]["_children"][0]["_children"][0]["_children"][0]["translation-no"]).Value);
    }

    [Fact]
    public void GenerateSampleTreeModel_SingleResource_NoSplitInKey()
    {
        var resources = new List<LocalizationResource>
        {
            new("ThisIsResourceKey", false)
            {
                Translations = new LocalizationResourceTranslationCollection(false)
                {
                    new() // invariant
                    {
                        Language = "", Value = "Invariant"
                    },
                    new() { Language = "en", Value = "English" },
                    new() { Language = "lv", Value = "Latvian" }
                }
            }
        };

        var languages =
            new AvailableLanguage[] { new("English", 1, new CultureInfo("en")), new("Latvian", 2, new CultureInfo("lv")) };

        var sut = new LocalizationResourceApiTreeModel(resources, languages, languages, 100, 100, new UiOptions());

        var result = sut.ConvertToApiModel(resources);
        var first = result.Single();

        Assert.NotNull(first);
        Assert.Equal("ThisIsResourceKey", first[_segmentPropertyName]);
    }
}

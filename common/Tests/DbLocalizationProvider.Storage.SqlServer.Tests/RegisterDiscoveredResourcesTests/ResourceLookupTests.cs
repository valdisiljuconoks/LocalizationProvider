using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Internal;
using Xunit;

namespace DbLocalizationProvider.Storage.SqlServer.Tests.RegisterDiscoveredResourcesTests;

/// <summary>
/// The sync used to find each discovered resource with <c>allResources.FirstOrDefault(r => r.ResourceKey == key)</c>.
/// It now uses a dictionary built once per call. These tests check that the lookup and the generated SQL are unchanged.
/// </summary>
public class ResourceLookupTests
{
    private static LocalizationResource Existing(int id, string key, bool? isModified = false, params (string Language, string Value)[] translations)
    {
        var resource = new LocalizationResource(key, false) { Id = id, IsModified = isModified };
        foreach (var (language, value) in translations)
        {
            resource.Translations.Add(new LocalizationResourceTranslation { ResourceId = id, Language = language, Value = value });
        }

        return resource;
    }

    private static DiscoveredResource Discovered(string key, string oldKey = null, bool isHidden = false, params (string Culture, string Value)[] translations)
    {
        var list = translations.Length == 0
            ? new List<DiscoveredTranslation> { new($"Value of {key}", string.Empty), new($"Value of {key}", "en") }
            : translations.Select(t => new DiscoveredTranslation(t.Value, t.Culture)).ToList();

        return new DiscoveredResource(null, key, list, "Property", null, typeof(string), true, isHidden) { OldResourceKey = oldKey };
    }

    private static Lazy<Func<string, LocalizationResource>> DictionaryLookup(IEnumerable<LocalizationResource> all) =>
        new(() => ResourceRepository.CreateResourceLookup(all), LazyThreadSafetyMode.ExecutionAndPublication);

    private static Lazy<Func<string, LocalizationResource>> LinearLookup(IEnumerable<LocalizationResource> all) =>
        new(() => key => all.FirstOrDefault(r => r.ResourceKey == key), LazyThreadSafetyMode.ExecutionAndPublication);

    [Fact]
    public void Lookup_IsOrdinalAndCaseSensitive_AndFirstOccurrenceWins()
    {
        var first = Existing(1, "Key");
        var all = new List<LocalizationResource> { first, Existing(2, "key"), Existing(3, "Key"), Existing(4, "ß") };

        var lookup = ResourceRepository.CreateResourceLookup(all);

        Assert.Same(first, lookup("Key"));
        Assert.Equal(2, lookup("key").Id);
        Assert.Null(lookup("KEY"));
        Assert.Null(lookup("ss"));
        Assert.Null(lookup(null));
        Assert.Null(lookup("missing"));
    }

    [Fact]
    public void Lookup_DegenerateStorage_BehavesLikeFirstOrDefault()
    {
        var lists = new List<List<LocalizationResource>>
        {
            new(),
            new() { Existing(1, "a"), Existing(2, null), Existing(3, "b") },
            new() { Existing(1, "a"), null, Existing(3, "b") },
            new() { null, Existing(1, "a") },
            new() { Existing(1, ""), Existing(2, " a"), Existing(3, "a ") }
        };
        var probes = new[] { "a", "A", "b", null, "", " a", "a " };

        foreach (var list in lists)
        {
            var lookup = ResourceRepository.CreateResourceLookup(list);
            foreach (var key in probes)
            {
                Assert.Equal(Outcome(() => list.FirstOrDefault(r => r.ResourceKey == key)), Outcome(() => lookup(key)));
            }
        }

        Assert.Throws<ArgumentNullException>(() => ResourceRepository.CreateResourceLookup(null)("a"));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void RegistrationScript_IsIdenticalToLinearLookup(bool flexibleRefactoringMode)
    {
        var (discovered, all) = CreateResources(1000);
        var dictionary = DictionaryLookup(all);
        var linear = LinearLookup(all);

        var groups = 0;
        foreach (var group in discovered.SplitByCount(400))
        {
            var expected = ResourceRepository.BuildRegistrationScript(group, linear, flexibleRefactoringMode);
            var actual = ResourceRepository.BuildRegistrationScript(group, dictionary, flexibleRefactoringMode);

            Assert.Equal(expected, actual);
            groups++;
        }

        Assert.Equal(3, groups);
    }

    [Fact]
    public void RegistrationScript_UpdatesExistingResource_AndInsertsNewOne()
    {
        var all = new List<LocalizationResource> { Existing(42, "Existing.Key", false, ("en", "old")) };
        var discovered = new List<DiscoveredResource>
        {
            Discovered("Existing.Key", translations: ("en", "new")),
            Discovered("New.Key", translations: ("en", "It's new"))
        };

        var script = ResourceRepository.BuildRegistrationScript(discovered, DictionaryLookup(all), false);

        Assert.Contains("UPDATE LocalizationResources SET FromCode = 1, IsHidden = 0 where [Id] = 42", script);
        Assert.Contains("SET [Value] = N'new' WHERE ResourceId=42 and [Language]='en'", script);
        Assert.DoesNotContain("VALUES ('Existing.Key'", script);
        Assert.Contains("VALUES ('New.Key', GETUTCDATE(), 'type-scanner', 1, 0, 0)", script);
        Assert.Contains("N'It''s new'", script);
    }

    [Fact]
    public void StorageIsEnumeratedOncePerCall_NotOncePerDiscoveredResource()
    {
        var (discovered, all) = CreateResources(1000);
        var counting = new CountingEnumerable<LocalizationResource>(all);
        var lookup = DictionaryLookup(counting);

        foreach (var group in discovered.SplitByCount(400))
        {
            ResourceRepository.BuildRegistrationScript(group, lookup, false);
        }

        Assert.Equal(1, counting.Enumerations);
    }

    // Deterministic mix: existing and new keys, keys that differ only by case, renamed keys, apostrophes,
    // invariant and English translations, and existing rows that are modified, unmodified or unknown.
    private static (List<DiscoveredResource> Discovered, List<LocalizationResource> All) CreateResources(int count)
    {
        var discovered = new List<DiscoveredResource>();
        var all = new List<LocalizationResource>();

        for (var i = 0; i < count; i++)
        {
            var key = $"MyProject.Resources.Type{i / 10}.Property{i}" + (i % 17 == 0 ? "'s" : string.Empty);
            var oldKey = i % 23 == 0 ? $"MyProject.Resources.Old{i}" : null;
            discovered.Add(Discovered(key, oldKey, i % 11 == 0, (string.Empty, $"Invariant {i}"), ("en", $"English {i}")));

            switch (i % 5)
            {
                case 0: // new resource
                    break;
                case 1:
                    all.Add(Existing(i + 1, key, false, (string.Empty, $"Invariant {i}"), ("en", $"English {i}")));
                    break;
                case 2:
                    all.Add(Existing(i + 1, key, false, ("en", $"Changed {i}")));
                    break;
                case 3:
                    all.Add(Existing(i + 1, key, i % 2 == 0 ? true : null, ("en", $"English {i}")));
                    break;
                default: // stored with different casing: not a match, as before
                    all.Add(Existing(i + 1, key.ToUpperInvariant(), false));
                    break;
            }
        }

        for (var i = 0; i < count / 2; i++)
        {
            all.Add(Existing(100000 + i, $"MyProject.Unrelated.Key{i}"));
        }

        return (discovered, all);
    }

    private static object Outcome(Func<LocalizationResource> f)
    {
        try
        {
            return f();
        }
        catch (Exception ex)
        {
            return ex.GetType();
        }
    }

    private sealed class CountingEnumerable<T>(IEnumerable<T> inner) : IEnumerable<T>
    {
        public int Enumerations { get; private set; }

        public IEnumerator<T> GetEnumerator()
        {
            Enumerations++;
            return inner.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}

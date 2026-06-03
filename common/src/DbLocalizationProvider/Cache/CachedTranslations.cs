// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Cache;

/// <summary>
/// Compact representation of a resource's translations, sized for long-lived cache residency.
/// Stores only what <see cref="Queries.GetTranslation.Handler"/> needs at runtime - a culture-indexed
/// map of translation values - and discards admin/persistence fields carried by
/// <see cref="LocalizationResource"/> and <see cref="LocalizationResourceTranslation"/>.
/// </summary>
internal sealed class CachedTranslations
{
    private static readonly ConcurrentDictionary<string, string> s_cultureKeyPool =
        new(StringComparer.OrdinalIgnoreCase);

    public static readonly CachedTranslations NonExisting = new(translations: null);

    private readonly Dictionary<string, string>? _byCulture;

    private CachedTranslations(Dictionary<string, string>? translations)
    {
        _byCulture = translations;
    }

    public int Count => _byCulture?.Count ?? 0;

    public static CachedTranslations From(LocalizationResource resource)
    {
        ArgumentNullException.ThrowIfNull(resource);

        var source = resource.Translations;
        if (source == null || source.Count == 0)
        {
            return NonExisting;
        }

        var map = new Dictionary<string, string>(source.Count, StringComparer.OrdinalIgnoreCase);
        for (var i = 0; i < source.Count; i++)
        {
            var t = source[i];
            var language = t.Language ?? string.Empty;
            var key = s_cultureKeyPool.GetOrAdd(language, language);
            map[key] = t.Value;
        }

        return new CachedTranslations(map);
    }

    /// <summary>
    /// Mirrors <see cref="LocalizationResourceTranslationCollection.ByLanguage(string, bool)"/>.
    /// </summary>
    public string? ByLanguage(string language, bool invariantCultureFallback)
    {
        ArgumentNullException.ThrowIfNull(language);

        if (_byCulture == null)
        {
            return invariantCultureFallback ? null : string.Empty;
        }

        if (_byCulture.TryGetValue(language, out var value))
        {
            return value;
        }

        if (invariantCultureFallback)
        {
            return _byCulture.TryGetValue(string.Empty, out var invariant) ? invariant : null;
        }

        return string.Empty;
    }

    /// <summary>
    /// Mirrors <see cref="LocalizationResourceTranslationCollection.GetValueWithFallback(CultureInfo, IReadOnlyCollection{CultureInfo})"/>.
    /// </summary>
    public string? GetValueWithFallback(CultureInfo language, IReadOnlyCollection<CultureInfo> fallbackLanguages)
    {
        ArgumentNullException.ThrowIfNull(language);
        ArgumentNullException.ThrowIfNull(fallbackLanguages);

        if (_byCulture == null)
        {
            return null;
        }

        if (_byCulture.TryGetValue(language.Name, out var direct))
        {
            return direct;
        }

        var parent = language.Parent;
        if (!parent.Equals(CultureInfo.InvariantCulture)
            && _byCulture.TryGetValue(parent.Name, out var fromParent))
        {
            return fromParent;
        }

        if (fallbackLanguages is IReadOnlyList<CultureInfo> indexed)
        {
            var startIndex = 0;
            var count = indexed.Count;
            for (var i = 0; i < count; i++)
            {
                if (indexed[i].Equals(language))
                {
                    startIndex = i + 1;
                    break;
                }
            }

            for (var i = startIndex; i < count; i++)
            {
                if (_byCulture.TryGetValue(indexed[i].Name, out var found))
                {
                    return found;
                }
            }

            return null;
        }

        var passedRequested = false;
        foreach (var fallbackCulture in fallbackLanguages)
        {
            if (!passedRequested)
            {
                if (fallbackCulture.Equals(language))
                {
                    passedRequested = true;
                }

                continue;
            }

            if (_byCulture.TryGetValue(fallbackCulture.Name, out var found))
            {
                return found;
            }
        }

        if (!passedRequested)
        {
            foreach (var fallbackCulture in fallbackLanguages)
            {
                if (_byCulture.TryGetValue(fallbackCulture.Name, out var found))
                {
                    return found;
                }
            }
        }

        return null;
    }
}

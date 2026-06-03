// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using System.Globalization;

namespace DbLocalizationProvider.Abstractions;

/// <summary>
/// Collection of translations.
/// </summary>
public class LocalizationResourceTranslationCollection : List<LocalizationResourceTranslation>
{
    private readonly bool _enableInvariantCultureFallback;

    /// <summary>
    /// Initializes new instance of the class. Required for JSON deserialization; invariant fallback is disabled by default.
    /// </summary>
    public LocalizationResourceTranslationCollection() { }

    /// <summary>
    /// Initializes new instance of the class with specified fallback to invariant language setting.
    /// </summary>
    /// <param name="enableInvariantCultureFallback">Should we use invariant fallback or not.</param>
    public LocalizationResourceTranslationCollection(bool enableInvariantCultureFallback)
    {
        _enableInvariantCultureFallback = enableInvariantCultureFallback;
    }

    /// <summary>
    /// Finds translation the by language.
    /// </summary>
    /// <param name="language">The language.</param>
    /// <returns>Translation class</returns>
    public LocalizationResourceTranslation? FindByLanguage(CultureInfo language)
    {
        return FindByLanguage(language.Name);
    }

    /// <summary>
    /// Finds translation by language.
    /// </summary>
    /// <param name="language">The language.</param>
    /// <returns>Translation class</returns>
    public LocalizationResourceTranslation? FindByLanguage(string? language)
    {
        for (var i = 0; i < Count; i++)
        {
            var translation = this[i];
            if (string.Equals(translation.Language, language, StringComparison.OrdinalIgnoreCase))
            {
                return translation;
            }
        }

        return null;
    }

    /// <summary>
    /// Find translation in invariant culture.
    /// </summary>
    /// <returns>Translation class</returns>
    public LocalizationResourceTranslation? InvariantTranslation()
    {
        return FindByLanguage(CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Finds translation by language.
    /// </summary>
    /// <param name="language">The language.</param>
    /// <returns>Translation class</returns>
    public string? ByLanguage(CultureInfo language)
    {
        return ByLanguage(language.Name);
    }

    /// <summary>
    /// Finds translation by language.
    /// </summary>
    /// <param name="language">The language.</param>
    /// <returns>Translation class</returns>
    public string? ByLanguage(string? language)
    {
        return ByLanguage(language, _enableInvariantCultureFallback);
    }

    /// <summary>
    /// Finds translation by language.
    /// </summary>
    /// <param name="language">The language.</param>
    /// <param name="invariantCultureFallback">if set to <c>true</c> invariant culture fallback is used.</param>
    /// <returns>Translation class</returns>
    public string? ByLanguage(CultureInfo? language, bool invariantCultureFallback)
    {
        return ByLanguage(language.Name, invariantCultureFallback);
    }

    /// <summary>
    /// Finds translation by language.
    /// </summary>
    /// <param name="language">The language.</param>
    /// <param name="invariantCultureFallback">if set to <c>true</c> [invariant culture fallback].</param>
    /// <returns>Translation class</returns>
    /// <exception cref="ArgumentNullException">language</exception>
    public string? ByLanguage(string? language, bool invariantCultureFallback)
    {
        ArgumentNullException.ThrowIfNull(language);

        var translation = FindByLanguage(language);

        return translation != null
            ? translation.Value
            : invariantCultureFallback
                ? FindByLanguage(string.Empty)?.Value
                : string.Empty;
    }

    /// <summary>
    /// Checks whether translation exists in given language.
    /// </summary>
    /// <param name="language">The language.</param>
    /// <returns><c>true</c> is translation exists; otherwise <c>false</c></returns>
    /// <exception cref="ArgumentNullException">language</exception>
    public bool ExistsLanguage(string language)
    {
        ArgumentNullException.ThrowIfNull(language);

        return FindByLanguage(language) != null;
    }

    /// <summary>
    /// Get translation in given language or in any of fallback languages
    /// </summary>
    /// <param name="language">Language in which to get translation first</param>
    /// <param name="fallbackLanguages">
    /// If translation does not exist in language supplied by parameter <paramref name="language" /> then this list
    /// of fallback languages is used to find translation
    /// </param>
    /// <returns>Translation in requested language or uin any fallback languages; <c>null</c> otherwise if translation is not found</returns>
    public string? GetValueWithFallback(CultureInfo? language, IReadOnlyCollection<CultureInfo> fallbackLanguages)
    {
        return GetValueWithFallback(language.Name, fallbackLanguages);
    }

    /// <summary>
    /// Get translation in given language or in any of fallback languages
    /// </summary>
    /// <param name="language">Language in which to get translation first</param>
    /// <param name="fallbackLanguages">
    /// If translation does not exist in language supplied by parameter <paramref name="language" /> then this list
    /// of fallback languages is used to find translation
    /// </param>
    /// <returns>Translation in requested language or uin any fallback languages; <c>null</c> otherwise if translation is not found</returns>
    public string? GetValueWithFallback(string? language, IReadOnlyCollection<CultureInfo> fallbackLanguages)
    {
        ArgumentNullException.ThrowIfNull(language);
        ArgumentNullException.ThrowIfNull(fallbackLanguages);

        var inRequestedLanguage = FindByLanguage(language);
        if (inRequestedLanguage != null)
        {
            return inRequestedLanguage.Value;
        }

        // Try parent culture (e.g., fr-BE -> fr).
        var requestedCulture = CultureInfo.GetCultureInfo(language);
        var parent = requestedCulture.Parent;
        if (!parent.Equals(CultureInfo.InvariantCulture))
        {
            var inParentLanguage = FindByLanguage(parent.Name);
            if (inParentLanguage != null)
            {
                return inParentLanguage.Value;
            }
        }

        // Walk the fallback chain. If the requested culture appears in the chain,
        // skip past it (and everything before) - those have already been tried.
        if (fallbackLanguages is IReadOnlyList<CultureInfo> indexed)
        {
            var startIndex = 0;
            var count = indexed.Count;
            for (var i = 0; i < count; i++)
            {
                if (indexed[i].Equals(requestedCulture))
                {
                    startIndex = i + 1;
                    break;
                }
            }

            for (var i = startIndex; i < count; i++)
            {
                var found = FindByLanguage(indexed[i].Name);
                if (found != null)
                {
                    return found.Value;
                }
            }

            return null;
        }

        // Generic IReadOnlyCollection - rare in practice. Skip past the requested
        // culture if present, then walk the remainder.
        var passedRequested = false;
        foreach (var fallbackCulture in fallbackLanguages)
        {
            if (!passedRequested)
            {
                if (fallbackCulture.Equals(requestedCulture))
                {
                    passedRequested = true;
                }

                continue;
            }

            var found = FindByLanguage(fallbackCulture.Name);
            if (found != null)
            {
                return found.Value;
            }
        }

        if (!passedRequested)
        {
            // Requested culture was not in the fallback chain - walk the whole chain.
            foreach (var fallbackCulture in fallbackLanguages)
            {
                var found = FindByLanguage(fallbackCulture.Name);
                if (found != null)
                {
                    return found.Value;
                }
            }
        }

        return null;
    }
}

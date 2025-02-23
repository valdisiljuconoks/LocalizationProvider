// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace DbLocalizationProvider.Abstractions;

/// <summary>
/// Collection of translations.
/// </summary>
public class LocalizationResourceTranslationCollection : List<LocalizationResourceTranslation>
{
    private readonly bool _enableInvariantCultureFallback;

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
        return this.FirstOrDefault(t => t.Language == language);
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

        return this.FirstOrDefault(t => t.Language == language) != null;
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

        // check if we have regional language. if so - maybe we have parent language available
        var cultureInfo = CultureInfo.GetCultureInfo(language);
        if (!cultureInfo.Parent.Equals(CultureInfo.InvariantCulture))
        {
            var inParentLanguage = FindByLanguage(cultureInfo.Parent.Name);
            if (inParentLanguage != null)
            {
                return inParentLanguage.Value;
            }
        }

        // find if requested language is not "inside" fallback languages
        var culture = CultureInfo.GetCultureInfo(language);
        var searchableLanguages = fallbackLanguages.ToList();

        if (fallbackLanguages.Contains(culture))
        {
            // requested language is inside fallback languages, so we need to "continue" from there
            var restOfFallbackLanguages = fallbackLanguages.SkipWhile(c => !Equals(c, culture)).ToList();

            // check if we are not at the end of the list
            if (restOfFallbackLanguages.Any())
            {
                // if there are still elements - we have to skip 1 (as this is requested language)
                searchableLanguages = restOfFallbackLanguages.Skip(1).ToList();
            }
        }

        foreach (var fallbackLanguage in searchableLanguages)
        {
            var translationInFallback = FindByLanguage(fallbackLanguage);
            if (translationInFallback != null)
            {
                return translationInFallback.Value;
            }
        }

        return null;
    }
}

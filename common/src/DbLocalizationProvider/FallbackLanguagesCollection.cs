// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using System.Globalization;

namespace DbLocalizationProvider;

/// <summary>
/// Collection of fallback language settings. Contains default settings (flat list without specific fallback for specific languages).
/// </summary>
public class FallbackLanguagesCollection
{
    private readonly Dictionary<string, FallbackLanguages> _collection = [];

    private readonly FallbackLanguages _defaultFallbackLanguages;

    /// <summary>
    /// Creates new instance of this collection.
    /// </summary>
    public FallbackLanguagesCollection()
    {
        _defaultFallbackLanguages = new FallbackLanguages(this);
    }

    /// <summary>
    /// Creates new instance of this collection.
    /// </summary>
    /// <param name="fallbackCulture">Specifies default fallback language.</param>
    public FallbackLanguagesCollection(CultureInfo fallbackCulture)
    {
        _defaultFallbackLanguages = new FallbackLanguages(this) { fallbackCulture };
    }

    /// <summary>
    /// Get list of fallback languages configured for <paramref name="language" />.
    /// </summary>
    /// <param name="language">Language to get fallback languages for.</param>
    /// <returns>The list of registered fallback languages for given <paramref name="language" />.</returns>
    public FallbackLanguages GetFallbackLanguages(CultureInfo language)
    {
        ArgumentNullException.ThrowIfNull(language);

        return GetFallbackLanguages(language.Name);
    }

    /// <summary>
    /// Get list of fallback languages configured for <paramref name="language" />.
    /// </summary>
    /// <param name="language">Language to get fallback languages for.</param>
    /// <returns>The list of registered fallback languages for given <paramref name="language" />.</returns>
    public FallbackLanguages GetFallbackLanguages(string language)
    {
        ArgumentNullException.ThrowIfNull(language);

        return _collection.TryGetValue(language, out var fallbackLanguages)
            ? fallbackLanguages
            : _defaultFallbackLanguages;
    }

    /// <summary>
    /// Adds new fallback language settings for specified language.
    /// </summary>
    /// <param name="notFoundCulture">Fallback languages will be enforced when resource for this language is not found.</param>
    /// <returns>List of fallback languages on which you can call extension methods to get list configured.</returns>
    public FallbackLanguages Add(CultureInfo notFoundCulture)
    {
        ArgumentNullException.ThrowIfNull(notFoundCulture);

        if (_collection.ContainsKey(notFoundCulture.Name))
        {
            throw new ArgumentException($"Fallback languages already have setting for `{notFoundCulture.Name}` language");
        }

        var list = new FallbackLanguages(this);
        _collection.Add(notFoundCulture.Name, list);

        return list;
    }
}

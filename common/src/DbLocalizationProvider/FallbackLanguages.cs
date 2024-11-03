// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using DbLocalizationProvider.Internal;

namespace DbLocalizationProvider;

/// <summary>
/// List of fallback languages.
/// </summary>
public class FallbackLanguages : IReadOnlyCollection<CultureInfo>
{
    private readonly List<CultureInfo> _fallbackLanguages = new();

    internal FallbackLanguages(FallbackLanguagesCollection parentCollection)
    {
        FallbackLanguagesCollection = parentCollection;
    }

    internal FallbackLanguagesCollection FallbackLanguagesCollection { get; }

    /// <inheritdoc />
    public IEnumerator<CultureInfo> GetEnumerator()
    {
        return _fallbackLanguages.GetEnumerator();
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <inheritdoc />
    public int Count => _fallbackLanguages.Count;

    /// <summary>
    /// Registers fallback language.
    /// </summary>
    /// <param name="fallbackLanguage">The fallback language.</param>
    /// <returns>The same list to support chaining</returns>
    /// <exception cref="ArgumentNullException">fallbackLanguage</exception>
    public FallbackLanguages Try(CultureInfo fallbackLanguage)
    {
        if (fallbackLanguage == null)
        {
            throw new ArgumentNullException(nameof(fallbackLanguage));
        }

        _fallbackLanguages.Add(fallbackLanguage);

        return this;
    }

    /// <summary>
    /// Add new language to the list of fallback languages.
    /// </summary>
    /// <param name="language">Language to add.</param>
    /// <returns>The same list so you can do fluent stuff.</returns>
    public FallbackLanguages Add(CultureInfo language)
    {
        _fallbackLanguages.Add(language);

        return this;
    }

    /// <summary>
    /// Registers fallback languages.
    /// </summary>
    /// <param name="fallbackLanguages">The fallback languages.</param>
    /// <returns>The same list of registered fallback languages to support API chaining (that fluent thingy).</returns>
    /// <exception cref="ArgumentNullException">fallbackLanguages</exception>
    public FallbackLanguages Try(IList<CultureInfo> fallbackLanguages)
    {
        if (fallbackLanguages == null)
        {
            throw new ArgumentNullException(nameof(fallbackLanguages));
        }

        fallbackLanguages.ForEach(_fallbackLanguages.Add);

        return this;
    }

    /// <summary>
    /// Registered specified fallback language.
    /// </summary>
    /// <param name="fallbackLanguage">The fallback language.</param>
    /// <returns>The same list of registered fallback languages to support API chaining (that fluent thingy).</returns>
    public FallbackLanguages Then(CultureInfo fallbackLanguage)
    {
        return Try(fallbackLanguage);
    }
}

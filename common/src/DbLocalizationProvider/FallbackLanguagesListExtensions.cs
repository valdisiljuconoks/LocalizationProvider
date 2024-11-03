// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Globalization;

namespace DbLocalizationProvider;

/// <summary>
/// Extensions for language fallback list.
/// </summary>
public static class FallbackLanguagesListExtensions
{
    /// <summary>
    /// Extension method to use to configure fallback languages. Use this method when you want to specify which languages to use when
    /// <paramref name="notFoundCulture" /> language was not found.
    /// </summary>
    /// <param name="list">List of fallback languages.</param>
    /// <param name="notFoundCulture">Configure fallback languages for this language.</param>
    /// <returns>The same list of registered fallback languages to support API chaining (that fluent thingy).</returns>
    public static FallbackLanguages When(this FallbackLanguages list, CultureInfo notFoundCulture)
    {
        return list.FallbackLanguagesCollection.Add(notFoundCulture);
    }
}

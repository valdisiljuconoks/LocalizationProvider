// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Collections.Generic;
using System.Linq;
using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Sync;

/// <summary>
/// Provides extension methods for collections of <see cref="DiscoveredTranslation"/>.
/// </summary>
public static class ListOfDiscoveredTranslationExtensions
{
    /// <summary>
    /// Gets the default translation from the collection of <see cref="DiscoveredTranslation"/>.
    /// </summary>
    /// <param name="target">The collection of <see cref="DiscoveredTranslation"/>.</param>
    /// <returns>The default translation if found; otherwise, <c>null</c>.</returns>
    public static string? DefaultTranslation(this ICollection<DiscoveredTranslation> target)
    {
        return target.FirstOrDefault(t => !string.IsNullOrEmpty(t.Culture))?.Translation;
    }
}

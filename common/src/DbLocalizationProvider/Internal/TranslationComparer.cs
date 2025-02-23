// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Collections.Generic;
using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Internal;

/// <summary>
/// Compares two <see cref="LocalizationResourceTranslation" /> objects for equality.
/// </summary>
internal class TranslationComparer(bool ignoreInvariantCulture) : IEqualityComparer<LocalizationResourceTranslation>
{
    /// <summary>
    /// Determines whether the specified <see cref="LocalizationResourceTranslation" /> objects are equal.
    /// </summary>
    /// <param name="x">The first <see cref="LocalizationResourceTranslation" /> to compare.</param>
    /// <param name="y">The second <see cref="LocalizationResourceTranslation" /> to compare.</param>
    /// <returns><c>true</c> if the specified objects are equal; otherwise, <c>false</c>.</returns>
    public bool Equals(LocalizationResourceTranslation? x, LocalizationResourceTranslation? y)
    {
        if (ReferenceEquals(x, y))
        {
            return true;
        }

        if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
        {
            return false;
        }

        if (x.GetType() != y.GetType())
        {
            return false;
        }

        if (x.Language == string.Empty && y.Language == string.Empty && ignoreInvariantCulture)
        {
            return true;
        }

        return string.Equals(x.Language, y.Language) && string.Equals(x.Value, y.Value);
    }

    /// <summary>
    /// Returns a hash code for the specified <see cref="LocalizationResourceTranslation" />.
    /// </summary>
    /// <param name="obj">The <see cref="LocalizationResourceTranslation" /> for which a hash code is to be returned.</param>
    /// <returns>A hash code for the specified object.</returns>
    public int GetHashCode(LocalizationResourceTranslation obj)
    {
        unchecked
        {
            return obj.Language?.GetHashCode() ?? 0;
        }
    }
}

/// <summary>
/// Compares two <see cref="LocalizationResource" /> objects for equality.
/// </summary>
internal class ResourceComparer : IEqualityComparer<LocalizationResource>
{
    /// <summary>
    /// Determines whether the specified <see cref="LocalizationResource" /> objects are equal.
    /// </summary>
    /// <param name="x">The first <see cref="LocalizationResource" /> to compare.</param>
    /// <param name="y">The second <see cref="LocalizationResource" /> to compare.</param>
    /// <returns><c>true</c> if the specified objects are equal; otherwise, <c>false</c>.</returns>
    public bool Equals(LocalizationResource? x, LocalizationResource? y)
    {
        if (ReferenceEquals(x, y))
        {
            return true;
        }

        if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
        {
            return false;
        }

        if (x.GetType() != y.GetType())
        {
            return false;
        }

        return string.Equals(x.ResourceKey, y.ResourceKey);
    }

    /// <summary>
    /// Returns a hash code for the specified <see cref="LocalizationResource" />.
    /// </summary>
    /// <param name="obj">The <see cref="LocalizationResource" /> for which a hash code is to be returned.</param>
    /// <returns>A hash code for the specified object.</returns>
    public int GetHashCode(LocalizationResource obj)
    {
        unchecked
        {
            return (obj.ResourceKey.GetHashCode() * 397) ^ obj.ResourceKey.GetHashCode();
        }
    }
}

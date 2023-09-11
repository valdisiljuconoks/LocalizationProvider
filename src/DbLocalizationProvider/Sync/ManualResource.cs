// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Globalization;

namespace DbLocalizationProvider.Sync;

/// <summary>
/// Instance of manually crafted resource.
/// </summary>
public class ManualResource
{
    /// <summary>
    /// Create new manual resource.
    /// </summary>
    /// <param name="key">Key of the resource.</param>
    /// <param name="translation">Translation of the resource for given <paramref name="language" />.</param>
    /// <param name="language">For which language this translation is.</param>
    public ManualResource(string key, string translation, CultureInfo language)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentNullException(nameof(key));
        }

        Key = key;

        Translation = translation ?? throw new ArgumentNullException(nameof(translation));
        Language = language ?? throw new NullReferenceException($"Resource language cannot be null (Key: {key}).");
    }

    /// <summary>
    /// Key of the resource.
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// Translation of the resource for given <see cref="Language" />.
    /// </summary>
    public string Translation { get; }

    /// <summary>
    /// For which language this translation is.
    /// </summary>
    public CultureInfo Language { get; }
}

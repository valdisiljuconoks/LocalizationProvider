// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Diagnostics;

namespace DbLocalizationProvider.Abstractions;

/// <summary>
/// Class describing discovered translation.
/// </summary>
[DebuggerDisplay("Translation: {Translation} / Culture: {Culture}")]
public class DiscoveredTranslation
{
    /// <summary>
    /// Creates new instance
    /// </summary>
    /// <param name="translation">Found translation.</param>
    /// <param name="culture">Translation language.</param>
    public DiscoveredTranslation(string translation, string? culture)
    {
        Translation = translation;
        Culture = culture;
    }

    /// <summary>
    /// Found translation.
    /// </summary>
    public string Translation { get; internal set; }

    /// <summary>
    /// Translation language.
    /// </summary>
    public string? Culture { get; }
}

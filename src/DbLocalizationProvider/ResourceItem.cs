// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Globalization;

namespace DbLocalizationProvider;

/// <summary>
/// Represents item fo the localizable resource
/// </summary>
public class ResourceItem
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ResourceItem" /> class.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    /// <param name="sourceCulture">The source culture.</param>
    public ResourceItem(string key, string value, CultureInfo sourceCulture)
    {
        Key = key;
        Value = value;
        SourceCulture = sourceCulture;
    }

    /// <summary>
    /// Gets the key.
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// Gets the value.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Gets the source culture.
    /// </summary>
    public CultureInfo SourceCulture { get; }
}

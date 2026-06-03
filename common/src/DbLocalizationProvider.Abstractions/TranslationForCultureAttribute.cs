// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Globalization;

namespace DbLocalizationProvider.Abstractions;

/// <summary>
/// If you wanna provide additional translations for the same resource for multiple languages, you gonna need this
/// attribute.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
public class TranslationForCultureAttribute : Attribute
{
    /// <summary>
    /// Obviously creates new instance of the attribute
    /// </summary>
    /// <param name="translation">Translation of the resource for given language.</param>
    /// <param name="culture">
    /// Language for the additional translation (will be used as argument for <see cref="CultureInfo" />
    /// ).
    /// </param>
    public TranslationForCultureAttribute(string translation, string culture)
    {
        Translation = translation;
        Culture = CultureInfo.GetCultureInfo(culture).Name;
    }

    /// <summary>
    /// Translation of the resource for given language.
    /// </summary>
    public string Translation { get; }

    /// <summary>
    /// Language for the additional translation (will be used as argument for <see cref="CultureInfo" />).
    /// </summary>
    public string? Culture { get; }
}

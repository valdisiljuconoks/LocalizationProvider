// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;

namespace DbLocalizationProvider.AdminUI.Models;

/// <summary>
/// Description class for supported languages
/// </summary>
public class CultureApiModel
{
    /// <summary>
    /// Creates new instance
    /// </summary>
    /// <param name="code">ISO code of the language (e.g. en-US)</param>
    /// <param name="display">Display name of the language</param>
    public CultureApiModel(string? code, string display)
    {
        Code = code ?? throw new ArgumentNullException(nameof(code));
        Display = display ?? throw new ArgumentNullException(nameof(display));
        TitleDisplay = $"{display}{(code != string.Empty ? " (" + code + ")" : string.Empty)}";
    }

    /// <summary>
    /// ISO code of the language (e.g. en-US)
    /// </summary>
    public string? Code { get; }

    /// <summary>
    /// Display name of the language
    /// </summary>
    public string Display { get; }

    /// <summary>
    /// Display name of the language in the title bar of the modal window
    /// </summary>
    public string TitleDisplay { get; }
}

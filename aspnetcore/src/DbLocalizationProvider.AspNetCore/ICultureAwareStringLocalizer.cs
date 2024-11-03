// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Globalization;
using Microsoft.Extensions.Localization;

namespace DbLocalizationProvider.AspNetCore;

/// <summary>
/// Workaround interface for changing language on <see cref="IStringLocalizer" />
/// </summary>
public interface ICultureAwareStringLocalizer
{
    /// <summary>
    /// Change language of the provider and returns string localizer with specified language.
    /// </summary>
    /// <param name="language">Language to change to.</param>
    /// <returns>Localizer with specified language.</returns>
    IStringLocalizer ChangeLanguage(CultureInfo language);
}

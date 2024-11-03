// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Globalization;
using System.Linq.Expressions;
using Microsoft.Extensions.Localization;

namespace DbLocalizationProvider.AspNetCore;

/// <summary>
/// Some extensions for <see cref="IStringLocalizer" />.
/// </summary>
public static class IStringLocalizerExtensions
{
    /// <summary>
    /// Returns resource translation.
    /// </summary>
    /// <param name="target">Target type for which extension methods are defined.</param>
    /// <param name="model">Expression of the resource key.</param>
    /// <param name="formatArguments">Maybe some formatting is needed (like substitution of the placeholders).</param>
    /// <returns>Resource translation (if any).</returns>
    public static LocalizedString GetString(
        this IStringLocalizer target,
        Expression<Func<object>> model,
        params object[] formatArguments)
    {
        return target[target.GetResourceName(model), formatArguments];
    }

    /// <summary>
    /// Returns resource translation by given language.
    /// </summary>
    /// <param name="target">Target type for which extension methods are defined.</param>
    /// <param name="model">Expression of the resource key.</param>
    /// <param name="language"></param>
    /// <param name="formatArguments">Maybe some formatting is needed (like substitution of the placeholders).</param>
    /// <returns>Resource translation (if any).</returns>
    public static LocalizedString GetStringByCulture(
        this IStringLocalizer target,
        Expression<Func<object>> model,
        CultureInfo language,
        params object[] formatArguments)
    {
        if (model == null)
        {
            throw new ArgumentNullException(nameof(model));
        }

        if (language == null)
        {
            throw new ArgumentNullException(nameof(language));
        }

        var localizer = target.GetField<DbStringLocalizer>("_localizer");
        if (localizer is ICultureAwareStringLocalizer cultureAwareLocalizer)
        {
            return cultureAwareLocalizer.ChangeLanguage(language)[target.GetResourceName(model), formatArguments];
        }

        return null;
    }

    private static string GetResourceName(this IStringLocalizer target, LambdaExpression model)
    {
        var localizer = target.GetField<DbStringLocalizer>("_localizer");
        if (localizer != null)
        {
            return localizer.ExpressionHelper.GetFullMemberName(model);
        }

        return string.Empty;
    }
}

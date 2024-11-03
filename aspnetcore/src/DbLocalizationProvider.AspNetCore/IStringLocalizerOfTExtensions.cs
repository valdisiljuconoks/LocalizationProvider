// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Globalization;
using System.Linq.Expressions;
using Microsoft.Extensions.Localization;

namespace DbLocalizationProvider.AspNetCore;

/// <summary>
/// Some extensions for <see cref="IStringLocalizer{T}" />.
/// </summary>
public static class IStringLocalizerOfTExtensions
{
    /// <summary>
    /// Returns resource translation.
    /// </summary>
    /// <typeparam name="T">Type of the model (resource class).</typeparam>
    /// <param name="target">Target type for which extension methods are defined.</param>
    /// <param name="model">Expression of the resource key.</param>
    /// <param name="formatArguments">Maybe some formatting is needed (like substitution of the placeholders).</param>
    /// <returns>Resource translation (if any).</returns>
    public static LocalizedString GetString<T>(
        this IStringLocalizer<T> target,
        Expression<Func<T, object>> model,
        params object[] formatArguments)
    {
        return target[GetResourceName(target, model), formatArguments];
    }

    /// <summary>
    /// Returns resource translation by given language.
    /// </summary>
    /// <typeparam name="T">Type of the model (resource class).</typeparam>
    /// <param name="target">Target type for which extension methods are defined.</param>
    /// <param name="model">Expression of the resource key.</param>
    /// <param name="language"></param>
    /// <param name="formatArguments">Maybe some formatting is needed (like substitution of the placeholders).</param>
    /// <returns>Resource translation (if any).</returns>
    public static LocalizedString GetStringByCulture<T>(
        this IStringLocalizer<T> target,
        Expression<Func<T, object>> model,
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
            return cultureAwareLocalizer.ChangeLanguage(language)[GetResourceName(target, model), formatArguments];
        }

        return null;
    }

    private static string GetResourceName<T>(IStringLocalizer<T> target, LambdaExpression model)
    {
        var localizer = target.GetField<DbStringLocalizer>("_localizer");
        if (localizer != null)
        {
            return localizer.ExpressionHelper.GetFullMemberName(model);
        }

        return string.Empty;
    }
}

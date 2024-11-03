// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Globalization;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc.Localization;

namespace DbLocalizationProvider.AspNetCore;

/// <summary>
/// For the stuff that's missing.
/// </summary>
public static class IHtmlLocalizerExtensions
{
    /// <summary>
    /// Gets resource translation.
    /// </summary>
    /// <param name="target">Localizer.</param>
    /// <param name="model">Resource expression.</param>
    /// <param name="formatArguments">Message formatting arguments.</param>
    /// <returns>Html string if translation is found for resource in requested language.</returns>
    public static LocalizedHtmlString GetString(
        this IHtmlLocalizer target,
        Expression<Func<object>> model,
        params object[] formatArguments)
    {
        return target[GetMemberName(target, model), formatArguments];
    }

    /// <summary>
    /// Returns resource in requested language (USE WITH CAUTION! is this involves some magic to get it done).
    /// </summary>
    /// <param name="target">Localizer.</param>
    /// <param name="model">Resource expression.</param>
    /// <param name="language">Language in which you would like to get resource translation back.</param>
    /// <param name="formatArguments">Message formatting arguments.</param>
    /// <returns>Html string if translation is found for resource in requested language.</returns>
    public static LocalizedHtmlString GetStringByCulture(
        this IHtmlLocalizer target,
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

        if (target is ICultureAwareHtmlLocalizer cultureAwareLocalizer)
        {
            return cultureAwareLocalizer.ChangeLanguage(language)[GetMemberName(target, model), formatArguments];
        }

        return null;
    }

    /// <summary>
    /// Gets resource translation.
    /// </summary>
    /// <param name="target">Localizer.</param>
    /// <param name="model">Resource expression.</param>
    /// <param name="formatArguments">Message formatting arguments.</param>
    /// <returns>Html string if translation is found for resource in requested language.</returns>
    public static LocalizedHtmlString GetString<T>(
        this IHtmlLocalizer<T> target,
        Expression<Func<T, object>> model,
        params object[] formatArguments)
    {
        return target[GetMemberName(target, model), formatArguments];
    }

    /// <summary>
    /// Returns resource in requested language (USE WITH CAUTION! is this involves some magic to get it done).
    /// </summary>
    /// <param name="target">Localizer.</param>
    /// <param name="model">Resource expression.</param>
    /// <param name="language">Language in which you would like to get resource translation back.</param>
    /// <param name="formatArguments">Message formatting arguments.</param>
    /// <returns>Html string if translation is found for resource in requested language.</returns>
    public static LocalizedHtmlString GetStringByCulture<T>(
        this IHtmlLocalizer<T> target,
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

        if (target is ICultureAwareHtmlLocalizer cultureAwareLocalizer)
        {
            return cultureAwareLocalizer.ChangeLanguage(language)[GetMemberName(target, model), formatArguments];
        }

        return null;
    }

    private static string GetMemberName(IHtmlLocalizer target, LambdaExpression model)
    {
        if (target is ILocalizationServicesAccessor accessor)
        {
            return accessor.ExpressionHelper.GetFullMemberName(model);
        }

        return string.Empty;
    }
}

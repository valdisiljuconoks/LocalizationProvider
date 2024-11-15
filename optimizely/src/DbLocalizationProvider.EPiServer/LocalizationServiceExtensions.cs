// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Globalization;
using System.Linq.Expressions;
using DbLocalizationProvider.Internal;
using DbLocalizationProvider.Queries;
using DbLocalizationProvider.Sync;
using EPiServer.Framework.Localization;
using EPiServer.ServiceLocation;
using Microsoft.Extensions.Options;

namespace DbLocalizationProvider.EPiServer;

/// <summary>
/// None reads these docs anyways..
/// </summary>
public static class LocalizationServiceExtensions
{
    /// <summary>
    /// If you want to use strongly-typed API to work with translations in Optimizely context - this method might come handy.
    /// </summary>
    /// <param name="service">Optimizely localization service.</param>
    /// <param name="resource">Expression to the resource.</param>
    /// <param name="formatArguments">If you need some formatting afterwards - pass in parameters here.</param>
    /// <returns>Translation for the resource for current language; if this fails - <c>null</c> obviously.</returns>
    public static string GetString(this LocalizationService service, Expression<Func<object>> resource, params object[] formatArguments)
    {
        return GetStringByCulture(service,
            resource,
            TryGetQueryExecutor()?.Execute(new GetCurrentUICulture.Query()) ?? CultureInfo.CurrentUICulture,
            formatArguments);
    }

    /// <summary>
    /// If you want to use strongly-typed API to work with translations in Optimizely context - this method might come handy.
    /// </summary>
    /// <param name="service">Optimizely localization service.</param>
    /// <param name="resource">Expression to the resource.</param>
    /// <param name="culture">If you need translation for other language as current one.</param>
    /// <param name="formatArguments">If you need some formatting afterwards - pass in parameters here.</param>
    /// <returns>Translation for the resource for given language; if this fails - <c>null</c> obviously.</returns>
    public static string GetStringByCulture(this LocalizationService service,
        Expression<Func<object>> resource,
        CultureInfo culture,
        params object[] formatArguments)
    {
        var helper = TryGetExpressionHelper();
        var resourceKey = helper.GetFullMemberName(resource);

        return GetStringByCulture(service, resourceKey, culture, formatArguments);
    }

    /// <summary>
    /// If you want to use strongly-typed API to work with translations in Optimizely context - this method might come handy.
    /// </summary>
    /// <param name="service">Optimizely localization service.</param>
    /// <param name="resourceKey">Key of the resource.</param>
    /// <param name="culture">If you need translation for other language as current one.</param>
    /// <param name="formatArguments">If you need some formatting afterwards - pass in parameters here.</param>
    /// <returns>Translation for the resource for given language; if this fails - <c>null</c> obviously.</returns>
    public static string GetStringByCulture(this LocalizationService service,
        string resourceKey,
        CultureInfo culture,
        params object[] formatArguments)
    {
        var translation = service.GetStringByCulture(resourceKey, culture);

        return string.IsNullOrEmpty(translation) ? translation : LocalizationProvider.Format(translation, formatArguments);
    }

    private static ExpressionHelper TryGetExpressionHelper()
    {
        return ServiceLocator.Current.TryGetExistingInstance<ExpressionHelper>(out var helper)
            ? helper
            : new ExpressionHelper(
                new ResourceKeyBuilder(
                    new ScanState(),
                    new OptionsWrapper<ConfigurationContext>(new ConfigurationContext())));
    }

    private static IQueryExecutor TryGetQueryExecutor()
    {
        return ServiceLocator.Current.TryGetExistingInstance<IQueryExecutor>(out var queryExecutor) ? queryExecutor : default;
    }
}

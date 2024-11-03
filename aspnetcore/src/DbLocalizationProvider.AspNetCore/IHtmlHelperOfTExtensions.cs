// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using DbLocalizationProvider.AspNetCore.ClientsideProvider;
using DbLocalizationProvider.Internal;
using DbLocalizationProvider.Queries;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using AppContext = DbLocalizationProvider.AspNetCore.ClientsideProvider.AppContext;

namespace DbLocalizationProvider.AspNetCore;

public static class IHtmlHelperOfTExtensions
{
    public static IHtmlContent Translate(
        this IHtmlHelper htmlHelper,
        Expression<Func<object>> expression,
        params object[] formatArguments)
    {
        return TranslateByCulture(htmlHelper, expression, GetCurrentUICulture(htmlHelper), formatArguments);
    }

    public static IHtmlContent Translate(
        this IHtmlHelper htmlHelper,
        Enum target,
        params object[] formatArguments)
    {
        return TranslateByCulture(htmlHelper, target, GetCurrentUICulture(htmlHelper), formatArguments);
    }

    public static IHtmlContent Translate(
        this IHtmlHelper htmlHelper,
        Expression<Func<object>> expression,
        Type customAttribute,
        params object[] formatArguments)
    {
        return TranslateByCulture(htmlHelper, expression, customAttribute, GetCurrentUICulture(htmlHelper), formatArguments);
    }

    public static IHtmlContent TranslateByCulture(
        this IHtmlHelper htmlHelper,
        Enum target,
        CultureInfo language,
        params object[] formatArguments)
    {
        return new HtmlString(GetLocalizationProvider(htmlHelper).TranslateByCulture(target, language, formatArguments));
    }

    public static IHtmlContent TranslateByCulture(
        this IHtmlHelper htmlHelper,
        Expression<Func<object>> expression,
        Type customAttribute,
        CultureInfo language,
        params object[] formatArguments)
    {
        if (htmlHelper == null)
        {
            throw new ArgumentNullException(nameof(htmlHelper));
        }

        if (expression == null)
        {
            throw new ArgumentNullException(nameof(expression));
        }

        if (!typeof(Attribute).IsAssignableFrom(customAttribute))
        {
            throw new ArgumentException($"Given type `{customAttribute.FullName}` is not of type `System.Attribute`");
        }

        if (language == null)
        {
            throw new ArgumentNullException(nameof(language));
        }

        var resourceKey =
            GetResourceKeyBuilder(htmlHelper)
                .BuildResourceKey(GetExpressionHelper(htmlHelper).GetFullMemberName(expression), customAttribute);

        return new HtmlString(GetLocalizationProvider(htmlHelper).GetStringByCulture(resourceKey, language, formatArguments));
    }

    public static IHtmlContent TranslateByCulture(
        this IHtmlHelper htmlHelper,
        Expression<Func<object>> expression,
        CultureInfo language,
        params object[] formatArguments)
    {
        if (htmlHelper == null)
        {
            throw new ArgumentNullException(nameof(htmlHelper));
        }

        if (expression == null)
        {
            throw new ArgumentNullException(nameof(expression));
        }

        if (language == null)
        {
            throw new ArgumentNullException(nameof(language));
        }

        return new HtmlString(GetLocalizationProvider(htmlHelper).GetStringByCulture(expression, language, formatArguments));
    }

    public static IHtmlContent TranslateFor<TModel, TResult>(
        this IHtmlHelper<TModel> htmlHelper,
        Expression<Func<TModel, TResult>> expression,
        params object[] formatArguments)
    {
        return TranslateForByCulture(htmlHelper, expression, GetCurrentUICulture(htmlHelper), formatArguments);
    }

    public static IHtmlContent TranslateFor<TModel, TResult>(
        this IHtmlHelper<TModel> htmlHelper,
        Expression<Func<TModel, TResult>> expression,
        Type customAttribute,
        params object[] formatArguments)
    {
        return TranslateForByCulture(htmlHelper, expression, customAttribute, GetCurrentUICulture(htmlHelper), formatArguments);
    }

    public static IHtmlContent TranslateForByCulture<TModel, TResult>(
        this IHtmlHelper<TModel> htmlHelper,
        Expression<Func<TModel, TResult>> expression,
        CultureInfo language,
        params object[] formatArguments)
    {
        if (htmlHelper == null)
        {
            throw new ArgumentNullException(nameof(htmlHelper));
        }

        if (expression == null)
        {
            throw new ArgumentNullException(nameof(expression));
        }

        if (language == null)
        {
            throw new ArgumentNullException(nameof(language));
        }

        return new HtmlString(
            GetLocalizationProvider(htmlHelper)
                .GetStringByCulture(GetExpressionHelper(htmlHelper).GetFullMemberName(expression),
                                    language,
                                    formatArguments));
    }

    public static IHtmlContent TranslateForByCulture<TModel, TResult>(
        this IHtmlHelper<TModel> htmlHelper,
        Expression<Func<TModel, TResult>> expression,
        Type customAttribute,
        CultureInfo language,
        params object[] formatArguments)
    {
        if (htmlHelper == null)
        {
            throw new ArgumentNullException(nameof(htmlHelper));
        }

        if (expression == null)
        {
            throw new ArgumentNullException(nameof(expression));
        }

        if (!typeof(Attribute).IsAssignableFrom(customAttribute))
        {
            throw new ArgumentException($"Given type `{customAttribute.FullName}` is not of type `System.Attribute`");
        }

        if (language == null)
        {
            throw new ArgumentNullException(nameof(language));
        }

        var resourceKey =
            GetResourceKeyBuilder(htmlHelper)
                .BuildResourceKey(GetExpressionHelper(htmlHelper).GetFullMemberName(expression), customAttribute);

        return new HtmlString(GetLocalizationProvider(htmlHelper).GetStringByCulture(resourceKey, language, formatArguments));
    }

    public static IHtmlContent DescriptionFor<TModel, TValue>(
        this IHtmlHelper<TModel> htmlHelper,
        Expression<Func<TModel, TValue>> expression,
        params object[] formatArguments)
    {
        return DescriptionByCultureFor(htmlHelper, expression, GetCurrentUICulture(htmlHelper), formatArguments);
    }

    public static IHtmlContent DescriptionByCultureFor<TModel, TValue>(
        this IHtmlHelper<TModel> htmlHelper,
        Expression<Func<TModel, TValue>> expression,
        CultureInfo language,
        params object[] formatArguments)
    {
        if (htmlHelper == null)
        {
            throw new ArgumentNullException(nameof(htmlHelper));
        }

        if (expression == null)
        {
            throw new ArgumentNullException(nameof(expression));
        }

        if (language == null)
        {
            throw new ArgumentNullException(nameof(language));
        }

        return new HtmlString(GetLocalizationProvider(htmlHelper)
                                  .GetStringByCulture(
                                      GetExpressionHelper(htmlHelper).GetFullMemberName(expression) + "-Description",
                                      language,
                                      formatArguments));
    }

    public static IHtmlContent GetTranslations<TModel>(
        this IHtmlHelper<TModel> helper,
        Type containerType,
        string language = null,
        string alias = null,
        bool debug = false,
        bool camelCase = false)
    {
        return GetTranslations((IHtmlHelper)helper, containerType, language, alias, debug, camelCase);
    }

    public static IHtmlContent GetTranslations<TModel>(
        this IHtmlHelper<TModel> htmlHelper,
        Expression<Func<object>> model,
        string language = null,
        string alias = null,
        bool debug = false,
        bool camelCase = false)
    {
        return GenerateScriptTag(language, alias, debug, GetExpressionHelper(htmlHelper).GetFullMemberName(model), camelCase);
    }

    public static IHtmlContent GetTranslations(
        this IHtmlHelper htmlHelper,
        Type containerType,
        string language = null,
        string alias = null,
        bool debug = false,
        bool camelCase = false)
    {
        if (containerType == null)
        {
            throw new ArgumentNullException(nameof(containerType));
        }

        return GenerateScriptTag(language,
                                 alias,
                                 debug,
                                 GetResourceKeyBuilder(htmlHelper).BuildResourceKey(containerType),
                                 camelCase);
    }

    public static IHtmlContent GetTranslations(
        this IHtmlHelper htmlHelper,
        Expression<Func<object>> model,
        string language = null,
        string alias = null,
        bool debug = false,
        bool camelCase = false)
    {
        if (model == null)
        {
            throw new ArgumentNullException(nameof(model));
        }

        return GenerateScriptTag(language, alias, debug, GetExpressionHelper(htmlHelper).GetFullMemberName(model), camelCase);
    }

    private static IHtmlContent GenerateScriptTag(
        string language,
        string alias,
        bool debug,
        string resourceKey,
        bool camelCase)
    {
        // if 1st request
        var mergeScript = string.Empty;
        var httpItems = AppContext.Service.HttpContext.Items;

        if (httpItems["__DbLocalizationProvider_JsHandler_1stRequest"] == null)
        {
            httpItems.Add("__DbLocalizationProvider_JsHandler_1stRequest", false);
            mergeScript =
                $"<script src=\"{ClientsideConfigurationContext.RootPath}/{ClientsideConfigurationContext.DeepMergeScriptName}\"></script>";
        }

        var url = $"{ClientsideConfigurationContext.RootPath}/{resourceKey.Replace("+", "---")}";
        var parameters = new Dictionary<string, string>();

        if (!string.IsNullOrEmpty(language))
        {
            parameters.Add("lang", language);
        }

        if (!string.IsNullOrEmpty(alias))
        {
            parameters.Add("alias", alias);
        }

        if (debug)
        {
            parameters.Add("debug", "true");
        }

        if (camelCase)
        {
            parameters.Add("camel", "true");
        }

        if (parameters.Any())
        {
            url += "?" + ToQueryString(parameters);
        }

        return new HtmlString($"{mergeScript}<script src=\"{url}\"></script>");
    }

    private static string ToQueryString(Dictionary<string, string> parameters)
    {
        if (parameters == null)
        {
            throw new ArgumentNullException(nameof(parameters));
        }

        if (!parameters.Any())
        {
            return string.Empty;
        }

        return string.Join("&", parameters.Select(kv => $"{kv.Key}={kv.Value}"));
    }


    private static ResourceKeyBuilder GetResourceKeyBuilder(IHtmlHelper htmlHelper)
    {
        return GetService<ResourceKeyBuilder>(htmlHelper);
    }

    private static ExpressionHelper GetExpressionHelper(IHtmlHelper htmlHelper)
    {
        return GetService<ExpressionHelper>(htmlHelper);
    }

    private static ILocalizationProvider GetLocalizationProvider(IHtmlHelper htmlHelper)
    {
        return GetService<ILocalizationProvider>(htmlHelper);
    }

    private static T GetService<T>(IHtmlHelper htmlHelper)
    {
        return htmlHelper.ViewContext.HttpContext.RequestServices.GetService<T>();
    }

    private static CultureInfo GetCurrentUICulture(IHtmlHelper htmlHelper)
    {
        return GetService<IQueryExecutor>(htmlHelper).Execute(new GetCurrentUICulture.Query());
    }
}

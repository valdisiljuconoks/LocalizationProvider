// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Internal;
using DbLocalizationProvider.Queries;
using DbLocalizationProvider.Sync;
using Microsoft.Extensions.Options;

namespace DbLocalizationProvider;

/// <summary>
/// Main class to use when resource translation is needed.
/// </summary>
public class LocalizationProvider : ILocalizationProvider
{
    private readonly ExpressionHelper _expressionHelper;
    private readonly FallbackLanguagesCollection _fallbackCollection;
    private readonly ResourceKeyBuilder _keyBuilder;
    private readonly ReflectionConverter _reflectionConverter;
    private readonly IResourceService _resourceService;

    /// <summary>
    /// Creates new localization provider with all the required settings and services injected.
    /// </summary>
    /// <param name="keyBuilder">
    /// Key builder (with help of <paramref name="expressionHelper" /> this dependency will help to translate from lambda
    /// expression to resource key as string).
    /// </param>
    /// <param name="expressionHelper">Can walk lambda expressions and return string representation of the expression.</param>
    /// <param name="context">ConfigurationContext (fallback language collection will be used from this).</param>
    /// <param name="queryExecutor">Small utility robot to help with queries.</param>
    /// <param name="scanState">Scanner state.</param>
    public LocalizationProvider(
        ResourceKeyBuilder keyBuilder,
        ExpressionHelper expressionHelper,
        IOptions<ConfigurationContext> context,
        IResourceService resourceService)
    {
        _keyBuilder = keyBuilder;
        _expressionHelper = expressionHelper;
        _resourceService = resourceService;
        _fallbackCollection = context.Value._fallbackCollection;
    }

    /// <summary>
    /// Gets translation for the resource with specific key.
    /// </summary>
    /// <param name="resourceKey">Key of the resource to look translation for.</param>
    /// <returns>Translation for the resource with specific key.</returns>
    /// <remarks><see cref="CultureInfo.CurrentUICulture" /> is used as language.</remarks>
    public virtual string GetString(string resourceKey)
    {
        return GetString(resourceKey, _resourceService.GetCurrentCulture());
    }

    /// <summary>
    /// Gets translation for the resource with specific key.
    /// </summary>
    /// <param name="resourceKey">Key of the resource to look translation for.</param>
    /// <param name="culture">
    /// If you want to get translation for other language as <see cref="CultureInfo.CurrentUICulture" />,
    /// then specify that language here.
    /// </param>
    /// <returns>Translation for the resource with specific key.</returns>
    public virtual string GetString(string resourceKey, CultureInfo culture)
    {
        return GetStringByCulture(resourceKey, culture);
    }

    /// <summary>
    /// Gets translation for the resource (reference to the resource is specified as lambda expression).
    /// </summary>
    /// <param name="resource">Lambda expression for the resource.</param>
    /// <param name="formatArguments">
    /// If you have placeholders in translation to replace to - use this argument to specify those.
    /// </param>
    /// <returns>Translation for the resource with specific key.</returns>
    /// <remarks><see cref="CultureInfo.CurrentUICulture" /> is used as language.</remarks>
    public virtual string GetString(Expression<Func<object>> resource, params object[] formatArguments)
    {
        return GetStringByCulture(resource, _resourceService.GetCurrentCulture(), formatArguments);
    }

    /// <summary>
    /// Gets translation for the resource (reference to the resource is specified as lambda expression).
    /// </summary>
    /// <param name="resource">Lambda expression for the resource.</param>
    /// <param name="culture">
    /// If you want to get translation for other language as <see cref="CultureInfo.CurrentUICulture" />,
    /// then specific that language here.
    /// </param>
    /// <param name="formatArguments">
    /// If you have placeholders in translation to replace to - use this argument to specify those.
    /// </param>
    /// <returns>Translation for the resource with specific key.</returns>
    public virtual string GetString(Expression<Func<object>> resource, CultureInfo culture, params object[] formatArguments)
    {
        return GetStringByCulture(resource, culture, formatArguments);
    }

    /// <summary>
    /// Gets key and translations for the specified culture.
    /// </summary>
    /// <param name="culture">
    /// If you want to get translation for other language as <see cref="CultureInfo.CurrentUICulture" />,
    /// then specify that language here.
    /// </param>
    /// <returns>Translation for the resource with specific key.</returns>
    public virtual Dictionary<string, string?> GetStringsByCulture(CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(culture);

        var localizationResources = _resourceService.GetAllResources();
        var translationDictionary = localizationResources.ToDictionary(res => res.Key, res => res.Value.Translations.ByLanguage(culture, true));

        return translationDictionary;
    }

    /// <summary>
    /// Gets translation for the resource (reference to the resource is specified as lambda expression).
    /// </summary>
    /// <param name="resource">Lambda expression for the resource.</param>
    /// <param name="culture">
    /// If you want to get translation for other language as <see cref="CultureInfo.CurrentUICulture" />, then specific
    /// that language here.
    /// </param>
    /// <param name="formatArguments">
    /// If you have placeholders in translation to replace to - use this argument to specify those.
    /// </param>
    /// <returns>Translation for the resource with specific key.</returns>
    public virtual string GetStringByCulture(
        Expression<Func<object>> resource,
        CultureInfo culture,
        params object[] formatArguments)
    {
        if (resource == null)
        {
            throw new ArgumentNullException(nameof(resource));
        }

        var resourceKey = _expressionHelper.GetFullMemberName(resource);

        return GetStringByCulture(resourceKey, culture, formatArguments);
    }

    /// <summary>
    /// Gets translation for the resource with specific key.
    /// </summary>
    /// <param name="resourceKey">Key of the resource to look translation for.</param>
    /// <param name="culture">
    /// If you want to get translation for other language as <see cref="CultureInfo.CurrentUICulture" />,
    /// then specify that language here.
    /// </param>
    /// <param name="formatArguments">
    /// If you have placeholders in translation to replace to - use this argument to specify
    /// those.
    /// </param>
    /// <returns>Translation for the resource with specific key.</returns>
    public virtual string GetStringByCulture(string resourceKey, CultureInfo culture, params object[] formatArguments)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(resourceKey);
        ArgumentNullException.ThrowIfNull(culture);

        var resourceValue = _resourceService.GetTranslation(resourceKey, culture);

        try
        {
            return Format(resourceValue, formatArguments);
        }
        catch (Exception)
        {
            // TODO: log
        }

        return resourceValue;
    }

    /// <summary>
    /// Give a type to this method and it will return instance of the type but translated
    /// </summary>
    /// <typeparam name="T">Type of the target class you want to translate</typeparam>
    /// <returns>Translated class based on <see cref="CultureInfo.CurrentUICulture" /> language</returns>
    public T Translate<T>()
    {
        return Translate<T>(_resourceService.GetCurrentCulture());
    }

    /// <summary>
    /// Give a type to this method, and it will return instance of the type but translated
    /// </summary>
    /// <typeparam name="T">Type of the target class you want to translate</typeparam>
    /// <param name="language">Language to use during translation</param>
    /// <returns>Translated class</returns>
    public T Translate<T>(CultureInfo language)
    {
        return _reflectionConverter.Convert<T>(language.Name, _fallbackCollection);
    }

    /// <summary>
    /// Translates the specified enum with some formatting arguments (if needed).
    /// </summary>
    /// <param name="target">The enum to translate.</param>
    /// <param name="formatArguments">The format arguments.</param>
    /// <returns>Translated enum values</returns>
    public string Translate(Enum target, params object[] formatArguments)
    {
        return TranslateByCulture(target, _resourceService.GetCurrentCulture(), formatArguments);
    }

    /// <summary>
    /// Translates the specified enum with some formatting arguments (if needed).
    /// </summary>
    /// <param name="target">The enum to translate.</param>
    /// <param name="culture">The culture.</param>
    /// <param name="formatArguments">The format arguments.</param>
    /// <returns>Translated enum values</returns>
    /// <exception cref="ArgumentNullException">
    /// target
    /// or
    /// culture
    /// </exception>
    public string TranslateByCulture(Enum target, CultureInfo culture, params object[] formatArguments)
    {
        if (target == null)
        {
            throw new ArgumentNullException(nameof(target));
        }

        if (culture == null)
        {
            throw new ArgumentNullException(nameof(culture));
        }

        var resourceKey = _keyBuilder.BuildResourceKey(target.GetType(), target.ToString());

        return GetStringByCulture(resourceKey, culture, formatArguments);
    }

    /// <inheritdoc />
    public string GetStringWithInvariantFallback(Expression<Func<object>> resource, params object[] formatArguments)
    {
        if (resource == null)
        {
            throw new ArgumentNullException(nameof(resource));
        }

        var resourceKey = _expressionHelper.GetFullMemberName(resource);
        var resourceValue = _resourceService.GetTranslation(resourceKey, _resourceService.GetCurrentCulture(), true);

        return Format(resourceValue, formatArguments);
    }

    /// <summary>
    /// Gets translation for the resource (reference to the resource is specified as lambda expression).
    /// </summary>
    /// <param name="resource">Lambda expression for the resource.</param>
    /// <param name="attribute">
    /// Type of the custom attribute (registered in
    /// <see cref="ConfigurationContext.CustomAttributes" /> collection).
    /// </param>
    /// <param name="formatArguments">
    /// If you have placeholders in translation to replace to - use this argument to specify
    /// those.
    /// </param>
    /// <returns>Translation for the resource with specific key.</returns>
    /// <remarks><see cref="CultureInfo.CurrentUICulture" /> is used as language.</remarks>
    public virtual string GetString(Expression<Func<object>> resource, Type attribute, params object[] formatArguments)
    {
        return GetStringByCulture(resource, attribute, _resourceService.GetCurrentCulture(), formatArguments);
    }

    /// <summary>
    /// Gets translation for the resource (reference to the resource is specified as lambda expression).
    /// </summary>
    /// <param name="resource">Lambda expression for the resource.</param>
    /// <param name="attribute">
    /// Type of the custom attribute (registered in
    /// <see cref="ConfigurationContext.CustomAttributes" /> collection).
    /// </param>
    /// <param name="culture">
    /// If you want to get translation for other language as <see cref="CultureInfo.CurrentUICulture" />,
    /// then specific that language here.
    /// </param>
    /// <param name="formatArguments">
    /// If you have placeholders in translation to replace to - use this argument to specify
    /// those.
    /// </param>
    /// <returns>Translation for the resource with specific key in language specified  in <paramref name="culture" />.</returns>
    public virtual string GetStringByCulture(
        Expression<Func<object>> resource,
        Type attribute,
        CultureInfo culture,
        params object[] formatArguments)
    {
        if (resource == null)
        {
            throw new ArgumentNullException(nameof(resource));
        }

        var resourceKey = _expressionHelper.GetFullMemberName(resource);
        resourceKey = _keyBuilder.BuildResourceKey(resourceKey, attribute);

        return GetStringByCulture(resourceKey, culture, formatArguments);
    }

    /// <summary>
    /// Converts a localized resource dictionary to a translated dictionary based on the specified type.
    /// </summary>
    /// <typeparam name="T">The type to retrieve localized resources for.</typeparam>
    /// <returns>A dictionary containing the localized resources translated to the current culture.</returns>
    /// <exception cref="ArgumentException">Thrown when the object does not have a LocalizedResourceAttribute.</exception>
    public IDictionary<string, string> ToDictionary<T>()
    {
        return ToDictionary(typeof(T));
    }

    /// <summary>
    /// Converts a localized resource dictionary to a translated dictionary based on the specified type.
    /// </summary>
    /// <typeparam name="T">The type to retrieve localized resources for.</typeparam>
    /// <param name="culture">Culture to get translations in.</param>
    /// <returns>A dictionary containing the localized resources translated to the current culture.</returns>
    /// <exception cref="ArgumentException">Thrown when the object does not have a LocalizedResourceAttribute.</exception>
    public IDictionary<string, string> ToDictionary<T>(CultureInfo culture)
    {
        return ToDictionary(typeof(T), culture);
    }

    /// <summary>
    /// Converts a localized resource dictionary to a translated dictionary based on the specified type.
    /// </summary>
    /// <param name="type">The type to retrieve localized resources for.</param>
    /// <returns>A dictionary containing the localized resources translated to the current culture.</returns>
    /// <exception cref="ArgumentException">Thrown when the object does not have a LocalizedResourceAttribute.</exception>
    public IDictionary<string, string> ToDictionary(Type type)
    {
        return ToDictionary(type, _resourceService.GetCurrentCulture());
    }

    /// <summary>
    /// Converts a localized resource dictionary to a translated dictionary based on the specified type.
    /// </summary>
    /// <param name="type">The type to retrieve localized resources for.</param>
    /// <param name = "culture" > Culture to get translations in.</param>
    /// <returns>A dictionary containing the localized resources translated to the current culture.</returns>
    /// <exception cref="ArgumentException">Thrown when the object does not have a LocalizedResourceAttribute.</exception>
    public IDictionary<string, string> ToDictionary(Type type, CultureInfo culture)
    {
        _ = Attribute.GetCustomAttribute(type, typeof(LocalizedResourceAttribute)) ?? throw new ArgumentException($"Object needs to have a {nameof(LocalizedResourceAttribute)} to be converted");

        return GetLocalizedResourceTranslations(type, culture)
            .ToDictionary(k => k.Key, v => v.Value);
    }

    internal IEnumerable<KeyValuePair<string, string>> GetLocalizedResourceTranslations(Type type, CultureInfo culture)
    {
        foreach (var property in type.GetProperties())
        {
            if (property.IsHidden())
            {
                continue;
            }

            var resourceKey = _keyBuilder.BuildResourceKey(type, property.Name);

            yield return new(property.Name, GetString(resourceKey, culture));
        }
    }

    internal static string Format(string message, params object[] formatArguments)
    {
        if (formatArguments == null || formatArguments.Length == 0)
        {
            return message;
        }

        // check if first element is not scalar - format with named placeholders
        var first = formatArguments.First();

        return !first.GetType().IsSimpleType()
            ? FormatWithAnonymousObject(message, first)
            : string.Format(message, formatArguments);
    }

    private static string FormatWithAnonymousObject(string message, object model)
    {
        var type = model.GetType();
        if (type == typeof(string))
        {
            return string.Format(message, model);
        }

        const char Prefix = '{';
        const char Postfix = '}';
        if (!message.Contains(Prefix) || !message.Contains(Postfix))
        {
            return message;
        }

        var properties = type.GetProperties();

        ReadOnlySpan<char> tmp = message;

        // The rest of the method is based on https://stackoverflow.com/a/74391485/11963 (Licensed under CC BY-SA 4.0)
        // we store the occurrences in the queue while calculating the length of the final string
        // so we don't have to search for them the 2nd time later
        var occurrences = new Queue<(int at, int task)>();
        var offset = 0;
        var resultLength = tmp.Length;

        int prefixIndex;
        while ((prefixIndex = tmp.IndexOf(Prefix)) != -1)
        {
            (int at, int task) next = (prefixIndex, -1);
            for (var i = 0; i < properties.Length; i++)
            {
                // we expect the postfix to be at this place
                var postfixIndex = prefixIndex + properties[i].Name.Length + 1;
                if (tmp.Length > postfixIndex // check that we don't cross the bounds
                    && tmp[postfixIndex] == Postfix // check that the postfix IS were we expect it to be
                    && tmp.Slice(prefixIndex + 1, postfixIndex - prefixIndex - 1).SequenceEqual(properties[i].Name)) // compare all the characters in between the delimiters
                {
                    next.task = i;
                    break;
                }
            }

            if (next.task == -1)
            {
                // this delimiter character is just part of the string, so skip it
                tmp = tmp[(prefixIndex + 1)..];
                offset += prefixIndex + 1;
                continue;
            }

            var newStart = next.at + properties[next.task].Name.Length + 2;
            tmp = tmp[newStart..];

            occurrences.Enqueue((next.at + offset, next.task));
            offset += newStart;

            resultLength += (properties[next.task].GetValue(model)?.ToString()?.Length ?? 0) - properties[next.task].Name.Length - 2;
        }

        var result = string.Create(resultLength, (message, model, properties, occurrences), (chars, state) =>
        {
            var message = state.message;
            var model = state.model;
            var replaceTasks = state.properties;
            var occurrences = state.occurrences;

            var position = 0;

            ReadOnlySpan<char> origin = message;
            var lastStart = 0;

            while (occurrences.Count != 0)
            {
                var next = occurrences.Dequeue();

                var value = replaceTasks[next.task].GetValue(model)?.ToString();
                if (value is null)
                {
                    continue;
                }

                origin[lastStart..next.at].CopyTo(chars[position..]);
                value.CopyTo(chars[(position + next.at - lastStart)..]);
                position += next.at - lastStart + value.Length;
                lastStart = next.at + replaceTasks[next.task].Name.Length + 2;
            }

            origin[lastStart..].CopyTo(chars[position..]);
        });

        return result;
    }
}

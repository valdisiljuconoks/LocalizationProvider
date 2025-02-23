// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Queries;
using DbLocalizationProvider.Sync;
using Newtonsoft.Json.Linq;

namespace DbLocalizationProvider.Json;

/// <summary>
/// Class used in various clientside localization resource provider operations
/// </summary>
public class JsonConverter
{
    private readonly IQueryExecutor _queryExecutor;
    private readonly ScanState _state;

    /// <summary>
    /// Creates new instance of the JSON converter.
    /// </summary>
    /// <param name="queryExecutor">small helper guy to execute queries.</param>
    /// <param name="state">Scanner state.</param>
    public JsonConverter(IQueryExecutor queryExecutor, ScanState state)
    {
        _queryExecutor = queryExecutor;
        _state = state;
    }

    /// <summary>
    /// Gets the JSON object from given resource class.
    /// </summary>
    /// <param name="resourceClassName">Name of the resource class.</param>
    /// <param name="fallbackCollection">List of fallback languages collection.</param>
    /// <param name="camelCase">if set to <c>true</c> JSON properties will be in camelCase; otherwise PascalCase is used.</param>
    /// <returns>JSON object that represents resource</returns>
    public JObject GetJson(string resourceClassName, FallbackLanguagesCollection fallbackCollection, bool camelCase = false)
    {
        return GetJson(resourceClassName,
                       _queryExecutor.Execute(new GetCurrentUICulture.Query()).Name,
                       fallbackCollection,
                       camelCase);
    }

    /// <summary>
    /// Gets the JSON object from given resource class.
    /// </summary>
    /// <param name="resourceClassName">Name of the resource class.</param>
    /// <param name="languageName">Name of the language.</param>
    /// <param name="fallbackCollection">List of fallback languages collection.</param>
    /// <param name="camelCase">if set to <c>true</c> JSON properties will be in camelCase; otherwise PascalCase is used.</param>
    /// <returns>JSON object that represents resource</returns>
    public JObject GetJson(
        string resourceClassName,
        string? languageName,
        FallbackLanguagesCollection fallbackCollection,
        bool camelCase = false)
    {
        var allResources = _queryExecutor.Execute(new GetAllResources.Query());

        var filteredResources = 
            allResources
            .Where(kv => kv.Key.StartsWith(resourceClassName, StringComparison.Ordinal))
            .ToList();

        return Convert(
            filteredResources,
            allResources,
            languageName,
            fallbackCollection,
            camelCase);
    }

    internal JObject Convert(
        List<KeyValuePair<string, LocalizationResource>> resources,
        string? language,
        CultureInfo fallbackCulture,
        bool camelCase)
    {
        return Convert(resources,
                       null,
                       language,
                       new FallbackLanguagesCollection(fallbackCulture),
                       camelCase);
    }

    internal JObject Convert(
        List<KeyValuePair<string, LocalizationResource>> resources,
        Dictionary<string, LocalizationResource>? allResources,
        string? language,
        FallbackLanguagesCollection fallbackCollection,
        bool camelCase)
    {
        var result = new JObject();

        foreach (var kv in resources)
        {
            // we need to process key names and supported nested classes with "+" symbols in keys
            // so we replace those with dots to have proper object nesting on client side
            var key = kv.Key.Replace('+', '.');
            if (!key.Contains('.'))
            {
                continue;
            }

            var segments = key.Split('.');
            if (segments.Length > 0 && camelCase)
            {
                segments = [.. segments.Select(CamelCase)];
            }

            // let's try to look for translation explicitly in requested language
            // if there is no translation in given language -> worth to look in fallback culture *and* invariant (if configured to do so)
            var translation = kv.Value.Translations.GetValueWithFallback(
                language,
                fallbackCollection.GetFallbackLanguages(language));

            // check if resource is redirected to another resource
            // if so - we must use target resource translation instead
            if (_state.UseResourceAttributeCache.TryGetValue(key, out var targetResourceKey) 
                && (allResources?.TryGetValue(targetResourceKey, out var foundResource) ?? false))
            {
                translation = foundResource.Translations.GetValueWithFallback(
                    language,
                    fallbackCollection.GetFallbackLanguages(language));
            }

            // there is nothing at the other end - so we should not generate key at all
            if (translation == null)
            {
                continue;
            }

            Aggregate(result,
                      segments,
                      (e, segment) =>
                      {
                          e[segment] ??= new JObject();
                          return (e[segment] as JObject)!;
                      },
                      (o, s) => { o[s] = translation; });
        }

        return result;
    }

    private static void Aggregate(
        JObject seed,
        string[]? segments,
        Func<JObject, string, JObject> act,
        Action<JObject, string> last)
    {
        if (segments == null || segments.Length == 0)
        {
            return;
        }

        var lastElement = segments[^1];
        var seqWithNoLast = segments.Take(..^1);
        var s = seqWithNoLast.Aggregate(seed, act);

        last(s, lastElement);
    }

    private static string CamelCase(string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        
        if (text.Length == 0 || char.IsLower(text, 0))
        {
            return text;
        }

        return string.Create(text.Length, text, (chars, state) =>
        {
            state.AsSpan().CopyTo(chars);
            chars[0] = char.ToLower(chars[0]);
        });
    }
}

// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Queries;

namespace DbLocalizationProvider.Sync;

/// <summary>
/// Helper class to be more DI friendly with extension methods.
/// </summary>
public class DiscoveredTranslationBuilder
{
    private readonly IQueryExecutor _executor;

    /// <summary>
    /// Creates new instance of the class.
    /// </summary>
    /// <param name="executor">The executor of queries.</param>
    public DiscoveredTranslationBuilder(IQueryExecutor executor)
    {
        _executor = executor;
    }

    /// <summary>
    /// Creates new translation class from single found text.
    /// </summary>
    /// <param name="translation">Text of the resource translation.</param>
    /// <returns>Discovered translation (as list for easier other API support)</returns>
    public List<DiscoveredTranslation> FromSingle(string translation)
    {
        var defaultCulture = _executor.Execute(new DetermineDefaultCulture.Query());

        var result = new List<DiscoveredTranslation>
        {
            // invariant translation
            new(translation, CultureInfo.InvariantCulture.Name)
        };

        // register additional culture if default is not set to invariant
        if (defaultCulture != string.Empty)
        {
            result.Add(new DiscoveredTranslation(translation, defaultCulture));
        }

        return result;
    }

    /// <summary>
    /// Gets all translations.
    /// </summary>
    /// <param name="mi">The member info type to get resources from.</param>
    /// <param name="resourceKey">The resource key.</param>
    /// <param name="defaultTranslation">The default translation.</param>
    /// <returns>List of discovered resources</returns>
    /// <exception cref="DuplicateResourceTranslationsException">
    /// Duplicate translations for the same culture for following
    /// resource: `{resourceKey}`
    /// </exception>
    public ICollection<DiscoveredTranslation> GetAllTranslations(
        MemberInfo mi,
        string resourceKey,
        string defaultTranslation)
    {
        var translations = FromSingle(defaultTranslation);
        var additionalTranslations = mi.GetCustomAttributes<TranslationForCultureAttribute>().ToList();

        if (!additionalTranslations.Any())
        {
            return translations;
        }

        if (additionalTranslations.GroupBy(t => t.Culture).Any(g => g.Count() > 1))
        {
            throw new DuplicateResourceTranslationsException(
                $"Duplicate translations for the same culture for following resource: `{resourceKey}`");
        }

        additionalTranslations.ForEach(t =>
        {
            // check if specified culture in attribute is actual culture
            if (!TryGetCultureInfo(t.Culture, out _))
            {
                throw new ArgumentException($"Culture `{t.Culture}` for resource `{resourceKey}` is not supported.");
            }

            var existingTranslation = translations.FirstOrDefault(x => x.Culture == t.Culture);
            if (existingTranslation != null)
            {
                existingTranslation.Translation = t.Translation;
            }
            else
            {
                translations.Add(new DiscoveredTranslation(t.Translation, t.Culture));
            }
        });

        return translations;
    }

    private static bool TryGetCultureInfo(string? cultureCode, out CultureInfo? culture)
    {
        try
        {
            culture = CultureInfo.GetCultureInfo(cultureCode);
            return true;
        }
        catch (CultureNotFoundException) { }

        culture = null;
        return false;
    }
}

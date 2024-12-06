// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DbLocalizationProvider.Queries;
using Microsoft.Extensions.Options;

namespace DbLocalizationProvider.EPiServer;

/// <inheritdoc />
public class DatabaseLocalizationProvider : global::EPiServer.Framework.Localization.LocalizationProvider
{
    private readonly ConfigurationContext _context;
    private readonly IQueryExecutor _queryExecutor;

    /// <inheritdoc />
    public DatabaseLocalizationProvider(IOptions<ConfigurationContext> context, IQueryExecutor queryExecutor)
    {
        _context = context.Value;
        _queryExecutor = queryExecutor;
    }

    /// <inheritdoc />
    public override IEnumerable<CultureInfo> AvailableLanguages
    {
        get
        {
            return _queryExecutor
                   .Execute(new AvailableLanguages.Query())
                   .Select(al => al.CultureInfo);
        }
    }

    /// <inheritdoc />
    public override string GetString(string originalKey, string[] normalizedKey, CultureInfo culture)
    {
        // this is special case for Episerver ;)
        // https://world.episerver.com/forum/developer-forum/-Episerver-75-CMS/Thread-Container/2019/10/takes-a-lot-of-time-for-epi-cms-resources-to-load-on-dxc-service/

        return _context.ShouldLookupResource(originalKey)
            ? _queryExecutor.Execute(new GetTranslation.Query(originalKey, culture))
            : null;
    }

    /// <inheritdoc />
    public override IEnumerable<global::EPiServer.Framework.Localization.ResourceItem> GetAllStrings(
        string originalKey,
        string[] normalizedKey,
        CultureInfo culture)
    {
        // this is special case for Episerver ;)
        // https://world.episerver.com/forum/developer-forum/-Episerver-75-CMS/Thread-Container/2019/10/takes-a-lot-of-time-for-epi-cms-resources-to-load-on-dxc-service/
        if (!_context.ShouldLookupResource(originalKey))
        {
            return [];
        }

        var q = new GetAllResources.Query();
        var allResources = _queryExecutor
                           .Execute(q)
                           .Where(kv =>
                               kv.Key.StartsWith(originalKey, StringComparison.Ordinal)
                               && kv.Value.Translations != null
                               && kv.Value.Translations.Exists(t => t.Language == culture.Name))
                           .ToList();

        if (!allResources.Any())
        {
            return [];
        }

        return allResources
               .Select(r => new global::EPiServer.Framework.Localization.ResourceItem(
                   r.Key,
                   r.Value.Translations.FindByLanguage(culture)?.Value,
                   culture))
               .ToList();
    }
}

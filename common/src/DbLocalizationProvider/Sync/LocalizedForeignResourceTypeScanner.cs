// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Internal;
using DbLocalizationProvider.Refactoring;
using Microsoft.Extensions.Options;

namespace DbLocalizationProvider.Sync;

internal class LocalizedForeignResourceTypeScanner(
    ResourceKeyBuilder keyBuilder,
    OldResourceKeyBuilder oldKeyBuilder,
    ScanState state,
    IOptions<ConfigurationContext> configurationContext,
    DiscoveredTranslationBuilder translationBuilder)
    : IResourceTypeScanner
{
#pragma warning disable CS8618 // will be initialized in ShouldScan method
    private IResourceTypeScanner _actualScanner;
#pragma warning restore CS8618

    public bool ShouldScan(Type target)
    {
        if (target.BaseType == typeof(Enum))
        {
            _actualScanner = new LocalizedEnumTypeScanner(keyBuilder, translationBuilder);
        }
        else
        {
            _actualScanner =
                new LocalizedResourceTypeScanner(keyBuilder,
                                                 oldKeyBuilder,
                                                 state,
                                                 configurationContext,
                                                 translationBuilder);
        }

        return true;
    }

    public string? GetResourceKeyPrefix(Type target, string? keyPrefix = null)
    {
        return _actualScanner.GetResourceKeyPrefix(target, keyPrefix);
    }

    public ICollection<DiscoveredResource> GetClassLevelResources(Type target, string resourceKeyPrefix)
    {
        return _actualScanner.GetClassLevelResources(target, resourceKeyPrefix);
    }

    public ICollection<DiscoveredResource> GetResources(Type target, string resourceKeyPrefix)
    {
        var discoveredResources = _actualScanner.GetResources(target, resourceKeyPrefix);

        // check whether we need to scan also complex properties
        var includeComplex = configurationContext.Value.ForeignResources?.Get(target)?.IncludeComplexProperties ?? false;
        if (includeComplex)
        {
            discoveredResources.ForEach(r =>
            {
                if (!r.IsSimpleType)
                {
                    r.IncludedExplicitly = true;
                }
            });
        }

        return discoveredResources;
    }
}

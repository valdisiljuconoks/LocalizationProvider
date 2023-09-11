// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Sync.Collectors;

internal class ResourceKeyAttributeCollector : IResourceCollector
{
    private readonly ResourceKeyBuilder _keyBuilder;
    private readonly DiscoveredTranslationBuilder _translationBuilder;

    public ResourceKeyAttributeCollector(ResourceKeyBuilder keyBuilder, DiscoveredTranslationBuilder translationBuilder)
    {
        _keyBuilder = keyBuilder;
        _translationBuilder = translationBuilder;
    }

    public IEnumerable<DiscoveredResource> GetDiscoveredResources(
        Type target,
        object instance,
        MemberInfo mi,
        string translation,
        string resourceKey,
        string resourceKeyPrefix,
        bool typeKeyPrefixSpecified,
        bool isHidden,
        string typeOldName,
        string typeOldNamespace,
        Type declaringType,
        Type returnType,
        bool isSimpleType)
    {
        // check if there are [ResourceKey] attributes
        var keyAttributes = mi.GetCustomAttributes<ResourceKeyAttribute>().ToList();

        return keyAttributes.Select(attr =>
        {
            var translations = _translationBuilder.GetAllTranslations(
                mi,
                resourceKey,
                string.IsNullOrEmpty(attr.Value) ? translation : attr.Value);

            return new DiscoveredResource(
                mi,
                _keyBuilder.BuildResourceKey(typeKeyPrefixSpecified ? resourceKeyPrefix : null, attr.Key, string.Empty),
                translations,
                null,
                declaringType,
                returnType,
                true) { FromResourceKeyAttribute = true };
        });
    }
}

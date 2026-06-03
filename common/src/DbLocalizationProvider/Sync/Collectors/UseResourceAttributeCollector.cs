// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using System.Reflection;
using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Sync.Collectors;

internal class UseResourceAttributeCollector(ResourceKeyBuilder keyBuilder, ScanState state) : IResourceCollector
{
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
        // try to understand if there is resource "redirect" - [UseResource(..)]
        var resourceRef = mi.GetCustomAttribute<UseResourceAttribute>();
        if (resourceRef == null)
        {
            yield break;
        }

        state.UseResourceAttributeCache.TryAdd(
            resourceKey,
            keyBuilder.BuildResourceKey(resourceRef.TargetContainer, resourceRef.PropertyName));

        yield return new DiscoveredResource(
            mi,
            resourceKey,
            new List<DiscoveredTranslation>(),
            mi.Name,
            declaringType,
            returnType,
            isSimpleType,
            true)
        {
            TypeName = target.Name,
            TypeNamespace = target.Namespace,
            TypeOldNamespace = typeOldNamespace
        };
    }
}

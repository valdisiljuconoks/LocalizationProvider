// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Refactoring;
using Microsoft.Extensions.Options;

namespace DbLocalizationProvider.Sync.Collectors;

internal class CustomAttributeCollector : IResourceCollector
{
    private readonly IOptions<ConfigurationContext> _configurationContext;
    private readonly ResourceKeyBuilder _keyBuilder;
    private readonly OldResourceKeyBuilder _oldKeyBuilder;
    private readonly DiscoveredTranslationBuilder _translationBuilder;

    public CustomAttributeCollector(
        ResourceKeyBuilder keyBuilder,
        OldResourceKeyBuilder oldKeyBuilder,
        IOptions<ConfigurationContext> configurationContext,
        DiscoveredTranslationBuilder translationBuilder)
    {
        _keyBuilder = keyBuilder;
        _oldKeyBuilder = oldKeyBuilder;
        _configurationContext = configurationContext;
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
        // scan custom registered attributes (if any)
        foreach (var descriptor in _configurationContext.Value.CustomAttributes.ToList())
        {
            var customAttributes = mi.GetCustomAttributes(descriptor.CustomAttribute);
            foreach (var customAttribute in customAttributes)
            {
                var customAttributeKey = _keyBuilder.BuildResourceKey(resourceKey, customAttribute);
                var propertyName = customAttributeKey.Split('.').Last();
                var oldResourceKeys = _oldKeyBuilder.GenerateOldResourceKey(target,
                                                                            propertyName,
                                                                            mi,
                                                                            resourceKeyPrefix,
                                                                            typeOldName,
                                                                            typeOldNamespace);
                var foreignTranslation = string.Empty;
                if (descriptor.GenerateTranslation)
                {
                    var z1 = customAttribute.GetType().ToString();
                    var z2 = customAttribute.ToString();

                    foreignTranslation = !z1.Equals(z2) ? z2 : propertyName;
                }

                yield return new DiscoveredResource(mi,
                                                    customAttributeKey,
                                                    _translationBuilder.FromSingle(foreignTranslation),
                                                    propertyName,
                                                    declaringType,
                                                    returnType,
                                                    isSimpleType)
                {
                    TypeName = target.Name,
                    TypeNamespace = target.Namespace,
                    TypeOldName = oldResourceKeys.Item2,
                    TypeOldNamespace = typeOldNamespace,
                    OldResourceKey = oldResourceKeys.Item1
                };
            }
        }
    }
}

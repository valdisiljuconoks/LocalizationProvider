// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Abstractions.Refactoring;
using DbLocalizationProvider.Refactoring;

namespace DbLocalizationProvider.Sync;

internal class LocalizedResourceTypeScanner : LocalizedTypeScannerBase, IResourceTypeScanner
{
    public LocalizedResourceTypeScanner(
        ResourceKeyBuilder keyBuilder,
        OldResourceKeyBuilder oldKeyBuilder,
        ScanState state,
        ConfigurationContext configurationContext,
        DiscoveredTranslationBuilder translationBuilder) :
        base(keyBuilder, oldKeyBuilder, state, configurationContext, translationBuilder) { }

    public bool ShouldScan(Type target)
    {
        return (!target.IsNested ? target : target.DeclaringType)?
               .GetCustomAttribute<LocalizedResourceAttribute>()
               != null
               && target.BaseType != typeof(Enum);
    }

    public string GetResourceKeyPrefix(Type target, string keyPrefix = null)
    {
        var resourceAttribute = target.GetCustomAttribute<LocalizedResourceAttribute>();

        return !string.IsNullOrEmpty(resourceAttribute?.KeyPrefix)
            ? resourceAttribute.KeyPrefix
            : string.IsNullOrEmpty(keyPrefix)
                ? target.FullName
                : keyPrefix;
    }

    public ICollection<DiscoveredResource> GetResources(Type target, string resourceKeyPrefix)
    {
        var attr = target.GetCustomAttribute<LocalizedResourceAttribute>();
        var resourceSources = GetResourceSources(target, attr);
        var isKeyPrefixSpecified = !string.IsNullOrEmpty(attr?.KeyPrefix);
        var isHidden = target.GetCustomAttribute<HiddenAttribute>() != null;

        var refactoringInfo = target.GetCustomAttribute<RenamedResourceAttribute>();

        return DiscoverResourcesFromTypeMembers(target,
                                                resourceSources,
                                                resourceKeyPrefix,
                                                isKeyPrefixSpecified,
                                                isHidden,
                                                refactoringInfo?.OldName,
                                                refactoringInfo?.OldNamespace);
    }

    private ICollection<MemberInfo> GetResourceSources(Type target, LocalizedResourceAttribute attribute)
    {
        var onlyDeclared = false;
        var allProperties = true;

        if (attribute != null)
        {
            onlyDeclared = !attribute.Inherited;
            allProperties = !attribute.OnlyIncluded;
        }

        var flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;
        if (onlyDeclared)
        {
            flags = flags | BindingFlags.DeclaredOnly;
        }

        return target.GetProperties(flags | BindingFlags.GetProperty)
            .Union(target.GetFields(flags).Cast<MemberInfo>())
            .Where(pi => pi.GetCustomAttribute<IgnoreAttribute>() == null)
            .Where(pi => allProperties || pi.GetCustomAttribute<IncludeAttribute>() != null)
            .ToList();
    }
}

// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Abstractions.Refactoring;

namespace DbLocalizationProvider.Sync
{
    internal class LocalizedModelTypeScanner : LocalizedTypeScannerBase, IResourceTypeScanner
    {
        public bool ShouldScan(Type target)
        {
            return target.GetCustomAttribute<LocalizedModelAttribute>() != null;
        }

        public string GetResourceKeyPrefix(Type target, string keyPrefix = null)
        {
            var modelAttribute = target.GetCustomAttribute<LocalizedModelAttribute>();

            return !string.IsNullOrEmpty(modelAttribute?.KeyPrefix) ? modelAttribute.KeyPrefix : target.FullName;
        }

        public ICollection<DiscoveredResource> GetResources(Type target, string resourceKeyPrefix)
        {
            var resourceSources = GetResourceSources(target);
            var attr = target.GetCustomAttribute<LocalizedModelAttribute>();
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

        private ICollection<MemberInfo> GetResourceSources(Type target)
        {
            var modelAttribute = target.GetCustomAttribute<LocalizedModelAttribute>();
            if (modelAttribute == null) return new List<MemberInfo>();

            var flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;

            if (!modelAttribute.Inherited) flags = flags | BindingFlags.DeclaredOnly;

            return target.GetProperties(flags | BindingFlags.GetProperty)
                         .Union(target.GetFields(flags).Cast<MemberInfo>())
                         .Where(pi => pi.GetCustomAttribute<IgnoreAttribute>() == null)
                         .Where(pi => !modelAttribute.OnlyIncluded || pi.GetCustomAttribute<IncludeAttribute>() != null)
                         .ToList();
        }
    }
}

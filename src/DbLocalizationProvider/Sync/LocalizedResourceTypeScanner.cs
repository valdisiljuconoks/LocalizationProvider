// Copyright (c) 2018 Valdis Iljuconoks.
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Abstractions.Refactoring;

namespace DbLocalizationProvider.Sync
{
    internal class LocalizedResourceTypeScanner : LocalizedTypeScannerBase, IResourceTypeScanner
    {
        public bool ShouldScan(Type target)
        {
            return target.GetCustomAttribute<LocalizedResourceAttribute>() != null && target.BaseType != typeof(Enum);
        }

        public string GetResourceKeyPrefix(Type target, string keyPrefix = null)
        {
            var resourceAttribute = target.GetCustomAttribute<LocalizedResourceAttribute>();

            return !string.IsNullOrEmpty(resourceAttribute?.KeyPrefix)
                       ? resourceAttribute.KeyPrefix
                       : (string.IsNullOrEmpty(keyPrefix) ? target.FullName : keyPrefix);
        }

        public ICollection<DiscoveredResource> GetResources(Type target, string resourceKeyPrefix)
        {
            var resourceSources = GetResourceSources(target);
            var attr = target.GetCustomAttribute<LocalizedResourceAttribute>();
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
            var attr = target.GetCustomAttribute<LocalizedResourceAttribute>();
            var onlyDeclared = false;
            var allProperties = true;

            if(attr != null)
            {
                onlyDeclared = !attr.Inherited;
                allProperties = !attr.OnlyIncluded;
            }

            var flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;
            if(onlyDeclared)
                flags = flags | BindingFlags.DeclaredOnly;

            return target.GetProperties(flags | BindingFlags.GetProperty)
                         .Union(target.GetFields(flags).Cast<MemberInfo>())
                         .Where(pi => pi.GetCustomAttribute<IgnoreAttribute>() == null)
                         .Where(pi => allProperties || pi.GetCustomAttribute<IncludeAttribute>() != null)
                         .ToList();
        }
    }
}

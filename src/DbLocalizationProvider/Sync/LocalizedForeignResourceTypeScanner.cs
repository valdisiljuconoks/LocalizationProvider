// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using DbLocalizationProvider.Internal;

namespace DbLocalizationProvider.Sync
{
    internal class LocalizedForeignResourceTypeScanner : IResourceTypeScanner
    {
        private IResourceTypeScanner _actualScanner;

        public bool ShouldScan(Type target)
        {
            if (target.BaseType == typeof(Enum))
            {
                _actualScanner = new LocalizedEnumTypeScanner();
            }
            else
            {
                _actualScanner = new LocalizedResourceTypeScanner();
            }

            return true;
        }

        public string GetResourceKeyPrefix(Type target, string keyPrefix = null)
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
            var includeComplex = ConfigurationContext.Current.ForeignResources.Get(target)?.IncludeComplexProperties ?? false;
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
}

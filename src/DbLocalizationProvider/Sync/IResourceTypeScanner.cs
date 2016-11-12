using System;
using System.Collections.Generic;

namespace DbLocalizationProvider.Sync
{
    internal interface IResourceTypeScanner
    {
        bool ShouldScan(Type target);
        string GetResourceKeyPrefix(Type target, string keyPrefix = null);
        ICollection<DiscoveredResource> GetClassLevelResources(Type target, string resourceKeyPrefix);
        ICollection<DiscoveredResource> GetResources(Type target, string resourceKeyPrefix);
    }
}
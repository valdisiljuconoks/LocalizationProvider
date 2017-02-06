using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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

        public ICollection<DiscoveredResource> GetClassLevelResources(Type target, string resourceKeyPrefix)
        {
            return new List<DiscoveredResource>();
        }

        public ICollection<DiscoveredResource> GetResources(Type target, string resourceKeyPrefix)
        {
            var resourceSources = GetResourceSources(target);
            var attr = target.GetCustomAttribute<LocalizedResourceAttribute>();
            var isKeyPrefixSpecified = !string.IsNullOrEmpty(attr?.KeyPrefix);

            return resourceSources.SelectMany(pi => DiscoverResourcesFromProperty(pi, resourceKeyPrefix, isKeyPrefixSpecified)).ToList();
        }

        private ICollection<PropertyInfo> GetResourceSources(Type target)
        {
            var flags = BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Static;

            return target.GetProperties(flags)
                         .Where(pi => pi.GetCustomAttribute<IgnoreAttribute>() == null).ToList();
        }
    }
}
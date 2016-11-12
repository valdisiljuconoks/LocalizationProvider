using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DbLocalizationProvider.Internal;

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

        public ICollection<DiscoveredResource> GetClassLevelResources(Type target, string resourceKeyPrefix)
        {
            var result = new List<DiscoveredResource>();
            var resourceAttributesOnModelClass = target.GetCustomAttributes<ResourceKeyAttribute>().ToList();
            if(!resourceAttributesOnModelClass.Any())
                return result;

            foreach (var resourceKeyAttribute in resourceAttributesOnModelClass)
            {
                result.Add(new DiscoveredResource(null,
                                                  ResourceKeyBuilder.BuildResourceKey(resourceKeyPrefix, resourceKeyAttribute.Key, separator: string.Empty),
                                                  null,
                                                  resourceKeyAttribute.Value,
                                                  target,
                                                  typeof(string),
                                                  true));
            }

            return result;
        }

        public ICollection<DiscoveredResource> GetResources(Type target, string resourceKeyPrefix)
        {
            var resourceSources = GetResourceSources(target);
            var attr = target.GetCustomAttribute<LocalizedModelAttribute>();
            var isKeyPrefixSpecified = !string.IsNullOrEmpty(attr?.KeyPrefix);

            return resourceSources.SelectMany(pi => DiscoverResourcesFromProperty(pi, resourceKeyPrefix, isKeyPrefixSpecified)).ToList();
        }

        private ICollection<PropertyInfo> GetResourceSources(Type target)
        {
            var modelAttribute = target.GetCustomAttribute<LocalizedModelAttribute>();
            if(modelAttribute == null)
                return new List<PropertyInfo>();

            var flags = BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Static;

            if(!modelAttribute.Inherited)
                flags = flags | BindingFlags.DeclaredOnly;

            return target.GetProperties(flags)
                         .Where(pi => pi.GetCustomAttribute<IgnoreAttribute>() == null)
                         .Where(pi => !modelAttribute.OnlyIncluded || pi.GetCustomAttribute<IncludeAttribute>() != null).ToList();
        }
    }
}
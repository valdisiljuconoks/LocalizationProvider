using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Internal;

namespace DbLocalizationProvider.Sync
{
    internal class LocalizedEnumTypeScanner : IResourceTypeScanner
    {
        public bool ShouldScan(Type target)
        {
            return target.BaseType == typeof(Enum) && target.GetCustomAttribute<LocalizedResourceAttribute>() != null;
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
            return Enumerable.Empty<DiscoveredResource>().ToList();
        }

        public ICollection<DiscoveredResource> GetResources(Type target, string resourceKeyPrefix)
        {
            var enumType = Enum.GetUnderlyingType(target);
            var isHidden = target.GetCustomAttribute<HiddenAttribute>() != null;

            return target.GetMembers(BindingFlags.Public | BindingFlags.Static)
                         .Select(mi =>
                         {
                             var isResourceHidden = isHidden || mi.GetCustomAttribute<HiddenAttribute>() != null;
                             return new DiscoveredResource(mi,
                                                           ResourceKeyBuilder.BuildResourceKey(target, mi.Name),
                                                           mi.Name,
                                                           mi.Name,
                                                           target,
                                                           enumType,
                                                           enumType.IsSimpleType(),
                                                           isResourceHidden);
                         })
                         .ToList();
        }
    }
}

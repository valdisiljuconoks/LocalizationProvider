using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DbLocalizationProvider.Internal;

namespace DbLocalizationProvider.Sync.Collectors
{
    internal class ResourceKeyAttributeCollector : IResourceCollector
    {
        public IEnumerable<DiscoveredResource> GetDiscoveredResources(Type target,
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
            // check if there are [ResourceKey] attributes
            var keyAttributes = mi.GetCustomAttributes<ResourceKeyAttribute>().ToList();

            foreach(var resourceKeyAttribute in keyAttributes)
                yield return new DiscoveredResource(mi,
                    ResourceKeyBuilder.BuildResourceKey(typeKeyPrefixSpecified ? resourceKeyPrefix : null, resourceKeyAttribute.Key, string.Empty),
                    DiscoveredTranslation.FromSingle(string.IsNullOrEmpty(resourceKeyAttribute.Value)
                        ? translation
                        : resourceKeyAttribute.Value),
                    null,
                    declaringType,
                    returnType,
                    true)
                {
                    FromResourceKeyAttribute = true
                };
        }
    }
}

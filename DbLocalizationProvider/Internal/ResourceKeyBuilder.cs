using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using DbLocalizationProvider.Sync;

namespace DbLocalizationProvider.Internal
{
    internal static class ResourceKeyBuilder
    {
        internal static string BuildResourceKey(string prefix, MemberInfo mi, string separator = ".")
        {
            return BuildResourceKey(prefix, mi.Name);
        }

        internal static string BuildResourceKey(string prefix, string name, string separator = ".")
        {
            return string.IsNullOrEmpty(prefix) ? name : prefix.JoinNonEmpty(separator, name);
        }

        internal static string BuildResourceKey(string beginning, Stack<string> keyStack)
        {
            return keyStack.Aggregate(beginning, (prefix, name) => BuildResourceKey(prefix, name));
        }

        internal static string BuildResourceKey(Type cotnainerType, Stack<string> keyStack)
        {
            return BuildResourceKey(cotnainerType, keyStack.Aggregate(string.Empty, (prefix, name) => BuildResourceKey(prefix, name)));
        }

        internal static string BuildResourceKey(string keyPrefix, ValidationAttribute attribute)
        {
            if(attribute == null)
                throw new ArgumentNullException(nameof(attribute));

            var result = $"{keyPrefix}-{attribute.GetType().Name.Replace("Attribute", string.Empty)}";
            if(attribute.GetType().IsAssignableFrom(typeof(DataTypeAttribute)))
            {
                result += ((DataTypeAttribute) attribute).DataType;
            }

            return result;
        }

        internal static string BuildResourceKey(Type containerType, string propertyName, ValidationAttribute attribute)
        {
            return BuildResourceKey(BuildResourceKey(containerType, propertyName), attribute);
        }

        internal static string BuildResourceKey(Type containerType, string propertyName, string separator = ".")
        {
            var modelAttribute = containerType.GetCustomAttribute<LocalizedModelAttribute>();

            // we need to understand where to look for the property
            // 1. verify that property is declared on the passed in container type
            if(modelAttribute == null || modelAttribute.Inherited)
                return containerType.FullName.JoinNonEmpty(separator, propertyName);

            // 2. if not - then we scan through discovered and cached properties during initial scanning process and try to find on which type that property is declared
            var declaringType = FindPropertyDeclaringType(containerType, propertyName);
            return declaringType != null
                       ? declaringType.FullName.JoinNonEmpty(separator, propertyName)
                       : containerType.FullName.JoinNonEmpty(separator, propertyName);
        }

        private static Type FindPropertyDeclaringType(Type containerType, string propertyName)
        {
            while (true)
            {
                if(containerType == null)
                    return null;

                List<string> properties;
                if(TypeDiscoveryHelper.DiscoveredResourceCache.TryGetValue(containerType, out properties))
                {
                    // property was found on the container type level
                    if(properties.Contains(propertyName))
                    {
                        return containerType;
                    }
                }

                containerType = containerType.BaseType;
            }
        }
    }
}

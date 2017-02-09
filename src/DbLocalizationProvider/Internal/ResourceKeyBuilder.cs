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

        internal static string BuildResourceKey(string keyPrefix, Attribute attribute)
        {
            if(attribute == null)
                throw new ArgumentNullException(nameof(attribute));

            var result = BuildResourceKey(keyPrefix, attribute.GetType());
            if(attribute.GetType().IsAssignableFrom(typeof(DataTypeAttribute)))
                result += ((DataTypeAttribute) attribute).DataType;

            return result;
        }

        internal static string BuildResourceKey(string keyPrefix, Type attributeType)
        {
            if(attributeType == null)
                throw new ArgumentNullException(nameof(attributeType));

            if(!typeof(Attribute).IsAssignableFrom(attributeType))
                throw new ArgumentException($"Given type `{attributeType.FullName}` is not of type `System.Attribute`");

            return $"{keyPrefix}-{attributeType.Name.Replace("Attribute", string.Empty)}";
        }

        internal static string BuildResourceKey(Type containerType, string propertyName, Type attributeType)
        {
            return BuildResourceKey(BuildResourceKey(containerType, propertyName), attributeType);
        }

        internal static string BuildResourceKey(Type containerType, string propertyName, Attribute attribute)
        {
            return BuildResourceKey(BuildResourceKey(containerType, propertyName), attribute);
        }

        internal static string BuildResourceKey(Type containerType, string propertyName, string separator = ".")
        {
            var modelAttribute = containerType.GetCustomAttribute<LocalizedModelAttribute>();

            var pi = containerType.GetProperty(propertyName);
            if(pi != null)
            {
                var propertyResourceKeyAttribute = pi.GetCustomAttribute<ResourceKeyAttribute>();
                if(propertyResourceKeyAttribute != null)
                {
                    // check if container type has resource key set
                    var prefix = string.Empty;
                    if(!string.IsNullOrEmpty(modelAttribute?.KeyPrefix))
                        prefix = modelAttribute.KeyPrefix;

                    var resourceAttributeOnClass = containerType.GetCustomAttribute<LocalizedResourceAttribute>();
                    if(!string.IsNullOrEmpty(resourceAttributeOnClass?.KeyPrefix))
                        prefix = resourceAttributeOnClass.KeyPrefix;

                    return prefix.JoinNonEmpty(string.Empty, propertyResourceKeyAttribute.Key);
                }
            }

            // we need to understand where to look for the property
            // 1. verify that property is declared on given container type
            if(modelAttribute == null || modelAttribute.Inherited)
                return containerType.FullName.JoinNonEmpty(separator, propertyName);

            // 2. if not - then we scan through discovered and cached properties during initial scanning process and try to find on which type that property is declared
            var declaringTypeName = FindPropertyDeclaringTypeName(containerType, propertyName);
            return declaringTypeName != null
                       ? declaringTypeName.JoinNonEmpty(separator, propertyName)
                       : containerType.FullName.JoinNonEmpty(separator, propertyName);
        }

        private static string FindPropertyDeclaringTypeName(Type containerType, string propertyName)
        {
            // make private copy
            var currentContainerType = containerType;

            while (true)
            {
                if(currentContainerType == null)
                    return null;

                List<string> properties;
                var fullName = currentContainerType.FullName;

                if(currentContainerType.IsGenericType && !currentContainerType.IsGenericTypeDefinition)
                    fullName = currentContainerType.GetGenericTypeDefinition().FullName;

                if(TypeDiscoveryHelper.DiscoveredResourceCache.TryGetValue(fullName, out properties))
                {
                    // property was found in the container
                    if(properties.Contains(propertyName))
                        return fullName;
                }

                currentContainerType = currentContainerType.BaseType;
            }
        }
    }
}

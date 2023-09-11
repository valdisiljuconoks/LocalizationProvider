// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Internal;
using DbLocalizationProvider.Sync;

namespace DbLocalizationProvider
{
    /// <summary>
    /// This class can help you in some weird cases when you need to compose resource key manually.
    /// You should avoid to do so, but just in case.. then use this class.
    /// </summary>
    public class ResourceKeyBuilder
    {
        private readonly ScanState _state;
        private readonly ConfigurationContext _context;

        /// <summary>
        /// Initiates new instance of <see cref="ResourceKeyBuilder"/>.
        /// </summary>
        /// <param name="state">State of the scanning process.</param>
        /// <param name="context">Config state</param>
        public ResourceKeyBuilder(ScanState state, ConfigurationContext context)
        {
            _state = state;
            _context = context;
        }

        /// <summary>
        /// Builds resource key based on prefix and name of the resource
        /// </summary>
        /// <param name="prefix">Prefix for the resource key (usually namespace of the container)</param>
        /// <param name="name">Actual resource name (usually property name)</param>
        /// <param name="separator">Separator in between (usually `.`)</param>
        /// <returns>Full length resource key</returns>
        public string BuildResourceKey(string prefix, string name, string separator = ".")
        {
            return string.IsNullOrEmpty(prefix) ? name : JoinPrefixAndKey(prefix, name, separator);
        }

        /// <summary>
        /// Recursively builds resource key out of collected stack of strings
        /// </summary>
        /// <param name="containerType">Type of the container class for the resource</param>
        /// <param name="keyStack">Collected stack of strings - usually while walking the expression tree</param>
        /// <returns>Full length resource key</returns>
        public string BuildResourceKey(Type containerType, Stack<string> keyStack)
        {
            return BuildResourceKey(containerType,
                                    keyStack.Aggregate(string.Empty, (prefix, name) => BuildResourceKey(prefix, name)));
        }

        /// <summary>
        /// Builds resource key based on prefix and name of the resource
        /// </summary>
        /// <param name="keyPrefix">Prefix for the resource key (usually namespace of the container)</param>
        /// <param name="attribute">Attribute used for the resource - like `[Display]`, `[Description]`, etc.</param>
        /// <returns>Full length resource key</returns>
        public string BuildResourceKey(string keyPrefix, Attribute attribute)
        {
            if (attribute == null)
            {
                throw new ArgumentNullException(nameof(attribute));
            }

            var result = BuildResourceKey(keyPrefix, attribute.GetType());
            if (attribute.GetType().IsAssignableFrom(typeof(DataTypeAttribute)))
            {
                result += ((DataTypeAttribute)attribute).DataType;
            }

            return result;
        }

        /// <summary>
        /// Builds resource key based on prefix and name of the resource
        /// </summary>
        /// <param name="keyPrefix">Prefix for the resource key (usually namespace of the container)</param>
        /// <param name="attributeType">Type of the attribute used for the resource - like `[Display]`, `[Description]`, etc.</param>
        /// <returns>Full length resource key</returns>
        public string BuildResourceKey(string keyPrefix, Type attributeType)
        {
            if (attributeType == null)
            {
                throw new ArgumentNullException(nameof(attributeType));
            }

            if (!typeof(Attribute).IsAssignableFrom(attributeType))
            {
                throw new ArgumentException($"Given type `{attributeType.FullName}` is not of type `System.Attribute`");
            }

            return $"{keyPrefix}-{attributeType.Name.Replace("Attribute", string.Empty)}";
        }

        /// <summary>
        /// Builds resource key based on prefix and name of the resource
        /// </summary>
        /// <param name="containerType">Type of the container class for the resource</param>
        /// <param name="memberName">Actual resource name (usually property name)</param>
        /// <param name="attributeType">Type of the attribute used for the resource - like `[Display]`, `[Description]`, etc.</param>
        /// <returns>Full length resource key</returns>
        public string BuildResourceKey(Type containerType, string memberName, Type attributeType)
        {
            return BuildResourceKey(BuildResourceKey(containerType, memberName), attributeType);
        }

        /// <summary>
        /// Builds resource key based on prefix and name of the resource
        /// </summary>
        /// <param name="containerType">Type of the container class for the resource</param>
        /// <param name="memberName">Actual resource name (usually property name)</param>
        /// <param name="attribute">Attribute used for the resource - like `[Display]`, `[Description]`, etc.</param>
        /// <returns>Full length resource key</returns>
        public string BuildResourceKey(Type containerType, string memberName, Attribute attribute)
        {
            return BuildResourceKey(BuildResourceKey(containerType, memberName), attribute);
        }

        /// <summary>
        /// Builds resource key
        /// </summary>
        /// <param name="containerType">Type of the container class for the resource</param>
        /// <param name="memberName">Actual resource name (usually property name)</param>
        /// <param name="separator">Separator in between (usually `.`)</param>
        /// <returns>Full length resource key</returns>
        public string BuildResourceKey(Type containerType, string memberName, string separator = ".")
        {
            var modelAttribute = containerType.GetCustomAttribute<LocalizedModelAttribute>();
            var mi = containerType.GetMember(memberName).FirstOrDefault();

            var prefix = string.Empty;

            if (!string.IsNullOrEmpty(modelAttribute?.KeyPrefix))
            {
                prefix = modelAttribute.KeyPrefix;
            }

            var resourceAttributeOnClass = containerType.GetCustomAttribute<LocalizedResourceAttribute>();
            if (!string.IsNullOrEmpty(resourceAttributeOnClass?.KeyPrefix))
            {
                prefix = resourceAttributeOnClass.KeyPrefix;
            }

            if (mi != null)
            {
                var resourceKeyAttributes = mi.GetCustomAttributes<ResourceKeyAttribute>().ToList();
                if (resourceKeyAttributes.Count == 1)
                {
                    return JoinPrefixAndKey(prefix, resourceKeyAttributes.First().Key, string.Empty);
                }
            }

            if (!string.IsNullOrEmpty(prefix))
            {
                return JoinPrefixAndKey(prefix, memberName, separator);
            }

            // ##### we need to understand where to look for the property
            var potentialResourceKey = JoinPrefixAndKey(containerType.FullName, memberName, separator);

            // 1. maybe property has [UseResource] attribute, if so - then we need to look for "redirects"
            if (_state.UseResourceAttributeCache.TryGetValue(potentialResourceKey, out var redirectedResourceKey))
            {
                return redirectedResourceKey;
            }

            // 2. verify that property is declared on given container type
            if (modelAttribute == null || modelAttribute.Inherited)
            {
                return potentialResourceKey;
            }

            // 3. if not - then we scan through discovered and cached properties during initial scanning process and try to find on which type that property is declared
            var declaringTypeName = FindPropertyDeclaringTypeName(containerType, memberName);

            return declaringTypeName != null
                ? JoinPrefixAndKey(declaringTypeName, memberName, separator)
                : potentialResourceKey;
        }

        /// <summary>
        /// Builds resource key for type of container
        /// </summary>
        /// <param name="containerType">Type of the container (usually class decorated with `[LocalizedModel]` or `[LocalizedResource]`</param>
        /// <returns>Full length resource key</returns>
        public string BuildResourceKey(Type containerType)
        {
            var modelAttribute = containerType.GetCustomAttribute<LocalizedModelAttribute>();
            var resourceAttribute = containerType.GetCustomAttribute<LocalizedResourceAttribute>();

            if (modelAttribute == null && resourceAttribute == null)
            {
                throw new ArgumentException(
                    $"Type `{containerType.FullName}` is not decorated with localizable attributes ([LocalizedModelAttribute] or [LocalizedResourceAttribute])",
                    nameof(containerType));
            }

            return containerType.FullName;
        }

        private string FindPropertyDeclaringTypeName(Type containerType, string memberName)
        {
            // make private copy
            var currentContainerType = containerType;

            while (true)
            {
                if (currentContainerType == null)
                {
                    return null;
                }

                var fullName = currentContainerType.FullName;
                if (currentContainerType.IsGenericType && !currentContainerType.IsGenericTypeDefinition)
                {
                    fullName = currentContainerType.GetGenericTypeDefinition().FullName;
                }

                if (TypeDiscoveryHelper.DiscoveredResourceCache.TryGetValue(fullName, out var properties))
                {
                    // property was found in the container
                    if (properties.Contains(memberName))
                    {
                        return fullName;
                    }
                }

                currentContainerType = currentContainerType.BaseType;
            }
        }

        private string JoinPrefixAndKey(string prefix, string key, string separator)
        {
            return _context.EnableLegacyMode() && prefix.StartsWith("/")
                ? prefix.JoinNonEmpty(prefix.EndsWith("/") ? string.Empty : "/", key)
                : prefix.JoinNonEmpty(separator, key);
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using DbLocalizationProvider.Internal;

namespace DbLocalizationProvider.Sync
{
    internal abstract class LocalizedTypeScannerBase
    {
        protected IEnumerable<DiscoveredResource> DiscoverResourcesFromProperty(PropertyInfo pi, string resourceKeyPrefix, bool typeKeyPrefixSpecified)
        {
            // check if there are [ResourceKey] attributes
            var keyAttributes = pi.GetCustomAttributes<ResourceKeyAttribute>().ToList();
            var resourceKey = ResourceKeyBuilder.BuildResourceKey(resourceKeyPrefix, pi);
            var translation = GetResourceValue(pi);

            if(!keyAttributes.Any())
            {
                yield return new DiscoveredResource(pi,
                                                    resourceKey,
                                                    translation,
                                                    pi.Name,
                                                    pi.PropertyType,
                                                    pi.GetMethod.ReturnType,
                                                    pi.GetMethod.ReturnType.IsSimpleType());

                // try to fetch also [Display()] attribute to generate new "...-Description" resource => usually used for help text labels
                var displayAttribute = pi.GetCustomAttribute<DisplayAttribute>();
                if(displayAttribute?.Description != null)
                {
                    yield return new DiscoveredResource(pi,
                                                        $"{resourceKey}-Description",
                                                        displayAttribute.Description,
                                                        $"{pi.Name}-Description",
                                                        pi.PropertyType,
                                                        pi.GetMethod.ReturnType,
                                                        pi.GetMethod.ReturnType.IsSimpleType());
                }

                var validationAttributes = pi.GetCustomAttributes<ValidationAttribute>();
                foreach (var validationAttribute in validationAttributes)
                {
                    var validationResourceKey = ResourceKeyBuilder.BuildResourceKey(resourceKey, validationAttribute);
                    var propertyName = validationResourceKey.Split('.').Last();
                    yield return new DiscoveredResource(pi,
                                                        validationResourceKey,
                                                        string.IsNullOrEmpty(validationAttribute.ErrorMessage) ? propertyName : validationAttribute.ErrorMessage,
                                                        propertyName,
                                                        pi.PropertyType,
                                                        pi.GetMethod.ReturnType,
                                                        pi.GetMethod.ReturnType.IsSimpleType());
                }
            }

            foreach (var resourceKeyAttribute in keyAttributes)
            {
                yield return new DiscoveredResource(pi,
                                                    ResourceKeyBuilder.BuildResourceKey(typeKeyPrefixSpecified ? resourceKeyPrefix : null,
                                                                                        resourceKeyAttribute.Key,
                                                                                        separator: string.Empty),
                                                    string.IsNullOrEmpty(resourceKeyAttribute.Value) ? translation : resourceKeyAttribute.Value,
                                                    null,
                                                    pi.PropertyType,
                                                    pi.GetMethod.ReturnType,
                                                    true);
            }
        }

        private static string GetResourceValue(PropertyInfo pi)
        {
            var result = pi.Name;

            // try to extract resource value
            var methodInfo = pi.GetGetMethod();
            if(IsStringProperty(methodInfo.ReturnType))
            {
                try
                {
                    if(methodInfo.IsStatic)
                    {
                        result = methodInfo.Invoke(null, null) as string;
                    }
                    else
                    {
                        if(pi.DeclaringType != null)
                        {
                            var targetInstance = Activator.CreateInstance(pi.DeclaringType);
                            var propertyReturnValue = methodInfo.Invoke(targetInstance, null) as string;
                            if(propertyReturnValue != null)
                            {
                                result = propertyReturnValue;
                            }
                        }
                    }
                }
                catch
                {
                    // if we fail to retrieve value for the resource - fair enough
                }
            }

            var attributes = pi.GetCustomAttributes(true);
            var displayAttribute = attributes.OfType<DisplayAttribute>().FirstOrDefault();

            if(!string.IsNullOrEmpty(displayAttribute?.GetName()))
            {
                result = displayAttribute.GetName();
            }

            var displayNameAttribute = attributes.OfType<DisplayNameAttribute>().FirstOrDefault();
            if(!string.IsNullOrEmpty(displayNameAttribute?.DisplayName))
            {
                result = displayNameAttribute.DisplayName;
            }

            return result;
        }

        internal static bool IsStringProperty(Type returnType)
        {
            return returnType == typeof(string);
        }
    }
}
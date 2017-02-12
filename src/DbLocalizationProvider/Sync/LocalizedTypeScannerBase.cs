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
        protected IEnumerable<DiscoveredResource> DiscoverResourcesFromMember(MemberInfo pi, string resourceKeyPrefix, bool typeKeyPrefixSpecified)
        {
            // check if there are [ResourceKey] attributes
            var keyAttributes = pi.GetCustomAttributes<ResourceKeyAttribute>().ToList();
            var resourceKey = ResourceKeyBuilder.BuildResourceKey(resourceKeyPrefix, pi);
            var translation = GetResourceValue(pi);

            Type declaringType = null;
            Type returnType = null;
            var isSimpleType = false;

            if(pi is PropertyInfo)
            {
                var info = (PropertyInfo) pi;
                declaringType = info.PropertyType;
                returnType = info.GetMethod.ReturnType;
                isSimpleType = returnType.IsSimpleType();
            }
            else if(pi is FieldInfo)
            {
                var info = (FieldInfo) pi;
                declaringType = info.GetUnderlyingType();
                returnType = info.GetUnderlyingType();
                isSimpleType = returnType.IsSimpleType();
            }

            if(!keyAttributes.Any())
            {
                yield return new DiscoveredResource(pi,
                                                    resourceKey,
                                                    translation,
                                                    pi.Name,
                                                    declaringType,
                                                    returnType,
                                                    isSimpleType);

                // try to fetch also [Display()] attribute to generate new "...-Description" resource => usually used for help text labels
                var displayAttribute = pi.GetCustomAttribute<DisplayAttribute>();
                if(displayAttribute?.Description != null)
                {
                    yield return new DiscoveredResource(pi,
                                                        $"{resourceKey}-Description",
                                                        displayAttribute.Description,
                                                        $"{pi.Name}-Description",
                                                        declaringType,
                                                        returnType,
                                                        isSimpleType);
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
                                                        declaringType,
                                                        returnType,
                                                        isSimpleType);
                }

                // scan custom registered attributes (if any)
                foreach (var descriptor in ConfigurationContext.Current.CustomAttributes)
                {
                    var customAttributes = pi.GetCustomAttributes(descriptor.CustomAttribute);
                    foreach (var customAttribute in customAttributes)
                    {
                        var customAttributeKey = ResourceKeyBuilder.BuildResourceKey(resourceKey, customAttribute);
                        var propertyName = customAttributeKey.Split('.').Last();
                        yield return new DiscoveredResource(pi,
                                                            customAttributeKey,
                                                            descriptor.GenerateTranslation ? propertyName : string.Empty,
                                                            propertyName,
                                                            declaringType,
                                                            returnType,
                                                            isSimpleType);
                    }
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
                                                    declaringType,
                                                    returnType,
                                                    true)
                {
                    FromResourceKeyAttribute = true
                };
            }
        }

        private static string GetResourceValue(MemberInfo pi)
        {
            var result = pi.Name;

            // try to extract resource value from property
            
            if(pi is PropertyInfo)
            {
                var info = (PropertyInfo) pi;
                var methodInfo = info.GetGetMethod();
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
            }
            else if(pi is FieldInfo)
            {
                var info = (FieldInfo) pi;
                if(info.IsStatic)
                {
                    result = info.GetValue(null).ToString();
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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Internal;

namespace DbLocalizationProvider.Sync
{
    internal abstract class LocalizedTypeScannerBase
    {
        protected ICollection<DiscoveredResource> DiscoverResourcesFromTypeMembers(Type type, ICollection<MemberInfo> members, string resourceKeyPrefix, bool typeKeyPrefixSpecified, bool isHidden)
        {
            object typeInstance = null;

            try
            {
                typeInstance = Activator.CreateInstance(type);
            }
            catch (Exception e)
            {
            }

            return members.SelectMany(mi => DiscoverResourcesFromMember(typeInstance, mi, resourceKeyPrefix, typeKeyPrefixSpecified, isHidden)).ToList();
        }

        private IEnumerable<DiscoveredResource> DiscoverResourcesFromMember(object instance, MemberInfo mi, string resourceKeyPrefix, bool typeKeyPrefixSpecified, bool isHidden)
        {
            // check if there are [ResourceKey] attributes
            var keyAttributes = mi.GetCustomAttributes<ResourceKeyAttribute>().ToList();
            var resourceKey = ResourceKeyBuilder.BuildResourceKey(resourceKeyPrefix, mi.Name);
            var translation = GetResourceValue(instance, mi);

            Type declaringType = null;
            Type returnType = null;
            var isSimpleType = false;

            if(mi is PropertyInfo)
            {
                var info = (PropertyInfo) mi;
                declaringType = info.PropertyType;
                returnType = info.GetMethod.ReturnType;
                isSimpleType = returnType.IsSimpleType();
            }
            else if(mi is FieldInfo)
            {
                var info = (FieldInfo) mi;
                declaringType = info.GetUnderlyingType();
                returnType = info.GetUnderlyingType();
                isSimpleType = returnType.IsSimpleType();
            }

            if(!keyAttributes.Any())
            {
                var isResourceHidden = isHidden || mi.GetCustomAttribute<HiddenAttribute>() != null;
                yield return new DiscoveredResource(mi,
                                                    resourceKey,
                                                    translation,
                                                    mi.Name,
                                                    declaringType,
                                                    returnType,
                                                    isSimpleType,
                                                    isResourceHidden);

                // try to fetch also [Display()] attribute to generate new "...-Description" resource => usually used for help text labels
                var displayAttribute = mi.GetCustomAttribute<DisplayAttribute>();
                if(displayAttribute?.Description != null)
                {
                    yield return new DiscoveredResource(mi,
                                                        $"{resourceKey}-Description",
                                                        displayAttribute.Description,
                                                        $"{mi.Name}-Description",
                                                        declaringType,
                                                        returnType,
                                                        isSimpleType);
                }

                var validationAttributes = mi.GetCustomAttributes<ValidationAttribute>();
                foreach (var validationAttribute in validationAttributes)
                {
                    if(validationAttribute.GetType() == typeof(DataTypeAttribute))
                        continue;

                    var validationResourceKey = ResourceKeyBuilder.BuildResourceKey(resourceKey, validationAttribute);
                    var propertyName = validationResourceKey.Split('.').Last();
                    yield return new DiscoveredResource(mi,
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
                    var customAttributes = mi.GetCustomAttributes(descriptor.CustomAttribute);
                    foreach (var customAttribute in customAttributes)
                    {
                        var customAttributeKey = ResourceKeyBuilder.BuildResourceKey(resourceKey, customAttribute);
                        var propertyName = customAttributeKey.Split('.').Last();
                        yield return new DiscoveredResource(mi,
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
                yield return new DiscoveredResource(mi,
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

        private static string GetResourceValue(object instance, MemberInfo mi)
        {
            var result = mi.Name;

            if(mi is PropertyInfo)
            {
                // try to extract resource value from property
                var info = (PropertyInfo) mi;
                var methodInfo = info.GetGetMethod();
                if(IsStringProperty(methodInfo.ReturnType))
                {
                    try
                    {
                        if(!methodInfo.IsStatic)
                        {
                            if(mi.DeclaringType != null && instance != null)
                            {
                                var propertyValue = methodInfo.Invoke(instance, null) as string;
                                if(propertyValue != null)
                                    result = propertyValue;
                            }
                        }
                        else
                            result = methodInfo.Invoke(null, null) as string ?? result;
                    }
                    catch
                    {
                        // if we fail to retrieve value for the resource - fair enough
                    }
                }
            }
            else if(mi is FieldInfo)
            {
                // try to extract resource value from field
                var info = (FieldInfo) mi;
                if(info.IsStatic)
                    result = info.GetValue(null) as string ?? result;
                else
                {
                    if(instance != null)
                    {
                        var fieldValue = info.GetValue(instance) as string;
                        if(fieldValue != null)
                            result = fieldValue;
                    }
                }
            }

            var attributes = mi.GetCustomAttributes(true);
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

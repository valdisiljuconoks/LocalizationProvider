// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Internal;
using DbLocalizationProvider.Refactoring;
using DbLocalizationProvider.Sync.Collectors;
using Microsoft.Extensions.Options;

namespace DbLocalizationProvider.Sync;

internal abstract class LocalizedTypeScannerBase(
    ResourceKeyBuilder keyBuilder,
    OldResourceKeyBuilder oldKeyBuilder,
    ScanState state,
    IOptions<ConfigurationContext> configurationContext,
    DiscoveredTranslationBuilder translationBuilder)
{
    private readonly ICollection<IResourceCollector> _collectors = new List<IResourceCollector>
    {
        new UseResourceAttributeCollector(keyBuilder, state),
        new CustomAttributeCollector(keyBuilder, oldKeyBuilder, configurationContext, translationBuilder),
        new ValidationAttributeCollector(keyBuilder, oldKeyBuilder, translationBuilder),
        new ResourceKeyAttributeCollector(keyBuilder, translationBuilder),
        new DisplayAttributeCollector(oldKeyBuilder, translationBuilder),
        new CasualResourceCollector(oldKeyBuilder, translationBuilder)
    };

    public ICollection<DiscoveredResource> GetClassLevelResources(Type target, string resourceKeyPrefix)
    {
        var result = new List<DiscoveredResource>();
        var resourceAttributesOnModelClass = target.GetCustomAttributes<ResourceKeyAttribute>().ToList();
        if (!resourceAttributesOnModelClass.Any())
        {
            return result;
        }

        foreach (var resourceKeyAttribute in resourceAttributesOnModelClass)
        {
            result.Add(
                new DiscoveredResource(
                    null,
                    keyBuilder.BuildResourceKey(resourceKeyPrefix, resourceKeyAttribute.Key, string.Empty),
                    translationBuilder.FromSingle(resourceKeyAttribute.Value),
                    resourceKeyAttribute.Value,
                    target,
                    typeof(string),
                    true));
        }

        return result;
    }

    protected ICollection<DiscoveredResource> DiscoverResourcesFromTypeMembers(
        Type target,
        ICollection<MemberInfo> members,
        string resourceKeyPrefix,
        bool typeKeyPrefixSpecified,
        bool isHidden,
        string? typeOldName = null,
        string? typeOldNamespace = null)
    {
        object typeInstance = null;

        try
        {
            typeInstance = Activator.CreateInstance(target);
        }
        catch (Exception) { }

        return members.SelectMany(
                mi => DiscoverResourcesFromMember(
                    target,
                    typeInstance,
                    mi,
                    resourceKeyPrefix,
                    typeKeyPrefixSpecified,
                    isHidden,
                    typeOldName,
                    typeOldNamespace))
            .ToList();
    }

    private ICollection<DiscoveredResource> DiscoverResourcesFromMember(
        Type target,
        object instance,
        MemberInfo mi,
        string resourceKeyPrefix,
        bool typeKeyPrefixSpecified,
        bool isHidden,
        string typeOldName = null,
        string typeOldNamespace = null)
    {
        var resourceKey = keyBuilder.BuildResourceKey(resourceKeyPrefix, mi.Name);
        var translation = GetResourceValue(instance, mi);

        Type declaringType = null;
        Type returnType = null;
        var isSimpleType = false;

        switch (mi)
        {
            case PropertyInfo propertyInfo:
                declaringType = propertyInfo.PropertyType;
                returnType = propertyInfo.GetMethod.ReturnType;
                isSimpleType = returnType.IsSimpleType();
                break;
            case FieldInfo fieldInfo:
                declaringType = fieldInfo.GetUnderlyingType();
                returnType = fieldInfo.GetUnderlyingType();
                isSimpleType = returnType.IsSimpleType();
                break;
        }

        var result = new List<DiscoveredResource>();

        foreach (var collector in _collectors)
        {
            result.AddRange(
                collector.GetDiscoveredResources(
                        target,
                        instance,
                        mi,
                        translation,
                        resourceKey,
                        resourceKeyPrefix,
                        typeKeyPrefixSpecified,
                        isHidden,
                        typeOldName,
                        typeOldNamespace,
                        declaringType,
                        returnType,
                        isSimpleType)
                    .ToList());
        }

        return result;
    }

    private static string GetResourceValue(object instance, MemberInfo mi)
    {
        var result = mi.Name;

        switch (mi)
        {
            case PropertyInfo info1:
                // try to extract resource value from property
                var methodInfo = info1.GetGetMethod();
                if (IsStringProperty(methodInfo.ReturnType))
                {
                    try
                    {
                        if (!methodInfo.IsStatic)
                        {
                            if (mi.DeclaringType != null && instance != null)
                            {
                                if (methodInfo.Invoke(instance, null) is string propertyValue)
                                {
                                    result = propertyValue;
                                }
                            }
                        }
                        else
                        {
                            result = methodInfo.Invoke(null, null) as string ?? result;
                        }
                    }
                    catch
                    {
                        // if we fail to retrieve value for the resource - fair enough
                    }
                }

                break;
            case FieldInfo fieldInfo:
                // try to extract resource value from field
                if (fieldInfo.IsStatic)
                {
                    result = fieldInfo.GetValue(null) as string ?? result;
                }
                else
                {
                    if (instance != null)
                    {
                        if (fieldInfo.GetValue(instance) is string fieldValue)
                        {
                            result = fieldValue;
                        }
                    }
                }

                break;
        }

        var attributes = mi.GetCustomAttributes(true);
        var displayAttribute = attributes.OfType<DisplayAttribute>().FirstOrDefault();

        if (!string.IsNullOrEmpty(displayAttribute?.GetName()))
        {
            result = displayAttribute.GetName();
        }

        var displayNameAttribute = attributes.OfType<DisplayNameAttribute>().FirstOrDefault();
        if (!string.IsNullOrEmpty(displayNameAttribute?.DisplayName))
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

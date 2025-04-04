// Copyright (c) Stefan Holm Olsen. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using System.Reflection;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Queries;
using DbLocalizationProvider.Sync;

namespace DbLocalizationProvider;

/// <summary>
/// Much faster translations to object converter based on reflection
/// </summary>
public class ReflectionConverter(ScanState scanState, ResourceKeyBuilder keyBuilder, IResourceService resourceService)
{
    /// <summary>
    /// Creates an object of <typeparam name="T"></typeparam> and fills with translations
    /// </summary>
    /// <param name="languageName">Specify in which language you are going to use</param>
    /// <param name="fallbackCollection">Fallback languages collection</param>
    /// <typeparam name="T">Specify target object type</typeparam>
    /// <returns>If all is good, will return object of type <typeparam name="T"></typeparam> filled with translations of matching keys</returns>
    public T Convert<T>(string languageName, FallbackLanguagesCollection fallbackCollection)
    {
        // TODO: Can the dictionary be cached?
        var resources = resourceService.GetAllResources();

        var newObject = Activator.CreateInstance<T>();

        FillProperties(newObject!, languageName, resources, fallbackCollection);

        return newObject;
    }

    private void FillProperties(
        object instance,
        string languageName,
        IDictionary<string, LocalizationResource> resources,
        FallbackLanguagesCollection fallbackCollection)
    {
        var type = instance!.GetType();
        var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static);

        foreach (var propertyInfo in properties)
        {
            if (!propertyInfo.CanWrite)
            {
                continue;
            }

            if (propertyInfo.MemberType == MemberTypes.NestedType)
            {
                var nestedObject = Activator.CreateInstance(propertyInfo.PropertyType);
                if (nestedObject == null)
                {
                    continue;
                }

                FillProperties(nestedObject, languageName, resources, fallbackCollection);

                propertyInfo.SetValue(instance, nestedObject);
            }

            if (propertyInfo.PropertyType != typeof(string))
            {
                continue;
            }

            string? translation;
            var key = keyBuilder.BuildResourceKey(type, propertyInfo.Name);
            if (scanState.UseResourceAttributeCache.TryGetValue(key, out var targetResourceKey) 
                && resources.TryGetValue(targetResourceKey, out var foundResource))
            {
                translation = foundResource.Translations.GetValueWithFallback(
                    languageName,
                    fallbackCollection.GetFallbackLanguages(languageName));

                propertyInfo.SetValue(instance, translation);
                continue;
            }

            if (!resources.TryGetValue(key, out var resource))
            {
                continue;
            }

            translation = resource.Translations.GetValueWithFallback(
                languageName,
                fallbackCollection.GetFallbackLanguages(languageName));

            propertyInfo.SetValue(instance, translation);
        }
    }
}

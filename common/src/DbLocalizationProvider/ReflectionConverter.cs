using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Queries;
using DbLocalizationProvider.Sync;

namespace DbLocalizationProvider;

public class ReflectionConverter
{
    private readonly IQueryExecutor _queryExecutor;
    private readonly ScanState _scanState;

    public ReflectionConverter(IQueryExecutor queryExecutor, ScanState scanState)
    {
        _queryExecutor = queryExecutor;
        _scanState = scanState;
    }

    public T Convert<T>(string languageName, FallbackLanguagesCollection fallbackCollection)
    {
        // TODO: Can the dictionary be cached?
        // TODO: Can we go around query execution and use repository directly?
        var resources = _queryExecutor
            .Execute(new GetAllResources.Query())
            .ToDictionary(x => x.ResourceKey, StringComparer.Ordinal);

        var newObject = Activator.CreateInstance<T>();

        FillProperties(newObject!, languageName, resources, fallbackCollection);
        
        return newObject;
    }

    private void FillProperties(object instance, string languageName, Dictionary<string, LocalizationResource> resources, FallbackLanguagesCollection fallbackCollection)
    {
        var type = instance!.GetType();
        var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static);
        foreach (var propertyInfo in properties)
        {
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
            string key = $"{type.FullName}.{propertyInfo.Name}";
            if (_scanState.UseResourceAttributeCache.TryGetValue(key, out var targetResourceKey))
            {
                if (resources.TryGetValue(targetResourceKey, out var foundResource))
                {
                    translation = foundResource.Translations.GetValueWithFallback(
                        languageName,
                        fallbackCollection.GetFallbackLanguages(languageName));
                    
                    propertyInfo.SetValue(instance, translation);
                    continue;
                }
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

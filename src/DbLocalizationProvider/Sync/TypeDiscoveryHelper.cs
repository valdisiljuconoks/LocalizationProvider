using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DbLocalizationProvider.Sync
{
    internal class TypeDiscoveryHelper
    {
        internal static ConcurrentDictionary<string, List<string>> DiscoveredResourceCache = new ConcurrentDictionary<string, List<string>>();
        private readonly List<IResourceTypeScanner> _scanners = new List<IResourceTypeScanner>();

        public TypeDiscoveryHelper()
        {
            _scanners.Add(new LocalizedModelTypeScanner());
            _scanners.Add(new LocalizedResourceTypeScanner());
        }

        public IEnumerable<DiscoveredResource> ScanResources(Type target, string keyPrefix = null, IResourceTypeScanner scanner = null)
        {
            var typeScanner = scanner;

            if(scanner == null)
                typeScanner = _scanners.FirstOrDefault(s => s.ShouldScan(target));

            if(typeScanner == null)
                return new List<DiscoveredResource>();

            if (target.IsGenericParameter)
                return new List<DiscoveredResource>();

            var resourceKeyPrefix = typeScanner.GetResourceKeyPrefix(target, keyPrefix);

            var buffer = new List<DiscoveredResource>();
            buffer.AddRange(typeScanner.GetClassLevelResources(target, resourceKeyPrefix));

            buffer.AddRange(typeScanner.GetResources(target, resourceKeyPrefix));

            var result = buffer.Where(t => t.IsSimpleType || t.Info == null || t.Info.GetCustomAttribute<IncludeAttribute>() != null)
                               .ToList();

            foreach (var property in buffer.Where(t => !t.IsSimpleType))
            {
                if(!property.IsSimpleType)
                    result.AddRange(ScanResources(property.DeclaringType, property.Key, typeScanner));
            }

            var duplicateKeys = result.GroupBy(r => r.Key).Where(g => g.Count() > 1).ToList();
            if(duplicateKeys.Any())
                throw new DuplicateResourceKeyException($"Duplicate keys: [{string.Join(", ", duplicateKeys.Select(g => g.Key))}]");

            // add scanned resources to the cache
            DiscoveredResourceCache.TryAdd(target.FullName, result.Where(r => !string.IsNullOrEmpty(r.PropertyName)).Select(r => r.PropertyName).ToList());

            return result;
        }

        internal static List<List<Type>> GetTypes(params Func<Type, bool>[] filters)
        {
            if(filters == null)
            {
                throw new ArgumentNullException(nameof(filters));
            }

            var result = new List<List<Type>>();
            for (var i = 0; i < filters.Length; i++)
            {
                result.Add(new List<Type>());
            }

            var assemblies = GetAssemblies();
            foreach (var assembly in assemblies)
            {
                try
                {
                    var types = assembly.GetTypes();
                    for (var i = 0; i < filters.Length; i++)
                    {
                        result[i].AddRange(types.Where(filters[i]));
                    }
                }
                catch (Exception)
                {
                    // ignored
                }
            }

            return result;
        }

        internal static IEnumerable<Type> GetTypesWithAttribute<T>() where T : Attribute
        {
            return GetTypes(t => t.GetCustomAttribute<T>() != null).FirstOrDefault();
        }

        internal static IEnumerable<Type> GetTypesChildOf<T>()
        {
            var allTypes = new List<Type>();
            foreach (var assembly in GetAssemblies())
            {
                allTypes.AddRange(GetTypesChildOfInAssembly(typeof(T), assembly));
            }

            return allTypes;
        }

        //internal static IEnumerable<DiscoveredResource> GetAllProperties(Type type, string keyPrefix = null, bool contextAwareScanning = true)
        //{
        //    var resourceKeyPrefix = type.FullName;
        //    var typeKeyPrefixSpecified = false;
        //    var properties = new List<DiscoveredResource>();
        //    var modelAttribute = type.GetCustomAttribute<LocalizedModelAttribute>();

        //    //if(contextAwareScanning)
        //    //{
        //    // this is resource class scanning - try to fetch resource key prefix attribute if set there
        //    //var resourceAttribute = type.GetCustomAttribute<LocalizedResourceAttribute>();
        //    //if(!string.IsNullOrEmpty(resourceAttribute?.KeyPrefix))
        //    //{
        //    //    resourceKeyPrefix = resourceAttribute.KeyPrefix;
        //    //    typeKeyPrefixSpecified = true;
        //    //}
        //    //else
        //    //{
        //    //    resourceKeyPrefix = string.IsNullOrEmpty(keyPrefix) ? type.FullName : keyPrefix;
        //    //}
        //    //}
        //    //else
        //    //{
        //    // this is model scanning - try to fetch resource key prefix attribute if set there
        //    //if(!string.IsNullOrEmpty(modelAttribute?.KeyPrefix))
        //    //{
        //    //    resourceKeyPrefix = modelAttribute.KeyPrefix;
        //    //    typeKeyPrefixSpecified = true;
        //    //}

        //    //var resourceAttributesOnModelClass = type.GetCustomAttributes<ResourceKeyAttribute>().ToList();
        //    //if(resourceAttributesOnModelClass.Any())
        //    //{
        //    //    foreach (var resourceKeyAttribute in resourceAttributesOnModelClass)
        //    //    {
        //    //        properties.Add(new DiscoveredResource(null,
        //    //                                              ResourceKeyBuilder.BuildResourceKey(resourceKeyPrefix, resourceKeyAttribute.Key, separator: string.Empty),
        //    //                                              null,
        //    //                                              resourceKeyAttribute.Value,
        //    //                                              type,
        //    //                                              typeof(string),
        //    //                                              true));
        //    //    }
        //    //}
        //    //}

        //    //if(type.BaseType == typeof(Enum))
        //    //{
        //    //    properties.AddRange(type.GetMembers(BindingFlags.Public | BindingFlags.Static)
        //    //                            .Select(mi => new DiscoveredResource(mi,
        //    //                                                                 ResourceKeyBuilder.BuildResourceKey(resourceKeyPrefix, mi),
        //    //                                                                 mi.Name,
        //    //                                                                 mi.Name,
        //    //                                                                 type,
        //    //                                                                 Enum.GetUnderlyingType(type),
        //    //                                                                 Enum.GetUnderlyingType(type).IsSimpleType())).ToList());
        //    //}
        //    //else
        //    //{
        //    //    var flags = BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Static;
        //    //    if (modelAttribute != null && !modelAttribute.Inherited)
        //    //        flags = flags | BindingFlags.DeclaredOnly;

        //    //    properties.AddRange(type.GetProperties(flags)
        //    //                            .Where(pi => pi.GetCustomAttribute<IgnoreAttribute>() == null)
        //    //                            .Where(pi => modelAttribute == null || !modelAttribute.OnlyIncluded || pi.GetCustomAttribute<IncludeAttribute>() != null)
        //    //                            .SelectMany(pi => DiscoverResourcesFromProperty(pi, resourceKeyPrefix, typeKeyPrefixSpecified)).ToList());
        //    //}

        //    //var duplicateKeys = properties.GroupBy(r => r.Key).Where(g => g.Count() > 1).ToList();
        //    //if(duplicateKeys.Any())
        //    //{
        //    //    throw new DuplicateResourceKeyException($"Duplicate keys: [{string.Join(", ", duplicateKeys.Select(g => g.Key))}]");
        //    //}

        //    // first we can filter out all simple and/or complex included properties from the type as starting list of discovered resources

        //    var results = new List<DiscoveredResource>(properties.Where(t => t.IsSimpleType || t.Info == null || t.Info.GetCustomAttribute<IncludeAttribute>() != null));

        //    //foreach (var property in properties)
        //    //{
        //    //    var pi = property.Info;
        //    //    var deeperModelType = property.ReturnType;

        //    //    if(!property.IsSimpleType)
        //    //    {
        //    //        // if this is not a simple type - we need to scan deeper only if deeper model has attribute annotation
        //    //        if(contextAwareScanning || deeperModelType.GetCustomAttribute<LocalizedModelAttribute>() != null)
        //    //        {
        //    //            results.AddRange(GetAllProperties(property.DeclaringType, property.Key, contextAwareScanning));
        //    //        }
        //    //    }

        //    //    if(pi == null)
        //    //        continue;

        //    //    var validationAttributes = pi.GetCustomAttributes<ValidationAttribute>();
        //    //    foreach (var validationAttribute in validationAttributes)
        //    //    {
        //    //        var resourceKey = ResourceKeyBuilder.BuildResourceKey(property.Key, validationAttribute);
        //    //        var propertyName = resourceKey.Split('.').Last();
        //    //        results.Add(new DiscoveredResource(pi,
        //    //                                           resourceKey,
        //    //                                           string.IsNullOrEmpty(validationAttribute.ErrorMessage) ? propertyName : validationAttribute.ErrorMessage,
        //    //                                           propertyName,
        //    //                                           property.DeclaringType,
        //    //                                           property.ReturnType,
        //    //                                           property.ReturnType.IsSimpleType()));
        //    //    }
        //    //}

        //    // add scanned resources to the cache
        //    //DiscoveredResourceCache.TryAdd(type.FullName, results.Where(r => !string.IsNullOrEmpty(r.PropertyName)).Select(r => r.PropertyName).ToList());

        //    return results;
        //}

        private static IEnumerable<Assembly> GetAssemblies()
        {
            return ConfigurationContext.Current.AssemblyScanningFilter == null
                       ? AppDomain.CurrentDomain.GetAssemblies()
                       : AppDomain.CurrentDomain.GetAssemblies().Where(ConfigurationContext.Current.AssemblyScanningFilter);
        }

        private static IEnumerable<Type> GetTypesChildOfInAssembly(Type type, Assembly assembly)
        {
            return SelectTypes(assembly, t => t.IsSubclassOf(type) && !t.IsAbstract);
        }

        private static IEnumerable<Type> SelectTypes(Assembly assembly, Func<Type, bool> filter)
        {
            try
            {
                return assembly.GetTypes().Where(filter);
            }
            catch (Exception)
            {
                // there could be situations when type could not be loaded 
                // this may happen if we are visiting *all* loaded assemblies in application domain 
                return new List<Type>();
            }
        }
    }
}

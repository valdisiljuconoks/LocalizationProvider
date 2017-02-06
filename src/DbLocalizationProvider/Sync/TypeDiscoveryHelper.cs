using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DbLocalizationProvider.Internal;

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
            _scanners.Add(new EnumTypeScanner());
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

            // throw up if there are any duplicate resources manually registered
            var duplicateKeys = result.Where(r => r.FromResourceKeyAttribute).GroupBy(r => r.Key).Where(g => g.Count() > 1).ToList();
            if(duplicateKeys.Any())
                throw new DuplicateResourceKeyException($"Duplicate keys: [{string.Join(", ", duplicateKeys.Select(g => g.Key))}]");

            // we need to filter out duplicate resources (this comes from the case when the same model is used in multiple places
            // in the same parent container type. for instance: billing address and office address. both of them will be registered
            // under Address container type - twice, one via billing context - another one via office address property).
            result = result.DistinctBy(r => r.Key).ToList();

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

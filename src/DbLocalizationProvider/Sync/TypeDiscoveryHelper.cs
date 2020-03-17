// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DbLocalizationProvider.Internal;

namespace DbLocalizationProvider.Sync
{
    /// <summary>
    ///     This class can help you to discover types in assemblies and provide meta-data about found localizable resources
    /// </summary>
    public class TypeDiscoveryHelper
    {
        internal static ConcurrentDictionary<string, List<string>> DiscoveredResourceCache = new ConcurrentDictionary<string, List<string>>();
        internal static ConcurrentDictionary<string, string> UseResourceAttributeCache = new ConcurrentDictionary<string, string>();

        private readonly List<IResourceTypeScanner> _scanners = new List<IResourceTypeScanner>();

        public TypeDiscoveryHelper()
        {
            if (ConfigurationContext.Current.TypeScanners != null && ConfigurationContext.Current.TypeScanners.Any())
            {
                _scanners.AddRange(ConfigurationContext.Current.TypeScanners);
            }
            else
            {
                _scanners.Add(new LocalizedModelTypeScanner());
                _scanners.Add(new LocalizedResourceTypeScanner());
                _scanners.Add(new LocalizedEnumTypeScanner());
                _scanners.Add(new LocalizedForeignResourceTypeScanner());
            }
        }

        /// <summary>
        /// Scan assemblies and return discovered resources from target type
        /// </summary>
        /// <param name="target">Class to scan resources in</param>
        /// <param name="keyPrefix">Resource key prefix (if needed)</param>
        /// <param name="scanner">Which scanner to use to discover resources</param>
        /// <returns>Discovered resources from found assemblies</returns>
        public IEnumerable<DiscoveredResource> ScanResources(Type target, string keyPrefix = null, IResourceTypeScanner scanner = null)
        {
            var typeScanner = scanner;

            if (scanner == null) typeScanner = _scanners.FirstOrDefault(s => s.ShouldScan(target));
            if (typeScanner == null) return Enumerable.Empty<DiscoveredResource>();
            if (target.IsGenericParameter) return Enumerable.Empty<DiscoveredResource>();

            var resourceKeyPrefix = typeScanner.GetResourceKeyPrefix(target, keyPrefix);

            var buffer = new List<DiscoveredResource>();
            buffer.AddRange(typeScanner.GetClassLevelResources(target, resourceKeyPrefix));
            buffer.AddRange(typeScanner.GetResources(target, resourceKeyPrefix));

            var result = buffer.Where(t => t.IsIncluded()).ToList();

            foreach(var property in buffer.Where(t => t.IsComplex()))
            {
                if (!property.IsSimpleType)
                {
                    if (property.ReturnType != typeof(object) && property.ReturnType.IsAssignableFrom(target))
                    {
                        throw new RecursiveResourceReferenceException(
                            $"Property `{property.PropertyName}` in `{target.FullName}` has the same return type as enclosing class. This will result in StackOverflowException. Consider adding [Ignore] attribute.");
                    }

                    result.AddRange(ScanResources(property.DeclaringType, property.Key, typeScanner));
                }
            }

            // throw up if there are any duplicate resources manually registered
            var duplicateKeys = result.Where(r => r.FromResourceKeyAttribute).GroupBy(r => r.Key).Where(g => g.Count() > 1).ToList();

            if (duplicateKeys.Any()) throw new DuplicateResourceKeyException($"Duplicate keys: [{string.Join(", ", duplicateKeys.Select(g => g.Key))}]");

            // we need to filter out duplicate resources (this comes from the case when the same model is used in multiple places
            // in the same parent container type. for instance: billing address and office address. both of them will be registered
            // under Address container type - twice, one via billing context - another one via office address property).
            result = result.DistinctBy(r => r.Key).ToList();

            // add scanned resources to the cache
            DiscoveredResourceCache.TryAdd(target.FullName, result.Where(r => !string.IsNullOrEmpty(r.PropertyName)).Select(r => r.PropertyName).ToList());

            return result;
        }

        /// <summary>
        /// Returns found types (assemblies are limited by <see cref="ConfigurationContext.AssemblyScanningFilter"/>)
        /// </summary>
        /// <param name="filters">List of additional type filters (this is used to collect various types with single scan operation - sort of profiles)</param>
        /// <returns>List of found types for provided filters</returns>
        public static List<List<Type>> GetTypes(params Func<Type, bool>[] filters)
        {
            if (filters == null) throw new ArgumentNullException(nameof(filters));

            var result = new List<List<Type>>();
            for (var i = 0; i < filters.Length; i++)
            {
                result.Add(new List<Type>());
            }

            var assemblies = GetAssemblies(ConfigurationContext.Current.AssemblyScanningFilter, ConfigurationContext.Current.ScanAllAssemblies);
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

        /// <summary>
        ///  Finds types with specified attribute
        /// </summary>
        /// <typeparam name="T">Attribute type</typeparam>
        /// <returns>List of found types by specified attribute</returns>
        public static IEnumerable<Type> GetTypesWithAttribute<T>() where T : Attribute
        {
            return GetTypes(t => t.GetCustomAttribute<T>() != null).FirstOrDefault();
        }

        /// <summary>
        /// Finds all child classes of specified base class
        /// </summary>
        /// <typeparam name="T">Type of the base class</typeparam>
        /// <returns>Child classes of specified base class</returns>
        public static IEnumerable<Type> GetTypesChildOf<T>()
        {
            var allTypes = new List<Type>();
            foreach (var assembly in GetAssemblies(ConfigurationContext.Current.AssemblyScanningFilter, ConfigurationContext.Current.ScanAllAssemblies))
            {
                allTypes.AddRange(GetTypesChildOfInAssembly(typeof(T), assembly));
            }

            return allTypes;
        }

        internal static IEnumerable<Assembly> GetAssemblies(Func<Assembly, bool> assemblyFilter, bool scanAllAssemblies)
        {
            var allAssemblies = scanAllAssemblies ? GetAllAssemblies() : AppDomain.CurrentDomain.GetAssemblies();

            return allAssemblies.Where(a => a.FullName.StartsWith("DbLocalizationProvider"))
                                .Concat(allAssemblies.Where(assemblyFilter))
                                .Distinct();
        }

        private static Assembly[] GetAllAssemblies()
        {
            var list = new List<string>();
            var stack = new Stack<Assembly>();
            var result = new List<Assembly>();

            stack.Push(Assembly.GetEntryAssembly());

            do
            {
                var asm = stack.Pop();

                result.Add(asm);

                foreach (var reference in asm.GetReferencedAssemblies())
                    if (!list.Contains(reference.FullName))
                    {
                        stack.Push(Assembly.Load(reference));
                        list.Add(reference.FullName);
                    }
            }
            while (stack.Count > 0);

            return result.ToArray();
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

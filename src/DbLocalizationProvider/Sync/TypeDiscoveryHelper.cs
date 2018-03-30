// Copyright (c) 2018 Valdis Iljuconoks.
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Internal;

namespace DbLocalizationProvider.Sync
{
    internal class TypeDiscoveryHelper
    {
        internal static ConcurrentDictionary<string, List<string>> DiscoveredResourceCache = new ConcurrentDictionary<string, List<string>>();
        internal static ConcurrentDictionary<string, string> UseResourceAttributeCache = new ConcurrentDictionary<string, string>();

        private readonly List<IResourceTypeScanner> _scanners = new List<IResourceTypeScanner>();

        public TypeDiscoveryHelper()
        {
            if(ConfigurationContext.Current.TypeScanners != null && ConfigurationContext.Current.TypeScanners.Any())
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

        public IEnumerable<DiscoveredResource> ScanResources(Type target, string keyPrefix = null, IResourceTypeScanner scanner = null)
        {
            var typeScanner = scanner;

            if(scanner == null)
                typeScanner = _scanners.FirstOrDefault(s => s.ShouldScan(target));

            if(typeScanner == null)
                return Enumerable.Empty<DiscoveredResource>();

            if(target.IsGenericParameter)
                return Enumerable.Empty<DiscoveredResource>();

            var resourceKeyPrefix = typeScanner.GetResourceKeyPrefix(target, keyPrefix);

            var buffer = new List<DiscoveredResource>();
            buffer.AddRange(typeScanner.GetClassLevelResources(target, resourceKeyPrefix));
            buffer.AddRange(typeScanner.GetResources(target, resourceKeyPrefix));

            var result = buffer.Where(t => t.IsSimpleType || t.Info == null || t.Info.GetCustomAttribute<IncludeAttribute>() != null)
                               .ToList();

            foreach(var property in buffer.Where(t => !t.IsSimpleType))
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
                throw new ArgumentNullException(nameof(filters));

            var result = new List<List<Type>>();
            for(var i = 0; i < filters.Length; i++)
                result.Add(new List<Type>());

            var assemblies = GetAssemblies(ConfigurationContext.Current.AssemblyScanningFilter);
            foreach(var assembly in assemblies)
            {
                try
                {
                    var types = assembly.GetTypes();
                    for(var i = 0; i < filters.Length; i++)
                    {
                        result[i].AddRange(types.Where(filters[i]));
                    }
                }
                catch(Exception)
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
            foreach(var assembly in GetAssemblies(ConfigurationContext.Current.AssemblyScanningFilter))
            {
                allTypes.AddRange(GetTypesChildOfInAssembly(typeof(T), assembly));
            }

            return allTypes;
        }

        internal static IEnumerable<Assembly> GetAssemblies(Func<Assembly, bool> assemblyFilter)
        {
            var allAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            return allAssemblies.Where(a => a.FullName.StartsWith("DbLocalizationProvider"))
                                .Concat(allAssemblies.Where(assemblyFilter))
                                .Distinct();
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
            catch(Exception)
            {
                // there could be situations when type could not be loaded
                // this may happen if we are visiting *all* loaded assemblies in application domain
                return new List<Type>();
            }
        }
    }
}

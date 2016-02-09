using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DbLocalizationProvider.Sync
{
    internal class TypeDiscoveryHelper
    {
        internal static IEnumerable<Type> GetTypesWithAttribute<T>() where T : Attribute
        {
            return GetAssemblies().SelectMany(a => SelectTypes(a, t => t.GetCustomAttribute<T>() != null));
        }

        internal static IEnumerable<Type> GetTypesChildOf<T>()
        {
            var allTypes = new List<Type>();
            foreach (var assembly in GetAssemblies())
            {
                allTypes.AddRange(GetTypesChildOfInAssembly(typeof (T), assembly));
            }

            return allTypes;
        }

        internal static IEnumerable<Type> GetTypesOfInterface<T>()
        {
            var allTypes = new List<Type>();
            foreach (var assembly in GetAssemblies())
            {
                allTypes.AddRange(GetInterfacesInAssembly(typeof (T), assembly));
            }

            return allTypes;
        }

        internal static IEnumerable<Tuple<PropertyInfo, string>> GetAllProperties(Type type, string keyPrefix = null)
        {
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Static)
                                 .Select(pi => Tuple.Create(pi,
                                                            $"{(string.IsNullOrEmpty(keyPrefix) ? type.FullName : keyPrefix)}.{pi.Name}")).ToList();

            var buffer = new List<Tuple<PropertyInfo, string>>();

            foreach (var property in properties)
            {
                var pi = property.Item1;

                if (!IsSimple(pi.GetMethod.ReturnType))
                {
                    // if this is not a simple type - we need to scan deeper
                    buffer.AddRange(GetAllProperties(pi.PropertyType, property.Item2));
                }
            }

            properties.AddRange(buffer);
            return properties;
        }

        internal static bool IsStaticStringProperty(PropertyInfo info)
        {
            return info.GetGetMethod().IsStatic && info.GetGetMethod().ReturnType == typeof (string);
        }

        private static IEnumerable<Assembly> GetAssemblies()
        {
            return AppDomain.CurrentDomain.GetAssemblies();
        }

        private static IEnumerable<Type> GetTypesChildOfInAssembly(Type type, Assembly assembly)
        {
            return SelectTypes(assembly, t => t.IsSubclassOf(type) && !t.IsAbstract);
        }

        private static IEnumerable<Type> GetInterfacesInAssembly(Type @interface, Assembly assembly)
        {
            return SelectTypes(assembly, t => !t.IsAbstract && t.GetInterfaces().AsEnumerable().Contains(@interface));
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

        private static bool IsSimple(Type type)
        {
            return type.IsPrimitive
                   || type.IsEnum
                   || type == typeof (string)
                   || type == typeof (decimal);
        }
    }
}

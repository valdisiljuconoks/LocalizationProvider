using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DbLocalizationProvider.Sync
{
    internal class TypeDiscoveryHelper
    {
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

        public static IEnumerable<string> GetAllProperties(Type type)
        {
            return type.GetProperties(BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Static)
                       .Select(pi => $"{type.FullName}.{pi.Name}");
        }
    }
}

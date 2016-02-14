using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Castle.Core.Internal;

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

        internal static IEnumerable<Tuple<PropertyInfo, string, string>> GetAllProperties(Type type, string keyPrefix = null, bool contextAwareScanning = true)
        {
            var resourceKeyPrefix = type.FullName;
            if (contextAwareScanning)
            {
                resourceKeyPrefix = string.IsNullOrEmpty(keyPrefix) ? type.FullName : keyPrefix;
            }

            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Static)
                                 .Select(pi => Tuple.Create(pi,
                                                            $"{resourceKeyPrefix}.{pi.Name}",
                                                            GetResourceValue(pi, $"{resourceKeyPrefix}.{pi.Name}"))).ToList();

            var buffer = new List<Tuple<PropertyInfo, string, string>>();

            foreach (var property in properties)
            {
                var pi = property.Item1;

                if (!IsSimple(pi.GetMethod.ReturnType))
                {
                    // if this is not a simple type - we need to scan deeper
                    buffer.AddRange(GetAllProperties(pi.PropertyType, property.Item2, contextAwareScanning));
                }

                var validationAttributes = pi.GetAttributes<ValidationAttribute>();
                foreach (var validationAttribute in validationAttributes)
                {
                    var resourceValue = $"{property.Item2}-{validationAttribute.GetType().Name.Replace("Attribute", string.Empty)}";
                    buffer.Add(Tuple.Create(pi,
                                            resourceValue,
                                            string.IsNullOrEmpty(validationAttribute.ErrorMessage) ? resourceValue : validationAttribute.ErrorMessage));
                }
            }

            properties.AddRange(buffer);
            return properties;
        }

        private static string GetResourceValue(PropertyInfo pi, string defaultResourceValue)
        {
            var result = defaultResourceValue;
            var attributes = pi.GetCustomAttributes(true);
            var displayAttribute = attributes.OfType<DisplayAttribute>().FirstOrDefault();

            if (displayAttribute != null)
            {
                result = displayAttribute.GetName();
            }

            var displayNameAttribute = attributes.OfType<DisplayNameAttribute>().FirstOrDefault();
            if (displayNameAttribute != null)
            {
                result = displayNameAttribute.DisplayName;
            }

            return result;
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

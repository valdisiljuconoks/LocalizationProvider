using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Castle.Core.Internal;
using EPiServer.DataAnnotations;

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

        internal static IEnumerable<Type> GetTypesOfInterface(Type targeType)
        {
            var allTypes = new List<Type>();
            foreach (var assembly in GetAssemblies())
            {
                allTypes.AddRange(GetInterfacesInAssembly(targeType, assembly));
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
                                 .Where(pi => pi.GetCustomAttribute<IgnoreAttribute>() == null)
                                 .Select(pi => Tuple.Create(pi,
                                                            $"{resourceKeyPrefix}.{pi.Name}",
                                                            GetResourceValue(pi, pi.Name))).ToList();

            var buffer = new List<Tuple<PropertyInfo, string, string>>(properties.Where(t => IsSimple(t.Item1.GetMethod.ReturnType)
                                                                                             || t.Item1.GetCustomAttribute<IncludeAttribute>() != null));

            foreach (var property in properties)
            {
                var pi = property.Item1;
                var deeperModelType = pi.GetMethod.ReturnType;

                if (!IsSimple(deeperModelType))
                {
                    // if this is not a simple type - we need to scan deeper only if deeper model has attribute annotation
                    if (contextAwareScanning || deeperModelType.GetCustomAttribute<LocalizedModelAttribute>() != null)
                    {
                        buffer.AddRange(GetAllProperties(pi.PropertyType, property.Item2, contextAwareScanning));
                    }
                }

                var validationAttributes = pi.GetAttributes<ValidationAttribute>();
                foreach (var validationAttribute in validationAttributes)
                {
                    var resourceKey = $"{property.Item2}-{validationAttribute.GetType().Name.Replace("Attribute", string.Empty)}";
                    var resourceValue = resourceKey.Split('.').Last();
                    buffer.Add(Tuple.Create(pi,
                                            resourceKey,
                                            string.IsNullOrEmpty(validationAttribute.ErrorMessage) ? resourceValue : validationAttribute.ErrorMessage));
                }
            }

            //properties.AddRange(buffer);
            return buffer;
        }

        internal static bool IsStringProperty(MethodInfo info)
        {
            return info.ReturnType == typeof (string);
        }

        private static string GetResourceValue(PropertyInfo pi, string defaultResourceValue)
        {
            var result = defaultResourceValue;

            // try to extract resource value
            var methodInfo = pi.GetGetMethod();
            if (IsStringProperty(methodInfo))
            {
                try
                {
                    if (methodInfo.IsStatic)
                    {
                        result = methodInfo.Invoke(null, null) as string;
                    }
                    else
                    {
                        if (pi.DeclaringType != null)
                        {
                            var targetInstance = Activator.CreateInstance(pi.DeclaringType);
                            var propertyReturnValue = methodInfo.Invoke(targetInstance, null) as string;
                            if (propertyReturnValue != null)
                            {
                                result = propertyReturnValue;
                            }
                        }
                    }
                }
                catch
                {
                    // if we fail to retrieve value for the resource
                }
            }

            var attributes = pi.GetCustomAttributes(true);
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
            return SelectTypes(assembly,
                               t => !t.IsAbstract
                                    && t.GetInterfaces().AsEnumerable().Any(i =>
                                                                            {
                                                                                if (i.IsGenericType)
                                                                                {
                                                                                    return i.GetGenericTypeDefinition() == @interface;
                                                                                }

                                                                                return i == @interface;
                                                                            }));
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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Castle.Core.Internal;
using DbLocalizationProvider.DataAnnotations;
using EPiServer.DataAnnotations;

namespace DbLocalizationProvider.Sync
{
    internal class TypeDiscoveryHelper
    {
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

        internal static IEnumerable<DiscoveredResource> GetAllProperties(Type type, string keyPrefix = null, bool contextAwareScanning = true)
        {
            var resourceKeyPrefix = type.FullName;
            if(contextAwareScanning)
            {
                resourceKeyPrefix = string.IsNullOrEmpty(keyPrefix) ? type.FullName : keyPrefix;
            }

            List<DiscoveredResource> properties;
            if(type.BaseType == typeof(Enum))
            {
                properties = type.GetMembers(BindingFlags.Public | BindingFlags.Static)
                                 .Select(mi => new DiscoveredResource(mi,
                                                                      ResourceKeyBuilder.BuildResourceKey(resourceKeyPrefix, mi),
                                                                      mi.Name,
                                                                      type,
                                                                      Enum.GetUnderlyingType(type))).ToList();
            }
            else
            {
                properties = type.GetProperties(BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Static)
                                 .Where(pi => pi.GetCustomAttribute<IgnoreAttribute>() == null)
                                 .Select(pi => new DiscoveredResource(pi,
                                                                      ResourceKeyBuilder.BuildResourceKey(resourceKeyPrefix, pi),
                                                                      GetResourceValue(pi),
                                                                      pi.PropertyType,
                                                                      pi.GetMethod.ReturnType)).ToList();
            }

            var buffer = new List<DiscoveredResource>(properties.Where(t => IsSimple(t.ReturnType)
                                                                            || t.Info.GetCustomAttribute<IncludeAttribute>() != null));

            foreach (var property in properties)
            {
                var pi = property.Info;
                var deeperModelType = property.ReturnType;

                if(!IsSimple(deeperModelType))
                {
                    // if this is not a simple type - we need to scan deeper only if deeper model has attribute annotation
                    if(contextAwareScanning || deeperModelType.GetCustomAttribute<LocalizedModelAttribute>() != null)
                    {
                        buffer.AddRange(GetAllProperties(property.DeclaringType, property.Key, contextAwareScanning));
                    }
                }

                var validationAttributes = pi.GetAttributes<ValidationAttribute>();
                foreach (var validationAttribute in validationAttributes)
                {
                    var resourceKey = ModelMetadataLocalizationHelper.BuildResourceKey(property.Key, validationAttribute);
                    var resourceValue = resourceKey.Split('.').Last();
                    buffer.Add(new DiscoveredResource(pi,
                                                      resourceKey,
                                                      string.IsNullOrEmpty(validationAttribute.ErrorMessage) ? resourceValue : validationAttribute.ErrorMessage,
                                                      property.DeclaringType,
                                                      property.ReturnType));
                }
            }

            return buffer;
        }

        internal static bool IsStringProperty(Type returnType)
        {
            return returnType == typeof(string);
        }

        internal static bool IsSimple(Type type)
        {
            return PrimitiveTypes.IsPrimitive(type);
        }

        private static string GetResourceValue(PropertyInfo pi)
        {
            var result = pi.Name;

            // property definition contains ResourceKey attribute
            var keyAttribute = pi.GetCustomAttribute<ResourceKeyAttribute>();
            if(!string.IsNullOrEmpty(keyAttribute?.Value))
            {
                return keyAttribute.Value;
            }

            // try to extract resource value
            var methodInfo = pi.GetGetMethod();
            if(IsStringProperty(methodInfo.ReturnType))
            {
                try
                {
                    if(methodInfo.IsStatic)
                    {
                        result = methodInfo.Invoke(null, null) as string;
                    }
                    else
                    {
                        if(pi.DeclaringType != null)
                        {
                            var targetInstance = Activator.CreateInstance(pi.DeclaringType);
                            var propertyReturnValue = methodInfo.Invoke(targetInstance, null) as string;
                            if(propertyReturnValue != null)
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

            if(!string.IsNullOrEmpty(displayAttribute?.GetName()))
            {
                result = displayAttribute.GetName();
            }

            var displayNameAttribute = attributes.OfType<DisplayNameAttribute>().FirstOrDefault();
            if(!string.IsNullOrEmpty(displayNameAttribute?.DisplayName))
            {
                result = displayNameAttribute.DisplayName;
            }

            return result;
        }

        private static IEnumerable<Assembly> GetAssemblies()
        {
            return AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.FullName.StartsWith("Microsoft")
                                                                      || !a.FullName.StartsWith("System")
                                                                      || !a.FullName.StartsWith("EPiServer"));
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

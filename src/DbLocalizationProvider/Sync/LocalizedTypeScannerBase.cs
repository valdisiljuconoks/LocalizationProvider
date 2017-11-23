using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Abstractions.Refactoring;
using DbLocalizationProvider.Internal;

namespace DbLocalizationProvider.Sync
{
    internal abstract class LocalizedTypeScannerBase
    {
        public ICollection<DiscoveredResource> GetClassLevelResources(Type target, string resourceKeyPrefix)
        {
            var result = new List<DiscoveredResource>();
            var resourceAttributesOnModelClass = target.GetCustomAttributes<ResourceKeyAttribute>().ToList();
            if(!resourceAttributesOnModelClass.Any())
                return result;

            foreach(var resourceKeyAttribute in resourceAttributesOnModelClass)
            {
                result.Add(new DiscoveredResource(null,
                                                  ResourceKeyBuilder.BuildResourceKey(resourceKeyPrefix, resourceKeyAttribute.Key, separator: string.Empty),
                                                  DiscoveredTranslation.FromSingle(resourceKeyAttribute.Value),
                                                  resourceKeyAttribute.Value,
                                                  target,
                                                  typeof(string),
                                                  true));
            }

            return result;
        }

        protected ICollection<DiscoveredResource> DiscoverResourcesFromTypeMembers(
            Type target,
            ICollection<MemberInfo> members,
            string resourceKeyPrefix,
            bool typeKeyPrefixSpecified,
            bool isHidden,
            string typeOldName = null,
            string typeOldNamespace = null)
        {
            object typeInstance = null;

            try
            {
                typeInstance = Activator.CreateInstance(target);
            }
            catch(Exception e) { }

            return members.SelectMany(mi => DiscoverResourcesFromMember(target,
                                                                        typeInstance,
                                                                        mi,
                                                                        resourceKeyPrefix,
                                                                        typeKeyPrefixSpecified,
                                                                        isHidden,
                                                                        typeOldName,
                                                                        typeOldNamespace)).ToList();
        }

        private IEnumerable<DiscoveredResource> DiscoverResourcesFromMember(
            Type target,
            object instance,
            MemberInfo mi,
            string resourceKeyPrefix,
            bool typeKeyPrefixSpecified,
            bool isHidden,
            string typeOldName = null,
            string typeOldNamespace = null)
        {
            // check if there are [ResourceKey] attributes
            var keyAttributes = mi.GetCustomAttributes<ResourceKeyAttribute>().ToList();
            var resourceKey = ResourceKeyBuilder.BuildResourceKey(resourceKeyPrefix, mi.Name);
            var translation = GetResourceValue(instance, mi);

            Type declaringType = null;
            Type returnType = null;
            var isSimpleType = false;

            if(mi is PropertyInfo)
            {
                var info = (PropertyInfo)mi;
                declaringType = info.PropertyType;
                returnType = info.GetMethod.ReturnType;
                isSimpleType = returnType.IsSimpleType();
            }
            else if(mi is FieldInfo)
            {
                var info = (FieldInfo)mi;
                declaringType = info.GetUnderlyingType();
                returnType = info.GetUnderlyingType();
                isSimpleType = returnType.IsSimpleType();
            }

            if(!keyAttributes.Any())
            {
                var isResourceHidden = isHidden || mi.GetCustomAttribute<HiddenAttribute>() != null;

                // try to understand if there is resource "redirect" - [UseResource(..)]
                var resourceRef = mi.GetCustomAttribute<UseResourceAttribute>();
                if(resourceRef != null)
                {
                    TypeDiscoveryHelper.UseResourceAttributeCache.TryAdd(resourceKey, ResourceKeyBuilder.BuildResourceKey(resourceRef.TargetContainer, resourceRef.PropertyName));
                }
                else
                {
                    var translations = DiscoveredTranslation.FromSingle(translation);

                    var additionalTranslationsAttributes = mi.GetCustomAttributes<TranslationForCultureAttribute>();

                    if(additionalTranslationsAttributes != null && additionalTranslationsAttributes.Any())
                        translations.AddRange(additionalTranslationsAttributes.Select(a => new DiscoveredTranslation(a.Translation, a.Culture)));

                    var oldResourceKeys = GenerateOldResourceKey(target, mi.Name, mi, resourceKeyPrefix, typeOldName, typeOldNamespace);

                    yield return new DiscoveredResource(mi,
                                                        resourceKey,
                                                        translations,
                                                        mi.Name,
                                                        declaringType,
                                                        returnType,
                                                        isSimpleType,
                                                        isResourceHidden)
                                 {
                                     TypeName = target.Name,
                                     TypeNamespace = target.Namespace,
                                     TypeOldName = oldResourceKeys.Item2,
                                     TypeOldNamespace = typeOldNamespace,
                                     OldResourceKey = oldResourceKeys.Item1
                                 };
                }

                // try to fetch also [Display()] attribute to generate new "...-Description" resource => usually used for help text labels
                var displayAttribute = mi.GetCustomAttribute<DisplayAttribute>();
                if(displayAttribute?.Description != null)
                {
                    var propertyName = $"{mi.Name}-Description";
                    var oldResourceKeys = GenerateOldResourceKey(target, propertyName, mi, resourceKeyPrefix, typeOldName, typeOldNamespace);
                    yield return new DiscoveredResource(mi,
                                                        $"{resourceKey}-Description",
                                                        DiscoveredTranslation.FromSingle(displayAttribute.Description),
                                                        propertyName,
                                                        declaringType,
                                                        returnType,
                                                        isSimpleType)
                                 {
                                     TypeName = target.Name,
                                     TypeNamespace = target.Namespace,
                                     TypeOldName = oldResourceKeys.Item2,
                                     TypeOldNamespace = typeOldNamespace,
                                     OldResourceKey = oldResourceKeys.Item1
                                 };
                }

                var validationAttributes = mi.GetCustomAttributes<ValidationAttribute>();
                foreach(var validationAttribute in validationAttributes)
                {
                    if(validationAttribute.GetType() == typeof(DataTypeAttribute))
                        continue;

                    var validationResourceKey = ResourceKeyBuilder.BuildResourceKey(resourceKey, validationAttribute);
                    var propertyName = validationResourceKey.Split('.').Last();

                    var oldResourceKeys = GenerateOldResourceKey(target, propertyName, mi, resourceKeyPrefix, typeOldName, typeOldNamespace);

                    yield return new DiscoveredResource(mi,
                                                        validationResourceKey,
                                                        DiscoveredTranslation.FromSingle(string.IsNullOrEmpty(validationAttribute.ErrorMessage)
                                                                                             ? propertyName
                                                                                             : validationAttribute.ErrorMessage),
                                                        propertyName,
                                                        declaringType,
                                                        returnType,
                                                        isSimpleType)
                                 {
                                     TypeName = target.Name,
                                     TypeNamespace = target.Namespace,
                                     TypeOldName = oldResourceKeys.Item2,
                                     TypeOldNamespace = typeOldNamespace,
                                     OldResourceKey = oldResourceKeys.Item1
                                 };
                }

                // scan custom registered attributes (if any)
                foreach(var descriptor in ConfigurationContext.Current.CustomAttributes.ToList())
                {
                    var customAttributes = mi.GetCustomAttributes(descriptor.CustomAttribute);
                    foreach(var customAttribute in customAttributes)
                    {
                        var customAttributeKey = ResourceKeyBuilder.BuildResourceKey(resourceKey, customAttribute);
                        var propertyName = customAttributeKey.Split('.').Last();
                        var oldResourceKeys = GenerateOldResourceKey(target, propertyName, mi, resourceKeyPrefix, typeOldName, typeOldNamespace);
                        var foreignTranslation = string.Empty;
                        if(descriptor.GenerateTranslation)
                        {
                            var z1 = customAttribute.GetType().ToString();
                            var z2 = customAttribute.ToString();

                            foreignTranslation = !z1.Equals(z2) ? z2 : propertyName;
                        }

                        yield return new DiscoveredResource(mi,
                                                            customAttributeKey,
                                                            DiscoveredTranslation.FromSingle(foreignTranslation),
                                                            propertyName,
                                                            declaringType,
                                                            returnType,
                                                            isSimpleType)
                                     {
                                         TypeName = target.Name,
                                         TypeNamespace = target.Namespace,
                                         TypeOldName = oldResourceKeys.Item2,
                                         TypeOldNamespace = typeOldNamespace,
                                         OldResourceKey = oldResourceKeys.Item1
                                     };
                    }
                }
            }

            foreach(var resourceKeyAttribute in keyAttributes)
            {
                yield return new DiscoveredResource(mi,
                                                    ResourceKeyBuilder.BuildResourceKey(typeKeyPrefixSpecified ? resourceKeyPrefix : null,
                                                                                        resourceKeyAttribute.Key,
                                                                                        separator: string.Empty),
                                                    DiscoveredTranslation.FromSingle(string.IsNullOrEmpty(resourceKeyAttribute.Value) ? translation : resourceKeyAttribute.Value),
                                                    null,
                                                    declaringType,
                                                    returnType,
                                                    true)
                             {
                                 FromResourceKeyAttribute = true
                             };
            }
        }

        private static Tuple<string, string> GenerateOldResourceKey(
            Type target,
            string property,
            MemberInfo mi,
            string resourceKeyPrefix,
            string typeOldName,
            string typeOldNamespace)
        {
            string oldResourceKey = null;
            var propertyName = property;
            var finalOldTypeName = typeOldName;

            var refactoringAttribute = mi.GetCustomAttribute<RenamedResourceAttribute>();
            if(refactoringAttribute != null)
            {
                finalOldTypeName = propertyName = property.Replace(mi.Name, refactoringAttribute.OldName);
                oldResourceKey = ResourceKeyBuilder.BuildResourceKey(resourceKeyPrefix, propertyName);
            }

            if(!string.IsNullOrEmpty(typeOldName) && string.IsNullOrEmpty(typeOldNamespace))
            {
                oldResourceKey = ResourceKeyBuilder.BuildResourceKey(ResourceKeyBuilder.BuildResourceKey(target.Namespace, typeOldName), propertyName);
                // special treatment for the nested resources
                if(target.IsNested)
                {
                    oldResourceKey = ResourceKeyBuilder.BuildResourceKey(target.FullName.Replace(target.Name, typeOldName), propertyName);
                    var declaringTypeRefacotringInfo = target.DeclaringType.GetCustomAttribute<RenamedResourceAttribute>();
                    if(declaringTypeRefacotringInfo != null)
                    {
                        if(!string.IsNullOrEmpty(declaringTypeRefacotringInfo.OldName) && string.IsNullOrEmpty(declaringTypeRefacotringInfo.OldNamespace))
                        {
                            oldResourceKey =
                                ResourceKeyBuilder.BuildResourceKey(ResourceKeyBuilder.BuildResourceKey(target.Namespace, $"{declaringTypeRefacotringInfo.OldName}+{typeOldName}"),
                                                                    propertyName);
                        }

                        if(!string.IsNullOrEmpty(declaringTypeRefacotringInfo.OldName) && !string.IsNullOrEmpty(declaringTypeRefacotringInfo.OldNamespace))
                        {
                            oldResourceKey = ResourceKeyBuilder.BuildResourceKey(ResourceKeyBuilder.BuildResourceKey(declaringTypeRefacotringInfo.OldNamespace,
                                                                                                                     $"{declaringTypeRefacotringInfo.OldName}+{typeOldName}"),
                                                                                 propertyName);
                        }
                    }
                }
            }

            if(string.IsNullOrEmpty(typeOldName) && !string.IsNullOrEmpty(typeOldNamespace))
            {
                oldResourceKey = ResourceKeyBuilder.BuildResourceKey(ResourceKeyBuilder.BuildResourceKey(typeOldNamespace, target.Name), propertyName);
                // special treatment for the nested resources
                if(target.IsNested)
                    oldResourceKey = ResourceKeyBuilder.BuildResourceKey(target.FullName.Replace(target.Namespace, typeOldNamespace), propertyName);
            }

            if(!string.IsNullOrEmpty(typeOldName) && !string.IsNullOrEmpty(typeOldNamespace))
            {
                oldResourceKey = ResourceKeyBuilder.BuildResourceKey(ResourceKeyBuilder.BuildResourceKey(typeOldNamespace, typeOldName), propertyName);
                // special treatment for the nested resources
                if(target.IsNested)
                    oldResourceKey = ResourceKeyBuilder.BuildResourceKey(target.FullName.Replace(target.Namespace, typeOldNamespace).Replace(target.Name, typeOldName),
                                                                         propertyName);
            }

            return new Tuple<string, string>(oldResourceKey, finalOldTypeName);
        }

        private static string GetResourceValue(object instance, MemberInfo mi)
        {
            var result = mi.Name;

            if(mi is PropertyInfo)
            {
                // try to extract resource value from property
                var info = (PropertyInfo)mi;
                var methodInfo = info.GetGetMethod();
                if(IsStringProperty(methodInfo.ReturnType))
                {
                    try
                    {
                        if(!methodInfo.IsStatic)
                        {
                            if(mi.DeclaringType != null && instance != null)
                            {
                                var propertyValue = methodInfo.Invoke(instance, null) as string;
                                if(propertyValue != null)
                                    result = propertyValue;
                            }
                        }
                        else
                            result = methodInfo.Invoke(null, null) as string ?? result;
                    }
                    catch
                    {
                        // if we fail to retrieve value for the resource - fair enough
                    }
                }
            }
            else if(mi is FieldInfo)
            {
                // try to extract resource value from field
                var info = (FieldInfo)mi;
                if(info.IsStatic)
                    result = info.GetValue(null) as string ?? result;
                else
                {
                    if(instance != null)
                    {
                        var fieldValue = info.GetValue(instance) as string;
                        if(fieldValue != null)
                            result = fieldValue;
                    }
                }
            }

            var attributes = mi.GetCustomAttributes(true);
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

        internal static bool IsStringProperty(Type returnType)
        {
            return returnType == typeof(string);
        }
    }
}

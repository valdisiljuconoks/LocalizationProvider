using System;
using System.Reflection;
using DbLocalizationProvider.Abstractions.Refactoring;
using DbLocalizationProvider.Internal;

namespace DbLocalizationProvider.Refactoring
{
    internal class OldResourceKeyBuilder
    {
        internal static Tuple<string, string> GenerateOldResourceKey(
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
    }
}

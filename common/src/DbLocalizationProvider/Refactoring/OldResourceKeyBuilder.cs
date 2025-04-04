// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Reflection;
using DbLocalizationProvider.Abstractions.Refactoring;

namespace DbLocalizationProvider.Refactoring;

internal class OldResourceKeyBuilder(ResourceKeyBuilder keyBuilder)
{
    internal Tuple<string, string> GenerateOldResourceKey(
        Type target,
        string property,
        MemberInfo mi,
        string resourceKeyPrefix,
        string typeOldName,
        string typeOldNamespace)
    {
        var oldResourceKey = string.Empty;
        var propertyName = property;
        var finalOldTypeName = typeOldName;

        var refactoringAttribute = mi.GetCustomAttribute<RenamedResourceAttribute>();
        if (refactoringAttribute != null)
        {
            finalOldTypeName = propertyName = property.Replace(mi.Name, refactoringAttribute.OldName);
            oldResourceKey = BuildKey(resourceKeyPrefix, propertyName);
        }

        if (!string.IsNullOrEmpty(typeOldName) && string.IsNullOrEmpty(typeOldNamespace))
        {
            oldResourceKey = BuildKey(BuildKey(target.Namespace!, typeOldName), propertyName);

            // special treatment for the nested resources
            if (target.IsNested)
            {
                oldResourceKey = BuildKey(target.FullName!.Replace(target.Name, typeOldName), propertyName);
                var declaringTypeRefactoringInfo = target.DeclaringType!.GetCustomAttribute<RenamedResourceAttribute>();
                if (declaringTypeRefactoringInfo != null)
                {
                    if (!string.IsNullOrEmpty(declaringTypeRefactoringInfo.OldName)
                        && string.IsNullOrEmpty(declaringTypeRefactoringInfo.OldNamespace))
                    {
                        oldResourceKey = BuildKey(
                            BuildKey(target.Namespace!, $"{declaringTypeRefactoringInfo.OldName}+{typeOldName}"),
                            propertyName);
                    }

                    if (!string.IsNullOrEmpty(declaringTypeRefactoringInfo.OldName)
                        && !string.IsNullOrEmpty(declaringTypeRefactoringInfo.OldNamespace))
                    {
                        oldResourceKey = BuildKey(
                            BuildKey(declaringTypeRefactoringInfo.OldNamespace,
                                     $"{declaringTypeRefactoringInfo.OldName}+{typeOldName}"),
                            propertyName);
                    }
                }
            }
        }

        if (string.IsNullOrEmpty(typeOldName) && !string.IsNullOrEmpty(typeOldNamespace))
        {
            oldResourceKey = BuildKey(BuildKey(typeOldNamespace, target.Name), propertyName);

            // special treatment for the nested resources
            if (target.IsNested)
            {
                oldResourceKey = BuildKey(target.FullName!.Replace(target.Namespace!, typeOldNamespace), propertyName);
            }
        }

        if (!string.IsNullOrEmpty(typeOldName) && !string.IsNullOrEmpty(typeOldNamespace))
        {
            oldResourceKey = BuildKey(BuildKey(typeOldNamespace, typeOldName), propertyName);

            // special treatment for the nested resources
            if (target.IsNested)
            {
                oldResourceKey = BuildKey(
                    target.FullName!.Replace(target.Namespace!, typeOldNamespace).Replace(target.Name, typeOldName),
                    propertyName);
            }
        }

        return new Tuple<string, string>(oldResourceKey, finalOldTypeName);
    }

    private string BuildKey(string resourceKeyPrefix, string propertyName)
    {
        return keyBuilder.BuildResourceKey(resourceKeyPrefix, propertyName);
    }
}

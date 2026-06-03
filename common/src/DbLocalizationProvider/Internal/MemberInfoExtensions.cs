// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Reflection;

namespace DbLocalizationProvider.Internal;

internal static class MemberInfoExtensions
{
    public static Type GetUnderlyingType(this MemberInfo member)
    {
        return member.MemberType switch
        {
            MemberTypes.TypeInfo => (TypeInfo)member,
            MemberTypes.Field => ((FieldInfo)member).FieldType,
            MemberTypes.Property => ((PropertyInfo)member).DeclaringType!,
            _ => throw new ArgumentException("Input MemberInfo must be of type FieldInfo or PropertyInfo")
        };
    }
}

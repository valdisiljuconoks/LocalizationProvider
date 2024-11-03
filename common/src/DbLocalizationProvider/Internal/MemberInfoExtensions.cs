// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Reflection;

namespace DbLocalizationProvider.Internal;

internal static class MemberInfoExtensions
{
    public static Type GetUnderlyingType(this MemberInfo member)
    {
        switch (member.MemberType)
        {
            case MemberTypes.TypeInfo:
                return (TypeInfo)member;
            case MemberTypes.Field:
                return ((FieldInfo)member).FieldType;
            case MemberTypes.Property:
                return ((PropertyInfo)member).DeclaringType;
            default:
                throw new ArgumentException("Input MemberInfo must be of type FieldInfo or PropertyInfo");
        }
    }
}

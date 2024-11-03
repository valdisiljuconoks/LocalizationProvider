// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Reflection;

namespace DbLocalizationProvider.AspNetCore;

/// <summary>
/// Some reflection related extensions on <see cref="object" /> type.
/// </summary>
internal static class ObjectExtensions
{
    /// <summary>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="instance"></param>
    /// <param name="fieldName"></param>
    /// <returns></returns>
    internal static T GetField<T>(this object instance, string fieldName) where T : class
    {
        var bindFlags = BindingFlags.Instance | BindingFlags.NonPublic;
        var field = instance.GetType().GetField(fieldName, bindFlags);

        return field?.GetValue(instance) as T;
    }

    internal static T GetField<T, TBase>(this object instance, string fieldName) where T : class
    {
        var bindFlags = BindingFlags.Instance | BindingFlags.NonPublic;
        var field = typeof(TBase).GetField(fieldName, bindFlags);

        return field?.GetValue(instance) as T;
    }

    internal static void SetField<T>(this object instance, string fieldName, object value)
    {
        var bindFlags = BindingFlags.Instance | BindingFlags.NonPublic;
        var field = typeof(T).GetField(fieldName, bindFlags);

        field?.SetValue(instance, value);
    }
}

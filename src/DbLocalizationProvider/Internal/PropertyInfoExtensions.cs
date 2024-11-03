using System;
using System.Linq;
using System.Reflection;
using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Internal;

/// <summary>
/// Some extensions for properties
/// </summary>
public static class PropertyInfoExtensions
{
    /// <summary>
    /// Checks whether property is hidden or not.
    /// </summary>
    /// <param name="property">Property to check for.</param>
    /// <returns><c>true</c> if property is hidden, otherwise <c>false</c>.</returns>
    public static bool IsHidden(this PropertyInfo property)
    {
        return Attribute.GetCustomAttributes(property).FirstOrDefault(a => a is HiddenAttribute) != null;
    }
}
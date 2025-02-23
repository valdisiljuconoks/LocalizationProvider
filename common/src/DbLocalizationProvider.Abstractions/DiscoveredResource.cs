// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace DbLocalizationProvider.Abstractions;

/// <summary>
/// Class is used to describe resource found during scanning process. It clearly is having way more properties as LocalizedResource (metadata
/// luggage you know?!).
/// </summary>
[DebuggerDisplay("Key: {Key}, Translations: {Translations.Count}")]
public class DiscoveredResource
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DiscoveredResource"/> class.
    /// </summary>
    /// <param name="info">The member information.</param>
    /// <param name="key">The resource key.</param>
    /// <param name="translations">The translations for the resource.</param>
    /// <param name="propertyName">The property name, if applicable.</param>
    /// <param name="declaringType">The type that declares the resource.</param>
    /// <param name="returnType">The return type of the resource.</param>
    /// <param name="isSimpleType">Indicates whether the resource is of a simple type.</param>
    /// <param name="isHidden">Indicates whether the resource is hidden.</param>
    public DiscoveredResource(
        MemberInfo info,
        string key,
        ICollection<DiscoveredTranslation> translations,
        string? propertyName,
        Type declaringType,
        Type returnType,
        bool isSimpleType,
        bool isHidden = false)
    {
        Info = info;
        Key = key;
        Translations = translations;
        PropertyName = propertyName;
        DeclaringType = declaringType;
        ReturnType = returnType;
        IsSimpleType = isSimpleType;
        IsHidden = isHidden;
    }

    /// <summary>
    /// Gets the resource key.
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// Gets the translations for the resource.
    /// </summary>
    public ICollection<DiscoveredTranslation> Translations { get; }

    /// <summary>
    /// Gets the property name, if applicable.
    /// </summary>
    public string? PropertyName { get; }

    /// <summary>
    /// Gets the type that declares the resource.
    /// </summary>
    public Type DeclaringType { get; }

    /// <summary>
    /// Gets the return type of the resource.
    /// </summary>
    public Type ReturnType { get; }

    /// <summary>
    /// Gets a value indicating whether the resource is of a simple type.
    /// </summary>
    public bool IsSimpleType { get; }

    /// <summary>
    /// Gets the member information.
    /// </summary>
    public MemberInfo? Info { get; }

    /// <summary>
    /// Gets or sets a value indicating whether the resource key is from a resource key attribute.
    /// </summary>
    public bool FromResourceKeyAttribute { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the resource is hidden.
    /// </summary>
    public bool IsHidden { get; set; }

    /// <summary>
    /// Gets or sets the old name of the type.
    /// </summary>
    public string? TypeOldName { get; set; }

    /// <summary>
    /// Gets or sets the old namespace of the type.
    /// </summary>
    public string? TypeOldNamespace { get; set; }

    /// <summary>
    /// Gets or sets the name of the type.
    /// </summary>
    public string? TypeName { get; set; }

    /// <summary>
    /// Gets or sets the namespace of the type.
    /// </summary>
    public string? TypeNamespace { get; set; }

    /// <summary>
    /// Gets or sets the old resource key.
    /// </summary>
    public string? OldResourceKey { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the resource is included explicitly.
    /// </summary>
    public bool? IncludedExplicitly { get; set; }

    /// <summary>
    /// Determines whether the resource is included.
    /// </summary>
    /// <returns><c>true</c> if the resource is included; otherwise, <c>false</c>.</returns>
    public bool IsIncluded()
    {
        return IsSimpleType || Info == null || Info.GetCustomAttribute<IncludeAttribute>() != null || (IncludedExplicitly.HasValue && IncludedExplicitly.Value);
    }

    /// <summary>
    /// Determines whether the resource is complex.
    /// </summary>
    /// <returns><c>true</c> if the resource is complex; otherwise, <c>false</c>.</returns>
    public bool IsComplex()
    {
        return !IsSimpleType && (!IncludedExplicitly.HasValue || !IncludedExplicitly.Value);
    }
}

// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;

namespace DbLocalizationProvider;

/// <summary>
/// You can add custom attributes to the collection of known types (like <c>[Required]</c>) to scan for.
/// </summary>
public class CustomAttributeDescriptor
{
    /// <summary>
    /// Creates new instance of this type.
    /// </summary>
    /// <param name="target">Please specify type of the custom attribute you would like to look for and register.</param>
    /// <param name="generateTranslation">
    /// If you will set this to <c>true</c> translation will be created for discovered
    /// resources; otherwise - translation will not be created (even if <see cref="object.ToString" /> method will be
    /// overwritten.
    /// </param>
    public CustomAttributeDescriptor(Type target, bool generateTranslation = true)
    {
        if (target == null)
        {
            throw new ArgumentNullException(nameof(target));
        }

        if (!typeof(Attribute).IsAssignableFrom(target))
        {
            throw new ArgumentException($"Given type `{target.FullName}` is not of type `System.Attribute`");
        }

        CustomAttribute = target;
        GenerateTranslation = generateTranslation;
    }

    /// <summary>
    /// Target type to scan and register resources from.
    /// </summary>
    public Type CustomAttribute { get; set; }

    /// <summary>
    /// Flag indicating whether we need to generate also translations. Set this flag to <c>true</c> if you want default translation also be added
    /// to the storage for these "foreign" resources.
    /// </summary>
    public bool GenerateTranslation { get; }
}

/// <summary>
/// Extensions for collection of <see cref="CustomAttributeDescriptor" />.
/// </summary>
public static class CustomAttributeDescriptorCollectionExtensions
{
    /// <summary>
    /// Add new foreign resource descriptor to the collection.
    /// </summary>
    /// <param name="target">Target collection of already discovered / registered foreign resources.</param>
    /// <param name="customAttribute">Type of the attribute to add to the collection.</param>
    /// <param name="generateTranslation">
    /// If you will set this to <c>true</c> translation will be created for discovered
    /// resources; otherwise - translation will not be created (even if <see cref="object.ToString" /> method will be
    /// overwritten.
    /// </param>
    /// <returns>The same collection so you can do fluent stuff.</returns>
    public static ICollection<CustomAttributeDescriptor> Add(
        this ICollection<CustomAttributeDescriptor> target,
        Type customAttribute,
        bool generateTranslation = true)
    {
        target.Add(new CustomAttributeDescriptor(customAttribute, generateTranslation));

        return target;
    }

    /// <summary>
    /// Add new foreign resource descriptor (by given type specified in <typeparamref name="T" />) to the collection.
    /// </summary>
    /// <typeparam name="T">Type of the attribute to add to the collection.</typeparam>
    /// <param name="target">Target collection of already discovered / registered foreign resources.</param>
    /// <param name="generateTranslation">
    /// If you will set this to <c>true</c> translation will be created for discovered
    /// resources; otherwise - translation will not be created (even if <see cref="object.ToString" /> method will be
    /// overwritten.
    /// </param>
    /// <returns>The same collection so you can do fluent stuff.</returns>
    public static ICollection<CustomAttributeDescriptor> Add<T>(
        this ICollection<CustomAttributeDescriptor> target,
        bool generateTranslation = true)
    {
        target.Add(new CustomAttributeDescriptor(typeof(T), generateTranslation));

        return target;
    }
}

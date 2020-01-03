// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;

namespace DbLocalizationProvider
{
    /// <summary>
    ///     You can add custom attributes to the collection of known types to scan for using this class.
    /// </summary>
    public class CustomAttributeDescriptor
    {
        /// <summary>
        ///     Creates new instance of this type.
        /// </summary>
        /// <param name="target">Please specify type of the custom attribute you would like to look for and register.</param>
        /// <param name="generateTranslation">
        ///     If you will set this to <c>true</c> translation will be created for discovered
        ///     resources; otherwise - translation will not be created (even if <see cref="object.ToString" /> method will be
        ///     overwritten.
        /// </param>
        public CustomAttributeDescriptor(Type target, bool generateTranslation = true)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (!typeof(Attribute).IsAssignableFrom(target)) throw new ArgumentException($"Given type `{target.FullName}` is not of type `System.Attribute`");

            CustomAttribute = target;
            GenerateTranslation = generateTranslation;
        }

        public Type CustomAttribute { get; set; }

        public bool GenerateTranslation { get; }
    }

    public static class CustomAttributeDescriptorCollectionExtensions
    {
        public static ICollection<CustomAttributeDescriptor> Add(this ICollection<CustomAttributeDescriptor> target, Type customAttribute, bool generateTranslation = true)
        {
            target.Add(new CustomAttributeDescriptor(customAttribute, generateTranslation));

            return target;
        }

        public static ICollection<CustomAttributeDescriptor> Add<T>(this ICollection<CustomAttributeDescriptor> target, bool generateTranslation = true)
        {
            target.Add(new CustomAttributeDescriptor(typeof(T), generateTranslation));

            return target;
        }
    }
}

// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using System.Linq;
using DbLocalizationProvider.Internal;

namespace DbLocalizationProvider
{
    /// <summary>
    ///     Use this class if you would like to include "foreign" types in scanning and resource discovery process.
    ///     Foreign resources here means types that are not decorated with either <see cref="LocalizedResourceAttribute" /> or
    ///     <see cref="LocalizedModelAttribute" /> attributes.
    ///     Foreign resources usually are located in assemblies to which you don't have access to the source code.
    /// </summary>
    public class ForeignResourceDescriptor
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ForeignResourceDescriptor" /> class.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <exception cref="ArgumentNullException">target</exception>
        public ForeignResourceDescriptor(Type target)
        {
            ResourceType = target ?? throw new ArgumentNullException(nameof(target));
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ForeignResourceDescriptor" /> class.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="includeComplexProperties">if set to <c>true</c> [include complex properties].</param>
        /// <exception cref="ArgumentNullException">target</exception>
        public ForeignResourceDescriptor(Type target, bool includeComplexProperties)
        {
            ResourceType = target ?? throw new ArgumentNullException(nameof(target));
            IncludeComplexProperties = includeComplexProperties;
        }

        /// <summary>
        ///     Target type that contains resources (properties or fields).
        /// </summary>
        public Type ResourceType { get; }

        /// <summary>
        ///     This is handy in cases when you don't have access to source code of the resource class (which is obvious in foreign
        ///     resources case) and want to include complex properties as resources.
        ///     Then just add foreign resource descriptor with this flag set to <c>true</c>.
        /// </summary>
        public bool IncludeComplexProperties { get; }
    }

    /// <summary>
    ///     Static extension class
    /// </summary>
    public static class ICollectionOfForeignResourceDescriptorExtensions
    {
        /// <summary>
        ///     Adds the specified type to the foreign resource collection.
        /// </summary>
        /// <param name="collection">The foreign resource collection.</param>
        /// <param name="target">The foreign resource class.</param>
        /// <returns>The same list to support API chaining</returns>
        public static ICollection<ForeignResourceDescriptor> Add(this ICollection<ForeignResourceDescriptor> collection, Type target)
        {
            collection.Add(new ForeignResourceDescriptor(target));

            return collection;
        }

        /// <summary>
        ///     Adds the specified type to the foreign resource collection.
        /// </summary>
        /// <typeparam name="T">Type of the foreign resource</typeparam>
        /// <param name="collection">The foreign resource collection.</param>
        /// <returns>The same list to support API chaining</returns>
        public static ICollection<ForeignResourceDescriptor> Add<T>(this ICollection<ForeignResourceDescriptor> collection)
        {
            collection.Add(new ForeignResourceDescriptor(typeof(T)));

            return collection;
        }

        /// <summary>
        ///     Adds range of specified types to the foreign resource collection.
        /// </summary>
        /// <param name="collection">The collection of foreign resources.</param>
        /// <param name="targets">The list of foreign resource types.</param>
        public static void AddRange(this ICollection<ForeignResourceDescriptor> collection, IEnumerable<Type> targets)
        {
            targets.ForEach(t => collection.Add(new ForeignResourceDescriptor(t)));
        }

        /// <summary>
        ///     Gets the specified foreign resource type.
        /// </summary>
        /// <param name="collection">The collection of foreign resource types.</param>
        /// <param name="target">The target.</param>
        /// <returns></returns>
        public static ForeignResourceDescriptor Get(this ICollection<ForeignResourceDescriptor> collection, Type target)
        {
            return collection.FirstOrDefault(_ => _.ResourceType == target);
        }
    }
}

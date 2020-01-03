// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
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
        public ForeignResourceDescriptor(Type target)
        {
            ResourceType = target ?? throw new ArgumentNullException(nameof(target));
        }

        /// <summary>
        ///     Target type that contains resources (properties or fields).
        /// </summary>
        public Type ResourceType { get; }
    }

    public static class ICollectionOfForeignResourceDescriptorExtensions
    {
        public static ICollection<ForeignResourceDescriptor> Add(this ICollection<ForeignResourceDescriptor> collection, Type target)
        {
            collection.Add(new ForeignResourceDescriptor(target));

            return collection;
        }

        public static ICollection<ForeignResourceDescriptor> Add<T>(this ICollection<ForeignResourceDescriptor> collection)
        {
            collection.Add(new ForeignResourceDescriptor(typeof(T)));

            return collection;
        }

        public static void AddRange(this ICollection<ForeignResourceDescriptor> collection, IEnumerable<Type> targets)
        {
            targets.ForEach(t => collection.Add(new ForeignResourceDescriptor(t)));
        }
    }
}

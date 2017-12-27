// Copyright © 2017 Valdis Iljuconoks.
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using DbLocalizationProvider.Internal;

namespace DbLocalizationProvider
{
    /// <summary>
    /// Use this class if you would like to include "foreign" types in scanning and resource discovery process.
    /// Foreign resources here means types that are not decorated with either <see cref="LocalizedResourceAttribute"/> or <see cref="LocalizedModelAttribute"/> attributes.
    /// Foreign resources usually are located in assemblies to which you don't have access to the source code.
    /// </summary>
    public class ForeignResourceDescriptor
    {
        public ForeignResourceDescriptor(Type target)
        {
            ResourceType = target ?? throw new ArgumentNullException(nameof(target));
        }

        /// <summary>
        /// Target type that contains resources (properties or fields).
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

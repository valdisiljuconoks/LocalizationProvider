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

namespace DbLocalizationProvider
{
    /// <summary>
    /// You can add custom attributes to the collection of known types to scan for using this class.
    /// </summary>
    public class CustomAttributeDescriptor
    {
        /// <summary>
        /// Creates new instance of this type.
        /// </summary>
        /// <param name="target">Please specify type of the custom atttribute you would like to look for and register.</param>
        /// <param name="generateTranslation">If you will set this to <c>true</c> translation will be created for discovered resources; otherwise - translation will not be created (even if <see cref="object.ToString"/> method will be overwritten.</param>
        public CustomAttributeDescriptor(Type target, bool generateTranslation = true)
        {
            if(target == null)
                throw new ArgumentNullException(nameof(target));

            if(!typeof(Attribute).IsAssignableFrom(target))
                throw new ArgumentException($"Given type `{target.FullName}` is not of type `System.Attribute`");

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

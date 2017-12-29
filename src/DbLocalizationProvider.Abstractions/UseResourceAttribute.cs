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

namespace DbLocalizationProvider.Abstractions
{
    /// <summary>
    /// Sometimes you just feel lazy enough to reuse already existing resources and not generating new ones. Well, this attribute does exactlty that.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class UseResourceAttribute : Attribute
    {
        /// <summary>
        /// Creates new instance of this attribute. What else?
        /// </summary>
        /// <param name="targetContainer">References type that contains resource that will be used here.</param>
        /// <param name="propertyName">Name of the property to use for the reference</param>
        public UseResourceAttribute(Type targetContainer, string propertyName)
        {
            TargetContainer = targetContainer ?? throw new ArgumentNullException(nameof(targetContainer));
            PropertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));
        }

        /// <summary>
        /// References type that contains resource that will be used here.
        /// </summary>
        public Type TargetContainer { get; }

        /// <summary>
        /// Name of the property to use for the reference
        /// </summary>
        public string PropertyName { get; }
    }
}

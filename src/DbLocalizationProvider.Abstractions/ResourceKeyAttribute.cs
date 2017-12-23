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

namespace DbLocalizationProvider
{
    /// <summary>
    /// Use this attribute if you want to register multiple resources for the same field or property with different resource keys
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Class, AllowMultiple = true)]
    public class ResourceKeyAttribute : Attribute
    {
        /// <summary>
        /// Creates new instance of the attribute
        /// </summary>
        /// <param name="key">Name of the resource key</param>
        public ResourceKeyAttribute(string key) : this(key, null) { }

        /// <summary>
        /// Creates new instance of the attribute.
        /// </summary>
        /// <param name="key">This is the key of the resource</param>
        /// <param name="value">This is the default translation for the resource</param>
        public ResourceKeyAttribute(string key, string value)
        {
            if(string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            Key = key;
            Value = value;
        }

        /// <summary>
        ///Key for the resource
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Default translation for the resource
        /// </summary>
        public string Value { get; set; }
    }
}

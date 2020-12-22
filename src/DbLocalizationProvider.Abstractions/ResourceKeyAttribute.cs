// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;

namespace DbLocalizationProvider.Abstractions
{
    /// <summary>
    ///     Use this attribute if you want to register multiple resources for the same field or property with different
    ///     resource keys
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Class, AllowMultiple = true)]
    public class ResourceKeyAttribute : Attribute
    {
        /// <summary>
        ///     Creates new instance of the attribute
        /// </summary>
        /// <param name="key">Name of the resource key</param>
        public ResourceKeyAttribute(string key) : this(key, null) { }

        /// <summary>
        ///     Creates new instance of the attribute.
        /// </summary>
        /// <param name="key">This is the key of the resource</param>
        /// <param name="value">This is the default translation for the resource</param>
        public ResourceKeyAttribute(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            Key = key;
            Value = value;
        }

        /// <summary>
        ///     Key for the resource
        /// </summary>
        public string Key { get; }

        /// <summary>
        ///     Default translation for the resource
        /// </summary>
        public string Value { get; set; }
    }
}

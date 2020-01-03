// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider
{
    /// <summary>
    ///     One of the main attributes of the library. Tells that decorated class might contain localized resources.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum)]
    public class LocalizedResourceAttribute : Attribute
    {
        /// <summary>
        ///     You can use this property to override default resource key generation and provide your own prefix for underlying
        ///     porperties.
        /// </summary>
        public string KeyPrefix { get; set; }

        /// <summary>
        ///     Flag to indicate whether you care about your parents. If set to <c>false</c> - properties from parent type will not
        ///     be considered as part of this type.
        /// </summary>
        public bool Inherited { get; set; } = true;

        /// <summary>
        ///     Tells synchronized to take only properties (or fields) decorated *only* with <see cref="IncludeAttribute" />.
        /// </summary>
        public bool OnlyIncluded { get; set; } = false;
    }
}
